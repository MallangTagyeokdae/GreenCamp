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
    public class SizeWindow : FluidWindow<SizeWindow>
    {
        private const string WINDOW_TITLE = "Size";
        public const string k_WindowMenuPath = "Tools/Doozy/UI Designer/";

        [MenuItem(k_WindowMenuPath + WINDOW_TITLE, priority = -800)]
        public static void Open() => InternalOpenWindow(WINDOW_TITLE);

        private FloatField snapIntervalFloatField { get; set; }
        private FluidField optionsFluidField { get; set; }
        private FluidField sizeIncreaseFluidField { get; set; }
        private FluidField sizeDecreaseFluidField { get; set; }

        private FluidRangeSlider sizeIncreaseSlider { get; set; }
        private FluidRangeSlider sizeDecreaseSlider { get; set; }

        private FluidToggleCheckbox relativeChangeCheckbox { get; set; }
        private FluidToggleCheckbox snapToIntervalCheckbox { get; set; }
        private FluidToggleCheckbox snapToValuesCheckbox { get; set; }

        private FluidToggleButtonTab xTabButton { get; set; }
        private FluidToggleButtonTab yTabButton { get; set; }

        private bool sizeX
        {
            get => EditorPrefs.GetBool(EditorPrefsKey(nameof(sizeX)), true);
            set => EditorPrefs.SetBool(EditorPrefsKey(nameof(sizeX)), value);
        }

        private bool sizeY
        {
            get => EditorPrefs.GetBool(EditorPrefsKey(nameof(sizeY)), true);
            set => EditorPrefs.SetBool(EditorPrefsKey(nameof(sizeY)), value);
        }

        private float snapInterval
        {
            get => EditorPrefs.GetFloat(EditorPrefsKey(nameof(snapInterval)), 1f);
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

            #region Increase Size

            sizeIncreaseSlider =
                new FluidRangeSlider(0, 128)
                    .SetTooltip("Increase the delta size of the selected objects by or to the specified value.")
                    .SetAccentColor(EditorColors.UIDesigner.Color)
                    .SetSnapInterval(snapInterval)
                    .SetSnapValues(1f, 0f, 4f, 8f, 12f, 16f, 20f, 24f, 32f, 40f, 48f, 56, 64f, 72f, 80f, 88f, 96f, 104f, 112f, 120f, 128f)
                    .SetAutoResetValue(0)
                    .SetSliderValue(0);

            sizeIncreaseSlider.onStartValueChange.AddListener(DesignerEditorUtils.StartSize);
            sizeIncreaseSlider.onEndValueChange.AddListener(DesignerEditorUtils.EndSize);
            sizeIncreaseSlider.onValueChanged.AddListener(UpdateSize);

            sizeIncreaseFluidField =
                FluidField.Get()
                    .SetStyleFlexGrow(0)
                    .SetStyleFlexShrink(0)
                    // .SetLabelText("Increase Size")
                    .SetIcon(EditorSpriteSheets.UIDesigner.Icons.SizeIncrease)
                    .AddFieldContent(sizeIncreaseSlider);

            #endregion

            #region Decrease Size

            sizeDecreaseSlider =
                new FluidRangeSlider(-128, 0)
                    .SetTooltip("Decrease the delta size of the selected objects by or to the specified value.")
                    .SetAccentColor(EditorColors.UIDesigner.Color)
                    .SetSnapInterval(snapInterval)
                    .SetSnapValues(1f, 0f, -4f, -8f, -12f, -16f, -20f, -24f, -32f, -40f, -48f, -56, -64f, -72f, -80f, -88f, -96f, -104f, -112f, -120f, -128f)
                    .SetAutoResetValue(0)
                    .SetSliderValue(0);

            sizeDecreaseSlider.onStartValueChange.AddListener(DesignerEditorUtils.StartSize);
            sizeDecreaseSlider.onEndValueChange.AddListener(DesignerEditorUtils.EndSize);
            sizeDecreaseSlider.onValueChanged.AddListener(UpdateSize);

            sizeDecreaseFluidField =
                FluidField.Get()
                    .SetStyleFlexGrow(0)
                    .SetStyleFlexShrink(0)
                    // .SetLabelText("Decrease Size")
                    .SetIcon(EditorSpriteSheets.UIDesigner.Icons.SizeDecrease)
                    .AddFieldContent(sizeDecreaseSlider);

            #endregion


            #region Options

            xTabButton =
                GetToggleButton()
                    .SetLabelText("X")
                    .SetTooltip("Change the delta size of the selected objects along the X axis by or to the specified value.")
                    .SetOnValueChanged(evt => sizeX = evt.newValue)
                    .SetTabPosition(TabPosition.TabOnLeft);

            yTabButton =
                GetToggleButton()
                    .SetLabelText("Y")
                    .SetTooltip("Change the delta size of the selected objects along the Y axis by or to the specified value.")
                    .SetOnValueChanged(evt => sizeY = evt.newValue)
                    .SetTabPosition(TabPosition.TabOnRight);

            xTabButton.SetIsOn(sizeX, false);
            yTabButton.SetIsOn(sizeY, false);

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
                        "If enabled, the change in size will be relative to the current delta size of the selected objects.\n" +
                        "If disabled, the change in size will be absolute."
                    )
                    .SetToggleAccentColor(EditorSelectableColors.UIDesigner.Color)
                    .SetIsOn(relativeChange, false)
                    .SetOnValueChanged(evt => relativeChange = evt.newValue);

            snapToIntervalCheckbox =
                FluidToggleCheckbox.Get()
                    .SetLabelText("Snap to Interval")
                    .SetTooltip
                    (
                        "If enabled, the size change will snap to the nearest interval.\n" +
                        "If disabled, the size change will be continuous."
                    )
                    .SetToggleAccentColor(EditorSelectableColors.UIDesigner.Color)
                    .SetIsOn(snapToInterval, false)
                    .SetOnValueChanged(evt =>
                    {
                        snapToInterval = evt.newValue;
                        sizeIncreaseSlider.SetSnapToInterval(evt.newValue);
                        sizeDecreaseSlider.SetSnapToInterval(evt.newValue);
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
                sizeIncreaseSlider.SetSnapInterval(evt.newValue);
                sizeDecreaseSlider.SetSnapInterval(evt.newValue);
            });

            sizeIncreaseSlider.SetSnapInterval(snapIntervalFloatField.value);
            sizeDecreaseSlider.SetSnapInterval(snapIntervalFloatField.value);

            snapToValuesCheckbox =
                FluidToggleCheckbox.Get()
                    .SetLabelText("Snap to Values")
                    .SetTooltip
                    (
                        "If enabled, the size will snap to the marked predefined values."
                    )
                    .SetToggleAccentColor(EditorSelectableColors.UIDesigner.Color)
                    .SetIsOn(snapToValues, false)
                    .SetOnValueChanged(evt =>
                    {
                        snapToValues = evt.newValue;
                        sizeIncreaseSlider.SetSnapToValues(evt.newValue);
                        sizeDecreaseSlider.SetSnapToValues(evt.newValue);
                    });

            sizeIncreaseSlider.SetSnapToValues(snapToValuesCheckbox.isOn);
            sizeDecreaseSlider.SetSnapToValues(snapToValuesCheckbox.isOn);

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
                .AddChild(sizeIncreaseFluidField)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(sizeDecreaseFluidField)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(optionsFluidField);
        }

        private void UpdateSize(float value)
        {
            if (sizeX & sizeY)
            {
                DesignerEditorUtils.UpdateSizeXY(value, value, relativeChange);
                return;
            }

            if (sizeX)
            {
                DesignerEditorUtils.UpdateSizeX(value, relativeChange);
                return;
            }

            if (sizeY)
            {
                DesignerEditorUtils.UpdateSizeY(value, relativeChange);
                return;
            }

            Debug.Log("UIDesigner - Unable to change delta size - No size axis selected");
        }

        private FluidToggleButtonTab GetToggleButton() =>
            FluidToggleButtonTab.Get()
                .SetToggleAccentColor(EditorSelectableColors.UIDesigner.Color)
                .SetStyleFlexShrink(0)
                .SetElementSize(ElementSize.Normal)
                .SetContainerColorOff(DesignUtils.tabButtonColorOff);
    }
}
