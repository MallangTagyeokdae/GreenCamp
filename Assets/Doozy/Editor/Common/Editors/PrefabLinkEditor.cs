// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Common;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Common.Editors
{
    public abstract class PrefabLinkEditor<T> : UnityEditor.Editor where T : PrefabLink
    {
        protected T castedTarget => (T)target;
        protected IEnumerable<T> castedTargets => targets.Cast<T>();

        protected virtual Color accentColor => EditorColors.UIManager.DataComponent;
        protected virtual EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.DataComponent;

        protected VisualElement root { get; set; }
        protected FluidComponentHeader componentHeader { get; set; }
        protected VisualElement toolbarContainer { get; private set; }
        protected VisualElement contentContainer { get; private set; }
        protected FluidToggleGroup tabsGroup { get; set; }

        protected FluidButton validateButton { get; set; }
        protected FluidButton databaseButton { get; set; }

        protected FluidField prefabNameFluidField { get; set; }
        protected FluidField prefabFluidField { get; set; }

        protected TextField prefabNameTextField { get; set; }
        protected ObjectField prefabObjectField { get; set; }

        protected SerializedProperty propertyPrefabName { get; set; }
        protected SerializedProperty propertyPrefab { get; set; }

        protected virtual void OnEnable() {}

        protected virtual void OnDisable() {}

        protected virtual void OnDestroy()
        {
            componentHeader?.Recycle();
            validateButton?.Recycle();
            databaseButton?.Recycle();
            prefabNameFluidField?.Recycle();
            prefabFluidField?.Recycle();
        }

        public override VisualElement CreateInspectorGUI()
        {
            FindProperties();
            InitializeEditor();
            Compose();
            return root;
        }

        protected virtual void FindProperties()
        {
            propertyPrefabName = serializedObject.FindProperty("PrefabName");
            propertyPrefab = serializedObject.FindProperty("Prefab");
        }

        protected virtual void InitializeEditor()
        {
            root = DesignUtils.editorRoot;
            componentHeader =
                DesignUtils.editorComponentHeader
                    .SetAccentColor(accentColor)
                    .SetComponentNameText(typeof(T).Name);

            toolbarContainer = DesignUtils.editorToolbarContainer;
            tabsGroup = FluidToggleGroup.Get().SetControlMode(FluidToggleGroup.ControlMode.OneToggleOn);
            contentContainer = DesignUtils.editorContentContainer;

            validateButton =
                FluidButton.Get("Validate", EditorSpriteSheets.EditorUI.Icons.Atom)
                    .SetTooltip("Validate link settings and add to database")
                    .SetButtonStyle(ButtonStyle.Contained)
                    .SetElementSize(ElementSize.Small);

            databaseButton =
                FluidButton.Get("Database", EditorSpriteSheets.EditorUI.Icons.GenericDatabase)
                    .SetTooltip("Open the Database window")
                    .SetButtonStyle(ButtonStyle.Contained)
                    .SetElementSize(ElementSize.Small);

            prefabNameTextField = DesignUtils.NewTextField(propertyPrefabName).SetStyleFlexGrow(1);
            prefabNameTextField.SetEnabled(false);
            prefabNameFluidField = FluidField.Get("Name").AddFieldContent(prefabNameTextField);

            prefabObjectField = DesignUtils.NewObjectField(propertyPrefab, typeof(GameObject)).SetStyleFlexGrow(1);
            prefabObjectField.RegisterValueChangedCallback(evt =>
            {
                if (evt == null) return;
                root.schedule.Execute(() =>
                {
                    if (target == null) return;
                    validateButton.OnClick?.Invoke();
                });
            });
            prefabFluidField = FluidField.Get("Prefab").AddFieldContent(prefabObjectField);

            // root.schedule.Execute(() => validateButton.OnClick?.Invoke());
        }

        protected virtual VisualElement Toolbar()
        {
            return
                toolbarContainer;
        }

        protected virtual VisualElement Content()
        {
            return
                contentContainer
                    .AddChild(prefabNameFluidField)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(prefabFluidField)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild
                    (
                        DesignUtils.row
                            .AddChild(DesignUtils.flexibleSpace)
                            .AddChild(validateButton)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(databaseButton)
                            .AddChild(DesignUtils.flexibleSpace)
                    )
                ;
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
