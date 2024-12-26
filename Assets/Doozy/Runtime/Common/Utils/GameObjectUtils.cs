// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Common.Extensions;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.Common.Utils
{
    public static class GameObjectUtils
    {
        /// <summary> Adds a new GameObject with the attached MonoBehavior of type T </summary>
        /// <param name="isSingleton"> If TRUE, it will check if there isn't another GameObject with the MonoBehavior attached. If there is, it will select it (Editor only)</param>
        /// <param name="selectGameObjectAfterCreation"> If TRUE, after creating a new GameObject, it will get automatically selected (Editor only)</param>
        /// <typeparam name="T"> MonoBehaviour </typeparam>
        public static T AddToScene<T>(bool isSingleton, bool selectGameObjectAfterCreation) where T : MonoBehaviour =>
            AddToScene<T>(ObjectNames.NicifyVariableName(typeof(T).Name), isSingleton, selectGameObjectAfterCreation);

        /// <summary> Adds a new GameObject with the attached MonoBehavior of type T </summary>
        /// <param name="gameObjectName"> The name of the newly created GameObject </param>
        /// <param name="isSingleton"> If TRUE, it will check if there isn't another GameObject with the MonoBehavior attached. If there is, it will select it (Editor only)</param>
        /// <param name="selectGameObjectAfterCreation"> If TRUE, after creating a new GameObject, it will get automatically selected (Editor only)</param>
        /// <typeparam name="T"> MonoBehaviour </typeparam>
        public static T AddToScene<T>(string gameObjectName, bool isSingleton, bool selectGameObjectAfterCreation) where T : MonoBehaviour
        {
            T component = Object.FindObjectOfType<T>();
            if (component != null && isSingleton)
            {
                // Debugger.Log($"Cannot add another <{typeof(T).Name}> to this Scene because it's a singleton and you should not have more than one.");
                #if UNITY_EDITOR
                UnityEditor.Selection.activeObject = component;
                #endif
                return component;
            }

            component = new GameObject(gameObjectName, typeof(T)).GetComponent<T>();
            GameObject go = component.gameObject;

            #if UNITY_EDITOR
            if
            (
                !isSingleton &&
                UnityEditor.Selection.activeObject != null &&
                UnityEditor.Selection.activeObject is GameObject parent
            )
            {
                UnityEditor.GameObjectUtility.SetParentAndAlign(go, parent.gameObject);
                RectTransform parentRT = parent.GetComponent<RectTransform>();
                if (parentRT != null)
                {
                    RectTransform childRT = go.GetComponent<RectTransform>();
                    if (childRT == null) childRT = go.AddComponent<RectTransform>();
                    childRT
                        .ResetLocalScaleToOne()
                        .ResetLocalPosition()
                        .ResetAnchoredPosition3D();
                }
            }

            UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Created " + gameObjectName);
            if (selectGameObjectAfterCreation) UnityEditor.Selection.activeObject = go;
            #endif
            return component;
        }
    }
}
