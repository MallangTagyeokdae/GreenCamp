// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Internal;
using Doozy.Editor.UIElements;
using Doozy.Editor.UIManager.Windows;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using Doozy.Runtime.UIManager.ScriptableObjects;
using Doozy.Runtime.UIManager.Triggers;
using TMPro;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace Doozy.Editor.UIManager.Editors.Triggers
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UITooltipTrigger), true)]
    public class UITooltipTriggerEditor : EditorUIEditor<UITooltipTrigger>
    {
        protected override Color accentColor => EditorColors.UIManager.ListenerComponent;
        protected override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.ListenerComponent;

        protected FluidTab settingsTab { get; set; }
        protected FluidTab overridesTab { get; set; }
        protected FluidTab callbacksTab { get; set; }

        protected FluidAnimatedContainer settingsAnimatedContainer { get; set; }
        protected FluidAnimatedContainer overridesAnimatedContainer { get; set; }
        protected FluidAnimatedContainer callbacksAnimatedContainer { get; set; }

        protected GameObject prefab { get; private set; }

        protected List<string> tooltipNames { get; set; }

        protected SerializedProperty propertyUITooltipName { get; set; }
        protected SerializedProperty propertyShowOnPointerEnter { get; set; }
        protected SerializedProperty propertyHideOnPointerExit { get; set; }
        protected SerializedProperty propertyShowOnPointerClick { get; set; }
        protected SerializedProperty propertyHideOnPointerClick { get; set; }
        protected SerializedProperty propertyOverrideParentMode { get; set; }
        protected SerializedProperty propertyParentMode { get; set; }
        protected SerializedProperty propertyOverrideTrackingMode { get; set; }
        protected SerializedProperty propertyTrackingMode { get; set; }
        protected SerializedProperty propertyOverridePositionMode { get; set; }
        protected SerializedProperty propertyPositionMode { get; set; }
        protected SerializedProperty propertyParentTag { get; set; }
        protected SerializedProperty propertyFollowTag { get; set; }
        protected SerializedProperty propertyOverridePositionOffset { get; set; }
        protected SerializedProperty propertyPositionOffset { get; set; }
        protected SerializedProperty propertyOverrideMaximumWidth { get; set; }
        protected SerializedProperty propertyMaximumWidth { get; set; }
        protected SerializedProperty propertyShowDelay { get; set; }
        protected SerializedProperty propertyTexts { get; set; }
        protected SerializedProperty propertySprites { get; set; }
        protected SerializedProperty propertyEvents { get; set; }
        protected SerializedProperty propertyOnShowCallback { get; set; }
        protected SerializedProperty propertyOnHideCallback { get; set; }
        
        private bool initialized { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            settingsTab?.Dispose();
            overridesTab?.Dispose();
            callbacksTab?.Dispose();

            settingsAnimatedContainer?.Dispose();
            overridesAnimatedContainer?.Dispose();
            callbacksAnimatedContainer?.Dispose();
        }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyUITooltipName = serializedObject.FindProperty(nameof(UITooltipTrigger.UITooltipName));
            propertyShowOnPointerEnter = serializedObject.FindProperty(nameof(UITooltipTrigger.ShowOnPointerEnter));
            propertyHideOnPointerExit = serializedObject.FindProperty(nameof(UITooltipTrigger.HideOnPointerExit));
            propertyShowOnPointerClick = serializedObject.FindProperty(nameof(UITooltipTrigger.ShowOnPointerClick));
            propertyHideOnPointerClick = serializedObject.FindProperty(nameof(UITooltipTrigger.HideOnPointerClick));
            propertyOverrideParentMode = serializedObject.FindProperty(nameof(UITooltipTrigger.OverrideParentMode));
            propertyParentMode = serializedObject.FindProperty(nameof(UITooltipTrigger.ParentMode));
            propertyOverrideTrackingMode = serializedObject.FindProperty(nameof(UITooltipTrigger.OverrideTrackingMode));
            propertyTrackingMode = serializedObject.FindProperty(nameof(UITooltipTrigger.TrackingMode));
            propertyOverridePositionMode = serializedObject.FindProperty(nameof(UITooltipTrigger.OverridePositionMode));
            propertyPositionMode = serializedObject.FindProperty(nameof(UITooltipTrigger.PositionMode));
            propertyParentTag = serializedObject.FindProperty(nameof(UITooltipTrigger.ParentTag));
            propertyFollowTag = serializedObject.FindProperty(nameof(UITooltipTrigger.FollowTag));
            propertyOverridePositionOffset = serializedObject.FindProperty(nameof(UITooltipTrigger.OverridePositionOffset));
            propertyPositionOffset = serializedObject.FindProperty(nameof(UITooltipTrigger.PositionOffset));
            propertyOverrideMaximumWidth = serializedObject.FindProperty(nameof(UITooltipTrigger.OverrideMaximumWidth));
            propertyMaximumWidth = serializedObject.FindProperty(nameof(UITooltipTrigger.MaximumWidth));
            propertyShowDelay = serializedObject.FindProperty("ShowDelay");
            propertyTexts = serializedObject.FindProperty(nameof(UITooltipTrigger.Texts));
            propertySprites = serializedObject.FindProperty(nameof(UITooltipTrigger.Sprites));
            propertyEvents = serializedObject.FindProperty(nameof(UITooltipTrigger.Events));
            propertyOnShowCallback = serializedObject.FindProperty(nameof(UITooltipTrigger.OnShowCallback));
            propertyOnHideCallback = serializedObject.FindProperty(nameof(UITooltipTrigger.OnHideCallback));
        }

        protected override void InitializeEditor()
        {
            FindProperties();
            base.InitializeEditor();

            componentHeader
                .SetIcon(EditorSpriteSheets.UIManager.Icons.UITooltip)
                .SetElementSize(ElementSize.Small)
                .SetComponentNameText("UITooltip Trigger");

            InitializeSettings();
            InitializeOverrides();
            InitializeCallbacks();

            initialized = true;

            root.schedule.Execute(() => settingsTab.ButtonSetIsOn(true, false));

            //refresh overridesTab enabled indicator
            root.schedule.Execute(() =>
            {
                void UpdateIndicator(FluidTab tab, bool toggleOn, bool animateChange)
                {
                    if (tab.indicator.isOn != toggleOn)
                        tab.indicator.Toggle(toggleOn, animateChange);
                }

                //overridesTab - initial indicators state update (no animation)
                UpdateIndicator
                (
                    overridesTab,
                    propertyOverrideParentMode.boolValue |
                    propertyOverrideTrackingMode.boolValue |
                    propertyOverridePositionMode.boolValue |
                    propertyOverridePositionOffset.boolValue,
                    false
                );
                
                //callbacksTab - initial indicators state update (no animation)
                UpdateIndicator
                (
                    callbacksTab,
                    castedTarget.OnShowCallback.hasCallbacks |
                    castedTarget.OnHideCallback.hasCallbacks,
                    false
                );

                root.schedule.Execute(() =>
                {
                    //overridesTab - subsequent indicators state update (animated)
                    UpdateIndicator
                    (
                        overridesTab,
                        propertyOverrideParentMode.boolValue |
                        propertyOverrideTrackingMode.boolValue |
                        propertyOverridePositionMode.boolValue |
                        propertyOverridePositionOffset.boolValue,
                        true
                    );
                    
                    //callbacksTab - subsequent indicators state update (animated)
                    UpdateIndicator
                    (
                        callbacksTab,
                        castedTarget.OnShowCallback.hasCallbacks |
                        castedTarget.OnHideCallback.hasCallbacks,
                        true
                    );

                }).Every(200);
            });
        }

        private void InitializeSettings()
        {
            settingsAnimatedContainer = new FluidAnimatedContainer("Settings", true).Hide(false);
            settingsTab =
                new FluidTab()
                    .SetLabelText("Settings")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Settings)
                    .SetElementSize(ElementSize.Small)
                    .IndicatorSetEnabledColor(accentColor)
                    .ButtonSetAccentColor(selectableAccentColor)
                    .ButtonSetOnValueChanged(evt => settingsAnimatedContainer.Toggle(evt.newValue, evt.animateChange))
                    .AddToToggleGroup(tabsGroup);

            settingsAnimatedContainer.SetOnShowCallback(() =>
            {
                #region Containers - Texts Sprites Events

                VisualElement GetContainer(string containerName) =>
                    new VisualElement()
                        .SetName(containerName)
                        .SetStylePaddingTop(DesignUtils.k_Spacing);

                VisualElement textsContainer = GetContainer("Texts");
                VisualElement spritesContainer = GetContainer("Sprites");
                VisualElement eventsContainer = GetContainer("Events");

                #endregion

                #region Show Triggers

                FluidToggleSwitch showOnPointerEnterSwitch =
                    FluidToggleSwitch.Get("Pointer Enter")
                        .SetTooltip("Show on pointer enter")
                        .BindToProperty(propertyShowOnPointerEnter)
                        .SetToggleAccentColor(selectableAccentColor)
                        .SetTooltip("Show the tooltip when the pointer enters the trigger");

                FluidToggleSwitch showOnPointerClickSwitch =
                    FluidToggleSwitch.Get("Pointer Click")
                        .SetTooltip("Show on pointer click")
                        .BindToProperty(propertyShowOnPointerClick)
                        .SetToggleAccentColor(selectableAccentColor)
                        .SetTooltip("Show the tooltip when the pointer clicks the trigger");


                FluidField showTriggersFluidField =
                    FluidField.Get("Show on")
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddChild(showOnPointerEnterSwitch)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(showOnPointerClickSwitch)
                                .AddChild(DesignUtils.flexibleSpace)
                        );

                #endregion

                #region Hide Triggers

                FluidToggleSwitch hideOnPointerExitSwitch =
                    FluidToggleSwitch.Get("Pointer Exit")
                        .SetTooltip("Hide on pointer exit")
                        .BindToProperty(propertyHideOnPointerExit)
                        .SetToggleAccentColor(selectableAccentColor)
                        .SetTooltip("Hide the tooltip when the pointer exits the trigger (if the tooltip is shown)");

                FluidToggleSwitch hideOnPointerClickSwitch =
                    FluidToggleSwitch.Get("Pointer Click")
                        .SetTooltip("Hide on pointer click")
                        .BindToProperty(propertyHideOnPointerClick)
                        .SetToggleAccentColor(selectableAccentColor)
                        .SetTooltip("Hide the tooltip when the pointer clicks the trigger (if the tooltip is shown)");

                FluidField hideTriggersFluidField =
                    FluidField.Get("Hide on")
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddChild(hideOnPointerExitSwitch)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(hideOnPointerClickSwitch)
                                .AddChild(DesignUtils.flexibleSpace)
                        );

                #endregion

                #region Show Delay

                FloatField showDelayField =
                    DesignUtils.NewFloatField(propertyShowDelay)
                        .SetStyleWidth(40)
                        .SetTooltip("How long (in seconds), after the pointer enters the trigger, should the tooltip be shown");

                FluidField showDelayFluidField =
                    FluidField.Get("Show delay")
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddChild(showDelayField)
                                .AddChild(DesignUtils.fieldLabel.SetText("seconds").SetStyleAlignSelf(Align.Center).SetStyleMargins(DesignUtils.k_Spacing))
                        );

                #endregion

                #region UITooltip Name Popup

                FluidButton openDatabaseButton =
                    FluidButton.Get()
                        .SetButtonStyle(ButtonStyle.Clear)
                        .SetElementSize(ElementSize.Normal)
                        .SetStyleAlignSelf(Align.Center)
                        .SetStyleMarginRight(DesignUtils.k_Spacing * 2)
                        .SetButtonStyle(ButtonStyle.Contained)
                        .SetIcon(EditorSpriteSheets.UIManager.Icons.UITooltipDatabase)
                        .SetAccentColor(selectableAccentColor)
                        .SetOnClick(TooltipsDatabaseWindow.Open)
                        .SetTooltip("Open Tooltips Database Window");

                RefreshTooltipNames();

                PopupField<string> namePopupField =
                    new PopupField<string>(tooltipNames, 0)
                        .ResetLayout()
                        .BindToProperty(propertyUITooltipName);

                namePopupField.RegisterValueChangedCallback(evt =>
                {
                    if (evt == null) return;
                    if (evt.newValue == evt.previousValue) return;
                    if (string.IsNullOrEmpty(evt.newValue)) return;
                    UpdateEditor(namePopupField, textsContainer, spritesContainer, eventsContainer);
                });

                FluidField uiTooltipNameFluidField =
                    FluidField.Get("UITooltip")
                        .AddFieldContent(namePopupField);

                Refresh(namePopupField, textsContainer, spritesContainer, eventsContainer);

                #endregion

                settingsAnimatedContainer
                    .AddContent
                    (
                        DesignUtils.row
                            .AddChild(showTriggersFluidField)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(hideTriggersFluidField)
                    )
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(showDelayFluidField)
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent
                    (
                        DesignUtils.row
                            .AddChild(openDatabaseButton)
                            .AddChild(uiTooltipNameFluidField)
                    )
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(textsContainer)
                    .AddContent(spritesContainer)
                    .AddContent(eventsContainer)
                    .Bind(serializedObject);
            });
        }

        private void InitializeOverrides()
        {
            overridesAnimatedContainer = new FluidAnimatedContainer("Overrides", true).Hide(false);
            overridesTab =
                new FluidTab()
                    .SetLabelText("Overrides")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Plus)
                    .SetElementSize(ElementSize.Small)
                    .IndicatorSetEnabledColor(accentColor)
                    .ButtonSetAccentColor(selectableAccentColor)
                    .ButtonSetOnValueChanged(evt => overridesAnimatedContainer.Toggle(evt.newValue, evt.animateChange))
                    .AddToToggleGroup(tabsGroup);

            overridesAnimatedContainer.SetOnShowCallback(() =>
            {
                #region Parent Mode

                FluidToggleSwitch overrideParentModeSwitch =
                    FluidToggleSwitch.Get()
                        .SetTooltip("Override tooltip's ParentMode to use the value from this trigger instead of the one set in the tooltip")
                        .BindToProperty(propertyOverrideParentMode)
                        .SetToggleAccentColor(selectableAccentColor);

                EnumField parentModeEnumField =
                    DesignUtils.NewEnumField(propertyParentMode)
                        .SetStyleFlexGrow(1)
                        .SetTooltip("Set where a tooltip should be instantiated under");

                parentModeEnumField.SetEnabled(propertyOverrideParentMode.boolValue);
                overrideParentModeSwitch.SetOnValueChanged(evt =>
                {
                    parentModeEnumField.SetEnabled(evt.newValue);
                });

                FluidField parentModeFluidField =
                    FluidField.Get("Override Parent Mode")
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddChild(overrideParentModeSwitch)
                                .AddChild(parentModeEnumField)
                        );

                #endregion

                #region Tracking Mode

                FluidToggleSwitch overrideTrackingModeSwitch =
                    FluidToggleSwitch.Get()
                        .SetTooltip("Override tooltip's TrackingMode to use the value from this trigger instead of the one set in the tooltip")
                        .BindToProperty(propertyOverrideTrackingMode)
                        .SetToggleAccentColor(selectableAccentColor);

                EnumField trackingModeEnumField =
                    DesignUtils.NewEnumField(propertyTrackingMode)
                        .SetStyleFlexGrow(1)
                        .SetTooltip("Set how the tooltip behaves when it is shown");

                trackingModeEnumField.SetEnabled(propertyOverrideTrackingMode.boolValue);
                overrideTrackingModeSwitch.SetOnValueChanged(evt =>
                {
                    trackingModeEnumField.SetEnabled(evt.newValue);
                });

                FluidField trackingModeFluidField =
                    FluidField.Get("Override Tracking Mode")
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddChild(overrideTrackingModeSwitch)
                                .AddChild(trackingModeEnumField)
                        );

                #endregion

                #region Position Mode

                FluidToggleSwitch overridePositionModeSwitch =
                    FluidToggleSwitch.Get()
                        .SetTooltip("Override tooltip's PositionMode to use the value from this trigger instead of the one set in the tooltip")
                        .BindToProperty(propertyOverridePositionMode)
                        .SetToggleAccentColor(selectableAccentColor);

                EnumField positionModeEnumField =
                    DesignUtils.NewEnumField(propertyPositionMode)
                        .SetStyleFlexGrow(1)
                        .SetTooltip("Set where the tooltip should be positioned relative to the tracked target");

                positionModeEnumField.SetEnabled(propertyOverridePositionMode.boolValue);
                overridePositionModeSwitch.SetOnValueChanged(evt =>
                {
                    positionModeEnumField.SetEnabled(evt.newValue);
                });

                FluidField positionModeFluidField =
                    FluidField.Get("Override Position Mode")
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddChild(overridePositionModeSwitch)
                                .AddChild(positionModeEnumField)
                        );

                #endregion

                #region Parent Tag

                FluidField parentTagFluidField =
                    FluidField.Get("Parent Tag")
                        .SetTooltip("Id used to identify the designated parent where the tooltip should be parented under, after it has been instantiated")
                        .AddFieldContent(DesignUtils.NewPropertyField(propertyParentTag))
                        .SetStylePaddingTop(DesignUtils.k_Spacing)
                        .SetStylePaddingBottom(DesignUtils.k_Spacing);

                parentTagFluidField.SetStyleDisplay
                (
                    (UITooltip.Parenting)propertyParentMode.enumValueIndex == UITooltip.Parenting.UITag &
                    propertyOverrideParentMode.boolValue
                        ? DisplayStyle.Flex
                        : DisplayStyle.None
                );

                overrideParentModeSwitch.AddOnValueChanged(evt =>
                {
                    parentTagFluidField.SetStyleDisplay
                    (
                        (UITooltip.Parenting)propertyParentMode.enumValueIndex == UITooltip.Parenting.UITag &
                        evt.newValue
                            ? DisplayStyle.Flex
                            : DisplayStyle.None
                    );
                });

                parentModeEnumField.RegisterValueChangedCallback(evt =>
                {
                    if (evt?.newValue == null) return;
                    parentTagFluidField.SetStyleDisplay
                    (
                        (UITooltip.Parenting)evt.newValue == UITooltip.Parenting.UITag &
                        propertyOverrideParentMode.boolValue
                            ? DisplayStyle.Flex
                            : DisplayStyle.None
                    );
                });

                #endregion

                #region Follow Tag

                FluidField followTagFluidField =
                    FluidField.Get("Follow Tag")
                        .SetTooltip("Id used to identify the follow target when the tracking mode is set to FollowTarget")
                        .AddFieldContent(DesignUtils.NewPropertyField(propertyFollowTag))
                        .SetStylePaddingTop(DesignUtils.k_Spacing)
                        .SetStylePaddingBottom(DesignUtils.k_Spacing);


                followTagFluidField.SetStyleDisplay
                (
                    (UITooltip.Tracking)propertyTrackingMode.enumValueIndex == UITooltip.Tracking.FollowTarget &
                    propertyOverrideTrackingMode.boolValue
                        ? DisplayStyle.Flex
                        : DisplayStyle.None
                );

                overrideTrackingModeSwitch.AddOnValueChanged(evt =>
                {
                    followTagFluidField.SetStyleDisplay
                    (
                        (UITooltip.Tracking)propertyTrackingMode.enumValueIndex == UITooltip.Tracking.FollowTarget &
                        evt.newValue
                            ? DisplayStyle.Flex
                            : DisplayStyle.None
                    );
                });

                trackingModeEnumField.RegisterValueChangedCallback(evt =>
                {
                    if (evt?.newValue == null) return;
                    followTagFluidField.SetStyleDisplay
                    (
                        (UITooltip.Tracking)evt.newValue == UITooltip.Tracking.FollowTarget &
                        propertyOverrideTrackingMode.boolValue
                            ? DisplayStyle.Flex
                            : DisplayStyle.None
                    );
                });

                #endregion

                #region Position Offset

                FluidToggleSwitch overridePositionOffsetSwitch =
                    FluidToggleSwitch.Get()
                        .SetTooltip("Override tooltip's PositionOffset to use the value from this trigger instead of the one set in the tooltip")
                        .BindToProperty(propertyOverridePositionOffset)
                        .SetToggleAccentColor(selectableAccentColor);

                Vector3Field positionOffsetVector3Field =
                    DesignUtils.NewVector3Field(propertyPositionOffset)
                        .SetTooltip("Set the offset applied to the tooltip position, after all the positioning has been applied");

                positionOffsetVector3Field.Q(null, Vector3Field.inputUssClassName).SetStyleFlexShrink(1);

                positionOffsetVector3Field.SetEnabled(propertyOverridePositionOffset.boolValue);
                overridePositionOffsetSwitch.SetOnValueChanged(evt =>
                {
                    positionOffsetVector3Field.SetEnabled(evt.newValue);
                });

                FluidField positionOffsetFluidField =
                    FluidField.Get("Override Position Offset")
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddChild(overridePositionOffsetSwitch)
                                .AddChild(positionOffsetVector3Field)
                        );

                #endregion

                #region Maximum Width

                FluidToggleSwitch overrideMaxWidthSwitch =
                    FluidToggleSwitch.Get()
                        .SetTooltip("Override tooltip's MaximumWidth to use the value from this trigger instead of the one set in the tooltip")
                        .BindToProperty(propertyOverrideMaximumWidth)
                        .SetToggleAccentColor(selectableAccentColor);
                
                FloatField maxWidthFloatField =
                    DesignUtils.NewFloatField(propertyMaximumWidth)
                        .SetStyleFlexGrow(1)
                        .SetTooltip("Set the maximum width of the tooltip");
                
                maxWidthFloatField.SetEnabled(propertyOverrideMaximumWidth.boolValue);
                overrideMaxWidthSwitch.SetOnValueChanged(evt =>
                {
                    maxWidthFloatField.SetEnabled(evt.newValue);
                });
                
                FluidField maxWidthFluidField =
                    FluidField.Get("Override Maximum Width")
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddChild(overrideMaxWidthSwitch)
                                .AddChild(maxWidthFloatField)
                        );

                #endregion
                
                overridesAnimatedContainer
                    .AddContent
                    (
                        DesignUtils.row
                            .AddChild(parentModeFluidField)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(trackingModeFluidField)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(positionModeFluidField)
                    )
                    .AddContent(parentTagFluidField)
                    .AddContent(followTagFluidField)
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(positionOffsetFluidField)
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(maxWidthFluidField)
                    .Bind(serializedObject);
            });
        }

        private void InitializeCallbacks()
        {
            callbacksAnimatedContainer = new FluidAnimatedContainer("Callbacks", true).Hide();
            callbacksTab = 
                new FluidTab()
                    .SetLabelText("Callbacks")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.UnityEvent)
                    .SetElementSize(ElementSize.Small)
                    .IndicatorSetEnabledColor(DesignUtils.callbacksColor)
                    .ButtonSetAccentColor(DesignUtils.callbackSelectableColor)
                    .ButtonSetOnValueChanged(evt => callbacksAnimatedContainer.Toggle(evt.newValue, evt.animateChange))
                    .AddToToggleGroup(tabsGroup);
            
            callbacksAnimatedContainer.SetOnShowCallback(() =>
            {
                FluidField GetCallbackField(string labelText, SerializedProperty property) =>
                    FluidField.Get()
                        .SetLabelText(labelText)
                        .SetElementSize(ElementSize.Large)
                        .AddFieldContent(DesignUtils.NewPropertyField(property));

                callbacksAnimatedContainer
                    .AddContent(GetCallbackField("On Show Tooltip", propertyOnShowCallback))
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(GetCallbackField("On Hide Tooltip", propertyOnHideCallback))
                    .Bind(serializedObject);
            });
        }
        
        protected override VisualElement Toolbar()
        {
            return
                toolbarContainer
                    .AddChild(settingsTab)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(overridesTab)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(callbacksTab)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(DesignUtils.flexibleSpace)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild
                    (
                        DesignUtils.SystemButton_SortComponents
                        (
                            castedTarget.gameObject,
                            nameof(RectTransform),
                            nameof(UITooltipTrigger)
                        )
                    );
        }

        protected override VisualElement Content()
        {
            return contentContainer
                .AddChild(settingsAnimatedContainer)
                .AddChild(overridesAnimatedContainer)
                .AddChild(callbacksAnimatedContainer);
        }

        protected void Refresh
        (
            PopupField<string> namePopupField,
            VisualElement textsContainer,
            VisualElement spritesContainer,
            VisualElement eventsContainer
        )
        {
            RefreshTooltipNames();

            if (!tooltipNames.Contains(propertyUITooltipName.stringValue))
            {
                propertyUITooltipName.stringValue = UITooltip.k_DefaultTooltipName;
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
                serializedObject.Update();
            }

            UpdateEditor(namePopupField, textsContainer, spritesContainer, eventsContainer);
        }

        private void RefreshTooltipNames()
        {
            tooltipNames = tooltipNames ?? new List<string>();
            tooltipNames.Clear();
            tooltipNames.AddRange(UITooltipDatabase.instance.GetAllNames());
        }

        protected virtual void UpdateEditor
        (
            PopupField<string> namePopupField,
            VisualElement textsContainer,
            VisualElement spritesContainer,
            VisualElement eventsContainer
        )
        {
            if (!initialized) return;

            textsContainer?.RecycleAndClear().SetStyleDisplay(DisplayStyle.None);
            spritesContainer?.RecycleAndClear().SetStyleDisplay(DisplayStyle.None);
            eventsContainer?.RecycleAndClear().SetStyleDisplay(DisplayStyle.None);

            string tooltipName = namePopupField.value;

            if (tooltipName.Equals(UITooltip.k_DefaultTooltipName))
                return;

            prefab = UITooltipDatabase.instance.GetPrefab(tooltipName);
            if (prefab == null) return;

            UITooltip tooltip = prefab.GetComponent<UITooltip>();
            if (tooltip == null) return;

            tooltip.Validate();

            #region Texts

            if (tooltip.hasLabels && textsContainer != null)
            {
                textsContainer.SetStyleDisplay(DisplayStyle.Flex);

                if (propertyTexts.arraySize < tooltip.Labels.Count)
                {
                    while (propertyTexts.arraySize < tooltip.Labels.Count)
                    {
                        propertyTexts.InsertArrayElementAtIndex(propertyTexts.arraySize);
                        propertyTexts.GetArrayElementAtIndex(propertyTexts.arraySize - 1).stringValue = string.Empty;
                    }
                    serializedObject.ApplyModifiedPropertiesWithoutUndo();
                    serializedObject.Update();
                }

                for (int i = 0; i < tooltip.Labels.Count; i++)
                {
                    TextMeshProUGUI labelReference = tooltip.Labels[i];
                    SerializedProperty propertyAtIndex = propertyTexts.GetArrayElementAtIndex(i);
                    TextField textField = new TextField().ResetLayout().SetStyleFlexGrow(1).SetMultiline(true).BindToProperty(propertyAtIndex);
                    FluidField fluidField = FluidField.Get(labelReference.name).AddFieldContent(textField);
                    textsContainer.AddChild(fluidField);
                    if (i < tooltip.Labels.Count - 1)
                        textsContainer.AddChild(DesignUtils.spaceBlock);
                }
            }

            #endregion

            #region Sprites

            if (tooltip.hasImages && spritesContainer != null)
            {
                spritesContainer.SetStyleDisplay(DisplayStyle.Flex);

                if (propertySprites.arraySize < tooltip.Images.Count)
                {
                    while (propertySprites.arraySize < tooltip.Images.Count)
                    {
                        propertySprites.InsertArrayElementAtIndex(propertySprites.arraySize);
                        propertySprites.GetArrayElementAtIndex(propertySprites.arraySize - 1).objectReferenceValue = null;
                    }
                    serializedObject.ApplyModifiedPropertiesWithoutUndo();
                    serializedObject.Update();
                }

                for (int i = 0; i < tooltip.Images.Count; i++)
                {
                    Image imageReference = tooltip.Images[i];
                    SerializedProperty propertyAtIndex = propertySprites.GetArrayElementAtIndex(i);
                    ObjectField objectField = new ObjectField().ResetLayout().SetStyleFlexGrow(1).BindToProperty(propertyAtIndex).SetObjectType(typeof(Sprite)).SetAllowSceneObjects(false).SetStyleHeight(20, 20, 20);
                    UnityEngine.UIElements.Image image = new UnityEngine.UIElements.Image().SetStyleSize(32).SetStyleBackgroundScaleMode(ScaleMode.ScaleToFit).SetStyleAlignSelf(Align.Center).SetStyleMargins(DesignUtils.k_Spacing, DesignUtils.k_Spacing / 2f, DesignUtils.k_Spacing / 2f, DesignUtils.k_Spacing / 2f);

                    void UpdateImagePreview(Sprite sprite)
                    {
                        image.SetStyleDisplay(sprite != null ? DisplayStyle.Flex : DisplayStyle.None);
                        if (sprite == null) return;
                        image.SetStyleBackgroundImage(sprite.texture);
                    }

                    UpdateImagePreview((Sprite)objectField.value);

                    FluidField fluidField = FluidField.Get(imageReference.name).AddFieldContent(objectField).AddInfoElement(image);
                    fluidField.infoContainer.SetStyleJustifyContent(Justify.FlexEnd);

                    FloatReaction imageSizeReaction = Reaction.Get<FloatReaction>().SetDuration(0.2f).SetEditorHeartbeat();
                    imageSizeReaction.SetGetter(() => image.resolvedStyle.width).SetSetter(value => image.SetStyleSize(value));
                    imageSizeReaction.SetFrom(32);
                    imageSizeReaction.SetTo(128);

                    fluidField.RegisterCallback<PointerEnterEvent>(evt =>
                    {
                        if (objectField.value == null) return;
                        imageSizeReaction?.Play(PlayDirection.Forward);
                    });

                    fluidField.RegisterCallback<PointerLeaveEvent>(evt =>
                    {
                        if (objectField.value == null) return;
                        imageSizeReaction?.Play(PlayDirection.Reverse);
                    });

                    objectField.RegisterValueChangedCallback(evt =>
                    {
                        if (evt == null) return;
                        UpdateImagePreview((Sprite)evt.newValue);
                    });

                    spritesContainer.AddChild(fluidField);
                    if (i < tooltip.Images.Count - 1)
                        spritesContainer.AddChild(DesignUtils.spaceBlock);
                }
            }

            #endregion

            #region Events

            if (tooltip.hasButtons && eventsContainer != null)
            {
                eventsContainer.SetStyleDisplay(DisplayStyle.Flex);

                if (propertyEvents.arraySize < tooltip.Buttons.Count)
                {
                    while (propertyEvents.arraySize < tooltip.Buttons.Count)
                    {
                        propertyEvents.InsertArrayElementAtIndex(propertyEvents.arraySize);
                    }
                    serializedObject.ApplyModifiedPropertiesWithoutUndo();
                    serializedObject.Update();
                }

                for (int i = 0; i < tooltip.Buttons.Count; i++)
                {
                    UIButton buttonReference = tooltip.Buttons[i];
                    SerializedProperty propertyAtIndex = propertyEvents.GetArrayElementAtIndex(i);
                    // PropertyField unityEventField = DesignUtils.NewPropertyField(propertyAtIndex);
                    VisualElement unityEventField = DesignUtils.UnityEventField("OnClick", propertyAtIndex);
                    unityEventField.Bind(serializedObject);
                    FluidField fluidField = FluidField.Get(buttonReference.name).AddFieldContent(unityEventField);
                    eventsContainer.AddChild(fluidField);
                    if (i < tooltip.Buttons.Count - 1)
                        eventsContainer.AddChild(DesignUtils.spaceBlock);
                }
            }

            #endregion

            serializedObject.Update();
        }
    }
}
