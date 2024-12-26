// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.EditorUI
{
    public abstract class EditorUIEditor<Tclass> : UnityEditor.Editor where Tclass : MonoBehaviour
    {
        protected Tclass castedTarget => (Tclass)target;
        protected IEnumerable<Tclass> castedTargets => targets.Cast<Tclass>();

        protected virtual Color accentColor => EditorColors.EditorUI.Amber;
        protected virtual EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.EditorUI.Amber;

        protected VisualElement root { get; set; }
        protected FluidComponentHeader componentHeader { get; set; }
        protected VisualElement toolbarContainer { get; private set; }
        protected VisualElement contentContainer { get; private set; }
        protected FluidToggleGroup tabsGroup { get; set; }
        
        protected virtual void OnEnable() {}

        protected virtual void OnDisable() {}

        protected virtual void OnDestroy()
        {
            componentHeader?.Recycle();
        }

        public override VisualElement CreateInspectorGUI()
        {
            FindProperties();
            InitializeEditor();
            Compose();
            return root;
        }

        protected virtual void FindProperties() {}

        protected virtual void InitializeEditor()
        {
            root = DesignUtils.editorRoot;
            componentHeader =
                DesignUtils.editorComponentHeader
                    .SetAccentColor(accentColor)
                    .SetComponentNameText(typeof(Tclass).Name);
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