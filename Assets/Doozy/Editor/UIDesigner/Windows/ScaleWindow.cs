// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.EditorUI.Windows.Internal;
using Doozy.Editor.UIDesigner.Utils;
using Doozy.Editor.UIElements;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIDesigner.Windows
{
    public class ScaleWindow : FluidWindow<ScaleWindow>
    {
        private const string WINDOW_TITLE = "Scale";
        public const string k_WindowMenuPath = "Tools/Doozy/UI Designer/";

        [MenuItem(k_WindowMenuPath + WINDOW_TITLE, priority = -800)]
        public static void Open() => InternalOpenWindow(WINDOW_TITLE);

        private FloatField snapIntervalFloatField { get; set; }
        private FluidField optionsFluidField { get; set; }
        private FluidField scaleIncreaseFluidField { get; set; }
        private FluidField scaleDecreaseFluidField { get; set; }

        private FluidRangeSlider scaleIncreaseSlider { get; set; }
        private FluidRangeSlider scaleDecreaseSlider { get; set; }

        private FluidToggleCheckbox relativeChangeCheckbox { get; set; }
        private FluidToggleCheckbox snapToIntervalCheckbox { get; set; }
        private FluidToggleCheckbox snapToValuesCheckbox { get; set; }

        private FluidToggleButtonTab xTabButton { get; set; }
        private FluidToggleButtonTab yTabButton { get; set; }

        private bool scaleX
        {
            get => EditorPrefs.GetBool(EditorPrefsKey(nameof(scaleX)), true);
            set => EditorPrefs.SetBool(EditorPrefsKey(nameof(scaleX)), value);
        }

        private bool scaleY
        {
            get => EditorPrefs.GetBool(EditorPrefsKey(nameof(scaleY)), true);
            set => EditorPrefs.SetBool(EditorPrefsKey(nameof(scaleY)), value);
        }

        private float snapInterval
        {
            get => EditorPrefs.GetFloat(EditorPrefsKey(nameof(snapInterval)), 0.05f);
            set => EditorPrefs.SetFloat(EditorPrefsKey(nameof(snapInterval)), value);
        }

        private bool snapToInterval
        {
            get => EditorPrefs.GetBool(EditorPrefsKey(nameof(snapToInterval)), true);
            set => EditorPrefs.SetBool(EditorPrefsKey(nameof(snapToInterval)), value);
        }

        private bool snapToValues
        {
            get => EditorPrefs.GetBool(EditorPrefsKey(nameof(snapToValues)), true);
            set => EditorPrefs.SetBool(EditorPrefsKey(nameof(snapToValues)), value);
        }

        private bool relativeChange
        {
            get => EditorPrefs.GetBool(EditorPrefsKey(nameof(relativeChange)), true);
            set => EditorPrefs.SetBool(EditorPrefsKey(nameof(relativeChange)), value);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
            OnSelectionChanged();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (selected.Count == 0)
                return;

            Handles.BeginGUI();
            Color color = Handles.color;
            Handles.color = EditorColors.EditorUI.Amber;
            {
                //ToDo SceneView visuals
            }
            Handles.color = color;
            Handles.EndGUI();
        }

        protected override void CreateGUI()
        {
            Initialize();
            Compose();

            Selection.selectionChanged -= OnSelectionChanged;
            Selection.selectionChanged += OnSelectionChanged;
        }

        private static HashSet<RectTransform> selected { get; set; } = new HashSet<RectTransform>();

        private void OnSelectionChanged() =>
            selected = DesignerEditorUtils.selected;

        private void Initialize()
        {
            root
                .RecycleAndClear()
                .SetStylePadding(DesignUtils.k_Spacing2X);

            #region Increase Scale

            scaleIncreaseSlider =
                new FluidRangeSlider(0, 2)
                    .SetTooltip("Increase the scale the selected objects by or to the specified value.")
                    .SetAccentColor(EditorColors.UIDesigner.Color)
                    .SetSnapInterval(snapInterval)
                    .SetSnapValues(0.02f, 0f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1f, 1.1f, 1.2f, 1.3f, 1.4f, 1.5f, 1.6f, 1.7f, 1.8f, 1.9f, 2f)
                    .SetAutoResetValue(0)
                    .SetSliderValue(0);

            scaleIncreaseSlider.onStartValueChange.AddListener(DesignerEditorUtils.StartScale);
            scaleIncreaseSlider.onEndValueChange.AddListener(DesignerEditorUtils.EndScale);
            scaleIncreaseSlider.onValueChanged.AddListener(UpdateScale);

            scaleIncreaseFluidField =
                FluidField.Get()
                    .SetStyleFlexGrow(0)
                    .SetStyleFlexShrink(0)
                    // .SetLabelText("Increase Scale")
                    .SetIcon(EditorSpriteSheets.UIDesigner.Icons.ScaleIncrease)
                    .AddFieldContent(scaleIncreaseSlider);

            #endregion

            #region Decrease Scale

            scaleDecreaseSlider =
                new FluidRangeSlider(-2, 0)
                    .SetTooltip("Decrease the scale the selected objects by or to the specified value.")
                    .SetAccentColor(EditorColors.UIDesigner.Color)
                    .SetSnapInterval(snapInterval)
                    .SetSnapValues(0.02f, 0f, -0.1f, -0.2f, -0.3f, -0.4f, -0.5f, -0.6f, -0.7f, -0.8f, -0.9f, -1f, -1.1f, -1.2f, -1.3f, -1.4f, -1.5f, -1.6f, -1.7f, -1.8f, -1.9f, -2f)
                    .SetAutoResetValue(0)
                    .SetSliderValue(0);

            scaleDecreaseSlider.onStartValueChange.AddListener(DesignerEditorUtils.StartScale);
            scaleDecreaseSlider.onEndValueChange.AddListener(DesignerEditorUtils.EndScale);
            scaleDecreaseSlider.onValueChanged.AddListener(UpdateScale);

            scaleDecreaseFluidField =
                FluidField.Get()
                    .SetStyleFlexGrow(0)
                    .SetStyleFlexShrink(0)
                    // .SetLabelText("Decrease Scale")
                    .SetIcon(EditorSpriteSheets.UIDesigner.Icons.ScaleDecrease)
                    .AddFieldContent(scaleDecreaseSlider);

            #endregion

            #region Options

            xTabButton =
                GetToggleButton()
                    .SetLabelText("X")
                    .SetTooltip("Scale the selected objects along the X axis by or to the specified value.")
                    .SetOnValueChanged(evt => scaleX = evt.newValue)
                    .SetTabPosition(TabPosition.TabOnLeft);

            yTabButton =
                GetToggleButton()
                    .SetLabelText("Y")
                    .SetTooltip("Scale the selected objects along the Y axis by or to the specified value.")
                    .SetOnValueChanged(evt => scaleY = evt.newValue)
                    .SetTabPosition(TabPosition.TabOnRight);

            xTabButton.SetIsOn(scaleX, false);
            yTabButton.SetIsOn(scaleY, false);

            if (!xTabButton.isOn & !yTabButton.isOn)
            {
                xTabButton.SetIsOn(true, false);
                yTabButton.SetIsOn(true, false);
            }

            relativeChangeCheckbox =
                FluidToggleCheckbox.Get()
                    .SetLabelText("Relative Change")
                    .SetTooltip
                    (
                        "If enabled, the scale will be relative to the current scale of the selected objects.\n" +
                        "If disabled, the scale will be absolute."
                    )
                    .SetToggleAccentColor(EditorSelectableColors.UIDesigner.Color)
                    .SetIsOn(relativeChange, false)
                    .SetOnValueChanged(evt => relativeChange = evt.newValue);

            snapToIntervalCheckbox =
                FluidToggleCheckbox.Get()
                    .SetLabelText("Snap to Interval")
                    .SetTooltip
                    (
                        "If enabled, the scale will snap to the nearest interval.\n" +
                        "If disabled, the scale will be continuous."
                    )
                    .SetToggleAccentColor(EditorSelectableColors.UIDesigner.Color)
                    .SetIsOn(snapToInterval, false)
                    .SetOnValueChanged(evt =>
                    {
                        snapToInterval = evt.newValue;
                        scaleIncreaseSlider.SetSnapToInterval(evt.newValue);
                        scaleDecreaseSlider.SetSnapToInterval(evt.newValue);
                        snapIntervalFloatField.SetEnabled(evt.newValue);
                    });

            snapIntervalFloatField =
                new FloatField()
                    .ResetLayout()
                    .SetStyleWidth(60)
                    .SetTooltip
                    (
                        "The interval to snap to.\n" +
                        "If Snap to Interval is disabled, this value will be ignored."
                    );

            snapIntervalFloatField.value = snapInterval;

            snapIntervalFloatField.RegisterValueChangedCallback(evt =>
            {
                if (evt?.newValue == null) return;
                snapInterval = evt.newValue;
                scaleIncreaseSlider.SetSnapInterval(evt.newValue);
                scaleDecreaseSlider.SetSnapInterval(evt.newValue);
            });

            scaleIncreaseSlider.SetSnapInterval(snapIntervalFloatField.value);
            scaleDecreaseSlider.SetSnapInterval(snapIntervalFloatField.value);

            snapToValuesCheckbox =
                FluidToggleCheckbox.Get()
                    .SetLabelText("Snap to Values")
                    .SetTooltip
                    (
                        "If enabled, the scale will snap to the marked predefined values."
                    )
                    .SetToggleAccentColor(EditorSelectableColors.UIDesigner.Color)
                    .SetIsOn(snapToValues, false)
                    .SetOnValueChanged(evt =>
                    {
                        snapToValues = evt.newValue;
                        scaleIncreaseSlider.SetSnapToValues(evt.newValue);
                        scaleDecreaseSlider.SetSnapToValues(evt.newValue);
                    });

            scaleIncreaseSlider.SetSnapToValues(snapToValuesCheckbox.isOn);
            scaleDecreaseSlider.SetSnapToValues(snapToValuesCheckbox.isOn);

            optionsFluidField =
                FluidField.Get()
                    .SetStyleFlexGrow(0)
                    .SetStyleFlexShrink(0)
                    .AddFieldContent
                    (
                        new VisualElement()
                            .SetName("XYZ - Space - Relative Change")
                            .SetStyleFlexShrink(0)
                            .SetStyleFlexWrap(Wrap.Wrap)
                            .SetStyleFlexDirection(FlexDirection.Row)
                            .AddChild
                            (
                                new VisualElement()
                                    .SetName("XYZ")
                                    .SetStyleFlexDirection(FlexDirection.Row)
                                    .SetStylePadding(DesignUtils.k_Spacing2X)
                                    .AddChild(xTabButton)
                                    .AddSpace(1, 0)
                                    .AddChild(yTabButton)
                            )
                            .AddChild
                            (
                                new VisualElement()
                                    .SetName("Relative Change")
                                    .SetStyleFlexDirection(FlexDirection.Row)
                                    .SetStylePadding(DesignUtils.k_Spacing2X)
                                    .AddChild(relativeChangeCheckbox)
                            )
                    )
                    .AddFieldContent
                    (
                        new VisualElement()
                            .SetName("Snap to Interval - Snap Values")
                            .SetStyleFlexShrink(0)
                            .SetStyleFlexWrap(Wrap.Wrap)
                            .SetStyleFlexDirection(FlexDirection.Row)
                            .AddChild
                            (
                                new VisualElement()
                                    .SetName("Snap to Interval")
                                    .SetStyleFlexDirection(FlexDirection.Row)
                                    .SetStylePadding(DesignUtils.k_Spacing2X)
                                    .AddChild(snapToIntervalCheckbox)
                                    .AddChild(DesignUtils.spaceBlock)
                                    .AddChild(snapIntervalFloatField)
                            )
                            .AddChild
                            (
                                new VisualElement()
                                    .SetName("Snap to Values")
                                    .SetStyleFlexDirection(FlexDirection.Row)
                                    .SetStylePadding(DesignUtils.k_Spacing2X)
                                    .AddChild(snapToValuesCheckbox)
                            )
                    );

            #endregion
        }

        private void Compose()
        {
            root
                .AddChild(scaleIncreaseFluidField)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(scaleDecreaseFluidField)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(optionsFluidField);
        }

        private void UpdateScale(float value)
        {
            if (scaleX & scaleY)
            {
                DesignerEditorUtils.UpdateScaleXY(value, value, relativeChange);
                return;
            }

            if (scaleX)
            {
                DesignerEditorUtils.UpdateScaleX(value, relativeChange);
                return;
            }

            if (scaleY)
            {
                DesignerEditorUtils.UpdateScaleY(value, relativeChange);
                return;
            }

            Debug.Log("UIDesigner - Unable to scale - No scale axis selected");
        }

        private FluidToggleButtonTab GetToggleButton() =>
            FluidToggleButtonTab.Get()
                .SetToggleAccentColor(EditorSelectableColors.UIDesigner.Color)
                .SetStyleFlexShrink(0)
                .SetElementSize(ElementSize.Normal)
                .SetContainerColorOff(DesignUtils.tabButtonColorOff);
    }
}
