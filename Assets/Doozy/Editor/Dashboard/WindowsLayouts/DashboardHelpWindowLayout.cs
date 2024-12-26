// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Interfaces;
using Doozy.Editor.Reactor.Internal;
using Doozy.Runtime.Reactor.Extensions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.UIElements;
namespace Doozy.Editor.Dashboard.WindowsLayouts
{
    public class DashboardHelpWindowLayout : FluidWindowLayout, IDashboardWindowLayout
    {
        public int order => 1100;

        public override string layoutName => "Help";
        public sealed override List<Texture2D> animatedIconTextures => EditorSpriteSheets.EditorUI.Icons.QuestionMark;

        public override Color accentColor => EditorColors.Default.UnityThemeInversed;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Default.UnityThemeInversed;

        private float spacing2X => spacing * 2f;

        private ScrollView contentScrollView { get; set; }
        private VisualElement itemsContainer { get; set; }

        public DashboardHelpWindowLayout()
        {
            AddHeader("Help", "Resources", animatedIconTextures);
            content
                .ResetLayout()
                .SetStylePadding(DesignUtils.k_Spacing)
                .SetStyleBackgroundImage(EditorTextures.Dashboard.Backgrounds.DashboardBackground)
                .SetStyleBackgroundScaleMode(ScaleMode.ScaleAndCrop)
                .SetStyleBackgroundImageTintColor(EditorColors.Default.MenuBackgroundLevel0);

            sideMenu.Dispose();
            Initialize();
            Compose();
        }

        private void Initialize()
        {
            contentScrollView = new ScrollView();
            contentScrollView.contentContainer.SetStylePadding(spacing);
            itemsContainer =
                new VisualElement()
                    .SetStyleFlexDirection(FlexDirection.Row)
                    .SetStyleFlexWrap(Wrap.Wrap)
                    .SetStyleAlignContent(Align.FlexStart)
                    .SetStyleAlignItems(Align.FlexStart)
                    .SetStyleAlignSelf(Align.FlexStart);
        }

        private void Compose()
        {
            content.AddChild(contentScrollView);
            contentScrollView.contentContainer.AddChild(itemsContainer);

            itemsContainer
                .AddChild
                (
                    GetSection("Need Help?")
                        .AddChild(GetSectionTitle("Need Help?", "Here are some links to help you :)"))
                        .AddChild(GetHelpCard("Doozy UI Manager", "doozyui.com", "https://doozyui.com", EditorSpriteSheets.EditorUI.Icons.DoozyUI))
                        .AddChild(GetHelpCard("Knowledge Base", "docs.doozyui.com", "https://docs.doozyui.com", EditorSpriteSheets.EditorUI.Icons.BookOpen))
                        .AddChild(GetHelpCard("API Reference", "api.doozyui.com", "https://api.doozyui.com", EditorSpriteSheets.EditorUI.Icons.API))
                        .AddChild(GetHelpCard("Help Desk", "support.doozyui.com", "https://support.doozyui.com", EditorSpriteSheets.EditorUI.Icons.SupportRequest))
                )
                .AddChild
                (
                    GetSection("Get in Touch")
                        .AddChild(GetSectionTitle("Get in Touch", "Join the community, share your ideas, or just say hi!"))
                        .AddChild(GetHelpCard("Discord", "discord.doozyui.com", "https://discord.doozyui.com", EditorSpriteSheets.EditorUI.Icons.Discord))
                        .AddChild(GetHelpCard("YouTube", "youtube.doozyui.com", "https://youtube.doozyui.com", EditorSpriteSheets.EditorUI.Icons.Youtube))
                        .AddChild(GetHelpCard("Twitter", "twitter.doozyui.com", "https://twitter.doozyui.com", EditorSpriteSheets.EditorUI.Icons.Twitter))
                        .AddChild(GetHelpCard("Facebook", "facebook.doozyui.com", "https://facebook.doozyui.com", EditorSpriteSheets.EditorUI.Icons.Facebook))
                )
                .AddChild
                (
                    GetSection("Other Resources")
                        .AddChild(GetSectionTitle("Other Resources", "Useful links to other resources"))
                        .AddChild(GetHelpCard("Unity Manual", "docs.unity3d.com/Manual", "https://docs.unity3d.com/Manual/index.html", EditorSpriteSheets.EditorUI.Icons.Unity))
                        .AddChild(GetHelpCard("Unity Scripting API", "docs.unity3d.com/ScriptReference", "https://docs.unity3d.com/ScriptReference/index.html", EditorSpriteSheets.EditorUI.Icons.Manual))
                        .AddChild(GetHelpCard(".NET API Browser", "docs.microsoft.com/en-us/dotnet/api", "https://docs.microsoft.com/en-us/dotnet/api/", EditorSpriteSheets.EditorUI.Icons.Windows))
                );
        }

        private VisualElement GetSection(string sectionName) =>
            DesignUtils.column.SetName(sectionName)
                .SetStyleMargins(spacing)
                .SetStyleFlexGrow(0)
                .SetStyleFlexShrink(0)
                .SetStyleWidth(256, 256, 256);

        private VisualElement GetSectionTitle(string title, string subtitle)
        {
            var element = new VisualElement()
                .SetStyleFlexShrink(0)
                .SetStylePadding(spacing2X)
                .SetStyleMarginBottom(spacing);
            var titleLabel = DesignUtils.fieldLabel.SetText(title).SetStyleFontSize(15).SetStyleColor(EditorColors.Default.TextTitle);
            var subtitleLabel = DesignUtils.fieldLabel.SetText(subtitle).SetStyleFontSize(10).SetStyleColor(EditorColors.Default.TextSubtitle);

            element
                .AddChild(titleLabel)
                .AddChild(DesignUtils.dividerHorizontal)
                .AddChild(subtitleLabel);

            return element;
        }


        private VisualElement GetHelpCard(string title, string subtitle, string url, IEnumerable<Texture2D> iconTextures)
        {
            var element = new VisualElement()
                .SetStyleFlexShrink(0)
                .SetStyleMaxWidth(256)
                .SetStyleFlexDirection(FlexDirection.Row)
                .SetStyleBackgroundColor(EditorColors.Default.BoxBackground)
                .SetStyleAlignItems(Align.Center)
                .SetStyleBorderRadius(spacing2X)
                .SetStyleMarginBottom(spacing)
                .SetStylePadding(spacing, spacing, spacing2X, spacing);

            var iconImage =
                new Image()
                    .SetStyleSize(32)
                    .SetStylePadding(spacing2X)
                    .SetStyleFlexShrink(0)
                    .SetStyleBackgroundImageTintColor(EditorColors.Default.Icon);

            var iconImageReaction =
                iconImage.GetTexture2DReaction(iconTextures)
                    .SetEditorHeartbeat();

            var titleLabel =
                DesignUtils.fieldLabel
                    .SetText(title)
                    .SetStyleFontSize(13)
                    .SetStyleColor(EditorColors.Default.TextTitle);

            var subtitleLabel =
                DesignUtils.fieldLabel
                    .SetText(subtitle)
                    .SetStyleColor(EditorColors.Default.TextSubtitle);

            var button = FluidButton.Get()
                .SetLabelText("Open")
                .SetTooltip($"{title}\n{url}")
                .SetOnClick(() => Application.OpenURL(url))
                .SetIcon(EditorSpriteSheets.EditorUI.Arrows.ChevronRight)
                .SetStyleFlexGrow(0)
                .SetButtonStyle(ButtonStyle.Contained)
                .SetElementSize(ElementSize.Tiny);

            element.RegisterCallback<PointerEnterEvent>(evt => iconImageReaction?.Play());
            element.AddManipulator(new Clickable(() => iconImageReaction?.Play()));

            element
                .AddChild(iconImage)
                .AddSpace(spacing2X)
                .AddChild
                (
                    DesignUtils.column
                        .AddChild(titleLabel)
                        .AddChild(subtitleLabel)
                )
                .AddSpace(spacing2X)
                .AddChild(button);

            return element;
        }
    }
}
