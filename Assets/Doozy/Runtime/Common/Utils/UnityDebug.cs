// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;

namespace Doozy.Runtime.Common.Utils
{
	/// <summary> Class used as a bridge to connect the customized <see cref="Debugger"/> with the Debug class in Unity </summary>
	public class UnityDebug : ILogger
	{
		/// <summary> Log a message to the console </summary>
		/// <param name="message"> String or object to be converted to string representation for display </param>
		public void Log(object message) =>
			Debug.Log(message);

		/// <summary> Log a message to the console </summary>
		/// <param name="message"> String or object to be converted to string representation for display </param>
		/// <param name="context"> Object to which the message applies </param>
		public void Log(object message, Object context) =>
			Debug.Log(message, context);

		/// <summary> Log a warning message to the console </summary>
		/// <param name="message"> String or object to be converted to string representation for display </param>
		public void LogWarning(object message) =>
			Debug.LogWarning(message);

		/// <summary> Log a warning message to the console </summary>
		/// <param name="message"> String or object to be converted to string representation for display </param>
		/// <param name="context"> Object to which the message applies </param>
		public void LogWarning(object message, Object context) =>
			Debug.Log(message, context);

		/// <summary> Log an error message to the console </summary>
		/// <param name="message"> String or object to be converted to string representation for display </param>
		public void LogError(object message) =>
			Debug.Log(message);

		/// <summary> Log an error message to the console </summary>
		/// <param name="message"> String or object to be converted to string representation for display </param>
		/// <param name="context"> Object to which the message applies </param>
		public void LogError(object message, Object context) =>
			Debug.Log(message, context);
	}
}