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
using Align = Doozy.Runtime.UIDesigner.Align;
using ObjectNames = Doozy.Runtime.Common.Utils.ObjectNames;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIDesigner.Windows
{
    public class AlignWindow : FluidWindow<AlignWindow>
    {
        private const string WINDOW_TITLE = "Align";
        public const string k_WindowMenuPath = "Tools/Doozy/UI Designer/";

        [MenuItem(k_WindowMenuPath + WINDOW_TITLE, priority = -800)]
        public static void Open() => InternalOpenWindow(WINDOW_TITLE);

        private FluidField alignObjectsFluidField { get; set; }
        private FluidField distributeObjectsFluidField { get; set; }
        private FluidField alignToFluidField { get; set; }

        private FloatField distributeSpacingFloatField { get; set; }

        private FluidButton alignHorizontalCenterButton { get; set; }
        private FluidButton alignHorizontalLeftButton { get; set; }
        private FluidButton alignHorizontalRightButton { get; set; }
        private FluidButton alignVerticalBottomButton { get; set; }
        private FluidButton alignVerticalCenterButton { get; set; }
        private FluidButton alignVerticalTopButton { get; set; }
        private FluidButton distributeHorizontalCenterButton { get; set; }
        private FluidButton distributeHorizontalLeftButton { get; set; }
        private FluidButton distributeHorizontalRightButton { get; set; }
        private FluidButton distributeVerticalBottomButton { get; set; }
        private FluidButton distributeVerticalCenterButton { get; set; }
        private FluidButton distributeVerticalTopButton { get; set; }
        private FluidButton horizontalDistributeSpacingButton { get; set; }
        private FluidButton verticalDistributeSpacingButton { get; set; }

        private FluidToggleButtonTab alignModeCenterButton { get; set; }
        private FluidToggleButtonTab alignModeInsideButton { get; set; }
        private FluidToggleButtonTab alignModeOutsideButton { get; set; }
        private FluidToggleButtonTab alignToKeyObjectButton { get; set; }
        private FluidToggleButtonTab alignToParentButton { get; set; }
        private FluidToggleButtonTab alignToRootCanvasButton { get; set; }
        private FluidToggleButtonTab alignToSelectionButton { get; set; }

        private FluidToggleGroup alignModeToggleGroup { get; set; }
        private FluidToggleGroup alignToToggleGroup { get; set; }

        private FluidToggleCheckbox updateAnchorsCheckbox { get; set; }

        private ObjectField keyObjectField { get; set; }

        private AlignTo alignTo
        {
            get => (AlignTo)EditorPrefs.GetInt(EditorPrefsKey(nameof(alignTo)), (int)AlignTo.RootCanvas);
            set => EditorPrefs.SetInt(EditorPrefsKey(nameof(alignTo)), (int)value);
        }

        private AlignMode alignMode
        {
            get => (AlignMode)EditorPrefs.GetInt(EditorPrefsKey(nameof(alignMode)), (int)AlignMode.Inside);
            set => EditorPrefs.SetInt(EditorPrefsKey(nameof(alignMode)), (int)value);
        }

        private bool updateAnchors
        {
            get => EditorPrefs.GetBool(EditorPrefsKey(nameof(updateAnchors)), true);
            set => EditorPrefs.SetBool(EditorPrefsKey(nameof(updateAnchors)), value);
        }

        private float spacing
        {
            get => EditorPrefs.GetFloat(EditorPrefsKey(nameof(spacing)), 0);
            set => EditorPrefs.SetFloat(EditorPrefsKey(nameof(spacing)), value);
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

            #region Align Objects

            #region Align

            alignHorizontalLeftButton =
                GetButton()
                    .SetTooltip
                    (
                        "Horizontal Align Left\n\n" +
                        "Align the selected objects to the left side of the 'Align To' target."
                    )
                    .SetIcon(EditorSpriteSheets.UIDesigner.Icons.HorizontalAlignLeft)
                    .SetOnClick(() => DesignerEditorUtils.Align(alignTo, Align.HorizontalLeft, alignMode, updateAnchors, (RectTransform)keyObjectField.value));

            alignHorizontalCenterButton =
                GetButton()
                    .SetTooltip
                    (
                        "Horizontal Align Center\n\n" +
                        "Align the selected objects to the horizontal center of the 'Align To' target."
                    )
                    .SetIcon(EditorSpriteSheets.UIDesigner.Icons.HorizontalAlignCenter)
                    .SetOnClick(() => DesignerEditorUtils.Align(alignTo, Align.HorizontalCenter, alignMode, updateAnchors, (RectTransform)keyObjectField.value));

            alignHorizontalRightButton =
                GetButton()
                    .SetTooltip
                    (
                        "Horizontal Align Right\n\n" +
                        "Align the selected objects to the right side of the 'Align To' target."
                    )
                    .SetIcon(EditorSpriteSheets.UIDesigner.Icons.HorizontalAlignRight)
                    .SetOnClick(() => DesignerEditorUtils.Align(alignTo, Align.HorizontalRight, alignMode, updateAnchors, (RectTransform)keyObjectField.value));

            alignVerticalTopButton =
                GetButton()
                    .SetTooltip
                    (
                        "Vertical Align Top\n\n" +
                        "Align the selected objects to the top side of the 'Align To' target."
                    )
                    .SetIcon(EditorSpriteSheets.UIDesigner.Icons.VerticalAlignTop)
                    .SetOnClick(() => DesignerEditorUtils.Align(alignTo, Align.VerticalTop, alignMode, updateAnchors, (RectTransform)keyObjectField.value));

            alignVerticalCenterButton =
                GetButton()
                    .SetTooltip
                    (
                        "Vertical Align Center\n\n" +
                        "Align the selected objects to the vertical center of the 'Align To' target."
                    )
                    .SetIcon(EditorSpriteSheets.UIDesigner.Icons.VerticalAlignCenter)
                    .SetOnClick(() => DesignerEditorUtils.Align(alignTo, Align.VerticalCenter, alignMode, updateAnchors, (RectTransform)keyObjectField.value));

            alignVerticalBottomButton =
                GetButton()
                    .SetTooltip
                    (
                        "Vertical Align Bottom\n\n" +
                        "Align the selected objects to the bottom side of the 'Align To' target."
                    )
                    .SetIcon(EditorSpriteSheets.UIDesigner.Icons.VerticalAlignBottom)
                    .SetOnClick(() => DesignerEditorUtils.Align(alignTo, Align.VerticalBottom, alignMode, updateAnchors, (RectTransform)keyObjectField.value));

            #endregion

            #region Align Mode

            FluidToggleButtonTab GetAlignModeButton(AlignMode mode)
            {
                FluidToggleButtonTab button =
                    GetToggleButton()
                        .SetLabelText($"{mode}")
                        .SetIsOn(mode == alignMode, false)
                        .SetOnValueChanged(evt =>
                        {
                            if (!evt.newValue) return;
                            UpdateAlignMode(mode);
                        });

                return button;
            }

            alignModeInsideButton =
                GetAlignModeButton(AlignMode.Inside)
                    .SetTooltip
                    (
                        "Align Mode: Inside\n\n" +
                        "Align the items on the inside of the 'Align To' target."
                    )
                    .SetIcon(EditorSpriteSheets.UIDesigner.Icons.AlignModeInside)
                    .SetTabPosition(TabPosition.TabOnLeft);

            alignModeCenterButton =
                GetAlignModeButton(AlignMode.Center)
                    .SetTooltip
                    (
                        "Align Mode: Center\n\n" +
                        "Align the items centers on the edge of the 'Align To' target."
                    )
                    .SetIcon(EditorSpriteSheets.UIDesigner.Icons.AlignModeCenter)
                    .SetTabPosition(TabPosition.TabInCenter);

            alignModeOutsideButton =
                GetAlignModeButton(AlignMode.Outside)
                    .SetTooltip
                    (
                        "Align Mode: Outside\n\n" +
                        "Align the items on the outside of the 'Align To' target."
                    )
                    .SetIcon(EditorSpriteSheets.UIDesigner.Icons.AlignModeOutside)
                    .SetTabPosition(TabPosition.TabOnRight);

            alignModeToggleGroup =
                FluidToggleGroup.Get()
                    .SetControlMode(FluidToggleGroup.ControlMode.OneToggleOnEnforced);

            alignModeInsideButton.AddToToggleGroup(alignModeToggleGroup);
            alignModeCenterButton.AddToToggleGroup(alignModeToggleGroup);
            alignModeOutsideButton.AddToToggleGroup(alignModeToggleGroup);

            void UpdateAlignMode(AlignMode mode) =>
                alignMode = mode;

            #endregion

            updateAnchorsCheckbox =
                FluidToggleCheckbox.Get("Update Anchors")
                    .SetTooltip("Align also updates the anchors accordingly.")
                    .SetToggleAccentColor(EditorSelectableColors.UIDesigner.Color)
                    .SetIsOn(updateAnchors, false)
                    .SetOnValueChanged(evt => updateAnchors = evt.newValue);

            alignObjectsFluidField =
                FluidField.Get()
                    // .SetLabelText("Align Objects")
                    .SetStyleFlexGrow(0)
                    .SetStyleFlexShrink(0)
                    .SetElementSize(ElementSize.Small)
                    .AddFieldContent
                    (
                        new VisualElement()
                            .SetName("Align Objects")
                            .SetStyleFlexShrink(0)
                            .SetStyleFlexWrap(Wrap.Wrap)
                            .SetStyleFlexDirection(FlexDirection.Row)
                            .AddChild
                            (
                                new VisualElement()
                                    .SetName("Align")
                                    .SetStyleFlexDirection(FlexDirection.Row)
                                    .SetStylePadding(DesignUtils.k_Spacing2X)
                                    .AddChild(alignHorizontalLeftButton)
                                    .AddChild(DesignUtils.spaceBlock)
                                    .AddChild(alignHorizontalCenterButton)
                                    .AddChild(DesignUtils.spaceBlock)
                                    .AddChild(alignHorizontalRightButton)
                                    .AddChild(DesignUtils.spaceBlock4X)
                                    .AddChild(alignVerticalTopButton)
                                    .AddChild(DesignUtils.spaceBlock)
                                    .AddChild(alignVerticalCenterButton)
                                    .AddChild(DesignUtils.spaceBlock)
                                    .AddChild(alignVerticalBottomButton)
                            )
                            .AddChild
                            (
                                new VisualElement()
                                    .SetName("Align Mode")
                                    .SetStyleFlexDirection(FlexDirection.Row)
                                    .SetStylePadding(DesignUtils.k_Spacing2X)
                                    .AddChild(alignModeInsideButton)
                                    .AddSpace(1, 0)
                                    .AddChild(alignModeCenterButton)
                                    .AddSpace(1, 0)
                                    .AddChild(alignModeOutsideButton)
                            )
                            .AddChild
                            (
                                new VisualElement()
                                    .SetName("Update Anchors")
                                    .SetStylePadding(DesignUtils.k_Spacing2X)
                                    .SetStyleJustifyContent(Justify.Center)
                                    .AddChild(updateAnchorsCheckbox)
                            ));

            #endregion

            #region Distribute Objects

            #region Distribute

            distributeHorizontalLeftButton =
                GetButton()
                    .SetTooltip
                    (
                        "Horizontal Distribute Left\n\n" +
                        "Evenly distribute the selected objects between the leftmost and rightmost objects of the selection, " +
                        "based on the leftmost edges of the objects. Meaning the left edges will have the same horizontal distance between them."
                    )
                    .SetIcon(EditorSpriteSheets.UIDesigner.Icons.HorizontalDistributeLeft)
                    .SetOnClick(() => DesignerEditorUtils.Distribute(alignTo, Distribute.HorizontalLeft));

            distributeHorizontalCenterButton =
                GetButton()
                    .SetTooltip
                    (
                        "Horizontal Distribute Center\n\n" +
                        "Evenly distribute the selected objects between the leftmost and rightmost objects of the selection, " +
                        "based on the horizontal center of the objects. Meaning the horizontal centers will have the same horizontal distance between them."
                    )
                    .SetIcon(EditorSpriteSheets.UIDesigner.Icons.HorizontalDistributeCenter)
                    .SetOnClick(() => DesignerEditorUtils.Distribute(alignTo, Distribute.HorizontalCenter));

            distributeHorizontalRightButton =
                GetButton()
                    .SetTooltip
                    (
                        "Horizontal Distribute Right\n\n" +
                        "Evenly distribute the selected objects between the leftmost and rightmost objects of the selection, " +
                        "based on the rightmost edges of the objects. Meaning the right edges will have the same horizontal distance between them."
                    )
                    .SetIcon(EditorSpriteSheets.UIDesigner.Icons.HorizontalDistributeRight)
                    .SetOnClick(() => DesignerEditorUtils.Distribute(alignTo, Distribute.HorizontalRight));

            distributeVerticalTopButton =
                GetButton()
                    .SetTooltip
                    (
                        "Vertical Distribute Top\n\n" +
                        "Evenly distribute the selected objects between the uppermost and the lowest most objects of the selection, " +
                        "based on the topmost edges of the objects. Meaning the top edges will have the same vertical distance between them."
                    )
                    .SetIcon(EditorSpriteSheets.UIDesigner.Icons.VerticalDistributeTop)
                    .SetOnClick(() => DesignerEditorUtils.Distribute(alignTo, Distribute.VerticalTop));

            distributeVerticalCenterButton =
                GetButton()
                    .SetTooltip
                    (
                        "Vertical Distribute Center\n\n" +
                        "Evenly distribute the selected objects between the uppermost and the lowest most objects of the selection, " +
                        "based on the center of the objects. Meaning the center of the objects will have the same vertical distance between them."
                    )
                    .SetIcon(EditorSpriteSheets.UIDesigner.Icons.VerticalDistributeCenter)
                    .SetOnClick(() => DesignerEditorUtils.Distribute(alignTo, Distribute.VerticalCenter));

            distributeVerticalBottomButton =
                GetButton()
                    .SetTooltip
                    (
                        "Vertical Distribute Bottom\n\n" +
                        "Evenly distribute the selected objects between the uppermost and the lowest most objects of the selection, " +
                        "based on the bottommost edges of the objects. Meaning the bottom edges will have the same vertical distance between them."
                    )
                    .SetIcon(EditorSpriteSheets.UIDesigner.Icons.VerticalDistributeBottom)
                    .SetOnClick(() => DesignerEditorUtils.Distribute(alignTo, Distribute.VerticalBottom));

            verticalDistributeSpacingButton =
                GetButton()
                    .SetTooltip
                    (
                        "Vertical Distribute Spacing\n\n" +
                        "Distribute the items evenly spaced vertically.\n\n" +
                        "Note: Align to Key Object uses a custom value when setting the space between each item."
                    )
                    .SetIcon(EditorSpriteSheets.UIDesigner.Icons.DistributeSpacingVertical)
                    .SetOnClick(() => DesignerEditorUtils.VerticalDistributeSpacing(alignTo, (RectTransform)keyObjectField.value, spacing));

            #endregion

            #region Distribute Spacing

            horizontalDistributeSpacingButton =
                GetButton()
                    .SetTooltip
                    (
                        "Horizontal Distribute Spacing\n\n" +
                        "Distribute the items evenly spaced horizontally.\n\n" +
                        "Note: Align to Key Object uses a custom value when setting the space between each item."
                    )
                    .SetIcon(EditorSpriteSheets.UIDesigner.Icons.DistributeSpacingHorizontal)
                    .SetOnClick(() => DesignerEditorUtils.HorizontalDistributeSpacing(alignTo, (RectTransform)keyObjectField.value, spacing));

            distributeSpacingFloatField =
                new FloatField()
                    .ResetLayout()
                    .SetStyleWidth(60)
                    .SetTooltip("Custom spacing value used when distributing items vertically or horizontally");

            distributeSpacingFloatField.SetEnabled(alignTo == AlignTo.KeyObject);
            distributeSpacingFloatField.value = spacing;
            distributeSpacingFloatField.RegisterValueChangedCallback(evt =>
            {
                spacing = evt.newValue;
            });

            #endregion

            distributeObjectsFluidField =
                FluidField.Get()
                    // .SetLabelText("Distribute Objects")
                    .SetStyleFlexGrow(0)
                    .SetStyleFlexShrink(0)
                    .SetElementSize(ElementSize.Small)
                    .AddFieldContent
                    (
                        new VisualElement()
                            .SetName("Distribute Objects")
                            .SetStyleFlexShrink(0)
                            .SetStyleFlexWrap(Wrap.Wrap)
                            .SetStyleFlexDirection(FlexDirection.Row)
                            .AddChild
                            (
                                new VisualElement()
                                    .SetName("Distribute")
                                    .SetStyleFlexDirection(FlexDirection.Row)
                                    .SetStylePadding(DesignUtils.k_Spacing2X)
                                    .AddChild(distributeVerticalTopButton)
                                    .AddChild(DesignUtils.spaceBlock)
                                    .AddChild(distributeVerticalCenterButton)
                                    .AddChild(DesignUtils.spaceBlock)
                                    .AddChild(distributeVerticalBottomButton)
                                    .AddChild(DesignUtils.spaceBlock4X)
                                    .AddChild(distributeHorizontalLeftButton)
                                    .AddChild(DesignUtils.spaceBlock)
                                    .AddChild(distributeHorizontalCenterButton)
                                    .AddChild(DesignUtils.spaceBlock)
                                    .AddChild(distributeHorizontalRightButton)
                            )
                            .AddChild
                            (
                                new VisualElement()
                                    .SetName("Distribute Spacing")
                                    .SetStyleFlexDirection(FlexDirection.Row)
                                    .SetStylePadding(DesignUtils.k_Spacing2X)
                                    .AddChild(verticalDistributeSpacingButton)
                                    .AddChild(DesignUtils.spaceBlock)
                                    .AddChild(horizontalDistributeSpacingButton)
                                    .AddChild(DesignUtils.spaceBlock2X)
                                    .AddChild(distributeSpacingFloatField)
                            )
                    );

            #endregion

            #region Align to

            FluidToggleButtonTab GetAlignToButton(AlignTo to, string tooltip) =>
                GetToggleButton()
                    // .SetStyleWidth(96)
                    .SetLabelText(ObjectNames.NicifyVariableName($"{to}"))
                    .SetTooltip(tooltip)
                    .SetIsOn(to == alignTo, false)
                    .SetOnValueChanged(evt =>
                    {
                        if (!evt.newValue) return;
                        UpdateAlignTo(to);
                    });

            alignToRootCanvasButton =
                GetAlignToButton(AlignTo.RootCanvas, "Align to Root Canvas")
                    .SetIcon(EditorSpriteSheets.UIDesigner.Icons.RootCanvas)
                    .SetTabPosition(TabPosition.TabOnLeft)
                    .SetTooltip
                    (
                        "Align To: Root Canvas\n\n" +
                        "Align and distribute the selected objects to the Root Canvas of the first selected object."
                    );

            alignToParentButton =
                GetAlignToButton(AlignTo.Parent, "Align to Parent")
                    .SetIcon(EditorSpriteSheets.UIDesigner.Icons.Parent)
                    .SetTabPosition(TabPosition.TabInCenter)
                    .SetTooltip
                    (
                        "Align To: Parent\n\n" +
                        "Align the selected objects to their respective parent object.\n\n" +
                        "Distribute the selected objects to the parent of the first object."
                    );

            alignToSelectionButton =
                GetAlignToButton(AlignTo.Selection, "Align to Selection")
                    .SetIcon(EditorSpriteSheets.UIDesigner.Icons.Selection)
                    .SetTabPosition(TabPosition.TabOnRight)
                    .SetTooltip
                    (
                        "Align To: Selection\n\n" +
                        "Align and distribute the selected objects to the selection bounding box."
                    );

            alignToKeyObjectButton =
                GetAlignToButton(AlignTo.KeyObject, "Align to Key Object")
                    .SetIcon(EditorSpriteSheets.UIDesigner.Icons.KeyObject)
                    .SetTabPosition(TabPosition.TabOnLeft)
                    .SetTooltip
                    (
                        "Align To: Key Object\n\n" +
                        "Align and distribute the selected objects to a key object.\n\n" +
                        "When Align to Key Object is selected, a custom spacing value can be set to distribute the objects"
                    );

            alignToToggleGroup =
                FluidToggleGroup.Get()
                    .SetControlMode(FluidToggleGroup.ControlMode.OneToggleOnEnforced);

            alignToRootCanvasButton.AddToToggleGroup(alignToToggleGroup);
            alignToParentButton.AddToToggleGroup(alignToToggleGroup);
            alignToSelectionButton.AddToToggleGroup(alignToToggleGroup);
            alignToKeyObjectButton.AddToToggleGroup(alignToToggleGroup);

            void UpdateAlignTo(AlignTo to)
            {
                alignTo = to;
                distributeSpacingFloatField?.SetEnabled(alignTo == AlignTo.KeyObject);
            }

            keyObjectField =
                new ObjectField()
                    .SetObjectType(typeof(RectTransform))
                    .ResetLayout()
                    .SetStyleWidth(158)
                    .SetTooltip
                    (
                        "Reference to the Key Object used to align and distribute the selected objects.\n\n" +
                        "When Align to Key Object is selected, a custom spacing value can be set to distribute the objects"
                    );

            keyObjectField.RegisterValueChangedCallback(evt =>
            {
                if (evt == null) return;
                if (evt.newValue != null)
                {
                    alignToKeyObjectButton?.SetIsOn(true);
                    return;
                }
                alignToRootCanvasButton?.SetIsOn(true);
            });

            alignToFluidField =
                FluidField.Get()
                    // .SetLabelText("Align To")
                    .SetStyleFlexGrow(0)
                    .SetStyleFlexShrink(0)
                    .SetElementSize(ElementSize.Small)
                    .AddFieldContent
                    (
                        new VisualElement()
                            .SetName("Align To")
                            .SetStyleFlexShrink(0)
                            .SetStyleFlexWrap(Wrap.Wrap)
                            .SetStyleFlexDirection(FlexDirection.Column)
                            .SetStylePadding(DesignUtils.k_Spacing2X)
                            .AddChild
                            (
                                new VisualElement()
                                    .SetStyleFlexDirection(FlexDirection.Row)
                                    .AddChild(alignToRootCanvasButton)
                                    .AddSpace(1, 0)
                                    .AddChild(alignToParentButton)
                                    .AddSpace(1, 0)
                                    .AddChild(alignToSelectionButton)
                            )
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(
                                new VisualElement()
                                    .SetStyleFlexDirection(FlexDirection.Row)
                                    .SetStyleAlignItems(UnityEngine.UIElements.Align.Center)
                                    .AddChild(alignToKeyObjectButton)
                                    .AddSpace(1, 0)
                                    .AddChild(keyObjectField)
                            )
                    );

            #endregion
        }

        private void Compose()
        {
            root
                .AddChild(alignObjectsFluidField)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(distributeObjectsFluidField)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(alignToFluidField)
                ;
        }

        private FluidButton GetButton() =>
            FluidButton.Get()
                .SetStyleFlexShrink(0)
                .SetButtonStyle(ButtonStyle.Contained)
                .SetElementSize(ElementSize.Small);

        private FluidToggleButtonTab GetToggleButton() =>
            FluidToggleButtonTab.Get()
                .SetToggleAccentColor(EditorSelectableColors.UIDesigner.Color)
                .SetStyleFlexShrink(0)
                .SetElementSize(ElementSize.Small)
                .SetContainerColorOff(DesignUtils.tabButtonColorOff);

    }
}
