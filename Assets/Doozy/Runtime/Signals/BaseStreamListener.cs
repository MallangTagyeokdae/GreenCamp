// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Runtime.Signals
{
    public abstract class BaseStreamListener : MonoBehaviour
    {
        /// <summary> Signal receiver used to listen for signals </summary>
        public SignalReceiver receiver { get; protected set; }
        
        /// <summary> Flag that keeps track of whether the signal receiver is connected to a signal stream or not </summary>
        public bool isConnected { get; protected set; }

        protected BaseStreamListener()
        {
            isConnected = false;
            receiver = new SignalReceiver().SetOnSignalCallback(ProcessSignal);
        }
        
        /// <summary>
        /// Connects the signal receiver to a signal stream
        /// </summary>
        public virtual void Connect()
        {
            if (isConnected) return;
            ConnectReceiver();
            isConnected = true;
        }

        /// <summary>
        /// Disconnects the signal receiver from a signal stream
        /// </summary>
        public virtual void Disconnect()
        {
            if (!isConnected) return;
            DisconnectReceiver();
            isConnected = false;
        }

        /// <summary>
        /// Connect the signal receiver to a signal stream
        /// </summary>
        protected abstract void ConnectReceiver();
        
        /// <summary>
        /// Disconnect the signal receiver from a signal stream
        /// </summary>
        protected abstract void DisconnectReceiver();
        
        /// <summary>
        /// Process the signal received from the signal stream
        /// </summary>
        /// <param name="signal"></param>
        protected abstract void ProcessSignal(Signal signal);
    }
}
