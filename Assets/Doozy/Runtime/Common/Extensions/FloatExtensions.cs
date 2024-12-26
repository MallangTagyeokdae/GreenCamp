// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;

namespace Doozy.Runtime.Common.Extensions
{
    /// <summary> Extension methods for the float (single) struct </summary>
    public static class FloatExtensions
    {
        /// <summary> Round number with the given number of decimals </summary>
        /// <param name="target"> Target number </param>
        /// <param name="decimals"> Number of decimals </param>
        /// <returns> Rounded float </returns>
        public static float Round(this float target, int decimals = 1) =>
            (float)Math.Round(target, decimals);

        /// <summary> Clamp the value between the given minimum float and maximum float values </summary>
        /// <param name="target"> Target float </param>
        /// <param name="min"> The minimum floating point value to compare against </param>
        /// <param name="max"> The maximum floating point value to compare against </param>
        /// <returns> Clamped float </returns>
        public static float Clamp(this float target, float min, float max) =>
            Mathf.Clamp(target, min, max);

        /// <summary> Clamp value between 0 and 1 </summary>
        /// <param name="target"> Target float </param>
        /// <returns> Clamped float </returns>
        public static float Clamp01(this float target) =>
            Mathf.Clamp01(target);

        /// <summary> Compare two floating point values and returns true if they are similar </summary>
        /// <param name="target"> Target floating point </param>
        /// <param name="otherValue"> Other floating point to compare against </param>
        /// <returns> TRUE if the values are similar and FALSE otherwise </returns>
        public static bool Approximately(this float target, float otherValue) =>
            Mathf.Approximately(target, otherValue);
        
        /// <summary> Returns the absolute value of the given float </summary>
        /// <param name="target"> Target float </param>
        /// <returns> Absolute value of the given float </returns>
        public static float Abs(this float target) =>
            Mathf.Abs(target);
        
        /// <summary> Returns the smallest integer greater to or equal to the given float </summary>
        /// <param name="target"> Target float </param>
        /// <returns> Smallest integer greater to or equal to the given float </returns>
        public static float RoundToMultiple(this float target, float multiple)
        {
            float remainder = target % multiple;
            if (remainder < 0.5f) return target - remainder;
            return target + multiple - remainder;
        }
        
        /// <summary> Returns the smallest integer greater to or equal to the given float </summary>
        public static float RoundToMultiple(this float target, int multiple) =>
            target.RoundToMultiple((float)multiple);
        
        /// <summary> Returns the smallest integer greater to or equal to the given float </summary>
        public static float RoundToMultiple(this float target, float multiple, float offset)
        {
            float remainder = target % multiple;
            if (remainder < offset) return target - remainder;
            return target + multiple - remainder;
        }
        
        /// <summary> Returns the smallest integer greater to or equal to the given float </summary>
        /// <param name="target"> Target float </param>
        /// <param name="otherValue"> Other value </param>
        /// <param name="tolerance"> Tolerance (step) </param>
        public static bool CloseTo(this float target, float otherValue, float tolerance) =>
            Mathf.Abs(target - otherValue) <= tolerance;
        
    }
}
