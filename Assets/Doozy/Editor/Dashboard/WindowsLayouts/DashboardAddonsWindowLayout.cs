// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.Common.Addons;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Interfaces;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.UIElements;
namespace Doozy.Editor.Dashboard.WindowsLayouts
{
    public sealed class DashboardAddonsWindowLayout : FluidWindowLayout, IDashboardWindowLayout
    {
        public int order => 1000;

        public override string layoutName => "Addons";
        public sealed override List<Texture2D> animatedIconTextures => EditorSpriteSheets.EditorUI.Icons.Store;

        public override Color accentColor => EditorColors.EditorUI.Amber;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.EditorUI.Amber;

        private float spacing2X => spacing * 2f;

        private static List<AddonInfo> database => AddonInfoDatabase.instance.Database;
        
        private ScrollView contentScrollView { get; set; }
        private VisualElement itemsContainer { get; set; }

        public DashboardAddonsWindowLayout()
        {
            // AddHeader("Addons", "Widgets, UI FX, UI Packs and more...", animatedIconTextures);
            content.ResetLayout();

            contentScrollView = new ScrollView();
            contentScrollView.contentContainer.SetStylePadding(spacing2X);
            itemsContainer =
                new VisualElement()
                // .SetStyleFlexDirection(FlexDirection.Row)
                // .SetStyleFlexWrap(Wrap.Wrap)
                // .SetStyleAlignContent(Align.FlexStart)
                // .SetStyleAlignItems(Align.FlexStart)
                // .SetStyleAlignSelf(Align.FlexStart)
                ;

            sideMenu.RemoveSearch();
            sideMenu.IsCollapsable(true);
            sideMenu.SetCustomWidth(200);

            Initialize();
            Compose();
        }


        private void Initialize()
        {
            #region Side Menu

            foreach (string addonCategory in database.Select(item => item.AddonCategory).Distinct())
            {
                //SIDE MENU BUTTON
                FluidToggleButtonTab sideMenuButton = sideMenu.AddButton(addonCategory, selectableAccentColor);

                //Try to get the proper icon for it
                string cleanAddonCategory = addonCategory.RemoveWhitespaces().RemoveAllSpecialCharacters();
                List<Texture2D> categoryIcon = EditorSpriteSheets.EditorUI.Icons.Prefab;
                foreach (EditorSpriteSheets.UIManager.UIMenu.SpriteSheetName sheetName in Enum.GetValues(typeof(EditorSpriteSheets.UIManager.UIMenu.SpriteSheetName)))
                {
                    if (!sheetName.ToString().Equals(cleanAddonCategory)) continue;
                    categoryIcon = EditorSpriteSheets.UIManager.UIMenu.GetTextures(sheetName);
                }
                sideMenuButton.SetIcon(categoryIcon);
                sideMenuButton.iconReaction.SetDuration(0.8f);

                sideMenuButton.OnValueChanged += evt =>
                {
                    if (!evt.newValue) return;
                    itemsContainer.RecycleAndClear();
                    foreach (AddonInfo item in database.Where(info => info.AddonCategory.Equals(addonCategory)))
                        itemsContainer
                            .AddChild(GetItem(item))
                            .AddChild(DesignUtils.spaceBlock2X);
                };
            }

            #endregion
        }

        private static VisualElement GetItem(AddonInfo info)
        {
            const float borderRadius = DesignUtils.k_Spacing4X;
            VisualElement element =
                new VisualElement()
                    .SetName($"Item: {info.AddonCategory} - {info.AddonName}")
                    .SetStyleFlexGrow(1)
                    .SetStyleFlexShrink(0)
                    .SetStyleOverflow(Overflow.Hidden)
                    .SetStylePadding(DesignUtils.k_Spacing)
                    .SetStyleBackgroundColor(EditorColors.Default.BoxBackground)
                    .SetStyleBorderRadius(borderRadius);

            var content =
                new VisualElement()
                    .SetStyleFlexDirection(FlexDirection.Row)
                    .SetName("Content");

            const float imageWidth = 420f;
            const float imageHeight = 280f;
            Image addonImage =
                new Image()
                    .SetName("Addon Image")
                    .SetStyleFlexShrink(0)
                    .SetStyleSize(imageWidth, imageHeight)
                    .SetStyleBorderRadius(borderRadius, 0, 0, borderRadius)
                    .SetStyleBackgroundImage(info.AddonImage)
                    .SetStyleBackgroundScaleMode(ScaleMode.ScaleToFit);

            var infoContainer =
                new VisualElement()
                    .SetName("Info Container")
                    .SetStyleFlexGrow(1)
                    .SetStylePadding(8)
                    .SetStyleMinWidth(402);

            var titleContainer =
                new VisualElement()
                    .SetName("Title Container")
                    .SetStyleFlexDirection(FlexDirection.Row)
                    .SetStyleAlignItems(Align.Center);

            var categoryLabel =
                DesignUtils.fieldLabel
                    .SetName($"Category Label - {info.AddonCategory}")
                    .SetStyleFontSize(16)
                    .SetStyleColor(EditorColors.Default.TextSubtitle)
                    .SetText(info.AddonCategory);

            var nameLabel =
                DesignUtils.fieldLabel
                    .SetName($"Name Label - {info.AddonName}")
                    .SetStyleFontSize(20)
                    .SetStyleColor(EditorColors.Default.TextTitle)
                    .SetText(info.AddonName);

            var descriptionLabel =
                DesignUtils.fieldLabel
                    .SetName($"Description Label")
                    .SetStyleFontSize(11)
                    .SetStyleColor(EditorColors.Default.TextDescription)
                    .SetWhiteSpace(WhiteSpace.Normal)
                    .SetText(info.AddonDescription);


            var buttonsContainer =
                new VisualElement()
                    .SetStyleAlignItems(Align.FlexEnd)
                    .SetStyleFlexDirection(FlexDirection.Row);

            bool hasYouTubePresentationButton = !string.IsNullOrEmpty(info.YouTubePresentationURL);
            bool hasYouTubeTutorialButton = !string.IsNullOrEmpty(info.YouTubeTutorialURL);
            bool hasManualButton = !string.IsNullOrEmpty(info.ManualURL);
            bool hasDoozyButton = !info.DoozyURL.IsNullOrEmpty();
            bool hasAssetStoreButton = !info.UnityAssetStoreURL.IsNullOrEmpty();

            //Add buttons
            FluidButton GetButton
            (
                string label,
                List<Texture2D> iconTextures,
                string url,
                EditorSelectableColorInfo selectableAccentColor
            )
                => FluidButton.Get(label)
                    .SetIcon(iconTextures)
                    .SetStyleFlexShrink(0)
                    .SetStyleMarginLeft(DesignUtils.k_Spacing)
                    .SetStyleMarginRight(DesignUtils.k_Spacing)
                    .SetButtonStyle(ButtonStyle.Outline)
                    .SetElementSize(ElementSize.Small)
                    .SetAccentColor(selectableAccentColor)
                    .SetTooltip(url)
                    .SetOnClick(() => Application.OpenURL(url));

            //YouTube Presentation
            if (hasYouTubePresentationButton)
                buttonsContainer
                    .AddChild
                    (
                        GetButton
                        (
                            "YouTube",
                            EditorSpriteSheets.EditorUI.Icons.Youtube,
                            info.YouTubePresentationURL,
                            EditorSelectableColors.Default.UnityThemeInversed
                        )
                    );

            //YouTube Tutorial
            if (hasYouTubeTutorialButton)
                buttonsContainer
                    .AddChild
                    (
                        GetButton
                        (
                            "Tutorial",
                            EditorSpriteSheets.EditorUI.Icons.Youtube,
                            info.YouTubeTutorialURL,
                            EditorSelectableColors.Default.UnityThemeInversed
                        )
                    );

            //Manual
            if (hasManualButton)
                buttonsContainer
                    .AddChild
                    (
                        GetButton
                        (
                            "Manual",
                            EditorSpriteSheets.EditorUI.Icons.BookOpen,
                            info.ManualURL,
                            EditorSelectableColors.Default.UnityThemeInversed
                        )
                    );

            //Get from Doozy Button
            var getFromDoozyButton =
                GetButton
                    (
                        "Get",
                        EditorSpriteSheets.EditorUI.Icons.Store,
                        info.DoozyURL,
                        EditorSelectableColors.EditorUI.Amber
                    )
                    .SetButtonStyle(ButtonStyle.Contained)
                    .SetElementSize(ElementSize.Large);


            //Unity Asset Store
            var assetStoreButton =
                GetButton
                (
                    "Asset Store",
                    EditorSpriteSheets.EditorUI.Icons.Unity,
                    info.UnityAssetStoreURL,
                    EditorSelectableColors.Default.UnityThemeInversed
                );

            if (!hasDoozyButton)
            {
                buttonsContainer.AddFlexibleSpace();
                assetStoreButton
                    .SetButtonStyle(ButtonStyle.Contained)
                    .SetElementSize(ElementSize.Large);

                if (hasAssetStoreButton)
                    buttonsContainer.AddChild(assetStoreButton);
            }
            else
            {
                if (hasAssetStoreButton)
                    buttonsContainer.AddChild(assetStoreButton);
                buttonsContainer.AddFlexibleSpace();
                buttonsContainer.AddChild(getFromDoozyButton);
            }

            return
                element
                    .AddChild
                    (
                        content
                            .AddChild(addonImage)
                            .AddChild(DesignUtils.spaceBlock2X)
                            .AddChild
                            (
                                infoContainer
                                    .AddChild
                                    (
                                        titleContainer
                                            .AddChild(categoryLabel)
                                            .AddChild(DesignUtils.spaceBlock)
                                            .AddChild(DesignUtils.dividerVertical)
                                            .AddChild(DesignUtils.spaceBlock)
                                            .AddChild(nameLabel)
                                    )
                                    .AddChild(DesignUtils.spaceBlock2X)
                                    .AddChild(descriptionLabel)
                                    .AddChild(DesignUtils.spaceBlock2X)
                                    .AddFlexibleSpace()
                                    .AddChild(buttonsContainer)
                            )
                    );
        }


        private void Compose()
        {
            content.AddChild(contentScrollView);
            contentScrollView.contentContainer.AddChild(itemsContainer);
        }
    }
}
