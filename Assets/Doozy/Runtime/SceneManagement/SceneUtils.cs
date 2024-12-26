// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Common;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.SceneManagement.ScriptableObjects;
using UnityEngine.SceneManagement;
namespace Doozy.Runtime.SceneManagement
{
    public static class SceneUtils
    {
        private static SceneManagementSettings settings => SceneManagementSettings.instance;
        
        /// <summary> Create a SceneLoader that loads the Scene asynchronously in the background by its index in Build Settings, then returns a reference to the newly created SceneLoader </summary>
        /// <param name="sceneBuildIndex"> Index of the Scene in the Build Settings to load </param>
        /// <param name="loadSceneMode"> If LoadSceneMode.Single then all current Scenes will be unloaded before loading </param>
        /// <param name="progressor"> Progressor that will get referenced to the SceneLoader, to get updated while the scene loads </param>
        // public static SceneLoader LoadSceneAsync(int sceneBuildIndex, LoadSceneMode loadSceneMode, Progressor progressor = null)
        // {
        //     if (settings.debugMode) Debugger.Log($"LoadSceneAsync - sceneBuildIndex: {sceneBuildIndex} / loadSceneMode: {loadSceneMode} / has Progressor: {(progressor == null ? "No" : "Yes")}", SceneDirector.instance);
        //     SceneLoader loader = SceneLoader.GetLoader();
        //     loader.SetSceneBuildIndex(sceneBuildIndex)
        //         .SetLoadSceneBy(GetSceneBy.BuildIndex)
        //         .SetProgressor(progressor)
        //         .SetLoadSceneMode(loadSceneMode)
        //         .LoadSceneAsync();
        //     return loader;
        // }
    }
}
