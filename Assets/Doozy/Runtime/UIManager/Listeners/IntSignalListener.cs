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
    /// Connects to a specific stream and reacts to meta-signals, that have an integer value payload
    /// </summary>
    [AddComponentMenu("Signals/Listeners/Int Signal Listener")]
    public class IntSignalListener : SignalListener
    {
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/Signals/Listeners/Int Signal Listener", false, 8)]
        private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        {
            GameObjectUtils.AddToScene<IntSignalListener>("Int Signal Listener", false, true);
        }
        #endif

        /// <summary>
        /// Callback invoked when the listener receives a signal with an integer value payload
        /// </summary>
        public IntEvent OnIntSignal = new IntEvent();

        protected override void ProcessSignal(Signal signal)
        {
            base.ProcessSignal(signal);
            if (signal == null) return;
            if (signal.valueType != typeof(int)) return;
            OnIntSignal?.Invoke((int)signal.valueAsObject);
        }
    }
}
