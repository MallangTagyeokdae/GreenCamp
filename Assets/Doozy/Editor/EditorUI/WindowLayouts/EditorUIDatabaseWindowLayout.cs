// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Editor.EditorUI.WindowLayouts
{
    public abstract class EditorUIDatabaseWindowLayout : FluidWindowLayout
    {
        public override Color accentColor => EditorColors.EditorUI.Amber;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.EditorUI.Amber;
        
        protected FluidButton refreshDatabaseButton { get; private set; }

        protected EditorUIDatabaseWindowLayout()
        {
            sideMenu
                .SetMenuLevel(FluidSideMenu.MenuLevel.Level_2);
        }
        
        protected void InitializeRefreshDatabaseButton(string labelText, string tooltipText, EditorSelectableColorInfo selectableColor, UnityAction onClickCallback)
        {
            refreshDatabaseButton = DesignUtils.Buttons.RefreshDatabase(labelText, tooltipText, selectableColor, onClickCallback);
        }
    }
}
