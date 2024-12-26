// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEditor;
using UnityEngine;
namespace Doozy.Editor.Common.Utils
{
    public static class AssetUtils
    {
        /// <summary> Delete any sub objects inside a main asset </summary>
        /// <param name="mainAsset"> Target asset </param>
        public static void RemoveSubAssets(Object mainAsset)
        {
            Object[] assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(mainAsset));
            foreach (Object asset in assets)
            {
                if (asset == null) continue;
                if (asset.Equals(mainAsset)) continue;
                UnityEngine.Object.DestroyImmediate(asset, true);
            }
        }
    }
}
