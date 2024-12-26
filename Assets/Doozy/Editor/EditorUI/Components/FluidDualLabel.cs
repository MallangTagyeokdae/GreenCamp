// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.EditorUI.Components
{
    /// <summary> Component with two labels (title and description) that can be set in either horizontal or vertical layout </summary>
    public class FluidDualLabel : VisualElement
    {
        public enum Layout
        {
            Horizontal,
            Vertical
        }

        internal static Font titleFont => EditorFonts.Inter.Light;
        internal static Font descriptionFont => EditorFonts.Ubuntu.Regular;
        internal static Color titleColor => EditorColors.Default.TextTitle;
        internal static Color descriptionColor => EditorColors.Default.TextSubtitle;

        private ElementSize elementSize { get; set; }

        public VisualElement root { get; private set; }
        public Label titleLabel { get; private set; }
        public Label descriptionLabel { get; private set; }

        public FluidDualLabel()
        {
            Initialize();
            Compose();
            this
                .ResetTitleColor()
                .ResetDescriptionColor()
                .SetLayout(Layout.Horizontal)
                .SetElementSize(ElementSize.Normal);
        }

        public FluidDualLabel(string title, string description) : this()
        {
            this.titleLabel.text = title;
            this.descriptionLabel.text = description;
        }
        
        protected virtual void Initialize()
        {
            Add(root = new VisualElement().SetStyleFlexGrow(1));

            titleLabel =
                new Label()
                    .ResetLayout()
                    .SetStyleUnityFont(titleFont);

            descriptionLabel =
                new Label()
                    .ResetLayout()
                    .SetStyleUnityFont(descriptionFont);
        }

        protected virtual void Compose()
        {
            root
                .AddChild(titleLabel)
                .AddChild(descriptionLabel);
        }

        internal void UpdateElementSize(ElementSize size)
        {
            int titleSize = 10;
            int descriptionSize = 9;
            switch (size)
            {
                case ElementSize.Tiny:
                    titleSize = 8;
                    descriptionSize = 7;
                    break;
                case ElementSize.Small:
                    titleSize = 9;
                    descriptionSize = 8;
                    break;
                case ElementSize.Normal:
                    titleSize = 10;
                    descriptionSize = 9;
                    break;
                case ElementSize.Large:
                    titleSize = 11;
                    descriptionSize = 10;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(size), size, null);
            }

            titleLabel.SetStyleFontSize(titleSize);
            descriptionLabel.SetStyleFontSize(descriptionSize);
        }
    }

    public static class FluidInfoLabelExtensions
    {
        /// <summary> Set title text color </summary>
        /// <param name="target"> Target </param>
        /// <param name="color"> Text color </param>
        public static T SetTitleColor<T>(this T target, Color color) where T : FluidDualLabel
        {
            target.titleLabel.SetStyleColor(color);
            return target;
        }
        
        /// <summary> Set description text color </summary>
        /// <param name="target"> Target </param>
        /// <param name="color"> Text color </param>
        public static T SetDescriptionTextColor<T>(this T target, Color color) where T : FluidDualLabel
        {
            target.descriptionLabel.SetStyleColor(color);
            return target;
        }
        
        /// <summary> Reset title color </summary>
        /// <param name="target"> Target </param>
        public static T ResetTitleColor<T>(this T target) where T : FluidDualLabel =>
            target.SetTitleColor(FluidDualLabel.titleColor);

        /// <summary> Reset description color </summary>
        /// <param name="target"> Target </param>
        public static T ResetDescriptionColor<T>(this T target) where T : FluidDualLabel =>
            target.SetDescriptionTextColor(FluidDualLabel.descriptionColor);

        /// <summary> Show title </summary>
        /// <param name="target"> Target </param>
        public static T ShowTitle<T>(this T target) where T : FluidDualLabel
        {
            target.titleLabel.SetStyleDisplay(DisplayStyle.Flex);
            return target;
        }
        
        /// <summary> Hide title </summary>
        /// <param name="target"> Target </param>
        public static T HideTitle<T>(this T target) where T : FluidDualLabel
        {
            target.titleLabel.SetStyleDisplay(DisplayStyle.None);
            return target;
        }
        
        /// <summary> Show description </summary>
        /// <param name="target"> Target </param>
        public static T ShowDescription<T>(this T target) where T : FluidDualLabel
        {
            target.descriptionLabel.SetStyleDisplay(DisplayStyle.Flex);
            return target;
        }
        
        /// <summary> Hide description </summary>
        /// <param name="target"> Target </param>
        public static T HideDescription<T>(this T target) where T : FluidDualLabel
        {
            target.descriptionLabel.SetStyleDisplay(DisplayStyle.None);
            return target;
        }
        
        /// <summary> Set element size </summary>
        /// <param name="target"> Target </param>
        /// <param name="size"> Element size </param>
        public static T SetElementSize<T>(this T target, ElementSize size) where T : FluidDualLabel
        {
            target.UpdateElementSize(size);
            return target;
        }

        /// <summary> Set title text </summary>
        /// <param name="target"> Target </param>
        /// <param name="text"> Title text </param>
        public static T SetTitle<T>(this T target, string text) where T : FluidDualLabel
        {
            target.titleLabel.text = text;
            return target;
        }

        /// <summary> Set description text </summary>
        /// <param name="target"> Target </param>
        /// <param name="text"> Description text </param>
        public static T SetDescription<T>(this T target, string text) where T : FluidDualLabel
        {
            target.descriptionLabel.text = text;
            return target;
        }

        /// <summary> Set horizontal or vertical layout </summary>
        /// <param name="target"> Target </param>
        /// <param name="layout"> Horizontal or Vertical layout </param>
        public static T SetLayout<T>(this T target, FluidDualLabel.Layout layout) where T : FluidDualLabel
        {
            target.titleLabel.ClearMargins();
            switch (layout)
            {
                case FluidDualLabel.Layout.Horizontal:
                    target.root
                        .SetStyleFlexDirection(FlexDirection.Row);

                    target.titleLabel
                        .SetStyleMarginRight(DesignUtils.k_Spacing)
                        .SetStyleTextAlign(TextAnchor.MiddleLeft)
                        .SetStyleFlexGrow(0);

                    target.descriptionLabel
                        .SetStyleTextAlign(TextAnchor.MiddleLeft)
                        .SetStyleFlexGrow(1);

                    break;
                case FluidDualLabel.Layout.Vertical:
                    target.root
                        .SetStyleFlexDirection(FlexDirection.Column);

                    target.titleLabel
                        .SetStyleMarginBottom(DesignUtils.k_Spacing)
                        .SetStyleTextAlign(TextAnchor.UpperLeft)
                        .SetStyleFlexGrow(1);

                    target.descriptionLabel
                        .SetStyleTextAlign(TextAnchor.UpperLeft)
                        .SetStyleFlexGrow(1);

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(layout), layout, null);
            }
            return target;
        }
    }
}
