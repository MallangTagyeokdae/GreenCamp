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
    /// Specialized reaction built to work with a ReflectedInt
    /// Designed to animate a int value over time
    /// </summary>
    [Serializable]
    public class ReflectedIntReaction : IntReaction
    {
        /// <summary> Int value target accessed via reflection </summary>
        public ReflectedInt valueTarget { get; private set; }
        
          [SerializeField] private bool Enabled;
        /// <summary> Enable or disable the animation </summary>
        public bool enabled
        {
            get => Enabled;
            set => Enabled = value;
        }

        [SerializeField] private int StartValue;
        /// <summary> Start value </summary>
        public int startValue
        {
            get => StartValue;
            set
            {
                StartValue = value;
                if (isActive) UpdateValues();
            }
        }

        /// <summary> Current reflected value (can be different from the animation's currentValue) </summary>
        public int currentReflectedValue
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

        [SerializeField] private int FromCustomValue;
        /// <summary> Custom used to calculate the From value, when the FromReferenceValue is set to CustomValue </summary>
        public int fromCustomValue
        {
            get => FromCustomValue;
            set => FromCustomValue = value;
        }

        [SerializeField] private int ToCustomValue;
        /// <summary> Custom used to calculate the To value, when the ToReferenceValue is set to CustomValue </summary>
        public int toCustomValue
        {
            get => ToCustomValue;
            set => ToCustomValue = value;
        }

        [SerializeField] private int FromOffset;
        /// <summary> Offset value added to the From value, when the FromReferenceValue is set to either StartValue or CurrentValue </summary>
        public int fromOffset
        {
            get => FromOffset;
            set => FromOffset = value;
        }

        [SerializeField] private int ToOffset;
        /// <summary> Offset value added to the To value, when the ToReferenceValue is set to either StartValue or CurrentValue </summary>
        public int toOffset
        {
            get => ToOffset;
            set => ToOffset = value;
        }

        public override void Reset()
        {
            base.Reset();

            valueTarget = null;

            FromReferenceValue = ReferenceValue.StartValue;
            FromCustomValue = 0;
            FromOffset = 0;

            ToReferenceValue = ReferenceValue.StartValue;
            ToCustomValue = 100;
            ToOffset = 0;
        }

        /// <summary> Set the target reflected value for this reaction </summary>
        /// <param name="target"> Reflected Value </param>
        public ReflectedIntReaction SetTarget(ReflectedInt target)
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
        
        private int GetValue(ReferenceValue referenceValue, int offset, int customValue)
        {
            int value = referenceValue switch
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
