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
using Doozy.Runtime.UIManager.Content.Internal;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Doozy.Editor.UIManager.Editors.Content.Internal
{
    public abstract class BaseDateTimeContentEditor : UnityEditor.Editor
    {
        protected virtual Color accentColor => EditorColors.UIManager.DateTime;
        protected virtual EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.DateTime;

        protected DateTimeComponent castedDateTimeComponent => (DateTimeComponent)target;
        protected List<DateTimeComponent> castedDateTimeContents => targets.Cast<DateTimeComponent>().ToList();

        protected VisualElement root { get; set; }
        protected FluidComponentHeader componentHeader { get; set; }
        protected VisualElement toolbarContainer { get; private set; }
        protected VisualElement contentContainer { get; private set; }

        protected FluidToggleGroup tabsGroup { get; set; }
        protected FluidTab settingsTab { get; set; }
        protected FluidTab labelsTab { get; set; }
        protected FluidTab callbacksTab { get; set; }

        protected FluidAnimatedContainer settingsAnimatedContainer { get; set; }
        protected FluidAnimatedContainer labelsAnimatedContainer { get; set; }
        protected FluidAnimatedContainer callbacksAnimatedContainer { get; set; }

        protected SerializedProperty propertyLabels { get; set; }
        protected SerializedProperty propertyTimescaleMode { get; set; }
        protected SerializedProperty propertyUpdateInterval { get; set; }
        protected SerializedProperty propertyUpdateOnStart { get; set; }
        protected SerializedProperty propertyUpdateOnEnable { get; set; }
        protected SerializedProperty propertyYears { get; set; }
        protected SerializedProperty propertyMonths { get; set; }
        protected SerializedProperty propertyDays { get; set; }
        protected SerializedProperty propertyHours { get; set; }
        protected SerializedProperty propertyMinutes { get; set; }
        protected SerializedProperty propertySeconds { get; set; }
        protected SerializedProperty propertyMilliseconds { get; set; }
        protected SerializedProperty propertyOnStart { get; set; }
        protected SerializedProperty propertyOnStop { get; set; }
        protected SerializedProperty propertyOnFinish { get; set; }
        protected SerializedProperty propertyOnCancel { get; set; }
        protected SerializedProperty propertyOnPause { get; set; }
        protected SerializedProperty propertyOnResume { get; set; }
        protected SerializedProperty propertyOnReset { get; set; }
        protected SerializedProperty propertyOnUpdate { get; set; }
        protected SerializedProperty propertyOnStartBehaviour { get; set; }
        protected SerializedProperty propertyOnEnableBehaviour { get; set; }
        protected SerializedProperty propertyOnDisableBehaviour { get; set; }
        protected SerializedProperty propertyOnDestroyBehaviour { get; set; }

        protected VisualElement GetControlButtons()
        {
            VisualElement container =
                new VisualElement()
                    .SetStyleFlexDirection(FlexDirection.Row)
                    .SetStylePaddingTop(DesignUtils.k_Spacing2X)
                    .SetStylePaddingBottom(DesignUtils.k_Spacing2X)
                    .SetStyleDisplay(EditorApplication.isPlayingOrWillChangePlaymode ? DisplayStyle.Flex : DisplayStyle.None);

            FluidButton GetButton(string labelText, UnityAction callback) =>
                FluidButton.Get()
                    .SetLabelText(labelText)
                    .SetElementSize(ElementSize.Normal)
                    .SetButtonStyle(ButtonStyle.Contained)
                    .SetOnClick(callback);

            return container
                .AddChild(DesignUtils.flexibleSpace)
                .AddChild(GetButton("Start", () =>
                {
                    if (!castedDateTimeComponent.isActiveAndEnabled)
                    {
                        EditorUtility.DisplayDialog("Cannot Start", $"The {castedDateTimeComponent.name} is not active and enabled", "Ok");
                        return;
                    }
                    if(castedDateTimeComponent.isRunning & !castedDateTimeComponent.isPaused)
                    {
                        EditorUtility.DisplayDialog("Cannot Start", $"The {castedDateTimeComponent.name} is already running", "Ok");
                        return;
                    }
                    
                    castedDateTimeComponent.StartTimer();
                }))
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(GetButton("Stop", () =>
                {
                    if (!castedDateTimeComponent.isActiveAndEnabled)
                    {
                        EditorUtility.DisplayDialog("Cannot Stop", $"The {castedDateTimeComponent.name} is not active and enabled", "Ok");
                        return;
                    }
                    if(!castedDateTimeComponent.isRunning)
                    {
                        EditorUtility.DisplayDialog("Cannot Stop", $"The {castedDateTimeComponent.name} is not running", "Ok");
                        return;
                    }
                    castedDateTimeComponent.StopTimer();
                }))
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(GetButton("Finish", () =>
                {
                    if (!castedDateTimeComponent.isActiveAndEnabled)
                    {
                        EditorUtility.DisplayDialog("Cannot Finish", $"The {castedDateTimeComponent.name} is not active and enabled", "Ok");
                        return;
                    }
                    if(!castedDateTimeComponent.isRunning)
                    {
                        EditorUtility.DisplayDialog("Cannot Finish", $"The {castedDateTimeComponent.name} is not running", "Ok");
                        return;
                    }
                    
                    castedDateTimeComponent.FinishTimer();
                }))
                .AddChild(DesignUtils.spaceBlock4X)
                .AddChild(GetButton("Pause", () =>
                {
                    if (!castedDateTimeComponent.isActiveAndEnabled)
                    {
                        EditorUtility.DisplayDialog("Cannot Pause", $"The {castedDateTimeComponent.name} is not active and enabled", "Ok");
                        return;
                    }
                    
                    if(!castedDateTimeComponent.isRunning)
                    {
                        EditorUtility.DisplayDialog("Cannot Pause", $"The {castedDateTimeComponent.name} is not running", "Ok");
                        return;
                    }
                    
                    if(castedDateTimeComponent.isPaused)
                    {
                        EditorUtility.DisplayDialog("Cannot Pause", $"The {castedDateTimeComponent.name} is already paused", "Ok");
                        return;
                    }
                    
                    castedDateTimeComponent.PauseTimer();
                }))
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(GetButton("Resume", () =>
                {
                    if (!castedDateTimeComponent.isActiveAndEnabled)
                    {
                        EditorUtility.DisplayDialog("Cannot Resume", $"The {castedDateTimeComponent.name} is not active and enabled", "Ok");
                        return;
                    }
                    
                    if(!castedDateTimeComponent.isRunning)
                    {
                        EditorUtility.DisplayDialog("Cannot Resume", $"The {castedDateTimeComponent.name} is not running", "Ok");
                        return;
                    }
                    
                    if(!castedDateTimeComponent.isPaused)
                    {
                        EditorUtility.DisplayDialog("Cannot Resume", $"The {castedDateTimeComponent.name} is not paused", "Ok");
                        return;
                    }
                    
                    castedDateTimeComponent.ResumeTimer();
                }))
                .AddChild(DesignUtils.spaceBlock4X)
                .AddChild(GetButton("Reset", () =>
                {
                    if (!castedDateTimeComponent.isActiveAndEnabled)
                    {
                        EditorUtility.DisplayDialog("Cannot Reset", $"The {castedDateTimeComponent.name} is not active and enabled", "Ok");
                        return;
                    }
                    
                    castedDateTimeComponent.ResetTimer();
                }))
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(GetButton("Cancel", () =>
                {
                    if (!castedDateTimeComponent.isActiveAndEnabled)
                    {
                        EditorUtility.DisplayDialog("Cannot Cancel", $"The {castedDateTimeComponent.name} is not active and enabled", "Ok");
                        return;
                    }
                    
                    castedDateTimeComponent.CancelTimer();
                }))
                .AddChild(DesignUtils.flexibleSpace);
        }

        protected virtual void OnDestroy()
        {
            componentHeader?.Recycle();

            tabsGroup?.Recycle();
            settingsTab?.Dispose();
            callbacksTab?.Dispose();

            settingsAnimatedContainer?.Dispose();
            labelsAnimatedContainer?.Dispose();
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
            propertyLabels = serializedObject.FindProperty("Labels");
            propertyTimescaleMode = serializedObject.FindProperty("TimescaleMode");
            propertyUpdateInterval = serializedObject.FindProperty("UpdateInterval");
            propertyUpdateOnStart = serializedObject.FindProperty("UpdateOnStart");
            propertyUpdateOnEnable = serializedObject.FindProperty("UpdateOnEnable");
            propertyYears = serializedObject.FindProperty("Years");
            propertyMonths = serializedObject.FindProperty("Months");
            propertyDays = serializedObject.FindProperty("Days");
            propertyHours = serializedObject.FindProperty("Hours");
            propertyMinutes = serializedObject.FindProperty("Minutes");
            propertySeconds = serializedObject.FindProperty("Seconds");
            propertyMilliseconds = serializedObject.FindProperty("Milliseconds");
            propertyOnStart = serializedObject.FindProperty("OnStart");
            propertyOnStop = serializedObject.FindProperty("OnStop");
            propertyOnFinish = serializedObject.FindProperty("OnFinish");
            propertyOnCancel = serializedObject.FindProperty("OnCancel");
            propertyOnPause = serializedObject.FindProperty("OnPause");
            propertyOnResume = serializedObject.FindProperty("OnResume");
            propertyOnReset = serializedObject.FindProperty("OnReset");
            propertyOnUpdate = serializedObject.FindProperty("OnUpdate");
            propertyOnStartBehaviour = serializedObject.FindProperty("OnStartBehaviour");
            propertyOnEnableBehaviour = serializedObject.FindProperty("OnEnableBehaviour");
            propertyOnDisableBehaviour = serializedObject.FindProperty("OnDisableBehaviour");
            propertyOnDestroyBehaviour = serializedObject.FindProperty("OnDestroyBehaviour");
        }

        protected virtual void InitializeEditor()
        {
            FindProperties();
            root = DesignUtils.GetEditorRoot();
            componentHeader = DesignUtils.editorComponentHeader.SetAccentColor(accentColor);
            toolbarContainer = DesignUtils.editorToolbarContainer;
            tabsGroup = FluidToggleGroup.Get().SetControlMode(FluidToggleGroup.ControlMode.OneToggleOn);
            contentContainer = DesignUtils.editorContentContainer;
            InitializeSettings();
            InitializeLabels();
            InitializeCallbacks();

            root.schedule.Execute(() => settingsTab.ButtonSetIsOn(true, false));

            //refresh tabs enabled indicator
            root.schedule.Execute(() =>
            {
                void UpdateIndicator(FluidTab tab, bool toggleOn, bool animateChange)
                {
                    if (tab == null) return;
                    if (tab.indicator.isOn != toggleOn)
                        tab.indicator.Toggle(toggleOn, animateChange);
                }

                //initial indicators state update (no animation)
                UpdateIndicator(callbacksTab, castedDateTimeComponent.hasCallbacks, false);

                //subsequent indicators state update (animated)
                root.schedule.Execute(() =>
                {
                    UpdateIndicator(callbacksTab, castedDateTimeComponent.hasCallbacks, true);

                }).Every(200);

            });
        }

        protected virtual void InitializeSettings()
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
                settingsAnimatedContainer
                    .AddContent(GetTimescaleModeAndUpdateInterval())
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(GetBehaviours())
                    .AddContent(DesignUtils.endOfLineBlock)
                    .Bind(serializedObject);
            });
        }

        protected virtual void InitializeLabels()
        {
            labelsAnimatedContainer = new FluidAnimatedContainer("Labels", true).Hide(false);
            labelsTab =
                new FluidTab()
                    .SetLabelText("Labels")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Label)
                    .SetElementSize(ElementSize.Small)
                    .IndicatorSetEnabledColor(accentColor)
                    .ButtonSetAccentColor(selectableAccentColor)
                    .ButtonSetOnValueChanged(evt => labelsAnimatedContainer.Toggle(evt.newValue, evt.animateChange))
                    .AddToToggleGroup(tabsGroup);

            labelsAnimatedContainer.SetOnShowCallback(() =>
            {
                FluidListView listView =
                    DesignUtils.NewPropertyListView(propertyLabels, "", "")
                        .ShowItemIndex(false)
                        .HideFooter(true);

                labelsAnimatedContainer
                    .AddContent(listView)
                    .AddContent(DesignUtils.endOfLineBlock)
                    .Bind(serializedObject);
            });
        }

        protected VisualElement GetTimescaleModeAndUpdateInterval()
        {
            EnumField timeScaleModeEnumField =
                DesignUtils.NewEnumField(propertyTimescaleMode)
                    .SetTooltip
                    (
                        "The timescale mode to use when updating the time\n\n" +
                        "Independent\n" +
                        "(Realtime) Not affected by the application's timescale value" +
                        "\n\n" +
                        "Dependent\n" +
                        "(Application Time) Affected by the application's timescale value"
                    )
                    .SetStyleFlexGrow(1);

            FloatField updateIntervalFloatField =
                DesignUtils.NewFloatField(propertyUpdateInterval)
                    .SetTooltip("The interval in seconds between each update")
                    .SetStyleFlexGrow(1);

            return
                DesignUtils.row
                    .SetName("Timescale Mode and Update Interval")
                    .AddChild(FluidField.Get("Timescale Mode").AddFieldContent(timeScaleModeEnumField))
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(FluidField.Get("Update Interval").AddFieldContent(updateIntervalFloatField));
        }

        protected static FluidField GetCallbackField(SerializedProperty property) =>
            FluidField.Get()
                .SetElementSize(ElementSize.Large)
                .AddFieldContent(DesignUtils.NewPropertyField(property));

        protected virtual void InitializeCallbacks()
        {
            callbacksAnimatedContainer = new FluidAnimatedContainer("Callbacks", true).Hide(false);
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
                callbacksAnimatedContainer
                    .AddContent(GetCallbackField(propertyOnStart))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(GetCallbackField(propertyOnStop))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(GetCallbackField(propertyOnFinish))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(GetCallbackField(propertyOnCancel))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(GetCallbackField(propertyOnReset))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(GetCallbackField(propertyOnUpdate))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(GetCallbackField(propertyOnPause))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(GetCallbackField(propertyOnResume))
                    .Bind(serializedObject);
            });
        }

        protected virtual VisualElement Toolbar()
        {
            return
                toolbarContainer
                    .AddChild(settingsTab)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(labelsTab)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(callbacksTab)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(DesignUtils.flexibleSpace)
                    .AddChild(DesignUtils.spaceBlock2X);
        }

        protected virtual VisualElement Content()
        {
            return
                contentContainer
                    .AddChild(settingsAnimatedContainer)
                    .AddChild(labelsAnimatedContainer)
                    .AddChild(callbacksAnimatedContainer)
                ;
        }

        protected virtual void Compose()
        {
            root
                .AddChild(GetControlButtons())
                .AddChild(componentHeader)
                .AddChild(Toolbar())
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(Content())
                .AddChild(DesignUtils.endOfLineBlock);
        }

        protected VisualElement GetBehaviours()
        {
            return DesignUtils.row
                .AddChild(FluidField.Get<EnumField>(propertyOnStartBehaviour, "On Start Behaviour"))
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(FluidField.Get<EnumField>(propertyOnEnableBehaviour, "OnEnable Behaviour"))
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(FluidField.Get<EnumField>(propertyOnDisableBehaviour, "OnDisable Behaviour"))
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(FluidField.Get<EnumField>(propertyOnDestroyBehaviour, "OnDestroy Behaviour"));
            ;
        }

        protected static FluidButton FormatButton(string labelText, string url = "") =>
            FluidButton.Get()
                .SetElementSize(ElementSize.Small)
                .SetButtonStyle(ButtonStyle.Contained)
                .SetLabelText(labelText)
                .SetIcon(EditorSpriteSheets.EditorUI.Icons.Popout)
                .SetTooltip(url)
                .SetOnClick(() => Application.OpenURL(url));

        protected static IntegerField GetIntegerField(SerializedProperty property) =>
            DesignUtils.NewIntegerField(property)
                .SetStyleFlexGrow(1);
    }
}
