// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Reactor.Components
{
    /// <summary> A tab button with an enabled indicator and a color indicator attached </summary>
    public class ColorAnimationTab : FluidTabBase<ColorAnimationTab>
    {
        private const float TAB_MIN_WIDTH = 68f;
        private const float COLOR_REFERENCE_WIDTH = 60f;

        public VisualElement referenceColorElement { get; }
        public Color referenceColor { get; private set; }

        public ColorAnimationTab()
        {
            referenceColorElement = GetReferenceColorElement();

            Reset();
        }

        public override void Reset()
        {
            base.Reset();

            this
                .ResetStyleSize()
                .SetStyleMinWidth(TAB_MIN_WIDTH)
                .ResetTabPosition()
                .SetElementSize(ElementSize.Small)
                .ButtonSetContainerColorOff(DesignUtils.tabButtonColorOff);

            this
                .AddChild(referenceColorElement);
        }

        public ColorAnimationTab SetReferenceColor(Color color)
        {
            referenceColor = color;
            referenceColorElement.SetStyleBackgroundColor(referenceColor);
            return this;
        }

        public static VisualElement GetReferenceColorElement() =>
            new VisualElement()
                .SetStyleHeight(6)
                .SetStyleWidth(COLOR_REFERENCE_WIDTH)
                .SetStyleAlignSelf(Align.Center)
                .SetStyleBorderRadius(3)
                .SetStyleMarginTop(DesignUtils.k_Spacing);

        public ColorAnimationTab RefreshTabReferenceColor(ColorAnimation animation)
        {
            bool isValid = animation.hasTarget && animation.isEnabled;
            referenceColorElement.SetStyleDisplay(isValid ? DisplayStyle.Flex : DisplayStyle.None);

            if (!isValid)
                return this;

            if (animation.animation.targetObject == null)
                animation.animation.SetTarget(animation.colorTarget);

            if (animation.animation.targetObject == null)
                return this;

            SetReferenceColor
            (
                animation.animation.GetValue
                (
                    animation.animation.toReferenceValue,
                    animation.animation.startColor,
                    animation.animation.currentColor,
                    animation.animation.toCustomValue,
                    animation.animation.toHueOffset,
                    animation.animation.toSaturationOffset,
                    animation.animation.toLightnessOffset,
                    animation.animation.toAlphaOffset
                )
            );

            return this;
        }
    }
}
