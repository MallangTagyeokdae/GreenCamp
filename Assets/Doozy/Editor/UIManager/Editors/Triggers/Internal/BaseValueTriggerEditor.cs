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
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Triggers.Internal;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable Unity.RedundantEventFunction

namespace Doozy.Editor.UIManager.Editors.Triggers.Internal
{
    public abstract class BaseValueTriggerEditor<Tbehaviour> : UnityEditor.Editor where Tbehaviour : MonoBehaviour
    {
        protected virtual Color accentColor => EditorColors.UIManager.ListenerComponent;
        protected virtual EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.ListenerComponent;

        protected BaseValueTrigger<Tbehaviour> castedTarget => (BaseValueTrigger<Tbehaviour>)target;
        protected IEnumerable<BaseValueTrigger<Tbehaviour>> castedTargets => targets.Cast<BaseValueTrigger<Tbehaviour>>();

        protected VisualElement root { get; private set; }
        protected FluidComponentHeader componentHeader { get; private set; }
        protected VisualElement toolbarContainer { get; private set; }
        protected VisualElement contentContainer { get; private set; }

        protected FluidToggleGroup tabsGroup { get; set; }
        protected FluidTab settingsTab { get; set; }
        protected FluidTab callbacksTab { get; set; }

        protected FluidAnimatedContainer settingsAnimatedContainer { get; set; }
        protected FluidAnimatedContainer callbacksAnimatedContainer { get; set; }

        protected SerializedProperty propertyTarget { get; set; }
        protected SerializedProperty propertyTriggerMode { get; set; }
        protected SerializedProperty propertyTriggerValue { get; set; }
        protected SerializedProperty propertyOnTrigger { get; set; }
        protected SerializedProperty propertyTriggerOnExactValueMatch { get; set; }
        protected SerializedProperty propertyTriggerOnIncrement { get; set; }
        protected SerializedProperty propertyTriggerOnDecrement { get; set; }

        protected abstract VisualElement GetTargetObjectField();

        protected virtual void OnEnable() {}

        protected virtual void OnDisable() {}

        protected virtual void OnDestroy()
        {
            componentHeader?.Recycle();
            tabsGroup?.Recycle();
            settingsTab?.Recycle();
            callbacksTab?.Recycle();
            settingsAnimatedContainer?.Dispose();
            callbacksAnimatedContainer?.Dispose();
        }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        protected virtual void FindProperties()
        {
            propertyTarget = serializedObject.FindProperty("Target");
            propertyTriggerMode = serializedObject.FindProperty("TriggerMode");
            propertyTriggerValue = serializedObject.FindProperty("TriggerValue");
            propertyOnTrigger = serializedObject.FindProperty("OnTrigger");
            propertyTriggerOnExactValueMatch = serializedObject.FindProperty("TriggerOnExactValueMatch");
            propertyTriggerOnIncrement = serializedObject.FindProperty("TriggerOnIncrement");
            propertyTriggerOnDecrement = serializedObject.FindProperty("TriggerOnDecrement");
        }

        protected virtual void InitializeEditor()
        {
            FindProperties();
            root = DesignUtils.GetEditorRoot();
            componentHeader =
                DesignUtils.editorComponentHeader
                    .SetAccentColor(accentColor)
                    .SetIcon(EditorSpriteSheets.UIManager.Icons.ValueTrigger)
                    .SetComponentTypeText("Value Trigger");
            toolbarContainer = DesignUtils.editorToolbarContainer;
            tabsGroup = FluidToggleGroup.Get().SetControlMode(FluidToggleGroup.ControlMode.OneToggleOn);
            contentContainer = DesignUtils.editorContentContainer;

            InitializeSettings();
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
                        castedTarget.OnTrigger.hasCallbacks;
                }
                
                //initial indicators state update (no animation)
                UpdateIndicator(callbacksTab, HasCallbacks(), false);

                root.schedule.Execute(() =>
                {
                    //update indicators state (with animation)
                    UpdateIndicator(callbacksTab, HasCallbacks(), true);
                    
                }).Every(200);
            });
        }

        protected virtual void InitializeSettings()
        {
            settingsAnimatedContainer = new FluidAnimatedContainer("Settings", true).Hide(false);
            settingsTab =
                GetTab("Settings")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Settings)
                    .ButtonSetOnValueChanged(evt => settingsAnimatedContainer.Toggle(evt.newValue, evt.animateChange));

            settingsAnimatedContainer.SetOnShowCallback(() =>
            {
                EnumField triggerModeEnumField =
                    DesignUtils.NewEnumField(propertyTriggerMode)
                        .SetTooltip("Trigger condition")
                        .SetStyleWidth(170)
                        .SetStyleFlexGrow(1);

                FloatField triggerValueFloatField =
                    DesignUtils.NewFloatField(propertyTriggerValue)
                        .SetTooltip("Value that will be used to trigger the event")
                        .SetStyleFlexGrow(1);

                FluidToggleCheckbox triggerOnExactValueMatchCheckbox =
                    FluidToggleCheckbox.Get()
                        .SetLabelText("Trigger On Exact Value Match")
                        .SetTooltip("If TRUE, the trigger will be activated when the value is EXACTLY equal to the TargetValue")
                        .SetToggleAccentColor(selectableAccentColor)
                        .BindToProperty(propertyTriggerOnExactValueMatch);

                FluidToggleCheckbox triggerOnIncrementCheckbox =
                    FluidToggleCheckbox.Get()
                        .SetLabelText("Trigger On Increment")
                        .SetTooltip("If TRUE, the trigger will be activated only when the value was increasing and it is now equal to the TargetValue")
                        .SetToggleAccentColor(selectableAccentColor)
                        .BindToProperty(propertyTriggerOnIncrement);

                FluidToggleCheckbox triggerOnDecrementCheckbox =
                    FluidToggleCheckbox.Get()
                        .SetLabelText("Trigger On Decrement")
                        .SetTooltip("If TRUE, the trigger will be activated only when the value was decreasing and it is now equal to the TargetValue")
                        .SetToggleAccentColor(selectableAccentColor)
                        .BindToProperty(propertyTriggerOnDecrement);

                ObjectField targetObjectField =
                    DesignUtils.NewObjectField(propertyTarget, typeof(Tbehaviour), true);

                FluidField triggerModeFluidField =
                    FluidField.Get()
                        .SetLabelText("Trigger Mode")
                        .SetStyleFlexGrow(0)
                        .AddFieldContent(triggerModeEnumField);

                FluidField triggerValueFluidField =
                    FluidField.Get()
                        .SetLabelText("Trigger Value")
                        .AddFieldContent
                        (
                            triggerValueFloatField
                        );

                FluidField triggerOnFluidField =
                    FluidField.Get()
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddChild(triggerOnExactValueMatchCheckbox)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(triggerOnIncrementCheckbox)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(triggerOnDecrementCheckbox)
                        )
                        .SetStyleMarginTop(DesignUtils.k_Spacing);

                triggerOnFluidField.SetStyleDisplay
                (
                    propertyTriggerMode.enumValueIndex == (int)BaseValueTrigger<Tbehaviour>.TriggerWhenValueIs.EqualTo
                        ? DisplayStyle.Flex
                        : DisplayStyle.None
                );

                triggerModeEnumField.RegisterValueChangedCallback(evt =>
                {
                    if (evt?.newValue == null) return;

                    triggerOnFluidField.SetStyleDisplay
                    (
                        (BaseValueTrigger<Tbehaviour>.TriggerWhenValueIs)evt.newValue == BaseValueTrigger<Tbehaviour>.TriggerWhenValueIs.EqualTo
                            ? DisplayStyle.Flex
                            : DisplayStyle.None
                    );
                });

                settingsAnimatedContainer
                    .AddContent
                    (
                        DesignUtils.row
                            .AddChild(triggerModeFluidField)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(triggerValueFluidField)
                    )
                    .AddContent(triggerOnFluidField)
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(GetTargetObjectField())
                    .Bind(serializedObject);
            });
        }

        protected virtual void InitializeCallbacks()
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
                callbacksAnimatedContainer
                    .AddContent(DesignUtils.NewPropertyField(propertyOnTrigger))
                    .Bind(serializedObject);
            });
        }
        
        protected virtual VisualElement Toolbar()
        {
            return
                toolbarContainer
                    .AddChild(settingsTab)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(callbacksTab)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(DesignUtils.flexibleSpace);
        }

        protected virtual VisualElement Content()
        {
            return
                contentContainer
                    .AddChild(settingsAnimatedContainer)
                    .AddChild(callbacksAnimatedContainer);
        }

        protected virtual void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild(Toolbar())
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(Content())
                .AddChild(DesignUtils.endOfLineBlock);
        }

        protected FluidTab GetTab(string labelText) =>
            new FluidTab()
                .SetLabelText(labelText)
                .IndicatorSetEnabledColor(accentColor)
                .ButtonSetAccentColor(selectableAccentColor)
                .AddToToggleGroup(tabsGroup);
    }
}
