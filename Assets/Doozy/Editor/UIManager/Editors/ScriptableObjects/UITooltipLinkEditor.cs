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
    [CustomEditor(typeof(UITooltipLink), true)]
    public class UITooltipLinkEditor : PrefabLinkEditor<UITooltipLink>
    {
        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetIcon(EditorSpriteSheets.UIManager.Icons.UITooltipLink)
                .SetComponentNameText("UITooltip")
                .SetComponentTypeText("Link");

            validateButton
                .SetOnClick(() =>
                {
                    UITooltipDatabase.instance.Validate();
                    UITooltipDatabase.instance.Add(castedTarget);
                });

            databaseButton
                .SetIcon(EditorSpriteSheets.UIManager.Icons.UITooltipDatabase)
                .SetOnClick(TooltipsDatabaseWindow.Open);
            
            // if (!EditorApplication.isPlayingOrWillChangePlaymode)
            //     UIPopupDatabase.instance.Validate();
        }
    }
}
