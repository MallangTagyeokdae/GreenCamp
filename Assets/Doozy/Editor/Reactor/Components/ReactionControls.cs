// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Internal;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Reactor.Extensions;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using EditorStyles = Doozy.Editor.EditorUI.EditorStyles;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Reactor.Components
{
    public class ReactionControls : VisualElement, IDisposable
    {
        public static EditorSelectableColorInfo accentColor => EditorSelectableColors.Reactor.Red;

        public TemplateContainer templateContainer { get; }

        public VisualElement layoutContainer { get; }
        public VisualElement content { get; }

        public Image icon { get; set; }
        public Texture2DReaction iconReaction { get; set; }

        public virtual void Dispose()
        {
            content.RecycleAndClear();
        }

        public ReactionControls()
        {
            this
                .SetStyleMarginTop(DesignUtils.k_Spacing2X)
                .SetStyleMarginBottom(DesignUtils.k_Spacing2X);

            Add(templateContainer = EditorLayouts.Reactor.ReactionControls.CloneTree());
            templateContainer
                .AddStyle(EditorStyles.Reactor.ReactionControls);

            layoutContainer = templateContainer.Q<VisualElement>("LayoutContainer");
            content = layoutContainer.Q<VisualElement>("Content");

            Initialize();
            Compose();

            this
                .ResetBackgroundColor()
                .SetIcon(EditorSpriteSheets.Reactor.Icons.ReactorIcon);
        }

        protected virtual void Initialize()
        {

            content
                .SetStyleFlexGrow(1);

            icon =
                new Image()
                    .SetName("Icon")
                    .SetStyleAlignSelf(Align.Center)
                    .SetStyleBackgroundImageTintColor(EditorColors.Reactor.Red)
                    .SetStyleSize(20)
                    .SetStyleFlexShrink(0);
            icon.AddManipulator(new Clickable(() => iconReaction?.Play()));
            icon.RegisterCallback<PointerEnterEvent>(evt => iconReaction?.Play());
        }

        protected virtual void Compose()
        {
        }

        public static FluidButton GetNewButton(Texture2D texture, string tooltip = "") =>
            FluidButton.Get()
                .SetIcon(texture)
                .SetAccentColor(accentColor)
                .SetTooltip(tooltip)
                .SetElementSize(ElementSize.Tiny);
        
        public static FluidButton GetNewButton(IEnumerable<Texture2D> textures, string tooltip = "") =>
            FluidButton.Get()
                .SetIcon(textures)
                .SetAccentColor(accentColor)
                .SetTooltip(tooltip)
                .SetElementSize(ElementSize.Tiny);

        public static FluidButton GetResetButton(UnityAction callback) =>
            GetNewButton(EditorSpriteSheets.EditorUI.Icons.Undo, "Reset the target to the StartValue").SetOnClick(callback);
    }

    public static class ReactionControlsExtensions
    {   
        public static T AddItem<T>(this T target, VisualElement item) where T : ReactionControls
        {
            target.content.AddChild(item);
            return target;
        }
        
        public static T AddDivider<T>(this T target) where T : ReactionControls =>
            target.AddItem(DesignUtils.dividerVertical);

        public static T AddSpaceBlock<T>(this T target) where T : ReactionControls =>
            target.AddItem(DesignUtils.spaceBlock);

        public static T AddSpaceBlock2X<T>(this T target) where T : ReactionControls =>
            target.AddItem(DesignUtils.spaceBlock2X);

        public static T AddButton<T>(this T target, FluidButton button) where T : ReactionControls =>
            target.AddItem(button);

        public static T AddButton<T>(this T target, IEnumerable<Texture2D> textures, string tooltip, UnityAction callback) where T : ReactionControls
        {
            target.AddButton(ReactionControls.GetNewButton(textures, tooltip).SetOnClick(callback));
            return target;
        }

        public static T AddReactionControls<T>
        (
            this T target,
            UnityAction resetCallback,
            UnityAction fromCallback,
            UnityAction playForwardCallback,
            UnityAction stopCallback,
            UnityAction playReverseCallback,
            UnityAction reverseCallback,
            UnityAction toCallback
        ) where T : ReactionControls
        {
            VisualElement editorOnlyContainer =
                    DesignUtils.row
                        .SetName("Editor Only Container")
                        .SetStyleDisplay(EditorApplication.isPlayingOrWillChangePlaymode ? DisplayStyle.None : DisplayStyle.Flex)
                        .SetStyleFlexGrow(0)
                        .SetStyleAlignItems(Align.Center)
                        .AddChild(ReactionControls.GetResetButton(resetCallback))
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(DesignUtils.dividerVertical)
                        .AddChild(DesignUtils.spaceBlock);
            
            return target
                .AddItem(editorOnlyContainer)
                .AddFromButton(fromCallback)
                .AddSpaceBlock2X()
                .AddPlayForwardButton(playForwardCallback)
                .AddSpaceBlock()
                .AddStopButton(stopCallback)
                .AddSpaceBlock()
                .AddPlayInReverseButton(playReverseCallback)
                .AddSpaceBlock2X()
                .AddReverseButton(reverseCallback)
                .AddSpaceBlock2X()
                .AddToButton(toCallback)
                .AddSpaceBlock2X()
                .AddFlexibleSpace()
                .AddItem(target.icon);
        }

        public static T AddResetButton<T>(this T target, UnityAction callback) where T : ReactionControls =>
            target.AddItem(ReactionControls.GetResetButton(callback));

        public static T AddFromButton<T>(this T target, UnityAction callback) where T : ReactionControls =>
            target.AddButton(EditorSpriteSheets.EditorUI.Icons.FirstFrame, "From", callback);

        public static T AddToButton<T>(this T target, UnityAction callback) where T : ReactionControls =>
            target.AddButton(EditorSpriteSheets.EditorUI.Icons.LastFrame, "To", callback);

        public static T AddPlayForwardButton<T>(this T target, UnityAction callback) where T : ReactionControls =>
            target.AddButton(EditorSpriteSheets.EditorUI.Icons.PlayForward, "Play Forward", callback);

        public static T AddPlayInReverseButton<T>(this T target, UnityAction callback) where T : ReactionControls =>
            target.AddButton(EditorSpriteSheets.EditorUI.Icons.PlayReverse, "Play in Reverse", callback);

        public static T AddStopButton<T>(this T target, UnityAction callback) where T : ReactionControls =>
            target.AddButton(EditorSpriteSheets.EditorUI.Icons.Stop, "Stop", callback);

        public static T AddReverseButton<T>(this T target, UnityAction callback) where T : ReactionControls =>
            target.AddButton(EditorSpriteSheets.EditorUI.Icons.Reverse, "Reverse", callback);

        /// <summary> Reset the background color of the controls to the default </summary>
        public static T ResetBackgroundColor<T>(this T controls) where T : ReactionControls =>
            controls.SetBackgroundColor(EditorColors.Default.Background);

        /// <summary> Set the background color of the controls </summary>
        /// <param name="controls"> The controls to set the background color of </param>
        /// <param name="color"> The color to set the controls to </param>
        public static T SetBackgroundColor<T>(this T controls, Color color) where T : ReactionControls
        {
            controls.content.SetStyleBackgroundColor(color);
            return controls;
        }


        /// <summary> Set Animated Icon </summary>
        /// <param name="target"> Target Button </param>
        /// <param name="textures"> Icon textures </param>
        public static T SetIcon<T>(this T target, IEnumerable<Texture2D> textures) where T : ReactionControls
        {
            if (target.iconReaction == null)
            {
                target.iconReaction = target.icon.GetTexture2DReaction(textures).SetEditorHeartbeat().SetDuration(0.6f);
            }
            else
            {
                target.iconReaction.SetTextures(textures);
            }
            target.icon.SetStyleDisplay(DisplayStyle.Flex);
            return target;
        }

        /// <summary> Set Static Icon </summary>
        /// <param name="target"> Target Button </param>
        /// <param name="iconTexture2D"> Icon texture </param>
        public static T SetIcon<T>(this T target, Texture2D iconTexture2D) where T : ReactionControls
        {
            target.iconReaction?.Recycle();
            target.iconReaction = null;
            target.icon.SetStyleBackgroundImage(iconTexture2D);
            target.icon.SetStyleDisplay(DisplayStyle.Flex);
            return target;
        }

        /// <summary> Clear the icon. If the icon is animated, its reaction will get recycled </summary>
        /// <param name="target"> Target Button </param>
        public static T ClearIcon<T>(this T target) where T : ReactionControls
        {
            target.iconReaction?.Recycle();
            target.iconReaction = null;
            target.icon.SetStyleBackgroundImage((Texture2D)null);
            target.icon.SetStyleDisplay(DisplayStyle.None);
            return target;
        }
    }
}
