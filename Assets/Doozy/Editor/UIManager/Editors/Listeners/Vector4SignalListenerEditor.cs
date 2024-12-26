// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Listeners;
using UnityEditor;

namespace Doozy.Editor.UIManager.Editors.Listeners
{
    [CustomEditor(typeof(Vector4SignalListener), true)]
    public class Vector4SignalListenerEditor : SignalListenerEditor
    {
        private Vector4SignalListener castedTarget => (Vector4SignalListener)target;
        private IEnumerable<Vector4SignalListener> castedTargets => targets.Cast<Vector4SignalListener>();

        private FluidField onVector4SignalFluidField { get; set; }
        
        private SerializedProperty propertyOnVector4Signal { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            onVector4SignalFluidField?.Recycle();
        }

        protected override void FindProperties()
        {
            base.FindProperties();
            propertyOnVector4Signal = serializedObject.FindProperty("OnVector4Signal");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();
            
            componentHeader
                .SetComponentNameText("Vector4")
                .SetComponentTypeText("Signal Listener")
                .AddManualButton()
                .AddApiButton()
                .AddYouTubeButton();

            onVector4SignalFluidField =
                FluidField.Get()
                    .AddFieldContent(DesignUtils.UnityEventField("UnityEvent with a Vector4 parameter", propertyOnVector4Signal));
        }

        protected override void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(idFluidField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(onVector4SignalFluidField)
                // .AddChild(DesignUtils.spaceBlock)
                // .AddChild(callbackFluidField)
                // .AddChild(DesignUtils.spaceBlock)
                // .AddChild(onSignalFluidField)
                .AddChild(DesignUtils.endOfLineBlock)
                ;
        }
    }
}
