// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Globalization;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Nody;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Nody;
using Doozy.Runtime.SceneManagement;
using Doozy.Runtime.SceneManagement.Nodes;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Doozy.Editor.SceneManagement.Nodes
{
    public class LoadSceneNodeView : FlowNodeView
    {
        public override Type nodeType => typeof(LoadSceneNode);
        public override IEnumerable<Texture2D> nodeIconTextures => EditorSpriteSheets.SceneManagement.Icons.LoadSceneNode;
        public override Color nodeAccentColor => EditorColors.SceneManagement.Component;
        public override EditorSelectableColorInfo nodeSelectableAccentColor => EditorSelectableColors.SceneManagement.Component;

        private FluidDualLabel allowSceneActivationInfoLabel { get; set; }
        private FluidDualLabel connectProgressorInfoLabel { get; set; }
        private FluidDualLabel loadSceneModeInfoLabel { get; set; }
        private FluidDualLabel progressorIdInfoLabel { get; set; }
        private FluidDualLabel sceneActivationDelayInfoLabel { get; set; }
        private FluidDualLabel sceneInfoLabel { get; set; }
        private FluidDualLabel preventLoadingSameSceneInfoLabel { get; set; }
        private FluidDualLabel waitForSceneToLoadInfoLabel { get; set; }

        private SerializedProperty propertyAllowSceneActivation { get; set; }
        private SerializedProperty propertyConnectProgressor { get; set; }
        private SerializedProperty propertyGetSceneBy { get; set; }
        private SerializedProperty propertyPreventLoadingSameScene { get; set; }
        private SerializedProperty propertyLoadSceneMode { get; set; }
        private SerializedProperty propertyProgressorId { get; set; }
        private SerializedProperty propertySceneActivationDelay { get; set; }
        private SerializedProperty propertySceneBuildIndex { get; set; }
        private SerializedProperty propertySceneName { get; set; }
        private SerializedProperty propertyWaitForSceneToLoad { get; set; }

        private string loadSceneModeInfoTitle =>
            "Load Scene Mode:";

        private string loadSceneModeInfoDescription =>
            ((LoadSceneMode)propertyLoadSceneMode.enumValueIndex).ToString();

        private string sceneInfoTitle =>
            propertyGetSceneBy.enumValueIndex == (int)GetSceneBy.Name
                ? "Scene Name:"
                : "Scene Build Index:";

        private string preventLoadingSameSceneInfoTitle =>
            "Prevent Loading Same Scene:";

        private string preventLoadingSameSceneInfoDescription =>
            propertyPreventLoadingSameScene.boolValue
                ? "Yes"
                : "No";

        private string sceneInfoDescription =>
            propertyGetSceneBy.enumValueIndex == (int)GetSceneBy.Name
                ? propertySceneName.stringValue.IsNullOrEmpty()
                    ? "---"
                    : propertySceneName.stringValue
                : propertySceneBuildIndex.intValue.ToString();

        private string allowSceneActivationInfoTitle =>
            "Allow Scene Activation:";

        private string allowSceneActivationInfoDescription =>
            propertyAllowSceneActivation.boolValue
                ? "Yes"
                : "No";

        private string sceneActivationDelayInfoTitle =>
            "Scene Activation Delay:";

        private string sceneActivationDelayInfoDescription =>
            propertySceneActivationDelay.floatValue.ToString(CultureInfo.InvariantCulture);

        private string waitForSceneToLoadInfoTitle =>
            "Wait for Scene to Load:";

        private string waitForSceneToLoadInfoDescription =>
            propertyWaitForSceneToLoad.boolValue
                ? "Yes"
                : "No";

        private string connectProgressorInfoTitle =>
            "Connect Progressor:";

        private string connectProgressorInfoDescription =>
            propertyConnectProgressor.boolValue
                ? "Yes"
                : "No";

        private string progressorIdInfoTitle =>
            "Progressor Id:";

        private string progressorIdInfoDescription =>
            ((LoadSceneNode)flowNode).ProgressorId.ToString();

        public LoadSceneNodeView(FlowGraphView graphView, FlowNode node) : base(graphView, node)
        {
        }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyAllowSceneActivation = serializedObject.FindProperty(nameof(LoadSceneNode.AllowSceneActivation));
            propertyConnectProgressor = serializedObject.FindProperty(nameof(LoadSceneNode.ConnectProgressor));
            propertyGetSceneBy = serializedObject.FindProperty(nameof(LoadSceneNode.GetSceneBy));
            propertyPreventLoadingSameScene = serializedObject.FindProperty(nameof(LoadSceneNode.PreventLoadingSameScene));
            propertyLoadSceneMode = serializedObject.FindProperty(nameof(LoadSceneNode.LoadSceneMode));
            propertyProgressorId = serializedObject.FindProperty(nameof(LoadSceneNode.ProgressorId));
            propertySceneActivationDelay = serializedObject.FindProperty(nameof(LoadSceneNode.SceneActivationDelay));
            propertySceneBuildIndex = serializedObject.FindProperty(nameof(LoadSceneNode.SceneBuildIndex));
            propertySceneName = serializedObject.FindProperty(nameof(LoadSceneNode.SceneName));
            propertyWaitForSceneToLoad = serializedObject.FindProperty(nameof(LoadSceneNode.WaitForSceneToLoad));
        }

        protected override void InitializeView()
        {
            base.InitializeView();

            FluidDualLabel GetDualLabel() =>
                new FluidDualLabel()
                    .SetElementSize(ElementSize.Small)
                    .SetDescriptionTextColor(nodeAccentColor);

            preventLoadingSameSceneInfoLabel = GetDualLabel();
            allowSceneActivationInfoLabel = GetDualLabel();
            connectProgressorInfoLabel = GetDualLabel();
            loadSceneModeInfoLabel = GetDualLabel();
            progressorIdInfoLabel = GetDualLabel().SetStyleMarginTop(DesignUtils.k_Spacing);
            sceneActivationDelayInfoLabel = GetDualLabel();
            waitForSceneToLoadInfoLabel = GetDualLabel();
            sceneInfoLabel = GetDualLabel();

            portDivider
                .SetStyleBackgroundColor(EditorColors.Nody.MiniMapBackground)
                .SetStyleMarginLeft(DesignUtils.k_Spacing)
                .SetStyleMarginRight(DesignUtils.k_Spacing)
                .SetStylePadding(DesignUtils.k_Spacing2X)
                .SetStyleBorderRadius(DesignUtils.k_Spacing)
                .SetStyleJustifyContent(Justify.Center)
                .AddChild(sceneInfoLabel)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(preventLoadingSameSceneInfoLabel)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(loadSceneModeInfoLabel)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(allowSceneActivationInfoLabel)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(sceneActivationDelayInfoLabel)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(waitForSceneToLoadInfoLabel)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(connectProgressorInfoLabel)
                .AddChild(progressorIdInfoLabel)
                ;
        }



        public override void RefreshNodeView()
        {
            base.RefreshNodeView();

            
            preventLoadingSameSceneInfoLabel.SetTitle(preventLoadingSameSceneInfoTitle).SetDescription(preventLoadingSameSceneInfoDescription);
            loadSceneModeInfoLabel.SetTitle(loadSceneModeInfoTitle).SetDescription(loadSceneModeInfoDescription);
            sceneInfoLabel.SetTitle(sceneInfoTitle).SetDescription(sceneInfoDescription);
            allowSceneActivationInfoLabel.SetTitle(allowSceneActivationInfoTitle).SetDescription(allowSceneActivationInfoDescription);
            sceneActivationDelayInfoLabel.SetTitle(sceneActivationDelayInfoTitle).SetDescription(sceneActivationDelayInfoDescription);
            waitForSceneToLoadInfoLabel.SetTitle(waitForSceneToLoadInfoTitle).SetDescription(waitForSceneToLoadInfoDescription);
            connectProgressorInfoLabel.SetTitle(connectProgressorInfoTitle).SetDescription(connectProgressorInfoDescription);

            progressorIdInfoLabel.SetTitle(progressorIdInfoTitle).SetDescription(progressorIdInfoDescription);
            progressorIdInfoLabel.SetStyleDisplay(propertyConnectProgressor.boolValue ? DisplayStyle.Flex : DisplayStyle.None);
        }
    }
}
