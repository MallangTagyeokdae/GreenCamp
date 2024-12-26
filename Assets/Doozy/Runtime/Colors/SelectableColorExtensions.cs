// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Doozy.Runtime.Colors
{
    /// <summary>
    /// Extension methods for <see cref="SelectableColor"/>
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class SelectableColorExtensions
    {
        /// <summary>
        /// Set the <see cref="SelectionState"/> for the target <see cref="SelectableColor"/>
        /// </summary>
        /// <param name="target"> Target <see cref="SelectableColor"/> </param>
        /// <param name="state"> New <see cref="SelectionState"/> </param>
        /// <typeparam name="T"> <see cref="SelectableColor"/> </typeparam>
        /// <returns> Returns itself </returns>
        public static T SetState<T>(this T target, SelectionState state) where T : SelectableColor
        {
            target.SetSelectionState(state);
            return target;
        }

        /// <summary>
        /// Set a new color for both dark and light themes for the Normal state of the target <see cref="SelectableColor"/>
        /// </summary>
        /// <param name="target"> Target <see cref="SelectableColor"/> </param>
        /// <param name="color"> New Color </param>
        /// <typeparam name="T"> <see cref="SelectableColor"/> </typeparam>
        /// <returns> Returns itself </returns>
        public static T SetNormalColor<T>(this T target, Color color) where T : SelectableColor =>
            target.SetNormalColor(color, color);

        /// <summary>
        /// Set new colors for dark and light themes for the Normal state of the target <see cref="SelectableColor"/> 
        /// </summary>
        /// <param name="target"> Target <see cref="SelectableColor"/> </param>
        /// <param name="colorOnDark"> New Color for the dark theme </param>
        /// <param name="colorOnLight"> New Color for the light theme </param>
        /// <typeparam name="T"> <see cref="SelectableColor"/> </typeparam>
        /// <returns> Returns itself </returns>
        public static T SetNormalColor<T>(this T target, Color colorOnDark, Color colorOnLight) where T : SelectableColor
        {
            target.Normal.ColorOnDark = colorOnDark;
            target.Normal.ColorOnLight = colorOnLight;

            if (target.currentState == SelectionState.Normal)
                target.SelectionStateChanged(target.normalColor);

            return target;
        }

        /// <summary>
        /// Set a new color for both dark and light themes for the Highlighted state of the target <see cref="SelectableColor"/>
        /// </summary>
        /// <param name="target"> Target <see cref="SelectableColor"/> </param>
        /// <param name="color"> New Color </param>
        /// <typeparam name="T"> <see cref="SelectableColor"/> </typeparam>
        /// <returns> Returns itself </returns>
        public static T SetHighlightedColor<T>(this T target, Color color) where T : SelectableColor =>
            target.SetHighlightedColor(color, color);

        /// <summary>
        /// Set new colors for dark and light themes for the Highlighted state of the target <see cref="SelectableColor"/> 
        /// </summary>
        /// <param name="target"> Target <see cref="SelectableColor"/> </param>
        /// <param name="colorOnDark"> New Color for the dark theme </param>
        /// <param name="colorOnLight"> New Color for the light theme </param>
        /// <typeparam name="T"> <see cref="SelectableColor"/> </typeparam>
        /// <returns> Returns itself </returns>
        public static T SetHighlightedColor<T>(this T target, Color colorOnDark, Color colorOnLight) where T : SelectableColor
        {
            target.Highlighted.ColorOnDark = colorOnDark;
            target.Highlighted.ColorOnLight = colorOnLight;

            if (target.currentState == SelectionState.Highlighted)
                target.SelectionStateChanged(target.highlightedColor);

            return target;
        }

        /// <summary>
        /// Set a new color for both dark and light themes for the Pressed state of the target <see cref="SelectableColor"/>
        /// </summary>
        /// <param name="target"> Target <see cref="SelectableColor"/> </param>
        /// <param name="color"> New Color </param>
        /// <typeparam name="T"> <see cref="SelectableColor"/> </typeparam>
        /// <returns> Returns itself </returns>
        public static T SetPressedColor<T>(this T target, Color color) where T : SelectableColor =>
            target.SetPressedColor(color, color);

        /// <summary>
        /// Set new colors for dark and light themes for the Pressed state of the target <see cref="SelectableColor"/> 
        /// </summary>
        /// <param name="target"> Target <see cref="SelectableColor"/> </param>
        /// <param name="colorOnDark"> New Color for the dark theme </param>
        /// <param name="colorOnLight"> New Color for the light theme </param>
        /// <typeparam name="T"> <see cref="SelectableColor"/> </typeparam>
        /// <returns> Returns itself </returns>
        public static T SetPressedColor<T>(this T target, Color colorOnDark, Color colorOnLight) where T : SelectableColor
        {
            target.Pressed.ColorOnDark = colorOnDark;
            target.Pressed.ColorOnLight = colorOnLight;

            if (target.currentState == SelectionState.Pressed)
                target.SelectionStateChanged(target.pressedColor);

            return target;
        }

        /// <summary>
        /// Set a new color for both dark and light themes for the Selected state of the target <see cref="SelectableColor"/>
        /// </summary>
        /// <param name="target"> Target <see cref="SelectableColor"/> </param>
        /// <param name="color"> New Color </param>
        /// <typeparam name="T"> <see cref="SelectableColor"/> </typeparam>
        /// <returns> Returns itself </returns>
        public static T SetSelectedColor<T>(this T target, Color color) where T : SelectableColor =>
            target.SetSelectedColor(color, color);

        /// <summary>
        /// Set new colors for dark and light themes for the Selected state of the target <see cref="SelectableColor"/> 
        /// </summary>
        /// <param name="target"> Target <see cref="SelectableColor"/> </param>
        /// <param name="colorOnDark"> New Color for the dark theme </param>
        /// <param name="colorOnLight"> New Color for the light theme </param>
        /// <typeparam name="T"> <see cref="SelectableColor"/> </typeparam>
        /// <returns> Returns itself </returns>
        public static T SetSelectedColor<T>(this T target, Color colorOnDark, Color colorOnLight) where T : SelectableColor
        {
            target.Selected.ColorOnDark = colorOnDark;
            target.Selected.ColorOnLight = colorOnLight;

            if (target.currentState == SelectionState.Selected)
                target.SelectionStateChanged(target.selectedColor);

            return target;
        }

        /// <summary>
        /// Set a new color for both dark and light themes for the Disabled state of the target <see cref="SelectableColor"/>
        /// </summary>
        /// <param name="target"> Target <see cref="SelectableColor"/> </param>
        /// <param name="color"> New Color </param>
        /// <typeparam name="T"> <see cref="SelectableColor"/> </typeparam>
        /// <returns> Returns itself </returns>
        public static T SetDisabledColor<T>(this T target, Color color) where T : SelectableColor =>
            target.SetDisabledColor(color, color);

        /// <summary>
        /// Set new colors for dark and light themes for the Disabled state of the target <see cref="SelectableColor"/> 
        /// </summary>
        /// <param name="target"> Target <see cref="SelectableColor"/> </param>
        /// <param name="colorOnDark"> New Color for the dark theme </param>
        /// <param name="colorOnLight"> New Color for the light theme </param>
        /// <typeparam name="T"> <see cref="SelectableColor"/> </typeparam>
        /// <returns> Returns itself </returns>
        public static T SetDisabledColor<T>(this T target, Color colorOnDark, Color colorOnLight) where T : SelectableColor
        {
            target.Disabled.ColorOnDark = colorOnDark;
            target.Disabled.ColorOnLight = colorOnLight;

            if (target.currentState == SelectionState.Disabled)
                target.SelectionStateChanged(target.disabledColor);

            return target;
        }
    }
}
