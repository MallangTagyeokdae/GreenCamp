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
    [CustomEditor(typeof(Vector3SignalListener), true)]
    public class Vector3SignalListenerEditor : SignalListenerEditor
    {
        private Vector3SignalListener castedTarget => (Vector3SignalListener)target;
        private IEnumerable<Vector3SignalListener> castedTargets => targets.Cast<Vector3SignalListener>();

        private FluidField onVector3SignalFluidField { get; set; }
        
        private SerializedProperty propertyOnVector3Signal { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            onVector3SignalFluidField?.Recycle();
        }

        protected override void FindProperties()
        {
            base.FindProperties();
            propertyOnVector3Signal = serializedObject.FindProperty("OnVector3Signal");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();
            
            componentHeader
                .SetComponentNameText("Vector3")
                .SetComponentTypeText("Signal Listener")
                .AddManualButton()
                .AddApiButton()
                .AddYouTubeButton();

            onVector3SignalFluidField =
                FluidField.Get()
                    .AddFieldContent(DesignUtils.UnityEventField("UnityEvent with a Vector3 parameter", propertyOnVector3Signal));
        }

        protected override void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(idFluidField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(onVector3SignalFluidField)
                // .AddChild(DesignUtils.spaceBlock)
                // .AddChild(callbackFluidField)
                // .AddChild(DesignUtils.spaceBlock)
                // .AddChild(onSignalFluidField)
                .AddChild(DesignUtils.endOfLineBlock)
                ;
        }
    }
}
