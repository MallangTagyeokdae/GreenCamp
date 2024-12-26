// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Components;
using TMPro;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Editors.Components
{
    [CustomEditor(typeof(UIStepper), true)]
    [CanEditMultipleObjects]
    public class UIStepperEditor : UnityEditor.Editor
    {
        public Color accentColor => EditorColors.UIManager.UIComponent;
        public EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.UIComponent;

        public UIStepper castedTarget => (UIStepper)target;
        public IEnumerable<UIStepper> castedTargets => targets.Cast<UIStepper>();

        private VisualElement root { get; set; }
        private FluidComponentHeader componentHeader { get; set; }
        private VisualElement toolbarContainer { get; set; }
        private VisualElement contentContainer { get; set; }

        private FluidToggleGroup tabsGroup { get; set; }
        private FluidTab settingsTab { get; set; }
        private FluidTab dragTab { get; set; }
        private FluidTab advancedTab { get; set; }
        private FluidTab callbacksTab { get; set; }

        private FluidAnimatedContainer settingsAnimatedContainer { get; set; }
        private FluidAnimatedContainer dragAnimatedContainer { get; set; }
        private FluidAnimatedContainer advancedAnimatedContainer { get; set; }
        private FluidAnimatedContainer callbacksAnimatedContainer { get; set; }

        private FluidField idField { get; set; }

        private SerializedProperty propertyId { get; set; }
        private SerializedProperty propertyAutoRepeatWaitTime { get; set; }
        private SerializedProperty propertyAutoRepeatWaitTimeReduction { get; set; }
        private SerializedProperty propertyAutoRepeatMinWaitTime { get; set; }
        private SerializedProperty propertyMinusButton { get; set; }
        private SerializedProperty propertyPlusButton { get; set; }
        private SerializedProperty propertyResetButton { get; set; }
        private SerializedProperty propertyTargetLabel { get; set; }
        private SerializedProperty propertyMinValue { get; set; }
        private SerializedProperty propertyMaxValue { get; set; }
        private SerializedProperty propertyValue { get; set; }
        private SerializedProperty propertyDefaultValue { get; set; }
        private SerializedProperty propertyResetValueOnEnable { get; set; }
        private SerializedProperty propertyStep { get; set; }
        private SerializedProperty propertyTargetSlider { get; set; }
        private SerializedProperty propertyTargetProgressor { get; set; }
        private SerializedProperty propertyInstantProgressorUpdate { get; set; }
        private SerializedProperty propertyValuePrecision { get; set; }
        private SerializedProperty propertyDragEnabled { get; set; }
        private SerializedProperty propertyDragHandle { get; set; }
        private SerializedProperty propertyDragDirection { get; set; }
        private SerializedProperty propertyMaxDragDistance { get; set; }
        private SerializedProperty propertyOnValueChanged { get; set; }
        private SerializedProperty propertyOnValueIncremented { get; set; }
        private SerializedProperty propertyOnValueDecremented { get; set; }
        private SerializedProperty propertyOnValueReset { get; set; }
        private SerializedProperty propertyOnValueReachedMin { get; set; }
        private SerializedProperty propertyOnValueReachedMax { get; set; }

        private void OnDestroy()
        {
            componentHeader?.Recycle();

            tabsGroup?.Recycle();
            settingsTab?.Dispose();
            advancedTab?.Dispose();
            callbacksTab?.Dispose();
            settingsAnimatedContainer?.Dispose();
            advancedAnimatedContainer?.Dispose();
            callbacksAnimatedContainer?.Dispose();
        }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        private void FindProperties()
        {
            propertyId = serializedObject.FindProperty(nameof(UIStepper.Id));
            propertyAutoRepeatWaitTime = serializedObject.FindProperty("AutoRepeatWaitTime");
            propertyAutoRepeatWaitTimeReduction = serializedObject.FindProperty("AutoRepeatWaitTimeReduction");
            propertyAutoRepeatMinWaitTime = serializedObject.FindProperty("AutoRepeatMinWaitTime");
            propertyMinusButton = serializedObject.FindProperty("MinusButton");
            propertyPlusButton = serializedObject.FindProperty("PlusButton");
            propertyResetButton = serializedObject.FindProperty("ResetButton");
            propertyTargetLabel = serializedObject.FindProperty("TargetLabel");
            propertyMinValue = serializedObject.FindProperty("MinValue");
            propertyMaxValue = serializedObject.FindProperty("MaxValue");
            propertyValue = serializedObject.FindProperty("Value");
            propertyDefaultValue = serializedObject.FindProperty("DefaultValue");
            propertyResetValueOnEnable = serializedObject.FindProperty("ResetValueOnEnable");
            propertyStep = serializedObject.FindProperty("Step");
            propertyTargetSlider = serializedObject.FindProperty("TargetSlider");
            propertyTargetProgressor = serializedObject.FindProperty("TargetProgressor");
            propertyInstantProgressorUpdate = serializedObject.FindProperty("InstantProgressorUpdate");
            propertyValuePrecision = serializedObject.FindProperty("ValuePrecision");
            propertyDragEnabled = serializedObject.FindProperty("DragEnabled");
            propertyDragHandle = serializedObject.FindProperty("DragHandle");
            propertyDragDirection = serializedObject.FindProperty("DragDirection");
            propertyMaxDragDistance = serializedObject.FindProperty("MaxDragDistance");
            propertyOnValueChanged = serializedObject.FindProperty("OnValueChanged");
            propertyOnValueIncremented = serializedObject.FindProperty("OnValueIncremented");
            propertyOnValueDecremented = serializedObject.FindProperty("OnValueDecremented");
            propertyOnValueReset = serializedObject.FindProperty("OnValueReset");
            propertyOnValueReachedMin = serializedObject.FindProperty("OnValueReachedMin");
            propertyOnValueReachedMax = serializedObject.FindProperty("OnValueReachedMax");
        }

        private void InitializeEditor()
        {
            FindProperties();
            root = DesignUtils.GetEditorRoot();
            componentHeader =
                DesignUtils.editorComponentHeader
                    .SetAccentColor(accentColor)
                    .SetComponentNameText("UIStepper")
                    .SetIcon(EditorSpriteSheets.UIManager.Icons.UIStepper)
                    .AddManualButton()
                    .AddYouTubeButton();
            toolbarContainer = DesignUtils.editorToolbarContainer;
            tabsGroup = FluidToggleGroup.Get().SetControlMode(FluidToggleGroup.ControlMode.OneToggleOn);
            contentContainer = DesignUtils.editorContentContainer;

            idField =
                FluidField.Get()
                    .AddFieldContent(DesignUtils.NewPropertyField(propertyId));

            InitializeSettings();
            InitializeDrag();
            InitializeAdvanced();
            InitializeCallbacks();

            root.schedule.Execute(() => settingsTab.ButtonSetIsOn(true, false));

            //refresh callbacks tab enabled indicator
            root.schedule.Execute(() =>
            {
                void UpdateIndicator(FluidTab tab, bool toggleOn, bool animateChange)
                {
                    if(tab == null) return;
                    if (tab.indicator.isOn != toggleOn)
                        tab.indicator.Toggle(toggleOn, animateChange);
                }

                bool HasCallbacks()
                {
                    if (castedTarget == null)
                        return false;
                    
                    return  
                        castedTarget.OnValueChanged.GetPersistentEventCount() > 0 |
                        castedTarget.OnValueIncremented.GetPersistentEventCount() > 0 |
                        castedTarget.OnValueDecremented.GetPersistentEventCount() > 0 |
                        castedTarget.OnValueReset.hasCallbacks |
                        castedTarget.OnValueReachedMin.hasCallbacks |
                        castedTarget.OnValueReachedMax.hasCallbacks;
                }
                
                //initial indicators state update (no animation)
                UpdateIndicator(callbacksTab, HasCallbacks(), false);

                root.schedule.Execute(() =>
                {
                    //update indicators state (with animation)
                    UpdateIndicator(callbacksTab, HasCallbacks(), true);
                    
                }).Every(200);
            });
            
            //refresh drag tab enabled indicator
            root.schedule.Execute(() =>
            {
                void UpdateIndicator(FluidTab tab, bool toggleOn, bool animateChange)
                {
                    if(tab == null) return;
                    if (tab.indicator.isOn != toggleOn)
                        tab.indicator.Toggle(toggleOn, animateChange);
                }

                bool HasDrag()
                {
                    if (castedTarget == null)
                        return false;
                    
                    return castedTarget.DragEnabled;
                }
                
                //initial indicators state update (no animation)
                UpdateIndicator(dragTab, HasDrag(), false);

                root.schedule.Execute(() =>
                {
                    //update indicators state (with animation)
                    UpdateIndicator(dragTab, HasDrag(), true);
                    
                }).Every(200);
            });
        }

        private void InitializeSettings()
        {
            settingsAnimatedContainer = new FluidAnimatedContainer("Settings", true).Hide(false);
            settingsTab =
                GetTab("Settings")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Settings)
                    .ButtonSetOnValueChanged(evt => settingsAnimatedContainer.Toggle(evt.newValue, evt.animateChange));

            settingsAnimatedContainer.SetOnShowCallback(() =>
            {
                FloatField minValueFloatField =
                    DesignUtils.NewFloatField(propertyMinValue)
                        .SetTooltip("The minimum value that the stepper can have")
                        .SetStyleFlexGrow(1);

                FloatField maxValueFloatField =
                    DesignUtils.NewFloatField(propertyMaxValue)
                        .SetTooltip("The maximum value that the stepper can reach")
                        .SetStyleFlexGrow(1);

                FluidField minMaxResetValuesFluidField =
                    FluidField.Get()
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddChild(FluidField.Get("Min Value").AddFieldContent(minValueFloatField))
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(FluidField.Get("Max Value").AddFieldContent(maxValueFloatField))
                        );

                FloatField valueFloatField =
                    DesignUtils.NewFloatField(propertyValue)
                        .SetTooltip("The current value of the stepper")
                        .SetStyleFlexGrow(1);

                FloatField stepFloatField =
                    DesignUtils.NewFloatField(propertyStep)
                        .SetTooltip("Value by which the stepper will increase or decrease when the value is changed ")
                        .SetStyleFlexGrow(1);

                IntegerField valuePrecisionFloatField =
                    DesignUtils.NewIntegerField(propertyValuePrecision)
                        .SetTooltip("Number of decimal places to round the value to, if the step is not a whole number")
                        .SetStyleFlexGrow(1);

                FluidField valueFluidField =
                    FluidField.Get()
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddChild(FluidField.Get("Current Value").AddFieldContent(valueFloatField))
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(FluidField.Get("Increment/Decrement Step").AddFieldContent(stepFloatField))
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(FluidField.Get("Value Precision").AddFieldContent(valuePrecisionFloatField))
                        );

                FloatField defaultValueFloatField =
                    DesignUtils.NewFloatField(propertyDefaultValue)
                        .SetTooltip("Reset value for the stepper")
                        .SetStyleFlexGrow(1);

                FluidToggleCheckbox resetValueOnEnableToggleCheckbox =
                    FluidToggleCheckbox.Get("Reset To Default Value OnEnable")
                        .SetTooltip("Automatically resets the value to the default value when the stepper OnEnable")
                        .SetToggleAccentColor(selectableAccentColor)
                        .BindToProperty(propertyResetValueOnEnable)
                        .SetStyleAlignSelf(Align.FlexEnd);

                FluidField resetValueFluidField =
                    FluidField.Get()
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddChild(FluidField.Get("Default (Reset) Value").AddFieldContent(defaultValueFloatField))
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(resetValueOnEnableToggleCheckbox)
                        );

                settingsAnimatedContainer
                    .AddContent(minMaxResetValuesFluidField)
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(valueFluidField)
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(resetValueFluidField)
                    .Bind(serializedObject);
            });
        }

        private void InitializeDrag()
        {
            dragAnimatedContainer = new FluidAnimatedContainer("Drag", true).Hide(true);
            dragTab =
                GetTab("Drag")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Drag)
                    .ButtonSetOnValueChanged(evt => dragAnimatedContainer.Toggle(evt.newValue, evt.animateChange));

            dragAnimatedContainer.SetOnShowCallback(() =>
            {
                FluidToggleSwitch dragEnabledToggleSwitch =
                    FluidToggleSwitch.Get("Drag Enabled")
                        .SetTooltip("Enable or disable the drag functionality for this stepper.")
                        .SetToggleAccentColor(selectableAccentColor)
                        .BindToProperty(propertyDragEnabled);

                ObjectField dragHandleObjectField =
                    DesignUtils.NewObjectField(propertyDragHandle, typeof(RectTransform))
                        .SetTooltip("Reference to the RectTransform used as the drag handle.")
                        .SetStyleFlexGrow(1);

                FluidField dragHandleFluidField =
                    FluidField.Get()
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddChild(FluidField.Get("Drag Handle").AddFieldContent(dragHandleObjectField))
                        );

                EnumField stepperDirectionField =
                    DesignUtils.NewEnumField(propertyDragDirection)
                        .SetTooltip("Drag direction for the Drag Handle to increase or decrease the value")
                        .SetStyleFlexGrow(1);

                FluidField stepperDirectionFluidField =
                    FluidField.Get()
                        .SetLabelText("Drag Direction")
                        .AddFieldContent(stepperDirectionField);

                FloatField maxDragDistanceField =
                    DesignUtils.NewFloatField(propertyMaxDragDistance)
                        .SetTooltip
                        (
                            "During a drag operation, this is the maximum distance the drag handle can be moved from the initial position.\n\n" +
                            "This also affects the speed of the value change (inversely proportional).\n\n" +
                            "The longer the distance the finer the control the user has to change the value."
                        )
                        .SetStyleFlexGrow(1);

                FluidField maxDragDistanceFluidField =
                    FluidField.Get()
                        .SetLabelText("Max Drag Distance")
                        .AddFieldContent(maxDragDistanceField);

                dragAnimatedContainer
                    .AddContent(dragEnabledToggleSwitch)
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(dragHandleFluidField)
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(stepperDirectionFluidField)
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(maxDragDistanceFluidField)
                    .Bind(serializedObject);
            });
        }

        private void InitializeAdvanced()
        {
            advancedAnimatedContainer = new FluidAnimatedContainer("Advanced", true).Hide(true);
            advancedTab =
                GetTab("Advanced")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Settings)
                    .ButtonSetOnValueChanged(evt => advancedAnimatedContainer.Toggle(evt.newValue, evt.animateChange));

            advancedAnimatedContainer.SetOnShowCallback(() =>
            {
                FluidField autoRepeatWaitTimeFluidField =
                    FluidField.Get<FloatField>(propertyAutoRepeatWaitTime, "Wait Time")
                        .SetTooltip
                        (
                            "The initial and maximum time in seconds to wait before the value starts to auto repeat (increment/decrement)"
                        );

                FluidField autoRepeatWaitTimeReductionFluidField =
                    FluidField.Get<FloatField>(propertyAutoRepeatWaitTimeReduction, "Wait Time Reduction")
                        .SetTooltip
                        (
                            "When the stepper is auto-repeating, the wait time between each increase/decrease will be reduced by multiplying the remaining wait time with this value until it reaches AutoRepeatMinWaitTime limit.\n\n" +
                            "This reduction makes the stepper feel more responsive and less laggy."
                        );

                FluidField autoRepeatMinWaitTimeFluidField =
                    FluidField.Get<FloatField>(propertyAutoRepeatMinWaitTime, "Min Wait Time")
                        .SetTooltip
                        (
                            "The minimum wait time between each increase/decrease when the stepper is auto-repeating"
                        );

                FluidField autoRepeatFluidField =
                    FluidField.Get("Auto Repeat")
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddChild(autoRepeatWaitTimeFluidField)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(autoRepeatWaitTimeReductionFluidField)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(autoRepeatMinWaitTimeFluidField)
                        );

                ObjectField minusButtonObjectField =
                    DesignUtils.NewObjectField(propertyMinusButton, typeof(UIButton))
                        .SetTooltip("Reference to a UIButton that will be used to decrease the value of the stepper")
                        .SetStyleFlexGrow(1);

                ObjectField plusButtonObjectField =
                    DesignUtils.NewObjectField(propertyPlusButton, typeof(UIButton))
                        .SetTooltip("Reference to a UIButton that will be used to increase the value of the stepper")
                        .SetStyleFlexGrow(1);

                ObjectField resetButtonObjectField =
                    DesignUtils.NewObjectField(propertyResetButton, typeof(UIButton))
                        .SetTooltip("Reference to a UIButton that will reset the stepper value to its default value")
                        .SetStyleFlexGrow(1);

                FluidField buttonsFluidField =
                    FluidField.Get()
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddChild(FluidField.Get("Minus Button").AddFieldContent(minusButtonObjectField))
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(FluidField.Get("Plus Button").AddFieldContent(plusButtonObjectField))
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(FluidField.Get("Reset Button").AddFieldContent(resetButtonObjectField))
                        );

                ObjectField targetLabelObjectField =
                    DesignUtils.NewObjectField(propertyTargetLabel, typeof(TMP_Text))
                        .SetTooltip("Reference to the value label that displays the current value")
                        .SetStyleFlexGrow(1);

                FluidField targetLabelFluidField =
                    FluidField.Get()
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddChild(FluidField.Get("Target Label").AddFieldContent(targetLabelObjectField))
                        );

                ObjectField targetProgressorObjectField =
                    DesignUtils.NewObjectField(propertyTargetProgressor, typeof(Progressor))
                        .SetTooltip
                        (
                            "Reference to a Progressor that will be updated when the stepper value changes."
                        )
                        .SetStyleFlexGrow(1);

                FluidToggleCheckbox instantProgressorUpdateToggleCheckbox =
                    FluidToggleCheckbox.Get("Instant Progressor Update")
                        .SetTooltip
                        (
                            "When true, the stepper will update the target progressor value with SetValueAt instead of PlayToValue.\n\n" +
                            "Basically, if true, the progressor will not animate when the stepper value changes. "
                        )
                        .SetToggleAccentColor(selectableAccentColor)
                        .BindToProperty(propertyInstantProgressorUpdate)
                        .SetStyleAlignSelf(Align.FlexEnd);

                FluidField progressorFluidField =
                    FluidField.Get()
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddChild(FluidField.Get("Target Progressor").AddFieldContent(targetProgressorObjectField))
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(instantProgressorUpdateToggleCheckbox)
                        );

                ObjectField targetSliderObjectField =
                    DesignUtils.NewObjectField(propertyTargetSlider, typeof(UISlider))
                        .SetTooltip
                        (
                            "Reference to a UISlider that will be updated when the stepper value changes.\n\n" +
                            "The slider value also updates the stepper value, when it changes."
                        )
                        .SetStyleFlexGrow(1);

                FluidField targetSliderFluidField =
                    FluidField.Get()
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddChild(FluidField.Get("Target Slider").AddFieldContent(targetSliderObjectField))
                        );

                advancedAnimatedContainer
                    .AddContent(autoRepeatFluidField)
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(buttonsFluidField)
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(targetLabelFluidField)
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(targetSliderFluidField)
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(progressorFluidField)
                    .Bind(serializedObject);
            });
        }

        private void InitializeCallbacks()
        {
            callbacksAnimatedContainer = new FluidAnimatedContainer("Callbacks", true).Hide(false);
            callbacksTab =
                GetTab("Callbacks")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.UnityEvent)
                    .IndicatorSetEnabledColor(DesignUtils.callbacksColor)
                    .ButtonSetAccentColor(DesignUtils.callbackSelectableColor)
                    .ButtonSetOnValueChanged(evt => callbacksAnimatedContainer.Toggle(evt.newValue, evt.animateChange));

            callbacksAnimatedContainer.SetOnShowCallback(() =>
            {
                FluidField valueChangedFluidField = FluidField.Get().AddFieldContent(DesignUtils.UnityEventField("On Value Changed", propertyOnValueChanged));
                FluidField valueIncrementedFluidField = FluidField.Get().AddFieldContent(DesignUtils.UnityEventField("On Value Incremented", propertyOnValueIncremented));
                FluidField valueDecrementedFluidField = FluidField.Get().AddFieldContent(DesignUtils.UnityEventField("On Value Decremented", propertyOnValueDecremented));

                callbacksAnimatedContainer
                    .AddContent(valueChangedFluidField)
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(valueIncrementedFluidField)
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(valueDecrementedFluidField)
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(DesignUtils.NewPropertyField(propertyOnValueReset))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(DesignUtils.NewPropertyField(propertyOnValueReachedMin))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(DesignUtils.NewPropertyField(propertyOnValueReachedMax))
                    .Bind(serializedObject);
            });
        }

        private VisualElement Toolbar()
        {
            return
                toolbarContainer
                    .AddChild(settingsTab)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(dragTab)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(advancedTab)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(callbacksTab)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(DesignUtils.flexibleSpace)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild
                    (
                        DesignUtils.SystemButton_RenameComponent
                        (
                            castedTarget.gameObject,
                            () => $"Stepper - {castedTarget.Id.Name}"
                        )
                    )
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild
                    (
                        DesignUtils.SystemButton_SortComponents
                        (
                            castedTarget.gameObject,
                            nameof(RectTransform),
                            nameof(UIStepper)
                        )
                    );
        }

        private VisualElement Content()
        {
            return
                contentContainer
                    .AddChild(settingsAnimatedContainer)
                    .AddChild(dragAnimatedContainer)
                    .AddChild(advancedAnimatedContainer)
                    .AddChild(callbacksAnimatedContainer);
        }

        private void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild(Toolbar())
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(Content())
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(idField)
                .AddChild(DesignUtils.endOfLineBlock);
        }

        private FluidTab GetTab(string labelText) =>
            new FluidTab()
                .SetLabelText(labelText)
                .IndicatorSetEnabledColor(accentColor)
                .ButtonSetAccentColor(selectableAccentColor)
                .AddToToggleGroup(tabsGroup);
    }
}
