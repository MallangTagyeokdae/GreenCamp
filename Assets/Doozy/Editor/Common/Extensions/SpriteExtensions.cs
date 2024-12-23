// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI.Utils;
using UnityEditor;
using UnityEditor.Sprites;
using UnityEngine;
using Object = UnityEngine.Object;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Common.Extensions
{
    /// <summary> Extension methods for Sprites </summary>
    public static class SpriteExtensions
    {
        /// <summary> Convert Sprite to Texture2D </summary>
        /// <param name="sprite"> Target Sprite </param>
        public static Texture2D ToTexture2D(this Sprite sprite)
        {
            if (sprite == null) throw new NullReferenceException(nameof(sprite));
            try
            {
                if (sprite.rect.width == sprite.texture.width)
                {
                    sprite.texture.name = sprite.name;
                    return sprite.texture;
                }

                var texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);

                Color[] newColors = sprite.texture.GetPixels
                (
                    Mathf.CeilToInt(sprite.textureRect.x),
                    Mathf.CeilToInt(sprite.textureRect.y),
                    Mathf.CeilToInt(sprite.textureRect.width),
                    Mathf.CeilToInt(sprite.textureRect.height)
                );

                texture.SetPixels(newColors);
                texture.Apply();

                texture.name = sprite.name;

                return texture;
            }
            catch
            {
                sprite.texture.name = sprite.name;
                return sprite.texture;
            }
        }

        /// <summary> Convert a collection of Sprite to a collection of Texture2D </summary>
        /// <param name="sprites"> Sprite collection </param>
        public static IEnumerable<Texture2D> ToTexture2D(this IEnumerable<Sprite> sprites)
        {
            if (sprites == null) throw new NullReferenceException(nameof(sprites));
            return sprites.Select(sprite => sprite.ToTexture2D());
        }
    }
}
