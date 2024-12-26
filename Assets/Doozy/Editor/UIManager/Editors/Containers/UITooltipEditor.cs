// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIManager.Editors.Containers.Internal;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using TMPro;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

// ReSharper disable UnusedMember.Global
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Containers
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UITooltip), true)]
    public class UITooltipEditor : BaseUIContainerEditor
    {
        public UITooltip castedTarget => (UITooltip)target;
        public IEnumerable<UITooltip> castedTargets => targets.Cast<UITooltip>();

        protected FluidTab referencesTab { get; set; }

        protected FluidAnimatedContainer referencesAnimatedContainer { get; set; }

        protected SerializedProperty propertyParentMode { get; set; }
        protected SerializedProperty propertyTrackingMode { get; set; }
        protected SerializedProperty propertyPositionMode { get; set; }
        protected SerializedProperty propertyParentTag { get; set; }
        protected SerializedProperty propertyFollowTag { get; set; }
        protected SerializedProperty propertyPositionOffset { get; set; }
        protected SerializedProperty propertyMaximumWidth { get; set; }
        protected SerializedProperty propertyKeepInScreen { get; set; }
        protected SerializedProperty propertyOverrideSorting { get; set; }
        protected SerializedProperty propertyHideOnAnyButton { get; set; }
        protected SerializedProperty propertyHideOnBackButton { get; set; }
        protected SerializedProperty propertyLabels { get; set; }
        protected SerializedProperty propertyImages { get; set; }
        protected SerializedProperty propertyButtons { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            referencesTab?.Dispose();
            referencesAnimatedContainer?.Dispose();
        }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyParentMode = serializedObject.FindProperty(nameof(UITooltip.ParentMode));
            propertyTrackingMode = serializedObject.FindProperty(nameof(UITooltip.TrackingMode));
            propertyPositionMode = serializedObject.FindProperty(nameof(UITooltip.PositionMode));
            propertyParentTag = serializedObject.FindProperty(nameof(UITooltip.ParentTag));
            propertyFollowTag = serializedObject.FindProperty(nameof(UITooltip.FollowTag));
            propertyPositionOffset = serializedObject.FindProperty(nameof(UITooltip.PositionOffset));
            propertyMaximumWidth = serializedObject.FindProperty(nameof(UITooltip.MaximumWidth));
            propertyKeepInScreen = serializedObject.FindProperty(nameof(UITooltip.KeepInScreen));
            propertyOverrideSorting = serializedObject.FindProperty(nameof(UITooltip.OverrideSorting));
            propertyHideOnAnyButton = serializedObject.FindProperty(nameof(UITooltip.HideOnAnyButton));
            propertyHideOnBackButton = serializedObject.FindProperty(nameof(UITooltip.HideOnBackButton));
            propertyLabels = serializedObject.FindProperty(nameof(UITooltip.Labels));
            propertyImages = serializedObject.FindProperty(nameof(UITooltip.Images));
            propertyButtons = serializedObject.FindProperty(nameof(UITooltip.Buttons));
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText("UITooltip")
                .SetIcon(EditorSpriteSheets.UIManager.Icons.UITooltip)
                .AddManualButton()
                .AddApiButton()
                .AddYouTubeButton();

            InitializeReferences();
        }

        protected virtual void InitializeReferences()
        {
            referencesAnimatedContainer = new FluidAnimatedContainer("References", true).Hide(false);
            referencesTab =
                new FluidTab()
                    .SetLabelText("References")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Link)
                    .SetElementSize(ElementSize.Small)
                    .IndicatorSetEnabledColor(accentColor)
                    .ButtonSetAccentColor(selectableAccentColor)
                    .ButtonSetOnValueChanged(evt => referencesAnimatedContainer.Toggle(evt.newValue, evt.animateChange))
                    .AddToToggleGroup(tabsGroup);

            referencesAnimatedContainer.SetOnShowCallback(() =>
            {
                FluidField Get(string fieldName, SerializedProperty property, Type objectType) =>
                    FluidField.Get(fieldName)
                        .AddFieldContent
                        (
                            DesignUtils.NewObjectListView(property, "", "", objectType)
                                .ShowItemIndex(false)
                                .SetItemHeight(48)
                        );

                FluidField labelsFluidField = Get("Labels", propertyLabels, typeof(TextMeshProUGUI));
                FluidField imagesFluidField = Get("Images", propertyImages, typeof(UnityEngine.UI.Image));
                FluidField buttonsFluidField = Get("Buttons", propertyButtons, typeof(UIButton));

                referencesAnimatedContainer
                    .AddContent(labelsFluidField)
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(imagesFluidField)
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(buttonsFluidField)
                    .AddContent(DesignUtils.endOfLineBlock)
                    .Bind(serializedObject);

            });
        }

        protected override void InitializeSettings()
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
                #region Parent Mode

                EnumField parentModeEnumField =
                    DesignUtils.NewEnumField(propertyParentMode)
                        .SetStyleFlexGrow(1)
                        .SetTooltip("Set where a tooltip should be instantiated under");

                FluidField parentModeFluidField =
                    FluidField.Get("Parent Mode")
                        .AddFieldContent(parentModeEnumField);

                #endregion

                #region Tracking Mode

                EnumField trackingModeEnumField =
                    DesignUtils.NewEnumField(propertyTrackingMode)
                        .SetStyleFlexGrow(1)
                        .SetTooltip("Set how the tooltip behaves when it is shown");

                FluidField trackingModeFluidField =
                    FluidField.Get("Tracking Mode")
                        .AddFieldContent(trackingModeEnumField);

                #endregion

                #region Position Mode

                EnumField positionModeEnumField =
                    DesignUtils.NewEnumField(propertyPositionMode)
                        .SetStyleFlexGrow(1)
                        .SetTooltip("Set where the tooltip should be positioned relative to the tracked target");

                FluidField positionModeFluidField =
                    FluidField.Get("Position Mode")
                        .AddFieldContent(positionModeEnumField);

                #endregion

                #region Parent Tag

                FluidField parentTagFluidField =
                    FluidField.Get("Parent Tag")
                        .SetTooltip("Id used to identify the designated parent where the tooltip should be parented under, after it has been instantiated")
                        .AddFieldContent(DesignUtils.NewPropertyField(propertyParentTag))
                        .SetStylePaddingTop(DesignUtils.k_Spacing)
                        .SetStylePaddingBottom(DesignUtils.k_Spacing);

                parentTagFluidField.SetStyleDisplay((UITooltip.Parenting)propertyParentMode.enumValueIndex == UITooltip.Parenting.UITag ? DisplayStyle.Flex : DisplayStyle.None);
                parentModeEnumField.RegisterValueChangedCallback(evt =>
                {
                    if (evt?.newValue == null) return;
                    parentTagFluidField.SetStyleDisplay((UITooltip.Parenting)evt.newValue == UITooltip.Parenting.UITag ? DisplayStyle.Flex : DisplayStyle.None);
                });

                #endregion

                #region Follow Tag

                FluidField followTagFluidField =
                    FluidField.Get("Follow Tag")
                        .SetTooltip("Id used to identify the follow target when the tracking mode is set to FollowTarget")
                        .AddFieldContent(DesignUtils.NewPropertyField(propertyFollowTag))
                        .SetStylePaddingTop(DesignUtils.k_Spacing)
                        .SetStylePaddingBottom(DesignUtils.k_Spacing);


                followTagFluidField.SetStyleDisplay((UITooltip.Tracking)propertyTrackingMode.enumValueIndex == UITooltip.Tracking.FollowTarget ? DisplayStyle.Flex : DisplayStyle.None);
                trackingModeEnumField.RegisterValueChangedCallback(evt =>
                {
                    if (evt?.newValue == null) return;
                    followTagFluidField.SetStyleDisplay((UITooltip.Tracking)evt.newValue == UITooltip.Tracking.FollowTarget ? DisplayStyle.Flex : DisplayStyle.None);
                });

                #endregion

                #region Position Offset

                FluidField positionOffsetFluidField =
                    FluidField.Get("Position Offset")
                        .AddFieldContent
                        (
                            DesignUtils.NewVector3Field(propertyPositionOffset)
                                .SetStyleFlexGrow(1)
                                .SetTooltip("Set the offset applied to the tooltip position, after all the positioning has been applied")
                        );

                #endregion

                #region Maximum Width

                FluidField maximumWidthFluidField =
                    FluidField.Get("Maximum Width")
                        .AddFieldContent
                        (
                            DesignUtils.NewFloatField(propertyMaximumWidth)
                                .SetStyleFlexGrow(1)
                                .SetTooltip("Set a maximum width for the tooltip. If the value is 0, the tooltip will be automatically sized to fit the content")
                        );

                #endregion

                #region Override Sorting

                FluidToggleSwitch overrideSortingToggleSwitch =
                    FluidToggleSwitch.Get(propertyOverrideSorting.boolValue ? "Enabled" : "Disabled")
                        .SetToggleAccentColor(selectableAccentColor)
                        .BindToProperty(propertyOverrideSorting)
                        .SetTooltip($"Enable override sorting and set the sorting order to the maximum value ({UITooltip.k_MaxSortingOrder}) for the Canvas component attached to this tooltip");

                overrideSortingToggleSwitch.SetOnValueChanged
                (
                    evt =>
                        overrideSortingToggleSwitch.SetLabelText(evt.newValue ? "Enabled" : "Disabled")
                );

                FluidField overrideSortingFluidField =
                    FluidField.Get("Override Sorting Order")
                        .AddFieldContent(overrideSortingToggleSwitch);

                #endregion

                #region Keep In Screen

                FluidToggleSwitch keepInScreenToggleSwitch =
                    FluidToggleSwitch.Get(propertyKeepInScreen.boolValue ? "Enabled" : "Disabled")
                        .SetToggleAccentColor(selectableAccentColor)
                        .BindToProperty(propertyKeepInScreen)
                        .SetTooltip("Keep the tooltip in screen at all times, while it is shown");

                keepInScreenToggleSwitch.SetOnValueChanged
                (
                    evt =>
                        keepInScreenToggleSwitch.SetLabelText(evt.newValue ? "Enabled" : "Disabled")
                );

                FluidField keepInScreenFluidField =
                    FluidField.Get("Keep In Screen")
                        .AddFieldContent(keepInScreenToggleSwitch);

                #endregion

                #region Hide Options

                FluidToggleSwitch buttonsHideTooltipToggleSwitch =
                    FluidToggleSwitch.Get("Any Button")
                        .SetToggleAccentColor(selectableAccentColor)
                        .BindToProperty(propertyHideOnAnyButton)
                        .SetTooltip
                        (
                            "Hide (close) the tooltip when the user clicks on any of the UIButton references." +
                            "\n\nAt runtime, a 'hide tooltip' event will be added to all the referenced UIButtons (if any)."
                        );

                FluidToggleSwitch hideOnBackButtonToggleSwitch =
                    FluidToggleSwitch.Get("'Back' Button")
                        .SetToggleAccentColor(selectableAccentColor)
                        .BindToProperty(propertyHideOnBackButton)
                        .SetTooltip("Set the next 'Back' button event to hide (close) this tooltip");

                FluidField hideOptionsFluidField =
                    FluidField.Get("Hide this tooltip via")
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddChild(buttonsHideTooltipToggleSwitch)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(hideOnBackButtonToggleSwitch)
                        );

                #endregion

                #region Auto Hide after Show

                Label hideDelayLabel =
                    DesignUtils.NewLabel("Auto Hide Delay")
                        .SetStyleMarginRight(DesignUtils.k_Spacing);

                FloatField hideDelayPropertyField =
                    DesignUtils.NewFloatField(propertyAutoHideAfterShowDelay)
                        .SetTooltip("Time interval after which Hide is triggered")
                        .SetStyleWidth(40)
                        .SetStyleMarginRight(DesignUtils.k_Spacing);

                Label secondsLabel =
                    DesignUtils.NewLabel("seconds");

                hideDelayLabel.SetEnabled(propertyAutoHideAfterShow.boolValue);
                hideDelayPropertyField.SetEnabled(propertyAutoHideAfterShow.boolValue);
                secondsLabel.SetEnabled(propertyAutoHideAfterShow.boolValue);

                FluidToggleSwitch autoHideAfterShowSwitch =
                    FluidToggleSwitch.Get()
                        .BindToProperty(propertyAutoHideAfterShow)
                        .SetTooltip("If TRUE, after Show, Hide it will get automatically triggered after the AutoHideAfterShowDelay time interval has passed")
                        .SetOnValueChanged(evt =>
                        {
                            if (evt?.newValue == null) return;
                            hideDelayLabel.SetEnabled(evt.newValue);
                            hideDelayPropertyField.SetEnabled(evt.newValue);
                            secondsLabel.SetEnabled(evt.newValue);
                        })
                        .SetToggleAccentColor(selectableAccentColor)
                        .SetStyleMarginRight(DesignUtils.k_Spacing);

                FluidField autoHideAfterShowFluidField =
                    FluidField.Get("Auto Hide after Show")
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddChild(autoHideAfterShowSwitch)
                                .AddChild(hideDelayLabel)
                                .AddChild(hideDelayPropertyField)
                                .AddChild(secondsLabel)
                                .AddChild(DesignUtils.flexibleSpace)
                        );

                #endregion

                settingsAnimatedContainer
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
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent
                    (
                        DesignUtils.row
                            .AddChild(positionOffsetFluidField)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(maximumWidthFluidField)
                    )
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent
                    (
                        DesignUtils.row
                            .AddChild(overrideSortingFluidField)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(keepInScreenFluidField)
                    )
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(hideOptionsFluidField)
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(autoHideAfterShowFluidField)
                    .AddContent(DesignUtils.endOfLineBlock)
                    .Bind(serializedObject);
            });
        }

        protected override VisualElement Toolbar()
        {
            return
                toolbarContainer
                    .AddChild(settingsTab)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(referencesTab)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(callbacksTab)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(progressorsTab)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(DesignUtils.flexibleSpace)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild
                    (
                        DesignUtils.SystemButton_SortComponents
                        (
                            castedContainer.gameObject,
                            nameof(RectTransform),
                            nameof(UITooltip)
                        )
                    );
        }

        protected override VisualElement Content()
        {
            return
                contentContainer
                    .AddChild(settingsAnimatedContainer)
                    .AddChild(referencesAnimatedContainer)
                    .AddChild(callbacksAnimatedContainer)
                    .AddChild(progressorsAnimatedContainer);
        }
    }
}
