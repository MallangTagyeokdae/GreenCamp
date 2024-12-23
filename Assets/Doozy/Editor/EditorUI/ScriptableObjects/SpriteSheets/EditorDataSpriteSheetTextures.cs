// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.EditorUI.ScriptableObjects.SpriteSheets
{
    public class EditorDataSpriteSheetTextures : ScriptableObject
    {
        public EditorSpriteSheetInfo SheetInfo;
        public List<TextureData> Data;

        private List<Texture2D> m_Textures;
        public List<Texture2D> textures =>
            m_Textures != null && m_Textures.Count > 0 && m_Textures[0] != null
                ? m_Textures
                : GetTextures();

        public EditorDataSpriteSheetTextures SetInfo(EditorSpriteSheetInfo info)
        {
            SheetInfo = info;
            name = info.SheetName;
            return this;
        }

        public EditorDataSpriteSheetTextures SetTextures(List<Texture2D> source)
        {
            if (Data == null)
            {
                Data = new List<TextureData>();
            }
            else
            {
                Data.Clear();
            }

            foreach (Texture2D texture in source)
                Data.Add(new TextureData(texture));

            return this;
        }


        private List<Texture2D> GetTextures()
        {
            m_Textures = new List<Texture2D>();

            foreach (TextureData data in Data)
                m_Textures.Add(data.GetTexture());

            return m_Textures;
        }

        public EditorDataSpriteSheetTextures SetDirtyAndSave()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
            return this;
        }

        [Serializable]
        public struct TextureData
        {
            public string Name;
            public int Width;
            public int Height;
            public byte[] Data;

            public TextureData(Texture2D texture)
            {
                Name = texture.name;
                Width = texture.width;
                Height = texture.height;
                Data = texture.EncodeToPNG();
            }

            public Texture2D GetTexture()
            {
                var texture = new Texture2D(Width, Height, TextureFormat.ARGB32, false);
                texture.LoadImage(Data, true);
                texture.name = Name;
                return texture;
            }
        }
    }
}
