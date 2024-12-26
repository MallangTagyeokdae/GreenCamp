// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Internal;
using Doozy.Runtime.Colors;
using Doozy.Runtime.Common.Events;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Easings;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.EditorUI.Components
{
    public class FluidRangeSlider : VisualElement
    {
        public VisualElement labelsContainer { get; }
        public VisualElement snapIntervalIndicatorsContainer { get; }
        public VisualElement snapValuesIndicatorsContainer { get; }
        public VisualElement sliderContainer { get; }
        public Slider slider { get; }

        public Label lowValueLabel { get; private set; }
        public Label highValueLabel { get; private set; }
        public Label valueLabel { get; private set; }

        public bool snapToInterval { get; set; } = true;
        public float snapInterval { get; set; } = 0.1f;

        public bool snapToValues { get; set; } = false;
        public float[] snapValues { get; set; } = { 0.1f, 0.5f, 1f, 2f, 5f, 10f };
        public float snapValuesInterval { get; set; } = 0.1f;

        public bool autoResetToValue { get; set; } = false;
        public float autoResetValue { get; set; } = 0f;

        public UnityEvent onStartValueChange { get; } = new UnityEvent();
        public UnityEvent onEndValueChange { get; } = new UnityEvent();
        public FloatEvent onValueChanged { get; } = new FloatEvent();

        private FloatReaction resetToValueReaction { get; set; }

        public VisualElement sliderTracker { get; }
        public VisualElement sliderDraggerBorder { get; }
        public VisualElement sliderDragger { get; }

        private const float TRACKER_OFFSET = 4;
        private float sliderTrackerWidth => sliderTracker.resolvedStyle.width - TRACKER_OFFSET * 2;
        private float length => slider.highValue - slider.lowValue;

        public FluidRangeSlider()
        {
            this
                .SetStyleFlexShrink(0)
                .SetStylePaddingLeft(DesignUtils.k_Spacing2X)
                .SetStylePaddingRight(DesignUtils.k_Spacing2X)
                .RegisterCallback<GeometryChangedEvent>(evt =>
                {
                    UpdateSnapValuesIndicators();
                });

            labelsContainer =
                new VisualElement()
                    .SetName("Labels Container")
                    .SetStyleFlexDirection(FlexDirection.Row)
                    .SetStyleJustifyContent(Justify.Center)
                    .SetStyleAlignItems(Align.FlexStart)
                    .SetStyleMarginTop(4)
                    .SetStyleFlexGrow(1);

            sliderContainer =
                new VisualElement()
                    .SetStyleHeight(28)
                    .SetName("Slider Container")
                    .SetStyleFlexGrow(1)
                    .SetStylePaddingTop(8);

            snapIntervalIndicatorsContainer =
                new VisualElement()
                    .SetName("Snap Interval Indicators Container")
                    .SetStylePosition(Position.Absolute)
                    .SetStyleLeft(0)
                    .SetStyleTop(0)
                    .SetStyleRight(0)
                    .SetStyleBottom(0);

            snapValuesIndicatorsContainer =
                new VisualElement()
                    .SetName("Snap Values Indicators Container")
                    .SetStylePosition(Position.Absolute)
                    .SetStyleLeft(0)
                    .SetStyleTop(0)
                    .SetStyleRight(0)
                    .SetStyleBottom(0);

            slider =
                new Slider()
                    .ResetLayout();

            sliderTracker = slider.Q<VisualElement>("unity-tracker");
            sliderDraggerBorder = slider.Q<VisualElement>("unity-dragger-border");
            sliderDragger = slider.Q<VisualElement>("unity-dragger");
            
            sliderDragger.SetStyleBorderColor(EditorColors.Default.BoxBackground.WithRGBShade(0.4f));

            FloatReaction sliderDraggerBorderReaction =
                Reaction.Get<FloatReaction>()
                    .SetEditorHeartbeat()
                    .SetGetter(() => sliderDraggerBorder.GetStyleWidth())
                    .SetSetter(value =>
                    {
                        sliderDraggerBorder.SetStyleSize(value);
                        float positionOffset = 5 - value * 0.25f;
                        sliderDraggerBorder.SetStyleLeft(positionOffset);
                        sliderDraggerBorder.SetStyleTop(positionOffset);
                    })
                    .SetDuration(0.15f)
                    .SetEase(Ease.OutSine);

            sliderDraggerBorderReaction.SetFrom(0f);
            sliderDraggerBorderReaction.SetTo(16f);

            sliderDragger.RegisterCallback<PointerEnterEvent>(evt => sliderDraggerBorderReaction?.Play());
            sliderDragger.RegisterCallback<PointerLeaveEvent>(evt => sliderDraggerBorderReaction?.Play(PlayDirection.Reverse));


            resetToValueReaction =
                Reaction
                    .Get<FloatReaction>()
                    .SetEditorHeartbeat()
                    .SetDuration(0.34f)
                    .SetEase(Ease.OutExpo)
                    .SetGetter(() => slider.value)
                    .SetSetter(value =>
                    {
                        slider.SetValueWithoutNotify(value);
                        if (valueLabel != null)
                        {
                            valueLabel.text = value.RoundToMultiple(snapInterval).ToString(CultureInfo.InvariantCulture);
                        }
                    });

            slider.RegisterValueChangedCallback(evt =>
            {
                if (evt?.newValue == null) return;
                float value = evt.newValue;

                bool snappedToValue = false;
                if (snapToValues)
                {
                    foreach (float snapValue in snapValues)
                        if (Math.Abs(snapValue - value) <= snapValuesInterval)
                        {
                            value = snapValue;
                            snappedToValue = true;
                            break;
                        }
                }

                if (!snappedToValue && snapToInterval)
                {
                    value = evt.newValue.RoundToMultiple(snapInterval);
                }

                if (valueLabel != null)
                {
                    valueLabel.text = value.ToString(CultureInfo.InvariantCulture);
                }

                slider.SetValueWithoutNotify(value);
                onValueChanged?.Invoke(value);
            });

            slider.RegisterCallback<PointerCaptureEvent>(evt =>
            {
                if (autoResetToValue & !slider.value.CloseTo(autoResetValue, 0.01f))
                    resetToValueReaction?.SetProgressAtOne();

                onStartValueChange?.Invoke();
                
                sliderDraggerBorderReaction?.Play(PlayDirection.Forward);
            });

            slider.RegisterCallback<PointerCaptureOutEvent>(evt =>
            {
                onEndValueChange?.Invoke();
                sliderDraggerBorderReaction?.Play(PlayDirection.Reverse);

                if (!autoResetToValue) return;
                resetToValueReaction.SetFrom(slider.value);
                resetToValueReaction.SetTo(autoResetValue);
                resetToValueReaction.Play();

            });

            Initialize();
            Compose();
        }

        public FluidRangeSlider(float lowValue, float highValue) : this() =>
            this.SetSliderLowAndHighValues(lowValue, highValue);

        private void Initialize()
        {
            Label GetLabel() =>
                DesignUtils.fieldLabel
                    .SetStyleMinWidth(40)
                    .SetStyleWidth(40)
                    .SetStyleFontSize(11);

            valueLabel =
                GetLabel()
                    .SetStyleColor(EditorColors.Default.UnityThemeInversed)
                    .SetStyleTextAlign(TextAnchor.LowerCenter)
                    .SetStyleFontSize(13);

            lowValueLabel = GetLabel().SetStyleTextAlign(TextAnchor.UpperLeft);
            highValueLabel = GetLabel().SetStyleTextAlign(TextAnchor.UpperRight);

            labelsContainer
                .AddChild(lowValueLabel)
                .AddChild(DesignUtils.flexibleSpace)
                .AddChild(valueLabel)
                .AddChild(DesignUtils.flexibleSpace)
                .AddChild(highValueLabel);

            sliderContainer
                .SetStyleHeight(20)
                .AddChild(snapIntervalIndicatorsContainer)
                .AddChild(snapValuesIndicatorsContainer)
                .AddChild(slider);
        }

        private void Compose()
        {
            this
                .AddChild(sliderContainer)
                .AddChild(labelsContainer);
        }

        internal List<float> CleanValues(IEnumerable<float> values)
        {
            float minValue = Mathf.Min(slider.lowValue, slider.highValue);
            float maxValue = Mathf.Max(slider.lowValue, slider.highValue);
            var list = values.Where(v => v >= minValue && v <= maxValue).ToList();
            list.Sort();
            return list;
        }

        internal void UpdateSnapValuesIndicators()
        {
            snapValuesIndicatorsContainer.Clear();
            if (!snapToValues) return;
            if (float.IsNaN(sliderTrackerWidth)) return;
            foreach (float snapValue in CleanValues(snapValues))
            {
                float normalizedValue = (snapValue - slider.lowValue) / length;
                float position = normalizedValue * sliderTrackerWidth + TRACKER_OFFSET;

                Label label =
                    DesignUtils.fieldLabel
                        .SetText(snapValue.ToString(CultureInfo.InvariantCulture))
                        .SetStyleMarginBottom(2)
                        .SetStyleTextAlign(TextAnchor.MiddleCenter)
                        .SetStyleWidth(30)
                        .SetStyleMarginLeft(-14);

                snapValuesIndicatorsContainer
                    .AddChild
                    (
                        new VisualElement()
                            .SetName($"Snap Value Indicator: {snapValue}")
                            .SetStylePosition(Position.Absolute)
                            .SetStyleLeft(position)
                            .AddChild(label)
                            .AddChild
                            (
                                DesignUtils.dividerVertical
                                    .SetStyleMargins(0)
                                    .SetStyleHeight(8)
                            )
                    );

                if (snapValue.Approximately(slider.lowValue) || snapValue.Approximately(slider.highValue))
                {
                    label.visible = false;
                }
            }
        }
    }

    public static class FluidRangeSliderExtensions
    {
        /// <summary> Set an accent color for the slider </summary>
        /// <param name="target"> Target </param>
        /// <param name="color"> Accent color </param>
        public static T SetAccentColor<T>(this T target, Color color) where T : FluidRangeSlider
        {
            target.valueLabel.SetStyleColor(color);
            target.sliderDraggerBorder.SetStyleBackgroundColor(color.WithAlpha(0.2f));
            target.sliderDragger.SetStyleBackgroundColor(color);
            return target;
        }
        
        /// <summary> Adds the given callback to the list of callbacks that will be invoked when the slider value changes. </summary>
        /// <param name="slider"> Target </param>
        /// <param name="callback"> Callback triggered when the slider value changes. </param>
        public static T AddOnValueChangedListener<T>(this T slider, UnityAction<float> callback) where T : FluidRangeSlider
        {
            slider.onValueChanged.AddListener(callback);
            return slider;
        }

        /// <summary> Removes the given callback from the slider's onValueChanged event. </summary>
        /// <param name="slider"> Target </param>
        /// <param name="callback"> Callback </param>
        public static T RemoveOnValueChangedListener<T>(this T slider, UnityAction<float> callback) where T : FluidRangeSlider
        {
            slider.onValueChanged.RemoveListener(callback);
            return slider;
        }

        /// <summary> Removes all listeners from the slider's onValueChanged event. </summary>
        /// <param name="slider"> Target </param>
        public static T RemoveAllOnValueChangedListeners<T>(this T slider) where T : FluidRangeSlider
        {
            slider.onValueChanged.RemoveAllListeners();
            return slider;
        }


        /// <summary> Set whether the slider should snap to a snap interval </summary>
        /// <param name="target"> Target </param>
        /// <param name="value"> Snap to interval </param>
        public static T SetSnapToInterval<T>(this T target, bool value) where T : FluidRangeSlider
        {
            target.snapToInterval = value;
            return target;
        }

        /// <summary> Set a snap interval for the slider. Also sets snap to interval as true </summary>
        /// <param name="target"> Target </param>
        /// <param name="value"> Snap interval value </param>
        public static T SetSnapInterval<T>(this T target, float value) where T : FluidRangeSlider
        {
            target.snapInterval = value;
            target.snapToInterval = true;
            return target;
        }

        /// <summary> Set whether the slider should snap to a set of snap values </summary>
        /// <param name="target"> Target </param>
        /// <param name="value"> Snap to values </param>
        public static T SetSnapToValues<T>(this T target, bool value) where T : FluidRangeSlider
        {
            target.snapToValues = value;
            target.UpdateSnapValuesIndicators();
            return target;
        }

        /// <summary> Set the snap values for the slider. Also sets snap to values as true </summary>
        /// <param name="target"> Target </param>
        /// <param name="snapValuesInterval"> Snap values interval (how close does the slider value need to be to snap to a snap values value) </param>
        /// <param name="values"> Snap values </param>
        public static T SetSnapValues<T>(this T target, float snapValuesInterval, params float[] values) where T : FluidRangeSlider
        {
            if (values.Length == 0)
            {
                throw new ArgumentException("Must have at least one snap value");
            }

            target.snapValues = target.CleanValues(values).ToArray();
            target.snapValuesInterval = snapValuesInterval;
            target.snapToValues = true;
            target.UpdateSnapValuesIndicators();
            return target;
        }

        /// <summary> Set whether the slider should snap to a set auto reset value </summary>
        /// <param name="target"> Target </param>
        /// <param name="value"> Snap to auto reset value </param>
        public static T SetAutoResetToValue<T>(this T target, bool value) where T : FluidRangeSlider
        {
            target.autoResetToValue = value;
            return target;
        }

        /// <summary> Set the auto reset value for the slider. Also sets auto reset to value as true </summary>
        /// <param name="target"> Target </param>
        /// <param name="value"> Auto reset value </param>
        public static T SetAutoResetValue<T>(this T target, float value) where T : FluidRangeSlider
        {
            target.autoResetValue = value;
            target.autoResetToValue = true;
            return target;
        }

        /// <summary> Set the low value of the slider </summary>
        /// <param name="fluidRangeSlider"> Target </param>
        /// <param name="lowValue"> New low value </param>
        public static T SetSliderLowValue<T>(this T fluidRangeSlider, float lowValue) where T : FluidRangeSlider
        {
            fluidRangeSlider.slider.lowValue = lowValue;
            fluidRangeSlider.lowValueLabel.text = lowValue.ToString(CultureInfo.InvariantCulture);
            return fluidRangeSlider;
        }

        /// <summary> Set the high value of the slider </summary>
        /// <param name="fluidRangeSlider"> Target </param>
        /// <param name="highValue"> New high value </param>
        public static T SetSliderHighValue<T>(this T fluidRangeSlider, float highValue) where T : FluidRangeSlider
        {
            fluidRangeSlider.slider.highValue = highValue;
            fluidRangeSlider.highValueLabel.text = highValue.ToString(CultureInfo.InvariantCulture);
            return fluidRangeSlider;
        }

        /// <summary> Set the low and high values of the slider </summary>
        /// <param name="fluidRangeSlider"> Target </param>
        /// <param name="lowValue"> New low value </param>
        /// <param name="highValue"> New high value </param>
        public static T SetSliderLowAndHighValues<T>(this T fluidRangeSlider, float lowValue, float highValue) where T : FluidRangeSlider
        {
            return
                fluidRangeSlider
                    .SetSliderLowValue(lowValue)
                    .SetSliderHighValue(highValue);
        }

        /// <summary> Set the current value of the slider </summary>
        /// <param name="fluidRangeSlider"> Target </param>
        /// <param name="value"> New value </param>
        public static T SetSliderValue<T>(this T fluidRangeSlider, float value) where T : FluidRangeSlider
        {
            fluidRangeSlider.slider.value = value;
            fluidRangeSlider.valueLabel.text = value.ToString(CultureInfo.InvariantCulture);
            return fluidRangeSlider;
        }

        /// <summary> Set the slider's direction </summary>
        /// <param name="fluidRangeSlider"> Target </param>
        /// <param name="direction"> New slider direction </param>
        public static T SetSliderDirection<T>(this T fluidRangeSlider, SliderDirection direction) where T : FluidRangeSlider
        {
            fluidRangeSlider.slider.direction = direction;
            return fluidRangeSlider;
        }
    }
}
