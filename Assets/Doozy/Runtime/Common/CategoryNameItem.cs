// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Common.Extensions;
using UnityEngine;

namespace Doozy.Runtime.Common
{
    /// <summary> Pair of two strings used as a single entry in Category Name databases </summary>
    [Serializable]
    public class CategoryNameItem
    {
        /// <summary> Default string value for Category </summary>
        public const string k_DefaultCategory = "None";
        /// <summary> Default string value for Name </summary>
        public const string k_DefaultName = "None";

        [SerializeField] private string Category;
        /// <summary> Category string value </summary>
        public string category => Category;

        [SerializeField] private string Name;
        /// <summary> Name string value </summary>
        public string name => Name;

        /// <summary> Construct a new <see cref="CategoryNameItem"/> with default values </summary>
        public CategoryNameItem()
        {
            Category = k_DefaultCategory;
            Name = k_DefaultName;
        }
        /// <summary> Construct a new <see cref="CategoryNameItem"/> with the given category value and the default name value </summary>
        /// <param name="category"> Category value </param>
        public CategoryNameItem(string category)
        {
            Category = category;
            Name = k_DefaultName;
        }

        /// <summary> Construct a new <see cref="CategoryNameItem"/> with the given category and name values </summary>
        /// <param name="category"> Category value </param>
        /// <param name="name"> Name value </param>
        /// <param name="removeWhitespaces"> Remove whitespaces from values </param>
        /// <param name="removeSpecialCharacters"> Remove special characters from values </param>
        public CategoryNameItem(string category, string name, bool removeWhitespaces = true, bool removeSpecialCharacters = true)
        {
            Category = CleanString(category, removeWhitespaces, removeSpecialCharacters);
            Name = CleanString(name, removeWhitespaces, removeSpecialCharacters);
        }

        /// <summary> Set a new Category value </summary>
        /// <param name="newCategory"> Target value </param>
        /// <param name="removeWhitespaces"> Remove all whitespaces from the target string </param>
        /// <param name="removeSpecialCharacters"> Remove all special characters from the target string </param>
        /// <returns> Operation result (True or False) and a success or failure reason message </returns>
        public (bool, string) SetCategory(string newCategory, bool removeWhitespaces = true, bool removeSpecialCharacters = true)
        {
            if (newCategory.RemoveWhitespaces().RemoveAllSpecialCharacters().IsNullOrEmpty())
                return (false, $"Invalid '{nameof(newCategory)}'. It cannot be null or empty or contain special characters");
            Category = CleanString(newCategory, removeWhitespaces, removeSpecialCharacters);
            return (true, $"'{nameof(Category)}' renamed to: {Category}");
        }

        /// <summary> Set a new Name value </summary>
        /// <param name="newName"> Target value </param>
        /// <param name="removeWhitespaces"> Remove all whitespaces from the target string </param>
        /// <param name="removeSpecialCharacters"> Remove all special characters from the target string </param>
        /// <returns> Operation result (True or False) and a success or failure reason message </returns>
        public (bool, string) SetName(string newName, bool removeWhitespaces = true, bool removeSpecialCharacters = true)
        {
            if (newName.RemoveWhitespaces().RemoveAllSpecialCharacters().IsNullOrEmpty())
                return (false, $"Invalid '{nameof(newName)}'. It cannot be null or empty or contain special characters");
            Name = CleanString(newName, removeWhitespaces, removeSpecialCharacters);
            return (true, $"'{nameof(Name)}' renamed to: {Name}");
        }

        /// <summary> Cleans the string by removing any empty spaces (at the start of the string and/or the end of the string) </summary>
        /// <param name="value"> Target value </param>
        /// <param name="removeWhitespaces"> Remove all whitespaces from the target string </param>
        /// <param name="removeSpecialCharacters"> Remove all special characters from the target string </param>
        /// <returns> The cleaned string </returns>
        public static string CleanString(string value, bool removeWhitespaces = true, bool removeSpecialCharacters = true)
        {
            if (removeWhitespaces) value = value.RemoveWhitespaces();
            if (removeSpecialCharacters) value = value.RemoveAllSpecialCharacters();
            return value.Trim();
        }
    }
}
