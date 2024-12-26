// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Nody.Nodes.Internal;
using Doozy.Runtime.SceneManagement;
using Doozy.Runtime.SceneManagement.Nodes;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.SceneManagement.Nodes
{
    [CustomEditor(typeof(UnloadSceneNode))]
    public class UnloadSceneNodeEditor : FlowNodeEditor
    {
        public override Color nodeAccentColor => EditorColors.SceneManagement.Component;
        public override EditorSelectableColorInfo nodeSelectableAccentColor => EditorSelectableColors.SceneManagement.Component;
        public override IEnumerable<Texture2D> nodeIconTextures => EditorSpriteSheets.SceneManagement.Icons.UnloadSceneNode;

        private FluidField getSceneByFluidField { get; set; }
        private FluidField sceneBuildIndexFluidField { get; set; }
        private FluidField sceneNameFluidField { get; set; }
        private FluidToggleSwitch waitForSceneToUnloadSwitch { get; set; }

        private SerializedProperty propertyGetSceneBy { get; set; }
        private SerializedProperty propertySceneBuildIndex { get; set; }
        private SerializedProperty propertySceneName { get; set; }
        private SerializedProperty propertyWaitForSceneToUnload { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            getSceneByFluidField?.Recycle();
            sceneBuildIndexFluidField?.Recycle();
            sceneNameFluidField?.Recycle();
            waitForSceneToUnloadSwitch?.Recycle();
        }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyGetSceneBy = serializedObject.FindProperty(nameof(UnloadSceneNode.GetSceneBy));
            propertySceneBuildIndex = serializedObject.FindProperty(nameof(UnloadSceneNode.SceneBuildIndex));
            propertySceneName = serializedObject.FindProperty(nameof(UnloadSceneNode.SceneName));
            propertyWaitForSceneToUnload = serializedObject.FindProperty(nameof(UnloadSceneNode.WaitForSceneToUnload));
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(UnloadSceneNode)))
                .SetIcon(EditorSpriteSheets.SceneManagement.Icons.UnloadSceneNode)
                .SetAccentColor(EditorColors.SceneManagement.Component)
                .AddManualButton()
                .AddApiButton()
                .AddYouTubeButton()
                ;

            EnumField getSceneByEnumField =
                DesignUtils.NewEnumField(propertyGetSceneBy).SetStyleFlexGrow(1);

            getSceneByFluidField =
                FluidField.Get()
                    .SetLabelText("Get Scene By")
                    .AddFieldContent(getSceneByEnumField)
                    .SetStyleMaxWidth(112);

            sceneNameFluidField =
                FluidField.Get<TextField>(propertySceneName)
                    .SetLabelText("Scene Name");

            sceneBuildIndexFluidField =
                FluidField.Get<IntegerField>(propertySceneBuildIndex)
                    .SetLabelText("Scene Build Index");

            sceneNameFluidField.SetStyleDisplay(propertyGetSceneBy.enumValueIndex == (int)GetSceneBy.Name ? DisplayStyle.Flex : DisplayStyle.None);
            sceneBuildIndexFluidField.SetStyleDisplay(propertyGetSceneBy.enumValueIndex == (int)GetSceneBy.BuildIndex ? DisplayStyle.Flex : DisplayStyle.None);
            getSceneByEnumField.RegisterValueChangedCallback(evt =>
            {
                if (evt?.newValue == null) return;
                sceneNameFluidField.SetStyleDisplay((GetSceneBy)evt.newValue == GetSceneBy.Name ? DisplayStyle.Flex : DisplayStyle.None);
                sceneBuildIndexFluidField.SetStyleDisplay((GetSceneBy)evt.newValue == GetSceneBy.BuildIndex ? DisplayStyle.Flex : DisplayStyle.None);
            });


            waitForSceneToUnloadSwitch =
                FluidToggleSwitch.Get()
                    .SetLabelText("Wait for Scene to Unload")
                    .SetToggleAccentColor(nodeSelectableAccentColor)
                    .BindToProperty(propertyWaitForSceneToUnload);

            AutoRefreshNodeView(); // <<< IMPORTANT - this updates the NodeView
        }

        protected override void Compose()
        {
            base.Compose();

            root
                .AddChild
                (
                    DesignUtils.column
                        .AddChild
                        (
                            DesignUtils.row
                                .AddChild(getSceneByFluidField)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(sceneNameFluidField)
                                .AddChild(sceneBuildIndexFluidField)
                        )
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild
                        (
                            DesignUtils.row
                                .AddChild(waitForSceneToUnloadSwitch)
                        )
                )
                ;
        }
    }
}
