// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;

namespace Doozy.Runtime.Colors
{
    /// <summary>
    /// A pair of colors, one for a dark theme and one for a light theme
    /// </summary>
    [Serializable]
    public class ThemeColor
    {
        /// <summary> Returns TRUE if the dark theme is active </summary>
        public virtual bool isDarkTheme { get; set; }

        /// <summary> Color for the dark theme </summary>
        public Color ColorOnDark;
        /// <summary> Color for the light theme </summary>
        public Color ColorOnLight;

        /// <summary> If isDarkTheme is TRUE it returns the color on dark, otherwise it returns the color on light </summary>
        public Color color =>
            isDarkTheme
                ? ColorOnDark
                : ColorOnLight;

        /// <summary>
        /// Construct a new <see cref="ThemeColor"/>
        /// </summary>
        public ThemeColor()
        {
            ColorOnDark = ColorOnLight = Color.white;
        }
    }
}
