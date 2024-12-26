// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.Common
{
	/// <summary> List database of key and value pairs </summary>
	/// <typeparam name="TKey"> Key type </typeparam>
	/// <typeparam name="TValue"> Value type </typeparam>
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	[Serializable]
	public class ListDatabase<TKey, TValue> : IListDatabase<TKey, TValue>
	{
		private readonly Type m_keyType;
		private readonly Type m_valueType;
		
		/// <summary> Database of keys and lists of values </summary>
		public Dictionary<TKey, List<TValue>> Database { get; }

		/// <summary> All the keys in the database </summary>
		public Dictionary<TKey, List<TValue>>.KeyCollection Keys => Database.Keys;
		
		/// <summary> All the values in the database </summary>
		public Dictionary<TKey, List<TValue>>.ValueCollection Values => Database.Values;
		
		public ListDatabase()
		{
			m_keyType = typeof(TKey);
			m_valueType = typeof(TValue);
			Database = new Dictionary<TKey, List<TValue>>();
		}

		/// <summary> Add a new key to the database with the default value </summary>
		/// <param name="key"> New key </param>
		public void Add(TKey key)
		{
			if (Database.ContainsKey(key)) return;
			Database.Add(key, new List<TValue>());
		}

		/// <summary> Add a new key to the database with the given value </summary>
		/// <param name="key"> New key </param>
		/// <param name="value"> New value </param>
		public void Add(TKey key, TValue value)
		{
			if (ContainsKey(key))
			{
				if (ContainsValue(key, value))
					return;
				if (Database[key] == null)
					Database[key] = new List<TValue>();
				Database[key].Add(value);
				return;
			}

			Database.Add(key, new List<TValue> {value});
		}

		/// <summary> Clear the database </summary>
		public void Clear() =>
			Database.Clear();

		/// <summary> Check if the database contains the given key </summary>
		/// <param name="key"> Key to search for </param>
		/// <returns> True or False </returns>
		public bool ContainsKey(TKey key) =>
			Database.ContainsKey(key);

		/// <summary> Check if the database contains the given key and value pair </summary>
		/// <param name="key"> Key to search for </param>
		/// <param name="value"> Value to search for </param>
		/// <returns> True or False </returns>
		public bool ContainsValue(TKey key, TValue value) =>
			ContainsKey(key) && Database[key].Contains(value);

		/// <summary> Check if the database contains the given value </summary>
		/// <param name="value"> Value to search for </param>
		/// <returns> True or False </returns>
		public bool ContainsValue(TValue value) =>
			Database.Keys.Any(key => Database[key].Contains(value));

		/// <summary> Get the number of keys in the database </summary>
		/// <returns> Number of keys in the database </returns>
		public int CountKeys() =>
			Database.Keys.Count;

		/// <summary> Get the number of values for the given key </summary>
		/// <param name="key"> Key to search for </param>
		/// <returns> Number of values for the given key </returns>
		public int CountValues(TKey key) =>
			Database.ContainsKey(key) 
				? Database[key].Count
				: 0;

		/// <summary> Get a list of all the values for the given key </summary>
		/// <param name="key"> Key to search for </param>
		/// <returns> A new List with all the values for the given key </returns>
		public List<TValue> GetValues(TKey key) =>
			key == null || !ContainsKey(key) || Database[key] == null
				? new List<TValue>()
				: Database[key].ToList();

		/// <summary> Get a list of all the keys in the database </summary>
		/// <returns> A new list with all the keys in the database </returns>
		public List<TKey> GetKeys() =>
			Keys.ToList();
		
		/// <summary> Remove the given key from the database </summary>
		/// <param name="key"> Key to remove </param>
		public void Remove(TKey key)
		{
			if (!ContainsKey(key)) return;
			Database.Remove(key);
		}

		/// <summary> Remove the value from the given key </summary>
		/// <param name="key"> Key to search for  </param>
		/// <param name="value"> Value to remove </param>
		/// <param name="deleteEmptyKey"> If TRUE and the list of values is empty, the key will be removed as well </param>
		public void Remove(TKey key, TValue value, bool deleteEmptyKey = true)
		{
			if (!ContainsValue(key, value)) return;
			Database[key].Remove(value);
			if (!deleteEmptyKey) return;
			if (Database[key].Count == 0) Database.Remove(key);
		}

		/// <summary> Remove the value from the database </summary>
		/// <param name="value"> Value to remove </param>
		/// <param name="deleteEmptyKey"> If TRUE and the list of values is empty, the key will be removed as well </param>
		public void Remove(TValue value, bool deleteEmptyKey = true)
		{
			if (!ContainsValue(value)) return;
			var keysToRemove = new List<TKey>();
			foreach (TKey key in Database.Keys.Where(key => Database[key].Contains(value)))
			{
				Database[key].Remove(value);
				if (!deleteEmptyKey) continue;
				if (Database[key].Count == 0) keysToRemove.Add(key);
			}

			if (!deleteEmptyKey) return;
			foreach (TKey key in keysToRemove) Database.Remove(key);
		}

		/// <summary> Validate the database by removing null entries </summary>
		/// <param name="deleteEmptyKeys"> If TRUE and empty keys are found, they will be removed </param>
		public void Validate(bool deleteEmptyKeys = true)
		{
			var keysToRemove = new List<TKey>();
			foreach (TKey key in Database.Keys)
			{
				for (int i = Database[key].Count - 1; i >= 0; i--)
				{
					TValue value = Database[key][i];
					if (value != null) continue;
					Database[key].RemoveAt(i);
				}

				if (!deleteEmptyKeys) continue;
				if (Database[key].Count == 0) keysToRemove.Add(key);
			}

			if (!deleteEmptyKeys) return;
			foreach (TKey key in keysToRemove) Database.Remove(key);
		}
	}
}