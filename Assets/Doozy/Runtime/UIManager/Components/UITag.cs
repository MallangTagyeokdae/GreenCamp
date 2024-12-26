// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Attributes;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.UIManager.Components
{
    /// <summary> UI component used to tag a RectTransform in order to be able to easily find it at runtime </summary>
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("UI/Components/UITag")]
    public partial class UITag : MonoBehaviour
    {
        /// <summary> UITags database </summary>
        [ClearOnReload]
        public static HashSet<UITag> database { get; private set; } = new HashSet<UITag>();

        [ExecuteOnReload]
        private static void OnReload()
        {
            database ??= new HashSet<UITag>();
        }

        /// <summary> UITag identifier </summary>
        public UITagId Id;

        protected UITag()
        {
            Id = new UITagId();
        }

        protected virtual void Awake()
        {
            database.Add(this);
        }

        protected virtual void OnEnable()
        {
            database.Remove(null);
        }

        protected virtual void OnDestroy()
        {
            database.Remove(this);
            database.Remove(null);
        }

        #region Static Methods

        /// <summary>
        /// Get the first UITag with the given category and name.
        /// If no UITag is found, null is returned.
        /// </summary>
        /// <param name="category"> UITag category </param>
        /// <param name="name"> UITag name </param>
        /// <returns> The first UITag with the given category and name. </returns>
        public static UITag GetFirstTag(string category, string name) =>
            database.FirstOrDefault(tag => tag.Id.Category == category && tag.Id.Name == name);

        /// <summary> Get all the registered tags with the given category and name </summary>
        /// <param name="category"> UITag category </param>
        /// <param name="name"> UITag name (from the given category) </param>
        /// <returns> All the registered tags with the given category and name </returns>
        public static IEnumerable<UITag> GetTags(string category, string name) =>
            database.Where(item => item.Id.Category.Equals(category) && item.Id.Name.Equals(name));

        /// <summary> Get all the registered tags with the given category </summary>
        /// <param name="category"> UITag category </param>
        /// <returns> All the registered tags with the given category </returns>
        public static IEnumerable<UITag> GetAllTagsInCategory(string category) =>
            database.Where(item => item.Id.Category.Equals(category));

        #endregion
    }
}
