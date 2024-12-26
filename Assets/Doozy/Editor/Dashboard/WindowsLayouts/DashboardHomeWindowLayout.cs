// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Interfaces;
using Doozy.Editor.WindowLayouts;
using Doozy.Runtime.Colors;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Reactor.ScriptableObjects;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable UnusedType.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
// ReSharper disable UnusedMember.Local

namespace Doozy.Editor.Dashboard.WindowsLayouts
{
    public class DashboardHomeWindowLayout : FluidWindowLayout, IDashboardWindowLayout
    {
        public int order => 0;

        public override string layoutName => "Home";
        public sealed override List<Texture2D> animatedIconTextures => EditorSpriteSheets.EditorUI.Icons.DoozyUI;

        public override Color accentColor => EditorColors.Default.UnityThemeInversed;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Default.UnityThemeInversed;

        private HomeSection doozyBarSection { get; set; }
        private HomeSection inputSettingsSection { get; set; }
        private HomeSection reactorSettingsSection { get; set; }

        public DashboardHomeWindowLayout()
        {
            AddHeader("Doozy UI Manager", "Dashboard", animatedIconTextures);
            content
                .ResetLayout()
                .SetStylePadding(DesignUtils.k_Spacing)
                .SetStyleBackgroundImage(EditorTextures.Dashboard.Backgrounds.DashboardBackground)
                .SetStyleBackgroundScaleMode(ScaleMode.ScaleAndCrop)
                .SetStyleBackgroundImageTintColor(EditorColors.Default.MenuBackgroundLevel0);

            sideMenu.Dispose();

            Initialize();
            Compose();
        }

        private void Initialize()
        {
            doozyBarSection =
                new HomeSection()
                    .SetTitle("Quick Access")
                    .AddChild
                    (
                        new DoozyBarWindowLayout()
                            .SetStyleFlexDirection(FlexDirection.Column)
                    );

            inputSettingsSection =
                new HomeSection()
                    .SetTitle("Input Settings")
                    .AddChild
                    (
                        FluidField.Get("Input Handling").SetStyleFlexGrow(0).ClearBackground()
                            .AddFieldContent(DesignUtils.NewLabel(ObjectNames.NicifyVariableName(UIManagerInputSettings.k_InputHandling.ToString())))
                    )
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild
                    (
                        FluidField.Get("Multiplayer Mode").SetStyleFlexGrow(0).ClearBackground()
                            .AddFieldContent(DesignUtils.NewLabel(UIManagerInputSettings.instance.multiplayerMode ? "Enabled" : "Disabled"))
                    )
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild
                    (
                        FluidField.Get("'Back' Button Cooldown").SetStyleFlexGrow(0).ClearBackground()
                            .AddFieldContent(DesignUtils.NewLabel(UIManagerInputSettings.instance.backButtonCooldown + " seconds"))
                    );

            reactorSettingsSection =
                new HomeSection()
                    .SetTitle("Reactor Settings")
                    .AddChild
                    (
                        FluidField.Get("Editor Heartbeat").SetStyleFlexGrow(0).ClearBackground()
                            .AddFieldContent(DesignUtils.NewLabel(ObjectNames.NicifyVariableName(ReactorSettings.editorFPS.ToString()) + " FPS"))
                    )
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild
                    (
                        FluidField.Get("Runtime Heartbeat").SetStyleFlexGrow(0).ClearBackground()
                            .AddFieldContent(DesignUtils.NewLabel(ObjectNames.NicifyVariableName(ReactorSettings.runtimeFPS.ToString()) + " FPS"))
                    );
        }



        private void Compose()
        {
            content
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleFlexGrow(0)
                        .AddChild(doozyBarSection)
                        .AddChild(inputSettingsSection)
                        .AddChild(reactorSettingsSection)
                        .AddFlexibleSpace()
                )
                .AddFlexibleSpace()
                ;
        }

        private static VisualElement newSection =>
            new VisualElement()
                .SetStyleFlexShrink(0)
                .SetStylePadding(DesignUtils.k_Spacing2X)
                .SetStyleMargins(DesignUtils.k_Spacing / 2f)
                .SetStyleBackgroundColor(EditorColors.Default.BoxBackground.WithAlpha(0.9f))
                .SetStyleBorderRadius(DesignUtils.k_Spacing2X);

        private class HomeSection : VisualElement
        {
            public Label title { get; private set; }

            public HomeSection()
            {
                this
                    .SetStyleFlexShrink(0)
                    .SetStylePadding(DesignUtils.k_Spacing2X)
                    .SetStyleMargins(DesignUtils.k_Spacing / 2f)
                    .SetStyleBackgroundColor(EditorColors.Default.BoxBackground.WithAlpha(0.9f))
                    .SetStyleBorderRadius(DesignUtils.k_Spacing2X);

                title =
                    DesignUtils.NewLabel()
                        .SetStyleAlignSelf(Align.Center)
                        .SetStyleMarginBottom(DesignUtils.k_Spacing)
                        .SetStyleDisplay(DisplayStyle.None);

                this
                    .AddChild(title);
            }

            public HomeSection SetTitle(string text = "")
            {
                title
                    .SetText(text)
                    .SetStyleDisplay(text.IsNullOrEmpty() ? DisplayStyle.None : DisplayStyle.Flex);
                return this;
            }

            public HomeSection ClearTitle() =>
                SetTitle();
        }

    }


}
