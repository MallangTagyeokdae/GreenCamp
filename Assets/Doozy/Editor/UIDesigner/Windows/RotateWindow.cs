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
using Doozy.Runtime.UIDesigner;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Space = Doozy.Runtime.UIDesigner.Space;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIDesigner.Windows
{
    public class RotateWindow : FluidWindow<RotateWindow>
    {
        private const string WINDOW_TITLE = "Rotate";
        public const string k_WindowMenuPath = "Tools/Doozy/UI Designer/";

        [MenuItem(k_WindowMenuPath + WINDOW_TITLE, priority = -800)]
        public static void Open() => InternalOpenWindow(WINDOW_TITLE);

        private FloatField snapIntervalFloatField { get; set; }
        private FluidField optionsFluidField { get; set; }
        private FluidField rotation180ToZeroFluidField { get; set; }
        private FluidField rotationZeroToNegative180FluidField { get; set; }

        private FluidRangeSlider rotation180ToZeroSlider { get; set; }
        private FluidRangeSlider rotationZeroToNegative180Slider { get; set; }

        private FluidToggleCheckbox relativeChangeCheckbox { get; set; }
        private FluidToggleCheckbox snapToIntervalCheckbox { get; set; }
        private FluidToggleCheckbox snapToValuesCheckbox { get; set; }

        private FluidToggleButtonTab xTabButton { get; set; }
        private FluidToggleButtonTab yTabButton { get; set; }
        private FluidToggleButtonTab zTabButton { get; set; }

        private FluidToggleGroup spaceToggleGroup { get; set; }
        private FluidToggleButtonTab localSpaceButton { get; set; }
        private FluidToggleButtonTab worldSpaceButton { get; set; }

        private Space space
        {
            get => (Space)EditorPrefs.GetInt(EditorPrefsKey(nameof(space)), (int)Space.Local);
            set => EditorPrefs.SetInt(EditorPrefsKey(nameof(space)), (int)value);
        }

        private bool rotateX
        {
            get => EditorPrefs.GetBool(EditorPrefsKey(nameof(rotateX)), true);
            set => EditorPrefs.SetBool(EditorPrefsKey(nameof(rotateX)), value);
        }

        private bool rotateY
        {
            get => EditorPrefs.GetBool(EditorPrefsKey(nameof(rotateY)), true);
            set => EditorPrefs.SetBool(EditorPrefsKey(nameof(rotateY)), value);
        }

        private bool rotateZ
        {
            get => EditorPrefs.GetBool(EditorPrefsKey(nameof(rotateZ)), true);
            set => EditorPrefs.SetBool(EditorPrefsKey(nameof(rotateZ)), value);
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
                .SetStylePadding(DesignUtils.k_Spacing2X)
                // .SetStyleBackgroundColor(EditorColors.Default.BoxBackground)
                ;

            #region 0 to -180

            rotationZeroToNegative180Slider =
                new FluidRangeSlider(0, -180)
                    .SetTooltip
                    (
                        "Rotate the selected objects clockwise by or to the specified value."
                    )
                    .SetAccentColor(EditorColors.UIDesigner.Color)
                    .SetSnapInterval(snapInterval)
                    .SetSnapValues(4f, -180, -120, -90, -60, -45, -30, -15, 0)
                    .SetAutoResetValue(0)
                    .SetSliderValue(0);

            rotationZeroToNegative180Slider.onStartValueChange.AddListener(DesignerEditorUtils.StartRotation);
            rotationZeroToNegative180Slider.onEndValueChange.AddListener(DesignerEditorUtils.EndRotation);
            rotationZeroToNegative180Slider.onValueChanged.AddListener(UpdateRotation);

            rotationZeroToNegative180FluidField =
                FluidField.Get()
                    .SetStyleFlexGrow(0)
                    .SetStyleFlexShrink(0)
                    // .SetLabelText("Clockwise")
                    .SetIcon(EditorSpriteSheets.UIDesigner.Icons.RotateRight)
                    .AddFieldContent(rotationZeroToNegative180Slider);

            #endregion

            #region 180 to 0

            rotation180ToZeroSlider =
                new FluidRangeSlider(180, 0)
                    .SetTooltip
                    (
                        "Rotate the selected objects counter-clockwise by or to the specified value."
                    )
                    .SetAccentColor(EditorColors.UIDesigner.Color)
                    .SetSnapInterval(1f)
                    .SetSnapValues(4f, 0, 15, 30, 45, 60, 90, 120, 180)
                    .SetAutoResetValue(0)
                    .SetSliderValue(0);

            rotation180ToZeroSlider.onStartValueChange.AddListener(DesignerEditorUtils.StartRotation);
            rotation180ToZeroSlider.onEndValueChange.AddListener(DesignerEditorUtils.EndRotation);
            rotation180ToZeroSlider.onValueChanged.AddListener(UpdateRotation);

            rotation180ToZeroFluidField =
                FluidField.Get()
                    .SetStyleFlexGrow(0)
                    .SetStyleFlexShrink(0)
                    // .SetLabelText("Counter-Clockwise")
                    .SetIcon(EditorSpriteSheets.UIDesigner.Icons.RotateLeft)
                    .AddFieldContent(rotation180ToZeroSlider);

            #endregion

            #region Options

            xTabButton =
                GetToggleButton()
                    .SetLabelText("X")
                    .SetTooltip("Rotate the selected objects around the X axis by or to the specified value.")
                    .SetOnValueChanged(evt => rotateX = evt.newValue)
                    .SetTabPosition(TabPosition.TabOnLeft);

            yTabButton =
                GetToggleButton()
                    .SetLabelText("Y")
                    .SetTooltip("Rotate the selected objects around the Y axis by or to the specified value.")
                    .SetOnValueChanged(evt => rotateY = evt.newValue)
                    .SetTabPosition(TabPosition.TabInCenter);

            zTabButton =
                GetToggleButton()
                    .SetLabelText("Z")
                    .SetTooltip("Rotate the selected objects around the Z axis by or to the specified value.")
                    .SetOnValueChanged(evt => rotateZ = evt.newValue)
                    .SetTabPosition(TabPosition.TabOnRight);

            xTabButton.SetIsOn(rotateX, false);
            yTabButton.SetIsOn(rotateY, false);
            zTabButton.SetIsOn(rotateZ, false);

            if (!xTabButton.isOn & !yTabButton.isOn & !zTabButton.isOn)
                zTabButton.SetIsOn(true, false);

            localSpaceButton =
                GetToggleButton()
                    .SetLabelText("Local")
                    .SetTooltip("Rotate the selected objects in local space.")
                    .SetOnValueChanged(evt =>
                    {
                        if (!evt.newValue) return;
                        space = Space.Local;
                    })
                    .SetTabPosition(TabPosition.TabOnLeft);

            worldSpaceButton =
                GetToggleButton()
                    .SetLabelText("World")
                    .SetTooltip("Rotate the selected objects in world space.")
                    .SetOnValueChanged(evt =>
                    {
                        if (!evt.newValue) return;
                        space = Space.World;
                    })
                    .SetTabPosition(TabPosition.TabOnRight);

            localSpaceButton.SetIsOn(space == Space.Local, false);
            worldSpaceButton.SetIsOn(space == Space.World, false);

            if (!localSpaceButton.isOn && !worldSpaceButton.isOn)
                localSpaceButton.SetIsOn(true, false);

            spaceToggleGroup =
                FluidToggleGroup.Get()
                    .SetControlMode(FluidToggleGroup.ControlMode.OneToggleOnEnforced);

            localSpaceButton.AddToToggleGroup(spaceToggleGroup);
            worldSpaceButton.AddToToggleGroup(spaceToggleGroup);

            relativeChangeCheckbox =
                FluidToggleCheckbox.Get()
                    .SetLabelText("Relative Change")
                    .SetTooltip
                    (
                        "If enabled, the rotation will be relative to the current rotation of the selected objects.\n" +
                        "If disabled, the rotation will be absolute."
                    )
                    .SetToggleAccentColor(EditorSelectableColors.UIDesigner.Color)
                    .SetIsOn(relativeChange, false)
                    .SetOnValueChanged(evt => relativeChange = evt.newValue);

            snapToIntervalCheckbox =
                FluidToggleCheckbox.Get()
                    .SetLabelText("Snap to Interval")
                    .SetTooltip
                    (
                        "If enabled, the rotation will snap to the nearest interval.\n" +
                        "If disabled, the rotation will be continuous."
                    )
                    .SetToggleAccentColor(EditorSelectableColors.UIDesigner.Color)
                    .SetIsOn(snapToInterval, false)
                    .SetOnValueChanged(evt =>
                    {
                        snapToInterval = evt.newValue;
                        rotationZeroToNegative180Slider.SetSnapToInterval(evt.newValue);
                        rotation180ToZeroSlider.SetSnapToInterval(evt.newValue);
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
                rotationZeroToNegative180Slider.SetSnapInterval(evt.newValue);
                rotation180ToZeroSlider.SetSnapInterval(evt.newValue);
            });

            rotationZeroToNegative180Slider.SetSnapInterval(snapIntervalFloatField.value);
            rotation180ToZeroSlider.SetSnapInterval(snapIntervalFloatField.value);

            snapToValuesCheckbox =
                FluidToggleCheckbox.Get()
                    .SetLabelText("Snap to Values")
                    .SetTooltip
                    (
                        "If enabled, the rotation will snap to the marked predefined values."
                    )
                    .SetToggleAccentColor(EditorSelectableColors.UIDesigner.Color)
                    .SetIsOn(snapToValues, false)
                    .SetOnValueChanged(evt =>
                    {
                        snapToValues = evt.newValue;
                        rotationZeroToNegative180Slider.SetSnapToValues(evt.newValue);
                        rotation180ToZeroSlider.SetSnapToValues(evt.newValue);
                    });

            rotationZeroToNegative180Slider.SetSnapToValues(snapToValuesCheckbox.isOn);
            rotation180ToZeroSlider.SetSnapToValues(snapToValuesCheckbox.isOn);

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
                                    .AddSpace(1, 0)
                                    .AddChild(zTabButton)
                            )
                            .AddChild
                            (
                                new VisualElement()
                                    .SetName("Relative Change")
                                    .SetStyleFlexDirection(FlexDirection.Row)
                                    .SetStylePadding(DesignUtils.k_Spacing2X)
                                    .AddChild(relativeChangeCheckbox)
                            )
                            .AddChild
                            (
                                new VisualElement()
                                    .SetName("Space")
                                    .SetStyleFlexDirection(FlexDirection.Row)
                                    .SetStylePadding(DesignUtils.k_Spacing2X)
                                    .AddChild(localSpaceButton)
                                    .AddSpace(1, 0)
                                    .AddChild(worldSpaceButton)
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
                .AddChild(rotationZeroToNegative180FluidField)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(rotation180ToZeroFluidField)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(optionsFluidField);
        }

        private void UpdateRotation(float value)
        {
            if (rotateX & rotateY & rotateZ)
            {
                DesignerEditorUtils.UpdateRotation(space, value, value, value, relativeChange);
                return;
            }

            if (rotateX & rotateY)
            {
                DesignerEditorUtils.UpdateRotationXY(space, value, value, relativeChange);
                return;
            }

            if (rotateX & rotateZ)
            {
                DesignerEditorUtils.UpdateRotationXZ(space, value, value, relativeChange);
                return;
            }

            if (rotateY & rotateZ)
            {
                DesignerEditorUtils.UpdateRotationYZ(space, value, value, relativeChange);
                return;
            }

            if (rotateX)
            {
                DesignerEditorUtils.UpdateRotation(space, Axis.X, value, relativeChange);
                return;
            }

            if (rotateY)
            {
                DesignerEditorUtils.UpdateRotation(space, Axis.Y, value, relativeChange);
                return;
            }

            if (rotateZ)
            {
                DesignerEditorUtils.UpdateRotation(space, Axis.Z, value, relativeChange);
                return;
            }

            Debug.Log("UIDesigner - Unable to rotate - No rotation axis selected");
        }


        private FluidToggleButtonTab GetToggleButton() =>
            FluidToggleButtonTab.Get()
                .SetToggleAccentColor(EditorSelectableColors.UIDesigner.Color)
                .SetStyleFlexShrink(0)
                .SetElementSize(ElementSize.Normal)
                .SetContainerColorOff(DesignUtils.tabButtonColorOff);
    }
}
