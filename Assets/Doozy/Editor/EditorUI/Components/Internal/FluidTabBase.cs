// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.EditorUI.Events;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Pooler;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Doozy.Editor.EditorUI.Components.Internal
{
    public abstract class FluidTabBase<T> : PoolableElement<T> where T : VisualElement, IPoolable, new()
    {
        public FluidToggleButtonTab button { get; private set; }
        public EnabledIndicator indicator { get; private set; }

        public bool buttonIsOn
        {
            get => button.isOn;
            set => button.SetIsOn(value, true);
        }

        public bool indicatorIsOn
        {
            get => indicator.isOn;
            set => indicator.Toggle(value, true);
        }

        public override void Dispose()
        {
            base.Dispose();
            button.Dispose();
            indicator.Dispose();
        }

        public override void Reset()
        {
            button = new FluidToggleButtonTab();
            indicator = new EnabledIndicator();

            this
                .RecycleAndClear()
                .AddChild(indicator)
                .AddChild(button);
        }
    }

    public static class FluidTabBaseExtensions
    {
        public static T AddToToggleGroup<T>(this T target, FluidToggleGroup toggleGroup) where T : FluidTabBase<T>, new()
        {
            target.button.AddToToggleGroup(toggleGroup);
            return target;
        }
        
        public static T RemoveFromToggleGroup<T>(this T target) where T : FluidTabBase<T>, new()
        {
            target.button.RemoveFromToggleGroup();
            return target;
        }
        
        public static T SetIndicatorPosition<T>(this T target, IndicatorPosition indicatorPosition) where T : FluidTabBase<T>, new()
        {
            target.indicator
                .SetStylePosition(Position.Relative)
                .SetStyleWidth(StyleKeyword.Auto)
                .SetStyleHeight(StyleKeyword.Auto);

            switch (indicatorPosition)
            {
                case IndicatorPosition.Left:
                    target.SetStyleFlexDirection(FlexDirection.RowReverse);
                    target.indicator.SetStyleWidth(1);
                    break;
                case IndicatorPosition.Top:
                    target.SetStyleFlexDirection(FlexDirection.ColumnReverse);
                    target.indicator.SetStyleHeight(1);
                    break;
                case IndicatorPosition.Right:
                    target.SetStyleFlexDirection(FlexDirection.Row);
                    target.indicator.SetStyleWidth(1);
                    break;
                case IndicatorPosition.Bottom:
                    target.SetStyleFlexDirection(FlexDirection.Column);
                    target.indicator.SetStyleHeight(1);
                    break;
                case IndicatorPosition.Custom:
                    target.indicator.SetStylePosition(Position.Absolute);

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(indicatorPosition), indicatorPosition, null);
            }

            return target;
        }

        public static T ResetTabPosition<T>(this T target) where T : FluidTabBase<T>, new() =>
            target.SetTabPosition(TabPosition.TabOnBottom);

        public static T SetTabPosition<T>(this T target, TabPosition tabPosition) where T : FluidTabBase<T>, new()
        {
            target.button.SetTabPosition(tabPosition);
            target.indicator.SetStyleBorderRadius(0);

            switch (tabPosition)
            {
                case TabPosition.FloatingTab:
                    target.SetIndicatorPosition(IndicatorPosition.Custom);
                    target
                        .indicator
                        .SetStyleLeft(0)
                        .SetStyleTop(0)
                        .SetStyleRight(0)
                        .SetStyleBottom(0)
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
                        .indicator
                        .SetStyleLeft(-1f)
                        .SetStyleTop(-1f)
                        .SetStyleRight(-1f)
                        .SetStyleBottom(-1f)
                        .SetStyleBorderRadius(6)
                        ;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tabPosition), tabPosition, null);
            }
            return target;
        }

        public static T ButtonUpdateVisualState<T>(this T target, bool animateChange = true) where T : FluidTabBase<T>, new()
        {
            target.button.UpdateVisualState(animateChange);
            return target;
        }

        public static T ButtonSetContainerColorOff<T>(this T target, Color color) where T : FluidTabBase<T>, new()
        {
            target.button.SetContainerColorOff(color);
            return target;
        }

        public static T ClearIcon<T>(this T target) where T : FluidTabBase<T>, new()
        {
            target.button.ClearIcon();
            return target;
        }

        public static T SetIcon<T>(this T target, Texture2D texture) where T : FluidTabBase<T>, new()
        {
            target.button.SetIcon(texture);
            return target;
        }

        public static T SetIcon<T>(this T target, IEnumerable<Texture2D> textures) where T : FluidTabBase<T>, new()
        {
            target.button.SetIcon(textures);
            return target;
        }

        public static T ClearLabelText<T>(this T target) where T : FluidTabBase<T>, new()
        {
            target.button.ClearLabelText();
            return target;
        }

        public static T SetLabelText<T>(this T target, string value) where T : FluidTabBase<T>, new()
        {
            target.button.SetLabelText(value);
            return target;
        }

        public static T SetElementSize<T>(this T target, ElementSize value) where T : FluidTabBase<T>, new()
        {
            target.button.SetElementSize(value);
            return target;
        }

        public static T ButtonSetTabPosition<T>(this T target, TabPosition value) where T : FluidTabBase<T>, new()
        {
            target.button.SetTabPosition(value);
            return target;
        }

        public static T ButtonSetTabContent<T>(this T target, TabContent value) where T : FluidTabBase<T>, new()
        {
            target.button.SetTabContent(value);
            return target;
        }

        public static T ButtonSetLayoutOrientation<T>(this T target, LayoutOrientation value) where T : FluidTabBase<T>, new()
        {
            target.button.SetLayoutOrientation(value);
            return target;
        }

        public static T ButtonResetColors<T>(this T target) where T : FluidTabBase<T>, new()
        {
            target.button.ResetColors();
            return target;
        }

        public static T ButtonEnable<T>(this T target) where T : FluidTabBase<T>, new()
        {
            target.button.Enable();
            return target;
        }

        public static T ButtonDisable<T>(this T target) where T : FluidTabBase<T>, new()
        {
            target.button.Disable();
            return target;
        }

        public static T ButtonSetIsOn<T>(this T target, bool newValue, bool animateChange = true) where T : FluidTabBase<T>, new()
        {
            target.button.SetIsOn(newValue, animateChange);
            return target;
        }

        public static T ButtonSetBindingPath<T>(this T target, string bindingPath) where T : FluidTabBase<T>, new()
        {
            target.button.BindToProperty(bindingPath);
            return target;
        }

        public static T ButtonSetAccentColor<T>(this T target, EditorSelectableColorInfo value) where T : FluidTabBase<T>, new()
        {
            target.button.SetToggleAccentColor(value);
            return target;
        }

        public static T ButtonResetAccentColor<T>(this T target) where T : FluidTabBase<T>, new()
        {
            target.button.ResetAccentColor();
            return target;
        }

        public static T ButtonSetIconContainerColor<T>(this T target, EditorSelectableColorInfo value) where T : FluidTabBase<T>, new()
        {
            target.button.SetIconContainerColor(value);
            return target;
        }

        public static T ButtonSetOnValueChanged<T>(this T target, UnityAction<FluidBoolEvent> callback) where T : FluidTabBase<T>, new()
        {
            target.button.SetOnValueChanged(callback);
            return target;
        }

        public static T ButtonAddOnValueChanged<T>(this T target, UnityAction<FluidBoolEvent> callback) where T : FluidTabBase<T>, new()
        {
            target.button.AddOnValueChanged(callback);
            return target;
        }

        public static T ButtonClearOnValueChanged<T>(this T target) where T : FluidTabBase<T>, new()
        {
            target.button.ClearOnValueChanged();
            return target;
        }

        public static T ButtonSetOnClick<T>(this T target, UnityAction callback) where T : FluidTabBase<T>, new()
        {
            target.button.SetOnClick(callback);
            return target;
        }

        public static T ButtonAddOnClick<T>(this T target, UnityAction callback) where T : FluidTabBase<T>, new()
        {
            target.button.AddOnClick(callback);
            return target;
        }

        public static T ButtonClearOnClick<T>(this T target) where T : FluidTabBase<T>, new()
        {
            target.button.ClearOnClick();
            return target;
        }

        public static T IndicatorSetEnabledColor<T>(this T target, Color color, bool animateChange = false) where T : FluidTabBase<T>, new()
        {
            target.indicator.SetEnabledColor(color, animateChange);
            return target;
        }

        public static T IndicatorSetDisabledColor<T>(this T target, Color color, bool animateChange = false) where T : FluidTabBase<T>, new()
        {
            target.indicator.SetDisabledColor(color, animateChange);
            return target;
        }

        public static T IndicatorToggle<T>(this T target, bool enabled, bool animateChange = false) where T : FluidTabBase<T>, new()
        {
            target.indicator.Toggle(enabled, animateChange);
            return target;
        }

        public static T IndicatorSetEnabled<T>(this T target, bool animateChange = true, bool forced = false) where T : FluidTabBase<T>, new()
        {
            target.indicator.SetEnabled(animateChange, forced);
            return target;
        }

        public static T IndicatorSetDisabled<T>(this T target, bool animateChange = true, bool forced = false) where T : FluidTabBase<T>, new()
        {
            target.indicator.SetDisabled(animateChange, forced);
            return target;
        }

        public static T IndicatorSetIconIsLooping<T>(this T target, bool isLooping) where T : FluidTabBase<T>, new()
        {
            target.indicator.IconIsLooping(isLooping);
            return target;
        }

        public static T IndicatorSetIcon<T>(this T target, IEnumerable<Texture2D> textures, bool isLooping = false) where T : FluidTabBase<T>, new()
        {
            target.indicator.SetIcon(textures, isLooping);
            return target;
        }

        public static T IndicatorSetIcon<T>(this T target, Texture2D texture) where T : FluidTabBase<T>, new()
        {
            target.indicator.SetIcon(texture);
            return target;
        }

        public static T IndicatorClearIcon<T>(this T target) where T : FluidTabBase<T>, new()
        {
            target.indicator.ClearIcon();
            return target;
        }

        public static T IndicatorSetSize<T>(this T target, int width, int height) where T : FluidTabBase<T>, new()
        {
            target.indicator.SetSize(width, height);
            return target;
        }

        public static T IndicatorSetSize<T>(this T target, int size) where T : FluidTabBase<T>, new()
        {
            target.indicator.SetSize(size);
            return target;
        }

        public static T IndicatorResetSize<T>(this T target) where T : FluidTabBase<T>, new()
        {
            target.indicator.ResetSize();
            return target;
        }

        public static T IndicatorUpdate<T>(this T target) where T : FluidTabBase<T>, new()
        {
            target.indicator.Update();
            return target;
        }
    }
}
