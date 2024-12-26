// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Components;
using Doozy.Editor.Reactor.Editors.Animators.Internal;
using Doozy.Editor.Reactor.Ticker;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animators;
using Doozy.Runtime.Reactor.Animators.Internal;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Reactor.Editors
{
    [CustomEditor(typeof(ReactorController), true)]
    [CanEditMultipleObjects]
    public class ReactorControllerEditor : UnityEditor.Editor
    {
        public ReactorController castedTarget => (ReactorController)target;
        public List<ReactorController> castedTargets => targets.Cast<ReactorController>().ToList();

        public static Color accentColor => EditorColors.Reactor.Red;
        public static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Reactor.Red;

        private VisualElement root { get; set; }
        private VisualElement toolbarContainer { get; set; }
        private VisualElement contentContainer { get; set; }

        private FluidComponentHeader componentHeader { get; set; }
        private ReactionControls reactionControls { get; set; }

        protected FluidToggleGroup tabsGroup { get; private set; }
        protected FluidTab settingsTab { get; private set; }
        protected FluidTab behaviourTab { get; private set; }
        protected FluidTab nameTab { get; private set; }

        protected FluidAnimatedContainer settingsAnimatedContainer { get; private set; }
        protected FluidAnimatedContainer behaviourAnimatedContainer { get; private set; }
        protected FluidAnimatedContainer controllerNameAnimatedContainer { get; private set; }

        private SerializedProperty propertyAnimators { get; set; }
        private SerializedProperty propertyControllerMode { get; set; }
        private SerializedProperty propertyControllerName { get; set; }
        private SerializedProperty propertyOnEnableBehaviour { get; set; }
        private SerializedProperty propertyOnStartBehaviour { get; set; }
        private SerializedProperty propertyOverrideAnimatorsBehaviors { get; set; }

        private bool resetToStartValue { get; set; }

        private void OnEnable()
        {
            if (Application.isPlaying) return;
            resetToStartValue = false;
            foreach (ReactorController controller in castedTargets.RemoveNulls())
                foreach (ReactorAnimator animator in controller.animators.RemoveNulls())
                    animator.animatorInitialized = false;
        }

        private void OnDisable()
        {
            if (Application.isPlaying) return;
            if (resetToStartValue) ResetToStartValue();
        }

        private void OnDestroy()
        {
            reactionControls?.Dispose();

            componentHeader?.Recycle();

            tabsGroup?.Recycle();
            settingsTab?.Recycle();
            behaviourTab?.Recycle();
            nameTab?.Recycle();

            settingsAnimatedContainer?.Dispose();
            behaviourAnimatedContainer?.Dispose();
            controllerNameAnimatedContainer?.Dispose();
        }

        private void ResetToStartValue()
        {
            foreach (ReactorController controller in castedTargets.RemoveNulls())
                foreach (ReactorAnimator animator in controller.animators.RemoveNulls())
                {
                    if (animator == null) continue;
                    animator.Stop();
                    animator.ResetToStartValues();
                }
        }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        private void FindProperties()
        {
            propertyAnimators = serializedObject.FindProperty("Animators");
            propertyControllerMode = serializedObject.FindProperty(nameof(ReactorController.ControllerMode));
            propertyControllerName = serializedObject.FindProperty(nameof(ReactorController.ControllerName));
            propertyOnEnableBehaviour = serializedObject.FindProperty(nameof(ReactorController.OnEnableBehaviour));
            propertyOnStartBehaviour = serializedObject.FindProperty(nameof(ReactorController.OnStartBehaviour));
            propertyOverrideAnimatorsBehaviors = serializedObject.FindProperty(nameof(ReactorController.OverrideAnimatorsBehaviors));
        }

        private void InitializeEditor()
        {
            FindProperties();
            root = DesignUtils.GetEditorRoot();

            componentHeader = FluidComponentHeader.Get()
                .SetAccentColor(accentColor)
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(ReactorController)))
                .SetIcon(EditorSpriteSheets.Reactor.Icons.ReactorController)
                .SetElementSize(ElementSize.Large)
                .AddManualButton()
                .AddApiButton()
                .AddYouTubeButton();

            toolbarContainer =
                new VisualElement()
                    .SetName("Toolbar Container")
                    .SetStyleFlexDirection(FlexDirection.Row)
                    .SetStyleMarginTop(-4)
                    .SetStyleMarginLeft(47)
                    .SetStyleMarginRight(4)
                    .SetStyleFlexGrow(1);

            contentContainer =
                new VisualElement()
                    .SetName("Content Container")
                    .SetStyleFlexGrow(1)
                    .SetStyleMarginLeft(43);

            tabsGroup = FluidToggleGroup.Get().SetControlMode(FluidToggleGroup.ControlMode.OneToggleOn);

            InitializeReactionControls();
            InitializeSettings();
            InitializeBehaviour();
            InitializeControllerName();
        }

        private void HeartbeatCheck()
        {
            if (Application.isPlaying) return;
            foreach (ReactorController controller in castedTargets)
                foreach (ReactorAnimator a in controller.animators.RemoveNulls())
                {
                    if (a.animatorInitialized) return;
                    resetToStartValue = true;
                    a.InitializeAnimator();
                    foreach (EditorHeartbeat eh in a.SetHeartbeat<EditorHeartbeat>().Cast<EditorHeartbeat>())
                        eh.StartSceneViewRefresh(a);
                }
        }

        private void InitializeReactionControls()
        {
            reactionControls =
                new ReactionControls()
                    .SetBackgroundColor(EditorColors.Default.BoxBackground)
                    .AddReactionControls
                    (
                        resetCallback: () =>
                        {
                            HeartbeatCheck();
                            ResetToStartValue();
                        },
                        fromCallback: () =>
                        {
                            HeartbeatCheck();
                            foreach (ReactorController controller in castedTargets.RemoveNulls())
                                controller.SetProgressAtZero();
                        },
                        playForwardCallback: () =>
                        {
                            HeartbeatCheck();
                            foreach (ReactorController controller in castedTargets.RemoveNulls())
                                controller.Play(PlayDirection.Forward);
                        },
                        stopCallback: () =>
                        {
                            HeartbeatCheck();
                            foreach (ReactorController controller in castedTargets.RemoveNulls())
                                controller.Stop();
                        },
                        playReverseCallback: () =>
                        {
                            HeartbeatCheck();
                            foreach (ReactorController controller in castedTargets.RemoveNulls())
                                controller.Play(PlayDirection.Reverse);
                        },
                        reverseCallback: () =>
                        {
                            HeartbeatCheck();
                            foreach (ReactorController controller in castedTargets.RemoveNulls())
                                controller.Reverse();
                        },
                        toCallback: () =>
                        {
                            HeartbeatCheck();
                            foreach (ReactorController controller in castedTargets.RemoveNulls())
                                controller.SetProgressAtOne();
                        }
                    );
        }

        private void InitializeSettings()
        {
            settingsAnimatedContainer = new FluidAnimatedContainer("Settings", true).Hide(false);
            settingsTab =
                FluidTab.Get()
                    .SetLabelText("Settings")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Settings)
                    .SetElementSize(ElementSize.Small)
                    .ButtonSetAccentColor(selectableAccentColor)
                    .IndicatorSetEnabledColor(accentColor)
                    .ButtonSetOnValueChanged(evt => settingsAnimatedContainer.Toggle(evt.newValue));

            settingsTab.button.AddToToggleGroup(tabsGroup);

            settingsAnimatedContainer.SetOnShowCallback(() =>
            {
                FluidListView flv =
                    DesignUtils.NewObjectListView
                    (
                        propertyAnimators,
                        // "Reactor Animators",
                        "",
                        // "All the animators controlled by this controller",
                        "",
                        typeof(ReactorAnimator)
                    );

                FluidButton getAnimatorsButton =
                    FluidButton.Get()
                        .SetLabelText("Get Animators")
                        .SetTooltip("Get all Reactor Animators attached to this GameObject")
                        .SetIcon(EditorSpriteSheets.EditorUI.Icons.Search)
                        .SetAccentColor(selectableAccentColor)
                        .SetButtonStyle(ButtonStyle.Contained)
                        .SetElementSize(ElementSize.Small)
                        .SetOnClick(() =>
                        {
                            ReactorAnimator[] animators = castedTarget.GetComponentsInChildren<ReactorAnimator>();
                            Undo.RecordObject(castedTarget, "Get Animators");
                            castedTarget.animators.Clear();
                            castedTarget.animators.AddRange(animators);
                            HeartbeatCheck();
                        });


                EnumField controllerModeEnum = DesignUtils.NewEnumField(propertyControllerMode).SetStyleFlexGrow(1);
                FluidField controllerModeFluidField =
                    FluidField.Get()
                        .SetIcon(EditorSpriteSheets.EditorUI.Icons.More)
                        .AddFieldContent(controllerModeEnum);

                RefreshControllerMode((ReactorController.Mode)propertyControllerMode.enumValueIndex);
                controllerModeEnum.RegisterValueChangedCallback(evt =>
                    RefreshControllerMode((ReactorController.Mode)evt.newValue));

                void RefreshControllerMode(ReactorController.Mode mode)
                {
                    string info = mode switch
                                  {
                                      ReactorController.Mode.Manual    => "Set animators manually without any logic in Awake()",
                                      ReactorController.Mode.Automatic => "Automatically search and populate the controlled animators list in Awake()",
                                      _                                => "Unknown mode"
                                  };


                    controllerModeFluidField?.SetLabelText($"Control Mode - {info}");
                }

                settingsAnimatedContainer
                    .AddContent(controllerModeFluidField)
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent
                    (
                        DesignUtils.row
                            .AddChild(getAnimatorsButton)
                            .AddChild(DesignUtils.flexibleSpace)
                    )
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(flv)
                    .Bind(serializedObject);
            });

            //refresh settingsTab enabled indicator
            root.schedule.Execute(() =>
            {
                void UpdateIndicator(FluidTab tab, bool toggleOn, bool animateChange)
                {
                    if (tab.indicator.isOn != toggleOn)
                        tab.indicator.Toggle(toggleOn, animateChange);
                }

                //initial indicators state update (no animation)
                UpdateIndicator(settingsTab, castedTarget.hasAnimators, false);

                root.schedule.Execute(() =>
                {
                    //subsequent indicators state update (animated)
                    UpdateIndicator(settingsTab, castedTarget.hasAnimators, true);

                }).Every(200);
            });
        }

        private void InitializeBehaviour()
        {
            behaviourAnimatedContainer = new FluidAnimatedContainer("Behaviour", true).Hide(false);
            behaviourTab =
                FluidTab.Get()
                    .SetLabelText("Behaviour")
                    .SetElementSize(ElementSize.Small)
                    .ButtonSetAccentColor(EditorSelectableColors.Default.Action)
                    .IndicatorSetEnabledColor(EditorColors.Default.Action)
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.UIBehaviour)
                    .ButtonSetOnValueChanged(evt => behaviourAnimatedContainer.Toggle(evt.newValue));

            behaviourTab.button.AddToToggleGroup(tabsGroup);

            //refresh behaviourTab enabled indicator
            root.schedule.Execute(() =>
            {
                //initial indicators state update (no animation)
                UpdateBehaviourTabIndicator(false);
                behaviourAnimatedContainer.schedule.Execute(() =>
                {
                    //subsequent indicators state update (animated)
                    UpdateBehaviourTabIndicator(true);
                }).Every(200);

                void UpdateBehaviourTabIndicator(bool animateChange)
                {
                    bool toggleOn =
                        (AnimatorBehaviour)propertyOnStartBehaviour.enumValueIndex != AnimatorBehaviour.Disabled ||
                        (AnimatorBehaviour)propertyOnEnableBehaviour.enumValueIndex != AnimatorBehaviour.Disabled ||
                        propertyOverrideAnimatorsBehaviors.boolValue;
                    if (toggleOn != behaviourTab.indicator.isOn)
                        behaviourTab.indicator.Toggle(toggleOn, animateChange);
                }
            });

            behaviourAnimatedContainer.SetOnShowCallback(() =>
            {
                FluidField GetBehaviourFluidField(SerializedProperty property, string behaviourName)
                {
                    EnumField enumField = DesignUtils.NewEnumField(property.propertyPath).SetStyleFlexGrow(1);
                    FluidField fluidField =
                        FluidField.Get()
                            .SetIcon(EditorSpriteSheets.EditorUI.Icons.UIBehaviour)
                            .SetElementSize(ElementSize.Small).AddFieldContent(enumField);

                    UpdateLabel((AnimatorBehaviour)property.enumValueIndex);
                    enumField.RegisterValueChangedCallback(evt => UpdateLabel((AnimatorBehaviour)evt.newValue));

                    void UpdateLabel(AnimatorBehaviour behaviour) =>
                        fluidField.SetLabelText($"{behaviourName} - {BaseReactorAnimatorEditor.BehaviourInfo(behaviour)}");

                    return fluidField;
                }

                behaviourAnimatedContainer
                    .AddContent
                    (
                        FluidToggleSwitch.Get("Override Animators Behaviours")
                            .SetToggleAccentColor(EditorSelectableColors.Default.Action)
                            .BindToProperty(propertyOverrideAnimatorsBehaviors)
                    )
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(GetBehaviourFluidField(propertyOnStartBehaviour, "OnStart"))
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(GetBehaviourFluidField(propertyOnEnableBehaviour, "OnEnable"))
                    .AddContent(DesignUtils.endOfLineBlock)
                    .Bind(serializedObject);
            });
        }

        private void InitializeControllerName()
        {
            controllerNameAnimatedContainer = new FluidAnimatedContainer("Controller Name", true).Hide(false);
            nameTab =
                FluidTab.Get()
                    .SetLabelText("Controller Name")
                    .SetElementSize(ElementSize.Small)
                    .ButtonSetAccentColor(selectableAccentColor)
                    .ButtonSetOnValueChanged(evt => controllerNameAnimatedContainer.Toggle(evt.newValue));

            nameTab.button.AddToToggleGroup(tabsGroup);

            TextField controllerNameTextField = DesignUtils.NewTextField(propertyControllerName).SetStyleFlexGrow(1);
            controllerNameTextField.RegisterValueChangedCallback(evt =>
                UpdateComponentTypeText(evt.newValue));

            void UpdateComponentTypeText(string nameOfTheAnimator)
            {
                nameOfTheAnimator = nameOfTheAnimator.IsNullOrEmpty() ? string.Empty : $" - {nameOfTheAnimator}";
                componentHeader.SetComponentTypeText(nameOfTheAnimator);
            }

            UpdateComponentTypeText(propertyControllerName.stringValue);

            controllerNameAnimatedContainer.SetOnShowCallback(() =>
            {
                controllerNameAnimatedContainer
                    .AddContent
                    (
                        FluidField.Get()
                            .SetLabelText("Controller Name")
                            .SetTooltip("Name of the Controller")
                            .AddFieldContent(controllerNameTextField)
                            .SetIcon(EditorSpriteSheets.EditorUI.Icons.Label)
                    )
                    .AddContent(DesignUtils.endOfLineBlock)
                    .Bind(serializedObject);
            });
        }

        protected virtual VisualElement Toolbar()
        {
            return
                toolbarContainer
                    .AddChild(settingsTab)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(behaviourTab)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(DesignUtils.flexibleSpace)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(nameTab)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild
                    (
                        DesignUtils.SystemButton_SortComponents
                        (
                            castedTarget.gameObject,
                            nameof(RectTransform),
                            nameof(ReactorController),
                            nameof(ColorAnimator),
                            nameof(FloatAnimator),
                            nameof(IntAnimator),
                            nameof(SpriteAnimator),
                            nameof(UIAnimator),
                            nameof(Vector2Animator),
                            nameof(Vector3Animator)
                        )
                    );
        }

        protected virtual VisualElement Content()
        {
            return
                contentContainer
                    .AddChild(settingsAnimatedContainer)
                    .AddChild(behaviourAnimatedContainer)
                    .AddChild(controllerNameAnimatedContainer);
        }

        private void Compose()
        {
            root
                .AddChild(reactionControls)
                .AddChild(componentHeader)
                .AddChild(Toolbar())
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(Content())
                .AddChild(DesignUtils.endOfLineBlock);
        }
    }
}
