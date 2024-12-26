// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Common;
using Doozy.Runtime.UIDesigner.Utils;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using EditorStyles = Doozy.Editor.EditorUI.EditorStyles;
namespace Doozy.Editor.Common.Layouts
{
    public class PrefabLinkDatabaseItemRow : PoolableElement<PrefabLinkDatabaseItemRow>
    {
        private static Color layoutContainerNormalColor => EditorColors.Default.Background;
        private static Color layoutContainerHoverColor => EditorColors.Default.WindowHeaderBackground;
        private static Color initialContainerColor => layoutContainerNormalColor;
        private static Color textColor => EditorColors.Default.TextDescription;

        public static Font font => EditorFonts.Ubuntu.Light;

        public TemplateContainer templateContainer { get; }
        public VisualElement layoutContainer { get; }
        public VisualElement leftContainer { get; }
        public VisualElement middleContainer { get; }
        public VisualElement rightContainer { get; }

        public FluidButton buttonFind { get; }
        public FluidButton buttonDelete { get; }

        public TextField prefabNameTextField { get; set; }
        public ObjectField prefabObjectField { get; set; }

        public PrefabLink target { get; private set; }
        public SerializedObject serializedObject { get; set; }
        protected SerializedProperty propertyPrefabName { get; set; }
        protected SerializedProperty propertyPrefab { get; set; }

        public UnityAction<PrefabLink> deleteHandler { get; set; }

        public static PrefabLinkDatabaseItemRow Get
        (
            PrefabLink item,
            UnityAction<PrefabLink> deleteCallback
        )
            => Get()
                .SetTarget(item)
                .SetDeleteHandler(deleteCallback);

        public PrefabLinkDatabaseItemRow()
        {
            this.SetStyleFlexShrink(0);

            Add(templateContainer = EditorLayouts.Common.PrefabLinkDatabaseItemRow.CloneTree());

            templateContainer
                .SetStyleFlexGrow(1)
                .AddStyle(EditorStyles.Common.PrefabLinkDatabaseItemRow);

            layoutContainer = templateContainer.Q<VisualElement>(nameof(layoutContainer));
            leftContainer = layoutContainer.Q<VisualElement>(nameof(leftContainer));
            middleContainer = layoutContainer.Q<VisualElement>(nameof(middleContainer));
            rightContainer = layoutContainer.Q<VisualElement>(nameof(rightContainer));

            layoutContainer.SetStyleBackgroundColor(initialContainerColor);

            prefabNameTextField =
                new TextField()
                    .ResetLayout()
                    .SetStyleFlexGrow(1)
                    .SetStyleFlexShrink(1);
            prefabNameTextField.SetEnabled(false);

            prefabObjectField =
                new ObjectField()
                    .ResetLayout()
                    .SetStyleFlexGrow(1)
                    .SetStyleFlexShrink(1);
            prefabObjectField.SetEnabled(false);

            buttonFind = NewButtonFind();
            buttonDelete = NewButtonDelete();

            buttonDelete.SetOnClick(() =>
            {
                if (deleteHandler == null) throw new NullReferenceException(nameof(deleteHandler));
                deleteHandler.Invoke(target);
            });

            middleContainer
                .AddChild(prefabNameTextField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(prefabObjectField);

            rightContainer
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(buttonFind)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(buttonDelete);
        }

        public override void Reset()
        {
            this.Unbind();

            target = null;

            serializedObject = null;
            propertyPrefabName = null;
            propertyPrefab = null;

            deleteHandler = null;

            buttonFind
                .ClearOnClick()
                .SetTooltip(string.Empty);
        }

        public PrefabLinkDatabaseItemRow SetTarget(PrefabLink link)
        {
            Reset();
            if (link == null) return this;
            target = link;
            serializedObject = new SerializedObject(link);
            propertyPrefabName = serializedObject.FindProperty("PrefabName");
            propertyPrefab = serializedObject.FindProperty("Prefab");

            prefabNameTextField.BindToProperty(propertyPrefabName);
            prefabObjectField.BindToProperty(propertyPrefab);
            this.Bind(serializedObject);
            serializedObject.Update();

            string assetPath = AssetDatabase.GetAssetPath(link);
            bool selectOnFind = false;
            buttonFind
                .SetTooltip(assetPath)
                .SetOnClick(() =>
                {
                    EditorGUIUtility.PingObject(link);

                    if (selectOnFind)
                    {
                        Selection.activeObject = link;
                        selectOnFind = false;
                        return;
                    }

                    selectOnFind = true;
                });

            return this;
        }

        public PrefabLinkDatabaseItemRow SetDeleteHandler(UnityAction<PrefabLink> deleteCallback)
        {
            deleteHandler = deleteCallback;
            return this;
        }

        private static FluidButton NewButtonFind() =>
            FluidButton.Get()
                .SetStyleFlexShrink(0)
                .SetButtonStyle(ButtonStyle.Contained)
                .SetElementSize(ElementSize.Tiny)
                .SetIcon(EditorSpriteSheets.EditorUI.Icons.Location)
                .SetAccentColor(EditorSelectableColors.Default.Action)
                .SetLabelText("Find");

        private static FluidButton NewButtonDelete() =>
            FluidButton.Get()
                .SetStyleFlexShrink(0)
                .SetButtonStyle(ButtonStyle.Contained)
                .SetElementSize(ElementSize.Tiny)
                .SetIcon(EditorSpriteSheets.EditorUI.Icons.Close)
                .SetAccentColor(EditorSelectableColors.Default.Remove)
                .SetLabelText("Delete")
                .SetTooltip("Remove this link from the database and delete the asset file");
    }
}
