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
    public class DashboardEditorUIWindowLayout : FluidWindowLayout, IDashboardWindowLayout
    {
        
        public int order => 900;

        public override string layoutName => "Editor UI";
        public sealed override List<Texture2D> animatedIconTextures => EditorSpriteSheets.EditorUI.Icons.EditorUI;

        public override Color accentColor => EditorColors.Default.UnityThemeInversed;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Default.UnityThemeInversed;

        public DashboardEditorUIWindowLayout()
        {
            content.ResetLayout();

            sideMenu
                .SetMenuLevel(FluidSideMenu.MenuLevel.Level_1)
                .RemoveSearch()
                .IsCollapsable(true)
                .SetCustomWidth(200);

            Initialize();
            Compose();
        }

        private void Initialize()
        {
            #region Side Menu

            //get all the types that implement the IEditorUIDatabaseWindowLayout interface
            //they are used to generate the side menu buttons and to get/display the corresponding content
            IEnumerable<IEditorUIDatabaseWindowLayout> layouts =
                TypeCache.GetTypesDerivedFrom(typeof(IEditorUIDatabaseWindowLayout))               //get all the types that derive from IEditorUIDatabaseWindowLayout
                    .Select(type => (IEditorUIDatabaseWindowLayout)Activator.CreateInstance(type)) //create an instance of the type
                    .OrderBy(l => l.order)                                                         //sort the layouts by order (set in each layout's class)
                    .ThenBy(l => l.layoutName);                                                    //sort the layouts by name (set in each layout's class)
            
            
            //order indicator used to add spacing between the tabs, when the difference is greater or equal to 50
            int previousOrder = -1;
            
            //add buttons to side menu
            foreach (IEditorUIDatabaseWindowLayout l in layouts)
            {
                //INJECT SPACE
                if (l.order > 0 && l.order - previousOrder >= 50) //if the layout order difference is greater or equal than 50
                    sideMenu.AddSpaceBetweenButtons();            //add a vertical space between side menu buttons
                previousOrder = l.order;                          //keep track of the previous layout order
                
                //SIDE MENU BUTTON
                FluidToggleButtonTab sideMenuButton = sideMenu.AddButton(l.layoutName, l.selectableAccentColor);

                //ADD SIDE MENU BUTTON ICON (animated or static)
                if (l.animatedIconTextures?.Count > 0)
                    sideMenuButton.SetIcon(l.animatedIconTextures); // <<< ANIMATED ICON
                else if (l.staticIconTexture != null)
                    sideMenuButton.SetIcon(l.staticIconTexture); // <<< STATIC ICON

                //WINDOW LAYOUT (added to the content container when the button is pressed)                
                VisualElement customWindowLayout = ((VisualElement)l).SetStyleFlexGrow(1);

                sideMenuButton.SetToggleAccentColor(((IEditorUIDatabaseWindowLayout)customWindowLayout).selectableAccentColor);

                sideMenuButton.OnValueChanged += evt =>
                {
                    if (!evt.newValue) return;
                    content.Clear();
                    content.Add(customWindowLayout);
                };
            }

            #endregion
        }

        private void Compose()
        {

        }
    }
}
