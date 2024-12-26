// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Mody.Components;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Mody
{
    public class ModyTriggerEditor<T> : EditorUIEditor<T> where T : SignalProvider
    {
        protected override Color accentColor => EditorColors.Mody.Trigger;
        protected override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Mody.Trigger;

        protected FluidTab settingsTab { get; set; }
        protected FluidTab callbacksTab { get; set; }

        protected FluidAnimatedContainer settingsAnimatedContainer { get; set; }
        protected FluidAnimatedContainer callbacksAnimatedContainer { get; set; }

        private ModyProviderStateIndicator m_StateIndicator;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            m_StateIndicator?.Dispose();
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetIcon(EditorSpriteSheets.Mody.Icons.ModyTrigger)
                .SetElementSize(ElementSize.Small)
                .SetComponentNameText($"{castedTarget.attributes.id.Category}.{castedTarget.attributes.id.Name}")
                .SetComponentTypeText($"{castedTarget.attributes.id.Type} Trigger");

            toolbarContainer
                .SetStyleMarginLeft(42)
                .SetStyleMarginTop(-2);
            
            m_StateIndicator = new ModyProviderStateIndicator().SetStyleMarginLeft(DesignUtils.k_Spacing2X);
            m_StateIndicator.UpdateState(castedTarget, castedTarget.currentState);
            castedTarget.onStateChanged = state => m_StateIndicator.UpdateState(castedTarget, state);

            if (EditorApplication.isPlayingOrWillChangePlaymode)
                componentHeader.AddElement(m_StateIndicator);

            InitializeSettings();
            InitializeCallbacks();

            // root.schedule.Execute(() => settingsTab.ButtonSetIsOn(true, false));
        }

        protected FluidTab GetTab(string labelText) =>
            new FluidTab()
                .SetElementSize(ElementSize.Small)
                .SetLabelText(labelText)
                .IndicatorSetEnabledColor(accentColor)
                .ButtonSetAccentColor(selectableAccentColor)
                .AddToToggleGroup(tabsGroup);


        protected virtual void InitializeSettings()
        {
            settingsAnimatedContainer = new FluidAnimatedContainer("Settings", true).Hide(false);
            settingsTab =
                GetTab("Settings")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Settings)
                    .ButtonSetOnValueChanged(evt => settingsAnimatedContainer.Toggle(evt.newValue, evt.animateChange));

            settingsAnimatedContainer.SetOnShowCallback(() =>
            {
                settingsAnimatedContainer
                    .AddContent
                    (
                        DesignUtils.row
                            .AddChild(GetCooldownFluidField(serializedObject))
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(GetTimescaleFluidField(serializedObject))
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(DesignUtils.flexibleSpace)
                    )
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
        }

        protected override VisualElement Toolbar()
        {
            return 
                base.Toolbar()
                .AddChild(settingsTab)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(callbacksTab)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(DesignUtils.flexibleSpace);
        }

        protected override VisualElement Content()
        {
            return base.Content()
                .AddChild(settingsAnimatedContainer)
                .AddChild(callbacksAnimatedContainer);
        }

        protected override void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild(Toolbar())
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(Content())
                .AddChild(DesignUtils.endOfLineBlock);

        }

        protected static FluidField GetCooldownFluidField(SerializedObject serializedObject) =>
            TimeField
            (
                serializedObject,
                "SignalCooldown",
                ModyProviderStateIndicator.cooldownColor,
                "Cooldown",
                "Cooldown\n\nCooldown time after a signal was sent. During this time, no Signal will be sent",
                EditorSpriteSheets.EditorUI.Icons.Cooldown
            );

        protected static FluidField GetTimescaleFluidField(SerializedObject serializedObject)
        {
            SerializedProperty targetProperty = serializedObject.FindProperty("SignalTimescale");
            EnumField enumField =
                new EnumField()
                    .ResetLayout()
                    .BindToProperty(targetProperty)
                    .SetStyleMinWidth(94);

            FluidField field =
                FluidField.Get()
                    // .SetLabelText("Timescale")
                    .SetTooltip
                    (
                        "Timescale" +
                        "\n\nDetermine if the Signal's timers will be affected by the application's timescale" +
                        "\n\nTimescale.Independent - (Realtime)\nNot affected by the application's timescale value" +
                        "\n\nTimescale.Dependent - (Application Time)\nAffected by the application's timescale value"
                    )
                    .SetElementSize(ElementSize.Small)
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.TimeScale)
                    .SetStyleMaxWidth(160)
                    .AddFieldContent(enumField);
            return field;
        }

        protected static FluidField TimeField(SerializedObject serializedObject, string targetPropertyName, Color enabledColor, string fieldLabelText, string fieldTooltip, IEnumerable<Texture2D> fieldIconTextures)
        {
            SerializedProperty targetProperty = serializedObject.FindProperty(targetPropertyName);
            EnabledIndicator indicator = EnabledIndicator.Get().SetIcon(fieldIconTextures).SetEnabledColor(enabledColor).SetSize(18);
            FloatField floatField = new FloatField().ResetLayout().BindToProperty(targetProperty).SetStyleFlexGrow(1);
            floatField.RegisterValueChangedCallback(evt => indicator.Toggle(evt.newValue > 0));
            indicator.Toggle(targetProperty.floatValue > 0, false);

            return FluidField.Get()
                // .SetLabelText(fieldLabelText)
                .SetTooltip(fieldTooltip)
                .SetElementSize(ElementSize.Tiny)
                .SetStyleMaxWidth(120)
                .AddFieldContent
                (
                    DesignUtils.row.SetStyleFlexGrow(0)
                        .AddChild(indicator.SetStyleMarginRight(DesignUtils.k_Spacing))
                        .AddChild(floatField)
                );
        }
    }
}
