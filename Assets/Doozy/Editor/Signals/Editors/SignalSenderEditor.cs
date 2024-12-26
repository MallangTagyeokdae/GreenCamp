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
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ObjectNames = Doozy.Runtime.Common.Utils.ObjectNames;

namespace Doozy.Editor.Signals.Editors
{
    [CustomEditor(typeof(SignalSender), true)]
    public class SignalSenderEditor : UnityEditor.Editor
    {
        private SignalSender castedTarget => (SignalSender)target;
        private IEnumerable<SignalSender> castedTargets => targets.Cast<SignalSender>();

        private static Color accentColor => EditorColors.Signals.Signal;
        private static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Signals.Signal;
        private static IEnumerable<Texture2D> signalSenderTextures => EditorSpriteSheets.Signals.Icons.SignalSender;

        private VisualElement root { get; set; }
        private FluidComponentHeader componentHeader { get; set; }

        private FluidField payloadFluidField { get; set; }
        private FluidField autoSendFluidField { get; set; }

        private PropertyField payloadPropertyField { get; set; }
        private FluidToggleSwitch sendOnStartSwitch { get; set; }
        private FluidToggleSwitch sendOnEnableSwitch { get; set; }
        private FluidToggleSwitch sendOnDisableSwitch { get; set; }
        private FluidToggleSwitch sendOnDestroySwitch { get; set; }

        private SerializedProperty propertyPayload { get; set; }
        private SerializedProperty propertySendOnStart { get; set; }
        private SerializedProperty propertySendOnEnable { get; set; }
        private SerializedProperty propertySendOnDisable { get; set; }
        private SerializedProperty propertySendOnDestroy { get; set; }

        public override VisualElement CreateInspectorGUI()
        {
            FindProperties();
            InitializeEditor();
            Compose();
            return root;
        }

        private void OnDestroy()
        {
            componentHeader?.Recycle();
        }

        private void FindProperties()
        {
            propertyPayload = serializedObject.FindProperty(nameof(SignalSender.Payload));
            propertySendOnStart = serializedObject.FindProperty(nameof(SignalSender.SendOnStart));
            propertySendOnEnable = serializedObject.FindProperty(nameof(SignalSender.SendOnEnable));
            propertySendOnDisable = serializedObject.FindProperty(nameof(SignalSender.SendOnDisable));
            propertySendOnDestroy = serializedObject.FindProperty(nameof(SignalSender.SendOnDestroy));
        }

        private void InitializeEditor()
        {
            root = 
                DesignUtils.editorRoot;
            
            componentHeader =
                DesignUtils.editorComponentHeader
                    .SetAccentColor(accentColor)
                    .SetIcon(signalSenderTextures.ToList())
                    .SetComponentTypeText(ObjectNames.NicifyVariableName(nameof(SignalSender)));

            payloadPropertyField =
                DesignUtils.NewPropertyField(propertyPayload);

            payloadFluidField =
                FluidField.Get()
                    .AddFieldContent(payloadPropertyField);

            sendOnStartSwitch =
                FluidToggleSwitch.Get("OnStart")
                    .SetTooltip("Automatically send a signal on Start")
                    .SetToggleAccentColor(selectableAccentColor)
                    .BindToProperty(propertySendOnStart);

            sendOnEnableSwitch =
                FluidToggleSwitch.Get("OnEnable")
                    .SetTooltip("Automatically send a signal on OnEnable")
                    .SetToggleAccentColor(selectableAccentColor)
                    .BindToProperty(propertySendOnEnable);

            sendOnDisableSwitch =
                FluidToggleSwitch.Get("OnDisable")
                    .SetTooltip("Automatically send a signal on OnDisable")
                    .SetToggleAccentColor(selectableAccentColor)
                    .BindToProperty(propertySendOnDisable);

            sendOnDestroySwitch =
                FluidToggleSwitch.Get("OnDestroy")
                    .SetTooltip("Automatically send a signal on OnDestroy")
                    .SetToggleAccentColor(selectableAccentColor)
                    .BindToProperty(propertySendOnDestroy);

            autoSendFluidField =
                FluidField.Get("Auto Send Signal")
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddChild(sendOnStartSwitch)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(sendOnEnableSwitch)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(sendOnDisableSwitch)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(sendOnDestroySwitch)
                    );
        }

        private void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(payloadFluidField)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(autoSendFluidField)
                .AddChild(DesignUtils.endOfLineBlock)
                ;
        }
    }
}
