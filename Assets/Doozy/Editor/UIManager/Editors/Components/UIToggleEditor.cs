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
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Components
{
    [CustomEditor(typeof(UIToggle), true)]
    [CanEditMultipleObjects]
    public sealed class UIToggleEditor : UISelectableBaseEditor
    {
        public override Color accentColor => EditorColors.UIManager.UIComponent;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.UIComponent;

        public UIToggle castedTarget => (UIToggle)target;
        public List<UIToggle> castedTargets => targets.Cast<UIToggle>().ToList();

        private FluidTab callbacksTab { get; set; }
        private FluidAnimatedContainer callbacksAnimatedContainer { get; set; }
        private FluidField idField { get; set; }

        private SerializedProperty propertyId { get; set; }
        private SerializedProperty propertyIsOn { get; set; }
        private SerializedProperty propertyCooldown { get; set; }
        private SerializedProperty propertyDisableWhenInCooldown { get; set; }
        private SerializedProperty propertyOnInstantToggleOffCallback { get; set; }
        private SerializedProperty propertyOnInstantToggleOnCallback { get; set; }
        private SerializedProperty propertyOnToggleOffCallback { get; set; }
        private SerializedProperty propertyOnToggleOnCallback { get; set; }
        private SerializedProperty propertyOnValueChangedCallback { get; set; }
        private SerializedProperty propertyToggleGroup { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            callbacksTab?.Dispose();
            callbacksAnimatedContainer?.Dispose();
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

            propertyId = serializedObject.FindProperty(nameof(UIToggle.Id));
            propertyCooldown = serializedObject.FindProperty(nameof(UIToggle.Cooldown));
            propertyDisableWhenInCooldown = serializedObject.FindProperty(nameof(UIToggle.DisableWhenInCooldown));
            propertyIsOn = serializedObject.FindProperty("IsOn");
            propertyOnInstantToggleOffCallback = serializedObject.FindProperty(nameof(UIToggle.OnInstantToggleOffCallback));
            propertyOnInstantToggleOnCallback = serializedObject.FindProperty(nameof(UIToggle.OnInstantToggleOnCallback));
            propertyOnToggleOffCallback = serializedObject.FindProperty(nameof(UIToggle.OnToggleOffCallback));
            propertyOnToggleOnCallback = serializedObject.FindProperty(nameof(UIToggle.OnToggleOnCallback));
            propertyOnValueChangedCallback = serializedObject.FindProperty(nameof(UIToggle.OnValueChangedCallback));
            propertyToggleGroup = serializedObject.FindProperty("ToggleGroup");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetAccentColor(accentColor)
                .SetComponentNameText("UIToggle")
                .SetIcon(EditorSpriteSheets.UIManager.Icons.UIToggle)
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048084542/UIToggle?atlOrigin=eyJpIjoiNjQ4NmRmNjIyNjY2NDM5YmEyOTBlMzhhZjFmZWI0ZDciLCJwIjoiYyJ9")
                .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.UIManager.Components.UIToggle.html")
                .AddYouTubeButton();


            idField = FluidField.Get().AddFieldContent(DesignUtils.NewPropertyField(propertyId));

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
                    
                    return castedTarget.OnToggleOnCallback.hasCallbacks ||
                           castedTarget.OnInstantToggleOnCallback.hasCallbacks ||
                           castedTarget.OnToggleOffCallback.hasCallbacks ||
                           castedTarget.OnInstantToggleOffCallback.hasCallbacks ||
                           castedTarget.OnValueChangedCallback?.GetPersistentEventCount() > 0;
                }

                //initial indicators state update (no animation)
                UpdateIndicator(callbacksTab, HasCallbacks(), false);

                //subsequent indicators state update (animated)
                root.schedule.Execute(() =>
                {
                    UpdateIndicator(callbacksTab, HasCallbacks(), true);

                }).Every(200);
            });
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

                #region Cooldown

                FluidToggleCheckbox disableWhenInCooldownCheckbox =
                    FluidToggleCheckbox.Get()
                        .SetLabelText("Disable when in Cooldown")
                        .SetTooltip("Set the toggle's interactable state to false during the cooldown time.")
                        .SetToggleAccentColor(selectableAccentColor)
                        .BindToProperty(propertyDisableWhenInCooldown)
                        .SetStyleAlignSelf(Align.FlexEnd);
                
                FluidField cooldownFluidField =
                    FluidField.Get("Cooldown")
                        .SetTooltip
                        (
                            "Cooldown time in seconds before the toggle can be clicked again." +
                            "\n\n" +
                            "This is useful when you want to prevent the toggle from being clicked multiple times in a short period of time." +
                            "\n\n" +
                            "Set to 0 to disable the cooldown and make the toggle clickable every time."
                        )
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .SetStyleAlignItems(Align.Center)
                                .AddChild(DesignUtils.NewFloatField(propertyCooldown).SetStyleWidth(40))
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(DesignUtils.fieldLabel.SetText("seconds"))
                                .AddChild(DesignUtils.spaceBlock2X)
                                .AddChild(disableWhenInCooldownCheckbox)
                        );
                
                #endregion
                
                #region Is On

                FluidToggleCheckbox isOnCheckbox =
                    FluidToggleCheckbox.Get()
                        .SetLabelText("Is On")
                        .BindToProperty(propertyIsOn)
                        .SetOnClick(() =>
                        {
                            if (Application.isPlaying)
                            {
                                foreach (var t in castedTargets)
                                    t.isOn = !castedTarget.isOn;
                                return;
                            }
                            HeartbeatCheck();
                            foreach (var a in selectableAnimators.RemoveNulls())
                            {
                                switch (a.ToggleCommand)
                                {
                                    case CommandToggle.On when !castedTarget.isOn:
                                    case CommandToggle.Off when castedTarget.isOn:
                                        continue;
                                    default:
                                        a.Play(castedSelectable.currentUISelectionState);
                                        break;
                                }
                            }
                        });

                #endregion

                #region Toggle Group

                ObjectField toggleGroupObjectField =
                    DesignUtils.NewObjectField(propertyToggleGroup, typeof(UIToggleGroup))
                        .SetTooltip("The toggle group this toggle belongs to")
                        .SetStyleFlexGrow(1);

                FluidField toggleGroupField =
                    FluidField.Get()
                        .SetLabelText("Toggle Group")
                        .SetIcon(EditorSpriteSheets.UIManager.Icons.UIToggleGroup)
                        .AddFieldContent(toggleGroupObjectField);

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
                    .AddContent(isOnCheckbox)
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(cooldownFluidField)
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(toggleGroupField)
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
                    .AddContent(GetField(propertyOnToggleOnCallback, "Toggle ON - toggle value changed from OFF to ON"))
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(GetField(propertyOnInstantToggleOnCallback, "Instant Toggle ON - toggle value changed from OFF to ON (without animations)"))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(GetField(propertyOnToggleOffCallback, "Toggle OFF - toggle value changed from ON to OFF"))
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(GetField(propertyOnInstantToggleOffCallback, "Instant Toggle OFF - toggle value changed from ON to OFF (without animations)"))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(GetField(propertyOnValueChangedCallback, "Toggle Value Changed - toggle value changed"))
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
                            nameof(UIToggle),
                            nameof(UIToggleGroup)
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
