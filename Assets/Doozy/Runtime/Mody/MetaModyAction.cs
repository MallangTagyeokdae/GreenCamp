// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Signals;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Mody
{
    /// <summary>
    /// Base class for actions, that have a value, in the Mody System.
    /// <para/> Any action with a value that interacts with the system needs to derive from this.
    /// <para/> It is used to start, stop and finish Module's (MonoBehaviour's) tasks.
    /// <para/> It can be triggered manually (via code) or by any Trigger registered to the system.
    /// </summary>
    /// <typeparam name="T"> Value type </typeparam>
    [Serializable]
    public abstract class MetaModyAction<T> : ModyAction
    {
        /// <summary> ModyAction's value </summary>
        public T ActionValue;

        /// <summary> Callback triggered by this ModyAction </summary>
        public UnityAction<T> actionCallback { get; private set; }

        /// <summary> Construct a new <see cref="MetaModyAction{T}"/> with the given settings </summary>
        /// <param name="behaviour"> MonoBehaviour the ModyAction belongs to </param>
        /// <param name="actionName"> Name of the action </param>
        /// <param name="callback"> Callback triggered by the ModyAction </param>
        protected MetaModyAction(MonoBehaviour behaviour, string actionName, UnityAction<T> callback) : base(behaviour, actionName)
        {
            actionCallback = callback;
            HasValue = true;
            ValueType = typeof(T);

            IgnoreSignalValue = false;
            ReactToAnySignal = false;
        }

        protected override void Run(Signal signal)
        {
            if (ReactToAnySignal)
            {
                if (IgnoreSignalValue)
                {
                    actionCallback?.Invoke(ActionValue); //invoke callbacks
                    return;
                }

                //do not react to any signal --> check for valid signal to update the value
                if (signal != null && signal.valueType == ValueType)
                    ActionValue = signal.GetValueUnsafe<T>(); //get value from signal

                actionCallback?.Invoke(ActionValue); //invoke callbacks
                return;
            }

            //do not react to any signal --> check for valid signal
            if (signal == null) return; //signal is null --> return
            if (!signal.hasValue) return; //signal does not have value --> return
            if (signal.valueType != ValueType) return; //signal value type does not match the action value type --> return

            if (IgnoreSignalValue)
            {
                actionCallback?.Invoke(ActionValue); //invoke callbacks
                return;
            }

            ActionValue = signal.GetValueUnsafe<T>(); //get value from signal
            actionCallback?.Invoke(ActionValue); //invoke callbacks
        }

        /// <summary> Set a new value to the ModyAction </summary>
        /// <param name="value"> New value </param>
        public void SetValue(T value) =>
            ActionValue = value;

        /// <summary> Set a new value to the ModyAction as an object </summary>
        /// <param name="objectValue"> New value as an object </param>
        /// <returns> True if the operation was a success and false otherwise </returns>
        public override bool SetValue(object objectValue) => 
            SetValue(objectValue, true);
        
        internal override bool SetValue(object objectValue, bool restrictValueType)
        {
            if (objectValue == null) return false;
            if (restrictValueType && objectValue.GetType() != ValueType)
                return false;
            try
            {
                SetValue((T)objectValue);
                return true;
            }
            catch
            {
                // ignored
                return false;
            }
        }
    }
}
