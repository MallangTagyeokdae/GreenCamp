using System;
using System.Collections.Generic;
using Doozy.Editor.Common.ScriptableObjects;
using Doozy.Editor.EditorUI;
using Doozy.Runtime.Common;
using Doozy.Runtime.Nody;
using UnityEditor;
using UnityEngine;
namespace Doozy.Editor.Nody.ScriptableObjects
{
    public class NodySettings : SingletonEditorScriptableObject<NodySettings>
    {
        /// <summary> Previously opened flow graph. Used by the Nody window </summary>
        public FlowGraph Graph;

        private static FlowGraphView flowGraphView => NodyWindow.instance.flowGraphView;
        private static FlowGraph flowGraph => NodyWindow.instance.flowGraph;

        public void Reset()
        {
            Graph = null;
        }

        public static void SaveSettings()
        {
            // Debugger.Log($"{nameof(NodySettings)}.{nameof(SaveSettings)}");

            if (!NodyWindow.isOpen) return; //Nody not opened -> stop
            if (flowGraph == null || !EditorUtility.IsPersistent(flowGraph))
            {
                ResetSettings();
                // no graph is loaded -> stop
                // or
                // the graph loaded in the Nody Window is not an asset -> stop
                return;
            }
            flowGraph.EditorPosition = flowGraphView.viewTransform.position;
            flowGraph.EditorScale = flowGraphView.viewTransform.scale;
            EditorUtility.SetDirty(flowGraph);
            AssetDatabase.SaveAssetIfDirty(flowGraph);
            
            instance.Graph = flowGraph;
            Save();
        }

        public static void ResetSettings()
        {
            // Debugger.Log($"{nameof(NodySettings)}.{nameof(ResetSettings)}");
            
            instance.Reset();
            Save();
        }
    }
}
