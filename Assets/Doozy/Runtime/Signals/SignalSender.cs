// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Common.Utils;
using UnityEngine;
// ReSharper disable MemberCanBeProtected.Global

namespace Doozy.Runtime.Signals
{
    /// <summary>
    /// Specialized component used to send a signal, or a meta signal (if a payload value is provided)
    /// </summary>
    [AddComponentMenu("Signals/Signal Sender")]
    public partial class SignalSender : MonoBehaviour
    {
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/Signals/Signal Sender", false, 8)]
        private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        {
            GameObjectUtils.AddToScene<SignalSender>("Signal Sender", false, true);
        }
        #endif
        
        /// <summary> Payload to send with the signal </summary>
        public SignalPayload Payload = new SignalPayload();
    
        /// <summary> Automatically send a signal on Start </summary>
        public bool SendOnStart;
        
        /// <summary> Automatically send a signal on OnEnable </summary>
        public bool SendOnEnable;
        
        /// <summary> Automatically send a signal on OnDisable </summary>
        public bool SendOnDisable;
        
        /// <summary> Automatically send a signal on OnDestroy </summary>
        public bool SendOnDestroy;

        protected virtual void Start()
        {
            if (SendOnStart) SendSignal();
        }
        
        protected virtual void OnEnable()
        {
            if (SendOnEnable) SendSignal();
        }
        
        protected virtual void OnDisable()
        {
            if (SendOnDisable) SendSignal();
        }
        
        protected virtual void OnDestroy()
        {
            if (SendOnDestroy) SendSignal();
        }

        /// <summary> Send a Signal with the set payload value to the stream with the given stream id </summary>
        public virtual void SendSignal()
        {
            Payload?.SendSignal();
        }
    }
}
