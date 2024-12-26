// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.Common;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.EditorUI.Windows.Internal;
using Doozy.Editor.Interfaces;
using Doozy.Editor.Reactor.Internal;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Easings;
using Doozy.Runtime.Reactor.Extensions;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Dashboard.Windows
{
    public class DoozyDashboardWindow : FluidWindow<DoozyDashboardWindow>
    {
        private const string WINDOW_TITLE = "Doozy Dashboard";
        public const string k_WindowMenuPath = "Tools/Doozy/Dashboard";

        [MenuItem(k_WindowMenuPath, priority = -2000)]
        public static void Open() => InternalOpenWindow(WINDOW_TITLE);

        private TemplateContainer templateContainer { get; set; }
        private VisualElement layoutContainer { get; set; }
        private VisualElement sideMenuContainer { get; set; }
        private VisualElement contentContainer { get; set; }

        private const float MAX_ICON_SIZE = 64f;
        private const float MIN_ICON_SIZE = 32f;

        private string selectedTab { get; set; }
        private string selectedTabKey => EditorPrefsKey($"{nameof(selectedTab)}");

        private static IEnumerable<Texture2D> doozyIconTextures => EditorSpriteSheets.UIManager.Icons.UIManagerIcon;
        private Image doozyIconImage { get; set; }
        private Texture2DReaction doozyIconImageReaction { get; set; }

        private ScrollView contentScrollView { get; set; }
        private VisualElement itemsContainer { get; set; }

        private FluidSideMenu sideMenu { get; set; }
        private string sideMenuWidthKey => EditorPrefsKey($"{nameof(sideMenu)}.{nameof(sideMenu.customWidth)}");
        private string sideMenuIsCollapsedKey => EditorPrefsKey($"{nameof(sideMenu)}.{nameof(sideMenu.isCollapsed)}");
        private FluidResizer sideMenuResizer { get; set; }

        private VisualElement footerContainer { get; set; }
        private FluidPlaceholder emptyPlaceholder { get; set; }

        private ProductInfo m_DoozyProductInfo;
        private ProductInfo doozyProductInfo => m_DoozyProductInfo ??= m_DoozyProductInfo = ProductInfo.Get("Doozy UI Manager");

        // protected override void OnEnable()
        // {
        //     base.OnEnable();
        //     minSize = new Vector2(1200, 673);
        // }

        protected override void CreateGUI()
        {
            Initialize();
            Compose();
        }

        private void Initialize()
        {
            root
                .RecycleAndClear()
                .Add(templateContainer = EditorLayouts.UIManager.DoozyDashboardWindow.CloneTree());

            templateContainer
                .SetStyleFlexGrow(1)
                .AddStyle(EditorUI.EditorStyles.UIManager.DoozyDashboardWindow);

            layoutContainer = templateContainer.Q<VisualElement>(nameof(layoutContainer));
            sideMenuContainer = layoutContainer.Q<VisualElement>(nameof(sideMenuContainer));
            contentContainer = layoutContainer.Q<VisualElement>(nameof(contentContainer));

            InitializeDoozyIcon();
            InitializeSideMenu();
            InitializeContent();
            InitializeFooter();
        }

        private void InitializeDoozyIcon()
        {
            doozyIconImage =
                new Image()
                    .SetStyleFlexShrink(0)
                    .SetStyleMargins(DesignUtils.k_Spacing)
                    .SetStyleAlignSelf(Align.Center);

            doozyIconImageReaction =
                doozyIconImage
                    .GetTexture2DReaction(doozyIconTextures)
                    .SetEditorHeartbeat();

            root.schedule.Execute(() => UpdateDoozyIconSize(EditorPrefs.GetBool(sideMenuIsCollapsedKey, false) ? MIN_ICON_SIZE : sideMenu.customWidth));

            doozyIconImage.RegisterCallback<PointerEnterEvent>(evt => doozyIconImageReaction?.Play());
            doozyIconImage.AddManipulator(new Clickable(() => doozyIconImageReaction?.Play()));
        }

        private void UpdateDoozyIconSize(float size)
        {
            size = Mathf.Max(MIN_ICON_SIZE, size);
            size = Mathf.Min(MAX_ICON_SIZE, size);
            doozyIconImage.SetStyleSize(size);
        }

        private void InitializeSideMenu()
        {
            //setup side menu
            sideMenu =
                new FluidSideMenu()
                    .SetMenuLevel(FluidSideMenu.MenuLevel.Level_0)
                    .IsCollapsable(true)
                    .SetCustomWidth(EditorPrefs.GetInt(sideMenuWidthKey, 200));

            bool sideMenuIsCollapsed = EditorPrefs.GetBool(sideMenuIsCollapsedKey, false);
            //update side menu collapse state
            if (sideMenu.isCollapsable) sideMenu.ToggleMenu(!sideMenuIsCollapsed, false);
            sideMenu.OnCollapse += () => EditorPrefs.SetBool(sideMenuIsCollapsedKey, true);
            sideMenu.OnExpand += () => EditorPrefs.SetBool(sideMenuIsCollapsedKey, false);

            //add doozy icon to the side menu header
            sideMenu.headerContainer
                .SetStyleDisplay(DisplayStyle.Flex)
                .AddChild(doozyIconImage);

            //connect the doozy icon size to the side menu expand/collapse reaction
            sideMenu.expandCollapseReaction.AddOnUpdateCallback(() =>
            {
                float currentValue = sideMenu.expandCollapseReaction.currentValue;
                float size = Mathf.LerpUnclamped(MIN_ICON_SIZE, sideMenu.customWidth, currentValue);
                UpdateDoozyIconSize(size);
            });

            //setup side menu resizer
            sideMenuResizer = new FluidResizer(FluidResizer.Position.Right);
            sideMenuResizer.onPointerMoveEvent += evt =>
            {
                if (sideMenu.isCollapsed) return;
                int delta = (int)(sideMenu.customWidth + evt.deltaPosition.x);
                sideMenu.SetCustomWidth(delta);
                UpdateDoozyIconSize(delta);
            };
            sideMenuResizer.onPointerUp += evt =>
            {
                if (sideMenu.isCollapsed) return;
                EditorPrefs.SetInt(sideMenuWidthKey, sideMenu.customWidth);
            };

            //add the menu and the resizer to the side menu container
            sideMenuContainer.Add
            (
                DesignUtils.row
                    .AddChild(sideMenu)
                    .AddChild(sideMenuResizer)
            );

            //get all the types that implement the IDashboardWindowLayout interface
            //they are used to generate the side menu buttons and to get/display the corresponding content
            IEnumerable<IDashboardWindowLayout> layouts =
                TypeCache.GetTypesDerivedFrom(typeof(IDashboardWindowLayout))               //get all the types that derive from IDashboardWindowLayout
                    .Select(type => (IDashboardWindowLayout)Activator.CreateInstance(type)) //create an instance of the type
                    .OrderBy(layout => layout.order)                                        //sort the layouts by order (set in each layout's class)
                    .ThenBy(layout => layout.layoutName);                                   //sort the layouts by name (set in each layout's class)


            //get the previously selected layout name
            string previouslySelectedLayoutName = EditorPrefs.GetString(selectedTabKey, string.Empty);
            //the previously selected tab reference
            FluidToggleButtonTab previouslySelectedTab = null;
            //order indicator used to add spacing between the tabs, when the difference is greater or equal to 50
            int previousOrder = -1;

            //add buttons to side menu
            foreach (IDashboardWindowLayout l in layouts)
            {
                //INJECT SPACE
                if (l.order > 0 && l.order - previousOrder >= 50) //if the layout order difference is greater or equal than 50
                    sideMenu.AddSpaceBetweenButtons();            //add a vertical space between side menu buttons
                previousOrder = l.order;                          //keep track of the previous layout order

                //SIDE MENU BUTTON
                FluidToggleButtonTab sideMenuButton = sideMenu.AddButton(l.layoutName, l.selectableAccentColor);

                if (!previouslySelectedLayoutName.IsNullOrEmpty() &&   //if a layout was previously selected
                    l.layoutName.Equals(previouslySelectedLayoutName)) //and the current layout is the same as the previously selected layout
                    previouslySelectedTab = sideMenuButton;            //set the previously selected tab to the current one

                //ADD SIDE MENU BUTTON ICON (animated or static)
                if (l.animatedIconTextures?.Count > 0)
                    sideMenuButton.SetIcon(l.animatedIconTextures); // <<< ANIMATED ICON
                else if (l.staticIconTexture != null)
                    sideMenuButton.SetIcon(l.staticIconTexture); // <<< STATIC ICON

                //WINDOW LAYOUT (added to the content container when the button is pressed)                
                VisualElement customWindowLayout = ((VisualElement)l).SetStyleFlexGrow(1);

                sideMenuButton.SetToggleAccentColor(((IDashboardWindowLayout)customWindowLayout).selectableAccentColor);

                //SIDE MENU BUTTON - ON VALUE CHANGED
                //show the appropriate window layout when the value of the side menu button changes
                sideMenuButton.OnValueChanged += evt =>
                {
                    if (!evt.newValue) return;
                    contentContainer.Clear();
                    contentContainer.Add(customWindowLayout);
                    EditorPrefs.SetString(selectedTabKey, l.layoutName);
                };
            }

            //select the previously selected tab (if any)
            root.schedule.Execute(() => previouslySelectedTab?.SetIsOn(true));
        }

        private void InitializeContent()
        {
            contentScrollView = new ScrollView().SetStyleFlexGrow(1);
            contentContainer
                .AddChild(contentScrollView);
        }

        private void InitializeFooter()
        {
            footerContainer =
                DesignUtils.row
                    .SetStyleFlexShrink(0)
                    .SetStyleFlexGrow(0)
                    .SetStyleAlignItems(Align.Center)
                    .SetStyleBackgroundColor(EditorColors.Default.BoxBackground)
                    .SetStylePaddingLeft(DesignUtils.k_Spacing2X)
                    .SetStylePaddingRight(DesignUtils.k_Spacing2X)
                    .SetStylePaddingTop(DesignUtils.k_Spacing)
                    .SetStylePaddingBottom(DesignUtils.k_Spacing)
                ;

            FluidButton GetSocialButton
            (
                List<Texture2D> iconTextures,
                EditorSelectableColorInfo selectableColorInfo,
                string tooltip,
                string url
            )
                => FluidButton.Get()
                    .SetStyleFlexShrink(0)
                    .SetIcon(iconTextures)
                    .SetAccentColor(selectableColorInfo)
                    .SetTooltip(tooltip)
                    .SetOnClick(() => Application.OpenURL(url));

            var youtubeButton =
                GetSocialButton
                (
                    EditorSpriteSheets.EditorUI.Icons.Youtube,
                    EditorSelectableColors.Brands.YouTube,
                    "YouTube",
                    "https://youtube.doozyui.com"
                );

            var twitterButton =
                GetSocialButton
                (
                    EditorSpriteSheets.EditorUI.Icons.Twitter,
                    EditorSelectableColors.Brands.Twitter,
                    "Twitter",
                    "https://twitter.doozyui.com"
                );

            var facebookButton =
                GetSocialButton
                (
                    EditorSpriteSheets.EditorUI.Icons.Facebook,
                    EditorSelectableColors.Brands.Facebook,
                    "Facebook",
                    "https://facebook.doozyui.com"
                );

            var discordButton =
                GetSocialButton
                (
                    EditorSpriteSheets.EditorUI.Icons.Discord,
                    EditorSelectableColors.Brands.Discord,
                    "Discord",
                    "https://discord.doozyui.com"
                );

            var doozyWebsiteButton =
                FluidButton.Get("doozyui.com")
                    .SetStyleFlexShrink(0)
                    .SetOnClick(() => Application.OpenURL("https://doozyui.com"));

            var doozyUiVersionLabel =
                DesignUtils.fieldLabel
                    .SetStyleFlexShrink(0)
                    .SetText(doozyProductInfo.nameAndVersion)
                    .SetStyleTextAlign(TextAnchor.MiddleRight)
                    .SetStyleFontSize(11);

            footerContainer
                .AddChild(youtubeButton)
                .AddChild(twitterButton)
                .AddChild(facebookButton)
                .AddChild(discordButton)
                .AddFlexibleSpace()
                .AddChild(doozyWebsiteButton)
                .AddFlexibleSpace()
                .AddChild(doozyUiVersionLabel)
                ;
        }

        private void Compose()
        {
            root.AddChild(footerContainer);
        }

    }
}
