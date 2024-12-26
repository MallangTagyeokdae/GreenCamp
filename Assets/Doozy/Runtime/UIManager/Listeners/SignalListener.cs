// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Listeners.Internal;
using UnityEngine;
using UnityEngine.Events;
// ReSharper disable UnassignedGetOnlyAutoProperty
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.UIManager.Listeners
{
    /// <summary>
    /// Connects to a specific stream and reacts to 'pings' (signals) sent through said stream
    /// </summary>
    [AddComponentMenu("Signals/Listeners/Signal Listener")]
    public class SignalListener : BaseListener
    {
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/Signals/Listeners/Signal Listener", false, 8)]
        private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        {
            GameObjectUtils.AddToScene<SignalListener>("Signal Listener", false, true);
        }
        #endif
        
        [SerializeField] protected StreamId StreamId;
        /// <summary> Stream Id </summary>
        public StreamId streamId => StreamId;

        /// <summary> The signal stream this listener is connected to </summary>
        public SignalStream stream { get; private set; }
        
        /// <summary> UnityEvent callback executed every time the listener is triggered </summary>
        public SignalEvent OnSignal = new SignalEvent();
        
        /// <summary> UnityAction callback executed when the listener is triggered </summary>
        public UnityAction<Signal> signalCallback { get; } 
        
        protected virtual void OnEnable()
        {
            ConnectReceiver();
        }
        
        protected virtual void OnDisable()
        {
            DisconnectReceiver();
        }
        
        protected override void ConnectReceiver() =>
            stream = SignalStream.Get(streamId.Category, streamId.Name).ConnectReceiver(receiver);

        protected override void DisconnectReceiver() =>
            stream.DisconnectReceiver(receiver);
        
        protected override void ProcessSignal(Signal signal)
        {
            signalCallback?.Invoke(signal);
            Callback?.Execute(signal);
            OnSignal?.Invoke(signal);
        }
    }
}
