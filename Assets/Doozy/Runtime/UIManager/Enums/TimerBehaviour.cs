// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Runtime.UIManager
{
    /// <summary> Defines how a Timer (DateTimeComponent) should behave </summary>
    public enum TimerBehaviour
    {
        /// <summary> Do nothing </summary>
        Disabled,
        
        /// <summary> Start the timer </summary>
        Start,
        
        /// <summary> Stop the timer </summary>
        Stop,
        
        /// <summary> Reset the timer and then start it </summary>
        ResetAndStart,
        
        /// <summary> Stop the timer and then reset it </summary>
        StopAndReset,
        
        /// <summary> Pause the timer </summary>
        Pause,
        
        /// <summary> Resume the timer </summary>
        Resume,
        
        /// <summary> Reset the timer </summary>
        Reset,
        
        /// <summary> Finish the timer </summary>
        Finish,
        
        /// <summary> Cancel the timer </summary>
        Cancel
    }
}
