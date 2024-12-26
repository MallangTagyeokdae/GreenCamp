// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.SceneManagement;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.SceneManagement.Editors
{
    [CustomEditor(typeof(SceneDirector), true)]
    public class SceneDirectorEditor : UnityEditor.Editor
    {
        private SceneDirector castedTarget => (SceneDirector)target;
        
        private static Color accentColor => EditorColors.SceneManagement.Component;
        private static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.SceneManagement.Component;
        
        private static IEnumerable<Texture2D> componentIconTextures => EditorSpriteSheets.SceneManagement.Icons.SceneDirector;
        private static IEnumerable<Texture2D> unityLogoIconTextures => EditorSpriteSheets.EditorUI.Icons.Unity;
        
        private VisualElement root { get; set; }
        private FluidComponentHeader componentHeader { get; set; }
        
        private FluidField onActiveSceneChangedFluidField { get; set; }
        private FluidField onSceneLoadedFluidField { get; set; }
        private FluidField onSceneUnloadedFluidField { get; set; }
        
        private FluidToggleSwitch debugModeSwitch { get; set; }
        
        private SerializedProperty propertyDebugMode { get; set; }
        private SerializedProperty propertyOnActiveSceneChanged { get; set; }
        private SerializedProperty propertyOnSceneLoaded { get; set; }
        private SerializedProperty propertyOnSceneUnloaded { get; set; }
        
        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        private void OnDestroy()
        {
            componentHeader?.Recycle();

            onActiveSceneChangedFluidField?.Recycle();
            onSceneLoadedFluidField?.Recycle();
            onSceneUnloadedFluidField?.Recycle();
        }
        
        private void FindProperties()
        {
            propertyDebugMode = serializedObject.FindProperty("DebugMode");
            propertyOnActiveSceneChanged = serializedObject.FindProperty("OnActiveSceneChanged");
            propertyOnSceneLoaded = serializedObject.FindProperty("OnSceneLoaded");
            propertyOnSceneUnloaded = serializedObject.FindProperty("OnSceneUnloaded");
        }

        private void InitializeEditor()
        {
            FindProperties();
            root = DesignUtils.GetEditorRoot();

            componentHeader =
                FluidComponentHeader.Get()
                    .SetElementSize(ElementSize.Large)
                    .SetAccentColor(accentColor)
                    .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(SceneDirector)))
                    .SetIcon(componentIconTextures.ToList())
                    .AddManualButton()
                    .AddApiButton()
                    .AddYouTubeButton();
            
            debugModeSwitch = DesignUtils.GetDebugSwitch(propertyDebugMode);

            onActiveSceneChangedFluidField =
                FluidField.Get()
                    .AddFieldContent(DesignUtils.UnityEventField("Executed when the active Scene has changed", propertyOnActiveSceneChanged));
            
            onSceneLoadedFluidField =
                FluidField.Get()
                    .AddFieldContent(DesignUtils.UnityEventField("Executed when a Scene has loaded", propertyOnSceneLoaded));
            
            onSceneUnloadedFluidField =
                FluidField.Get()
                    .AddFieldContent(DesignUtils.UnityEventField("Executed when a Scene has unloaded", propertyOnSceneUnloaded));
        }
        
        private void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleMargins(50, -4, DesignUtils.k_Spacing2X, DesignUtils.k_Spacing2X)
                        .AddChild(DesignUtils.flexibleSpace)
                        .AddChild(debugModeSwitch)
                )
                .AddChild(onActiveSceneChangedFluidField)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(onSceneLoadedFluidField)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(onSceneUnloadedFluidField)
                .AddChild(DesignUtils.endOfLineBlock)
                ;
        }
    }
}
