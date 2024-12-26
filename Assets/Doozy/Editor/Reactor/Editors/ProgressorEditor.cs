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
using Doozy.Editor.Reactor.Ticker;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Reactor.Editors
{
    [CustomEditor(typeof(Progressor), true)]
    [CanEditMultipleObjects]
    public class ProgressorEditor : UnityEditor.Editor
    {
        public Progressor castedTarget => (Progressor)target;
        public List<Progressor> castedTargets => targets.Cast<Progressor>().ToList();

        public static Color accentColor => EditorColors.Reactor.Red;
        public static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Reactor.Red;

        private VisualElement root { get; set; }
        private ReactionControls reactionControls { get; set; }
        private FluidComponentHeader componentHeader { get; set; }
        private VisualElement toolbarContainer { get; set; }
        private VisualElement contentContainer { get; set; }

        private FluidToggleGroup tabsGroup { get; set; }
        private FluidTab settingsTab { get; set; }
        private FluidTab callbacksTab { get; set; }
        private FluidTab progressTargetsTab { get; set; }
        private FluidTab progressorsTab { get; set; }

        private FluidAnimatedContainer settingsAnimatedContainer { get; set; }
        private FluidAnimatedContainer callbacksAnimatedContainer { get; set; }
        private FluidAnimatedContainer progressTargetsAnimatedContainer { get; set; }
        private FluidAnimatedContainer progressorsAnimatedContainer { get; set; }

        private FluidField idField { get; set; }

        private SerializedProperty propertyId { get; set; }
        private SerializedProperty propertyProgressTargets { get; set; }
        private SerializedProperty propertyProgressorTargets { get; set; }
        private SerializedProperty propertyFromValue { get; set; }
        private SerializedProperty propertyToValue { get; set; }
        private SerializedProperty propertyCustomResetValue { get; set; }
        private SerializedProperty propertyReaction { get; set; }
        private SerializedProperty propertyResetValueOnEnable { get; set; }
        private SerializedProperty propertyResetValueOnDisable { get; set; }
        private SerializedProperty propertyOnValueChanged { get; set; }
        private SerializedProperty propertyOnProgressChanged { get; set; }
        private SerializedProperty propertyOnValueIncremented { get; set; }
        private SerializedProperty propertyOnValueDecremented { get; set; }
        private SerializedProperty propertyOnValueReset { get; set; }
        private SerializedProperty propertyOnValueReachedFromValue { get; set; }
        private SerializedProperty propertyOnValueReachedToValue { get; set; }

        protected bool resetToStartValue { get; set; }

        private void OnEnable()
        {
            if (Application.isPlaying) return;
            resetToStartValue = false;
            ResetAnimatorInitializedState();
        }

        private void OnDisable()
        {
            if (Application.isPlaying) return;
            if (resetToStartValue) ResetToStartValues();
        }

        private void OnDestroy()
        {
            componentHeader?.Recycle();

            reactionControls?.Dispose();

            tabsGroup?.Recycle();
            settingsTab?.Recycle();
            callbacksTab?.Recycle();
            progressTargetsTab?.Recycle();
            progressorsTab?.Recycle();

            settingsAnimatedContainer?.Dispose();
            callbacksAnimatedContainer?.Dispose();
            progressTargetsAnimatedContainer?.Dispose();
            progressorsAnimatedContainer?.Dispose();

            idField?.Recycle();
        }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        private void ResetAnimatorInitializedState()
        {
            foreach (var p in castedTargets)
                p.initialized = false;    
        }

        private void ResetToStartValues()
        {
            foreach (var p in castedTargets)
            {
                p.Stop();
                p.ResetToStartValues();
            }    
        }
        
        private void SetProgressAtZero() =>
            castedTargets.ForEach(p => p.SetProgressAtZero());
        
        private void PlayForward() =>
            castedTargets.ForEach(p => p.Play(PlayDirection.Forward));
        
        private void Stop() =>
            castedTargets.ForEach(p => p.Stop());
        
        private void PlayReverse() =>
            castedTargets.ForEach(p => p.Play(PlayDirection.Reverse));
       
        private void Reverse() =>
            castedTargets.ForEach(p => p.Reverse());
        
        private void SetProgressAtOne() =>
            castedTargets.ForEach(p => p.SetProgressAtOne());

        private void HeartbeatCheck()
        {
            if (Application.isPlaying) return;
            foreach (var p in castedTargets)
            {
                EditorUtility.SetDirty(p);
                SceneView.RepaintAll();
                if (p.initialized & p.reaction.heartbeat.GetType() == typeof(EditorHeartbeat)) continue;
                resetToStartValue = true;
                p.Initialize();
                foreach (EditorHeartbeat eh in p.SetHeartbeat<EditorHeartbeat>().Cast<EditorHeartbeat>())
                    eh.StartSceneViewRefresh(p);
            }
        }

        private void InitializeReactionControls()
        {
            reactionControls = 
                new ReactionControls()
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
        
        private void FindProperties()
        {
            propertyId = serializedObject.FindProperty(nameof(Progressor.Id));
            propertyProgressTargets = serializedObject.FindProperty("ProgressTargets");
            propertyProgressorTargets = serializedObject.FindProperty("ProgressorTargets");
            propertyFromValue = serializedObject.FindProperty("FromValue");
            propertyToValue = serializedObject.FindProperty("ToValue");
            propertyCustomResetValue = serializedObject.FindProperty("CustomResetValue");
            propertyReaction = serializedObject.FindProperty("Reaction");
            propertyResetValueOnEnable = serializedObject.FindProperty("ResetValueOnEnable");
            propertyResetValueOnDisable = serializedObject.FindProperty("ResetValueOnDisable");
            propertyOnValueChanged = serializedObject.FindProperty("OnValueChanged");
            propertyOnProgressChanged = serializedObject.FindProperty("OnProgressChanged");
            propertyOnValueIncremented = serializedObject.FindProperty("OnValueIncremented");
            propertyOnValueDecremented = serializedObject.FindProperty("OnValueDecremented");
            propertyOnValueReset = serializedObject.FindProperty("OnValueReset");
            propertyOnValueReachedFromValue = serializedObject.FindProperty("OnValueReachedFromValue");
            propertyOnValueReachedToValue = serializedObject.FindProperty("OnValueReachedToValue");
        }

        private void InitializeEditor()
        {
            FindProperties();
            root = DesignUtils.GetEditorRoot();
            componentHeader =
                DesignUtils.editorComponentHeader
                    .SetAccentColor(accentColor)
                    .SetComponentNameText(nameof(Progressor))
                    .SetIcon(EditorSpriteSheets.Reactor.Icons.Progressor)
                    .AddManualButton()
                    .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.Reactor.Progressor.html")
                    .AddYouTubeButton();
            toolbarContainer = DesignUtils.editorToolbarContainer;
            tabsGroup = FluidToggleGroup.Get().SetControlMode(FluidToggleGroup.ControlMode.OneToggleOn);
            contentContainer = DesignUtils.editorContentContainer;

            idField =
                FluidField.Get()
                    .AddFieldContent(DesignUtils.NewPropertyField(propertyId));

            InitializeReactionControls();
            InitializeSettings();
            InitializeCallbacks();
            InitializeProgressTargets();
            InitializeProgressorTargets();

            // root.schedule.Execute(() => settingsTab.ButtonSetIsOn(true, false));

            //refresh tabs enabled indicator
            root.schedule.Execute(() =>
            {
                void UpdateIndicator(FluidTab tab, bool toggleOn, bool animateChange)
                {
                    if (tab == null) return;
                    if (tab.indicator.isOn != toggleOn)
                        tab.indicator.Toggle(toggleOn, animateChange);
                }

                bool HasCallbacks() =>
                    castedTarget != null &&
                    castedTarget.OnValueChanged.GetPersistentEventCount() > 0 |
                    castedTarget.OnProgressChanged.GetPersistentEventCount() > 0 |
                    castedTarget.OnValueIncremented.GetPersistentEventCount() > 0 |
                    castedTarget.OnValueDecremented.GetPersistentEventCount() > 0 |
                    castedTarget.OnValueReset.hasCallbacks |
                    castedTarget.OnValueReachedFromValue.hasCallbacks |
                    castedTarget.OnValueReachedToValue.hasCallbacks;

                bool HasProgressTargets() =>
                    castedTarget != null &&
                    castedTarget.progressTargets?.Count > 0;

                bool HasProgressors() =>
                    castedTarget != null &&
                    castedTarget.progressorTargets?.Count > 0;

                //initial indicators state update (no animation)
                UpdateIndicator(callbacksTab, HasCallbacks(), false);
                UpdateIndicator(progressTargetsTab, HasProgressTargets(), false);
                UpdateIndicator(progressorsTab, HasProgressors(), false);

                //subsequent indicators state update (animated)
                root.schedule.Execute(() =>
                {
                    UpdateIndicator(callbacksTab, HasCallbacks(), true);
                    UpdateIndicator(progressTargetsTab, HasProgressTargets(), true);
                    UpdateIndicator(progressorsTab, HasProgressors(), true);

                }).Every(200);
            });
        }

        private void InitializeSettings()
        {
            settingsAnimatedContainer = new FluidAnimatedContainer("Settings", true).Hide(false);
            settingsTab =
                FluidTab.Get()
                    .SetLabelText("Settings")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Settings)
                    .IndicatorSetEnabledColor(accentColor)
                    .ButtonSetAccentColor(selectableAccentColor)
                    .ButtonSetOnValueChanged(evt => settingsAnimatedContainer.Toggle(evt.newValue, evt.animateChange))
                    .AddToToggleGroup(tabsGroup);

            settingsAnimatedContainer.AddOnShowCallback(() =>
            {
                var fromValueFluidField = FluidField.Get<FloatField>("FromValue", "From Value");
                var toValueFluidField = FluidField.Get<FloatField>("ToValue", "To Value");
                EnumField resetValueOnEnableEnumField = DesignUtils.NewEnumField(propertyResetValueOnEnable).SetStyleWidth(120);
                FluidField resetValueOnEnableFluidField = FluidField.Get("OnEnable reset value to").AddFieldContent(resetValueOnEnableEnumField).SetStyleFlexGrow(0);
                var customResetValueFluidField = FluidField.Get<FloatField>("CustomResetValue", "Custom Reset Value");
                var reactionFluidField = FluidField.Get<PropertyField>("Reaction.Settings");
                resetValueOnEnableEnumField.RegisterValueChangedCallback(evt =>
                {
                    if (evt?.newValue == null) return;
                    customResetValueFluidField.SetEnabled((ResetValue)evt.newValue == ResetValue.CustomValue);
                });
                root.schedule.Execute(() =>
                {
                    if (customResetValueFluidField == null) return;
                    if (resetValueOnEnableEnumField?.value == null) return;
                    customResetValueFluidField.SetEnabled((ResetValue)resetValueOnEnableEnumField.value == ResetValue.CustomValue);
                });

                settingsAnimatedContainer
                    .AddContent
                    (
                        DesignUtils.row
                            .AddChild(fromValueFluidField)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(toValueFluidField)
                    )
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(reactionFluidField)
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent
                    (
                        DesignUtils.row
                            .AddChild(resetValueOnEnableFluidField)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(customResetValueFluidField)
                    )
                    .Bind(serializedObject);
            });
        }

        private void InitializeCallbacks()
        {
            callbacksAnimatedContainer = new FluidAnimatedContainer("Callbacks", true).Hide(false);
            callbacksTab =
                FluidTab.Get()
                    .SetLabelText("Callbacks")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.UnityEvent)
                    .IndicatorSetEnabledColor(DesignUtils.callbacksColor)
                    .ButtonSetAccentColor(DesignUtils.callbackSelectableColor)
                    .ButtonSetOnValueChanged(evt => callbacksAnimatedContainer.Toggle(evt.newValue, evt.animateChange))
                    .AddToToggleGroup(tabsGroup);

            callbacksAnimatedContainer.SetOnShowCallback(() =>
            {
                callbacksAnimatedContainer
                    .AddContent(FluidField.Get().AddFieldContent(DesignUtils.UnityEventField("Value Changed - [From, To]", propertyOnValueChanged)))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(FluidField.Get().AddFieldContent(DesignUtils.UnityEventField("Progress Changed - [0, 1]", propertyOnProgressChanged)))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(FluidField.Get().AddFieldContent(DesignUtils.UnityEventField("On Value Incremented", propertyOnValueIncremented)))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(FluidField.Get().AddFieldContent(DesignUtils.UnityEventField("On Value Decremented", propertyOnValueDecremented)))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(FluidField.Get().AddFieldContent(DesignUtils.NewPropertyField(propertyOnValueReset)))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(FluidField.Get().AddFieldContent(DesignUtils.NewPropertyField(propertyOnValueReachedFromValue)))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(FluidField.Get().AddFieldContent(DesignUtils.NewPropertyField(propertyOnValueReachedToValue)))
                    .Bind(serializedObject);
            });
        }

        private void InitializeProgressTargets()
        {
            progressTargetsAnimatedContainer = new FluidAnimatedContainer("Targets", true).Hide(false);
            progressTargetsTab =
                FluidTab.Get()
                    .SetLabelText("Targets")
                    .SetIcon(EditorSpriteSheets.Reactor.Icons.ProgressTarget)
                    .IndicatorSetEnabledColor(EditorColors.Reactor.Red)
                    .ButtonSetAccentColor(EditorSelectableColors.Reactor.Red)
                    .ButtonSetOnValueChanged(evt => progressTargetsAnimatedContainer.Toggle(evt.newValue, evt.animateChange))
                    .AddToToggleGroup(tabsGroup);

            progressTargetsAnimatedContainer.SetOnShowCallback(() =>
            {
                progressTargetsAnimatedContainer
                    .AddContent
                    (
                        DesignUtils.NewObjectListView
                        (
                            propertyProgressTargets,
                            "Progress Targets",
                            "Progress targets that get updated by this Progressor when activated",
                            typeof(ProgressTarget)
                        )
                    )
                    .Bind(serializedObject);
            });
        }

        private void InitializeProgressorTargets()
        {
            progressorsAnimatedContainer = new FluidAnimatedContainer("Progressors", true).Hide(false);
            progressorsTab =
                FluidTab.Get()
                    .SetLabelText("Progressors")
                    .SetIcon(EditorSpriteSheets.Reactor.Icons.Progressor)
                    .IndicatorSetEnabledColor(EditorColors.Reactor.Red)
                    .ButtonSetAccentColor(EditorSelectableColors.Reactor.Red)
                    .ButtonSetOnValueChanged(evt => progressorsAnimatedContainer.Toggle(evt.newValue, evt.animateChange))
                    .AddToToggleGroup(tabsGroup);

            progressorsAnimatedContainer.SetOnShowCallback(() =>
            {
                progressorsAnimatedContainer
                    .AddContent
                    (
                        DesignUtils.NewObjectListView
                        (
                            propertyProgressorTargets,
                            "Progressor Targets",
                            "Other Progressors that get updated by this Progressor when activated",
                            typeof(Progressor)
                        )
                    )
                    .Bind(serializedObject);
            });
        }

        private VisualElement Toolbar()
        {
            return
                toolbarContainer
                    .AddChild(settingsTab)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(callbacksTab)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(progressTargetsTab)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(progressorsTab)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(DesignUtils.flexibleSpace);
        }

        private VisualElement Content()
        {
            return
                contentContainer
                    .AddChild(settingsAnimatedContainer)
                    .AddChild(callbacksAnimatedContainer)
                    .AddChild(progressTargetsAnimatedContainer)
                    .AddChild(progressorsAnimatedContainer);
        }

        private void Compose()
        {
            root
                .AddChild(reactionControls)
                .AddChild(componentHeader)
                .AddChild(Toolbar())
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(Content())
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(idField)
                .AddChild(DesignUtils.endOfLineBlock);
        }
    }
}
