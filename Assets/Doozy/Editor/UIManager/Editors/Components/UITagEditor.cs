// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIManager.Editors.Components.Internal;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Animators;
using Doozy.Runtime.UIManager.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Components
{
    [CustomEditor(typeof(UITag), true)]
    [CanEditMultipleObjects]
    public class UITagEditor : UnityEditor.Editor
    {
        private Color accentColor => EditorColors.UIManager.UIComponent;
        private EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.UIComponent;

        public UITag castedTarget => (UITag)target;
        public IEnumerable<UITag> castedTargets => targets.Cast<UITag>();

        protected VisualElement root { get; set; }
        protected FluidComponentHeader componentHeader { get; set; }

        private FluidField idField { get; set; }

        private SerializedProperty propertyId { get; set; }

        private void OnDestroy()
        {
            componentHeader?.Recycle();
        }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        private void FindProperties()
        {
            propertyId = serializedObject.FindProperty(nameof(UITag.Id));
        }

        private void InitializeEditor()
        {
            FindProperties();
            root = DesignUtils.GetEditorRoot();
            componentHeader =
                DesignUtils.editorComponentHeader
                    .SetAccentColor(accentColor)
                    .SetComponentNameText("UITag")
                    .SetIcon(EditorSpriteSheets.UIManager.Icons.UITag)
                    .AddManualButton()
                    .AddApiButton()
                    .AddYouTubeButton();

            idField =
                FluidField.Get()
                    .AddFieldContent(DesignUtils.NewPropertyField(propertyId));
        }

        private void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(idField)
                .AddChild(DesignUtils.endOfLineBlock);
        }
    }
}
