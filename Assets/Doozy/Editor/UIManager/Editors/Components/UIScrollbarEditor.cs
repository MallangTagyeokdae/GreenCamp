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
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager;
using Doozy.Runtime.UIManager.Animators;
using Doozy.Runtime.UIManager.Components;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Slider = UnityEngine.UIElements.Slider;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Components
{
    [CustomEditor(typeof(UIScrollbar), true)]
    [CanEditMultipleObjects]
    public class UIScrollbarEditor : UISelectableBaseEditor
    {
        public override Color accentColor => EditorColors.UIManager.UIComponent;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.UIComponent;

        public UIScrollbar castedTarget => (UIScrollbar)target;
        public IEnumerable<UIScrollbar> castedTargets => targets.Cast<UIScrollbar>();

        private FluidTab callbacksTab { get; set; }
        private FluidAnimatedContainer callbacksAnimatedContainer { get; set; }
        private FluidField idField { get; set; }

        private SerializedProperty propertyDirection { get; set; }
        private SerializedProperty propertyHandleRect { get; set; }
        private SerializedProperty propertyNumberOfSteps { get; set; }
        private SerializedProperty propertyOnValueChanged { get; set; }
        private SerializedProperty propertySize { get; set; }
        private SerializedProperty propertyValue { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            callbacksTab?.Dispose();
            callbacksAnimatedContainer?.Dispose();
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
            propertyHandleRect = serializedObject.FindProperty("HandleRect");
            propertyNumberOfSteps = serializedObject.FindProperty("NumberOfSteps");
            propertyOnValueChanged = serializedObject.FindProperty(nameof(UIScrollbar.OnValueChangedCallback));
            propertySize = serializedObject.FindProperty("Size");
            propertyValue = serializedObject.FindProperty("Value");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetAccentColor(accentColor)
                .SetComponentNameText("UIScrollbar")
                .SetIcon(EditorSpriteSheets.UIManager.Icons.UIScrollbar)
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048936453/UIScrollbar?atlOrigin=eyJpIjoiZGVkNTE4NjRkMjA3NGZjY2FjYTVjNzhhMzE4ZDRkMmMiLCJwIjoiYyJ9")
                .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.UIManager.Components.UIScrollbar.html")
                .AddYouTubeButton();

            InitializeCallbacks();

            //refresh tabs enabled indicator
            root.schedule.Execute(() =>
            {
                void UpdateIndicator(FluidTab tab, bool toggleOn, bool animateChange)
                {
                    if (tab == null) return;
                    if (tab.indicator.isOn != toggleOn)
                        tab.indicator.Toggle(toggleOn, animateChange);
                }

                bool HasCallbacks()
                {
                    if (castedTarget == null)
                        return false;
      
                    return castedTarget.OnValueChangedCallback.GetPersistentEventCount() > 0;
                }

                //initial indicators state update (no animation)
                UpdateIndicator(callbacksTab, HasCallbacks(), false);

                //subsequent indicators state update (animated)
                root.schedule.Execute(() =>
                {
                    UpdateIndicator(callbacksTab, HasCallbacks(), true);

                }).Every(200);
            });
            
            //Update visuals
            root.schedule.Execute(() =>
            {
                if (castedTarget == null) return;
                if (serializedObject.isEditingMultipleObjects)
                {
                    foreach (UIScrollbar scrollbar in castedTargets)
                        scrollbar.UpdateVisuals();
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

                #region Handle

                ObjectField handleRectObjectField =
                    DesignUtils.NewObjectField(propertyHandleRect, typeof(RectTransform))
                        .SetStyleFlexGrow(1);

                FluidField handleRectFluidField =
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

                void UpdateDirection(SlideDirection direction)
                {
                    if (serializedObject.isEditingMultipleObjects)
                    {
                        foreach (UIScrollbar scrollbar in castedTargets)
                            scrollbar.SetDirection(direction, true);
                        return;
                    }

                    castedTarget.SetDirection(direction, true);
                }

                directionEnumField.RegisterValueChangedCallback(evt =>
                {
                    if (evt?.newValue == null) return;
                    UpdateDirection((SlideDirection)evt.newValue);
                });

                root.schedule.Execute(() =>
                {
                    UpdateDirection((SlideDirection)propertyDirection.enumValueIndex);
                });

                #endregion

                #region Number of Steps

                SliderInt numberOfStepsSliderInt =
                    new SliderInt()
                        .ResetLayout()
                        .SetStyleFlexGrow(1)
                        .BindToProperty(propertyNumberOfSteps);

                IntegerField numberOfStepsIntegerField =
                    DesignUtils.NewIntegerField(propertyNumberOfSteps)
                        .SetStyleWidth(30);

                FluidField numberOfStepsFluidField =
                    FluidField.Get()
                        .SetLabelText("Number of Steps")
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(numberOfStepsSliderInt)
                                .AddChild(DesignUtils.spaceBlock2X)
                                .AddChild(numberOfStepsIntegerField)
                        );

                numberOfStepsSliderInt.lowValue = UIScrollbar.k_MINNumberOfSteps;
                numberOfStepsSliderInt.highValue = UIScrollbar.k_MAXNumberOfSteps;
                numberOfStepsIntegerField.RegisterValueChangedCallback(evt =>
                {
                    if (castedTarget == null) return;
                    numberOfStepsIntegerField.value = numberOfStepsIntegerField.value.Clamp(UIScrollbar.k_MINNumberOfSteps, UIScrollbar.k_MAXNumberOfSteps);
                    castedTarget.UpdateVisuals();
                });

                #endregion

                #region Size

                Slider sizeSlider =
                    new Slider()
                        .ResetLayout()
                        .SetStyleFlexGrow(1)
                        .BindToProperty(propertySize);

                FloatField sizeFloatField =
                    DesignUtils.NewFloatField(propertySize)
                        .SetStyleWidth(60);

                FluidField sizeFluidField =
                    FluidField.Get()
                        .SetLabelText("Size")
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(sizeSlider)
                                .AddChild(DesignUtils.spaceBlock2X)
                                .AddChild(sizeFloatField)
                        );

                sizeSlider.lowValue = UIScrollbar.k_MINValue;
                sizeSlider.highValue = UIScrollbar.k_MAXValue;
                sizeFloatField.RegisterValueChangedCallback(evt =>
                {
                    if (castedTarget == null) return;
                    sizeFloatField.value = sizeFloatField.value.Clamp01().Round(3);
                    castedTarget.UpdateVisuals();
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
                        .SetStyleWidth(60);

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

                valueSlider.lowValue = UIScrollbar.k_MINValue;
                valueSlider.highValue = UIScrollbar.k_MAXValue;
                valueFloatField.RegisterValueChangedCallback(evt =>
                {
                    if (castedTarget == null) return;
                    int steps = propertyNumberOfSteps.intValue;
                    float newValue = evt.newValue;
                    newValue = steps > 0 ? Mathf.Round(newValue * (steps - 1)) / (steps - 1) : newValue;
                    newValue = newValue.Clamp01().Round(3);
                    if (!propertyValue.floatValue.Approximately(newValue))
                    {
                        propertyValue.floatValue = newValue;
                        serializedObject.ApplyModifiedPropertiesWithoutUndo();
                    }
                    castedTarget.UpdateVisuals();
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
                    .AddContent(handleRectFluidField)
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent
                    (
                        DesignUtils.row
                            .AddChild(directionFluidField)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(numberOfStepsFluidField)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(sizeFluidField)
                    )
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(valueFluidField)
                    .AddContent(DesignUtils.endOfLineBlock)
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
                FluidField GetField(SerializedProperty property, string fieldLabelText) =>
                    FluidField.Get()
                        .SetLabelText(fieldLabelText)
                        .SetElementSize(ElementSize.Small)
                        .AddFieldContent(DesignUtils.NewPropertyField(property));

                callbacksAnimatedContainer
                    .AddContent(GetField(propertyOnValueChanged, "Scrollbar Value Changed"))
                    .AddContent(DesignUtils.endOfLineBlock)
                    .Bind(serializedObject);
            });
        }

        protected override VisualElement Toolbar()
        {
            return
                toolbarContainer
                    .AddChild(settingsTab)
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
                        DesignUtils.SystemButton_SortComponents
                        (
                            ((UISelectable)target).gameObject,
                            nameof(RectTransform),
                            nameof(UIScrollbar)
                        )
                    );
        }

        protected override VisualElement Content()
        {
            return
                contentContainer
                    .AddChild(settingsAnimatedContainer)
                    .AddChild(statesAnimatedContainer)
                    .AddChild(behavioursAnimatedContainer)
                    .AddChild(callbacksAnimatedContainer)
                    .AddChild(navigationAnimatedContainer);
        }
    }
}
