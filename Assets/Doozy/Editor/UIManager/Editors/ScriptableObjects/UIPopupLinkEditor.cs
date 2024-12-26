// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Common.Editors;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.UIManager.Windows;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEditor;

namespace Doozy.Editor.UIManager.Editors.ScriptableObjects
{
    [CustomEditor(typeof(UIPopupLink), true)]
    public class UIPopupLinkEditor : PrefabLinkEditor<UIPopupLink>
    {
        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetIcon(EditorSpriteSheets.UIManager.Icons.UIPopupLink)
                .SetComponentNameText("UIPopup")
                .SetComponentTypeText("Link");

            validateButton
                .SetOnClick(() =>
                {
                    UIPopupDatabase.instance.Validate();
                    UIPopupDatabase.instance.Add(castedTarget);
                });

            databaseButton
                .SetIcon(EditorSpriteSheets.UIManager.Icons.UIPopupDatabase)
                .SetOnClick(PopupsDatabaseWindow.Open);

            // if (!EditorApplication.isPlayingOrWillChangePlaymode)
            //     UIPopupDatabase.instance.Validate();
        }
    }
}
