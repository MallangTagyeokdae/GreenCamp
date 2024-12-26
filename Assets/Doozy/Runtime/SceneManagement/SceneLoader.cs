// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections;
using System.Collections.Generic;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.Events;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Global;
using Doozy.Runtime.Mody;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.SceneManagement.ScriptableObjects;
using Doozy.Runtime.Signals;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Local

namespace Doozy.Runtime.SceneManagement
{
    /// <summary>
    ///     Loads any Scene either by scene name or scene build index and updates a Progressor to show the loading progress.
    ///     It can also trigger a set of 'actions' when the scene started loading (at 0% load progress) and/or when the scene has been loaded (but not activated) (at 90% load progress)
    /// </summary>
    [AddComponentMenu("Scene Management/Scene Loader")]
    public class SceneLoader : MonoBehaviour
    {
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/Scene Management/Scene Loader", false, 9)]
        private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        {
            GameObjectUtils.AddToScene<SceneLoader>(false, true);
        }
        #endif

        public enum State
        {
            /// <summary> Idle mode </summary>
            Idle,
            /// <summary> Starting to load a scene </summary>
            LoadScene,
            /// <summary> Is in the process of loading a scene </summary>
            Loading,
            /// <summary> Has finished loading a scene, but has not activated it yet </summary>
            SceneLoaded,
            /// <summary> Is activating the loaded scene </summary>
            ActivatingScene
        }

        /// <summary> Stream category name </summary>
        public const string k_StreamCategory = "SceneManagement";
        /// <summary> Stream name </summary>
        public const string k_StreamName = nameof(SceneLoader);

        public const GetSceneBy k_DefaultGetSceneBy = GetSceneBy.Name;
        public const LoadSceneMode k_DefaultLoadSceneMode = LoadSceneMode.Single;
        public const bool k_DefaultAutoSceneActivation = true;
        public const bool k_DefaultPreventLoadingSameScene = false;
        public const bool k_DefaultSelfDestructAfterSceneLoaded = false;
        public const float k_DefaultSceneActivationDelay = 0.2f;
        public const int k_DefaultBuildIndex = 0;
        public const string k_DefaultSceneName = "";

        /// <summary> Database used to keep track of all the SceneLoaders </summary>
        [ClearOnReload]
        public static HashSet<SceneLoader> database { get; } = new HashSet<SceneLoader>();

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

        /// <summary> Invoked when a scene started loading </summary>
        public ModyEvent OnLoadScene = new ModyEvent(nameof(OnLoadScene));

        /// <summary>
        /// Invoked when the scene has been loaded (the progress is at 0.9 (90%))
        /// and has not been activated yet (the reset 0.1 (10%)).
        /// <para/>
        /// When loading a scene, Unity first loads the scene (load progress from 0% to 90%)
        /// and then activates it (load progress from 90% to 100%). It's a two state process.
        /// <para/>
        /// This action is triggered after the scene has been loaded
        /// and before its activation (at 90% load progress)
        /// </summary>
        public ModyEvent OnSceneLoaded = new ModyEvent(nameof(OnSceneLoaded));

        /// <summary>
        /// Invoked after a scene was loaded and then activated.
        /// <para/>
        /// When loading a scene, Unity first loads the scene (load progress from 0% to 90%)
        /// and then activates it (load progress from 90% to 100%). It's a two state process.
        /// <para/>
        /// This action is triggered after the scene has been loaded and activated.
        /// </summary>
        public ModyEvent OnSceneActivated = new ModyEvent(nameof(OnSceneActivated));

        /// <summary> Event triggered when an async operation is running and its progress has been updated. </summary>
        public FloatEvent OnProgressChanged = new FloatEvent();

        /// <summary> Keeps track and manages the asyncOperation started when the scene loader begins to load a scene </summary>
        public AsyncOperation currentAsyncOperation { get; private set; }

        [SerializeField] private bool AllowSceneActivation = k_DefaultAutoSceneActivation;
        /// <summary>
        ///     Allow Scenes to be activated as soon as it is ready. <para/>
        ///     When loading a scene, Unity first loads the scene (load progress from 0% to 90%) and then activates it (load progress from 90% to 100%). It's a two state process. <para/>
        ///     This option can stop the scene activation (at 90% load progress), after the scene has been loaded and is ready. <para/>
        ///     Useful if you need to load several scenes at once and activate them in a specific order and/or at a specific time.
        /// </summary>
        public bool allowSceneActivation
        {
            get => AllowSceneActivation;
            set => AllowSceneActivation = value;
        }

        [SerializeField] private bool PreventLoadingSameScene = k_DefaultPreventLoadingSameScene;
        /// <summary> Prevent loading a scene that is already loaded. </summary>
        public bool preventLoadingSameScene
        {
            get => PreventLoadingSameScene;
            set => PreventLoadingSameScene = value;
        }

        /// <summary> Determines what load method this SceneLoader will use by default if the load scene method is called without any parameters </summary>
        public GetSceneBy GetSceneBy = k_DefaultGetSceneBy;

        /// <summary> Determines how the new scene is loaded by this SceneLoader if the load scene method is called without any parameters </summary>
        public LoadSceneMode LoadSceneMode = k_DefaultLoadSceneMode;

        [SerializeField] private List<Progressor> Progressors;
        /// <summary> Progressors updated when the scene loader progress changes. </summary>
        public List<Progressor> progressors => Progressors ?? (Progressors = new List<Progressor>());

        /// <summary> If an async operation is running, it returns the current load progress (float between 0 and 1) </summary>
        public float progress
        {
            get => m_Progress;
            private set
            {
                // Debugger.Log($"progress: {value}");
                m_Progress = value;
                progressors.ForEach(p => p.PlayToProgress(value));
                OnProgressChanged?.Invoke(value);
            }
        }

        /// <summary>
        ///     Sets for how long will the SceneLoader wait, after a scene has been loaded, before it starts the scene activation process (works only if AllowSceneActivation is enabled).
        ///     <para />
        ///     When loading a scene, Unity first loads the scene (load progress from 0% to 90%) and then activates it (load progress from 90% to 100%). It's a two state process.
        ///     <para />
        ///     This delay is after the scene has been loaded and before its activation (at 90% load progress)
        /// </summary>
        public float SceneActivationDelay = k_DefaultSceneActivationDelay;

        /// <summary> Index of the Scene in the Build Settings to load (when GetSceneBy is set to GetSceneBy.BuildIndex) </summary>
        public int SceneBuildIndex = k_DefaultBuildIndex;

        /// <summary> Name or path of the Scene to load (when GetSceneBy is set to GetSceneBy.Name) </summary>
        public string SceneName = k_DefaultSceneName;

        /// <summary> Mark this SceneLoader to self destruct (to destroy itself) after it loads a Scene </summary>
        public bool SelfDestructAfterSceneLoaded = k_DefaultSelfDestructAfterSceneLoaded;

        private State m_CurrentState = State.Idle;
        /// <summary> Current state the SceneLoader is in </summary>
        public State currentState
        {
            get => m_CurrentState;
            private set
            {
                bool stateChanged = m_CurrentState != value;
                m_CurrentState = value;
                if (stateChanged) stream?.SendSignal(new SceneLoaderSignalData(this));
            }
        }

        private bool m_LoadInProgress;           //keeps track if a scene load process is currently running
        private bool m_SceneLoadedAndReady;      //mark that the scene has not been loaded (load progress has not reached 90%)
        private bool m_ActivatingScene;          //TRUE when a scene is being activated
        private float m_SceneLoadedAndReadyTime; //
        private float m_Progress;                //updated when an async operation is running (float between 0 and 1)

        private void Awake() =>
            database.Add(this);

        private void OnEnable()
        {
            database.Remove(null);
            ResetProgress();
        }

        private void OnDestroy()
        {
            database.Remove(null);
            database.Remove(this);
        }

        private void Update()
        {
            if (currentAsyncOperation == null) return;
            float calculatedProgress = Mathf.Clamp01(currentAsyncOperation.progress / 0.9f); //update load progress [0, 0.9] > [0, 1]
            if (Math.Abs(progress - calculatedProgress) > 0.0001f) progress = calculatedProgress;
            if (debug && !m_ActivatingScene & !m_SceneLoadedAndReady) Log($"Load progress: {Mathf.Round(progress * 100)}%");

            if (!m_SceneLoadedAndReady & !m_ActivatingScene)
                currentState = State.Loading;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (!m_SceneLoadedAndReady && currentAsyncOperation.progress == 0.9f) // Loading completed
            {
                if (debug) Log($"Scene finished loading and is ready to be activated.");
                OnSceneLoaded?.Execute();
                currentState = State.SceneLoaded;
                m_SceneLoadedAndReady = true; //mark that the scene has been loaded and is now ready to be activated (bool needed to stop LoadBehavior.OnSceneLoaded.Invoke(gameObject) from executing more than once)
                m_SceneLoadedAndReadyTime = Time.realtimeSinceStartup;
            }

            if (m_SceneLoadedAndReady && !m_ActivatingScene && AllowSceneActivation)
            {
                if (SceneActivationDelay < 0) SceneActivationDelay = 0; //sanity check
                if (SceneActivationDelay >= 0 && Time.realtimeSinceStartup - m_SceneLoadedAndReadyTime > SceneActivationDelay)
                    ActivateLoadedScene();
            }

            if (m_ActivatingScene)
                currentState = State.ActivatingScene;

            if (!currentAsyncOperation.isDone) return;
            if (debug) Log($"Loaded scene has been activated.");
            OnSceneActivated?.Execute();
            m_LoadInProgress = false;
            currentAsyncOperation = null;
            currentState = State.Idle;
            if (SelfDestructAfterSceneLoaded) Coroutiner.Start(SelfDestruct());
        }

        /// <summary>
        /// Check if the scene with the given name or path is loaded.
        /// </summary>
        /// <param name="sceneName"> Scene name </param>
        public static bool IsSceneLoaded(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name == sceneName) return true;
            }

            return false;
        }

        /// <summary>
        /// Check if the scene with the given build index is loaded.
        /// </summary>
        /// <param name="sceneBuildIndex"> Scene build index </param>
        public static bool IsSceneLoaded(int sceneBuildIndex)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.buildIndex == sceneBuildIndex) return true;
            }

            return false;
        }


        /// <summary>
        ///     Activate the current loaded scene.
        ///     <para />
        ///     Works only if the SceneLoader has loaded a scene and its AllowSceneActivation option is set to false.
        ///     <para />
        ///     This method enables the 'allowSceneActivation' for the CurrentAsyncOperation that has been paused at 90%.
        ///     <para />
        ///     When loading a scene, Unity first loads the scene (load progress from 0% to 90%) and then activates it (load progress from 90% to 100%). It's a two state process.
        ///     <para />
        ///     This method is meant to be used for after the scene has been loaded and before its activation (at 90% load progress).
        /// </summary>
        public SceneLoader ActivateLoadedScene()
        {
            if (currentAsyncOperation == null) return this; //no load process is running
            if (debug) Log($"Activating Scene...");
            m_ActivatingScene = true;
            currentState = State.ActivatingScene;
            currentAsyncOperation.allowSceneActivation = true;
            return this;
        }

        /// <summary> Loads the Scene, with the current settings </summary>
        public SceneLoader LoadScene()
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (GetSceneBy)
            {
                case GetSceneBy.Name:
                    if (preventLoadingSameScene && IsSceneLoaded(SceneName)) return this;
                    SceneManager.LoadScene(SceneName, LoadSceneMode);
                    break;
                case GetSceneBy.BuildIndex:
                    if (preventLoadingSameScene && IsSceneLoaded(SceneBuildIndex)) return this;
                    SceneManager.LoadScene(SceneBuildIndex, LoadSceneMode);
                    break;
            }

            return this;
        }

        /// <summary> Loads the Scene, with the current settings, asynchronously in the background </summary>
        public SceneLoader LoadSceneAsync()
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (GetSceneBy)
            {
                case GetSceneBy.Name:
                    if (preventLoadingSameScene && IsSceneLoaded(SceneName)) return this;
                    LoadSceneAsync(SceneName, LoadSceneMode);
                    break;
                case GetSceneBy.BuildIndex:
                    if (preventLoadingSameScene && IsSceneLoaded(SceneBuildIndex)) return this;
                    LoadSceneAsync(SceneBuildIndex, LoadSceneMode);
                    break;
            }

            return this;
        }

        /// <summary> Loads a Scene asynchronously in the background, by its index in Build Settings </summary>
        /// <param name="sceneBuildIndex"> Index, in the Build Settings, of the Scene to load </param>
        /// <param name="mode"> If LoadSceneMode.Single then all current Scenes will be unloaded before activating the newly loaded scene </param>
        public SceneLoader LoadSceneAsync(int sceneBuildIndex, LoadSceneMode mode)
        {
            if (preventLoadingSameScene && IsSceneLoaded(sceneBuildIndex)) return this;
            currentAsyncOperation = SceneManager.LoadSceneAsync(sceneBuildIndex, mode);
            StartSceneLoad();
            return this;
        }

        /// <summary> Loads a Scene asynchronously in the background, by its name in Build Settings </summary>
        /// <param name="sceneName"> Name or path of the Scene to load </param>
        /// <param name="mode"> If LoadSceneMode.Single then all current Scenes will be unloaded before activating the newly loaded scene </param>
        public SceneLoader LoadSceneAsync(string sceneName, LoadSceneMode mode)
        {
            if (preventLoadingSameScene && IsSceneLoaded(sceneName)) return this;
            currentAsyncOperation = SceneManager.LoadSceneAsync(sceneName, mode);
            StartSceneLoad();
            return this;
        }

        /// <summary> Loads a Scene asynchronously in the background, by its index in Build Settings, with the LoadSceneMode.Additive setting </summary>
        /// <param name="sceneBuildIndex"> Index, in the Build Settings, of the Scene to load </param>
        public SceneLoader LoadSceneAsyncAdditive(int sceneBuildIndex) =>
            LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Additive);

        /// <summary> Loads a Scene asynchronously in the background, by its name in Build Settings, with the LoadSceneMode.Additive setting </summary>
        /// <param name="sceneName"> Name or path of the Scene to load </param>
        public SceneLoader LoadSceneAsyncAdditive(string sceneName) =>
            LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        /// <summary> Loads a Scene asynchronously in the background, by its index in Build Settings, with the LoadSceneMode.Single setting </summary>
        /// <param name="sceneBuildIndex"> Index, in the Build Settings, of the Scene to load </param>
        public SceneLoader LoadSceneAsyncSingle(int sceneBuildIndex) =>
            LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Single);

        /// <summary> Loads a Scene asynchronously in the background, by its name in Build Settings, with the LoadSceneMode.Single setting </summary>
        /// <param name="sceneName"> Name or path of the Scene to load </param>
        public SceneLoader LoadSceneAsyncSingle(string sceneName) =>
            LoadSceneAsync(sceneName, LoadSceneMode.Single);

        /// <summary> Set the AllowSceneActivation that that allows for a Scene to be activated as soon as it is ready </summary>
        /// <param name="whenReadyAllowSceneActivation"> Allow Scenes to be activated as soon as it is ready </param>
        public SceneLoader SetAllowSceneActivation(bool whenReadyAllowSceneActivation)
        {
            allowSceneActivation = whenReadyAllowSceneActivation;
            return this;
        }

        /// <summary> Set the GetSceneBy value, that determines what load method this SceneLoader will use by default </summary>
        /// <param name="getSceneBy"> Load method this SceneLoader will use if the load scene method is called without any parameters </param>
        public SceneLoader SetLoadSceneBy(GetSceneBy getSceneBy)
        {
            GetSceneBy = getSceneBy;
            return this;
        }

        /// <summary> Set the LoadSceneMode value, that determines how the new scene is loaded by this SceneLoader </summary>
        /// <param name="loadSceneMode"> Load mode used when loading a scene </param>
        public SceneLoader SetLoadSceneMode(LoadSceneMode loadSceneMode)
        {
            LoadSceneMode = loadSceneMode;
            return this;
        }

        /// <summary> Add a Progressor reference that will get updates when this SceneLoader loads a scene </summary>
        /// <param name="progressor"> The Progressor that will get updates when this SceneLoader loads a scene </param>
        public SceneLoader AddProgressor(Progressor progressor)
        {
            if (progressor == null) return this;
            progressors.RemoveNulls();
            if (progressors.Contains(progressor)) return this;
            progressors.Add(progressor);
            return this;
        }

        /// <summary> Remove a Progressor reference that would get updates when this SceneLoader loads a scene </summary>
        /// <param name="progressor"> The Progressor that would get updates when this SceneLoader loads a scene </param>
        public SceneLoader RemoveProgressor(Progressor progressor)
        {
            if (progressor == null) return this;
            progressors.RemoveNulls();
            if (!progressors.Contains(progressor)) return this;
            progressors.Remove(progressor);
            return this;
        }

        /// <summary> Clear the Progressor references that would get updates when this SceneLoader loads a scene </summary>
        public SceneLoader ClearProgressors()
        {
            progressors.Clear();
            return this;
        }


        /// <summary> Set the activation delay that determines how long will the SceneLoader wait, after a scene has been loaded, before it starts the scene activation process (works only if AllowSceneActivation is enabled) </summary>
        /// <param name="sceneActivationDelay"> How long will the SceneLoader wait, after a scene has been loaded, before it starts the scene activation process </param>
        public SceneLoader SetSceneActivationDelay(float sceneActivationDelay)
        {
            SceneActivationDelay = sceneActivationDelay;
            return this;
        }

        /// <summary> Set the SceneBuildIndex, in the Build Settings, of the Scene to load </summary>
        /// <param name="sceneBuildIndex"> Index, in the Build Settings, of the Scene to load </param>
        public SceneLoader SetSceneBuildIndex(int sceneBuildIndex)
        {
            SceneBuildIndex = sceneBuildIndex;
            return this;
        }

        /// <summary> Set the SceneName, name or path, of the Scene to load </summary>
        /// <param name="sceneName"> Name or path of the Scene to load </param>
        public SceneLoader SetSceneName(string sceneName)
        {
            SceneName = sceneName;
            return this;
        }

        /// <summary> Set this SceneLoader to self destruct (to destroy itself) after it loads a Scene </summary>
        /// <param name="selfDestruct"> Should this SceneLoader self destruct? </param>
        public SceneLoader SetSelfDestructAfterSceneLoaded(bool selfDestruct)
        {
            SelfDestructAfterSceneLoaded = selfDestruct;
            return this;
        }

        /// <summary> Sets the scene load Progress to zero </summary>
        private void ResetProgress()
        {
            progressors.RemoveNulls();
            progressors.ForEach(p => p.SetProgressAtZero());
            progress = 0;
        }

        private void StartSceneLoad()
        {
            ResetProgress();
            OnLoadScene?.Execute();
            currentState = State.LoadScene;
            currentAsyncOperation.allowSceneActivation = false; //update the scene activation mode
            m_LoadInProgress = true;                            //mark that a scene load process is running
            m_SceneLoadedAndReady = false;                      //mark that the scene has not been loaded (load progress has not reached 90%)
            m_ActivatingScene = false;
        }

        private IEnumerator AsynchronousLoad(string sceneName, LoadSceneMode mode)
        {
            // yield return null;
            ResetProgress();

            OnLoadScene?.Execute();

            currentAsyncOperation = SceneManager.LoadSceneAsync(sceneName, mode);

            if (currentAsyncOperation == null) yield break;

            currentAsyncOperation.allowSceneActivation = false; //update the scene activation mode
            m_LoadInProgress = true;                            //mark that a scene load process is running
            bool sceneLoadedAndReady = false;                   //mark that the scene has not been loaded (load progress has not reached 90%)
            bool activatingScene = false;

            // while (!currentAsyncOperation.isDone)
            while (m_LoadInProgress)
            {
                //if (currentAsyncOperation == null) yield break;

                // [0, 0.9] > [0, 1]
                progress = Mathf.Clamp01(currentAsyncOperation.progress / 0.9f); //update load progress

                if (debug && !activatingScene) Log($"Load progress: {Mathf.Round(progress * 100)}%");

                // Loading completed
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (!sceneLoadedAndReady && currentAsyncOperation.progress == 0.9f)
                {
                    // progress = 1f;

                    if (debug) Log($"Scene is ready to be activated.");

                    OnSceneLoaded.Execute();

                    sceneLoadedAndReady = true; //mark that the scene has been loaded and is now ready to be activated (bool needed to stop LoadBehavior.OnSceneLoaded.Invoke(gameObject) from executing more than once) 
                }

                if (sceneLoadedAndReady && !activatingScene)
                {
                    if (SceneActivationDelay < 0) SceneActivationDelay = 0; //sanity check
                    if (SceneActivationDelay > 0) yield return new WaitForSecondsRealtime(SceneActivationDelay);

                    if (AllowSceneActivation)
                    {
                        ActivateLoadedScene();
                        activatingScene = true;
                    }
                }

                if (currentAsyncOperation.isDone)
                {
                    if (debug) Log($"Scene has been activated.");
                    m_LoadInProgress = false;
                    // currentAsyncOperation = null;
                    if (SelfDestructAfterSceneLoaded) Coroutiner.Start(SelfDestruct());
                }

                yield return null;
            }
        }

        private IEnumerator AsynchronousLoad(int sceneBuildIndex, LoadSceneMode mode)
        {
            // yield return null;
            ResetProgress();
            OnLoadScene?.Execute();
            currentAsyncOperation = SceneManager.LoadSceneAsync(sceneBuildIndex, mode);

            if (currentAsyncOperation == null) yield break;

            currentAsyncOperation.allowSceneActivation = false; //update the scene activation mode
            m_LoadInProgress = true;                            //mark that a scene load process is running
            bool sceneLoadedAndReady = false;                   //mark that the scene has not been loaded (load progress has not reached 90%)
            bool activatingScene = false;

            // while (!currentAsyncOperation.isDone)
            while (m_LoadInProgress)
            {
                //if (currentAsyncOperation == null) yield break;

                // [0, 0.9] > [0, 1]
                progress = Mathf.Clamp01(currentAsyncOperation.progress / 0.9f); //update load progress
                if (debug && !activatingScene) Log($"Load progress: {Mathf.Round(progress * 100)}%");

                // Loading completed
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (!sceneLoadedAndReady && currentAsyncOperation.progress == 0.9f)
                {
                    // progress = 1f;
                    if (debug) Log($"Scene is ready to be activated.");
                    OnSceneLoaded?.Execute();
                    sceneLoadedAndReady = true; //mark that the scene has been loaded and is now ready to be activated (bool needed to stop LoadBehavior.OnSceneLoaded.Invoke(gameObject) from executing more than once) 
                }

                if (sceneLoadedAndReady && !activatingScene && AllowSceneActivation)
                {
                    if (SceneActivationDelay < 0) SceneActivationDelay = 0; //sanity check
                    if (SceneActivationDelay > 0) yield return new WaitForSecondsRealtime(SceneActivationDelay);
                    ActivateLoadedScene();
                    activatingScene = true;
                }

                if (currentAsyncOperation.isDone)
                {
                    if (debug) Log("[" + name + "] Scene has been activated.");
                    m_LoadInProgress = false;
                    // currentAsyncOperation = null;

                    if (SelfDestructAfterSceneLoaded)
                    {
                        Coroutiner.Start(SelfDestruct());
                    }
                }

                yield return null;
            }
        }

        private IEnumerator SelfDestruct()
        {
            yield return null;
            Destroy(gameObject);
        }

        /// <summary>
        ///  Activates all the loaded scenes for all the SceneLoaders that have scenes ready to be activated.
        ///  <para/>
        ///  A scene is ready to be activated if the load progress is at 0.9 (90%).
        /// </summary>
        public static void ActivateLoadedScenes()
        {
            if (settings.DebugMode) Log($"Activate Loaded Scenes", null);

            database.Remove(null);
            foreach (SceneLoader sceneLoader in database)
                sceneLoader.ActivateLoadedScene();
        }

        /// <summary> Creates a new GameObject with a SceneLoader script attached and then returns the reference to the newly created script </summary>
        /// <param name="parent"> Sets a parent for the newly created GameObject </param>
        public static SceneLoader GetLoader(Transform parent = null)
        {
            SceneLoader loader = new GameObject(nameof(SceneLoader)).AddComponent<SceneLoader>();
            if (parent != null)
            {
                loader.transform.SetParent(parent);
                return loader;
            }
            DontDestroyOnLoad(loader); //make sure this game object is not destroyed when loading a new scene
            return loader;
        }

        private void Log(string message) =>
            Log($"[{name}] {message}", this);

        private static void Log(string message, Object context) =>
            Debugger.Log($"({nameof(SceneLoader)}) {message}", context);
    }
}
