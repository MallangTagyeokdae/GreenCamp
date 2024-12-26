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
    [CustomEditor(typeof(StringSignalListener), true)]
    public class StringSignalListenerEditor : SignalListenerEditor
    {
        private StringSignalListener castedTarget => (StringSignalListener)target;
        private IEnumerable<StringSignalListener> castedTargets => targets.Cast<StringSignalListener>();

        private FluidField onStringSignalFluidField { get; set; }
        
        private SerializedProperty propertyOnStringSignal { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            onStringSignalFluidField?.Recycle();
        }

        protected override void FindProperties()
        {
            base.FindProperties();
            propertyOnStringSignal = serializedObject.FindProperty("OnStringSignal");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();
            
            componentHeader
                .SetComponentNameText("String")
                .SetComponentTypeText("Signal Listener")
                .AddManualButton()
                .AddApiButton()
                .AddYouTubeButton();

            onStringSignalFluidField =
                FluidField.Get()
                    .AddFieldContent(DesignUtils.UnityEventField("UnityEvent with a string parameter", propertyOnStringSignal));
        }

        protected override void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(idFluidField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(onStringSignalFluidField)
                // .AddChild(DesignUtils.spaceBlock)
                // .AddChild(callbackFluidField)
                // .AddChild(DesignUtils.spaceBlock)
                // .AddChild(onSignalFluidField)
                .AddChild(DesignUtils.endOfLineBlock)
                ;
        }
    }
}
