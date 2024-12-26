// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Common.Extensions;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.Reactor.Drawers
{
    [CustomPropertyDrawer(typeof(Vector3Animation), true)]
    public class Vector3AnimationDrawer : PropertyDrawer
    {
        private static Color accentColor => EditorColors.Reactor.Red;
        private static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Reactor.Red;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var drawer = new VisualElement();
            if (property == null) return drawer;
            drawer.RegisterCallback<DetachFromPanelEvent>(evt => drawer.RecycleAndClear());
            var target = property.GetTargetObjectOfProperty() as FloatAnimation;

            #region SerializedProperties

            //Animation
            SerializedProperty propertyAnimation = property.FindPropertyRelative("Animation");
            SerializedProperty propertyAnimationEnabled = propertyAnimation.FindPropertyRelative("Enabled");
            //Callbacks            
            SerializedProperty propertyOnPlayCallback = property.FindPropertyRelative("OnPlayCallback");
            SerializedProperty propertyOnStopCallback = property.FindPropertyRelative("OnStopCallback");
            SerializedProperty propertyOnFinishCallback = property.FindPropertyRelative("OnFinishCallback");

            #endregion

            #region ComponentHeader

            FluidComponentHeader componentHeader =
                FluidComponentHeader.Get()
                    .SetAccentColor(accentColor)
                    .SetElementSize(ElementSize.Tiny)
                    .SetComponentNameText("Reflected Vector3")
                    .SetComponentTypeText("Animation")
                    .AddManualButton()
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

        private static VisualElement GetAnimationContent(SerializedProperty propertyAnimation, SerializedProperty propertyAnimationEnabled)
        {
            SerializedProperty propertyFromReferenceValue = propertyAnimation.FindPropertyRelative("FromReferenceValue");
            SerializedProperty propertyToReferenceValue = propertyAnimation.FindPropertyRelative("ToReferenceValue");
            SerializedProperty propertyFromOffset = propertyAnimation.FindPropertyRelative("FromOffset");
            SerializedProperty propertyToOffset = propertyAnimation.FindPropertyRelative("ToOffset");
            SerializedProperty propertyFromCustomValue = propertyAnimation.FindPropertyRelative("FromCustomValue");
            SerializedProperty propertyToCustomValue = propertyAnimation.FindPropertyRelative("ToCustomValue");
            SerializedProperty propertySettings = propertyAnimation.FindPropertyRelative("Settings");

            var content = new VisualElement();
            content.SetEnabled(propertyAnimationEnabled.boolValue);
            FluidToggleSwitch enableSwitch = DesignUtils.GetEnableDisableSwitch(propertyAnimationEnabled, content, selectableAccentColor, "Animation");

            const int height = 42;

            FluidField fromReferenceValueFluidField = FluidField.Get<EnumField>(propertyFromReferenceValue, "From Value").SetStyleHeight(height, height, height);
            FluidField toReferenceValueFluidField = FluidField.Get<EnumField>(propertyToReferenceValue, "To Value").SetStyleHeight(height, height, height);

            FluidField fromOffsetFluidField = FluidField.Get<Vector3Field>(propertyFromOffset, "From Offset").SetStyleHeight(height, height, height);
            FluidField toOffsetFluidField = FluidField.Get<Vector3Field>(propertyToOffset, "To Offset").SetStyleHeight(height, height, height);

            FluidField fromCustomValueFluidField = FluidField.Get<Vector3Field>(propertyFromCustomValue, "From Custom Value").SetStyleHeight(height, height, height);
            FluidField toCustomValueFluidField = FluidField.Get<Vector3Field>(propertyToCustomValue, "To Custom Value").SetStyleHeight(height, height, height);

            PropertyField settingsPropertyField = DesignUtils.NewPropertyField(propertySettings).SetName("Animation Settings");

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
                        content.AddChild
                            (
                                DesignUtils.row
                                    .SetName("From To Settings")
                                    .AddChild
                                    (
                                        DesignUtils.column
                                            .SetName("From Settings")
                                            .AddChild(fromReferenceValueFluidField)
                                            .AddChild(DesignUtils.spaceBlock)
                                            .AddChild(fromOffsetFluidField)
                                            .AddChild(fromCustomValueFluidField)
                                    )
                                    .AddChild(DesignUtils.spaceBlock)
                                    .AddChild
                                    (
                                        DesignUtils.column
                                            .SetName("To Settings")
                                            .AddChild(toReferenceValueFluidField)
                                            .AddChild(DesignUtils.spaceBlock)
                                            .AddChild(toOffsetFluidField)
                                            .AddChild(toCustomValueFluidField)
                                    )
                            )
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(settingsPropertyField)
                            .AddChild(DesignUtils.endOfLineBlock)
                    );

            void Update()
            {
                fromOffsetFluidField.SetStyleDisplay((ReferenceValue)propertyFromReferenceValue.enumValueIndex == ReferenceValue.CustomValue ? DisplayStyle.None : DisplayStyle.Flex);
                fromCustomValueFluidField.SetStyleDisplay((ReferenceValue)propertyFromReferenceValue.enumValueIndex != ReferenceValue.CustomValue ? DisplayStyle.None : DisplayStyle.Flex);
                toOffsetFluidField.SetStyleDisplay((ReferenceValue)propertyToReferenceValue.enumValueIndex == ReferenceValue.CustomValue ? DisplayStyle.None : DisplayStyle.Flex);
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
    }
}
