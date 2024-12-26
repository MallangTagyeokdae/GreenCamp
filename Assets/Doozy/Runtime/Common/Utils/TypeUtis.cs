// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable UnusedMember.Global

namespace Doozy.Runtime.Common.Utils
{
	/// <summary> Utility class with optimized methods for working with Types </summary>
	public static class TypeUtils
	{
		/// <summary> Cast object to the given type </summary>
		/// <param name="input"> Target object </param>
		/// <typeparam name="T"> Type to cast to </typeparam>
		/// <returns> Object casted to the given type </returns>
		public static T CastObject<T>(object input) =>
			(T) input;

		/// <summary> Convert object to the given type </summary>
		/// <param name="input"> target object </param>
		/// <typeparam name="T"> Type to convert to </typeparam>
		/// <returns> Object converted to the given type </returns>
		public static T ConvertObject<T>(object input) =>
			(T) Convert.ChangeType(input, typeof(T));

		/// <summary> Get all the derived types of the given type </summary>
		/// <param name="type"> Type to search for </param>
		/// <returns> An IEnumerable of all the derived types of the given type </returns>
		public static IEnumerable<Type> GetDerivedTypesOfType(Type type) =>
			from domainAssembly in ReflectionUtils.domainAssemblies
			from assemblyType in domainAssembly.GetTypes()
			where type.IsAssignableFrom(assemblyType)
			where assemblyType.IsSubclassOf(type) && !assemblyType.IsAbstract
			select assemblyType;
	}
}