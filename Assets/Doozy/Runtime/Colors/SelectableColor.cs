// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming

namespace Doozy.Runtime.Colors
{
    /// <summary>
    /// A set of colors the for every possible selectable state: Normal, Highlighted, Pressed, Selected and Disabled
    /// </summary>
    public class SelectableColor
    {
        private bool m_IsDarkTheme;
        /// <summary> Returns TRUE if this color set is for a dark theme </summary>
        public bool isDarkTheme
        {
            get => m_IsDarkTheme;
            set
            {
                m_IsDarkTheme = value;
                Normal.isDarkTheme = m_IsDarkTheme;
                Highlighted.isDarkTheme = m_IsDarkTheme;
                Pressed.isDarkTheme = m_IsDarkTheme;
                Selected.isDarkTheme = m_IsDarkTheme;
                Disabled.isDarkTheme = m_IsDarkTheme;
                SelectionStateChanged(currentColor);
            }
        }

        /// <summary> Get the current <see cref="SelectionState"/> for this colors set </summary>
        public SelectionState currentState
        {
            get;
            private set;
        }

        /// <summary> UnityEvent triggered when the the current color changed due to a state change </summary>
        public readonly ColorEvent onStateChanged;
        /// <summary> Current active color </summary>
        public Color currentColor => GetCurrentColor();

        /// <summary> Theme color for the Normal <see cref="SelectionState"/> </summary>
        public readonly ThemeColor Normal;
        /// <summary> Color for the Normal state (takes isDarkTheme into account) </summary>
        public Color normalColor => Normal.color;

        /// <summary> Theme color for the Highlighted <see cref="SelectionState"/> </summary>
        public readonly ThemeColor Highlighted;
        /// <summary> Color for the Highlighted state (takes isDarkTheme into account) </summary>
        public Color highlightedColor => Highlighted.color;

        /// <summary> Theme color for the Pressed <see cref="SelectionState"/> </summary>
        public readonly ThemeColor Pressed;
        /// <summary> Color for the Pressed state (takes isDarkTheme into account) </summary>
        public Color pressedColor => Pressed.color;

        /// <summary> Theme color for the Selected <see cref="SelectionState"/> </summary>
        public readonly ThemeColor Selected;
        /// <summary> Color for the Selected state (takes isDarkTheme into account) </summary>
        public Color selectedColor => Selected.color;

        /// <summary> Theme color for the Disabled <see cref="SelectionState"/> </summary>
        public readonly ThemeColor Disabled;
        /// <summary> Color for the Disabled state (takes isDarkTheme into account) </summary>
        public Color disabledColor => Disabled.color;

        /// <summary>
        /// Construct a new <see cref="SelectableColor"/>
        /// </summary>
        /// <param name="onStateChanged"> Target color event </param>
        public SelectableColor(ColorEvent onStateChanged = null)
        {
            Normal = new ThemeColor { isDarkTheme = isDarkTheme };
            Highlighted = new ThemeColor { isDarkTheme = isDarkTheme };
            Pressed = new ThemeColor { isDarkTheme = isDarkTheme };
            Selected = new ThemeColor { isDarkTheme = isDarkTheme };
            Disabled = new ThemeColor { isDarkTheme = isDarkTheme };

            this.onStateChanged = onStateChanged ?? new ColorEvent();
            SetSelectionState(SelectionState.Normal);
        }

        private Color GetCurrentColor()
        {
            switch (currentState)
            {
                case SelectionState.Normal: return normalColor;
                case SelectionState.Highlighted: return highlightedColor;
                case SelectionState.Pressed: return pressedColor;
                case SelectionState.Selected: return selectedColor;
                case SelectionState.Disabled: return disabledColor;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        internal void SetSelectionState(SelectionState state)
        {
            currentState = state;
            SelectionStateChanged(currentColor);
        }

        internal void SelectionStateChanged(Color color)
        {
            onStateChanged?.Invoke(color);
        }
    }
}
