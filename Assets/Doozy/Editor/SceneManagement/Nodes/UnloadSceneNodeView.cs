// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
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
using UnityEngine.UIElements;

namespace Doozy.Editor.SceneManagement.Nodes
{
    public class UnloadSceneNodeView : FlowNodeView
    {
        public override Type nodeType => typeof(UnloadSceneNode);
        public override Color nodeAccentColor => EditorColors.SceneManagement.Component;
        public override EditorSelectableColorInfo nodeSelectableAccentColor => EditorSelectableColors.SceneManagement.Component;
        public override IEnumerable<Texture2D> nodeIconTextures => EditorSpriteSheets.SceneManagement.Icons.UnloadSceneNode;

        private FluidDualLabel sceneInfoLabel { get; set; }
        private FluidDualLabel waitInfoLabel { get; set; }

        private SerializedProperty propertyGetSceneBy { get; set; }
        private SerializedProperty propertySceneBuildIndex { get; set; }
        private SerializedProperty propertySceneName { get; set; }
        private SerializedProperty propertyWaitForSceneToUnload { get; set; }

        private string sceneInfoTitle =>
            propertyGetSceneBy.enumValueIndex == (int)GetSceneBy.Name
                ? "Scene Name:"
                : "Scene Build Index:";

        private string sceneInfoDescription =>
            propertyGetSceneBy.enumValueIndex == (int)GetSceneBy.Name
                ? propertySceneName.stringValue.IsNullOrEmpty()
                    ? "---"
                    : propertySceneName.stringValue
                : propertySceneBuildIndex.intValue.ToString();

        private string waitInfoTitle =>
            "Wait for Scene to Unload:";

        private string waitInfoDescription =>
            propertyWaitForSceneToUnload.boolValue
                ? "Yes"
                : "No";
        
        public UnloadSceneNodeView(FlowGraphView graphView, FlowNode node) : base(graphView, node)
        {
        }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyGetSceneBy = serializedObject.FindProperty(nameof(UnloadSceneNode.GetSceneBy));
            propertySceneBuildIndex = serializedObject.FindProperty(nameof(UnloadSceneNode.SceneBuildIndex));
            propertySceneName = serializedObject.FindProperty(nameof(UnloadSceneNode.SceneName));
            propertyWaitForSceneToUnload = serializedObject.FindProperty(nameof(UnloadSceneNode.WaitForSceneToUnload));
        }

        protected override void InitializeView()
        {
            base.InitializeView();

            sceneInfoLabel =
                new FluidDualLabel()
                    .SetElementSize(ElementSize.Small)
                    .SetDescriptionTextColor(nodeAccentColor);

            waitInfoLabel =
                new FluidDualLabel()
                    .SetTitle(waitInfoTitle)
                    .SetElementSize(ElementSize.Small)
                    .SetDescriptionTextColor(nodeAccentColor);

            portDivider
                .SetStyleBackgroundColor(EditorColors.Nody.MiniMapBackground)
                .SetStyleMarginLeft(DesignUtils.k_Spacing)
                .SetStyleMarginRight(DesignUtils.k_Spacing)
                .SetStylePadding(DesignUtils.k_Spacing2X)
                .SetStyleBorderRadius(DesignUtils.k_Spacing)
                .SetStyleJustifyContent(Justify.Center)
                .AddChild(sceneInfoLabel)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(waitInfoLabel);
        }

        public override void RefreshNodeView()
        {
            base.RefreshNodeView();

            sceneInfoLabel
                .SetTitle(sceneInfoTitle)
                .SetDescription(sceneInfoDescription);

            waitInfoLabel
                .SetTitle(waitInfoTitle)
                .SetDescription(waitInfoDescription);
        }
    }
}
