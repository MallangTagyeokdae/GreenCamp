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
    /// <summary> Link between a UITooltip prefab and the UITooltip database </summary>
    [CreateAssetMenu(menuName = "Doozy/Links/UITooltip Link", fileName = "UITooltip Link", order = -900)]
    [Serializable]
    public class UITooltipLink : PrefabLink
    {
        private const string PREFIX = "UITooltip - ";

        public UITooltipLink() : this(null) {}

        public UITooltipLink(GameObject prefab, string prefabName = null) : base(prefab, prefabName) {}

        public override void Validate()
        {
            if (!hasPrefab)
            {
                prefabName = string.Empty;
                name = nameof(UIPopupLink);
                UITooltipDatabase.instance.Remove(this);
                return;
            }

            if (prefabName.Equals(UITooltip.k_DefaultTooltipName))
            {
                UITooltipDatabase.instance.Remove(this);
                Debug.LogError
                (
                    $"[{nameof(UITooltipLink)}]: [{prefabName}] - The prefabName cannot be the same as the default tooltip name ({UITooltip.k_DefaultTooltipName})." +
                    $"Rename the prefab to something else."
                );
                return;
            }

            bool save = false;

            if (!prefabName.Equals(prefab.name))
            {
                prefabName = prefab.name.RemoveWhitespaces().RemoveAllSpecialCharacters();
                save = true;
            }

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
