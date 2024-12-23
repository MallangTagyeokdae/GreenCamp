// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Components;
using Doozy.Editor.Reactor.Ticker;
using Doozy.Editor.UIElements;
using Doozy.Editor.UIManager.Editors.Animators.Internal;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Animators;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Animators
{
    [CustomEditor(typeof(UIContainerUIAnimator), true)]
    [CanEditMultipleObjects]
    public class UIContainerUIAnimatorEditor : BaseUIContainerAnimatorEditor
    {
        public UIContainerUIAnimator castedTarget => (UIContainerUIAnimator)target;
        public IEnumerable<UIContainerUIAnimator> castedTargets => targets.Cast<UIContainerUIAnimator>();

        private UIAnimationTab showUIAnimationTab { get; set; }
        private UIAnimationTab hideUIAnimationTab { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            showUIAnimationTab?.Dispose();
            hideUIAnimationTab?.Dispose();
        }

        protected override void ResetAnimatorInitializedState()
        {
            foreach (var a in castedTargets)
                a.animatorInitialized = false;
        }

        protected override void ResetToStartValues()
        {
            foreach (var a in castedTargets)
            {
                a.StopAllReactions();
                a.ResetToStartValues();
            }
        }

        protected override void PlayShow()
        {
            foreach (var a in castedTargets)
                a.Show();
        }

        protected override void PlayHide()
        {
            foreach (var a in castedTargets)
                a.Hide();
        }

        protected override void PlayReverseShow()
        {
            foreach (var a in castedTargets)
                a.ReverseShow();
        }

        protected override void PlayReverseHide()
        {
            foreach (var a in castedTargets)
                a.ReverseHide();
        }

        protected override void HeartbeatCheck()
        {
            if (Application.isPlaying) return;
            foreach (var a in castedTargets)
                if (a.animatorInitialized == false)
                {
                    a.InitializeAnimator();
                    foreach (EditorHeartbeat eh in a.SetHeartbeat<EditorHeartbeat>().Cast<EditorHeartbeat>())
                        eh.StartSceneViewRefresh(a);
                }
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentTypeText("UIAnimator")
                .SetIcon(EditorSpriteSheets.Reactor.Icons.UIAnimator)
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048707091/UIContainer+UI+Animator?atlOrigin=eyJpIjoiZGRjNzc3ZGFkZDljNGQwMGJmMDMzMTMwMmFmYTNjNTUiLCJwIjoiYyJ9")
                .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.UIManager.Animators.UIContainerUIAnimator.html")
                .AddYouTubeButton();

            //refresh tabs enabled indicator
            root.schedule.Execute(() =>
            {
                void UpdateIndicator(UIAnimationTab tab, UIAnimation animation, bool animateChange)
                {
                    bool move = animation.Move.enabled;
                    bool rotate = animation.Rotate.enabled;
                    bool scale = animation.Scale.enabled;
                    bool fade = animation.Fade.enabled;

                    if (tab.moveIndicator.isOn != move)
                        tab.moveIndicator.Toggle(move, animateChange);

                    if (tab.rotateIndicator.isOn != rotate)
                        tab.rotateIndicator.Toggle(rotate, animateChange);

                    if (tab.scaleIndicator.isOn != scale)
                        tab.scaleIndicator.Toggle(scale, animateChange);

                    if (tab.fadeIndicator.isOn != fade)
                        tab.fadeIndicator.Toggle(fade, animateChange);
                }

                //initial indicators state update (no animation)
                UpdateIndicator(showUIAnimationTab, castedTarget.showAnimation, false);
                UpdateIndicator(hideUIAnimationTab, castedTarget.hideAnimation, false);

                //subsequent indicators state update (animated)
                root.schedule.Execute(() =>
                {
                    UpdateIndicator(showUIAnimationTab, castedTarget.showAnimation, true);
                    UpdateIndicator(hideUIAnimationTab, castedTarget.hideAnimation, true);

                }).Every(200);
            });
        }

        protected override void InitializeShow()
        {
            base.InitializeShow();

            //remove the default tab and replace it with the ui animation tab
            showTab?.RemoveFromToggleGroup();
            showTab?.Recycle();

            showUIAnimationTab =
                UIAnimationTab.Get()
                    .SetLabelText("Show")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Show)
                    .ButtonSetOnValueChanged(evt => showAnimatedContainer.Toggle(evt.newValue, evt.animateChange))
                    .AddToToggleGroup(tabsGroup);
        }
        
        protected override void InitializeHide()
        {
            base.InitializeHide();

            //remove the default tab and replace it with the ui animation tab
            hideTab?.RemoveFromToggleGroup();
            hideTab?.Recycle();

            hideUIAnimationTab =
                UIAnimationTab.Get()
                    .SetLabelText("Hide")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Hide)
                    .ButtonSetOnValueChanged(evt => hideAnimatedContainer.Toggle(evt.newValue, evt.animateChange))
                    .AddToToggleGroup(tabsGroup);
        }
        
        protected override VisualElement Toolbar()
        {
            return base.Toolbar()
                .RecycleAndClear()
                .AddChild(showUIAnimationTab)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(hideUIAnimationTab)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(DesignUtils.flexibleSpace);
        }
    }
}
