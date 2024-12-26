// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.Common.Layouts;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.Interfaces;
using Doozy.Editor.UIManager.Automation.Generators;
using Doozy.Editor.UIManager.ScriptableObjects;
using Doozy.Runtime.Common;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Doozy.Editor.UIManager.Layouts.Databases
{
    public class TagsDatabaseWindowLayout : CategoryNameGroupWindowLayout, IDashboardDatabaseWindowLayout, IUIManagerDatabaseWindowLayout
    {
        public int order => 0;
        
        public override string layoutName => "Tags";
        public sealed override List<Texture2D> animatedIconTextures => EditorSpriteSheets.UIManager.Icons.UITagDatabase;
        public override Color accentColor => EditorColors.UIManager.UIComponent;
        public sealed override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.UIComponent;

        protected override Object targetObject => UITagIdDatabase.instance;
        protected override UnityAction onUpdateCallback => UITagIdDatabase.instance.onUpdateCallback;
        protected override CategoryNameGroup<CategoryNameItem> database => UITagIdDatabase.instance.database;
        protected override string groupTypeName => "Tag";

        protected override Func<string, List<string>, bool> exportDatabaseHandler => UITagIdDatabase.instance.ExportRoamingDatabase;
        protected override Func<List<ScriptableObject>, bool> importDatabaseHandler => UITagIdDatabase.instance.ImportRoamingDatabases;
        protected override string roamingDatabaseTypeName => nameof(UITagIdRoamingDatabase);

        protected override UnityAction runEnumGenerator => () => UITagIdExtensionGenerator.Run(true, false, true);

        public TagsDatabaseWindowLayout()
        {
            AddHeader("Tags Database", "UITag Ids", animatedIconTextures);
            sideMenu
                .SetMenuLevel(FluidSideMenu.MenuLevel.Level_2)
                .IsCollapsable(false)
                .SetAccentColor(selectableAccentColor);
            Initialize();
        }
    }
}
