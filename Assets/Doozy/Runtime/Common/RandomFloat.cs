// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Doozy.Runtime.Common
{
    /// <summary>
    /// Data class used to get random float values from a given [min,max] interval
    /// </summary>
    [Serializable]
    public class RandomFloat
    {
        /// <summary> Minimum value for the interval </summary>
        [SerializeField] private float MIN;
        public float min
        {
            get => MIN;
            set => MIN = value;
        }

        /// <summary> Maximum value for the interval </summary>
        [SerializeField] private float MAX;
        public float max
        {
            get => MAX;
            set => MAX = value;
        }

        /// <summary>
        /// Current random value from the [MIN,MAX] interval
        /// <para/> Value updated every time 'randomValue' is used
        /// </summary>
        public float currentValue { get; private set; }

        /// <summary>
        /// Previous random value
        /// <para/> Used to make sure no two consecutive random values are used
        /// </summary>
        public float previousValue { get; private set; }

        /// <summary>
        /// Random number between MIN [inclusive] and MAX [inclusive] (Read Only)
        /// <para/> Updates both the currentValue and the previousValue
        /// </summary>
        public float randomValue
        {
            get
            {
                previousValue = currentValue;
                currentValue = random;
                int counter = 100; //fail-safe counter to avoid infinite loops (if min = max)
                while (Mathf.Approximately(currentValue, previousValue) && counter > 0)
                {
                    currentValue = random;
                    counter--;
                }
                return currentValue;
            }
        }

        /// <summary> Random value from the [MIN,MAX] interval </summary>
        private float random => Random.Range(MIN, MAX);

        /// <summary> Construct a new RandomFloat using the [min,max] interval values from the other RandomFloat </summary>
        /// <param name="other"> Other RandomFloat </param>
        public RandomFloat(RandomFloat other) : this(other.min, other.max) {}

        /// <summary> Construct a new RandomFloat with the default [min, max] interval of [0,1] </summary>
        public RandomFloat() : this(0, 1) {}

        /// <summary> Construct a new RandomFloat with the given min and max interval values </summary>
        /// <param name="minValue"> Min value </param>
        /// <param name="maxValue"> Max value </param>
        public RandomFloat(float minValue, float maxValue) =>
            Reset(minValue, maxValue);

        /// <summary> Reset the interval to the given min and max values </summary>
        /// <param name="minValue"> Min value </param>
        /// <param name="maxValue"> Max value </param>
        public void Reset(float minValue = 0, float maxValue = 1)
        {
            MIN = minValue;
            MAX = maxValue;
            previousValue = currentValue = minValue;
            // previousValue = random; //set a random previous value
            // currentValue = randomValue; //init a current random value
        }
    }
}
