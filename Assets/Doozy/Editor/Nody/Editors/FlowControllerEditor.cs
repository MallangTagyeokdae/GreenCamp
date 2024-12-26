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
using Doozy.Runtime.Nody;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Input;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable UnusedMember.Local

namespace Doozy.Editor.Nody.Editors
{
    [CustomEditor(typeof(FlowController), true)]
    public class FlowControllerEditor : UnityEditor.Editor
    {
        private static Color accentColor => EditorColors.Nody.Color;
        private static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Nody.Color;

        private FlowController castedTarget => (FlowController)target;
        private IEnumerable<FlowController> castedTargets => targets.Cast<FlowController>();

        private bool hasOnStartCallback => castedTarget.onStart?.GetPersistentEventCount() > 0;
        private bool hasOnStopCallback => castedTarget.onStop?.GetPersistentEventCount() > 0;
        private bool hasCallbacks => hasOnStartCallback | hasOnStopCallback;

        private VisualElement root { get; set; }
        private FluidComponentHeader componentHeader { get; set; }
        private VisualElement toolbarContainer { get; set; }
        private VisualElement contentContainer { get; set; }

        private FluidToggleGroup tabsGroup { get; set; }
        private FluidTab settingsTab { get; set; }
        private FluidTab callbacksTab { get; set; }

        private FluidAnimatedContainer callbacksAnimatedContainer { get; set; }
        private FluidAnimatedContainer settingsAnimatedContainer { get; set; }

        private FluidField flowField { get; set; }
        private FluidField flowTypeFluidField { get; set; }
        private FluidToggleSwitch dontDestroyOnSceneChangeSwitch { get; set; }
        private FluidField multiplayerInfoField { get; set; }

        private FluidButton openNodyButton { get; set; }

        private SerializedProperty propertyFlow { get; set; }
        private SerializedProperty propertyFlowType { get; set; }
        private SerializedProperty propertyOnStart { get; set; }
        private SerializedProperty propertyOnStop { get; set; }
        private SerializedProperty propertyDontDestroyOnSceneChange { get; set; }
        private SerializedProperty propertyMultiplayerInfo { get; set; }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        private void OnDestroy()
        {
            componentHeader?.Recycle();

            tabsGroup?.Recycle();
            settingsTab?.Dispose();
            callbacksTab?.Dispose();

            callbacksAnimatedContainer?.Dispose();
            settingsAnimatedContainer?.Dispose();

            flowField?.Recycle();
            flowTypeFluidField?.Recycle();
            dontDestroyOnSceneChangeSwitch?.Recycle();
            multiplayerInfoField?.Recycle();

            openNodyButton?.Recycle();
        }

        private void FindProperties()
        {
            propertyFlow = serializedObject.FindProperty("Flow");
            propertyFlowType = serializedObject.FindProperty("FlowType");
            propertyOnStart = serializedObject.FindProperty("OnStart");
            propertyOnStop = serializedObject.FindProperty("OnStop");
            propertyDontDestroyOnSceneChange = serializedObject.FindProperty("DontDestroyOnSceneChange");
            propertyMultiplayerInfo = serializedObject.FindProperty("MultiplayerInfo");
        }

        private void InitializeEditor()
        {
            FindProperties();
            root = DesignUtils.GetEditorRoot();
            componentHeader =
                DesignUtils.editorComponentHeader
                    .SetAccentColor(accentColor)
                    .SetComponentNameText((ObjectNames.NicifyVariableName(nameof(FlowController))))
                    .SetIcon(EditorSpriteSheets.Nody.Icons.GraphController)
                    .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048477732/Flow+Controller?atlOrigin=eyJpIjoiMzY3OGYxY2U4YTQ0NDI1Njk4MjVjNmVkMmI5ODAxZGEiLCJwIjoiYyJ9")
                    .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.Nody.FlowController.html")
                    .AddYouTubeButton();
            toolbarContainer = DesignUtils.editorToolbarContainer;
            tabsGroup = FluidToggleGroup.Get().SetControlMode(FluidToggleGroup.ControlMode.OneToggleOn);
            contentContainer = DesignUtils.editorContentContainer;

            InitializeSettings();
            InitializeCallbacks();

            root.schedule.Execute(() => settingsTab.ButtonSetIsOn(true, false));
        }

        private void InitializeSettings()
        {
            settingsAnimatedContainer = new FluidAnimatedContainer("Settings", true).Hide(false);
            settingsTab =
                new FluidTab()
                    .SetLabelText("Settings")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Settings)
                    .IndicatorSetEnabledColor(accentColor)
                    .ButtonSetAccentColor(selectableAccentColor)
                    .ButtonSetOnValueChanged(evt => settingsAnimatedContainer.Toggle(evt.newValue, evt.animateChange))
                    .AddToToggleGroup(tabsGroup);

            settingsAnimatedContainer.AddOnShowCallback(() =>
            {
                dontDestroyOnSceneChangeSwitch =
                    FluidToggleSwitch.Get()
                        .SetStyleMarginLeft(40)
                        .SetLabelText("Don't destroy controller on scene change")
                        .BindToProperty(propertyDontDestroyOnSceneChange);

                dontDestroyOnSceneChangeSwitch.SetEnabled(castedTarget.transform.parent == null);
                if (castedTarget.transform.parent != null && propertyDontDestroyOnSceneChange.boolValue)
                {
                    propertyDontDestroyOnSceneChange.boolValue = false;
                    serializedObject.ApplyModifiedPropertiesWithoutUndo();
                }

                openNodyButton =
                    FluidButton.Get()
                        .SetIcon(EditorSpriteSheets.Nody.Icons.Nody)
                        .SetLabelText("Nody")
                        .SetTooltip("Open referenced graph in Nody")
                        .SetStyleFlexShrink(0)
                        .SetAccentColor(EditorSelectableColors.Nody.Color)
                        .SetButtonStyle(ButtonStyle.Contained)
                        .SetElementSize(ElementSize.Tiny)
                        .SetOnClick(() =>
                        {
                            NodyWindow.Open();
                            NodyWindow.OpenGraph(castedTarget.flow);
                        });

                flowField =
                    FluidField.Get()
                        .SetLabelText("Flow Graph")
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .SetStyleFlexGrow(0)
                                .SetStyleFlexShrink(1)
                                .AddChild(DesignUtils.NewObjectField(propertyFlow, typeof(FlowGraph)).SetStyleFlexGrow(1))
                                .AddChild(DesignUtils.spaceBlock2X)
                                .AddChild(openNodyButton)
                        );

                flowTypeFluidField =
                    FluidField.Get()
                        .SetStyleFlexGrow(0)
                        .SetStyleFlexShrink(0)
                        .SetLabelText("Flow Type")
                        .AddFieldContent(DesignUtils.NewEnumField(propertyFlowType).SetStyleWidth(60).SetStyleFlexShrink(0));

                multiplayerInfoField =
                    FluidField.Get()
                        .SetLabelText("Player Index")
                        .AddFieldContent(DesignUtils.NewObjectField(propertyMultiplayerInfo, typeof(MultiplayerInfo)).SetStyleFlexGrow(1))
                        .SetStyleMarginTop(DesignUtils.k_Spacing2X)
                        .SetStyleDisplay(UIManagerInputSettings.instance.multiplayerMode ? DisplayStyle.Flex : DisplayStyle.None);

                settingsAnimatedContainer
                    .AddContent
                    (
                        DesignUtils.row
                            .AddChild(flowField)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(flowTypeFluidField)
                    )
                    .AddChild(DesignUtils.spaceBlock)
                    .AddContent(multiplayerInfoField)
                    .Bind(serializedObject);
            });
        }

        private void InitializeCallbacks()
        {
            callbacksAnimatedContainer = new FluidAnimatedContainer("Callbacks", true).Hide(false);
            callbacksTab =
                new FluidTab()
                    .SetLabelText("Callbacks")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.UnityEvent)
                    .IndicatorSetEnabledColor(DesignUtils.callbacksColor)
                    .ButtonSetAccentColor(DesignUtils.callbackSelectableColor)
                    .ButtonSetOnValueChanged(evt => callbacksAnimatedContainer.Toggle(evt.newValue, evt.animateChange))
                    .AddToToggleGroup(tabsGroup);

            callbacksAnimatedContainer.AddOnShowCallback(() =>
            {
                callbacksAnimatedContainer
                    .AddContent(
                        FluidField.Get()
                            .AddFieldContent(DesignUtils.UnityEventField("Controller started controlling a flow graph", propertyOnStart))
                    )
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(
                        FluidField.Get()
                            .AddFieldContent(DesignUtils.UnityEventField("Controller stopped controlling a flow graph", propertyOnStop))
                    )
                    .Bind(serializedObject);
            });

            //refresh tabs enabled indicator
            root.schedule.Execute(() =>
            {
                void UpdateIndicator(FluidTab tab, bool toggleOn, bool animateChange)
                {
                    if (tab == null) return;
                    if (tab.indicator.isOn != toggleOn)
                        tab.indicator.Toggle(toggleOn, animateChange);
                }

                bool HasOnStartCallbacks() => castedTarget.onStart != null && castedTarget.onStart.GetPersistentEventCount() > 0;
                bool HasOnStopCallbacks() => castedTarget.onStop != null && castedTarget.onStop.GetPersistentEventCount() > 0;
                bool HasCallbacks() => HasOnStartCallbacks() || HasOnStopCallbacks();

                //initial indicators state update (no animation)
                UpdateIndicator(callbacksTab, HasCallbacks(), false);

                //subsequent indicators state update (animated)
                callbacksTab.schedule.Execute(() => UpdateIndicator(callbacksTab, HasCallbacks(), true)).Every(200);
            });
        }

        private VisualElement Toolbar()
        {
            return
                toolbarContainer
                    .AddChild(settingsTab)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(callbacksTab)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(DesignUtils.flexibleSpace);
        }

        private VisualElement Content()
        {
            return
                contentContainer
                    .AddChild(settingsAnimatedContainer)
                    .AddChild(callbacksAnimatedContainer);
        }

        private void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild(Toolbar())
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(Content())
                .AddChild(DesignUtils.endOfLineBlock);
        }
    }
}
