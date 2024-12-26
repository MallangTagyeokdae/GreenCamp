// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.Interfaces;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace Doozy.Editor.Dashboard.WindowsLayouts
{
    // ReSharper disable once UnusedType.Global
    public class DashboardSettingsWindowLayout : FluidWindowLayout, IDashboardWindowLayout
    {
        public int order => 890;
        
        public override string layoutName => "Settings";
        public sealed override List<Texture2D> animatedIconTextures => EditorSpriteSheets.EditorUI.Icons.Settings;

        public override Color accentColor => EditorColors.Default.UnityThemeInversed;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Default.UnityThemeInversed;
        
        public DashboardSettingsWindowLayout()
        {
            content.ResetLayout();
           
            sideMenu.RemoveSearch();
            sideMenu.IsCollapsable(true);
            sideMenu.SetCustomWidth(200);
            
            Initialize();
            Compose();
        }

        private void Initialize()
        {
             #region Side Menu

            //get all the types that implement the IDashboardSettingsWindowLayout interface
            //they are used to generate the side menu buttons and to get/display the corresponding content
            IEnumerable<IDashboardSettingsWindowLayout> layouts =
                TypeCache.GetTypesDerivedFrom(typeof(IDashboardSettingsWindowLayout))               //get all the types that derive from IDashboardSettingsWindowLayout
                    .Select(type => (IDashboardSettingsWindowLayout)Activator.CreateInstance(type)) //create an instance of the type
                    .OrderBy(l => l.order)                                                          //sort the layouts by order (set in each layout's class)
                    .ThenBy(l => l.layoutName);                                                     //sort the layouts by name (set in each layout's class)


            //add buttons to side menu
            foreach (IDashboardSettingsWindowLayout l in layouts)
            {
                //SIDE MENU BUTTON
                FluidToggleButtonTab sideMenuButton = sideMenu.AddButton(l.layoutName, l.selectableAccentColor);

                //ADD SIDE MENU BUTTON ICON (animated or static)
                if (l.animatedIconTextures?.Count > 0)
                    sideMenuButton.SetIcon(l.animatedIconTextures); // <<< ANIMATED ICON
                else if (l.staticIconTexture != null)
                    sideMenuButton.SetIcon(l.staticIconTexture); // <<< STATIC ICON

                //WINDOW LAYOUT (added to the content container when the button is pressed)                
                VisualElement customWindowLayout = ((VisualElement)l).SetStyleFlexGrow(1);

                sideMenuButton.SetToggleAccentColor(((IDashboardSettingsWindowLayout)customWindowLayout).selectableAccentColor);

                sideMenuButton.OnValueChanged += evt =>
                {
                    if (!evt.newValue) return;
                    content.Clear();
                    content.Add(customWindowLayout);
                };
            }

            #endregion
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void Compose()
        {
            
        }
    }
}
