// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Doozy.Editor.Common.Utils;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Extensions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
namespace Doozy.Editor.EditorUI.ScriptableObjects.SpriteSheets
{
    [
        CreateAssetMenu
        (
            fileName = DEFAULT_ASSET_FILENAME,
            menuName = "Doozy/EditorUI/SpriteSheet Group",
            order = -497
        )
    ]
    public class EditorDataSpriteSheetGroup : ScriptableObject
    {
        private const string DEFAULT_ASSET_FILENAME = "_SpriteSheetGroup";

        [SerializeField] private string GroupCategory = "General";
        internal string groupCategory => GroupCategory;

        [SerializeField] private string GroupName;
        internal string groupName => GroupName;

        [SerializeField] private string RemoveStringFromName = "Sheet";
        internal string removeStringFromName => RemoveStringFromName;

        [SerializeField]
        private List<EditorSpriteSheetInfo> Sheets = new List<EditorSpriteSheetInfo>();
        internal List<EditorSpriteSheetInfo> sheets => Sheets;

        [SerializeField]
        private List<EditorDataSpriteSheetTextures> TextureSheets;


        internal void AddNewItem() =>
            Sheets.Insert(0, new EditorSpriteSheetInfo());

        internal void RemoveNullEntries() =>
            Sheets = Sheets.Where(item => item != null && item.SheetReference != null).ToList();

        internal void SortByFileName() =>
            Sheets = Sheets.OrderBy(item => item.SheetReference.name).ToList();

        internal void RemoveDuplicates() =>
            Sheets = Sheets.GroupBy(item => item.SheetReference).Select(item => item.First()).ToList();


        private void RefreshTextureSheets()
        {
            TextureSheets = new List<EditorDataSpriteSheetTextures>();
            foreach (Object asset in AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(this)))
                if (asset is EditorDataSpriteSheetTextures textureSheet)
                    TextureSheets.Add(textureSheet);
        }

        internal List<Texture2D> GetTextures(string sheetName)
        {
            EditorDataSpriteSheetTextures reference = GetSheetReference(sheetName);
            return reference != null ? reference.textures : null;
        }
        
        public EditorDataSpriteSheetTextures GetSheetReference(string sheetName)
        {
            if (TextureSheets == null || TextureSheets.Count == 0) RefreshTextureSheets();
            string cleanName = sheetName.RemoveWhitespaces().RemoveAllSpecialCharacters();
            foreach (EditorDataSpriteSheetTextures reference in TextureSheets)
                if (reference.SheetInfo.SheetName.Equals(cleanName))
                    return reference;
            
            Debug.LogWarning($"SpriteSheet '{sheetName}' not found! Returned null.");
            return null;
        }

        private void RemoveSubAssets() =>
            AssetUtils.RemoveSubAssets(this);

        public void LoadSpriteSheetsFromFolder(bool saveAssets = true)
        {
            Sheets.Clear();
            if (name.IsNullOrEmpty())
            {
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssetIfDirty(this);
            }
            string assetParentFolderPath = AssetDatabase.GetAssetPath(this).Replace($"{name}.asset", "");
            string[] files = Directory.GetFiles(assetParentFolderPath, "*.png", SearchOption.TopDirectoryOnly);

            if (files.Length == 0)
            {
                // AssetDatabase.MoveAssetToTrash(assetPath);
                return;
            }

            TextureUtils.SetTextureSettingsToSpriteSheet(files);
            List<Texture2D> textures = TextureUtils.GetTextures2D(assetParentFolderPath);
            foreach (Texture2D sheetReference in textures)
            {
                var sheet = new EditorSpriteSheetInfo();
                sheet.SheetReference = sheetReference;
                Sheets.Add(sheet);
            }

            Validate();

            Debugger.Log($"Found the '{groupCategory} > {groupName}' SpriteSheet Group ({sheets.Count} sheets)");
        }

        internal void Validate()
        {
            string path = AssetDatabase.GetAssetPath(this);
            string[] splitPath = path.Split('/');
            string folderName = splitPath[splitPath.Length - 2];

            GroupCategory =
                GroupCategory.IsNullOrEmpty()
                    ? "General"
                    : GroupCategory.RemoveWhitespaces().RemoveAllSpecialCharacters();

            GroupName = folderName.RemoveWhitespaces().RemoveAllSpecialCharacters();

            AssetDatabase.RenameAsset(path, $"{DEFAULT_ASSET_FILENAME}_{GroupName}_{groupCategory}");

            Sheets = Sheets ?? new List<EditorSpriteSheetInfo>();

            RemoveNullEntries();
            RemoveDuplicates();

            if (TextureSheets == null)
            {
                TextureSheets = new List<EditorDataSpriteSheetTextures>();
            }
            else
            {
                TextureSheets.Clear();
            }

            RemoveSubAssets();

            foreach (EditorSpriteSheetInfo sheetInfo in Sheets)
            {
                string fileName = sheetInfo.SheetReference.name;
                if (!RemoveStringFromName.IsNullOrEmpty())
                    fileName = fileName.Replace(RemoveStringFromName, "");
                sheetInfo.SheetName = fileName;
                sheetInfo.Group = this;
                sheetInfo.ValidateName();
                sheetInfo.LoadSpritesFromSheet();

                var sheetTextures = sheetInfo.GetSpritesAsTextures().ToList();
                for (int i = 0; i < sheetTextures.Count; i++)
                {
                    Texture2D sheetTexture = sheetTextures[i];
                    sheetTexture.name = sheetInfo.Sprites[i].name;
                }

                EditorDataSpriteSheetTextures textureSheet =
                    CreateInstance<EditorDataSpriteSheetTextures>()
                        .SetInfo(sheetInfo)
                        .SetTextures(sheetTextures);

                EditorUtility.SetDirty(textureSheet);
                AssetDatabase.AddObjectToAsset(textureSheet, this);
                TextureSheets.Add(textureSheet);
            }

            SetDirtyAndSave();
        }

        public EditorDataSpriteSheetGroup SetDirtyAndSave()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
            return this;
        }
      
    }
}
