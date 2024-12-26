// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.Common.ScriptableObjects;
using UnityEditor;

namespace Doozy.Editor.Common.Addons
{
    public class AddonInfoDatabase : SingletonEditorScriptableObject<AddonInfoDatabase>
    {
        public List<AddonInfo> Database = new List<AddonInfo>();

        /// <summary> Refresh the database by searching for all AddonInfo assets in the project </summary>
        public static void RefreshDatabase()
        {
            instance.Database ??= new List<AddonInfo>();
            instance.Database.Clear(); 
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(AddonInfo)}");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                AddonInfo addonInfo = AssetDatabase.LoadAssetAtPath<AddonInfo>(path);
                if (addonInfo != null) instance.Database.Add(addonInfo);
            }
            instance.Database =
                instance.Database
                    .OrderBy(item => item.AddonCategory)
                    .ThenBy(item => item.AddonName)
                    .ToList();
            EditorUtility.SetDirty(instance);
            AssetDatabase.SaveAssetIfDirty(instance);
        }
    }
}
