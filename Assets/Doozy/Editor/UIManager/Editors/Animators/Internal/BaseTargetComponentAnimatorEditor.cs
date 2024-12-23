// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Animators.Internal
{
    public abstract class BaseTargetComponentAnimatorEditor : UnityEditor.Editor
    {
        protected virtual Color accentColor => EditorColors.Reactor.Red;
        protected virtual EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Reactor.Red;

        public static IEnumerable<Texture2D> reactorIconTextures => EditorSpriteSheets.Reactor.Icons.ReactorIcon;
        
        protected VisualElement root { get; set; }
        protected FluidComponentHeader componentHeader { get; set; }
        protected VisualElement toolbarContainer { get; private set; }
        protected VisualElement contentContainer { get; private set; }
        
        protected SerializedProperty propertyController { get; set; }
        
        protected FluidToggleGroup tabsGroup { get; set; }

        protected virtual void OnEnable() {}
        
        protected virtual void OnDisable() {}
        
        protected virtual void OnDestroy()
        {
            componentHeader?.Recycle();
            tabsGroup?.Recycle();
        }
        
        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        protected virtual void FindProperties()
        {
            propertyController = serializedObject.FindProperty("Controller");
        }

        protected virtual void InitializeEditor()
        {
            FindProperties();
            root = DesignUtils.GetEditorRoot();
            componentHeader =
                DesignUtils.editorComponentHeader
                    .SetAccentColor(accentColor);
            toolbarContainer = DesignUtils.editorToolbarContainer;
            tabsGroup = FluidToggleGroup.Get().SetControlMode(FluidToggleGroup.ControlMode.OneToggleOn);
            contentContainer = DesignUtils.editorContentContainer;
        }

        protected virtual VisualElement Toolbar()
        {
            return
                toolbarContainer;
        }
        
        protected virtual VisualElement Content()
        {
            return
                contentContainer;
        }
        
        protected virtual void Compose()
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
