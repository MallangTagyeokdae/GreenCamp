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
using Doozy.Editor.Reactor.Ticker;
using Doozy.Editor.UIElements;
using Doozy.Editor.UIManager.Components;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager;
using Doozy.Runtime.UIManager.Animators;
using Doozy.Runtime.UIManager.Containers;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Containers.Internal
{
    public abstract class BaseUIContainerEditor : UnityEditor.Editor
    {
        protected virtual Color accentColor => EditorColors.UIManager.UIComponent;
        protected virtual EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.UIComponent;

        protected UIContainer castedContainer => (UIContainer)target;
        protected List<UIContainer> castedContainers => targets.Cast<UIContainer>().ToList();

        protected List<BaseUIContainerAnimator> containerAnimators { get; set; }

        protected VisualElement root { get; set; }
        protected ContainerReactionControls reactionControls { get; set; }
        protected FluidComponentHeader componentHeader { get; set; }
        protected VisualElement toolbarContainer { get; private set; }
        protected VisualElement contentContainer { get; private set; }

        protected FluidToggleGroup tabsGroup { get; set; }
        protected FluidTab settingsTab { get; set; }
        protected FluidTab callbacksTab { get; set; }
        protected FluidTab progressorsTab { get; set; }

        protected FluidAnimatedContainer settingsAnimatedContainer { get; set; }
        protected FluidAnimatedContainer callbacksAnimatedContainer { get; set; }
        protected FluidAnimatedContainer progressorsAnimatedContainer { get; set; }

        protected SerializedProperty propertyOnStartBehaviour { get; set; }
        protected SerializedProperty propertyOnShowCallback { get; set; }
        protected SerializedProperty propertyOnVisibleCallback { get; set; }
        protected SerializedProperty propertyOnHideCallback { get; set; }
        protected SerializedProperty propertyOnHiddenCallback { get; set; }
        protected SerializedProperty propertyOnVisibilityChangedCallback { get; set; }
        protected SerializedProperty propertyShowProgressors { get; set; }
        protected SerializedProperty propertyHideProgressors { get; set; }
        protected SerializedProperty propertyShowHideProgressors { get; set; }
        protected SerializedProperty propertyCustomStartPosition { get; set; }
        protected SerializedProperty propertyUseCustomStartPosition { get; set; }
        protected SerializedProperty propertyAutoHideAfterShow { get; set; }
        protected SerializedProperty propertyAutoHideAfterShowDelay { get; set; }
        protected SerializedProperty propertyDisableGameObjectWhenHidden { get; set; }
        protected SerializedProperty propertyDisableCanvasWhenHidden { get; set; }
        protected SerializedProperty propertyDisableGraphicRaycasterWhenHidden { get; set; }
        protected SerializedProperty propertyClearSelectedOnHide { get; set; }
        protected SerializedProperty propertyClearSelectedOnShow { get; set; }
        protected SerializedProperty propertyAutoSelectAfterShow { get; set; }
        protected SerializedProperty propertyAutoSelectTarget { get; set; }

        protected bool resetToStartValue { get; set; }

        protected virtual void OnEnable()
        {
            if (Application.isPlaying) return;
            resetToStartValue = false;
            SearchForAnimators();
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
            settingsTab?.Dispose();
            callbacksTab?.Dispose();
            progressorsTab?.Dispose();

            settingsAnimatedContainer?.Dispose();
            callbacksAnimatedContainer?.Dispose();
            progressorsAnimatedContainer?.Dispose();
        }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        protected virtual void ResetAnimatorInitializedState()
        {
            foreach (UIContainer c in castedContainers)
                foreach (var animator in containerAnimators.RemoveNulls())
                    if (animator.controller == c)
                        animator.animatorInitialized = false;
        }

        protected virtual void ResetToStartValues()
        {
            foreach (UIContainer c in castedContainers)
                foreach (var animator in containerAnimators.RemoveNulls())
                    if (animator.controller == c)
                    {
                        animator.StopAllReactions();
                        animator.ResetToStartValues();
                    }
        }

        protected virtual void PlayShow()
        {
            foreach (UIContainer c in castedContainers)
                foreach (var animator in containerAnimators.RemoveNulls())
                    if (animator.controller == c)
                        animator.Show();
        }

        protected virtual void PlayHide()
        {
            foreach (UIContainer c in castedContainers)
                foreach (var animator in containerAnimators.RemoveNulls())
                    if (animator.controller == c)
                        animator.Hide();
        }

        protected virtual void PlayReverseShow()
        {
            foreach (UIContainer c in castedContainers)
                foreach (var animator in containerAnimators.RemoveNulls())
                    if (animator.controller == c)
                        animator.ReverseShow();
        }

        protected virtual void PlayReverseHide()
        {
            foreach (UIContainer c in castedContainers)
                foreach (var animator in containerAnimators.RemoveNulls())
                    if (animator.controller == c)
                        animator.ReverseHide();
        }

        protected virtual void SearchForAnimators()
        {
            containerAnimators ??= new List<BaseUIContainerAnimator>();
            containerAnimators.Clear();
            if (EditorUtility.IsPersistent(castedContainer))
            {
                containerAnimators.AddRange(castedContainer.GetComponentsInChildren<BaseUIContainerAnimator>());
                return;
            }
            containerAnimators.AddRange(FindObjectsOfType<BaseUIContainerAnimator>());
        }

        protected virtual void HeartbeatCheck()
        {
            if (Application.isPlaying) return;

            foreach (UIContainer c in castedContainers)
                foreach (var a in containerAnimators.RemoveNulls())
                    if (a.controller == c && a.animatorInitialized == false)
                    {
                        resetToStartValue = true;
                        a.InitializeAnimator();
                        foreach (EditorHeartbeat eh in a.SetHeartbeat<EditorHeartbeat>().Cast<EditorHeartbeat>())
                            eh.StartSceneViewRefresh(a);
                    }
        }

        protected virtual void InitializeReactionControls()
        {
            reactionControls
                .SetBackgroundColor(color: EditorColors.Default.BoxBackground)
                .AddContainerControls
                (
                    resetCallback: () =>
                    {
                        HeartbeatCheck();
                        ResetToStartValues();
                    },
                    showCallback: () =>
                    {
                        if (Application.isPlaying)
                        {
                            castedContainers.ForEach(c => c.Show());
                            return;
                        }
                        HeartbeatCheck();
                        PlayShow();
                    },
                    hideCallback: () =>
                    {
                        if (Application.isPlaying)
                        {
                            castedContainers.ForEach(c => c.Hide());
                            return;
                        }
                        HeartbeatCheck();
                        PlayHide();
                    },
                    reverseShowCallback: () =>
                    {
                        if (Application.isPlaying) return;
                        HeartbeatCheck();
                        PlayReverseShow();
                    },
                    reverseHideCallback: () =>
                    {
                        if (Application.isPlaying) return;
                        HeartbeatCheck();
                        PlayReverseHide();
                    },
                    searchForAnimatorsCallback:
                    () =>
                    {
                        if (Application.isPlaying) return;
                        SearchForAnimators();
                    });
        }

        protected virtual void FindProperties()
        {
            propertyAutoHideAfterShow = serializedObject.FindProperty(nameof(UIContainer.AutoHideAfterShow));
            propertyAutoHideAfterShowDelay = serializedObject.FindProperty(nameof(UIContainer.AutoHideAfterShowDelay));
            propertyAutoSelectAfterShow = serializedObject.FindProperty(nameof(UIContainer.AutoSelectAfterShow));
            propertyAutoSelectTarget = serializedObject.FindProperty(nameof(UIContainer.AutoSelectTarget));
            propertyClearSelectedOnHide = serializedObject.FindProperty(nameof(UIContainer.ClearSelectedOnHide));
            propertyClearSelectedOnShow = serializedObject.FindProperty(nameof(UIContainer.ClearSelectedOnShow));
            propertyCustomStartPosition = serializedObject.FindProperty(nameof(UIContainer.CustomStartPosition));
            propertyDisableCanvasWhenHidden = serializedObject.FindProperty(nameof(UIContainer.DisableCanvasWhenHidden));
            propertyDisableGameObjectWhenHidden = serializedObject.FindProperty(nameof(UIContainer.DisableGameObjectWhenHidden));
            propertyDisableGraphicRaycasterWhenHidden = serializedObject.FindProperty(nameof(UIContainer.DisableGraphicRaycasterWhenHidden));
            propertyHideProgressors = serializedObject.FindProperty("HideProgressors");
            propertyOnHiddenCallback = serializedObject.FindProperty(nameof(UIContainer.OnHiddenCallback));
            propertyOnHideCallback = serializedObject.FindProperty(nameof(UIContainer.OnHideCallback));
            propertyOnShowCallback = serializedObject.FindProperty(nameof(UIContainer.OnShowCallback));
            propertyOnStartBehaviour = serializedObject.FindProperty(nameof(UIContainer.OnStartBehaviour));
            propertyOnVisibilityChangedCallback = serializedObject.FindProperty(nameof(UIContainer.OnVisibilityChangedCallback));
            propertyOnVisibleCallback = serializedObject.FindProperty(nameof(UIContainer.OnVisibleCallback));
            propertyShowHideProgressors = serializedObject.FindProperty("ShowHideProgressors");
            propertyShowProgressors = serializedObject.FindProperty("ShowProgressors");
            propertyUseCustomStartPosition = serializedObject.FindProperty(nameof(UIContainer.UseCustomStartPosition));
        }

        protected virtual void InitializeEditor()
        {
            FindProperties();
            root = DesignUtils.GetEditorRoot();
            reactionControls = new ContainerReactionControls();
            componentHeader = DesignUtils.editorComponentHeader.SetAccentColor(accentColor);
            toolbarContainer = DesignUtils.editorToolbarContainer;
            tabsGroup = FluidToggleGroup.Get().SetControlMode(FluidToggleGroup.ControlMode.OneToggleOn);
            contentContainer = DesignUtils.editorContentContainer;
            InitializeReactionControls();
            InitializeSettings();
            InitializeCallbacks();
            InitializeProgressors();

            root.schedule.Execute(() => settingsTab.ButtonSetIsOn(true, false));

            //refresh tabs enabled indicator
            root.schedule.Execute(() =>
            {
                void UpdateIndicator(FluidTab tab, bool toggleOn, bool animateChange)
                {
                    if (tab == null) return;
                    if (tab.indicator.isOn != toggleOn)
                        tab.indicator.Toggle(toggleOn, animateChange);
                }

                //initial indicators state update (no animation)
                UpdateIndicator(callbacksTab, castedContainer.hasCallbacks, false);
                UpdateIndicator(progressorsTab, castedContainer.hasProgressors, false);

                //subsequent indicators state update (animated)
                root.schedule.Execute(() =>
                {
                    UpdateIndicator(callbacksTab, castedContainer.hasCallbacks, true);
                    UpdateIndicator(progressorsTab, castedContainer.hasProgressors, true);

                }).Every(200);

            });
        }

        protected virtual void InitializeSettings()
        {
            settingsAnimatedContainer = new FluidAnimatedContainer("Settings", true).Hide(false);
            settingsTab =
                new FluidTab()
                    .SetLabelText("Settings")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Settings)
                    .SetElementSize(ElementSize.Small)
                    .IndicatorSetEnabledColor(accentColor)
                    .ButtonSetAccentColor(selectableAccentColor)
                    .ButtonSetOnValueChanged(evt => settingsAnimatedContainer.Toggle(evt.newValue, evt.animateChange))
                    .AddToToggleGroup(tabsGroup);

            settingsAnimatedContainer.SetOnShowCallback(() =>
            {
                #region Start Behaviour

                EnumField onStartBehaviourEnumField = DesignUtils.NewEnumField(propertyOnStartBehaviour.propertyPath).SetStyleFlexGrow(1);
                FluidField onStartBehaviourFluidField = FluidField.Get().AddFieldContent(onStartBehaviourEnumField);

                void RefreshStartBehaviourInfo(ContainerBehaviour behaviour)
                {
                    string info = behaviour switch
                                  {
                                      ContainerBehaviour.Disabled    => "Do nothing",
                                      ContainerBehaviour.InstantHide => "Instant Hide (no animation)",
                                      ContainerBehaviour.InstantShow => "Instant Show (no animation)",
                                      ContainerBehaviour.Hide        => "Hide (animated)",
                                      ContainerBehaviour.Show        => "Show (animated)",
                                      _                              => ""
                                  };

                    onStartBehaviourFluidField.SetLabelText($"OnStart - {info}");
                }

                RefreshStartBehaviourInfo((ContainerBehaviour)propertyOnStartBehaviour.enumValueIndex);
                onStartBehaviourEnumField.RegisterValueChangedCallback(evt =>
                {
                    if (evt?.newValue == null) return;
                    RefreshStartBehaviourInfo((ContainerBehaviour)evt.newValue);
                });

                #endregion

                #region Auto Hide after Show

                Label hideDelayLabel =
                    DesignUtils.NewLabel("Auto Hide Delay")
                        .SetStyleMarginRight(DesignUtils.k_Spacing);

                FloatField hideDelayPropertyField =
                    DesignUtils.NewFloatField(propertyAutoHideAfterShowDelay)
                        .SetTooltip("Time interval after which Hide is triggered")
                        .SetStyleWidth(40)
                        .SetStyleMarginRight(DesignUtils.k_Spacing);

                Label secondsLabel =
                    DesignUtils.NewLabel("seconds");

                hideDelayLabel.SetEnabled(propertyAutoHideAfterShow.boolValue);
                hideDelayPropertyField.SetEnabled(propertyAutoHideAfterShow.boolValue);
                secondsLabel.SetEnabled(propertyAutoHideAfterShow.boolValue);

                FluidToggleSwitch autoHideAfterShowSwitch =
                    FluidToggleSwitch.Get()
                        .BindToProperty(propertyAutoHideAfterShow)
                        .SetTooltip("If TRUE, after Show, Hide it will get automatically triggered after the AutoHideAfterShowDelay time interval has passed")
                        .SetOnValueChanged(evt =>
                        {
                            if (evt?.newValue == null) return;
                            hideDelayLabel.SetEnabled(evt.newValue);
                            hideDelayPropertyField.SetEnabled(evt.newValue);
                            secondsLabel.SetEnabled(evt.newValue);
                        })
                        .SetToggleAccentColor(selectableAccentColor)
                        .SetStyleMarginRight(DesignUtils.k_Spacing);

                FluidField autoHideAfterShowFluidField =
                    FluidField.Get("Auto Hide after Show")
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddChild(autoHideAfterShowSwitch)
                                .AddChild(hideDelayLabel)
                                .AddChild(hideDelayPropertyField)
                                .AddChild(secondsLabel)
                                .AddChild(DesignUtils.flexibleSpace)
                        );

                #endregion

                #region Custom Start Position

                PropertyField customStartPositionPropertyField =
                    DesignUtils.NewPropertyField(propertyCustomStartPosition)
                        .TryToHideLabel()
                        .SetTooltip("AnchoredPosition3D to snap to on Awake")
                        .SetStyleAlignSelf(Align.Center);

                FluidToggleSwitch useCustomStartPositionSwitch =
                    FluidToggleSwitch.Get()
                        .BindToProperty(propertyUseCustomStartPosition)
                        .SetToggleAccentColor(selectableAccentColor)
                        .SetTooltip("If TRUE, this view will 'snap' to the custom start position on Awake");

                FluidButton GetButton(IEnumerable<Texture2D> iconTextures, string labelText, string tooltip) =>
                    FluidButton.Get()
                        .SetIcon(iconTextures)
                        .SetLabelText(labelText)
                        .SetTooltip(tooltip)
                        .SetElementSize(ElementSize.Tiny)
                        .SetButtonStyle(ButtonStyle.Contained);

                FluidButton getCustomPositionButton =
                    GetButton
                        (
                            EditorSpriteSheets.EditorUI.Arrows.ArrowDown,
                            "Get",
                            "Set the current RectTransform anchoredPosition3D as the custom start position"
                        )
                        .SetOnClick(() =>
                        {
                            propertyCustomStartPosition.vector3Value = castedContainer.rectTransform.anchoredPosition3D;
                            serializedObject.ApplyModifiedProperties();
                        });

                FluidButton setCustomPositionButton =
                    GetButton
                        (
                            EditorSpriteSheets.EditorUI.Arrows.ArrowUp,
                            "Set",
                            "Snap the RectTransform current anchoredPosition3D to the set custom start position"
                        )
                        .SetOnClick(() =>
                        {
                            if (serializedObject.isEditingMultipleObjects)
                            {
                                // ReSharper disable once CoVariantArrayConversion
                                Undo.RecordObjects(castedContainers.Select(ct => ct.rectTransform).ToArray(), "Set Position");
                                foreach (UIContainer container in castedContainers)
                                    container.rectTransform.anchoredPosition3D = container.CustomStartPosition;
                                return;
                            }

                            Undo.RecordObject(castedContainer.rectTransform, "Set Position");
                            castedContainer.rectTransform.anchoredPosition3D = castedContainer.CustomStartPosition;
                        });

                FluidButton resetCustomPositionButton =
                    GetButton
                        (
                            EditorSpriteSheets.EditorUI.Icons.Reset,
                            "Reset",
                            "Reset the custom start position to (0,0,0)"
                        )
                        .SetOnClick(() =>
                        {
                            propertyCustomStartPosition.vector3Value = Vector3.zero;
                            serializedObject.ApplyModifiedProperties();
                        });

                FluidField customStartPositionFluidField =
                    FluidField.Get("Custom Start Position")
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .SetStyleAlignItems(Align.Center)
                                .AddChild(useCustomStartPositionSwitch)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(customStartPositionPropertyField)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(getCustomPositionButton)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(setCustomPositionButton)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(resetCustomPositionButton)
                        );

                customStartPositionPropertyField.SetEnabled(propertyUseCustomStartPosition.boolValue);
                getCustomPositionButton.SetEnabled(propertyUseCustomStartPosition.boolValue);
                setCustomPositionButton.SetEnabled(propertyUseCustomStartPosition.boolValue);
                resetCustomPositionButton.SetEnabled(propertyUseCustomStartPosition.boolValue);

                useCustomStartPositionSwitch.SetOnValueChanged(callback: evt =>
                {
                    customStartPositionPropertyField.SetEnabled(evt.newValue);
                    getCustomPositionButton.SetEnabled(evt.newValue);
                    setCustomPositionButton.SetEnabled(evt.newValue);
                    resetCustomPositionButton.SetEnabled(evt.newValue);
                });

                #endregion

                #region When Hidden

                FluidToggleSwitch disableGameObjectWhenHiddenSwitch =
                    FluidToggleSwitch.Get("GameObject")
                        .BindToProperty(propertyDisableGameObjectWhenHidden)
                        .SetToggleAccentColor(selectableAccentColor)
                        .SetTooltip("If TRUE, after Hide, the GameObject this component is attached to, will be disabled");

                FluidToggleSwitch disableCanvasWhenHiddenSwitch =
                    FluidToggleSwitch.Get("Canvas")
                        .BindToProperty(propertyDisableCanvasWhenHidden)
                        .SetToggleAccentColor(selectableAccentColor)
                        .SetTooltip("If TRUE, after Hide, the Canvas component found on the same GameObject this component is attached to, will be disabled");

                FluidToggleSwitch disableGraphicRaycasterWhenHiddenSwitch =
                    FluidToggleSwitch.Get("GraphicRaycaster")
                        .BindToProperty(propertyDisableGraphicRaycasterWhenHidden)
                        .SetToggleAccentColor(selectableAccentColor)
                        .SetTooltip("If TRUE, after Hide, the GraphicRaycaster component found on the same GameObject this component is attached to, will be disabled");

                FluidField whenHiddenFluidField =
                    FluidField.Get("When Hidden, disable")
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddChild(disableGameObjectWhenHiddenSwitch)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(disableCanvasWhenHiddenSwitch)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(disableGraphicRaycasterWhenHiddenSwitch)
                                .AddChild(DesignUtils.flexibleSpace)
                        );

                #endregion

                #region Selected

                FluidToggleSwitch clearSelectedOnShowSwitch =
                    FluidToggleSwitch.Get()
                        .BindToProperty(propertyClearSelectedOnShow)
                        .SetLabelText("Show")
                        .SetTooltip("If TRUE, when this container is shown, any GameObject that is selected by the EventSystem.current will get deselected")
                        .SetToggleAccentColor(selectableAccentColor);

                FluidToggleSwitch clearSelectedOnHideSwitch =
                    FluidToggleSwitch.Get()
                        .BindToProperty(propertyClearSelectedOnHide)
                        .SetLabelText("Hide")
                        .SetTooltip("If TRUE, when this container is hidden, any GameObject that is selected by the EventSystem.current will get deselected")
                        .SetToggleAccentColor(selectableAccentColor);

                FluidField clearSelectedFluidField =
                    FluidField.Get()
                        .SetLabelText("Clear Selected on")
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddChild(clearSelectedOnShowSwitch)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(clearSelectedOnHideSwitch)
                        )
                        .SetStyleMinWidth(150);

                FluidToggleSwitch autoSelectAfterShowSwitch =
                    FluidToggleSwitch.Get()
                        .BindToProperty(propertyAutoSelectAfterShow)
                        .SetTooltip("If TRUE, after this container has been shown, the referenced selectable GameObject will get automatically selected by EventSystem.current")
                        .SetToggleAccentColor(selectableAccentColor);

                ObjectField autoSelectTargetObjectField =
                    DesignUtils.NewObjectField(propertyAutoSelectTarget, typeof(GameObject))
                        .SetTooltip("Reference to the GameObject that should be selected after this view has been shown. Works only if AutoSelectAfterShow is TRUE");

                autoSelectTargetObjectField.SetEnabled(propertyAutoSelectAfterShow.boolValue);
                autoSelectAfterShowSwitch.SetOnValueChanged(evt =>
                {
                    if (evt?.newValue == null) return;
                    autoSelectTargetObjectField.SetEnabled(evt.newValue);
                });

                FluidField autoSelectAfterShowFluidField =
                    FluidField.Get()
                        .SetLabelText("Auto select GameObject after Show")
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddChild(autoSelectAfterShowSwitch)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(autoSelectTargetObjectField)
                        );

                #endregion

                settingsAnimatedContainer
                    .AddContent
                    (
                        DesignUtils.row
                            .AddChild(onStartBehaviourFluidField)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(autoHideAfterShowFluidField)
                    )
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(customStartPositionFluidField)
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(whenHiddenFluidField)
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent
                    (
                        DesignUtils.row
                            .AddChild(clearSelectedFluidField)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(autoSelectAfterShowFluidField)
                    )
                    .AddContent(DesignUtils.endOfLineBlock)
                    .Bind(serializedObject);

            });
        }

        protected virtual void InitializeCallbacks()
        {
            callbacksAnimatedContainer = new FluidAnimatedContainer("Callbacks", true).Hide(false);
            callbacksTab =
                new FluidTab()
                    .SetLabelText("Callbacks")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.UnityEvent)
                    .SetElementSize(ElementSize.Small)
                    .IndicatorSetEnabledColor(DesignUtils.callbacksColor)
                    .ButtonSetAccentColor(DesignUtils.callbackSelectableColor)
                    .ButtonSetOnValueChanged(evt => callbacksAnimatedContainer.Toggle(evt.newValue, evt.animateChange))
                    .AddToToggleGroup(tabsGroup);

            callbacksAnimatedContainer.SetOnShowCallback(() =>
            {
                FluidField GetCallbackField(string labelText, SerializedProperty property) =>
                    FluidField.Get()
                        .SetLabelText(labelText)
                        .SetElementSize(ElementSize.Large)
                        .AddFieldContent(DesignUtils.NewPropertyField(property));

                callbacksAnimatedContainer
                    .AddContent(GetCallbackField("Show animation started", propertyOnShowCallback))
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(GetCallbackField("Show animation finished - VISIBLE", propertyOnVisibleCallback))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(GetCallbackField("Hide animation started", propertyOnHideCallback))
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(GetCallbackField("Hide animation finished - HIDDEN", propertyOnHiddenCallback))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent
                    (
                        FluidField.Get()
                            .SetElementSize(ElementSize.Large)
                            .AddFieldContent(DesignUtils.UnityEventField("Visibility changed", propertyOnVisibilityChangedCallback))
                    )
                    .Bind(serializedObject);
            });
        }

        protected virtual void InitializeProgressors()
        {
            progressorsAnimatedContainer = new FluidAnimatedContainer("Progressors", true).Hide(false);
            progressorsTab =
                new FluidTab()
                    .SetLabelText("Progressors")
                    .SetIcon(EditorSpriteSheets.Reactor.Icons.Progressor)
                    .SetElementSize(ElementSize.Small)
                    .IndicatorSetEnabledColor(EditorColors.Reactor.Red)
                    .ButtonSetAccentColor(EditorSelectableColors.Reactor.Red)
                    .ButtonSetOnValueChanged(evt => progressorsAnimatedContainer.Toggle(evt.newValue, evt.animateChange))
                    .AddToToggleGroup(tabsGroup);

            progressorsAnimatedContainer.SetOnShowCallback(() =>
            {
                FluidListView GetProgressorsListView(SerializedProperty arrayProperty, string listTitle, string listDescription) =>
                    DesignUtils.NewObjectListView(arrayProperty, listTitle, listDescription, typeof(Progressor));

                progressorsAnimatedContainer
                    .AddContent
                    (
                        GetProgressorsListView
                        (
                            propertyShowProgressors,
                            "Show Progressors",
                            "Progressors triggered on Show. Plays forward on Show."
                        )
                    )
                    .AddContent(DesignUtils.spaceBlock4X)
                    .AddContent
                    (
                        GetProgressorsListView
                        (
                            propertyHideProgressors,
                            "Hide Progressors",
                            "Progressors triggered on Hide. Plays forward on Hide."
                        )
                    )
                    .AddContent(DesignUtils.spaceBlock4X)
                    .AddContent
                    (
                        GetProgressorsListView
                        (
                            propertyShowHideProgressors,
                            "Show/Hide Progressors",
                            "Progressors triggered on both Show and Hide. Plays forward on Show and in reverse on Hide."
                        )
                    );

                progressorsAnimatedContainer.Bind(serializedObject);
            });
        }

        protected virtual VisualElement Toolbar()
        {
            return
                toolbarContainer
                    .AddChild(settingsTab)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(callbacksTab)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(progressorsTab)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(DesignUtils.flexibleSpace)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild
                    (
                        DesignUtils.SystemButton_SortComponents
                        (
                            castedContainer.gameObject,
                            nameof(RectTransform),
                            nameof(UIContainer)
                        )
                    );
        }

        protected virtual VisualElement Content()
        {
            return
                contentContainer
                    .AddChild(settingsAnimatedContainer)
                    .AddChild(callbacksAnimatedContainer)
                    .AddChild(progressorsAnimatedContainer);
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
