// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Common.Events;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Signals;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Listeners
{
    /// <summary>
    /// Connects to a specific stream and reacts to meta-signals, that have a Vector4 value payload
    /// </summary>
    [AddComponentMenu("Signals/Listeners/Vector4 Signal Listener")]
    public class Vector4SignalListener : SignalListener
    {
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/Signals/Listeners/Vector4 Signal Listener", false, 8)]
        private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        {
            GameObjectUtils.AddToScene<Vector4SignalListener>("Vector4 Signal Listener", false, true);
        }
        #endif

        /// <summary>
        /// Callback invoked when the listener receives a signal with a Vector4 value payload
        /// </summary>
        public Vector4Event OnVector4Signal = new Vector4Event();

        protected override void ProcessSignal(Signal signal)
        {
            base.ProcessSignal(signal);
            if (signal == null) return;
            if (signal.valueType != typeof(Vector4)) return;
            OnVector4Signal?.Invoke((Vector4)signal.valueAsObject);
        }
    }
}
