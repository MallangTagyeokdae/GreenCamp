// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.EditorUI.Windows.Internal;
using Doozy.Editor.Nody.Automation.Generators;
using Doozy.Editor.Nody.ScriptableObjects;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Common;
using Doozy.Runtime.Nody;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;
using EditorStyles = Doozy.Editor.EditorUI.EditorStyles;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Nody
{
    public class NodyWindow : FluidWindow<NodyWindow>
    {
        private const string WINDOW_TITLE = "Nody";
        public const string k_WindowMenuPath = "Tools/Doozy/Nody/";

        [MenuItem(k_WindowMenuPath + "Window", priority = -900)]
        public static void Open()
        {
            InternalOpenWindow(WINDOW_TITLE);
        }

        [MenuItem("Tools/Doozy/Refresh/Nody", priority = -450)]
        public static void Refresh()
        {
            FlowNodeViewExtensionGenerator.Run();
            NodyNodeSearchWindowGenerator.Run();
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (Selection.activeObject is FlowGraph)
            {
                Open();
                OpenGraph((FlowGraph)Selection.activeObject);
                return true;
            }

            return false;
        }

        private static NodySettings settings => NodySettings.instance;

        public static Color accentColor => EditorColors.Nody.Color;
        public static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Nody.Color;

        private TemplateContainer templateContainer { get; set; }
        private VisualElement layoutContainer { get; set; }
        private VisualElement sideMenuContainer { get; set; }
        private VisualElement graphView { get; set; }

        public FlowGraphView flowGraphView { get; private set; }
        public FlowGraph flowGraph => flowGraphView?.flowGraph;

        private NodyMiniMap miniMap { get; set; }

        private FluidPlaceholder emptyPlaceholder { get; set; }

        private FluidSideMenu sideMenu { get; set; }
        private FluidToggleButtonTab showMinimapButton { get; set; }
        private FluidToggleButtonTab loadGraphButton { get; set; }
        private FluidToggleButtonTab saveGraphButton { get; set; }
        private FluidToggleButtonTab closeGraphButton { get; set; }

        private Label openedGraphPathLabel { get; set; }
        private FluidButton pingOpenGraphButton { get; set; }

        protected override void OnDestroy()
        {
            // Debugger.Log($"{nameof(NodyWindow)}.{nameof(OnDestroy)}");
            NodySettings.Save();
            base.OnDestroy();
        }

        private void UpdateSettings()
        {
            if (flowGraph == null) return;
            bool saveSettings =
                flowGraphView.viewTransform.position != flowGraph.EditorPosition ||
                flowGraphView.viewTransform.scale != flowGraph.EditorScale;
            if (!saveSettings) return;
            flowGraph.EditorPosition = flowGraphView.viewTransform.position;
            flowGraph.EditorScale = flowGraphView.viewTransform.scale;
        }

        protected override void OnPlayModeStateChanged(PlayModeStateChange playModeState)
        {
            // Debugger.Log($"{nameof(NodyWindow)}.{nameof(OnPlayModeStateChanged)}({playModeState})");
            switch (playModeState)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    //ignored
                    break;
                case PlayModeStateChange.EnteredEditMode:
                    //ignored
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    SaveSettings();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    //ignored
                    break;
            }
            base.OnPlayModeStateChanged(playModeState);
            switch (playModeState)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    LoadSettings();
                    break;
                case PlayModeStateChange.EnteredEditMode:
                    //ignored
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    //ignored
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    //ignored
                    break;
            }
        }

        protected override void CreateGUI()
        {
            // Debugger.Log($"{nameof(NodyWindow)}.{nameof(CreateGUI)}()");

            root
                .RecycleAndClear()
                .Add(templateContainer = EditorLayouts.Nody.NodyWindow.CloneTree());

            templateContainer
                .SetStyleFlexGrow(1)
                .AddStyle(EditorStyles.Nody.NodyWindow);

            layoutContainer = templateContainer.Q<VisualElement>(nameof(layoutContainer));
            sideMenuContainer = layoutContainer.Q<VisualElement>(nameof(sideMenuContainer));
            graphView = layoutContainer.Q<VisualElement>(nameof(graphView));

            emptyPlaceholder =
                FluidPlaceholder.Get()
                    .SetStyleAlignSelf(Align.Center)
                    .SetIcon(EditorSpriteSheets.Nody.Icons.Nody)
                    .SetAccentColor(EditorColors.Default.Background)
                    .Hide();

            CreateFlowGraphView();
            CreateMiniMap();
            CreateSideMenu();

            Compose();

            switch (currentPlayModeState)
            {
                case PlayModeStateChange.EnteredEditMode:
                case PlayModeStateChange.EnteredPlayMode:
                    LoadSettings();
                    break;
            }

            root.schedule.Execute(UpdateSettings).Every(250);
        }

        private void CreateSideMenu()
        {
            sideMenu =
                new FluidSideMenu()
                    .SetMenuLevel(FluidSideMenu.MenuLevel.Level_1)
                    .SetCustomWidth(160)
                    .IsCollapsable(true)
                    .CollapseMenu(false);

            #region LOAD --- Graph Button

            loadGraphButton =
                sideMenu.AddButton("Load", selectableAccentColor, false)
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Load)
                    .SetOnClick(() =>
                    {
                        saveGraphButton.schedule.Execute(() => loadGraphButton.SetIsOn(false));

                        FlowGraph graph = LoadGraphWithDialog<FlowGraph>();
                        if (graph == null) return;

                        Debugger.Log($"---> {nameof(LoadGraph)}({graph.name})");
                        OpenGraph(graph);
                        flowGraphView.schedule.Execute(() =>
                            {
                                Debugger.Log($"---> {nameof(LoadGraph)} FrameAll");
                                flowGraphView.FrameAll();
                            })
                            .ExecuteLater(100);
                    });

            #endregion

            #region SAVE --- Graph Button

            saveGraphButton =
                sideMenu.AddButton("Save", selectableAccentColor, false)
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Save)
                    .SetIsOn(false)
                    .SetOnClick(() =>
                    {
                        saveGraphButton.schedule.Execute(() => saveGraphButton.SetIsOn(false));

                        if (flowGraphView.flowGraph == null)
                        {
                            NodySettings.ResetSettings();
                            return;
                        }

                        EditorUtility.SetDirty(flowGraphView.flowGraph);
                        flowGraphView.flowGraph.nodes.ForEach(EditorUtility.SetDirty);
                        AssetDatabase.SaveAssets();
                        SaveSettings();
                    });

            #endregion

            #region CLOSE --- Graph Button

            closeGraphButton =
                sideMenu.AddButton("Close", selectableAccentColor, false)
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Close)
                    .SetIsOn(false)
                    .SetOnClick(() =>
                    {
                        closeGraphButton.schedule.Execute(() => closeGraphButton.SetIsOn(false));

                        if (flowGraphView.flowGraph == null)
                            return;

                        EditorUtility.SetDirty(flowGraphView.flowGraph);
                        flowGraphView.flowGraph.nodes.ForEach(EditorUtility.SetDirty);
                        AssetDatabase.SaveAssets();
                        CloseGraph();
                        NodySettings.ResetSettings();
                    });

            #endregion

            #region Show MiniMap Button

            showMinimapButton =
                sideMenu
                    .AddButton("MiniMap", selectableAccentColor, false)
                    .SetIcon(EditorSpriteSheets.Nody.Icons.Minimap)
                    .SetIsOn(miniMap.isVisible)
                    .SetOnValueChanged(evt =>
                    {
                        miniMap.SetIsVisible(evt.newValue);
                        UpdateShowMinimapButton();
                    })
                    .SetStyleMarginTop(DesignUtils.k_Spacing4X);

            UpdateShowMinimapButton();

            void UpdateShowMinimapButton()
            {
                showMinimapButton.buttonLabel.SetText
                (
                    miniMap.isVisible
                        ? "Hide MiniMap"
                        : "Show MiniMap"
                );
                showMinimapButton.SetTooltip(sideMenu.isCollapsed ? showMinimapButton.buttonLabel.text : string.Empty);
            }

            #endregion
        }

        private void CreateFlowGraphView()
        {
            flowGraphView =
                new FlowGraphView()
                {
                    OnNodeSelected = OnNodeSelectionChanged,
                };

            openedGraphPathLabel =
                DesignUtils.NewFieldNameLabel("Graph Path")
                    .SetStyleTextAlign(TextAnchor.MiddleLeft);

            pingOpenGraphButton =
                DesignUtils.GetNewTinyButton
                    (
                        string.Empty,
                        EditorSpriteSheets.EditorUI.Icons.Location,
                        EditorSelectableColors.Default.ButtonIcon,
                        "Ping the opened graph in the Project view"
                    )
                    .SetButtonStyle(ButtonStyle.Clear)
                    .SetOnClick(() =>
                    {
                        if (flowGraphView == null) return;
                        if (flowGraphView.flowGraph == null) return;
                        if (!AssetDatabase.IsMainAsset(flowGraphView.flowGraph)) return;
                        EditorGUIUtility.PingObject(flowGraphView.flowGraph);
                    });
        }

        private void CreateMiniMap()
        {
            miniMap = new NodyMiniMap();

            root.schedule.Execute(() =>
            {
                miniMap
                    .SetPosition
                    (
                        new Rect
                        (
                            DesignUtils.k_Spacing,
                            position.height - 200 - DesignUtils.k_Spacing,
                            200,
                            200
                        )
                    );

                miniMap.SetIsVisible(NodyMiniMap.GetPrefs_IsVisible());
            });
        }

        private void Compose()
        {
            sideMenuContainer
                .AddChild(sideMenu);


            graphView
                .AddChild
                (
                    flowGraphView
                        .AddChild(miniMap)
                )
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleHeight(24)
                        .SetStyleMarginTop(-24)
                        .SetStyleTop(-DesignUtils.k_Spacing)
                        .SetStyleMarginLeft(DesignUtils.k_Spacing)
                        .SetStyleAlignItems(Align.Center)
                        .SetStyleFlexGrow(0)
                        .AddChild(pingOpenGraphButton)
                        .AddChild(openedGraphPathLabel)
                )
                .AddChild
                (
                    DesignUtils.FullScreenVisualElement()
                        .AddChild(emptyPlaceholder)
                );
        }

        private void SaveSettings()
        {
            NodySettings.SaveSettings();
        }

        private void LoadSettings()
        {
            if (Selection.activeObject != null && Selection.activeObject is GameObject go)
            {
                FlowController fc = go.GetComponent<FlowController>();
                if (fc != null && fc.flow != null)
                {
                    OpenGraph(fc.flow);
                    return;
                }
            }

            if (settings.Graph == null)
            {
                ShowEmptyState();
                return; //no reference was saved -> stop
            }
            OpenGraph(settings.Graph);
        }

        private void OnSelectionChange()
        {
            // Debugger.Log($"{nameof(NodyWindow)}.{nameof(OnSelectionChange)}");

            if (flowGraphView?.flowGraph == null)
                CloseGraph();

            if (Selection.activeObject == null)
                return;

            if (Selection.activeObject is FlowGraph graph)
            {
                if (flowGraphView?.flowGraph == graph)
                    return;

                if (!EditorUtility.IsPersistent(graph))
                    return;

                OpenGraph(graph);
                return;
            }

            if (Selection.activeGameObject == null)
                return;

            FlowController controller = Selection.activeGameObject.GetComponent<FlowController>();

            if (controller == null)
                return;

            if (controller.flow == null)
                return;

            if (flowGraphView?.flowGraph == controller.flow)
                return;

            OpenGraph(controller.flow);
        }

        private void ShowEmptyState()
        {
            emptyPlaceholder?.Show();
            pingOpenGraphButton?.SetStyleDisplay(DisplayStyle.None);
            openedGraphPathLabel?.SetStyleDisplay(DisplayStyle.None).SetText(string.Empty);
        }

        private void HideEmptyState()
        {
            emptyPlaceholder?.Hide();
            pingOpenGraphButton?.SetStyleDisplay(DisplayStyle.Flex);
            openedGraphPathLabel?.SetStyleDisplay(DisplayStyle.Flex);
        }

        private static void OnNodeSelectionChanged(FlowNodeView nodeView)
        {
            if (nodeView == null)
            {
                Selection.activeObject = null;
                return;
            }
            Selection.activeObject = nodeView.flowNode;
        }

        public static void OpenGraph(FlowGraph graph)
        {
            // Debugger.Log($"{nameof(NodyWindow)}.{nameof(OpenGraph)}({(graph == null ? "null" : $"{graph.name}")})");

            if (graph == null)
                CloseGraph();

            if (graph == null)
            {
                if (Selection.activeGameObject == null)
                    return;

                FlowController controller = Selection.activeGameObject.GetComponent<FlowController>();

                if (controller == null)
                    return;

                graph = controller.flow;
            }

            if (graph == null) return;
            
            if (Application.isPlaying)
            {
                if (instance.flowGraphView == null) return;
                instance.flowGraphView.PopulateView(graph);
                instance.flowGraphView.UpdateViewTransform(graph.EditorPosition, graph.EditorScale);
                UpdatePathInfo();

                return;
            }

            if (!AssetDatabase.CanOpenForEdit(graph)) return;


            graph.ResetGraph();
            if (instance.flowGraphView == null) return;
            instance.flowGraphView.PopulateView(graph);
            instance.flowGraphView.UpdateViewTransform(graph.EditorPosition, graph.EditorScale);
            UpdatePathInfo();

            void UpdatePathInfo()
            {
                instance.HideEmptyState();
                if (EditorUtility.IsPersistent(graph))
                {
                    instance.openedGraphPathLabel.SetText(AssetDatabase.GetAssetPath(graph));
                }
                else
                {
                    instance.openedGraphPathLabel.SetText("Local Graph");
                    instance.pingOpenGraphButton.SetStyleDisplay(DisplayStyle.None);
                }
            }
        }

        public static void CloseGraph()
        {
            // Debugger.Log($"{nameof(NodyWindow)}.{nameof(CloseGraph)}");

            instance.ShowEmptyState();

            if (instance.flowGraphView == null) return;
            if (instance.flowGraphView.flowGraph != null)
                AssetDatabase.SaveAssetIfDirty(instance.flowGraphView.flowGraph);

            instance.flowGraphView.flowGraph = null;
            instance.flowGraphView.ClearGraphView(true);
        }

        /// <summary> Loads a graph from the given path </summary>
        /// <param name="path"> Asset path where the graph can be found </param>
        public static T LoadGraph<T>(string path) where T : FlowGraph
        {
            if (string.IsNullOrEmpty(path)) return null;
            T graph = AssetDatabase.LoadAssetAtPath<T>(FileUtil.GetProjectRelativePath(path));
            return graph == null ? null : graph;
        }

        /// <summary> Opens a file panel set to filter only the given Graph type and returns the selected asset file </summary>
        public static T LoadGraphWithDialog<T>() where T : FlowGraph
        {
            string path = EditorUtility.OpenFilePanelWithFilters
            (
                $"Load {ObjectNames.NicifyVariableName(typeof(T).Name)}",
                "",
                new[] { typeof(T).Name, "asset" }
            );
            return LoadGraph<T>(path);
        }

    }
}
