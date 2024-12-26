// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;
// ReSharper disable UnusedMember.Global

namespace Doozy.Runtime.Common
{
	/// <summary> Interface used as an extra layer when sending debug logs </summary>
	public interface ILogger
	{
		
		/// <summary> Log a message to the console </summary>
		/// <param name="message"> String or object to be converted to string representation for display </param>
		void Log(object message);
		
		/// <summary> Log a message to the console </summary>
		/// <param name="message"> String or object to be converted to string representation for display </param>
		/// <param name="context"> Object to which the message applies </param>
		void Log(object message, Object context);
		
		/// <summary> Log a warning message to the console </summary>
		/// <param name="message"> String or object to be converted to string representation for display </param>
		void LogWarning(object message);
		
		/// <summary> Log a warning message to the console </summary>
		/// <param name="message"> String or object to be converted to string representation for display </param>
		/// <param name="context"> Object to which the message applies </param>
		void LogWarning(object message, Object context);
		
		/// <summary> Log an error message to the console </summary>
		/// <param name="message"> String or object to be converted to string representation for display </param>
		void LogError(object message);
		
		/// <summary> Log an error message to the console </summary>
		/// <param name="message"> String or object to be converted to string representation for display </param>
		/// <param name="context"> Object to which the message applies </param>
		void LogError(object message, Object context);
	}
}