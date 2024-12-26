// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.UIElements.Extensions;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.EditorUI.Components
{
    /// <summary> A tab button with an enabled indicator attached </summary>
    public class FluidTab : FluidTabBase<FluidTab>
    {
        public FluidTab()
        {
            Reset();
        }

        public override void Reset()
        {
            base.Reset();

            this
                .ResetStyleSize()
                .ResetTabPosition()
                .SetElementSize(ElementSize.Small)
                .ButtonSetContainerColorOff(DesignUtils.tabButtonColorOff);
        }
    }
}