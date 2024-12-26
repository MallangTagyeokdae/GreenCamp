// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Components;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Components
{
    public class ContainerReactionControls : ReactionControls
    {
        public ContainerReactionControls AddContainerControls
        (
            UnityAction resetCallback,
            UnityAction showCallback,
            UnityAction hideCallback,
            UnityAction reverseShowCallback,
            UnityAction reverseHideCallback,
            UnityAction searchForAnimatorsCallback = null
        )
        {
            VisualElement resetButtonContainer =
                DesignUtils.row
                    .SetName("Reset Button Container")
                    .SetStyleDisplay(EditorApplication.isPlayingOrWillChangePlaymode ? DisplayStyle.None : DisplayStyle.Flex)
                    .SetStyleFlexGrow(0)
                    .SetStyleAlignItems(Align.Center)
                    .AddChild(GetResetButton(resetCallback))
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(DesignUtils.dividerVertical)
                    .AddChild(DesignUtils.spaceBlock2X);
            
            VisualElement editorOnlyContainer =
                DesignUtils.row
                    .SetName("Editor Only Container")
                    .SetStyleDisplay(EditorApplication.isPlayingOrWillChangePlaymode ? DisplayStyle.None : DisplayStyle.Flex)
                    .SetStyleFlexGrow(0)
                    .SetStyleAlignItems(Align.Center)
                    .AddChild(DesignUtils.dividerVertical)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild
                    (
                        //Reverse Show button
                        FluidButton.Get("r Show")
                            .SetTooltip("Reverse Show")
                            .SetIcon(EditorSpriteSheets.EditorUI.Icons.Show)
                            .SetButtonStyle(ButtonStyle.Contained)
                            .SetOnClick(reverseShowCallback)
                    )
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild
                    (
                        //Reverse Hide button
                        FluidButton.Get("r Hide")
                            .SetTooltip("Reverse Hide")
                            .SetIcon(EditorSpriteSheets.EditorUI.Icons.Hide)
                            .SetButtonStyle(ButtonStyle.Contained)
                            .SetOnClick(reverseHideCallback)
                    )
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(DesignUtils.dividerVertical)
                    .AddChild(DesignUtils.spaceBlock)
                    ;

            if (searchForAnimatorsCallback != null)
            {
                editorOnlyContainer
                    .AddChild
                    (
                        //Search for Animators button
                        GetNewButton(EditorSpriteSheets.EditorUI.Icons.Refresh, "Search for Animators\nUse this after you've added a new animator")
                            .SetOnClick(searchForAnimatorsCallback)
                    )
                    .AddChild(DesignUtils.spaceBlock);
            }

            return this
                .AddItem(resetButtonContainer)
                .AddShowButton(showCallback)
                .AddSpaceBlock()
                .AddHideButton(hideCallback)
                .AddSpaceBlock2X()
                .AddItem(editorOnlyContainer)
                .AddFlexibleSpace()
                .AddItem(icon);
        }

        public ContainerReactionControls AddShowButton(UnityAction callback) =>
            this.AddItem
            (
                FluidButton.Get()
                    .SetLabelText("Show")
                    .SetTooltip("Show")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Show)
                    .SetButtonStyle(ButtonStyle.Contained)
                    .ClearOnClick()
                    .SetOnClick(callback)
            );


        public ContainerReactionControls AddHideButton(UnityAction callback) =>
            this.AddItem
            (
                FluidButton.Get()
                    .SetLabelText("Hide")
                    .SetTooltip("Hide")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Hide)
                    .SetButtonStyle(ButtonStyle.Contained)
                    .ClearOnClick()
                    .SetOnClick(callback)
            );
    }
}
