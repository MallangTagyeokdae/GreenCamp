// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Common.ScriptableObjects;
using UnityEngine;

namespace Doozy.Runtime.UIManager.ScriptableObjects.Internal
{
    [Serializable]
    public abstract class PrefabLinkDatabase<Tdatabase, TprefabLink> : SingletonRuntimeScriptableObject<Tdatabase> where Tdatabase : SingletonRuntimeScriptableObject<Tdatabase> where TprefabLink : PrefabLink
    {
        [SerializeField] private List<TprefabLink> Database = new List<TprefabLink>();
        public List<TprefabLink> database => Database;

        public abstract string defaultLinkName { get; }
        public abstract string databaseName { get; }

        /// <summary>
        /// Get all the registered links names in the database.
        /// First name will always be the default name 'None'.
        /// </summary>
        /// <returns> List of links names </returns>
        public List<string> GetAllNames()
        {
            var names = new List<string>() { defaultLinkName };
            // Validate();
            // Sort();
            names.AddRange(Database.Select(link => link.prefabName.RemoveWhitespaces().RemoveAllSpecialCharacters()));
            return names;
        }

        /// <summary> Check it the database contains the given link </summary>
        /// <param name="link"> Link to check </param>
        /// <returns> TRUE if the database contains the given link, FALSE otherwise </returns>
        public bool Contains(TprefabLink link) =>
            Database.RemoveNulls().Contains(link);

        /// <summary> Check if the database contains a link with the given prefabName </summary>
        /// <param name="prefabName"> Name of the prefab </param>
        /// <returns> TRUE if the database contains a link with the given prefabName, FALSE otherwise </returns>
        public bool Contains(string prefabName) =>
            Database.RemoveNulls().Any(x => x.prefabName.Equals(prefabName));

        /// <summary> Check if the database contains a link with the given prefab reference </summary>
        /// <param name="prefab"> Reference to the prefab </param>
        /// <returns> TRUE if the database contains a link with the given prefab reference, FALSE otherwise </returns>
        public bool Contains(GameObject prefab) =>
            Database.RemoveNulls().Any(x => x.prefab == prefab);

        /// <summary> Add a new link to the database with the given prefab and prefabName </summary>
        /// <param name="link"> Link to add to the database </param>
        /// <returns> TRUE if the database was updated, FALSE if the database already contains a link with the given prefabName </returns>
        public bool Add(TprefabLink link)
        {
            if (link == null) return false;
            link.Validate();
            if (Contains(link)) return false;

            if (!link.hasPrefab)
            {
                // Debug.Log($"Cannot add a link with a null prefab reference to the {databaseName} database.");
                Remove(link);
                return false;
            }

            if (!link.hasPrefabName)
            {
                // Debug.Log($"Cannot add a link with a null or empty prefabName to the {databaseName} database.");
                Remove(link);
                return false;
            }

            if (Contains(link.prefab))
            {
                Debug.Log($"{databaseName} database already contains a link with the given prefab reference. Link not added.");
                return false;
            }

            if (Contains(link.prefabName))
            {
                Debug.Log($"{databaseName} database already contains a link with the given '{link.prefabName}' prefabName. Link not added.");
                return false;
            }

            Database.Add(link);
            Save();
            return true;
        }

        /// <summary> Remove a link from the database </summary>
        /// <param name="link"> Link to remove from the database </param>
        /// <returns> TRUE if the link was removed, FALSE if the link was not found in the database </returns>
        public bool Remove(TprefabLink link)
        {
            if (link == null) return false;
            if (!Contains(link)) return false;
            Database.Remove(link);
            #if UNITY_EDITOR
            {
                UnityEditor.AssetDatabase.MoveAssetToTrash(UnityEditor.AssetDatabase.GetAssetPath(link));
                Save();
            }
            #endif
            return true;
        }

        /// <summary> Remove a link from the database with the given prefabName </summary>
        /// <param name="prefabName"> Name of the prefab </param>
        /// <returns> TRUE if the database was updated, FALSE if the database does not contain a link with the given prefabName </returns>
        public bool Remove(string prefabName)
        {
            prefabName = prefabName.RemoveWhitespaces().RemoveAllSpecialCharacters();
            if (string.IsNullOrEmpty(prefabName)) return false;
            TprefabLink link = null;
            foreach (TprefabLink item in Database)
            {
                if (!item.prefabName.Equals(prefabName)) continue;
                link = item;
                break;
            }
            if (link == null) return false;
            Database.Remove(link);
            #if UNITY_EDITOR
            {
                UnityEditor.AssetDatabase.MoveAssetToTrash(UnityEditor.AssetDatabase.GetAssetPath(link));
                Save();
            }
            #endif
            return true;
        }

        /// <summary> Remove a link from the database with the given prefab reference </summary>
        /// <param name="prefab"> Reference to the prefab </param>
        /// <returns> TRUE if the database was updated, FALSE if the database does not contain a link with the given prefab reference </returns>
        public bool Remove(GameObject prefab)
        {
            if (prefab == null) return false;
            if (!Contains(prefab)) return false;
            TprefabLink link = null;
            foreach (TprefabLink item in Database)
            {
                if (item.prefab != prefab) continue;
                link = item;
                break;
            }
            if (link == null) return false;
            Database.Remove(link);
            #if UNITY_EDITOR
            {
                UnityEditor.AssetDatabase.MoveAssetToTrash(UnityEditor.AssetDatabase.GetAssetPath(link));
                Save();
            }
            #endif
            return true;
        }

        /// <summary> Remove the given link from the database and then delete the asset file from the project </summary>
        /// <param name="link"> Link to delete </param>
        /// <returns> TRUE if the operation was successful, FALSE otherwise </returns>
        public bool Delete(TprefabLink link)
        {
            if (link == null) return false;
            if (!Contains(link)) return false;
            Database.Remove(link);
            #if UNITY_EDITOR
            {
                UnityEditor.AssetDatabase.MoveAssetToTrash(UnityEditor.AssetDatabase.GetAssetPath(link));
                Save();
            }
            #endif
            return true;
        }

        /// <summary>
        /// Get the prefab reference of a link with the given prefabName.
        /// If the prefabName is not found, it will return null.
        /// <param name="tooltipName"> Name of the tooltip prefab </param>
        /// </summary>
        public GameObject GetPrefab(string tooltipName)
        {
            if (tooltipName.IsNullOrEmpty()) return null;
            if (tooltipName.Equals(defaultLinkName)) return null;
            tooltipName = tooltipName.RemoveWhitespaces().RemoveAllSpecialCharacters();
            foreach (TprefabLink t in Database)
                if (t.prefabName.Equals(tooltipName))
                    return t.prefab;
            return null;
        }


        /// <summary> Refresh the database </summary>
        /// <param name="saveAssets"> Write all unsaved asset changes to disk? </param>
        /// <param name="refreshAssetDatabase"> Scan and load all modified assets? </param>
        public void RefreshDatabase(bool saveAssets = true, bool refreshAssetDatabase = false)
        {
            #if UNITY_EDITOR
            {
                string title = $"{databaseName} - Refreshing Database";
                UnityEditor.EditorUtility.DisplayProgressBar(title, "Initializing...", 0f);
                Database.Clear();
                UnityEditor.EditorUtility.DisplayProgressBar(title, $"Searching for {typeof(TprefabLink).Name} files...", 0.1f);

                //FIND the GUIDs for all ScriptableObjects of the given type
                string[] guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(TprefabLink).Name}");

                //PROCESS ALL FOUND ASSETS (validate & add) 
                int foundCount = guids.Length;
                for (int i = 0; i < foundCount; i++)
                {
                    string guid = guids[i];
                    string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                    UnityEditor.EditorUtility.DisplayProgressBar(title, $"{assetPath}", 0.2f + 0.6f * ((i + 1f) / foundCount));
                    Add(UnityEditor.AssetDatabase.LoadAssetAtPath<TprefabLink>(assetPath));
                }

                UnityEditor.EditorUtility.DisplayProgressBar(title, "Validate and Sort...", 0.8f);
                Validate();

                //MARK DATABASE as DIRTY
                UnityEditor.EditorUtility.SetDirty(instance);

                UnityEditor.EditorUtility.DisplayProgressBar(title, "Saving...", 1f);
                if (saveAssets) Save();
                if (refreshAssetDatabase) UnityEditor.AssetDatabase.Refresh();
                UnityEditor.EditorUtility.ClearProgressBar();
            }
            #endif
        }

        /// <summary> Validate the database by removing invalid items </summary>
        public Tdatabase Validate()
        {
            // Debug.Log($"Validating {databaseName}...");

            bool save = false;
            for (int i = Database.Count - 1; i >= 0; i--)
            {
                TprefabLink link = Database[i];
                if
                (
                    link != null &&                   //link is not null
                    link.prefab != null &&            //link prefab is not null
                    !link.prefabName.IsNullOrEmpty()) //link prefab name is not null or empty
                {
                    link.Validate(); //validate the link
                    continue;        //continue to the next link
                }
                Database.RemoveAt(i);
                save = true;
            }
            
            Sort();
            
            if (save) Save();
            return instance;
        }

        /// <summary> Sort the database by prefabName. </summary>
        public Tdatabase Sort()
        {
            Database.Sort((x, y) => string.Compare(x.prefabName, y.prefabName, StringComparison.Ordinal));
            return instance;
        }

        public Tdatabase Save()
        {
            #if UNITY_EDITOR
            {
                UnityEditor.EditorUtility.SetDirty(instance);
                UnityEditor.AssetDatabase.SaveAssetIfDirty(instance);
            }
            #endif
            return instance;
        }

        public Tdatabase SaveAndRefresh()
        {
            #if UNITY_EDITOR
            {
                Save();
                UnityEditor.AssetDatabase.Refresh();
            }
            #endif
            return instance;
        }
    }
}
