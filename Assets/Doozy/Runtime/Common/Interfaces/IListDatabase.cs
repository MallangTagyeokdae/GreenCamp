// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Runtime.Common
{
	/// <summary> Interface used for List databases </summary>
	/// <typeparam name="TKey"> Key type </typeparam>
	/// <typeparam name="TValue"> Value type </typeparam>
	public interface IListDatabase<TKey, TValue>
	{
		/// <summary> Add a new key to the database with the default value </summary>
		/// <param name="key"> New key </param>
		void Add(TKey key);
		
		/// <summary> Add a new key to the database with the given value </summary>
		/// <param name="key"> New key </param>
		/// <param name="value"> New value </param>
		void Add(TKey key, TValue value);

		/// <summary> Clear the database </summary>
		void Clear();

		/// <summary> Check if the database contains the given key </summary>
		/// <param name="key"> Key to search for </param>
		/// <returns> True or False </returns>
		bool ContainsKey(TKey key);
		
		/// <summary> Check if the database contains the given key and value pair </summary>
		/// <param name="key"> Key to search for </param>
		/// <param name="value"> Value to search for </param>
		/// <returns> True or False </returns>
		bool ContainsValue(TKey key, TValue value);
		
		/// <summary> Check if the database contains the given value </summary>
		/// <param name="value"> Value to search for </param>
		/// <returns> True or False </returns>
		bool ContainsValue(TValue value);

		/// <summary> Get the number of keys in the database </summary>
		/// <returns> Number of keys in the database </returns>
		int CountKeys();
		
		/// <summary> Get the number of values for the given key </summary>
		/// <param name="key"> Key to search for </param>
		/// <returns> Number of values for the given key </returns>
		int CountValues(TKey key);

		/// <summary> Get a list of all the values for the given key </summary>
		/// <param name="key"> Key to search for </param>
		/// <returns> A new List with all the values for the given key </returns>
		List<TValue> GetValues(TKey key);
		
		/// <summary> Get a list of all the keys in the database </summary>
		/// <returns> A new list with all the keys in the database </returns>
		List<TKey> GetKeys();
		
		/// <summary> Remove the given key from the database </summary>
		/// <param name="key"> Key to remove </param>
		void Remove(TKey key);
		
		/// <summary> Remove the value from the given key </summary>
		/// <param name="key"> Key to search for  </param>
		/// <param name="value"> Value to remove </param>
		/// <param name="deleteEmptyKey"> If TRUE and the list of values is empty, the key will be removed as well </param>
		void Remove(TKey key, TValue value, bool deleteEmptyKey = true);
		
		/// <summary> Remove the value from the database </summary>
		/// <param name="value"> Value to remove </param>
		/// <param name="deleteEmptyKey"> If TRUE and the list of values is empty, the key will be removed as well </param>
		void Remove(TValue value, bool deleteEmptyKey = true);

		/// <summary> Validate the database by removing null entries </summary>
		/// <param name="deleteEmptyKeys"> If TRUE and empty keys are found, they will be removed </param>
		void Validate(bool deleteEmptyKeys = true);
	}
}