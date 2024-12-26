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
using Doozy.Editor.Reactor.Automation.Generators;
using Doozy.Editor.Reactor.ScriptableObjects;
using Doozy.Editor.UIManager;
using Doozy.Editor.UIManager.Automation.Generators;
using Doozy.Editor.UIManager.ScriptableObjects;
using Doozy.Runtime.Common;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace Doozy.Editor.Reactor.Layouts.Databases
{
    public sealed class ProgressorsDatabaseWindowLayout : CategoryNameGroupWindowLayout, IDashboardDatabaseWindowLayout, IUIManagerDatabaseWindowLayout
    {
        public int order => 0;
        
        public override string layoutName => "Progressors";
        public override List<Texture2D> animatedIconTextures => EditorSpriteSheets.Reactor.Icons.ProgressorDatabase;
        public override Color accentColor => EditorColors.Reactor.Red;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Reactor.Red;

        protected override Object targetObject => ProgressorIdDatabase.instance;
        protected override UnityAction onUpdateCallback => ProgressorIdDatabase.instance.onUpdateCallback; 
        protected override CategoryNameGroup<CategoryNameItem> database => ProgressorIdDatabase.instance.database;
        protected override string groupTypeName => "Progressor";

        protected override Func<string, List<string>, bool> exportDatabaseHandler => ProgressorIdDatabase.instance.ExportRoamingDatabase;
        protected override Func<List<ScriptableObject>, bool> importDatabaseHandler => ProgressorIdDatabase.instance.ImportRoamingDatabases;
        protected override string roamingDatabaseTypeName => nameof(ProgressorIdRoamingDatabase);

        protected override UnityAction runEnumGenerator => () => ProgressorIdExtensionGenerator.Run(true, false, true);
        
        public ProgressorsDatabaseWindowLayout()
        {
            AddHeader("Progressors Database", "Progressor Ids", animatedIconTextures);
            sideMenu
                .SetMenuLevel(FluidSideMenu.MenuLevel.Level_2)
                .IsCollapsable(false)
                .SetAccentColor(selectableAccentColor);
            Initialize();
        }
    }
}
