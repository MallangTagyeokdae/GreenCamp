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
    public class SteppersDatabaseWindowLayout : CategoryNameGroupWindowLayout, IDashboardDatabaseWindowLayout, IUIManagerDatabaseWindowLayout
    {
        public int order => 0;
        
        public override string layoutName => "Steppers";
        public override List<Texture2D> animatedIconTextures => EditorSpriteSheets.UIManager.Icons.UIStepperDatabase;
        public override Color accentColor => EditorColors.UIManager.UIComponent;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.UIComponent;

        protected override Object targetObject => UIStepperIdDatabase.instance;
        protected override UnityAction onUpdateCallback => UIStepperIdDatabase.instance.onUpdateCallback; 
        protected override CategoryNameGroup<CategoryNameItem> database => UIStepperIdDatabase.instance.database;
        protected override string groupTypeName => "Stepper";

        protected override Func<string, List<string>, bool> exportDatabaseHandler => UIStepperIdDatabase.instance.ExportRoamingDatabase;
        protected override Func<List<ScriptableObject>, bool> importDatabaseHandler => UIStepperIdDatabase.instance.ImportRoamingDatabases;
        protected override string roamingDatabaseTypeName => nameof(UIStepperIdRoamingDatabase);

        protected override UnityAction runEnumGenerator => () => UIStepperIdExtensionGenerator.Run(true, false, true);
        
        public SteppersDatabaseWindowLayout()
        {
            AddHeader("Steppers Database", "UIStepper Ids", animatedIconTextures);
            sideMenu
                .SetMenuLevel(FluidSideMenu.MenuLevel.Level_2)
                .IsCollapsable(false)
                .SetAccentColor(selectableAccentColor);
            Initialize();
        }
    }
}
