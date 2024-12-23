// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Diagnostics.CodeAnalysis;

namespace Doozy.Runtime.Common
{
    /// <summary>
    /// Data class used to set and save a category name pair as an id 
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
    public abstract class CategoryNameId : IEquatable<CategoryNameId>
    {
        /// <summary> Default string value for Category </summary>
        public static string defaultCategory => CategoryNameItem.k_DefaultCategory;
        /// <summary> Default string value for Name </summary>
        public static string defaultName => CategoryNameItem.k_DefaultName;

        /// <summary> Category value </summary>
        public string Category;
        /// <summary> Name value </summary>
        public string Name;
        /// <summary> Flag used by the editor if to lookup the category name values in the database or not </summary>
        public bool Custom;

        protected CategoryNameId()
        {
            Category = defaultCategory;
            Name = defaultName;
            Custom = false;
        }
        
        protected CategoryNameId(string category, string name, bool custom = false)
        {
            Category = category;
            Name = name;
            Custom = custom;
        }

        /// <summary> Convert to string </summary>
        /// <returns> Pretty string </returns>
        public override string ToString() => $"{Category} / {Name}";
        
        public static bool operator==(CategoryNameId a, CategoryNameId b) => !(a is null) && a.Equals(b);
        public static bool operator!=(CategoryNameId a, CategoryNameId b) => !(a == b);
        public bool Equals(CategoryNameId other) => !(other is null) && Category == other.Category && Name == other.Name;
        public override bool Equals(object obj) => obj is CategoryNameId other && Equals(other);
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Category != null ? Category.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Custom.GetHashCode();
                return hashCode;
            }
        }
    }
}
