// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Runtime.Global;
using Doozy.Runtime.Nody;
using Doozy.Runtime.Nody.Nodes.Internal;
using Doozy.Runtime.Reactor;
using UnityEngine.SceneManagement;
// ReSharper disable RedundantOverriddenMember

namespace Doozy.Runtime.SceneManagement.Nodes
{
    /// <summary>
    /// Specialized node used to load a scene.
    /// </summary>
    [Serializable]
    [NodyMenuPath("Scene Management", "Load Scene")] // <<< Change search menu options here category and node name
    public sealed class LoadSceneNode : SimpleNode
    {
        /// <summary> Determines what load method that will be used </summary>
        public GetSceneBy GetSceneBy = SceneLoader.k_DefaultGetSceneBy;
        /// <summary> Determines how the new scene is loaded </summary>
        public LoadSceneMode LoadSceneMode = SceneLoader.k_DefaultLoadSceneMode;
        /// <summary>
        ///     Allow Scenes to be activated as soon as it is ready. <para/>
        ///     When loading a scene, Unity first loads the scene (load progress from 0% to 90%) and then activates it (load progress from 90% to 100%). It's a two state process. <para/>
        ///     This option can stop the scene activation (at 90% load progress), after the scene has been loaded and is ready. <para/>
        ///     Useful if you need to load several scenes at once and activate them in a specific order and/or at a specific time.
        /// </summary>
        public bool AllowSceneActivation = SceneLoader.k_DefaultAutoSceneActivation;
        /// <summary>
        ///     Sets for how long will the SceneLoader wait, after a scene has been loaded, before it starts the scene activation process (works only if AllowSceneActivation is enabled).
        ///     <para />
        ///     When loading a scene, Unity first loads the scene (load progress from 0% to 90%) and then activates it (load progress from 90% to 100%). It's a two state process.
        ///     <para />
        ///     This delay is after the scene has been loaded and before its activation (at 90% load progress)
        /// </summary>
        public float SceneActivationDelay = SceneLoader.k_DefaultSceneActivationDelay;
        /// <summary> Index of the Scene in the Build Settings to load (when GetSceneBy is set to GetSceneBy.BuildIndex) </summary>
        public int SceneBuildIndex = SceneLoader.k_DefaultBuildIndex;
        /// <summary> Name or path of the Scene to load (when GetSceneBy is set to GetSceneBy.Name) </summary>
        public string SceneName = SceneLoader.k_DefaultSceneName;
        /// <summary> Do not go to the next node until the scene has been loaded </summary>
        public bool WaitForSceneToLoad = true;
        /// <summary> Prevent loading a scene that is already loaded </summary>
        public bool PreventLoadingSameScene = true;
        
        public bool ConnectProgressor = false;
        public ProgressorId ProgressorId = new ProgressorId();

        public LoadSceneNode()
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
            Run();
            if (!WaitForSceneToLoad)
                GoToNextNode(firstOutputPort);
        }

        private void Run()
        {
            if (PreventLoadingSameScene)
            {
                switch (GetSceneBy)
                {
                    case GetSceneBy.Name:
                    {
                        if (SceneLoader.IsSceneLoaded(SceneName))
                        {
                            GoToNextNode(firstOutputPort);
                            return;
                        }
                        break;
                    }
                    case GetSceneBy.BuildIndex:
                    {
                        if(SceneLoader.IsSceneLoaded(SceneBuildIndex))
                        {
                            GoToNextNode(firstOutputPort);
                            return;
                        }
                        
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            SceneLoader loader =
                SceneLoader.GetLoader()
                    .SetLoadSceneMode(LoadSceneMode)
                    .SetLoadSceneBy(GetSceneBy)
                    .SetSceneName(SceneName)
                    .SetSceneBuildIndex(SceneBuildIndex)
                    .SetAllowSceneActivation(AllowSceneActivation)
                    .SetSceneActivationDelay(SceneActivationDelay)
                    .SetSelfDestructAfterSceneLoaded(true);

            if (ConnectProgressor)
            {
                IEnumerable<Progressor> progressors = Progressor.GetProgressors(ProgressorId.Category, ProgressorId.Name);
                if (progressors != null)
                    foreach (Progressor progressor in progressors)
                        loader.AddProgressor(progressor);
            }

            if (WaitForSceneToLoad)
            {
                if (AllowSceneActivation)
                    loader.OnSceneActivated.Event.AddListener(() => Coroutiner.ExecuteLater(() => GoToNextNode(firstOutputPort), 1));
                    // loader.OnSceneActivated.Event.AddListener(() => GoToNextNode(firstOutputPort));
                else
                    loader.OnSceneLoaded.Event.AddListener(() => Coroutiner.ExecuteLater(() => GoToNextNode(firstOutputPort), 1));
                    // loader.OnSceneLoaded.Event.AddListener(() => GoToNextNode(firstOutputPort));
            }

            loader.LoadSceneAsync();
        }
    }
}
