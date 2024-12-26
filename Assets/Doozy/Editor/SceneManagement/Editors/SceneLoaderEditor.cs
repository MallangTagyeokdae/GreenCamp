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
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.SceneManagement;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ObjectNames = Doozy.Runtime.Common.Utils.ObjectNames;

namespace Doozy.Editor.SceneManagement.Editors
{
    [CustomEditor(typeof(SceneLoader), true)]
    public class SceneLoaderEditor : UnityEditor.Editor
    {
        private SceneLoader castedTarget => (SceneLoader)target;
        private List<SceneLoader> castedTargets => targets.Cast<SceneLoader>().ToList();

        private static Color accentColor => EditorColors.SceneManagement.Component;
        private static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.SceneManagement.Component;

        private VisualElement root { get; set; }
        private FluidComponentHeader componentHeader { get; set; }
        private VisualElement toolbarContainer { get; set; }
        private VisualElement contentContainer { get; set; }

        private FluidToggleGroup tabsGroup { get; set; }
        private FluidTab settingsTab { get; set; }
        private FluidTab callbacksTab { get; set; }
        private FluidTab progressorsTab { get; set; }

        private FluidAnimatedContainer settingsAnimatedContainer { get; set; }
        private FluidAnimatedContainer callbacksAnimatedContainer { get; set; }
        private FluidAnimatedContainer progressorsAnimatedContainer { get; set; }

        private FluidButton loadSceneButton { get; set; }
        private FluidField allowSceneActivationFluidField { get; set; }
        private FluidField getSceneByFluidField { get; set; }
        private FluidField preventLoadingSameSceneFluidField { get; set; }
        private FluidField loadSceneModeFluidField { get; set; }
        private FluidField sceneActivationDelayFluidField { get; set; }
        private FluidField sceneBuildIndexFluidField { get; set; }
        private FluidField sceneNameFluidField { get; set; }
        private FluidToggleSwitch allowSceneActivationSwitch { get; set; }
        private FluidToggleSwitch debugModeSwitch { get; set; }
        private FluidToggleSwitch preventLoadingSameSceneSwitch { get; set; }

        private SerializedProperty propertyAllowSceneActivation { get; set; }
        private SerializedProperty propertyDebugMode { get; set; }
        private SerializedProperty propertyGetSceneBy { get; set; }
        private SerializedProperty propertyPreventLoadingSameScene { get; set; }
        private SerializedProperty propertyLoadSceneMode { get; set; }
        private SerializedProperty propertyOnLoadScene { get; set; }
        private SerializedProperty propertyOnProgressChanged { get; set; }
        private SerializedProperty propertyOnSceneActivated { get; set; }
        private SerializedProperty propertyOnSceneLoaded { get; set; }
        private SerializedProperty propertyProgressors { get; set; }
        private SerializedProperty propertySceneActivationDelay { get; set; }
        private SerializedProperty propertySceneBuildIndex { get; set; }
        private SerializedProperty propertySceneName { get; set; }
        private SerializedProperty propertySelfDestructAfterSceneLoaded { get; set; }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        private void OnEnable()
        {
            castedTargets.ForEach(loader => loader.progressors.RemoveNulls());
        }

        private void OnDestroy()
        {
            componentHeader?.Recycle();

            settingsAnimatedContainer?.Dispose();
            callbacksAnimatedContainer?.Dispose();
            progressorsAnimatedContainer?.Dispose();

            tabsGroup?.Recycle();
            settingsTab?.Dispose();
            callbacksTab?.Dispose();
            progressorsTab?.Dispose();
        }

        private void FindProperties()
        {
            propertyAllowSceneActivation = serializedObject.FindProperty("AllowSceneActivation");
            propertyDebugMode = serializedObject.FindProperty(nameof(SceneLoader.DebugMode));
            propertyGetSceneBy = serializedObject.FindProperty(nameof(SceneLoader.GetSceneBy));
            propertyPreventLoadingSameScene = serializedObject.FindProperty("PreventLoadingSameScene");
            propertyLoadSceneMode = serializedObject.FindProperty(nameof(SceneLoader.LoadSceneMode));
            propertyOnLoadScene = serializedObject.FindProperty(nameof(SceneLoader.OnLoadScene));
            propertyOnProgressChanged = serializedObject.FindProperty(nameof(SceneLoader.OnProgressChanged));
            propertyOnSceneActivated = serializedObject.FindProperty(nameof(SceneLoader.OnSceneActivated));
            propertyOnSceneLoaded = serializedObject.FindProperty(nameof(SceneLoader.OnSceneLoaded));
            propertyProgressors = serializedObject.FindProperty("Progressors");
            propertySceneActivationDelay = serializedObject.FindProperty(nameof(SceneLoader.SceneActivationDelay));
            propertySceneBuildIndex = serializedObject.FindProperty(nameof(SceneLoader.SceneBuildIndex));
            propertySceneName = serializedObject.FindProperty(nameof(SceneLoader.SceneName));
            propertySelfDestructAfterSceneLoaded = serializedObject.FindProperty(nameof(SceneLoader.SelfDestructAfterSceneLoaded));
        }

        private void InitializeEditor()
        {
            FindProperties();
            root = DesignUtils.GetEditorRoot();
            componentHeader =
                DesignUtils.editorComponentHeader
                    .SetAccentColor(accentColor)
                    .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(SceneLoader)))
                    .SetIcon(EditorSpriteSheets.SceneManagement.Icons.SceneLoader)
                    .AddManualButton()
                    .AddApiButton()
                    .AddYouTubeButton();
            toolbarContainer = DesignUtils.editorToolbarContainer;
            tabsGroup = FluidToggleGroup.Get().SetControlMode(FluidToggleGroup.ControlMode.OneToggleOn);
            contentContainer = DesignUtils.editorContentContainer;
            debugModeSwitch = DesignUtils.GetDebugSwitch(propertyDebugMode);

            InitializeSettings();
            InitializeCallbacks();
            InitializeProgressors();

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
                loadSceneButton =
                    FluidButton.Get()
                        .SetLabelText("Load Scene")
                        .SetIcon(EditorSpriteSheets.EditorUI.Icons.Unity)
                        .SetAccentColor(selectableAccentColor)
                        .SetStyleFlexGrow(0)
                        .SetStyleFlexShrink(0)
                        .SetButtonStyle(ButtonStyle.Contained)
                        .SetOnClick(() => castedTarget.LoadSceneAsync())
                        .SetStyleDisplay(Application.isPlaying ? DisplayStyle.Flex : DisplayStyle.None)
                        .SetStylePaddingTop(DesignUtils.k_Spacing2X)
                        .SetStylePaddingBottom(DesignUtils.k_Spacing2X);

                loadSceneModeFluidField =
                    FluidField.Get<EnumField>(propertyLoadSceneMode)
                        .SetLabelText("Load Scene Mode")
                        .SetTooltip("Determines how the new scene is loaded by this SceneLoader if the load scene method is called without any parameters")
                        .SetStyleMaxWidth(112);

                EnumField getSceneByEnumField =
                    DesignUtils.NewEnumField(propertyGetSceneBy)
                        .SetStyleFlexGrow(1);

                getSceneByFluidField =
                    FluidField.Get()
                        .SetLabelText("Get Scene By")
                        .SetTooltip("Determines how the scene is found by this SceneLoader")
                        .AddFieldContent(getSceneByEnumField)
                        .SetStyleMaxWidth(112);

                sceneNameFluidField =
                    FluidField.Get<TextField>(propertySceneName)
                        .SetLabelText("Scene Name")
                        .SetTooltip("The name of the scene to load");

                sceneBuildIndexFluidField =
                    FluidField.Get<TextField>(propertySceneBuildIndex)
                        .SetLabelText("Scene Build Index")
                        .SetTooltip("The build index of the scene to load");

                sceneNameFluidField.SetStyleDisplay(propertyGetSceneBy.enumValueIndex == (int)GetSceneBy.Name ? DisplayStyle.Flex : DisplayStyle.None);
                sceneBuildIndexFluidField.SetStyleDisplay(propertyGetSceneBy.enumValueIndex == (int)GetSceneBy.BuildIndex ? DisplayStyle.Flex : DisplayStyle.None);
                getSceneByEnumField.RegisterValueChangedCallback(evt =>
                {
                    if (evt?.newValue == null) return;
                    sceneNameFluidField.SetStyleDisplay((GetSceneBy)evt.newValue == GetSceneBy.Name ? DisplayStyle.Flex : DisplayStyle.None);
                    sceneBuildIndexFluidField.SetStyleDisplay((GetSceneBy)evt.newValue == GetSceneBy.BuildIndex ? DisplayStyle.Flex : DisplayStyle.None);
                });

                preventLoadingSameSceneSwitch =
                    FluidToggleSwitch.Get()
                        .SetLabelText("Prevent Loading Same Scene")
                        .SetToggleAccentColor(selectableAccentColor)
                        .BindToProperty(propertyPreventLoadingSameScene);

                preventLoadingSameSceneFluidField =
                    FluidField.Get()
                        .SetTooltip("Prevents loading a scene that is already loaded")
                        .AddFieldContent(preventLoadingSameSceneSwitch);

                allowSceneActivationSwitch =
                    FluidToggleSwitch.Get()
                        .SetLabelText("Allow Scene Activation")
                        .SetToggleAccentColor(selectableAccentColor)
                        .BindToProperty(propertyAllowSceneActivation);

                allowSceneActivationFluidField =
                    FluidField.Get()
                        .SetTooltip
                        (
                            "Allow Scenes to be activated as soon as it is ready.\n\n" +
                            "When loading a scene, Unity first loads the scene (load progress from 0% to 90%) and then activates it (load progress from 90% to 100%). It's a two state process.\n\n" +
                            "This option can stop the scene activation (at 90% load progress), after the scene has been loaded and is ready.\n\n" +
                            "Useful if you need to load several scenes at once and activate them in a specific order and/or at a specific time."
                        )
                        .SetStyleFlexGrow(0)
                        .AddFieldContent(allowSceneActivationSwitch);

                allowSceneActivationFluidField.fieldContent.SetStyleJustifyContent(Justify.Center);

                sceneActivationDelayFluidField =
                    FluidField.Get<FloatField>(propertySceneActivationDelay)
                        .SetIcon(EditorSpriteSheets.EditorUI.Icons.Cooldown)
                        .SetLabelText("Scene Activation Delay")
                        .SetTooltip
                        (
                            "Sets for how long will the SceneLoader wait, after a scene has been loaded, before it starts the scene activation process (works only if AllowSceneActivation is enabled).\n\n" +
                            "When loading a scene, Unity first loads the scene (load progress from 0% to 90%) and then activates it (load progress from 90% to 100%). It's a two state process.\n\n" +
                            "This delay is after the scene has been loaded and before its activation (at 90% load progress)"
                        );

                sceneActivationDelayFluidField.SetEnabled(propertyAllowSceneActivation.boolValue);
                allowSceneActivationSwitch.SetOnValueChanged(evt => sceneActivationDelayFluidField.SetEnabled(evt.newValue));

                settingsAnimatedContainer
                    .AddContent(loadSceneButton)
                    .AddContent
                    (
                        DesignUtils.row
                            .AddChild(getSceneByFluidField)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(sceneNameFluidField)
                            .AddChild(sceneBuildIndexFluidField)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(loadSceneModeFluidField)
                    )
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(preventLoadingSameSceneFluidField)
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent
                    (
                        DesignUtils.row
                            .AddChild(allowSceneActivationFluidField)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(sceneActivationDelayFluidField)
                    )
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
                    .AddContent
                    (
                        FluidField.Get<PropertyField>(propertyOnLoadScene)
                            .SetLabelText("0% - Scene started loading")
                    )
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent
                    (
                        FluidField.Get<PropertyField>(propertyOnSceneLoaded)
                            .SetLabelText("90% - Scene has been loaded")
                    )
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent
                    (
                        FluidField.Get<PropertyField>(propertyOnSceneActivated)
                            .SetLabelText("100% - Scene was loaded and then activated")
                    )
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent
                    (
                        FluidField.Get<PropertyField>(propertyOnProgressChanged)
                            .SetLabelText("Scene is loading and the load progress has been updated")
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

                bool HasOnLoadSceneCallbacks() => castedTarget.OnLoadScene != null && castedTarget.OnLoadScene.hasCallbacks;

                bool HasOnSceneLoadedCallbacks() => castedTarget.OnSceneLoaded != null && castedTarget.OnSceneLoaded.hasCallbacks;

                bool HasOnSceneActivatedCallbacks() => castedTarget.OnSceneActivated != null && castedTarget.OnSceneActivated.hasCallbacks;

                bool HasOnProgressChangedCallbacks() => castedTarget.OnProgressChanged != null && castedTarget.OnProgressChanged.GetPersistentEventCount() > 0;

                bool HasCallbacks() => HasOnLoadSceneCallbacks() || HasOnSceneLoadedCallbacks() || HasOnSceneActivatedCallbacks() || HasOnProgressChangedCallbacks();

                //initial indicators state update (no animation)
                UpdateIndicator(callbacksTab, HasCallbacks(), false);

                //subsequent indicators state update (animated)
                callbacksTab.schedule.Execute(() => UpdateIndicator(callbacksTab, HasCallbacks(), true)).Every(200);
            });
        }

        private void InitializeProgressors()
        {
            progressorsAnimatedContainer = new FluidAnimatedContainer("Progressors", true).Hide(false);
            progressorsTab =
                new FluidTab()
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
                            propertyProgressors,
                            "Progressors",
                            "Updated when a scene is being loaded",
                            typeof(Progressor)
                        )
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

                bool HasProgressors() => castedTarget.progressors.Count > 0;

                //initial indicators state update (no animation)
                UpdateIndicator(progressorsTab, HasProgressors(), false);

                //subsequent indicators state update (animated)
                progressorsTab.schedule.Execute(() => UpdateIndicator(progressorsTab, HasProgressors(), true)).Every(200);
            });
        }

        private VisualElement Toolbar()
        {
            return
                toolbarContainer
                    .AddChild(settingsTab)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(callbacksTab)
                    .AddChild(DesignUtils.spaceBlock4X)
                    .AddChild(progressorsTab)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(DesignUtils.flexibleSpace)
                    .AddChild(debugModeSwitch);
        }

        private VisualElement Content()
        {
            return
                contentContainer
                    .AddChild(settingsAnimatedContainer)
                    .AddChild(callbacksAnimatedContainer)
                    .AddChild(progressorsAnimatedContainer);
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
