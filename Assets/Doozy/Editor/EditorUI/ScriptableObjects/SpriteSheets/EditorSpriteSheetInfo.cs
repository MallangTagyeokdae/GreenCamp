// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Doozy.Editor.Common.Extensions;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Common.Extensions;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.EditorUI.ScriptableObjects.SpriteSheets
{
    [Serializable]
    public class EditorSpriteSheetInfo
    {
        public string SheetName;
        public Texture2D SheetReference;

        public List<Sprite> Sprites;

        public EditorDataSpriteSheetGroup Group;
        public IEnumerable<Texture2D> textures => Group.GetTextures(SheetName);

        public EditorSpriteSheetInfo ValidateName()
        {
            SheetName =
                SheetReference == null
                    ? string.Empty
                    : SheetName.IsNullOrEmpty()
                        ? SheetReference.name
                        : SheetName;

            SheetName = SheetName.RemoveWhitespaces().RemoveAllSpecialCharacters();
            return this;
        }

        public IEnumerable<Texture2D> GetSpritesAsTextures() =>
            Sprites.ToTexture2D();

        /// <summary> Process the sheet by extracting all the contained sprites and convert them into list of textures </summary>
        public EditorSpriteSheetInfo LoadSpritesFromSheet()
        {
            if (SheetReference == null)
            {
                Debug.LogWarning($"Cannot extract textures. {nameof(SheetReference)} is null");
                return this;
            }
            #if UNITY_EDITOR
            {
                string assetPath = AssetDatabase.GetAssetPath(SheetReference); //get sheet asset path
                TextureUtils.SetTextureSettingsToSpriteSheet(assetPath);       //validate settings
                if (Sprites == null)                                           //extract sprites
                {
                    Sprites = new List<Sprite>(AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath).OfType<Sprite>());
                }
                else
                {
                    Sprites.Clear();
                    Sprites.AddRange(AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath).OfType<Sprite>());
                }

                //FIX NAMING
                {
                    string assetMetaPath = assetPath + ".meta";
                    string metaFile = File.ReadAllText(assetMetaPath);
                    if (!metaFile.Contains($"~{SheetReference.name}_{000}")) //don't process if the first sprite is correct
                    {
                        var oldSpriteNames = new Dictionary<string, string>();
                        var newSpriteNames = new Dictionary<string, string>();
                        for (int i = 0; i < Sprites.Count; i++)
                        {
                            Sprite sprite = Sprites[i];
                            string tempName = $"{Guid.NewGuid()}";
                            while (oldSpriteNames.ContainsKey(tempName))
                            {
                                tempName = $"{Guid.NewGuid()}";
                            }
                            oldSpriteNames.Add(tempName, sprite.name);
                            newSpriteNames.Add(tempName, $"~{SheetReference.name}_{i:000}");
                        }
                        //replace old name with temp name
                        metaFile = oldSpriteNames.Keys.Aggregate(metaFile, (current, key) => current.Replace(oldSpriteNames[key], key));
                        // replace temp name with new name
                        metaFile = newSpriteNames.Keys.Aggregate(metaFile, (current, key) => current.Replace(key, newSpriteNames[key]));
                        // in case of hidden meta files -> remove hidden attribute to execute operation
                        FileAttributes originalFileAttributes = File.GetAttributes(assetMetaPath);
                        File.SetAttributes(assetMetaPath, originalFileAttributes & ~FileAttributes.Hidden);
                        File.WriteAllText(assetMetaPath, metaFile);
                        File.SetAttributes(assetMetaPath, originalFileAttributes);
                        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                    }
                }
            }
            #endif
            return this;
        }
    }
}
