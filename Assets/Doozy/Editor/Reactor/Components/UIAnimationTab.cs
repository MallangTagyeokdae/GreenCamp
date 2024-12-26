// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.Events;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIElements;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable RedundantArgumentDefaultValue

namespace Doozy.Editor.Reactor.Components
{
    /// <summary> A tab button with four enabled indicators </summary>
    public class UIAnimationTab : PoolableElement<UIAnimationTab>
    {
        private const float TAB_MIN_WIDTH = 56f;
        private const int INDICATOR_SPACING = 2;

        public FluidToggleButtonTab button { get; private set; }
        public VisualElement indicatorsContainer { get; private set; }
        public EnabledIndicator moveIndicator { get; private set; }
        public EnabledIndicator rotateIndicator { get; private set; }
        public EnabledIndicator scaleIndicator { get; private set; }
        public EnabledIndicator fadeIndicator { get; private set; }

        public bool buttonIsOn
        {
            get => button.isOn;
            set => button.SetIsOn(value, true);
        }

        public bool moveIndicatorIsOn
        {
            get => moveIndicator.isOn;
            set => moveIndicator.Toggle(value, true);
        }

        public bool rotateIndicatorIsOn
        {
            get => rotateIndicator.isOn;
            set => rotateIndicator.Toggle(value, true);
        }

        public bool scaleIndicatorIsOn
        {
            get => scaleIndicator.isOn;
            set => scaleIndicator.Toggle(value, true);
        }

        public bool fadeIndicatorIsOn
        {
            get => fadeIndicator.isOn;
            set => fadeIndicator.Toggle(value, true);
        }

        public override void Dispose()
        {
            base.Dispose();
            button?.Dispose();
            moveIndicator?.Dispose();
            rotateIndicator?.Dispose();
            scaleIndicator?.Dispose();
            fadeIndicator?.Dispose();
        }

        public override void Reset()
        {
            button?.Dispose();
            moveIndicator?.Dispose();
            rotateIndicator?.Dispose();
            scaleIndicator?.Dispose();
            fadeIndicator?.Dispose();
            
            button = FluidToggleButtonTab.Get();
            indicatorsContainer = DesignUtils.row.SetName("Indicators");
            moveIndicator = EnabledIndicator.Get().SetName("Move").SetEnabledColor(EditorColors.Reactor.Move).SetStyleFlexGrow(1);
            rotateIndicator = EnabledIndicator.Get().SetName("Rotate").SetEnabledColor(EditorColors.Reactor.Rotate).SetStyleFlexGrow(1);
            scaleIndicator = EnabledIndicator.Get().SetName("Scale").SetEnabledColor(EditorColors.Reactor.Scale).SetStyleFlexGrow(1);
            fadeIndicator = EnabledIndicator.Get().SetName("Fade").SetEnabledColor(EditorColors.Reactor.Fade).SetStyleFlexGrow(1);


            this
                .RecycleAndClear()
                .ResetStyleSize()
                .SetStyleMinWidth(TAB_MIN_WIDTH)
                .ResetTabPosition()
                .SetElementSize(ElementSize.Small)
                .ButtonSetContainerColorOff(DesignUtils.tabButtonColorOff)
                .AddChild(
                    indicatorsContainer
                        .AddChild(moveIndicator)
                        .AddSpace(INDICATOR_SPACING, 0)
                        .AddChild(rotateIndicator)
                        .AddSpace(INDICATOR_SPACING, 0)
                        .AddChild(scaleIndicator)
                        .AddSpace(INDICATOR_SPACING, 0)
                        .AddChild(fadeIndicator)
                )
                .AddChild(button);
        }
    }

    public static class UIAnimationTabExtensions
    {
        public static T AddToToggleGroup<T>(this T target, FluidToggleGroup toggleGroup) where T : UIAnimationTab
        {
            target.button.AddToToggleGroup(toggleGroup);
            return target;
        }
        
        public static T RemoveFromToggleGroup<T>(this T target) where T : UIAnimationTab
        {
            target.button.RemoveFromToggleGroup();
            return target;
        }
        
        public static T SetIndicatorPosition<T>(this T target, IndicatorPosition indicatorPosition) where T : UIAnimationTab
        {
            target.indicatorsContainer
                .SetStylePosition(Position.Relative)
                .SetStyleWidth(StyleKeyword.Auto)
                .SetStyleHeight(StyleKeyword.Auto);

            switch (indicatorPosition)
            {
                case IndicatorPosition.Left:
                    target.SetStyleFlexDirection(FlexDirection.RowReverse);
                    target.indicatorsContainer.SetStyleWidth(1);
                    break;
                case IndicatorPosition.Top:
                    target.SetStyleFlexDirection(FlexDirection.ColumnReverse);
                    target.indicatorsContainer.SetStyleHeight(1);
                    break;
                case IndicatorPosition.Right:
                    target.SetStyleFlexDirection(FlexDirection.Row);
                    target.indicatorsContainer.SetStyleWidth(1);
                    break;
                case IndicatorPosition.Bottom:
                    target.SetStyleFlexDirection(FlexDirection.Column);
                    target.indicatorsContainer.SetStyleHeight(1);
                    break;
                case IndicatorPosition.Custom:
                    target.indicatorsContainer.SetStylePosition(Position.Absolute);

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(indicatorPosition), indicatorPosition, null);
            }

            return target;
        }

        public static T ResetTabPosition<T>(this T target) where T : UIAnimationTab =>
            target.SetTabPosition(TabPosition.TabOnBottom);

        public static T SetTabPosition<T>(this T target, TabPosition tabPosition) where T : UIAnimationTab
        {
            target.button.SetTabPosition(tabPosition);
            target.indicatorsContainer.SetStyleBorderRadius(0);

            switch (tabPosition)
            {
                case TabPosition.FloatingTab:
                    target.SetIndicatorPosition(IndicatorPosition.Custom);
                    target
                        .indicatorsContainer
                        .SetStyleLeft(0)
                        .SetStyleTop(0)
                        .SetStyleRight(0)
                        .SetStyleBottom(1)
                        .SetStyleBorderRadius(3)
                        ;
                    break;
                case TabPosition.TabOnLeft:
                    target.SetIndicatorPosition(IndicatorPosition.Left);
                    break;
                case TabPosition.TabOnTop:
                    target.SetIndicatorPosition(IndicatorPosition.Top);
                    break;
                case TabPosition.TabOnRight:
                    target.SetIndicatorPosition(IndicatorPosition.Right);
                    break;
                case TabPosition.TabOnBottom:
                    target.SetIndicatorPosition(IndicatorPosition.Bottom);
                    break;
                case TabPosition.TabInCenter:
                    target.SetIndicatorPosition(IndicatorPosition.Custom);
                    target
                        .indicatorsContainer
                        .SetStyleLeft(-1f)
                        .SetStyleTop(-1f)
                        .SetStyleRight(-1f)
                        .SetStyleBottom(0)
                        .SetStyleBorderRadius(6)
                        ;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tabPosition), tabPosition, null);
            }
            return target;
        }

        public static T ButtonUpdateVisualState<T>(this T target, bool animateChange = true) where T : UIAnimationTab
        {
            target.button.UpdateVisualState(animateChange);
            return target;
        }

        public static T ButtonSetContainerColorOff<T>(this T target, Color color) where T : UIAnimationTab
        {
            target.button.SetContainerColorOff(color);
            return target;
        }

        public static T ClearIcon<T>(this T target) where T : UIAnimationTab
        {
            target.button.ClearIcon();
            return target;
        }

        public static T SetIcon<T>(this T target, Texture2D texture) where T : UIAnimationTab
        {
            target.button.SetIcon(texture);
            return target;
        }

        public static T SetIcon<T>(this T target, IEnumerable<Texture2D> textures) where T : UIAnimationTab
        {
            target.button.SetIcon(textures);
            return target;
        }

        public static T ClearLabelText<T>(this T target) where T : UIAnimationTab
        {
            target.button.ClearLabelText();
            return target;
        }

        public static T SetLabelText<T>(this T target, string value) where T : UIAnimationTab
        {
            target.button.SetLabelText(value);
            return target;
        }

        public static T SetElementSize<T>(this T target, ElementSize value) where T : UIAnimationTab
        {
            target.button.SetElementSize(value);
            return target;
        }

        public static T ButtonSetTabPosition<T>(this T target, TabPosition value) where T : UIAnimationTab
        {
            target.button.SetTabPosition(value);
            return target;
        }

        public static T ButtonSetTabContent<T>(this T target, TabContent value) where T : UIAnimationTab
        {
            target.button.SetTabContent(value);
            return target;
        }

        public static T ButtonSetLayoutOrientation<T>(this T target, LayoutOrientation value) where T : UIAnimationTab
        {
            target.button.SetLayoutOrientation(value);
            return target;
        }

        public static T ButtonResetColors<T>(this T target) where T : UIAnimationTab
        {
            target.button.ResetColors();
            return target;
        }

        public static T ButtonEnable<T>(this T target) where T : UIAnimationTab
        {
            target.button.Enable();
            return target;
        }

        public static T ButtonDisable<T>(this T target) where T : UIAnimationTab
        {
            target.button.Disable();
            return target;
        }

        public static T ButtonSetIsOn<T>(this T target, bool newValue, bool animateChange = true) where T : UIAnimationTab
        {
            target.button.SetIsOn(newValue, animateChange);
            return target;
        }

        public static T ButtonSetBindingPath<T>(this T target, string bindingPath) where T : UIAnimationTab
        {
            target.button.BindToProperty(bindingPath);
            return target;
        }

        public static T ButtonSetAccentColor<T>(this T target, EditorSelectableColorInfo value) where T : UIAnimationTab
        {
            target.button.SetToggleAccentColor(value);
            return target;
        }

        public static T ButtonResetAccentColor<T>(this T target) where T : UIAnimationTab
        {
            target.button.ResetAccentColor();
            return target;
        }

        public static T ButtonSetIconContainerColor<T>(this T target, EditorSelectableColorInfo value) where T : UIAnimationTab
        {
            target.button.SetIconContainerColor(value);
            return target;
        }

        public static T ButtonSetOnValueChanged<T>(this T target, UnityAction<FluidBoolEvent> callback) where T : UIAnimationTab
        {
            target.button.SetOnValueChanged(callback);
            return target;
        }

        public static T ButtonAddOnValueChanged<T>(this T target, UnityAction<FluidBoolEvent> callback) where T : UIAnimationTab
        {
            target.button.AddOnValueChanged(callback);
            return target;
        }

        public static T ButtonClearOnValueChanged<T>(this T target) where T : UIAnimationTab
        {
            target.button.ClearOnValueChanged();
            return target;
        }

        public static T ButtonSetOnClick<T>(this T target, UnityAction callback) where T : UIAnimationTab
        {
            target.button.SetOnClick(callback);
            return target;
        }

        public static T ButtonAddOnClick<T>(this T target, UnityAction callback) where T : UIAnimationTab
        {
            target.button.AddOnClick(callback);
            return target;
        }

        public static T ButtonClearOnClick<T>(this T target) where T : UIAnimationTab
        {
            target.button.ClearOnClick();
            return target;
        }
    }
}
