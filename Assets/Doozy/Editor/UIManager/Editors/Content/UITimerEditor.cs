// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIManager.Editors.Content.Internal;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Content;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Editors.Content
{
    [CustomEditor(typeof(UITimer), true)]
    [CanEditMultipleObjects]
    public class UITimerEditor : BaseDateTimeContentEditor
    {
        private UITimer castedTarget => (UITimer)target;
        private IEnumerable<UITimer> castedTargets => targets.Cast<UITimer>();

        private FluidField infoFluidField { get; set; }
        private Label infoLabel { get; set; }

        private SerializedProperty propertyOnProgressChanged { get; set; }
        private SerializedProperty propertyTargetProgressor { get; set; }
        private SerializedProperty propertyInstantProgressorUpdate { get; set; }
        private SerializedProperty propertyOnLoop { get; set; }
        private SerializedProperty propertyLoops { get; set; }
        private SerializedProperty propertyLoopDelay { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            infoFluidField?.Recycle();
        }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyOnProgressChanged = serializedObject.FindProperty("OnProgressChanged");
            propertyTargetProgressor = serializedObject.FindProperty("TargetProgressor");
            propertyInstantProgressorUpdate = serializedObject.FindProperty("InstantProgressorUpdate");
            propertyOnLoop = serializedObject.FindProperty("OnLoop");
            propertyLoops = serializedObject.FindProperty("Loops");
            propertyLoopDelay = serializedObject.FindProperty("LoopDelay");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText("UI Timer")
                .SetIcon(EditorSpriteSheets.UIManager.Icons.UITimer)
                .AddManualButton()
                .AddApiButton()
                .AddYouTubeButton();

            infoLabel =
                DesignUtils.NewLabel("Timer is not running", 14)
                    .SetStyleColor(DesignUtils.disabledTextColor)
                    .SetStyleFlexGrow(1);

            infoFluidField =
                FluidField.Get()
                    .SetIcon(EditorSpriteSheets.UIManager.Icons.UITimer)
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
                    infoLabel.text = "Timer is not running";
                    infoLabel.SetStyleColor(DesignUtils.disabledTextColor);
                    return;
                }
                infoLabel.text = castedTarget.remainingTime.ToString("G");
                infoLabel.SetStyleColor(DesignUtils.fieldNameTextColor);
            }).Every(100);

            root.schedule.Execute(() =>
            {
                if (!castedTarget.isRunning)
                    return;
                infoFluidField?.iconReaction.Play();
            }).Every(1000);
        }

        protected override void InitializeSettings()
        {
            base.InitializeSettings();

            settingsAnimatedContainer.SetOnShowCallback(() =>
            {
                IntegerField yearsField = GetIntegerField(propertyYears);
                IntegerField monthsField = GetIntegerField(propertyMonths);
                IntegerField daysField = GetIntegerField(propertyDays);
                IntegerField hoursField = GetIntegerField(propertyHours);
                IntegerField minutesField = GetIntegerField(propertyMinutes);
                IntegerField secondsField = GetIntegerField(propertySeconds);
                IntegerField millisecondsField = GetIntegerField(propertyMilliseconds);

                IntegerField loopsField = GetIntegerField(propertyLoops);

                FluidField loopsFluidField =
                    FluidField.Get()
                        .SetLabelText("Loops")
                        .SetTooltip
                        (
                            "Number of times the timer restarts after it reaches the target time before it stops and completes" +
                            "(-1 means infinite loops)"
                        )
                        .AddFieldContent(loopsField);

                FloatField loopDelayField = DesignUtils.NewFloatField(propertyLoopDelay).SetStyleFlexGrow(1);


                FluidField loopDelayFluidField =
                    FluidField.Get()
                        .SetLabelText("Loop Delay (seconds)")
                        .SetTooltip("Time delay between loops (in seconds)")
                        .AddFieldContent(loopDelayField);


                ObjectField targetProgressorObjectField =
                    DesignUtils.NewObjectField(propertyTargetProgressor, typeof(Progressor))
                        .SetTooltip
                        (
                            "Reference to a Progressor that will be updated when the timer is updated."
                        )
                        .SetStyleFlexGrow(1);

                FluidToggleCheckbox instantProgressorUpdateToggleCheckbox =
                    FluidToggleCheckbox.Get("Instant Progressor Update")
                        .SetTooltip
                        (
                            "When true, the timer will update the target progressor value with SetProgressAt instead of PlayToProgress.\n\n" +
                            "Basically, if true, the progressor will not animate to the new value when the timer is updated."
                        )
                        .SetToggleAccentColor(selectableAccentColor)
                        .BindToProperty(propertyInstantProgressorUpdate)
                        .SetStyleAlignSelf(Align.FlexEnd);

                FluidField progressorFluidField =
                    FluidField.Get()
                        .AddFieldContent
                        (
                            DesignUtils.row
                                .AddChild(FluidField.Get("Target Progressor").AddFieldContent(targetProgressorObjectField))
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild(instantProgressorUpdateToggleCheckbox)
                        );

                settingsAnimatedContainer
                    .AddContent(GetTimescaleModeAndUpdateInterval())
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent
                    (
                        FluidField.Get("Countdown Duration")
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
                    .AddContent
                    (
                        DesignUtils.row
                            .AddChild(loopsFluidField)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(loopDelayFluidField)
                    )
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(GetBehaviours())
                    .AddContent(DesignUtils.spaceBlock4X)
                    .AddContent(progressorFluidField)
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
                    .AddContent(GetCallbackField(propertyOnLoop))
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
