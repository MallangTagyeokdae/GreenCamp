// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Components;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animators.Internal;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Reactor.Editors.Animators.Internal
{
    public abstract class BaseReactorAnimatorEditor : UnityEditor.Editor
    {
        public static Color accentColor => EditorColors.Reactor.Red;
        public static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Reactor.Red;

        public static IEnumerable<Texture2D> reactorIconTextures => EditorSpriteSheets.Reactor.Icons.ReactorIcon;

        protected VisualElement root { get; set; }
        protected ReactionControls reactionControls { get; set; }
        protected FluidComponentHeader componentHeader { get; set; }
        protected VisualElement toolbarContainer { get; private set; }
        protected VisualElement contentContainer { get; private set; }

        protected FluidToggleGroup tabsGroup { get; private set; }
        protected FluidTab animationTab { get; private set; }
        protected FluidTab behaviourTab { get; private set; }
        protected FluidTab nameTab { get; private set; }

        protected FluidAnimatedContainer animationAnimatedContainer { get; private set; }
        protected FluidAnimatedContainer behaviourAnimatedContainer { get; private set; }
        protected FluidAnimatedContainer animatorNameAnimatedContainer { get; private set; }

        protected SerializedProperty propertyAnimatorName { get; set; }
        protected SerializedProperty propertyAnimation { get; set; }
        protected SerializedProperty propertyOnStartBehaviour { get; set; }
        protected SerializedProperty propertyOnEnableBehaviour { get; set; }

        protected bool resetToStartValue { get; set; }

        protected virtual void OnEnable()
        {
            if (Application.isPlaying) return;
            resetToStartValue = false;
            ResetAnimatorInitializedState();
        }

        protected virtual void OnDisable()
        {
            if (Application.isPlaying) return;
            if (resetToStartValue) ResetToStartValues();
        }

        protected virtual void OnDestroy()
        {
            reactionControls?.Dispose();

            componentHeader?.Recycle();

            tabsGroup?.Recycle();
            animationTab?.Recycle();
            behaviourTab?.Recycle();
            nameTab?.Recycle();

            animationAnimatedContainer?.Dispose();
            behaviourAnimatedContainer?.Dispose();
            animatorNameAnimatedContainer?.Dispose();
        }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        protected abstract void ResetAnimatorInitializedState();
        protected abstract void ResetToStartValues();
        protected abstract void SetProgressAtZero();
        protected abstract void PlayForward();
        protected abstract void Stop();
        protected abstract void PlayReverse();
        protected abstract void Reverse();
        protected abstract void SetProgressAtOne();
        protected abstract void HeartbeatCheck();
        
        protected virtual void InitializeReactionControls()
        {
            reactionControls
                .AddReactionControls
                (
                    resetCallback: () =>
                    {
                        HeartbeatCheck();
                        ResetToStartValues();
                    },
                    fromCallback: () =>
                    {
                        HeartbeatCheck();
                        SetProgressAtZero();
                    },
                    playForwardCallback: () =>
                    {
                        HeartbeatCheck();
                        PlayForward();
                    },
                    stopCallback: () =>
                    {
                        HeartbeatCheck();
                        Stop();
                    },
                    playReverseCallback: () =>
                    {
                        HeartbeatCheck();
                        PlayReverse();
                    },
                    reverseCallback: () =>
                    {
                        HeartbeatCheck();
                        Reverse();
                    },
                    toCallback: () =>
                    {
                        HeartbeatCheck();
                        SetProgressAtOne();
                    }
                );
        }
        
        protected virtual void FindProperties()
        {
            propertyAnimatorName = serializedObject.FindProperty("AnimatorName");
            propertyAnimation = serializedObject.FindProperty("Animation");
            propertyOnStartBehaviour = serializedObject.FindProperty(nameof(ReactorAnimator.OnStartBehaviour));
            propertyOnEnableBehaviour = serializedObject.FindProperty(nameof(ReactorAnimator.OnEnableBehaviour));
        }

        protected virtual void InitializeEditor()
        {
            FindProperties();
            root = DesignUtils.GetEditorRoot();
            reactionControls = new ReactionControls();
            componentHeader =
                DesignUtils.editorComponentHeader
                    .SetComponentNameText("Reactor")
                    .SetComponentTypeText("Animator")
                    .SetIcon(reactorIconTextures.ToList())
                    .SetAccentColor(accentColor);
            toolbarContainer = DesignUtils.editorToolbarContainer;
            tabsGroup = FluidToggleGroup.Get().SetControlMode(FluidToggleGroup.ControlMode.OneToggleOn);
            contentContainer = DesignUtils.editorContentContainer;
            InitializeReactionControls();
            InitializeAnimation();
            InitializeBehaviour();
            InitializeAnimatorName();
        }

        protected virtual void InitializeAnimation()
        {
            animationAnimatedContainer = new FluidAnimatedContainer("Animation", true).Hide(false);
            animationTab =
                FluidTab.Get()
                    .SetLabelText("Animation")
                    .SetElementSize(ElementSize.Small)
                    .IndicatorSetEnabledColor(accentColor)
                    .ButtonSetAccentColor(selectableAccentColor)
                    .ButtonSetOnValueChanged(evt => animationAnimatedContainer.Toggle(evt.newValue))
                    .AddToToggleGroup(tabsGroup);

            animationAnimatedContainer.SetOnShowCallback(() =>
            {
                animationAnimatedContainer
                    .AddContent(DesignUtils.NewPropertyField(propertyAnimation))
                    .Bind(serializedObject);
            });
        }

        protected virtual void InitializeBehaviour()
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

            root.schedule.Execute(() =>
            {
                UpdateBehaviourTabIndicator(false);
                behaviourAnimatedContainer.schedule.Execute(() => UpdateBehaviourTabIndicator(true)).Every(200);

                void UpdateBehaviourTabIndicator(bool animateChange)
                {
                    bool toggleOn =
                        (AnimatorBehaviour)propertyOnStartBehaviour.enumValueIndex != AnimatorBehaviour.Disabled ||
                        (AnimatorBehaviour)propertyOnEnableBehaviour.enumValueIndex != AnimatorBehaviour.Disabled;
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
                        fluidField.SetLabelText($"{behaviourName} - {BehaviourInfo(behaviour)}");

                    return fluidField;
                }

                behaviourAnimatedContainer
                    .AddContent(GetBehaviourFluidField(propertyOnStartBehaviour, "OnStart"))
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(GetBehaviourFluidField(propertyOnEnableBehaviour, "OnEnable"))
                    .AddContent(DesignUtils.endOfLineBlock)
                    .Bind(serializedObject);
            });
        }

        public static string BehaviourInfo(AnimatorBehaviour value)
        {
            switch (value)
            {
                case AnimatorBehaviour.Disabled: return "Do nothing";
                case AnimatorBehaviour.PlayForward: return "Play the animation forward (from 0 to 1)";
                case AnimatorBehaviour.PlayReverse: return "Play the animation in reverse (from 1 to 0)";
                case AnimatorBehaviour.SetFromValue: return "Set the animation at 'from' value (at the start value of the animation)";
                case AnimatorBehaviour.SetToValue: return "Set the animation at 'to' value (at the end value of the animation) ";
                default: throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }

        protected virtual void InitializeAnimatorName()
        {
            animatorNameAnimatedContainer = new FluidAnimatedContainer("Animator Name", true).Hide(false);
            nameTab =
                FluidTab.Get()
                    .SetLabelText("Animator Name")
                    .SetElementSize(ElementSize.Small)
                    .ButtonSetAccentColor(selectableAccentColor)
                    .ButtonSetOnValueChanged(evt => animatorNameAnimatedContainer.Toggle(evt.newValue));
            nameTab.button.AddToToggleGroup(tabsGroup);

            animatorNameAnimatedContainer.SetOnShowCallback(() =>
            {
                TextField animatorNameTextField = 
                    DesignUtils.NewTextField(propertyAnimatorName).SetStyleFlexGrow(1);
                
                animatorNameTextField.RegisterValueChangedCallback(evt => UpdateComponentTypeText(evt.newValue));

                void UpdateComponentTypeText(string nameOfTheAnimator)
                {
                    nameOfTheAnimator = nameOfTheAnimator.IsNullOrEmpty() ? string.Empty : $" - {nameOfTheAnimator}";
                    componentHeader.SetComponentTypeText(nameOfTheAnimator);
                }

                UpdateComponentTypeText(propertyAnimatorName.stringValue);
                
                animatorNameAnimatedContainer
                    .AddContent
                    (
                        FluidField.Get()
                            .SetLabelText("Animator Name")
                            .SetTooltip("Name of the Animator")
                            .AddFieldContent(animatorNameTextField)
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
                    .AddChild(animationTab)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(behaviourTab)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(DesignUtils.flexibleSpace)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(nameTab);
        }

        protected virtual VisualElement Content()
        {
            return
                contentContainer
                    .AddChild(animationAnimatedContainer)
                    .AddChild(behaviourAnimatedContainer)
                    .AddChild(animatorNameAnimatedContainer);
        }

        protected virtual void Compose()
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
