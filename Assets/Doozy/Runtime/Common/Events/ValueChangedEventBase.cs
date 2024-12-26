// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.UIManager;
using UnityEngine;

namespace Doozy.Runtime.Common.Events
{
    /// <summary> Base class used to keep track of value changes </summary>
    /// <typeparam name="T"> Any type </typeparam>
    public abstract class ValueChangedEventBase<T> : IValueChangedEvent<T>
    {
        /// <summary> Previous value </summary>
        public T previousValue { get; }
        /// <summary> New value </summary>
        public T newValue { get; }
        /// <summary> Animate change flag </summary>
        public bool animateChange { get; }
        
        /// <summary> Flag to mark the event used </summary>
        public bool used { get; set; }
        /// <summary> Event timestamp </summary>
        public float timestamp { get; }
        
        /// <summary> Construct a value changed event </summary>
        /// <param name="previousValue"> Previous value </param>
        /// <param name="newValue"> New value </param>
        /// <param name="animateChange"> Animate change flag </param>
        protected ValueChangedEventBase(T previousValue, T newValue, bool animateChange)
        {
            this.previousValue = previousValue;
            this.newValue = newValue;
            this.animateChange = animateChange;
            
            used = false;
            timestamp = Time.time;
        }
    }
}
