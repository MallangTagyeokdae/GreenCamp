// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Editors.Animators.Internal;
using Doozy.Editor.Reactor.Ticker;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animators;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Reactor.Editors.Animators
{
    [CustomEditor(typeof(SpriteAnimator), true)]
    [CanEditMultipleObjects]
    public class SpriteAnimatorEditor : BaseReactorAnimatorEditor
    {
        private SpriteAnimator castedTarget => (SpriteAnimator)target;
        private List<SpriteAnimator> castedTargets => targets.Cast<SpriteAnimator>().ToList();

        private ObjectField spriteTargetObjectField { get; set; }
        private FluidField spriteTargetFluidField { get; set; }
        private SerializedProperty propertySpriteTarget { get; set; }
        private IVisualElementScheduledItem targetFinder { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            spriteTargetFluidField?.Recycle();
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
                if (!a.hasTarget) continue;
                resetToStartValue = true;
                a.InitializeAnimator();
                foreach (EditorHeartbeat eh in a.SetHeartbeat<EditorHeartbeat>().Cast<EditorHeartbeat>())
                    eh.StartSceneViewRefresh(a);
            }
        }

        protected override void FindProperties()
        {
            base.FindProperties();
            propertySpriteTarget = serializedObject.FindProperty("SpriteTarget");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText("Sprite Animator")
                .SetIcon(EditorSpriteSheets.Reactor.Icons.SpriteAnimator)
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/external/1046675529/ZTk4MjcyZjljOWM5NDVlYjkyNDI1NzE0OTFjZWYyYzk?atlOrigin=eyJpIjoiM2RlMWZmMjM0NjgyNGZlZjhiOTBmNGM5MTNjODIwNzYiLCJwIjoiYyJ9")
                .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.Reactor.Animators.SpriteAnimator.html")
                .AddYouTubeButton();
        }

        protected override void InitializeAnimation()
        {
            base.InitializeAnimation();

            animationTab.SetIcon(EditorSpriteSheets.Reactor.Icons.SpriteAnimation);

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

            spriteTargetObjectField =
                DesignUtils.NewObjectField(propertySpriteTarget, typeof(ReactorSpriteTarget))
                    .SetStyleFlexGrow(1)
                    .SetTooltip("Animation sprite target");

            spriteTargetFluidField =
                FluidField.Get()
                    .SetLabelText("Sprite Target")
                    .SetIcon(EditorSpriteSheets.Reactor.Icons.SpriteTarget)
                    .AddFieldContent(spriteTargetObjectField);

            //search for sprite target
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                targetFinder = root.schedule.Execute(() =>
                {
                    if (castedTarget == null) return;
                    if (castedTarget.spriteTarget != null)
                    {
                        castedTarget.animation.SetTarget(castedTarget.spriteTarget);
                        targetFinder.Pause();
                        return;
                    }

                    castedTarget.FindTarget();

                }).Every(1000);
            }
        }

        protected override void Compose()
        {
            root
                .AddChild(reactionControls)
                .AddChild(componentHeader)
                .AddChild(Toolbar())
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(Content())
                .AddChild(spriteTargetFluidField)
                .AddChild(DesignUtils.endOfLineBlock);
        }
    }
}
