// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Nody;
using Doozy.Runtime.Nody.Nodes.Internal;
// ReSharper disable RedundantOverriddenMember

namespace Doozy.Runtime.SceneManagement.Nodes
{
    /// <summary>
    ///     Activates all the Scenes that have been loaded by SceneLoaders and are ready to be activated and then jumps instantly to the next node in the Graph.
    ///     <para />
    ///     When loading a scene, Unity first loads the scene (load progress from 0% to 90%) and then activates it (load progress from 90% to 100%). It's a two state process.
    ///     <para />
    ///     A scene is ready to be activated if the load progress is at 0.9 (90%). This node activates these scenes (that have been loaded by SceneLoader and that have AllowSceneActivation set to false).
    /// </summary>
    [Serializable]
    [NodyMenuPath("Scene Management", "Activate Loaded Scenes")] // <<< Change search menu options here category and node name
    public sealed class ActivateLoadedScenesNode : SimpleNode
    {
        public ActivateLoadedScenesNode()
        {
            AddInputPort()                 // add a new input port
                .SetCanBeDeleted(false)    // port options
                .SetCanBeReordered(false); // port options

            AddOutputPort()                // add a new output port
                .SetCanBeDeleted(false)    // port options
                .SetCanBeReordered(false); // port options

            canBeDeleted = true;           // Used to prevent special nodes from being deleted in the editor
            
            runUpdate = false;             // Run Update when the node is active
            runFixedUpdate = false;        // Run FixedUpdate when the node is active
            runLateUpdate = false;         // Run LateUpdate when the node is active
            
            passthrough = true;            //allow the graph to bypass this node when going back
                
            clearGraphHistory = false;     //remove the possibility of being able to go back to previously active nodes
        }

        public override void OnEnter(FlowNode previousNode = null, FlowPort previousPort = null)
        {
            base.OnEnter(previousNode, previousPort);
            Run();                       
            GoToNextNode(firstOutputPort);
        }
        
        private void Run()
        {
            SceneLoader.ActivateLoadedScenes();
        }

    }
}