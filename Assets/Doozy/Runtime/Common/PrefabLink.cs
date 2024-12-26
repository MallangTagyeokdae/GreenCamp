// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;

namespace Doozy.Runtime.Common
{
    [Serializable]
    public abstract class PrefabLink : ScriptableObject
    {
        /// <summary> Name of the prefab </summary>
        [SerializeField] private string PrefabName;

        /// <summary> Name of the prefab </summary>
        public string prefabName
        {
            get => PrefabName;
            protected set => PrefabName = value;
        }

        /// <summary> The prefab reference </summary>
        [SerializeField] private GameObject Prefab;

        /// <summary> The prefab reference </summary>
        public GameObject prefab
        {
            get => Prefab;
            protected set => Prefab = value;
        }
        
        /// <summary> TRUE if the prefab reference is not null </summary>
        public bool hasPrefab => prefab != null;
        
        /// <summary> TRUE if the prefabName is not null or empty </summary>
        public bool hasPrefabName => !string.IsNullOrEmpty(prefabName);

        protected PrefabLink(GameObject prefab, string prefabName = null)
        {
            Prefab = prefab;
            PrefabName = prefabName;
        }

        public abstract void Validate();
    }
}
