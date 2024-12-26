// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.UIManager.Containers;
using UnityEngine;

namespace Doozy.Runtime.UIManager.ScriptableObjects
{
    /// <summary> Link between a UIPopup prefab and the UIPopup database </summary>
    [CreateAssetMenu(menuName = "Doozy/Links/UIPopup Link", fileName = "UIPopup Link", order = -900)]
    [Serializable]
    public class UIPopupLink : PrefabLink
    {
        private const string PREFIX = "UIPopup - ";

        public UIPopupLink() : this(null) {}

        public UIPopupLink(GameObject prefab, string prefabName = null) : base(prefab, prefabName) {}

        public override void Validate()
        {
            if (!hasPrefab)
            {
                prefabName = string.Empty;
                name = nameof(UIPopupLink);
                UIPopupDatabase.instance.Remove(this);
                return;
            }

            if (prefabName.Equals(UIPopup.k_DefaultPopupName))
            {
                UIPopupDatabase.instance.Remove(this);
                Debug.LogError
                (
                    $"[{nameof(UIPopupLink)}]: [{prefabName}] - The prefabName cannot be the same as the default popup name ({UIPopup.k_DefaultPopupName}). " +
                    $"Rename the prefab to something else."
                );
                return;
            }

            bool save = false;

            //if the prefab name is not set, set it to the prefab name
            if (!prefabName.Equals(prefab.name))
            {
                prefabName = prefab.name.RemoveWhitespaces().RemoveAllSpecialCharacters();
                save = true;
            }

            //if the prefab name is not set, set it to the prefab name
            if (!name.Equals(PREFIX + prefab.name))
            {
                name = PREFIX + prefab.name;
                #if UNITY_EDITOR
                {
                    UnityEditor.AssetDatabase.RenameAsset(UnityEditor.AssetDatabase.GetAssetPath(this), name);
                }
                #endif
                save = true;
            }

            if (!save) return;
            #if UNITY_EDITOR
            {
                if (this == null) return;
                if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) return;
                UnityEditor.EditorUtility.SetDirty(this);
                UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
            }
            #endif
        }
    }
}
