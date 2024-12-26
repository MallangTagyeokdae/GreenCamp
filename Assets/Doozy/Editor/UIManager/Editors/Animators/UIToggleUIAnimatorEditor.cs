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

namespace Doozy.Editor.UIManager.Editors.Animators
{
    [CustomEditor(typeof(UIToggleUIAnimator))]
    [CanEditMultipleObjects]
    public class UIToggleUIAnimatorEditor : BaseUIToggleAnimatorEditor
    {
        public UIToggleUIAnimator castedTarget => (UIToggleUIAnimator) target;
        public List<UIToggleUIAnimator> castedTargets => targets.Cast<UIToggleUIAnimator>().ToList();
        
        private UIAnimationTab onUIAnimationTab { get; set; }
        private UIAnimationTab offUIAnimationTab { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            onUIAnimationTab?.Dispose();
            offUIAnimationTab?.Dispose();
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
        
        protected override void PlayIsOn()
        {
            foreach (var a in castedTargets)
                a.PlayOnAnimation();
        }
        
        protected override void PlayIsOff()
        {
            foreach (var a in castedTargets)
                a.PlayOffAnimation();
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
                .AddManualButton()
                .AddApiButton()
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
                UpdateIndicator(onUIAnimationTab, castedTarget.onAnimation, false);
                UpdateIndicator(offUIAnimationTab, castedTarget.offAnimation, false);

                //subsequent indicators state update (animated)
                root.schedule.Execute(() =>
                {
                    UpdateIndicator(onUIAnimationTab, castedTarget.onAnimation, true);
                    UpdateIndicator(offUIAnimationTab, castedTarget.offAnimation, true);

                }).Every(200);
            });
        }

        protected override void InitializeOn()
        {
            base.InitializeOn();

            //remove the default tab and replace it with the ui animation tab
            onTab?.RemoveFromToggleGroup();
            onTab?.Recycle();

            onUIAnimationTab =
                UIAnimationTab.Get()
                    .SetLabelText("Show")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Show)
                    .ButtonSetOnValueChanged(evt => onAnimatedContainer.Toggle(evt.newValue, evt.animateChange))
                    .AddToToggleGroup(tabsGroup);
        }
        
        protected override void InitializeOff()
        {
            base.InitializeOff();

            //remove the default tab and replace it with the ui animation tab
            offTab?.RemoveFromToggleGroup();
            offTab?.Recycle();

            offUIAnimationTab =
                UIAnimationTab.Get()
                    .SetLabelText("Hide")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Hide)
                    .ButtonSetOnValueChanged(evt => offAnimatedContainer.Toggle(evt.newValue, evt.animateChange))
                    .AddToToggleGroup(tabsGroup);
        }
        
        protected override VisualElement Toolbar()
        {
            return base.Toolbar()
                .RecycleAndClear()
                .AddChild(onUIAnimationTab)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(offUIAnimationTab)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(DesignUtils.flexibleSpace);
        }
    }
}
