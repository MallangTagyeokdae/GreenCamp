// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Editor.Common.ScriptableObjects
{
    public class DynamicSearchProvider : ScriptableObject, ISearchWindowProvider
    {
        //key: searchPath
        //value: callback
        //pair: 42
        private HashSet<KeyValuePair<string, UnityAction>> items { get; set; } = new HashSet<KeyValuePair<string, UnityAction>>();
        private string title { get; set; } = "---";
        private string emptyTitle { get; set; } = "No match found!";
        public UnityAction<KeyValuePair<string, UnityAction>> onSelectEntryCallback { get; set; }

        /// <summary> Add a set of items to the search menu </summary>
        /// <param name="valuePairs"> For each pair, the key is the searchPath (menu path) and the value is the callback for the menu selection </param>
        /// <param name="clear"> Clear the items list before adding the items </param>
        public DynamicSearchProvider AddItems(IEnumerable<KeyValuePair<string, UnityAction>> valuePairs, bool clear = true)
        {
            if (clear) Clear();

            foreach (KeyValuePair<string, UnityAction> pair in valuePairs)
                items.Add(pair);

            return this;
        }

        private DynamicSearchProvider Clear()
        {
            items ??= new HashSet<KeyValuePair<string, UnityAction>>();
            items.Clear();
            return this;
        }

        public DynamicSearchProvider SetTitle(string value = "---")
        {
            title = value;
            return this;
        }

        public DynamicSearchProvider SetEmptyTitle(string value = "No match found!")
        {
            emptyTitle = value;
            return this;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var list = new List<SearchTreeEntry>();

            if (items.Count == 0) title = emptyTitle;
            list.Add(new SearchTreeGroupEntry(new GUIContent(title), 0));

            var sortedList = items.ToList();
            sortedList.Sort((a, b) =>
            {
                string[] aSplit = a.Key.Split('/');
                string[] bSplit = b.Key.Split('/');
                for (int i = 0; i < aSplit.Length; i++)
                {
                    if (i >= bSplit.Length)
                        return 1;

                    int value = string.Compare(aSplit[i], bSplit[i], StringComparison.Ordinal);

                    if (value == 0)
                        continue;

                    // make sure leaves go before nodes
                    if (aSplit.Length != bSplit.Length && (i == aSplit.Length - 1 || i == bSplit.Length - 1))
                        return aSplit.Length < bSplit.Length ? 1 : -1;

                    return value;
                }

                return 0;
            });

            var groups = new List<string>();
            foreach (KeyValuePair<string, UnityAction> item in sortedList)
            {
                string[] entryTitle = item.Key.Split('/');
                string groupName = "";
                for (int i = 0; i < entryTitle.Length - 1; i++)
                {
                    groupName += entryTitle[i];
                    if (!groups.Contains(groupName))
                    {
                        list.Add(new SearchTreeGroupEntry(new GUIContent(entryTitle[i]), i + 1));
                        groups.Add(groupName);
                    }
                    groupName += "/";
                }
                list.Add
                (
                    new SearchTreeEntry(new GUIContent(entryTitle.Last()))
                    {
                        level = entryTitle.Length,
                        userData = item
                    }
                );
            }

            return list;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var searchItem = (KeyValuePair<string, UnityAction>)searchTreeEntry.userData;
            searchItem.Value?.Invoke();
            onSelectEntryCallback?.Invoke(searchItem);
            return true;
        }
    }
}
