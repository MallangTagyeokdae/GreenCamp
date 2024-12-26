// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Runtime.Colors
{
    /// <summary> An enumeration of the possible selections states an UI object can be in </summary>
    public enum SelectionState
    {
        /// <summary> UI object can be selected </summary>
        Normal,

        /// <summary> UI object is highlighted </summary>
        Highlighted,

        /// <summary> UI object is pressed </summary>
        Pressed,

        /// <summary> UI object is selected </summary>
        Selected,

        /// <summary> UI object cannot be selected </summary>
        Disabled,
    }
}
