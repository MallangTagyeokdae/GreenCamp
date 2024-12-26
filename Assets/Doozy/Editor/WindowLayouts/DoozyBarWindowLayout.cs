// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Nody;
using Doozy.Editor.Reactor.Windows;
using Doozy.Editor.Signals.Windows;
using Doozy.Editor.UIDesigner.Windows;
using Doozy.Editor.UIManager.UIMenu;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine.UIElements;

namespace Doozy.Editor.WindowLayouts
{
    public class DoozyBarWindowLayout : VisualElement
    {
        private VisualElement nodySection { get; set; }
        private VisualElement signalsSection { get; set; }
        private VisualElement uiMenuSection { get; set; }
        private VisualElement reactorSection { get; set; }
        private VisualElement uiDesignerSection { get; set; }

        public DoozyBarWindowLayout()
        {
            Initialize();
            Compose();
        }

        private void Initialize()
        {
            this
                .SetStyleFlexDirection(FlexDirection.Row)
                .SetStyleFlexWrap(Wrap.Wrap);

            InitializeNody();
            InitializeSignals();
            InitializeUIMenu();
            InitializeReactor();
            InitializeUIDesigner();
        }

        private void InitializeNody()
        {
            FluidButton nodyButton =
                newButton
                    .SetIcon(EditorSpriteSheets.Nody.Icons.Nody)
                    .SetAccentColor(EditorSelectableColors.Nody.Color)
                    .SetOnClick(NodyWindow.Open)
                    .SetTooltip("Open the Nody window");

            FluidButton newNodeButton =
                newButton
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Plus)
                    .SetAccentColor(EditorSelectableColors.Nody.Color)
                    .SetOnClick(NodyCreateNodeWindow.Open)
                    .SetElementSize(ElementSize.Small)
                    .SetTooltip("Open the Nody Create Node window");

            FluidButton refreshButton =
                newButton
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Refresh)
                    .SetAccentColor(EditorSelectableColors.Nody.Color)
                    .SetOnClick(NodyWindow.Refresh)
                    .SetElementSize(ElementSize.Tiny)
                    .SetTooltip("Refresh Nody by searching for all Nody node types in the project and updating the Nody window search menu");

            nodySection =
                newSection
                    .AddChild(nodyButton)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(newNodeButton)
                    .AddFlexibleSpace()
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(DesignUtils.dividerVertical)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(refreshButton);
        }

        private void InitializeSignals()
        {
            FluidButton signalsConsoleButton =
                newButton
                    .SetIcon(EditorSpriteSheets.Signals.Icons.Signal)
                    .SetAccentColor(EditorSelectableColors.Signals.Signal)
                    .SetOnClick(SignalsConsoleWindow.Open)
                    .SetTooltip("Open the Signals Console Window");

            FluidButton streamsConsoleButton =
                newButton
                    .SetIcon(EditorSpriteSheets.Signals.Icons.SignalStream)
                    .SetAccentColor(EditorSelectableColors.Signals.Stream)
                    .SetOnClick(StreamsConsoleWindow.Open)
                    .SetTooltip("Open the Streams Console Window");

            FluidButton refreshButton =
                newButton
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Refresh)
                    .SetAccentColor(EditorSelectableColors.Signals.Signal)
                    .SetOnClick(SignalsWindow.RefreshProviders)
                    .SetElementSize(ElementSize.Tiny)
                    .SetTooltip("Refresh Signals providers by searching for all provider types in the project and adding them to the system");

            signalsSection =
                newSection
                    .AddChild(signalsConsoleButton)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(streamsConsoleButton)
                    .AddFlexibleSpace()
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(DesignUtils.dividerVertical)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(refreshButton);
        }

        private void InitializeUIMenu()
        {
            FluidButton uiMenuButton =
                newButton
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.UIMenu)
                    .SetAccentColor(EditorSelectableColors.Default.UnityThemeInversed)
                    .SetOnClick(UIMenuWindow.Open)
                    .SetTooltip("Open the UIMenu window");

            FluidButton refreshButton =
                newButton
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Refresh)
                    .SetAccentColor(EditorSelectableColors.Default.UnityThemeInversed)
                    .SetOnClick(UIMenuWindow.RefreshDatabase)
                    .SetElementSize(ElementSize.Tiny)
                    .SetTooltip("Refresh the UIMenu by searching for all UIMenuItem assets in the project and updating the database");

            uiMenuSection =
                newSection
                    .AddChild(uiMenuButton)
                    .AddFlexibleSpace()
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(DesignUtils.dividerVertical)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(refreshButton);
        }

        private void InitializeReactor()
        {
            FluidButton editorHeartbeatButton =
                newButton
                    .SetIcon(EditorSpriteSheets.Reactor.Icons.EditorHeartbeat)
                    .SetAccentColor(EditorSelectableColors.Reactor.Red)
                    .SetOnClick(ReactorEditorTickerWindow.Open)
                    .SetTooltip("Editor Heartbeat");

            FluidButton runtimeHeartbeatButton =
                newButton
                    .SetIcon(EditorSpriteSheets.Reactor.Icons.Heartbeat)
                    .SetAccentColor(EditorSelectableColors.Reactor.Red)
                    .SetOnClick(ReactorRuntimeTickerWindow.Open)
                    .SetTooltip("Runtime Heartbeat");

            FluidButton refreshButton =
                newButton
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Refresh)
                    .SetAccentColor(EditorSelectableColors.Reactor.Red)
                    .SetOnClick(ReactorWindow.RefreshDatabase)
                    .SetElementSize(ElementSize.Tiny)
                    .SetTooltip("Refresh Reactor by searching for all animation presets in the project and adding them to the database");

            reactorSection =
                newSection
                    .AddChild(editorHeartbeatButton)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(runtimeHeartbeatButton)
                    .AddFlexibleSpace()
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(DesignUtils.dividerVertical)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(refreshButton);
        }

        private void InitializeUIDesigner()
        {
            FluidButton alignButton =
                newButton
                    .SetIcon(EditorSpriteSheets.UIDesigner.Icons.HorizontalAlignLeft)
                    .SetAccentColor(EditorSelectableColors.UIDesigner.Color)
                    // .SetLabelText("Align")
                    .SetOnClick(AlignWindow.Open)
                    .SetTooltip("Open the UIDesigner Align Window");

            FluidButton rotateButton =
                newButton
                    .SetIcon(EditorSpriteSheets.UIDesigner.Icons.Rotate)
                    .SetAccentColor(EditorSelectableColors.UIDesigner.Color)
                    // .SetLabelText("Rotate")
                    .SetOnClick(RotateWindow.Open)
                    .SetTooltip("Open the UIDesigner Rotate Window");

            FluidButton scaleButton =
                newButton
                    .SetIcon(EditorSpriteSheets.UIDesigner.Icons.ScaleIncrease)
                    .SetAccentColor(EditorSelectableColors.UIDesigner.Color)
                    // .SetLabelText("Scale")
                    .SetOnClick(ScaleWindow.Open)
                    .SetTooltip("Open the UIDesigner Scale Window");

            FluidButton sizeButton =
                newButton
                    .SetIcon(EditorSpriteSheets.UIDesigner.Icons.SizeIncrease)
                    .SetAccentColor(EditorSelectableColors.UIDesigner.Color)
                    // .SetLabelText("Size")
                    .SetOnClick(SizeWindow.Open)
                    .SetTooltip("Open the UIDesigner Size Window");

            uiDesignerSection =
                newSection
                    .AddChild(alignButton)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(rotateButton)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(scaleButton)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(sizeButton);
        }

        private void Compose()
        {
            this
                .AddChild(nodySection)
                .AddChild(signalsSection)
                .AddChild(uiMenuSection)
                .AddChild(reactorSection)
                .AddChild(uiDesignerSection);
        }

        private static VisualElement newSection =>
            new VisualElement()
                .SetStyleFlexDirection(FlexDirection.Row)
                .SetStyleFlexShrink(0)
                .SetStyleFlexGrow(1)
                .SetStyleAlignItems(Align.Center)
                .SetStylePadding(DesignUtils.k_Spacing2X)
                .SetStyleMargins(DesignUtils.k_Spacing / 2f)
                .SetStyleBackgroundColor(EditorColors.Default.WindowHeaderIcon)
                .SetStyleBorderRadius(DesignUtils.k_Spacing2X);

        private static FluidButton newButton =>
            FluidButton.Get()
                .SetStyleFlexShrink(0)
                .SetButtonStyle(ButtonStyle.Contained)
                .SetElementSize(ElementSize.Large);

    }
}
