// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIManager.Editors.Content.Internal;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Content;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Editors.Content
{
    [CustomEditor(typeof(UIStopwatch), true)]
    [CanEditMultipleObjects]
    public class UIStopwatchEditor : BaseDateTimeContentEditor
    {
        private class LapElement : VisualElement
        {
            // ReSharper disable once MemberCanBePrivate.Local
            public UIStopwatch.Lap lap { get; }

            private Label lapNumberLabel { get; }
            private Label lapTimeLabel { get; }
            private Label lapDurationLabel { get; }



            public LapElement(UIStopwatch.Lap lap)
            {
                this.lap = lap;

                lapNumberLabel =
                    DesignUtils.NewLabel()
                        .SetStyleWidth(20)
                        .SetStyleTextAlign(TextAnchor.MiddleRight);

                lapTimeLabel =
                    DesignUtils.NewLabel()
                        .SetStyleTextAlign(TextAnchor.MiddleRight)
                        .SetStyleMinWidth(120)
                        .SetStyleFlexGrow(1);

                lapDurationLabel =
                    DesignUtils.NewLabel()
                        .SetStyleTextAlign(TextAnchor.MiddleRight)
                        .SetStyleMinWidth(120)
                        .SetStyleFlexGrow(1);

                this
                    .AddChild
                    (
                        DesignUtils.column
                            .AddChild
                            (
                                DesignUtils.row
                                    .AddChild(lapNumberLabel)
                                    .AddChild(DesignUtils.spaceBlock)
                                    .AddChild(lapDurationLabel)
                                    .AddChild(DesignUtils.spaceBlock)
                                    .AddChild(lapTimeLabel)
                                    .AddChild(DesignUtils.spaceBlock2X)
                            )
                            .AddChild(DesignUtils.dividerHorizontal)
                    )
                    .SetStylePaddingTop(DesignUtils.k_Spacing / 2f)
                    ;

                Refresh();
            }

            // ReSharper disable once MemberCanBePrivate.Local
            public void Refresh()
            {
                lapNumberLabel.text = lap.lapNumber.ToString();

                string timeFormatString;
                if (lap.lapTime.Hours > 0)
                {
                    timeFormatString = @"hh\:mm\:ss\.fff";
                }
                else if (lap.lapTime.Minutes > 0)
                {
                    timeFormatString = @"mm\:ss\.fff";
                }
                else
                {
                    timeFormatString = @"ss\.fff";
                }

                lapTimeLabel.text = lap.lapTime.ToString(timeFormatString);

                string durationFormatString;
                if (lap.lapDuration.Hours > 0)
                {
                    durationFormatString = @"hh\:mm\:ss\.fff";
                }
                else if (lap.lapDuration.Minutes > 0)
                {
                    durationFormatString = @"mm\:ss\.fff";
                }
                else
                {
                    durationFormatString = @"ss\.fff";
                }

                lapDurationLabel.text = lap.lapDuration.ToString(durationFormatString);
            }
        }

        private UIStopwatch castedTarget => (UIStopwatch)target;
        private IEnumerable<UIStopwatch> castedTargets => targets.Cast<UIStopwatch>();

        private FluidField infoFluidField { get; set; }
        private Label infoLabel { get; set; }
        private FluidButton addLapButton { get; set; }

        private SerializedProperty propertyOnLap { get; set; }

        private FluidField lapsFluidField { get; set; }
        private VisualElement lapListContainer { get; set; }
        private List<LapElement> lapElements { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            infoFluidField?.Recycle();
            addLapButton?.Recycle();
        }

        protected override void FindProperties()
        {
            base.FindProperties();
            propertyOnLap = serializedObject.FindProperty("OnLap");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText("UI Stopwatch")
                .SetIcon(EditorSpriteSheets.UIManager.Icons.UIStopwatch)
                .AddManualButton()
                .AddApiButton()
                .AddYouTubeButton();

            infoLabel =
                DesignUtils.NewLabel("Stopwatch is not running", 14)
                    .SetStyleColor(DesignUtils.disabledTextColor)
                    .SetStyleFlexGrow(1);

            infoFluidField =
                FluidField.Get()
                    .SetIcon(EditorSpriteSheets.UIManager.Icons.UIStopwatch)
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
                    infoLabel.text = "Stopwatch is not running";
                    infoLabel.SetStyleColor(DesignUtils.disabledTextColor);
                    return;
                }
                infoLabel.text = castedTarget.elapsedTime.ToString(@"hh\:mm\:ss\.fff");
                infoLabel.SetStyleColor(DesignUtils.fieldNameTextColor);
            }).Every(100);

            root.schedule.Execute(() =>
            {
                if (!castedTarget.isRunning)
                    return;
                infoFluidField?.iconReaction.Play();
            }).Every(1000);

            addLapButton =
                FluidButton.Get()
                    .SetElementSize(ElementSize.Tiny)
                    .SetButtonStyle(ButtonStyle.Contained)
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Plus)
                    .SetLabelText("Add Lap")
                    .SetTooltip("Add a new lap to the stopwatch")
                    .SetOnClick(() =>
                    {
                        castedTarget.AddLap();
                        UIStopwatch.Lap lastLap = castedTarget.laps.Last();
                        AddLapElement(lastLap);
                    });

            lapListContainer =
                new VisualElement();

            lapsFluidField =
                FluidField.Get()
                    .AddFieldContent
                    (
                        DesignUtils.column
                            .AddChild
                            (
                                DesignUtils.row
                                    .SetStyleMarginLeft(DesignUtils.k_Spacing)
                                    .AddChild
                                    (
                                        DesignUtils.fieldLabel
                                            .SetText("Laps")
                                            .SetStyleFontSize(14)
                                            .SetStyleAlignSelf(Align.FlexEnd)
                                    )
                                    .AddChild(DesignUtils.flexibleSpace)
                                    .AddChild(addLapButton)
                            )
                            .AddChild(DesignUtils.dividerHorizontal)
                            .AddChild(lapListContainer)
                    );

            lapElements ??= new List<LapElement>();

            root.schedule.Execute(() =>
            {
                addLapButton.SetEnabled(EditorApplication.isPlayingOrWillChangePlaymode && castedTarget.isRunning);

                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                    return;

                if (!castedTarget.isRunning)
                {
                    if (castedTarget.laps.Count > 0) return;
                    if (lapElements.Count <= 0) return;
                    lapElements.Clear();
                    lapListContainer.Clear();
                    return;
                }

                if (lapElements.Count == castedTarget.laps.Count)
                    return;

                if (lapElements.Count > castedTarget.laps.Count)
                {
                    lapElements.Clear();
                    lapListContainer.Clear();

                    foreach (UIStopwatch.Lap lap in castedTarget.laps)
                    {
                        AddLapElement(lap);
                    }
                    return;
                }

                if (castedTarget.laps.Count == 0)
                    return;

                //a new lap was added
                UIStopwatch.Lap lastLap = castedTarget.laps.Last();
                AddLapElement(lastLap);

            }).Every(200);
        }

        private void AddLapElement(UIStopwatch.Lap lap)
        {
            lapElements.Add(new LapElement(lap));

            lapListContainer
                .AddChild(lapElements.Last());
        }

        protected override void InitializeSettings()
        {
            base.InitializeSettings();

            settingsAnimatedContainer.SetOnShowCallback(() =>
            {
                IntegerField yearsField = GetIntegerField(propertyYears).DisableElement();
                IntegerField monthsField = GetIntegerField(propertyMonths).DisableElement();
                IntegerField daysField = GetIntegerField(propertyDays).DisableElement();
                IntegerField hoursField = GetIntegerField(propertyHours).DisableElement();
                IntegerField minutesField = GetIntegerField(propertyMinutes).DisableElement();
                IntegerField secondsField = GetIntegerField(propertySeconds).DisableElement();
                IntegerField millisecondsField = GetIntegerField(propertyMilliseconds).DisableElement();

                settingsAnimatedContainer
                    .AddContent(GetTimescaleModeAndUpdateInterval())
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent
                    (
                        FluidField.Get("Stopwatch Elapsed Time")
                            .SetElementSize(ElementSize.Large)
                            .AddFieldContent
                            (
                                DesignUtils.column
                                    .AddChild
                                    (
                                        DesignUtils.row
                                            .AddChild(FluidField.Get("Years").AddFieldContent(yearsField))
                                            .AddChild(DesignUtils.spaceBlock)
                                            .AddChild(FluidField.Get("Months").AddFieldContent(monthsField))
                                            .AddChild(DesignUtils.spaceBlock)
                                            .AddChild(FluidField.Get("Days").AddFieldContent(daysField))
                                    )
                                    .AddChild(DesignUtils.spaceBlock)
                                    .AddChild
                                    (
                                        DesignUtils.row
                                            .AddChild(FluidField.Get("Hours").AddFieldContent(hoursField))
                                            .AddChild(DesignUtils.spaceBlock)
                                            .AddChild(FluidField.Get("Minutes").AddFieldContent(minutesField))
                                            .AddChild(DesignUtils.spaceBlock)
                                            .AddChild(FluidField.Get("Seconds").AddFieldContent(secondsField))
                                            .AddChild(DesignUtils.spaceBlock)
                                            .AddChild(FluidField.Get("Milliseconds").AddFieldContent(millisecondsField))
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
                        "Standard TimeSpan Format",
                        "https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-timespan-format-strings"
                    );

                FluidButton customFormatButton =
                    FormatButton
                    (
                        "Custom TimeSpan Format",
                        "https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-timespan-format-strings"
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



        protected override void InitializeCallbacks()
        {
            base.InitializeCallbacks();

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
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(GetCallbackField(propertyOnLap))
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
                        .AddChild
                        (
                            DesignUtils.column
                                .AddChild(infoFluidField)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(lapsFluidField)
                        )
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
