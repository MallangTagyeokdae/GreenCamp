// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Components;
using Doozy.Editor.Reactor.Editors.Animators.Internal;
using Doozy.Editor.Reactor.Ticker;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animations;
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
    [CustomEditor(typeof(ColorAnimator), true)]
    [CanEditMultipleObjects]
    public class ColorAnimatorEditor : BaseReactorAnimatorEditor
    {
        private ColorAnimator castedTarget => (ColorAnimator)target;
        private List<ColorAnimator> castedTargets => targets.Cast<ColorAnimator>().ToList();

        private ColorAnimationTab colorTab { get; set; }

        private ObjectField colorTargetObjectField { get; set; }
        private FluidField colorTargetFluidField { get; set; }
        private SerializedProperty propertyColorTarget { get; set; }
        private IVisualElementScheduledItem targetFinder { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            colorTargetFluidField?.Recycle();
            colorTab?.Dispose();
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
            propertyColorTarget = serializedObject.FindProperty("ColorTarget");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText("Color Animator")
                .SetIcon(EditorSpriteSheets.Reactor.Icons.ColorAnimator)
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1046675529/Color+Animator?atlOrigin=eyJpIjoiNmIwYjkxNTQwNjdhNGZkYTg1NTNlOTRiOGZlY2I5ZjIiLCJwIjoiYyJ9")
                .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.Reactor.Animators.ColorAnimator.html")
                .AddYouTubeButton();
        }

        protected override void InitializeAnimation()
        {
            base.InitializeAnimation();

            //remove the default animation tab and replace it with the color tab
            animationTab?.RemoveFromToggleGroup();
            animationTab?.Recycle();

            colorTab =
                ColorAnimationTab.Get()
                    .SetIcon(EditorSpriteSheets.Reactor.Icons.ColorAnimation)
                    .SetLabelText("Animation")
                    .ButtonSetAccentColor(selectableAccentColor)
                    .IndicatorSetEnabledColor(accentColor)
                    .ButtonSetOnValueChanged(evt => animationAnimatedContainer.Toggle(evt.newValue))
                    .AddToToggleGroup(tabsGroup);

            colorTargetObjectField =
                DesignUtils.NewObjectField(propertyColorTarget, typeof(ReactorColorTarget))
                    .SetStyleFlexGrow(1)
                    .SetTooltip("Animation color target");

            colorTargetFluidField =
                FluidField.Get()
                    .SetLabelText("Color Target")
                    .SetIcon(EditorSpriteSheets.Reactor.Icons.ColorTarget)
                    .AddFieldContent(colorTargetObjectField);

            //search for color target
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                targetFinder = root.schedule.Execute(() =>
                {
                    if (castedTarget == null) 
                        return;
                    
                    if (castedTarget.colorTarget != null)
                    {
                        castedTarget.animation.SetTarget(castedTarget.colorTarget);
                        targetFinder.Pause();
                        return;
                    }

                    castedTarget.FindTarget();

                }).Every(1000);

            //refresh colorTab reference color
            root.schedule.Execute(() =>
            {
                if (castedTarget == null) return;

                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                    if (castedTarget.hasTarget && !castedTarget.animatorInitialized)
                        HeartbeatCheck();

                colorTab.RefreshTabReferenceColor(castedTarget.animation);

            }).Every(200);

            //refresh colorTab enabled indicator
            SerializedProperty propertyEnabled = serializedObject.FindProperty("Animation.Animation.Enabled");
            root.schedule.Execute(() =>
            {
                void UpdateIndicator(ColorAnimationTab tab, bool toggleOn, bool animateChange)
                {
                    if (tab.indicator.isOn != toggleOn)
                        tab.indicator.Toggle(toggleOn, animateChange);
                }

                //initial indicators state update (no animation)
                UpdateIndicator(colorTab, propertyEnabled.boolValue, false);

                root.schedule.Execute(() =>
                {
                    //subsequent indicators state update (animated)
                    UpdateIndicator(colorTab, propertyEnabled.boolValue, true);

                }).Every(200);
            });
        }

        protected override VisualElement Toolbar()
        {
            return
                toolbarContainer
                    .AddChild(colorTab)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(behaviourTab)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(DesignUtils.flexibleSpace)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(nameTab)
                    .AddChild(DesignUtils.spaceBlock);
        }

        protected override void Compose()
        {
            root
                .AddChild(reactionControls)
                .AddChild(componentHeader)
                .AddChild(Toolbar())
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(Content())
                .AddChild(colorTargetFluidField)
                .AddChild(DesignUtils.endOfLineBlock);
        }
    }
}
