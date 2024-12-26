// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Components;
using Doozy.Editor.Reactor.Editors.Animators.Internal;
using Doozy.Editor.Reactor.Ticker;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animators;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Reactor.Editors.Animators
{
    [CustomEditor(typeof(UIAnimator), true)]
    [CanEditMultipleObjects]
    public class UIAnimatorEditor : BaseReactorAnimatorEditor
    {
        private UIAnimator castedTarget => (UIAnimator)target;
        private List<UIAnimator> castedTargets => targets.Cast<UIAnimator>().ToList();

        private UIAnimationTab uiAnimationTab { get; set; }

        private SerializedProperty propertyMoveEnabled { get; set; }
        private SerializedProperty propertyRotateEnabled { get; set; }
        private SerializedProperty propertyScaleEnabled { get; set; }
        private SerializedProperty propertyFadeEnabled { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiAnimationTab?.Dispose();
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
                a.Stop();
                a.ResetToStartValues();
            }
        }
        
        protected override void SetProgressAtZero() =>
            castedTargets.ForEach(a => a.SetProgressAtZero());

        protected override void PlayForward() =>
            castedTargets.ForEach(a => a.Play(PlayDirection.Forward));
        
        protected override void Stop() =>
            castedTargets.ForEach(a => a.Stop());
        
        protected override void PlayReverse() =>
            castedTargets.ForEach(a => a.Play(PlayDirection.Reverse));
        
        protected override void Reverse() =>
            castedTargets.ForEach(a => a.Reverse());
        
        protected override void SetProgressAtOne() =>
            castedTargets.ForEach(a => a.SetProgressAtOne());

        protected override void HeartbeatCheck()
        {
            if (Application.isPlaying) return;
            foreach (var a in castedTargets)
            {
                if (a.animatorInitialized) continue;
                resetToStartValue = true;
                a.InitializeAnimator();
                foreach (EditorHeartbeat eh in a.SetHeartbeat<EditorHeartbeat>().Cast<EditorHeartbeat>())
                    eh.StartSceneViewRefresh(a);
            }
        }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyMoveEnabled = serializedObject.FindProperty("Animation.Move.Enabled");
            propertyRotateEnabled = serializedObject.FindProperty("Animation.Rotate.Enabled");
            propertyScaleEnabled = serializedObject.FindProperty("Animation.Scale.Enabled");
            propertyFadeEnabled = serializedObject.FindProperty("Animation.Fade.Enabled");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText("UI Animator")
                .SetIcon(EditorSpriteSheets.Reactor.Icons.UIAnimator)
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048150050/UI+Animator?atlOrigin=eyJpIjoiYzMxOTZiNGQwYmRjNGVkNTkxOTA1MGYyNzBlMGFmZWIiLCJwIjoiYyJ9")
                .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.Reactor.Animators.UIAnimator.html")
                .AddYouTubeButton();
        }

        protected override void InitializeAnimation()
        {
            base.InitializeAnimation();

            //remove the default animation tab and replace it with the color tab
            animationTab.button.RemoveFromToggleGroup();
            animationTab?.Recycle();

            uiAnimationTab =
                UIAnimationTab.Get()
                    .SetLabelText("Animation")
                    .SetIcon(EditorSpriteSheets.Reactor.Icons.UIAnimation)
                    .ButtonSetAccentColor(selectableAccentColor)
                    .ButtonSetOnValueChanged(evt => animationAnimatedContainer.Toggle(evt.newValue));

            uiAnimationTab.button.AddToToggleGroup(tabsGroup);

            //refresh animationTab enabled indicators
            root.schedule.Execute(() =>
            {
                void UpdateIndicator(UIAnimationTab tab, bool move, bool rotate, bool scale, bool fade, bool animateChange)
                {
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
                UpdateIndicator
                (
                    uiAnimationTab,
                    propertyMoveEnabled.boolValue,
                    propertyRotateEnabled.boolValue,
                    propertyScaleEnabled.boolValue,
                    propertyFadeEnabled.boolValue,
                    false
                );

                root.schedule.Execute(() =>
                {
                    //subsequent indicators state update (animated)
                    UpdateIndicator
                    (
                        uiAnimationTab,
                        propertyMoveEnabled.boolValue,
                        propertyRotateEnabled.boolValue,
                        propertyScaleEnabled.boolValue,
                        propertyFadeEnabled.boolValue,
                        true
                    );

                }).Every(200);
            });
        }

        protected override VisualElement Toolbar()
        {
            return
                toolbarContainer
                    .AddChild(uiAnimationTab)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(behaviourTab)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(DesignUtils.flexibleSpace)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(nameTab)
                    .AddChild(DesignUtils.spaceBlock);
        }
    }
}
