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


namespace Doozy.Editor.UIManager.Editors.Containers
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UIPopup), true)]
    public class UIPopupEditor : BaseUIContainerEditor
    {
        public UIPopup castedTarget => (UIPopup)target;
        public IEnumerable<UIPopup> castedTargets => targets.Cast<UIPopup>();

        protected FluidTab referencesTab { get; set; }

        protected FluidAnimatedContainer referencesAnimatedContainer { get; set; }

        protected SerializedProperty propertyParentMode { get; set; }
        protected SerializedProperty propertyParentTag { get; set; }
        protected SerializedProperty propertyOverrideSorting { get; set; }
        protected SerializedProperty propertyRestoreSelectedAfterHide { get; set; }
        protected SerializedProperty propertyBlockBackButton { get; set; }
        protected SerializedProperty propertyHideOnAnyButton { get; set; }
        protected SerializedProperty propertyHideOnBackButton { get; set; }
        protected SerializedProperty propertyOverlay { get; set; }
        protected SerializedProperty propertyHideOnClickOverlay { get; set; }
        protected SerializedProperty propertyContainer { get; set; }
        protected SerializedProperty propertyHideOnClickContainer { get; set; }
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

            propertyParentMode = serializedObject.FindProperty(nameof(UIPopup.ParentMode));
            propertyParentTag = serializedObject.FindProperty(nameof(UIPopup.ParentTag));
            propertyOverrideSorting = serializedObject.FindProperty(nameof(UIPopup.OverrideSorting));
            propertyRestoreSelectedAfterHide = serializedObject.FindProperty(nameof(UIPopup.RestoreSelectedAfterHide));
            propertyBlockBackButton = serializedObject.FindProperty(nameof(UIPopup.BlockBackButton));
            propertyHideOnAnyButton = serializedObject.FindProperty(nameof(UIPopup.HideOnAnyButton));
            propertyHideOnBackButton = serializedObject.FindProperty(nameof(UIPopup.HideOnBackButton));
            propertyOverlay = serializedObject.FindProperty(nameof(UIPopup.Overlay));
            propertyHideOnClickOverlay = serializedObject.FindProperty(nameof(UIPopup.HideOnClickOverlay));
            propertyContainer = serializedObject.FindProperty(nameof(UIPopup.Container));
            propertyHideOnClickContainer = serializedObject.FindProperty(nameof(UIPopup.HideOnClickContainer));
            propertyLabels = serializedObject.FindProperty(nameof(UIPopup.Labels));
            propertyImages = serializedObject.FindProperty(nameof(UIPopup.Images));
            propertyButtons = serializedObject.FindProperty(nameof(UIPopup.Buttons));
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText("UIPopup")
                .SetIcon(EditorSpriteSheets.UIManager.Icons.UIPopup)
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
                #region Overlay

                ObjectField overlayObjectField =
                    DesignUtils.NewObjectField(propertyOverlay, typeof(RectTransform))
                        .SetStyleFlexGrow(1)
                        .SetTooltip("Reference to the popup's Overlay RectTransform");

                FluidField overlayFluidField =
                    FluidField.Get("Overlay")
                        .AddFieldContent(overlayObjectField);

                #endregion

                #region Container

                ObjectField containerObjectField =
                    DesignUtils.NewObjectField(propertyContainer, typeof(RectTransform))
                        .SetStyleFlexGrow(1)
                        .SetTooltip("Reference to the popup's Container RectTransform");


                FluidField containerFluidField =
                    FluidField.Get("Container")
                        .AddFieldContent(containerObjectField);

                #endregion

                #region Parent Mode

                EnumField parentModeEnumField =
                    DesignUtils.NewEnumField(propertyParentMode)
                        .SetStyleFlexGrow(1)
                        .SetTooltip("Set where a popup should be instantiated under");

                FluidField parentModeFluidField =
                    FluidField.Get("Parent Mode")
                        .AddFieldContent(parentModeEnumField);

                #endregion

                #region Parent Tag

                FluidField parentTagFluidField =
                    FluidField.Get("Parent Tag")
                        .SetTooltip("Id used to identify the designated parent where the popup should be parented under, after it has been instantiated")
                        .AddFieldContent(DesignUtils.NewPropertyField(propertyParentTag))
                        .SetStylePaddingTop(DesignUtils.k_Spacing)
                        .SetStylePaddingBottom(DesignUtils.k_Spacing);

                parentTagFluidField.SetStyleDisplay((UIPopup.Parenting)propertyParentMode.enumValueIndex == UIPopup.Parenting.UITag ? DisplayStyle.Flex : DisplayStyle.None);
                parentModeEnumField.RegisterValueChangedCallback(evt =>
                {
                    if (evt?.newValue == null) return;
                    parentTagFluidField.SetStyleDisplay((UIPopup.Parenting)evt.newValue == UIPopup.Parenting.UITag ? DisplayStyle.Flex : DisplayStyle.None);
                });

                #endregion

                #region Override Sorting

                FluidToggleSwitch overrideSortingToggleSwitch =
                    FluidToggleSwitch.Get(propertyOverrideSorting.boolValue ? "Enabled" : "Disabled")
                        .SetToggleAccentColor(selectableAccentColor)
                        .BindToProperty(propertyOverrideSorting)
                        .SetTooltip($"Enable override sorting and set the sorting order to the maximum value ({UIPopup.k_MaxSortingOrder}) for the Canvas component attached to this popup");

                overrideSortingToggleSwitch.SetOnValueChanged
                (
                    evt =>
                        overrideSortingToggleSwitch.SetLabelText(evt.newValue ? "Enabled" : "Disabled")
                );

                FluidField overrideSortingFluidField =
                    FluidField.Get("Override Sorting Order")
                        .AddFieldContent(overrideSortingToggleSwitch);

                #endregion

                #region Hide on Back Button & Block Back Button

                FluidToggleSwitch blockBackButtonToggleSwitch =
                    FluidToggleSwitch.Get("Block 'Back' Button")
                        .SetToggleAccentColor(selectableAccentColor)
                        .BindToProperty(propertyBlockBackButton)
                        .SetTooltip("Block the 'Back' button when the popup is visible");

                FluidField backButtonFluidField =
                    FluidField.Get()
                        .AddFieldContent(blockBackButtonToggleSwitch);

                #endregion

                #region Hide Options

                FluidToggleSwitch buttonsHideTooltipToggleSwitch =
                    FluidToggleSwitch.Get("Any Button")
                        .SetToggleAccentColor(selectableAccentColor)
                        .BindToProperty(propertyHideOnAnyButton)
                        .SetTooltip
                        (
                            "Hide (close) the popup when the user clicks on any of the UIButton references." +
                            "\n\nAt runtime, a 'hide popup' event will be added to all the referenced UIButtons (if any)."
                        );

                FluidToggleSwitch hideOnBackButtonToggleSwitch =
                    FluidToggleSwitch.Get("'Back' Button")
                        .SetToggleAccentColor(selectableAccentColor)
                        .BindToProperty(propertyHideOnBackButton)
                        .SetTooltip("Set the next 'Back' button event to hide (close) this popup");

                FluidToggleSwitch hideOnClickOverlayToggleSwitch =
                    FluidToggleSwitch.Get("Click Overlay")
                        .SetToggleAccentColor(selectableAccentColor)
                        .BindToProperty(propertyHideOnClickOverlay)
                        .SetTooltip("Set the next click (on the Overlay) to hide (close) this popup");

                FluidToggleSwitch hideOnClickContainerToggleSwitch =
                    FluidToggleSwitch.Get("Click Container")
                        .SetToggleAccentColor(selectableAccentColor)
                        .BindToProperty(propertyHideOnClickContainer)
                        .SetTooltip("Set the next click (on the Container) to hide (close) this popup");


                FluidField hideOptionsFluidField =
                    FluidField.Get("Hide this popup via")
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddChild(buttonsHideTooltipToggleSwitch)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(hideOnBackButtonToggleSwitch)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(hideOnClickOverlayToggleSwitch)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(hideOnClickContainerToggleSwitch)
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

                #region Selected

                FluidToggleSwitch clearSelectedOnShowSwitch =
                    FluidToggleSwitch.Get()
                        .BindToProperty(propertyClearSelectedOnShow)
                        .SetLabelText("Show")
                        .SetTooltip("If TRUE, when this container is shown, any GameObject that is selected by the EventSystem.current will get deselected")
                        .SetToggleAccentColor(selectableAccentColor);

                FluidToggleSwitch clearSelectedOnHideSwitch =
                    FluidToggleSwitch.Get()
                        .BindToProperty(propertyClearSelectedOnHide)
                        .SetLabelText("Hide")
                        .SetTooltip("If TRUE, when this container is hidden, any GameObject that is selected by the EventSystem.current will get deselected")
                        .SetToggleAccentColor(selectableAccentColor);

                FluidField clearSelectedFluidField =
                    FluidField.Get()
                        .SetLabelText("Clear Selected on")
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddChild(clearSelectedOnShowSwitch)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(clearSelectedOnHideSwitch)
                        )
                        .SetStyleMinWidth(150);

                FluidToggleSwitch autoSelectAfterShowSwitch =
                    FluidToggleSwitch.Get()
                        .BindToProperty(propertyAutoSelectAfterShow)
                        .SetTooltip("If TRUE, after this container has been shown, the referenced selectable GameObject will get automatically selected by EventSystem.current")
                        .SetToggleAccentColor(selectableAccentColor);

                ObjectField autoSelectTargetObjectField =
                    DesignUtils.NewObjectField(propertyAutoSelectTarget, typeof(GameObject))
                        .SetTooltip("Reference to the GameObject that should be selected after this view has been shown. Works only if AutoSelectAfterShow is TRUE");

                autoSelectTargetObjectField.SetEnabled(propertyAutoSelectAfterShow.boolValue);
                autoSelectAfterShowSwitch.SetOnValueChanged(evt =>
                {
                    if (evt?.newValue == null) return;
                    autoSelectTargetObjectField.SetEnabled(evt.newValue);
                });

                FluidField autoSelectAfterShowFluidField =
                    FluidField.Get()
                        .SetLabelText("Auto select GameObject after Show")
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddChild(autoSelectAfterShowSwitch)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(autoSelectTargetObjectField)
                        );

                #endregion

                #region Restore Selected After Hide

                FluidToggleSwitch restoreSelectedAfterHideSwitch =
                    FluidToggleSwitch.Get(propertyRestoreSelectedAfterHide.boolValue ? "Enabled" : "Disabled")
                        .SetToggleAccentColor(selectableAccentColor)
                        .BindToProperty(propertyRestoreSelectedAfterHide)
                        .SetTooltip
                        (
                            "Reselect the previously selected GameObject when the popup is hidden." +
                            "\n\nThis is useful when the popup is hidden from a button that was selected before the popup was shown." +
                            "\n\nEventSystem.current is used to determine the currently selected GameObject."
                        );

                restoreSelectedAfterHideSwitch.SetOnValueChanged(evt => restoreSelectedAfterHideSwitch.SetLabelText(evt.newValue ? "Enabled" : "Disabled"));

                FluidField selectionRestoreFluidField =
                    FluidField.Get("Restore Selected after Hide")
                        .AddFieldContent(restoreSelectedAfterHideSwitch);

                #endregion

                settingsAnimatedContainer
                    .AddContent
                    (
                        DesignUtils.row
                            .AddChild(overlayFluidField)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(containerFluidField)
                    )
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(parentModeFluidField)
                    .AddContent(parentTagFluidField)
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(overrideSortingFluidField)
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(backButtonFluidField)
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(hideOptionsFluidField)
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(autoHideAfterShowFluidField)
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent
                    (
                        DesignUtils.row
                            .AddChild(clearSelectedFluidField)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(autoSelectAfterShowFluidField)
                    )
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(selectionRestoreFluidField)
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
                            nameof(UIPopup)
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
