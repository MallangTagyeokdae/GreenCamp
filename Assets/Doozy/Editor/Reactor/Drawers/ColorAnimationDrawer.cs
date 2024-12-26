// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Globalization;
using Doozy.Editor.Common.Extensions;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Doozy.Editor.Reactor.Drawers
{
    [CustomPropertyDrawer(typeof(ColorAnimation), true)]
    public class ColorAnimationDrawer : PropertyDrawer
    {
        private static Color accentColor => EditorColors.Reactor.Red;
        private static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Reactor.Red;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var drawer = new VisualElement();
            if (property == null) return drawer;
            drawer.RegisterCallback<DetachFromPanelEvent>(evt => drawer.RecycleAndClear());
            var target = property.GetTargetObjectOfProperty() as ColorAnimation;

            #region SerializedProperties

            //Animation
            SerializedProperty propertyAnimation = property.FindPropertyRelative("Animation");
            SerializedProperty propertyAnimationEnabled = propertyAnimation.FindPropertyRelative("Enabled");
            //CALLBACKS            
            SerializedProperty propertyOnPlayCallback = property.FindPropertyRelative("OnPlayCallback");
            SerializedProperty propertyOnStopCallback = property.FindPropertyRelative("OnStopCallback");
            SerializedProperty propertyOnFinishCallback = property.FindPropertyRelative("OnFinishCallback");

            #endregion

            #region ComponentHeader

            FluidComponentHeader componentHeader =
                FluidComponentHeader.Get()
                    .SetAccentColor(accentColor)
                    .SetElementSize(ElementSize.Tiny)
                    .SetComponentNameText("Color")
                    .SetComponentTypeText("Animation")
                    .AddManualButton("www.bit.ly/DoozyKnowledgeBase4")
                    .AddApiButton()
                    .AddYouTubeButton();

            #endregion

            #region Containers

            VisualElement contentContainer = new VisualElement().SetName("Content Container").SetStyleFlexGrow(1);
            FluidAnimatedContainer settingsAnimatedContainer = new FluidAnimatedContainer("Animation", true).Hide(false);
            FluidAnimatedContainer callbacksAnimatedContainer = new FluidAnimatedContainer("Callbacks", true).Hide(false);

            //settings container content
            settingsAnimatedContainer.SetOnShowCallback(() =>
            {
                settingsAnimatedContainer
                    .AddContent(GetAnimationContent(propertyAnimation, propertyAnimationEnabled))
                    .Bind(property.serializedObject);
            });

            //callbacks container content
            callbacksAnimatedContainer.SetOnShowCallback(() =>
            {
                callbacksAnimatedContainer
                    .AddContent
                    (
                        FluidField.Get()
                            .AddFieldContent(DesignUtils.NewPropertyField(propertyOnPlayCallback.propertyPath))
                            .AddFieldContent(DesignUtils.spaceBlock)
                            .AddFieldContent(DesignUtils.NewPropertyField(propertyOnStopCallback.propertyPath))
                            .AddFieldContent(DesignUtils.spaceBlock)
                            .AddFieldContent(DesignUtils.NewPropertyField(propertyOnFinishCallback.propertyPath))
                    )
                    .AddContent(DesignUtils.endOfLineBlock)
                    .Bind(property.serializedObject);
            });

            #endregion

            #region Toolbar

            VisualElement toolbarContainer =
                new VisualElement()
                    .SetName("Toolbar Container")
                    .SetStyleFlexDirection(FlexDirection.Row)
                    .SetStyleMarginTop(-1)
                    .SetStyleMarginLeft(4)
                    .SetStyleMarginRight(4)
                    .SetStyleFlexGrow(1);

            FluidTab settingsTab =
                FluidTab.Get()
                    .SetLabelText("Settings")
                    .SetElementSize(ElementSize.Small)
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Settings)
                    .ButtonSetAccentColor(selectableAccentColor)
                    .IndicatorSetEnabledColor(accentColor)
                    .ButtonSetOnValueChanged(evt => settingsAnimatedContainer.Toggle(evt.newValue));

            FluidTab callbacksTab =
                FluidTab.Get()
                    .SetLabelText("Callbacks")
                    .SetElementSize(ElementSize.Small)
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.UnityEvent)
                    .ButtonSetAccentColor(DesignUtils.callbackSelectableColor)
                    .IndicatorSetEnabledColor(DesignUtils.callbacksColor)
                    .ButtonSetOnValueChanged(evt => callbacksAnimatedContainer.Toggle(evt.newValue));

            //create tabs group
            FluidToggleGroup tabsGroup = FluidToggleGroup.Get().SetControlMode(FluidToggleGroup.ControlMode.OneToggleOn);
            settingsTab.button.AddToToggleGroup(tabsGroup);
            callbacksTab.button.AddToToggleGroup(tabsGroup);

            //update tab indicators
            drawer.schedule.Execute(() =>
            {
                void UpdateIndicator(FluidTab fluidTab, bool toggleOn, bool animateChange)
                {
                    if (fluidTab.indicator.isOn == toggleOn) return;
                    fluidTab.indicator.Toggle(toggleOn, animateChange);
                }

                bool HasCallbacks() =>
                    target != null &&
                    target.OnPlayCallback?.GetPersistentEventCount() > 0 |  //HasOnPlayCallback
                    target.OnStopCallback?.GetPersistentEventCount() > 0 |  //HasOnPlayCallback
                    target.OnFinishCallback?.GetPersistentEventCount() > 0; //HasOnFinishCallback

                //initial indicators state update (no animation)
                UpdateIndicator(settingsTab, propertyAnimationEnabled.boolValue, false);
                UpdateIndicator(callbacksTab, HasCallbacks(), false);

                drawer.schedule.Execute(() =>
                {
                    //subsequent indicators state update (animated)
                    UpdateIndicator(settingsTab, propertyAnimationEnabled.boolValue, true);
                    UpdateIndicator(callbacksTab, HasCallbacks(), true);

                }).Every(200);
            });
            
            toolbarContainer
                .AddChild(settingsTab)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(callbacksTab)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(DesignUtils.flexibleSpace);

            #endregion

            #region Compose

            drawer
                .AddChild(componentHeader)
                .AddChild(toolbarContainer)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild
                (
                    contentContainer
                        .AddChild(settingsAnimatedContainer)
                        .AddChild(callbacksAnimatedContainer)
                );

            #endregion

            return drawer;
        }

        private const int HEIGHT = 42;

        private static VisualElement GetAnimationContent
        (
            SerializedProperty propertyAnimation,
            SerializedProperty propertyAnimationEnabled
        )
        {
            SerializedProperty propertyFromReferenceValue = propertyAnimation.FindPropertyRelative("FromReferenceValue");
            SerializedProperty propertyToReferenceValue = propertyAnimation.FindPropertyRelative("ToReferenceValue");

            SerializedProperty propertyFromCustomValue = propertyAnimation.FindPropertyRelative("FromCustomValue");
            SerializedProperty propertyToCustomValue = propertyAnimation.FindPropertyRelative("ToCustomValue");

            SerializedProperty propertyFromHueOffset = propertyAnimation.FindPropertyRelative("FromHueOffset");
            SerializedProperty propertyToHueOffset = propertyAnimation.FindPropertyRelative("ToHueOffset");

            SerializedProperty propertyFromSaturationOffset = propertyAnimation.FindPropertyRelative("FromSaturationOffset");
            SerializedProperty propertyToSaturationOffset = propertyAnimation.FindPropertyRelative("ToSaturationOffset");

            SerializedProperty propertyFromLightnessOffset = propertyAnimation.FindPropertyRelative("FromLightnessOffset");
            SerializedProperty propertyToLightnessOffset = propertyAnimation.FindPropertyRelative("ToLightnessOffset");

            SerializedProperty propertyFromAlphaOffset = propertyAnimation.FindPropertyRelative("FromAlphaOffset");
            SerializedProperty propertyToAlphaOffset = propertyAnimation.FindPropertyRelative("ToAlphaOffset");

            SerializedProperty propertySettings = propertyAnimation.FindPropertyRelative("Settings");

            var content = new VisualElement();
            content.SetEnabled(propertyAnimationEnabled.boolValue);
            FluidToggleSwitch enableSwitch = DesignUtils.GetEnableDisableSwitch(propertyAnimationEnabled, content, selectableAccentColor, "Animation");


            FluidField fromReferenceValueFluidField = FluidField.Get<EnumField>(propertyFromReferenceValue, "From Color").SetStyleHeight(HEIGHT, HEIGHT, HEIGHT);
            FluidField toReferenceValueFluidField = FluidField.Get<EnumField>(propertyToReferenceValue, "To Color").SetStyleHeight(HEIGHT, HEIGHT, HEIGHT);

            #region Hue

            Label fromHueOffsetLabel = GetOffsetLabel(() => (propertyFromHueOffset.floatValue * 360).Round(0).ToString(CultureInfo.InvariantCulture));
            Label toHueOffsetLabel = GetOffsetLabel(() => (propertyToHueOffset.floatValue * 360).Round(0).ToString(CultureInfo.InvariantCulture));

            Slider fromHueOffsetSlider = DesignUtils.NewSlider(propertyFromHueOffset, -1f, 1f);
            Slider toHueOffsetSlider = DesignUtils.NewSlider(propertyToHueOffset, -1f, 1f);

            fromHueOffsetSlider.RegisterValueChangedCallback(evt =>
                fromHueOffsetLabel.SetText((evt.newValue * 360).Round(0).ToString(CultureInfo.InvariantCulture)));

            toHueOffsetSlider.RegisterValueChangedCallback(evt =>
                toHueOffsetLabel.SetText((evt.newValue * 360).Round(0).ToString(CultureInfo.InvariantCulture)));

            FluidField fromHueOffsetFluidField =
                GetOffsetField
                (
                    "Hue Offset", fromHueOffsetLabel, fromHueOffsetSlider,
                    () =>
                    {
                        propertyFromHueOffset.floatValue = 0f;
                        propertyFromHueOffset.serializedObject.ApplyModifiedProperties();
                    });

            FluidField toHueOffsetFluidField =
                GetOffsetField
                (
                    "Hue Offset", toHueOffsetLabel, toHueOffsetSlider,
                    () =>
                    {
                        propertyToHueOffset.floatValue = 0f;
                        propertyToHueOffset.serializedObject.ApplyModifiedProperties();
                    });

            #endregion

            #region Saturation

            Label fromSaturationOffsetLabel = GetOffsetLabel(() => $"{(propertyFromSaturationOffset.floatValue * 100).Round(0)}%");
            Label toSaturationOffsetLabel = GetOffsetLabel(() => $"{(propertyToSaturationOffset.floatValue * 100).Round(0)}%");

            Slider fromSaturationOffsetSlider = DesignUtils.NewSlider(propertyFromSaturationOffset, -1f, 1f);
            Slider toSaturationOffsetSlider = DesignUtils.NewSlider(propertyToSaturationOffset, -1f, 1f);

            fromSaturationOffsetSlider.RegisterValueChangedCallback(evt =>
                fromSaturationOffsetLabel.SetText($"{(evt.newValue * 100).Round(0)}%"));

            toSaturationOffsetSlider.RegisterValueChangedCallback(evt =>
                toSaturationOffsetLabel.SetText($"{(evt.newValue * 100).Round(0)}%"));

            FluidField fromSaturationOffsetFluidField =
                GetOffsetField
                (
                    "Saturation Offset", fromSaturationOffsetLabel, fromSaturationOffsetSlider,
                    () =>
                    {
                        propertyFromSaturationOffset.floatValue = 0f;
                        propertyFromSaturationOffset.serializedObject.ApplyModifiedProperties();
                    });

            FluidField toSaturationOffsetFluidField =
                GetOffsetField
                (
                    "Saturation Offset", toSaturationOffsetLabel, toSaturationOffsetSlider,
                    () =>
                    {
                        propertyToSaturationOffset.floatValue = 0f;
                        propertyToSaturationOffset.serializedObject.ApplyModifiedProperties();
                    });

            #endregion

            #region Lightness

            Label fromLightnessOffsetLabel = GetOffsetLabel(() => $"{(propertyFromLightnessOffset.floatValue * 100).Round(0)}%");
            Label toLightnessOffsetLabel = GetOffsetLabel(() => $"{(propertyToLightnessOffset.floatValue * 100).Round(0)}%");

            Slider fromLightnessOffsetSlider = DesignUtils.NewSlider(propertyFromLightnessOffset, -1f, 1f);
            Slider toLightnessOffsetSlider = DesignUtils.NewSlider(propertyToLightnessOffset, -1f, 1f);

            fromLightnessOffsetSlider.RegisterValueChangedCallback(evt =>
                fromLightnessOffsetLabel.SetText($"{(evt.newValue * 100).Round(0)}%"));

            toLightnessOffsetSlider.RegisterValueChangedCallback(evt =>
                toLightnessOffsetLabel.SetText($"{(evt.newValue * 100).Round(0)}%"));

            FluidField fromLightnessOffsetFluidField =
                GetOffsetField
                (
                    "Lightness Offset", fromLightnessOffsetLabel, fromLightnessOffsetSlider,
                    () =>
                    {
                        propertyFromLightnessOffset.floatValue = 0f;
                        propertyFromLightnessOffset.serializedObject.ApplyModifiedProperties();
                    });

            FluidField toLightnessOffsetFluidField =
                GetOffsetField
                (
                    "Lightness Offset", toLightnessOffsetLabel, toLightnessOffsetSlider,
                    () =>
                    {
                        propertyToLightnessOffset.floatValue = 0f;
                        propertyToLightnessOffset.serializedObject.ApplyModifiedProperties();
                    });

            #endregion

            #region Alpha

            Label fromAlphaOffsetLabel = GetOffsetLabel(() => $"{(propertyFromAlphaOffset.floatValue * 100).Round(0)}%");
            Label toAlphaOffsetLabel = GetOffsetLabel(() => $"{(propertyToAlphaOffset.floatValue * 100).Round(0)}%");

            Slider fromAlphaOffsetSlider = DesignUtils.NewSlider(propertyFromAlphaOffset, -1f, 1f);
            Slider toAlphaOffsetSlider = DesignUtils.NewSlider(propertyToAlphaOffset, -1f, 1f);

            fromAlphaOffsetSlider.RegisterValueChangedCallback(evt =>
                fromAlphaOffsetLabel.SetText($"{(evt.newValue * 100).Round(0)}%"));

            toAlphaOffsetSlider.RegisterValueChangedCallback(evt =>
                toAlphaOffsetLabel.SetText($"{(evt.newValue * 100).Round(0)}%"));

            FluidField fromAlphaOffsetFluidField =
                GetOffsetField
                (
                    "Alpha Offset", fromAlphaOffsetLabel, fromAlphaOffsetSlider,
                    () =>
                    {
                        propertyFromAlphaOffset.floatValue = 0f;
                        propertyFromAlphaOffset.serializedObject.ApplyModifiedProperties();
                    });

            FluidField toAlphaOffsetFluidField =
                GetOffsetField
                (
                    "Alpha Offset", toAlphaOffsetLabel, toAlphaOffsetSlider,
                    () =>
                    {
                        propertyToAlphaOffset.floatValue = 0f;
                        propertyToAlphaOffset.serializedObject.ApplyModifiedProperties();
                    });

            #endregion

            FluidField fromCustomValueFluidField = FluidField.Get<ColorField>(propertyFromCustomValue, "From Custom Color").SetStyleHeight(HEIGHT, HEIGHT, HEIGHT);
            FluidField toCustomValueFluidField = FluidField.Get<ColorField>(propertyToCustomValue, "To Custom Color").SetStyleHeight(HEIGHT, HEIGHT, HEIGHT);

            PropertyField settingsPropertyField = DesignUtils.NewPropertyField(propertySettings).SetName("Animation Settings");

            VisualElement fromOffsetContainer =
                DesignUtils.column
                    // .SetStyleFlexGrow(1)
                    .SetName("From Offset")
                    .AddChild(fromHueOffsetFluidField)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(fromSaturationOffsetFluidField)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(fromLightnessOffsetFluidField)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(fromAlphaOffsetFluidField);


            VisualElement toOffsetContainer =
                DesignUtils.column
                    // .SetStyleFlexGrow(1)
                    .SetName("To Offset")
                    .AddChild(toHueOffsetFluidField)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(toSaturationOffsetFluidField)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(toLightnessOffsetFluidField)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(toAlphaOffsetFluidField);

            VisualElement foldoutContent =
                new VisualElement()
                    .AddChild
                    (
                        DesignUtils.row
                            .AddChild(enableSwitch)
                            .AddChild(DesignUtils.flexibleSpace)
                    )
                    .AddChild
                    (
                        content
                            .AddChild
                            (
                                DesignUtils.row
                                    .SetName("From To Settings")
                                    .AddChild
                                    (
                                        DesignUtils.column
                                            .SetName("From Settings")
                                            .AddChild(fromReferenceValueFluidField)
                                            .AddChild(DesignUtils.spaceBlock)
                                            .AddChild(fromOffsetContainer)
                                            .AddChild(fromCustomValueFluidField)
                                    )
                                    .AddChild(DesignUtils.spaceBlock)
                                    .AddChild
                                    (
                                        DesignUtils.column
                                            .SetName("To Settings")
                                            .AddChild(toReferenceValueFluidField)
                                            .AddChild(DesignUtils.spaceBlock)
                                            .AddChild(toOffsetContainer)
                                            .AddChild(toCustomValueFluidField)
                                    )
                            )
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(settingsPropertyField)
                            .AddChild(DesignUtils.endOfLineBlock)
                    );

            void Update()
            {
                fromOffsetContainer.SetStyleDisplay((ReferenceValue)propertyFromReferenceValue.enumValueIndex == ReferenceValue.CustomValue ? DisplayStyle.None : DisplayStyle.Flex);
                fromCustomValueFluidField.SetStyleDisplay((ReferenceValue)propertyFromReferenceValue.enumValueIndex != ReferenceValue.CustomValue ? DisplayStyle.None : DisplayStyle.Flex);
                toOffsetContainer.SetStyleDisplay((ReferenceValue)propertyToReferenceValue.enumValueIndex == ReferenceValue.CustomValue ? DisplayStyle.None : DisplayStyle.Flex);
                toCustomValueFluidField.SetStyleDisplay((ReferenceValue)propertyToReferenceValue.enumValueIndex != ReferenceValue.CustomValue ? DisplayStyle.None : DisplayStyle.Flex);
            }

            //FromReferenceValue
            EnumField invisibleFieldRotateFromReferenceValueEnum = new EnumField { bindingPath = propertyFromReferenceValue.propertyPath }.SetStyleDisplay(DisplayStyle.None);
            foldoutContent.AddChild(invisibleFieldRotateFromReferenceValueEnum);
            invisibleFieldRotateFromReferenceValueEnum.RegisterValueChangedCallback(changeEvent => Update());

            //ToReferenceValue
            EnumField invisibleFieldRotateToReferenceValueEnum = new EnumField { bindingPath = propertyToReferenceValue.propertyPath }.SetStyleDisplay(DisplayStyle.None);
            foldoutContent.AddChild(invisibleFieldRotateToReferenceValueEnum);
            invisibleFieldRotateToReferenceValueEnum.RegisterValueChangedCallback(changeEvent => Update());

            foldoutContent.Bind(propertyAnimation.serializedObject);

            Update();
            return foldoutContent;
        }

        private static Label GetOffsetLabel(Func<string> value) =>
            DesignUtils.fieldLabel
                .ResetLayout()
                .SetText(value.Invoke())
                .SetStyleAlignSelf(Align.Center)
                .SetStyleTextAlign(TextAnchor.MiddleRight)
                .SetStyleWidth(24);

        private static FluidField GetOffsetField(string labelText, VisualElement label, VisualElement slider, UnityAction onClickCallback) =>
            FluidField.Get()
                .SetStyleHeight(HEIGHT, HEIGHT, HEIGHT)
                .SetLabelText(labelText)
                .AddFieldContent
                (
                    DesignUtils.row
                        .SetStyleJustifyContent(Justify.Center)
                        .AddChild(label)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(slider)
                        .AddChild
                        (
                            FluidButton.Get(EditorSpriteSheets.EditorUI.Icons.Reset)
                                .SetElementSize(ElementSize.Tiny)
                                .SetTooltip("Reset")
                                .SetOnClick(onClickCallback)
                        )
                );
    }
}
