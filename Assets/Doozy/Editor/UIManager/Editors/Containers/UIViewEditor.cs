// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIManager.Editors.Containers.Internal;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UIView = Doozy.Runtime.UIManager.Containers.UIView;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Doozy.Editor.UIManager.Editors.Containers
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UIView), true)]
    public class UIViewEditor : BaseUIContainerEditor
    {
        public UIView castedTarget => (UIView)target;
        public IEnumerable<UIView> castedTargets => targets.Cast<UIView>();

        private FluidField idField { get; set; }

        protected SerializedProperty propertyId { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            idField?.Recycle();
        }

        protected override void FindProperties()
        {
            base.FindProperties();
            propertyId = serializedObject.FindProperty(nameof(UIView.Id));
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText("UIView")
                .SetIcon(EditorSpriteSheets.UIManager.Icons.UIView)
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048281106/UIView?atlOrigin=eyJpIjoiMGIxNThlOTZjNTA3NDIyOWI3NWMzNTQ3MWZkYjE5ZTYiLCJwIjoiYyJ9")
                .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.UIManager.Containers.UIView.html")
                .AddYouTubeButton();

            idField = FluidField.Get().AddFieldContent(DesignUtils.NewPropertyField(propertyId));
        }

        protected override VisualElement Toolbar()
        {
            return
                toolbarContainer
                    .AddChild(settingsTab)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(callbacksTab)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(progressorsTab)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(DesignUtils.flexibleSpace)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(DesignUtils.SystemButton_RenameComponent
                        (
                            castedTarget.gameObject, () => $"View - {castedTarget.Id.Name}"
                        )
                    )
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild
                    (
                        DesignUtils.SystemButton_SortComponents
                        (
                            castedContainer.gameObject,
                            nameof(RectTransform),
                            nameof(UIView)
                        )
                    );
        }

        protected override void Compose()
        {
            root
                .AddChild(reactionControls)
                .AddChild(componentHeader)
                .AddChild(Toolbar())
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(Content())
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(idField)
                .AddChild(DesignUtils.endOfLineBlock);
        }
    }
}
