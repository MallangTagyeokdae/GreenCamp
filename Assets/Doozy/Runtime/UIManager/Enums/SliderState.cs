// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Runtime.UIManager
{
    /// <summary> Defines what happened with a UISlider </summary>
    public enum SliderState
    {
        /// <summary>
        /// Slider value has reset to the default value
        /// </summary>
        Reset,
        
        /// <summary>
        /// Slider value has changed
        /// </summary>
        ValueChanged,
        
        /// <summary>
        /// Slider value has increased
        /// </summary>
        ValueIncremented,
        
        /// <summary>
        /// Slider value has decreased
        /// </summary>
        ValueDecremented,
        
        /// <summary>
        /// Slider value has reached the minimum value
        /// </summary>
        ReachedMinValue,
        
        /// <summary>
        /// Slider value has reached the maximum value
        /// </summary>
        ReachedMaxValue
    }
}
