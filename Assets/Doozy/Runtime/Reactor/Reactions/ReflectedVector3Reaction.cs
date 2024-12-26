// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reflection;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Reactions
{
    /// <summary>
    /// Specialized reaction built to work with a ReflectedFloat
    /// Designed to animate a float value over time
    /// </summary>
    [Serializable]
    public class ReflectedVector3Reaction : Vector3Reaction
    {
        /// <summary> Vector3 value target accessed via reflection </summary>
        public ReflectedVector3 valueTarget { get; private set; }
        
        [SerializeField] private bool Enabled;
        /// <summary> Enable or disable the animation </summary>
        public bool enabled
        {
            get => Enabled;
            set => Enabled = value;
        }
        
        [SerializeField] private Vector3 StartValue;
        /// <summary> Start value </summary>
        public Vector3 startValue
        {
            get => StartValue;
            set
            {
                StartValue = value;
                if (isActive) UpdateValues();
            }
        }

        /// <summary> Current reflected value (can be different from the animation's currentValue) </summary>
        public Vector3 currentReflectedValue
        {
            get => valueTarget.value;
            set => valueTarget.value = value;
        }
        
        [SerializeField] private ReferenceValue FromReferenceValue = ReferenceValue.StartValue;
        /// <summary> Selector for the reference value used to calculate the From value </summary>
        public ReferenceValue fromReferenceValue
        {
            get => FromReferenceValue;
            set => FromReferenceValue = value;
        }

        [SerializeField] private ReferenceValue ToReferenceValue = ReferenceValue.StartValue;
        /// <summary> Selector for the reference value used to calculate the To value </summary>
        public ReferenceValue toReferenceValue
        {
            get => ToReferenceValue;
            set => ToReferenceValue = value;
        }

        [SerializeField] private Vector3 FromCustomValue;
        /// <summary> Custom used to calculate the From value, when the FromReferenceValue is set to CustomValue </summary>
        public Vector3 fromCustomValue
        {
            get => FromCustomValue;
            set => FromCustomValue = value;
        }

        [SerializeField] private Vector3 ToCustomValue;
        /// <summary> Custom used to calculate the To value, when the ToReferenceValue is set to CustomValue </summary>
        public Vector3 toCustomValue
        {
            get => ToCustomValue;
            set => ToCustomValue = value;
        }

        [SerializeField] private Vector3 FromOffset;
        /// <summary> Offset value added to the From value, when the FromReferenceValue is set to either StartValue or CurrentValue </summary>
        public Vector3 fromOffset
        {
            get => FromOffset;
            set => FromOffset = value;
        }

        [SerializeField] private Vector3 ToOffset;
        /// <summary> Offset value added to the To value, when the ToReferenceValue is set to either StartValue or CurrentValue </summary>
        public Vector3 toOffset
        {
            get => ToOffset;
            set => ToOffset = value;
        }
        
        public override void Reset()
        {
            base.Reset();

            valueTarget = null;

            FromReferenceValue = ReferenceValue.StartValue;
            FromCustomValue = Vector3.zero;
            FromOffset =Vector3.zero;

            ToReferenceValue = ReferenceValue.StartValue;
            ToCustomValue = Vector3.one;
            ToOffset = Vector3.zero;
        }

        /// <summary> Set the target reflected value for this reaction </summary>
        /// <param name="target"> Reflected Value </param>
        public ReflectedVector3Reaction SetTarget(ReflectedVector3 target)
        {
            this.SetTargetObject(target);
            valueTarget = target;
            startValue = target.value;
            getter = () => currentReflectedValue;
            setter = value => currentReflectedValue = value;
            return this;
        }
        
        /// <summary> Play the reaction </summary>
        /// <param name="inReverse"> If TRUE, the reaction will play in reverse </param>
        public override void Play(bool inReverse = false)
        {
            if (!isActive)
            {
                UpdateValues();
                SetValue(inReverse ? ToValue : FromValue);
            }
            base.Play(inReverse);
        }

        /// <summary> Play the reaction from the given start progress (from) to the current progress </summary>
        /// <param name="fromProgress"> From (start) progress </param>
        public override void PlayFromProgress(float fromProgress)
        {
            UpdateValues();
            base.PlayFromProgress(fromProgress);
        }
        
        /// <summary> Set the reaction's progress at the given target progress </summary>
        /// <param name="targetProgress"> Target progress </param>
        public override void SetProgressAt(float targetProgress)
        {
            UpdateValues();
            base.SetProgressAt(targetProgress);
        }

        /// <summary> Update From and To values by recalculating them </summary>
        public void UpdateValues()
        {
            SetFrom(GetValue(FromReferenceValue, FromOffset, FromCustomValue));
            SetTo(GetValue(ToReferenceValue, ToOffset, ToCustomValue));
        }
        
        private Vector3 GetValue(ReferenceValue referenceValue, Vector3 offset, Vector3 customValue)
        {
            Vector3 value = referenceValue switch
                            {
                                ReferenceValue.StartValue   => startValue + offset,
                                ReferenceValue.CurrentValue => currentReflectedValue + offset,
                                ReferenceValue.CustomValue  => customValue,
                                _                           => throw new ArgumentOutOfRangeException()
                            };
            return value;
        }
    }
}
