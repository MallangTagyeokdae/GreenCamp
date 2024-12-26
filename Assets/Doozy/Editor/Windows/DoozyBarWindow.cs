// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI.Windows.Internal;
using Doozy.Editor.WindowLayouts;
using Doozy.Runtime.UIElements.Extensions;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Editor.Windows
{
    public class DoozyBarWindow : FluidWindow<DoozyBarWindow>
    {
        public const string k_WindowTitle = "Doozy Bar";
        public const string k_WindowMenuPath = "Tools/Doozy/Doozy Bar";

        // [MenuItem(k_WindowMenuPath, priority = -2000)]
        public static void Open() => InternalOpenWindow(k_WindowTitle);

        protected override void CreateGUI()
        {
            var layout = new DoozyBarWindowLayout();
            root.AddChild(layout);
        }
    }
}
