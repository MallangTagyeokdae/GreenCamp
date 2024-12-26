// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.Common.Extensions;
using Doozy.Editor.EditorUI.ScriptableObjects.SpriteSheets;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Common.Extensions;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.UIManager.UIMenu
{
    [
        CreateAssetMenu
        (
            fileName = DEFAULT_ASSET_FILENAME,
            menuName = "Doozy/UI Menu Item",
            order = -500
        )
    ]
    public class UIMenuItem : ScriptableObject
    {
        private const string DEFAULT_ASSET_FILENAME = "Menu Item";

        public string cleanAssetName => 
            $"{DEFAULT_ASSET_FILENAME} - {PrefabTypeName} - {PrefabCategory} - {PrefabName}";

        [SerializeField] private GameObject Prefab;
        public GameObject prefab
        {
            get => Prefab;
            set => Prefab = value;
        }

        [SerializeField] private UIPrefabType PrefabType;
        public UIPrefabType prefabType
        {
            get => PrefabType;
            set
            {
                PrefabType = value;
                if (PrefabType != UIPrefabType.Custom)
                    PrefabTypeName = value.ToString();
            }
        }

        [SerializeField] private string PrefabTypeName;
        public string prefabTypeName
        {
            get => PrefabType == UIPrefabType.Custom ? PrefabTypeName : PrefabType.ToString();
            set
            {
                PrefabType = UIPrefabType.Custom;
                PrefabTypeName = value;
            }
        }

        [SerializeField] private string PrefabCategory;
        public string prefabCategory
        {
            get => PrefabCategory;
            set => PrefabCategory = value;
        }

        [SerializeField] private string PrefabName;
        public string prefabName
        {
            get => PrefabName;
            set => PrefabName = value;
        }

        [SerializeField] private PrefabInstantiateMode InstantiateMode;
        public PrefabInstantiateMode instantiateMode
        {
            get => InstantiateMode;
            set => InstantiateMode = value;
        }

        [SerializeField] private bool LockInstantiateMode;
        public bool lockInstantiateMode
        {
            get => LockInstantiateMode;
            set => LockInstantiateMode = value;
        }

        [SerializeField] private bool Colorize = false;
        public bool colorize
        {
            get => Colorize;
            set => Colorize = value;
        }

        [SerializeField] private float AnimationDuration = 0.6f;
        public float animationDuration
        {
            get => AnimationDuration;
            set => AnimationDuration = value;
        }

        [SerializeField] private List<string> Tags;
        public List<string> tags
        {
            get => Tags;
            set => Tags = value;
        }

        [SerializeField] private string InfoTag;
        public string infoTag
        {
            get => InfoTag;
            set => InfoTag = value;
        }

        [SerializeField] private List<Texture2D> Icon;
        public List<Texture2D> icon
        {
            get => Icon;
            set => Icon = value;
        }

        [SerializeField] private Texture2D SpriteSheet;
        public Texture2D spriteSheet => SpriteSheet;

        [SerializeField] private EditorDataSpriteSheetTextures SpriteSheetTextures;
        public EditorDataSpriteSheetTextures spriteSheetTextures
        {
            get => SpriteSheetTextures;
            internal set => SpriteSheetTextures = value;
        }

        public bool hasSpriteSheet => 
            spriteSheet != null && spriteSheet.IsSpriteSheet();

        public void ProcessSpriteSheet()
        {
            if (!hasSpriteSheet) return;
            Icon = spriteSheetTextures.textures;
        }

        public string cleanPrefabTypeName => PrefabType != UIPrefabType.Custom ? PrefabType.ToString() : PrefabTypeName.RemoveWhitespaces().RemoveAllSpecialCharacters();
        public string cleanPrefabCategory => PrefabCategory.RemoveWhitespaces().RemoveAllSpecialCharacters();
        public string cleanPrefabName => PrefabName.RemoveWhitespaces().RemoveAllSpecialCharacters();

        public bool isValid => hasPrefab && hasPrefabType && hasCategory && hasName;
        public bool hasPrefabType => PrefabType != UIPrefabType.Custom || !prefabTypeName.IsNullOrEmpty();
        public bool hasPrefab => Prefab != null;
        public bool hasCategory => !PrefabCategory.IsNullOrEmpty();
        public bool hasName => !PrefabName.IsNullOrEmpty();
        public bool hasIcon => Icon != null && Icon.Where(item => item != null).ToList().Count == 1;
        public bool hasAnimatedIcon => Icon != null && Icon.Where(item => item != null).ToList().Count > 1;
        public bool hasInfoTag => !InfoTag.IsNullOrEmpty();

        public UIMenuItem()
        {
            prefabType = UIPrefabType.Components;
        }

        public UIMenuItem Validate()
        {
            if (prefabType == UIPrefabType.Custom)
            {
                switch (PrefabTypeName)
                {
                    case "Container":
                        prefabType = UIPrefabType.Containers;
                        PrefabTypeName = UIPrefabType.Containers.ToString();
                        EditorUtility.SetDirty(this);
                        break;
                    case "Component":
                        prefabType = UIPrefabType.Components;
                        PrefabTypeName = UIPrefabType.Components.ToString();
                        EditorUtility.SetDirty(this);
                        break;
                }
            }
            
            if (name.Equals(cleanAssetName))
                return this;

            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(this), $"{cleanAssetName}.asset");
            EditorUtility.SetDirty(this);

            return this;
        }

        public UIMenuItem SortSpritesAz()
        {
            Icon = Icon.OrderBy(item => item.name).ToList();
            return this;
        }

        public UIMenuItem SortSpritesZa()
        {
            Icon = Icon.OrderByDescending(item => item.name).ToList();
            return this;
        }

        public UIMenuItem SortTagsAz()
        {
            Tags = Tags.OrderBy(tag => tag).ToList();
            return this;
        }

        public UIMenuItem SortTagsZa()
        {
            Tags = Tags.OrderByDescending(tag => tag).ToList();
            return this;
        }

        public void AddToScene()
        {
            UIMenuUtils.AddToScene(this);
        }

        public void SetSpriteSheet(Texture2D texture)
        {
            EditorDataSpriteSheetTextures sheet =
                CreateInstance<EditorDataSpriteSheetTextures>()
                    .SetTextures(texture.GetTextures());

            SpriteSheet = texture;
            sheet.name = texture.name;
            spriteSheetTextures = sheet;
            EditorUtility.SetDirty(sheet);
            AssetDatabase.AddObjectToAsset(sheet, this);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(sheet);
            AssetDatabase.SaveAssetIfDirty(this);
            ProcessSpriteSheet();            
        }
    }
}
