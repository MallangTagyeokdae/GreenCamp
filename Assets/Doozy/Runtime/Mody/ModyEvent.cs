// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Signals;
using UnityEngine.Events;

namespace Doozy.Runtime.Mody
{
    /// <summary>
    /// Mody specialized event that can trigger a UnityEvent and a set of ModyActionRunners  
    /// </summary>
    [Serializable]
    public class ModyEvent : ModyEventBase
    {
        /// <summary> UnityEvent invoked when this event is executed. Note that if this mody event is not enabled, this UnityEvent will not get invoked </summary>
        public UnityEvent Event = new UnityEvent();

        /// <summary>
        /// Returns TRUE if the Event (UnityEvent) has the persistent event listeners count greater than zero
        /// <para/> Persistent event listeners are the ones set in the Inspector
        /// </summary>
        public bool hasEvents => Event != null && Event.GetPersistentEventCount() > 0;
        
        /// <summary> Returns TRUE if this ModyEvent has runners or its Event (UnityEvent) has the non persistent event listeners count greater than zero </summary>
        public override bool hasCallbacks => hasRunners | hasEvents;

        public ModyEvent() : this(k_DefaultEventName) {}

        public ModyEvent(string eventName) : base(eventName) {}

        public override void Execute(Signal signal = null)
        {
            base.Execute(signal);
            Event?.Invoke();
        }
    }
}
