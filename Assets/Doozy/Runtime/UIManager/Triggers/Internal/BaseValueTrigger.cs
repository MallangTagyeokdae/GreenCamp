// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using System;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Mody;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Triggers.Internal
{
    public abstract class BaseValueTrigger<Tbehaviour> : MonoBehaviour where Tbehaviour : MonoBehaviour
    {
        public enum TriggerWhenValueIs
        {
            EqualTo,
            LessThan,
            LessThanOrEqualTo,
            GreaterThanOrEqualTo,
            GreaterThan,
        }

        private const float TOLERANCE = 0.0001f;

        [SerializeField] protected Tbehaviour Target;
        /// <summary> Target component that provides the value </summary>
        public Tbehaviour target
        {
            get
            {
                if (Target == null)
                    Target = GetComponent<Tbehaviour>();
                return Target;
            }
        }

        [SerializeField] protected TriggerWhenValueIs TriggerMode = TriggerWhenValueIs.EqualTo;
        /// <summary> Trigger condition </summary>
        public TriggerWhenValueIs triggerMode
        {
            get => TriggerMode;
            set
            {
                TriggerMode = value;
                ResetTrigger();
            }
        }

        [SerializeField] protected float TriggerValue;
        /// <summary> Value that will be used to trigger the event </summary>
        public float triggerValue
        {
            get => TriggerValue;
            set => TriggerValue = value;
        }

        /// <summary> Fired when the trigger is activated </summary>
        public ModyEvent OnTrigger = new ModyEvent();

        /// <summary> If TRUE, the trigger will be activated when the value is EXACTLY equal to the TargetValue </summary>
        public bool TriggerOnExactValueMatch;

        /// <summary> If TRUE, the trigger will be activated only when the value was increasing and it is now equal to the TargetValue </summary>
        public bool TriggerOnIncrement = true;

        /// <summary> If TRUE, the trigger will be activated only when the value was decreasing and it is now equal to the TargetValue </summary>
        public bool TriggerOnDecrement = true;

        /// <summary> Current value for the target </summary>
        protected abstract float value { get; }

        /// <summary> Internal flag used to prevent the trigger from firing multiple times </summary>
        private bool triggered { get; set; }

        /// <summary> Keeps track of the previous value to determine what changes occurred </summary>
        private float previousValue { get; set; }

        protected virtual void Reset()
        {
            Target = Target ? Target : GetComponent<Tbehaviour>();
        }

        private void OnValueChanged(float oldValue, float newValue)
        {
            // Debug.Log($"OnValueChanged: {oldValue} -> {newValue}");

            switch (triggerMode)
            {
                case TriggerWhenValueIs.EqualTo:
                {
                    //if the new value is different than the trigger value -> reset the trigger
                    if (triggered && Math.Abs(newValue - TriggerValue) > TOLERANCE)
                        ResetTrigger();

                    if (triggered)
                        return;

                    if (TriggerOnExactValueMatch & Math.Abs(newValue - TriggerValue) > TOLERANCE)
                        return;

                    if (oldValue < TriggerValue & newValue >= TriggerValue)
                    {
                        if (TriggerOnIncrement || !TriggerOnIncrement & !TriggerOnDecrement)
                        {
                            Trigger();
                            return;
                        }
                    }
                    
                    if (oldValue > TriggerValue & newValue <= TriggerValue)
                    {
                        if (TriggerOnDecrement || !TriggerOnIncrement & !TriggerOnDecrement)
                        {
                            Trigger();
                            return;
                        }
                    }

                }
                    break;
                case TriggerWhenValueIs.LessThan:
                {
                    //if the new value is greater than the trigger value -> reset the trigger 
                    if (triggered & newValue > TriggerValue)
                        ResetTrigger();

                    if (triggered)
                        return;

                    if (oldValue >= TriggerValue & 
                        newValue < TriggerValue & 
                        Math.Abs(oldValue - newValue) > TOLERANCE)
                        Trigger();
                }
                    break;
                case TriggerWhenValueIs.LessThanOrEqualTo:
                {
                    //if the new value is greater than the trigger value -> reset the trigger
                    if (triggered & newValue > TriggerValue)
                        ResetTrigger();

                    if (triggered)
                        return;

                    if (oldValue > TriggerValue & 
                        newValue <= TriggerValue)
                        Trigger();
                }
                    break;
                case TriggerWhenValueIs.GreaterThanOrEqualTo:
                {
                    //if the new value is less than the trigger value -> reset the trigger
                    if (triggered & newValue < TriggerValue)
                        ResetTrigger();

                    if (triggered)
                        return;

                    if (oldValue < TriggerValue & 
                        newValue >= TriggerValue)
                        Trigger();
                }
                    break;
                case TriggerWhenValueIs.GreaterThan:
                {
                    //if the new value is less than the trigger value -> reset the trigger
                    if (triggered & newValue < TriggerValue)
                        ResetTrigger();

                    if (triggered)
                        return;

                    if (oldValue <= TriggerValue & 
                        newValue > TriggerValue &
                        Math.Abs(oldValue - newValue) > TOLERANCE)
                        Trigger();
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnEnable()
        {
            triggered = false;
        }

        public virtual void Trigger()
        {
            triggered = true;
            OnTrigger?.Execute();
            // Debug.Log($"Triggering {name} - {GetType().Name} - at {TriggerValue}");
        }

        protected virtual void ResetTrigger()
        {
            triggered = false;
        }

        protected void LateUpdate()
        {
            if (Math.Abs(previousValue - value) < TOLERANCE) return; //value did not change
            OnValueChanged(previousValue, value);                    //trigger the OnValueChanged
            previousValue = value;                                   //update the previous value 
        }
    }
}
