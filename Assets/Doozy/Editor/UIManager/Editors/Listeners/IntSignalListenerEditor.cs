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
    [CustomEditor(typeof(IntSignalListener), true)]
    public class IntSignalListenerEditor : SignalListenerEditor
    {
        private IntSignalListener castedTarget => (IntSignalListener)target;
        private IEnumerable<IntSignalListener> castedTargets => targets.Cast<IntSignalListener>();

        private FluidField onIntSignalFluidField { get; set; }
        
        private SerializedProperty propertyOnIntSignal { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            onIntSignalFluidField?.Recycle();
        }

        protected override void FindProperties()
        {
            base.FindProperties();
            propertyOnIntSignal = serializedObject.FindProperty("OnIntSignal");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();
            
            componentHeader
                .SetComponentNameText("Int")
                .SetComponentTypeText("Signal Listener")
                .AddManualButton()
                .AddApiButton()
                .AddYouTubeButton();

            onIntSignalFluidField =
                FluidField.Get()
                    .AddFieldContent(DesignUtils.UnityEventField("UnityEvent with a int parameter", propertyOnIntSignal));
        }

        protected override void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(idFluidField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(onIntSignalFluidField)
                // .AddChild(DesignUtils.spaceBlock)
                // .AddChild(callbackFluidField)
                // .AddChild(DesignUtils.spaceBlock)
                // .AddChild(onSignalFluidField)
                .AddChild(DesignUtils.endOfLineBlock)
                ;
        }
    }
}
