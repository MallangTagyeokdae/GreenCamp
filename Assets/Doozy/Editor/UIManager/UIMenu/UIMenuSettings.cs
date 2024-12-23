// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Common.ScriptableObjects;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.UIMenu
{
    public class UIMenuSettings : SingletonEditorScriptableObject<UIMenuSettings>
    {
        public bool SelectNewlyCreatedObjects;
        public PrefabInstantiateModeSetting InstantiateMode;

        public const int k_MinItemSize = 64;
        public const int k_MaxItemSize = 256;
        public const int k_DefaultItemSize = 96;
        
        [SerializeField] private int ItemSize = k_DefaultItemSize;
        public int itemSize
        {
            get => ItemSize;
            set => ItemSize = Mathf.Clamp(value, k_MinItemSize, k_MaxItemSize);
        }
    }
}
