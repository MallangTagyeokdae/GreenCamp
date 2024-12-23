// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.EditorUI.Windows.Internal
{
    public abstract class FluidWindow<T> : EditorWindow where T : EditorWindow
    {
        protected string EditorPrefsKey(string variableName) => $"{GetType().FullName} - {variableName}";

        protected PlayModeStateChange currentPlayModeState { get; private set; } = PlayModeStateChange.EnteredEditMode;

        protected VisualElement root => rootVisualElement;
        protected VisualElement windowLayout { get; set; }

        // public static bool isOpen { get; private set; }
        public static bool isOpen => HasOpenInstances<T>();

        #region Instance

        private static T s_instance;

        public static T instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                s_instance = window;
                if (s_instance != null) return s_instance;
                s_instance = GetWindow<T>();
                return s_instance;
            }
        }

        /*
       * An alternative way to get Window, because
       * GetWindow<T>() forces window to be active and present
       */
        private static T window
        {
            get
            {
                T[] windows = Resources.FindObjectsOfTypeAll<T>();
                return windows.Length > 0 ? windows[0] : null;
            }
        }

        #endregion

        protected static void InternalOpenWindow(string windowTitle)
        {
            instance.Show();
            instance.titleContent.text = windowTitle;
        }

        protected virtual void Awake() {}

        protected virtual void OnEnable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        protected virtual void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        protected virtual void OnDestroy() {}

        protected virtual void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            // Debugger.Log($"(FluidWindow).{nameof(OnPlayModeStateChanged)}({state})");

            currentPlayModeState = state;
            switch (state)
            {
                case PlayModeStateChange.EnteredEditMode:
                case PlayModeStateChange.EnteredPlayMode:
                {
                    OnDestroy();
                    root.RecycleAndClear();
                    CreateGUI();
                    break;
                }
                case PlayModeStateChange.ExitingEditMode:
                case PlayModeStateChange.ExitingPlayMode:
                {
                    //ignored
                    break;
                }
            }
        }

        protected abstract void CreateGUI();
    }
}
