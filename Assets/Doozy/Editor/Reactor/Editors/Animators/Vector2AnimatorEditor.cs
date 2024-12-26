// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.Reactor.Components;
using Doozy.Editor.Reactor.Editors.Animators.Internal;
using Doozy.Editor.Reactor.Ticker;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animators;
using UnityEditor;
using UnityEngine;

namespace Doozy.Editor.Reactor.Editors.Animators
{
    [CustomEditor(typeof(Vector2Animator))]
    [CanEditMultipleObjects]
    public class Vector2AnimatorEditor : ReflectedValueAnimatorEditor
    {
        private Vector2Animator castedTarget => (Vector2Animator)target;
        private List<Vector2Animator> castedTargets => targets.Cast<Vector2Animator>().ToList();

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
                if (!a.ValueTarget.IsValid()) continue;
                resetToStartValue = true;
                a.InitializeAnimator();
                foreach (EditorHeartbeat eh in a.SetHeartbeat<EditorHeartbeat>().Cast<EditorHeartbeat>())
                    eh.StartSceneViewRefresh(a);
            }
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText("Vector2 Animator")
                .SetIcon(EditorSpriteSheets.Reactor.Icons.Vector2Animator)
                .AddManualButton()
                .AddApiButton()
                .AddYouTubeButton();
        }

        protected override void InitializeAnimation()
        {
            base.InitializeAnimation();

            animationTab.SetIcon(EditorSpriteSheets.Reactor.Icons.Vector2Animation);

            //refresh animationTab enabled indicator
            SerializedProperty propertyEnabled = serializedObject.FindProperty("Animation.Animation.Enabled");
            root.schedule.Execute(() =>
            {
                void UpdateIndicator(FluidTab tab, bool toggleOn, bool animateChange)
                {
                    if (tab.indicator.isOn != toggleOn)
                        tab.indicator.Toggle(toggleOn, animateChange);
                }

                //initial indicators state update (no animation)
                UpdateIndicator(animationTab, propertyEnabled.boolValue, false);

                root.schedule.Execute(() =>
                {
                    //subsequent indicators state update (animated)
                    UpdateIndicator(animationTab, propertyEnabled.boolValue, true);

                }).Every(200);
            });
        }
    }
}
