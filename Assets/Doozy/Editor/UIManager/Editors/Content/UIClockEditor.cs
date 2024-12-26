// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Doozy.Editor.Common.ScriptableObjects;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIManager.Editors.Content.Internal;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Content;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Editors.Content
{
    [CustomEditor(typeof(UIClock), true)]
    [CanEditMultipleObjects]
    public class UIClockEditor : BaseDateTimeContentEditor
    {
        private UIClock castedTarget => (UIClock)target;
        private List<UIClock> castedTargets => targets.Cast<UIClock>().ToList();

        private FluidField infoFluidField { get; set; }
        private Label infoLabel { get; set; }

        private SerializedProperty propertyAvailableTimeZones { get; set; }
        private SerializedProperty propertyTimeZoneId { get; set; }

        private FluidButton timeZoneSelectorButton { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            infoFluidField?.Recycle();
        }

        protected override void FindProperties()
        {
            base.FindProperties();
            propertyAvailableTimeZones = serializedObject.FindProperty("AvailableTimeZones");
            propertyTimeZoneId = serializedObject.FindProperty("TimeZoneId");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText("UI Clock")
                .SetIcon(EditorSpriteSheets.UIManager.Icons.UIClock)
                .AddManualButton()
                .AddApiButton()
                .AddYouTubeButton();

            infoLabel =
                DesignUtils.NewLabel("Clock is not running", 14)
                    .SetStyleColor(DesignUtils.disabledTextColor)
                    .SetStyleFlexGrow(1);

            infoFluidField =
                FluidField.Get()
                    .SetIcon(EditorSpriteSheets.UIManager.Icons.UIClock)
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddChild(infoLabel)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(DesignUtils.flexibleSpace)
                    );

            root.schedule.Execute(() =>
            {
                if (!castedTarget.isRunning)
                {
                    infoLabel.text = "Clock is not running";
                    infoLabel.SetStyleColor(DesignUtils.disabledTextColor);
                    return;
                }
                infoLabel.text = castedTarget.currentTime.ToString(CultureInfo.InvariantCulture);
                infoLabel.SetStyleColor(DesignUtils.fieldNameTextColor);
            }).Every(100);

            root.schedule.Execute(() =>
            {
                if (!castedTarget.isRunning)
                    return;
                infoFluidField?.iconReaction.Play();
            }).Every(1000);
        }

        private List<KeyValuePair<string, UnityAction>> GetTimeZoneSelectorMenuItems()
        {
            List<KeyValuePair<string, UnityAction>> menuItems = new List<KeyValuePair<string, UnityAction>>();

            var systemTimeZoneIds = TimeZoneInfo.GetSystemTimeZones().Select(info => info.Id).ToList();
            foreach (string timeZoneId in systemTimeZoneIds)
            {
                string timeZoneNameWithUtcOffset = GetTimeZoneNameWithUtcOffset(timeZoneId);
                menuItems.Add(new KeyValuePair<string, UnityAction>(timeZoneNameWithUtcOffset, () =>
                {
                    string id = timeZoneId;
                    castedTargets.ForEach(t => t.SetTimeZone(id));
                    serializedObject.Update();
                    timeZoneSelectorButton.SetLabelText(timeZoneNameWithUtcOffset);
                }));
            }
            return menuItems;
        }

        private static string GetTimeZoneNameWithUtcOffset(string timeZoneId)
        {
            int timezoneUtcOffset = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId).BaseUtcOffset.Hours;
            string utcOffset = $"UTC {(timezoneUtcOffset >= 0 ? "+" : "")}{timezoneUtcOffset:00}";
            return $"({utcOffset}) {timeZoneId}";
        }

        protected override void InitializeSettings()
        {
            base.InitializeSettings();

            timeZoneSelectorButton =
                FluidButton.Get()
                    .SetLabelText(GetTimeZoneNameWithUtcOffset(castedTarget.timeZoneId))
                    .SetElementSize(ElementSize.Normal)
                    .SetButtonStyle(ButtonStyle.Contained)
                    .SetStyleFlexGrow(1)
                    .SetOnClick(() =>
                    {
                        var searchWindowContext = new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition));

                        DynamicSearchProvider dsp =
                            CreateInstance<DynamicSearchProvider>()
                                .AddItems(GetTimeZoneSelectorMenuItems());
                        SearchWindow.Open(searchWindowContext, dsp);
                    });
            
            root.schedule.Execute(() =>
            {
                timeZoneSelectorButton.SetLabelText(GetTimeZoneNameWithUtcOffset(castedTarget.timeZoneId));
            }).Every(1000);
            
            settingsAnimatedContainer.SetOnShowCallback(() =>
            {
                IntegerField yearsField = GetIntegerField(propertyYears).DisableElement();
                IntegerField monthsField = GetIntegerField(propertyMonths).DisableElement();
                IntegerField daysField = GetIntegerField(propertyDays).DisableElement();
                IntegerField hoursField = GetIntegerField(propertyHours).DisableElement();
                IntegerField minutesField = GetIntegerField(propertyMinutes).DisableElement();
                IntegerField secondsField = GetIntegerField(propertySeconds).DisableElement();
                IntegerField millisecondsField = GetIntegerField(propertyMilliseconds).DisableElement();

                if (propertyTimeZoneId.stringValue.IsNullOrEmpty())
                {
                    propertyTimeZoneId.stringValue = TimeZoneInfo.Local.Id;
                    serializedObject.ApplyModifiedProperties();
                }

                settingsAnimatedContainer
                    .AddContent(GetTimescaleModeAndUpdateInterval())
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent
                    (
                        FluidField.Get("TimeZone")
                            .AddFieldContent(timeZoneSelectorButton)
                    )
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent
                    (
                        FluidField.Get("Current Time")
                            .SetElementSize(ElementSize.Large)
                            .AddFieldContent
                            (
                                DesignUtils.column
                                    .AddChild
                                    (
                                        DesignUtils.row
                                            .AddChild(FluidField.Get("Year").AddFieldContent(yearsField))
                                            .AddChild(DesignUtils.spaceBlock)
                                            .AddChild(FluidField.Get("Month").AddFieldContent(monthsField))
                                            .AddChild(DesignUtils.spaceBlock)
                                            .AddChild(FluidField.Get("").AddFieldContent(daysField))
                                    )
                                    .AddChild(DesignUtils.spaceBlock)
                                    .AddChild
                                    (
                                        DesignUtils.row
                                            .AddChild(FluidField.Get("Hour").AddFieldContent(hoursField))
                                            .AddChild(DesignUtils.spaceBlock)
                                            .AddChild(FluidField.Get("Minute").AddFieldContent(minutesField))
                                            .AddChild(DesignUtils.spaceBlock)
                                            .AddChild(FluidField.Get("Second").AddFieldContent(secondsField))
                                            .AddChild(DesignUtils.spaceBlock)
                                            .AddChild(FluidField.Get("Millisecond").AddFieldContent(millisecondsField))
                                    )
                            )
                    )
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(GetBehaviours())
                    .AddContent(DesignUtils.endOfLineBlock)
                    .Bind(serializedObject);
            });
        }

        protected override void InitializeLabels()
        {
            base.InitializeLabels();

            labelsAnimatedContainer.SetOnShowCallback(() =>
            {
                FluidListView listView =
                    DesignUtils.NewPropertyListView(propertyLabels, "", "")
                        .ShowItemIndex(false)
                        .HideFooter(true);

                FluidButton standardFormatButton =
                    FormatButton
                    (
                        "Standard DateTime Format",
                        "https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings"
                    );

                FluidButton customFormatButton =
                    FormatButton
                    (
                        "Custom DateTime Format",
                        "https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings"
                    );

                labelsAnimatedContainer
                    .AddContent(listView)
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent
                    (
                        FluidField.Get()
                            .SetIcon(EditorSpriteSheets.EditorUI.Icons.DotNet)
                            .AddFieldContent
                            (
                                DesignUtils.row
                                    .AddChild(standardFormatButton)
                                    .AddChild(DesignUtils.spaceBlock)
                                    .AddChild(customFormatButton)
                            )
                    )
                    .AddContent(DesignUtils.endOfLineBlock)
                    .Bind(serializedObject);
            });
        }

        protected override void Compose()
        {
            root
                .AddChild(GetControlButtons())
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild
                (
                    DesignUtils.row
                        .AddSpace(44, 0)
                        .AddChild(infoFluidField)
                )
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(componentHeader)
                .AddChild(Toolbar())
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(Content())
                .AddChild(DesignUtils.endOfLineBlock);
        }
    }
}
