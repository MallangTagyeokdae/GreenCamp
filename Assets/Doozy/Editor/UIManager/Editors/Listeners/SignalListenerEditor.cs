// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Listeners;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Listeners
{
    [CustomEditor(typeof(SignalListener), true)]
    public class SignalListenerEditor : UnityEditor.Editor
    {
        protected static Color accentColor => EditorColors.UIManager.ListenerComponent;
        protected static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.ListenerComponent;

        private SignalListener castedTarget => (SignalListener)target;
        private IEnumerable<SignalListener> castedTargets => targets.Cast<SignalListener>();

        protected VisualElement root { get; set; }
        protected FluidComponentHeader componentHeader { get; set; }

        protected PropertyField streamIdPropertyField { get; set; }
        protected PropertyField callbackPropertyField { get; set; }

        protected FluidField idFluidField { get; set; }
        protected FluidField callbackFluidField { get; set; }
        protected FluidField onSignalFluidField { get; set; }

        protected SerializedProperty propertyStreamId { get; set; }
        protected SerializedProperty propertyCallback { get; set; }
        protected SerializedProperty propertyOnSignal { get; set; }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        protected virtual void OnDestroy()
        {
            componentHeader?.Recycle();
            idFluidField?.Recycle();
            callbackFluidField?.Recycle();
            onSignalFluidField?.Recycle();
        }

        protected virtual void FindProperties()
        {
            propertyStreamId = serializedObject.FindProperty("StreamId");
            propertyCallback = serializedObject.FindProperty("Callback");
            propertyOnSignal = serializedObject.FindProperty("OnSignal");
        }

        protected virtual void InitializeEditor()
        {
            FindProperties();
            root = DesignUtils.GetEditorRoot();

            componentHeader =
                FluidComponentHeader.Get()
                    .SetElementSize(ElementSize.Large)
                    .SetAccentColor(accentColor)
                    .SetComponentNameText((ObjectNames.NicifyVariableName(nameof(SignalListener))))
                    .SetIcon(EditorSpriteSheets.UIManager.Icons.SignalListener)
                    .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048772618/Signal+Listener?atlOrigin=eyJpIjoiNmM1M2MyZDg1ZDM4NGYzZTljMWM3ZWZiNDYyZjg2MjAiLCJwIjoiYyJ9")
                    .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.UIManager.Listeners.SignalListener.html")
                    .AddYouTubeButton();

            streamIdPropertyField =
                DesignUtils.NewPropertyField(propertyStreamId);

            callbackPropertyField =
                DesignUtils.NewPropertyField(propertyCallback);

            idFluidField =
                FluidField.Get()
                    .AddFieldContent(streamIdPropertyField);

            callbackFluidField =
                FluidField.Get()
                    .AddFieldContent(callbackPropertyField);

            onSignalFluidField =
                FluidField.Get()
                    .AddFieldContent(DesignUtils.UnityEventField("UnityEvent with a Signal parameter", propertyOnSignal));

            root.schedule.Execute(() => callbackFluidField.Q<FluidToggleSwitch>()?.Recycle());

        }

        protected virtual void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(idFluidField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(callbackFluidField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(onSignalFluidField)
                .AddChild(DesignUtils.endOfLineBlock)
                ;
        }
    }
}
