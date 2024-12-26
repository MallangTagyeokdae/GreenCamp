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
using Doozy.Editor.UIElements;
using Doozy.Editor.UIManager.Editors.Components.Internal;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager;
using Doozy.Runtime.UIManager.Animators;
using Doozy.Runtime.UIManager.Components;
using TMPro;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Slider = UnityEngine.UIElements.Slider;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Components
{
    [CustomEditor(typeof(UISlider), true)]
    [CanEditMultipleObjects]
    public class UISliderEditor : UISelectableBaseEditor
    {
        public override Color accentColor => EditorColors.UIManager.UIComponent;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.UIComponent;

        public UISlider castedTarget => (UISlider)target;
        public IEnumerable<UISlider> castedTargets => targets.Cast<UISlider>();

        private FluidTab callbacksTab { get; set; }
        private FluidTab advancedTab { get; set; }
        
        private FluidAnimatedContainer callbacksAnimatedContainer { get; set; }
        private FluidAnimatedContainer advancedAnimatedContainer { get; set; }
        
        private FluidField idField { get; set; }

        private SerializedProperty propertyDirection { get; set; }
        private SerializedProperty propertyFillRect { get; set; }
        private SerializedProperty propertyHandleRect { get; set; }
        private SerializedProperty propertyId { get; set; }
        private SerializedProperty propertyMaxValue { get; set; }
        private SerializedProperty propertyMinValue { get; set; }
        private SerializedProperty propertyOnValueChangedCallback { get; set; }
        private SerializedProperty propertyOnValueChanged { get; set; }
        private SerializedProperty propertyOnValueIncremented { get; set; }
        private SerializedProperty propertyOnValueDecremented { get; set; }
        private SerializedProperty propertyOnValueReset { get; set; }
        private SerializedProperty propertyOnValueReachedMin { get; set; }
        private SerializedProperty propertyOnValueReachedMax { get; set; }
        private SerializedProperty propertyValue { get; set; }
        private SerializedProperty propertyWholeNumbers { get; set; }
        private SerializedProperty propertyTargetProgressor { get; set; }
        private SerializedProperty propertyInstantProgressorUpdate { get; set; }
        private SerializedProperty propertyValueLabel { get; set; }
        private SerializedProperty propertyMinValueLabel { get; set; }
        private SerializedProperty propertyMaxValueLabel { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            callbacksTab?.Dispose();
            advancedTab?.Dispose();
            callbacksAnimatedContainer?.Dispose();
            advancedAnimatedContainer?.Dispose();
            idField?.Recycle();
        }

        protected override void SearchForAnimators()
        {
            selectableAnimators ??= new List<BaseUISelectableAnimator>();
            selectableAnimators.Clear();

            //check if prefab was selected
            if (castedTargets.Any(s => s.gameObject.scene.name == null))
            {
                selectableAnimators.AddRange(castedSelectable.GetComponentsInChildren<BaseUISelectableAnimator>());
                return;
            }

            //not prefab
            selectableAnimators.AddRange(FindObjectsOfType<BaseUISelectableAnimator>());
        }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyDirection = serializedObject.FindProperty("Direction");
            propertyFillRect = serializedObject.FindProperty("FillRect");
            propertyHandleRect = serializedObject.FindProperty("HandleRect");
            propertyId = serializedObject.FindProperty(nameof(UISlider.Id));
            propertyMaxValue = serializedObject.FindProperty("MaxValue");
            propertyMinValue = serializedObject.FindProperty("MinValue");
            #pragma warning disable CS0618
            propertyOnValueChangedCallback = serializedObject.FindProperty(nameof(UISlider.OnValueChangedCallback));
            #pragma warning restore CS0618
            propertyOnValueChanged = serializedObject.FindProperty(nameof(UISlider.OnValueChanged));
            propertyOnValueIncremented = serializedObject.FindProperty(nameof(UISlider.OnValueIncremented));
            propertyOnValueDecremented = serializedObject.FindProperty(nameof(UISlider.OnValueDecremented));
            propertyOnValueReset = serializedObject.FindProperty(nameof(UISlider.OnValueReset));
            propertyOnValueReachedMin = serializedObject.FindProperty(nameof(UISlider.OnValueReachedMin));
            propertyOnValueReachedMax = serializedObject.FindProperty(nameof(UISlider.OnValueReachedMax));
            propertyValueLabel = serializedObject.FindProperty("ValueLabel");
            propertyMinValueLabel = serializedObject.FindProperty("MinValueLabel");
            propertyMaxValueLabel = serializedObject.FindProperty("MaxValueLabel");
            propertyValue = serializedObject.FindProperty("Value");
            propertyWholeNumbers = serializedObject.FindProperty("WholeNumbers");
            propertyTargetProgressor = serializedObject.FindProperty("TargetProgressor");
            propertyInstantProgressorUpdate = serializedObject.FindProperty("InstantProgressorUpdate");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetAccentColor(accentColor)
                .SetComponentNameText((ObjectNames.NicifyVariableName(nameof(UISlider))))
                .SetIcon(EditorSpriteSheets.UIManager.Icons.UISlider)
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1046577351/UISlider?atlOrigin=eyJpIjoiNDM1Yzg5NzdjZDIwNGY1YmI0ZTNmZWQ2YjViYTg5YzEiLCJwIjoiYyJ9")
                .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.UIManager.Components.UISlider.html")
                .AddYouTubeButton();

            idField =
                FluidField.Get()
                    .AddFieldContent(DesignUtils.NewPropertyField(propertyId));

            InitializeCallbacks();
            InitializeAdvanced();

            //refresh tabs enabled indicator
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
                        #pragma warning disable CS0618
                        castedTarget.OnValueChangedCallback.GetPersistentEventCount() > 0 |
                        #pragma warning restore CS0618
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

            //Update visuals
            root.schedule.Execute(() =>
            {
                if (castedTarget == null) return;
                if (serializedObject.isEditingMultipleObjects)
                {
                    foreach (UISlider slider in castedTargets)
                        slider.UpdateVisuals();
                    return;
                }
                castedTarget.UpdateVisuals();
            }).Every(100);
        }

        protected override void InitializeSettings()
        {
            base.InitializeSettings();

            settingsAnimatedContainer.SetOnShowCallback(() =>
            {

                #region Interactable

                FluidToggleCheckbox interactableCheckbox = FluidToggleCheckbox.Get()
                    .SetLabelText("Interactable")
                    .SetTooltip("Can the Selectable be interacted with?")
                    .BindToProperty(propertyInteractable);

                #endregion

                #region Deselect after Press

                FluidToggleCheckbox deselectAfterPressCheckbox = FluidToggleCheckbox.Get()
                    .SetLabelText("Deselect after Press")
                    .BindToProperty(propertyDeselectAfterPress);

                #endregion

                #region Fill

                ObjectField fieldRectObjectField =
                    DesignUtils.NewObjectField(propertyFillRect, typeof(RectTransform))
                        .SetStyleFlexGrow(1);

                FluidField fieldRectFluidField =
                    FluidField.Get()
                        .SetLabelText("Fill")
                        .AddFieldContent(fieldRectObjectField);

                #endregion

                #region Handle

                ObjectField handleRectObjectField =
                    DesignUtils.NewObjectField(propertyHandleRect, typeof(RectTransform))
                        .SetStyleFlexGrow(1);

                FluidField handleRectFieldField =
                    FluidField.Get()
                        .SetLabelText("Handle")
                        .AddFieldContent(handleRectObjectField);

                #endregion

                #region Direction

                EnumField directionEnumField =
                    DesignUtils.NewEnumField(propertyDirection)
                        .SetStyleWidth(120);

                FluidField directionFluidField =
                    FluidField.Get()
                        .SetLabelText("Direction")
                        .AddFieldContent(directionEnumField)
                        .SetStyleFlexGrow(0);

                void UpdateDirection(SlideDirection previousDirection, SlideDirection newDirection)
                {
                    foreach (UISlider slider in castedTargets)
                        slider.SetDirection(previousDirection, newDirection, true);
                }

                directionEnumField.RegisterValueChangedCallback(evt =>
                {
                    if (evt?.newValue == null) return;
                    UpdateDirection((SlideDirection)evt.previousValue, (SlideDirection)evt.newValue);
                });


                root.schedule.Execute(() =>
                {
                    var slideDirection = (SlideDirection)propertyDirection.enumValueIndex;
                    UpdateDirection(slideDirection, slideDirection);
                });

                #endregion

                #region Value

                Slider valueSlider =
                    new Slider()
                        .ResetLayout()
                        .SetStyleFlexGrow(1)
                        .BindToProperty(propertyValue);

                FloatField valueFloatField =
                    DesignUtils.NewFloatField(propertyValue)
                        .BindToProperty(propertyValue)
                        .SetStyleWidth(80);

                FluidField valueFluidField =
                    FluidField.Get()
                        .SetLabelText("Value")
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(valueSlider)
                                .AddChild(DesignUtils.spaceBlock2X)
                                .AddChild(valueFloatField)
                        );

                valueSlider.lowValue = propertyMinValue.floatValue;
                valueSlider.highValue = propertyMaxValue.floatValue;

                valueSlider.RegisterValueChangedCallback(evt =>
                {
                    if (evt?.newValue == null) return;
                    float newValue = castedTarget.wholeNumbers ? Mathf.Round(evt.newValue) : evt.newValue;
                    foreach (UISlider ct in castedTargets)
                        ct.SetValue(newValue);
                });
                
                valueFloatField.RegisterValueChangedCallback(evt =>
                {
                    if (evt?.newValue == null) return;
                    float newValue = castedTarget.wholeNumbers ? Mathf.Round(evt.newValue) : evt.newValue;
                    foreach (UISlider ct in castedTargets)
                        ct.SetValue(newValue);
                });
                
                #endregion

                #region Min Value

                FloatField minValueFloatField =
                    DesignUtils.NewFloatField(propertyMinValue)
                        .SetStyleFlexGrow(1);

                FluidField minValueFluidField =
                    FluidField.Get()
                        .SetLabelText("Min Value")
                        .AddFieldContent(minValueFloatField);

                minValueFloatField.RegisterValueChangedCallback(evt => valueSlider.lowValue = evt.newValue);

                #endregion

                #region Max Value

                FloatField maxValueFloatField =
                    DesignUtils.NewFloatField(propertyMaxValue)
                        .SetStyleFlexGrow(1);

                FluidField maxValueFluidField =
                    FluidField.Get()
                        .SetLabelText("Max Value")
                        .AddFieldContent(maxValueFloatField);

                maxValueFloatField.RegisterValueChangedCallback(evt => valueSlider.highValue = evt.newValue);

                #endregion

                #region Whole Numbers

                FluidToggleCheckbox wholeNumbersToggle =
                    FluidToggleCheckbox.Get()
                        .SetToggleAccentColor(selectableAccentColor)
                        .BindToProperty(propertyWholeNumbers)
                        .SetStyleMargins(-2f, -2f, 0f, -4f);

                FluidField wholeNumbersFluidField =
                    FluidField.Get()
                        .SetLabelText("Whole Numbers")
                        .AddFieldContent(wholeNumbersToggle)
                        .SetStyleFlexGrow(0);

                wholeNumbersToggle.SetOnValueChanged(evt =>
                {
                    if (!evt.newValue) return;
                    valueFloatField.value = Mathf.Round(valueFloatField.value);
                });

                #endregion

                settingsAnimatedContainer
                    .AddContent
                    (
                        DesignUtils.row
                            .AddChild(interactableCheckbox)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(deselectAfterPressCheckbox)
                    )
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent
                    (
                        DesignUtils.row
                            .AddChild(fieldRectFluidField)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(handleRectFieldField)
                    )
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent
                    (
                        DesignUtils.row
                            .AddChild(directionFluidField)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(minValueFluidField)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(maxValueFluidField)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(wholeNumbersFluidField)
                    )
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(valueFluidField)
                    .AddContent(DesignUtils.flexibleSpace)
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
                FluidField valueChangedCallbackFluidField = FluidField.Get().AddFieldContent(DesignUtils.UnityEventField("(Obsolete) On Value Changed Callback", propertyOnValueChangedCallback));
                FluidField valueChangedFluidField = FluidField.Get().AddFieldContent(DesignUtils.UnityEventField("On Value Changed", propertyOnValueChanged));
                FluidField valueIncrementedFluidField = FluidField.Get().AddFieldContent(DesignUtils.UnityEventField("On Value Incremented", propertyOnValueIncremented));
                FluidField valueDecrementedFluidField = FluidField.Get().AddFieldContent(DesignUtils.UnityEventField("On Value Decremented", propertyOnValueDecremented));

                callbacksAnimatedContainer
                    .AddContent(valueChangedCallbackFluidField)
                    .AddContent(DesignUtils.spaceBlock2X)
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

        private void InitializeAdvanced()
        {
            advancedAnimatedContainer = new FluidAnimatedContainer("Advanced", true).Hide(false);
            advancedTab =
                GetTab("Advanced")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Settings)
                    .ButtonSetOnValueChanged(evt => advancedAnimatedContainer.Toggle(evt.newValue, evt.animateChange));

            advancedAnimatedContainer.SetOnShowCallback(() =>
            {
                ObjectField valueLabelObjectField =
                    DesignUtils.NewObjectField(propertyValueLabel, typeof(TMP_Text))
                        .SetTooltip("Reference to the value label that displays the current value of the slider")
                        .SetStyleFlexGrow(1);
                
                ObjectField minValueLabelObjectField =
                    DesignUtils.NewObjectField(propertyMinValueLabel, typeof(TMP_Text))
                        .SetTooltip("Reference to the min value label that displays the min value of the slider")
                        .SetStyleFlexGrow(1);
                
                ObjectField maxValueLabelObjectField =
                    DesignUtils.NewObjectField(propertyMaxValueLabel, typeof(TMP_Text))
                        .SetTooltip("Reference to the max value label that displays the max value of the slider")
                        .SetStyleFlexGrow(1);
                
                FluidField labelsFluidField =
                    FluidField.Get()
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddChild(FluidField.Get("Value Label").AddFieldContent(valueLabelObjectField))
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(FluidField.Get("Min Value Label").AddFieldContent(minValueLabelObjectField))
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(FluidField.Get("Max Value Label").AddFieldContent(maxValueLabelObjectField))
                        );
                
                ObjectField targetProgressorObjectField =
                    DesignUtils.NewObjectField(propertyTargetProgressor, typeof(Progressor))
                        .SetTooltip
                        (
                            "Reference to a Progressor that will be updated when the slider value changes."
                        )
                        .SetStyleFlexGrow(1);

                FluidToggleCheckbox instantProgressorUpdateToggleCheckbox =
                    FluidToggleCheckbox.Get("Instant Progressor Update")
                        .SetTooltip
                        (
                            "When true, the slider will update the target progressor value with SetValueAt instead of PlayToValue.\n\n" +
                            "Basically, if true, the progressor will not animate when the slider value changes. "
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
                
                advancedAnimatedContainer
                    .AddContent(labelsFluidField)
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(progressorFluidField)
                    .Bind(serializedObject);
            });
        }
        
        protected override VisualElement Toolbar()
        {
            return
                toolbarContainer
                    .AddChild(settingsTab)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(advancedTab)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(statesTab)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(behavioursTab)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(callbacksTab)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(navigationTab)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(DesignUtils.flexibleSpace)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild
                    (
                        DesignUtils.SystemButton_RenameComponent
                        (
                            castedTarget.gameObject,
                            () => $"Toggle - {castedTarget.Id.Name}"
                        )
                    )
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild
                    (
                        DesignUtils.SystemButton_SortComponents
                        (
                            ((UISelectable)target).gameObject,
                            nameof(RectTransform),
                            nameof(UISlider)
                        )
                    );
        }

        protected override VisualElement Content()
        {
            return
                contentContainer
                    .AddChild(settingsAnimatedContainer)
                    .AddChild(advancedAnimatedContainer)
                    .AddChild(statesAnimatedContainer)
                    .AddChild(behavioursAnimatedContainer)
                    .AddChild(callbacksAnimatedContainer)
                    .AddChild(navigationAnimatedContainer);
        }

        protected override void Compose()
        {
            root
                .AddChild(reactionControls)
                .AddChild(componentHeader)
                .AddChild(Toolbar())
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(Content())
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(idField)
                .AddChild(DesignUtils.endOfLineBlock);
        }
    }
}
