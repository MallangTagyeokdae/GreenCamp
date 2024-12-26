// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Nody;
using Doozy.Runtime.Nody.Nodes.Internal;
using UnityEngine.SceneManagement;
// ReSharper disable RedundantOverriddenMember

namespace Doozy.Runtime.SceneManagement.Nodes
{
    /// <summary>
    ///     Tells the SceneDirector to unload a Scene either by scene name or scene build index.
    ///     <para />
    ///     This will destroy all GameObjects associated with the given Scene and remove the Scene from the SceneManager.
    ///     <para />
    ///     Besides unloading a Scene, the Unload Scene Node can wait until the target Scene has been unloaded before activating the next node in the Graph.
    ///     <para />
    ///     The next node in the Graph is the one connected to this node’s output socket.
    /// </summary>
    [Serializable]
    [NodyMenuPath("Scene Management", "Unload Scene")] // <<< Change search menu options here category and node name
    public sealed class UnloadSceneNode : SimpleNode
    {
        public GetSceneBy GetSceneBy = SceneLoader.k_DefaultGetSceneBy;
        public int SceneBuildIndex = SceneLoader.k_DefaultBuildIndex;
        public string SceneName = SceneLoader.k_DefaultSceneName;
        public bool WaitForSceneToUnload;

        public UnloadSceneNode()
        {
            AddInputPort()                 // add a new input port
                .SetCanBeDeleted(false)    // port options
                .SetCanBeReordered(false); // port options

            AddOutputPort()                // add a new output port
                .SetCanBeDeleted(false)    // port options
                .SetCanBeReordered(false); // port options

            canBeDeleted = true; // Used to prevent special nodes from being deleted in the editor

            runUpdate = false;      // Run Update when the node is active
            runFixedUpdate = false; // Run FixedUpdate when the node is active
            runLateUpdate = false;  // Run LateUpdate when the node is active

            passthrough = true; //allow the graph to bypass this node when going back

            clearGraphHistory = false; //remove the possibility of being able to go back to previously active nodes
        }

        // Called on the frame when this node becomes active
        public override void OnEnter(FlowNode previousNode = null, FlowPort previousPort = null)
        {
            base.OnEnter(previousNode, previousPort);
            if (WaitForSceneToUnload) SceneDirector.instance.onSceneUnloaded.AddListener(SceneUnloaded);
            Run();
            if (WaitForSceneToUnload) return;
            GoToNextNode(firstOutputPort);
        }

        private void SceneUnloaded(Scene unloadedScene)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (GetSceneBy)
            {
                case GetSceneBy.Name:
                    if (!unloadedScene.name.Equals(SceneName))
                        return;
                    break;
                case GetSceneBy.BuildIndex:
                    if (!unloadedScene.name.Equals(SceneManager.GetSceneByBuildIndex(SceneBuildIndex).name))
                        return;
                    break;
            }
            
            SceneDirector.instance.onSceneUnloaded.RemoveListener(SceneUnloaded);
            GoToNextNode(firstOutputPort);
        }

        private void Run()
        {
            switch (GetSceneBy)
            {
                case GetSceneBy.Name:
                    SceneDirector.UnloadSceneAsync(SceneName);
                    break;
                case GetSceneBy.BuildIndex:
                    SceneDirector.UnloadSceneAsync(SceneBuildIndex);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}
