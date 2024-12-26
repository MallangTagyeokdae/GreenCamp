// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.SceneManagement.Events;
using Doozy.Runtime.SceneManagement.ScriptableObjects;
using Doozy.Runtime.Signals;
using UnityEngine;
using UnityEngine.SceneManagement;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.SceneManagement
{
    /// <summary>
    ///     Loads and unloads scenes and has callbacks for when the active scene changed, a scene was loaded and a scene was unloaded.
    /// </summary>
    [AddComponentMenu("Scene Management/Scene Director")]
    public class SceneDirector : SingletonBehaviour<SceneDirector>
    {
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/Scene Management/Scene Director", false, 9)]
        private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        {
            GameObjectUtils.AddToScene<SceneDirector>(true, true);
        }
        #endif

        /// <summary> Stream category name </summary>
        public const string k_StreamCategory = "SceneManagement";
        public const string k_StreamName = nameof(SceneDirector);

        [ClearOnReload]
        private static SignalStream s_stream;
        /// <summary> Signal stream for this component type </summary>
        public static SignalStream stream => s_stream ??= SignalsService.GetStream(k_StreamCategory, k_StreamName);

        /// <summary> Reference to the SceneManagement global settings </summary>
        public static SceneManagementSettings settings => SceneManagementSettings.instance;

        /// <summary> Debug flag </summary>
        public bool debug => DebugMode | settings.DebugMode;

        /// <summary> Enable relevant debug messages to be printed to the console </summary>
        public bool DebugMode;

        [SerializeField] private ActiveSceneChangedEvent OnActiveSceneChanged;
        /// <summary> UnityEvent executed when the active Scene has changed </summary>
        public ActiveSceneChangedEvent onActiveSceneChanged => OnActiveSceneChanged ?? (OnActiveSceneChanged = new ActiveSceneChangedEvent());

        [SerializeField] private SceneLoadedEvent OnSceneLoaded;
        /// <summary> UnityEvent executed when a Scene has loaded </summary>
        public SceneLoadedEvent onSceneLoaded => OnSceneLoaded ?? (OnSceneLoaded = new SceneLoadedEvent());

        [SerializeField] private SceneUnloadedEvent OnSceneUnloaded;
        /// <summary> UnityEvent executed when a Scene has unloaded </summary>
        public SceneUnloadedEvent onSceneUnloaded => OnSceneUnloaded ?? (OnSceneUnloaded = new SceneUnloadedEvent());

        /// <summary> Signal receiver </summary>
        private SignalReceiver receiver { get; set; }

        private void ProcessSignal(Signal signal)
        {
            if (!signal.hasValue)
                return;

            if (!(signal.valueAsObject is SceneLoaderSignalData data))
                return;
        }

        protected override void Awake()
        {
            base.Awake();
            receiver = new SignalReceiver().SetOnSignalCallback(ProcessSignal);
            stream.ConnectReceiver(receiver);
        }

        private void OnEnable()
        {
            SceneManager.activeSceneChanged += ActiveSceneChanged;
            SceneManager.sceneLoaded += SceneLoaded;
            SceneManager.sceneUnloaded += SceneUnloaded;
        }

        private void OnDisable()
        {
            SceneManager.activeSceneChanged -= ActiveSceneChanged;
            SceneManager.sceneLoaded -= SceneLoaded;
            SceneManager.sceneUnloaded -= SceneUnloaded;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            stream.DisconnectReceiver(receiver);
        }

        /// <summary> Method called by the SceneManager.activeSceneChanged UnityAction </summary>
        /// <param name="current"> Replaced Scene </param>
        /// <param name="next"> Next Scene </param>
        private void ActiveSceneChanged(Scene current, Scene next)
        {
            onActiveSceneChanged?.Invoke(current, next);
            if (debug) Log($"{ObjectNames.NicifyVariableName(nameof(ActiveSceneChanged))} - Replaced Scene: {current.name} / Next Scene: {next.name}");
        }

        /// <summary> Method called by the SceneManager.sceneLoaded UnityAction </summary>
        /// <param name="scene"> Loaded Scene </param>
        /// <param name="mode"> LoadSceneMode used to load the scene </param>
        private void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            onSceneLoaded?.Invoke(scene, mode);
            if (debug) Log($"{ObjectNames.NicifyVariableName(nameof(SceneLoaded))} - Scene: {scene.name} / LoadSceneMode: {mode}");
        }

        /// <summary> Method called by the SceneManager.sceneUnloaded UnityAction </summary>
        /// <param name="unloadedScene"> Unloaded Scene</param>
        private void SceneUnloaded(Scene unloadedScene)
        {
            onSceneUnloaded?.Invoke(unloadedScene);
            if (debug) Log($"{ObjectNames.NicifyVariableName(nameof(SceneUnloaded))} - Scene: {unloadedScene.name}");
        }

        /// <summary> Create a SceneLoader that loads the Scene asynchronously in the background by its index in Build Settings, then returns a reference to the newly created SceneLoader </summary>
        /// <param name="sceneBuildIndex"> Index of the Scene in the Build Settings to load </param>
        /// <param name="loadSceneMode"> If LoadSceneMode.Single then all current Scenes will be unloaded before loading </param>
        public static SceneLoader LoadSceneAsync(int sceneBuildIndex, LoadSceneMode loadSceneMode)
        {
            if (instance.debug) Log($"{ObjectNames.NicifyVariableName(nameof(LoadSceneAsync))} - sceneBuildIndex: {sceneBuildIndex} / loadSceneMode: {loadSceneMode}", instance);
            var loader = SceneLoader.GetLoader();
            loader.SetSceneBuildIndex(sceneBuildIndex)
                .SetLoadSceneBy(GetSceneBy.BuildIndex)
                .SetLoadSceneMode(loadSceneMode)
                .LoadSceneAsync();
            return loader;
        }

        /// <summary> Create a SceneLoader that loads the Scene asynchronously in the background by its name in Build Settings, then returns a reference to the newly created SceneLoader </summary>
        /// <param name="sceneName"> Name or path of the Scene to load </param>
        /// <param name="loadSceneMode"> If LoadSceneMode.Single then all current Scenes will be unloaded before loading </param>
        public static SceneLoader LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode)
        {
            if (instance.debug) Log($"{ObjectNames.NicifyVariableName(nameof(LoadSceneAsync))} - sceneName: {sceneName} / loadSceneMode: {loadSceneMode}", instance);
            var loader = SceneLoader.GetLoader();
            loader.SetSceneName(sceneName)
                .SetLoadSceneBy(GetSceneBy.Name)
                .SetLoadSceneMode(loadSceneMode)
                .LoadSceneAsync();
            return loader;
        }

        /// <summary> Destroys all GameObjects associated with the given Scene and removes the Scene from the SceneManager </summary>
        /// <param name="scene"> Scene to unload. </param>
        public static AsyncOperation UnloadSceneAsync(Scene scene)
        {
            if (instance.debug) Log($"{ObjectNames.NicifyVariableName(nameof(UnloadSceneAsync))} - scene: {scene.name}", instance);
            return scene.IsValid() ? SceneManager.UnloadSceneAsync(scene) : null;
        }

        /// <summary> Destroys all GameObjects associated with the given Scene and removes the Scene from the SceneManager </summary>
        /// <param name="sceneBuildIndex"> Index of the Scene in BuildSettings </param>
        public static AsyncOperation UnloadSceneAsync(int sceneBuildIndex)
        {
            if (instance.debug) Log($"{ObjectNames.NicifyVariableName(nameof(UnloadSceneAsync))} - sceneBuildIndex: {sceneBuildIndex}", instance);
            return SceneManager.GetSceneByBuildIndex(sceneBuildIndex).IsValid() ? SceneManager.UnloadSceneAsync(sceneBuildIndex) : null;
        }

        /// <summary> Destroys all GameObjects associated with the given Scene and removes the Scene from the SceneManager </summary>
        /// <param name="sceneName"> Name or path of the Scene to unload. </param>
        public static AsyncOperation UnloadSceneAsync(string sceneName)
        {
            if (instance.debug) Log($"{ObjectNames.NicifyVariableName(nameof(UnloadSceneAsync))} - sceneName: {sceneName}", instance);
            return SceneManager.GetSceneByName(sceneName).IsValid() ? SceneManager.UnloadSceneAsync(sceneName) : null;

        }

        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary> Adds SceneDirector to scene and returns a reference to it </summary>
        public static SceneDirector AddToScene(bool selectGameObjectAfterCreation = false) =>
            GameObjectUtils.AddToScene<SceneDirector>(true, selectGameObjectAfterCreation);

        private void Log(string message) =>
            Log($"[{name}] {message}", this);

        private static void Log(string message, Object context) =>
            Debugger.Log($"({nameof(SceneDirector)}) {message}", context);
    }
}
