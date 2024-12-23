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
using Doozy.Editor.Reactor.Ticker;
using Doozy.Editor.UIElements;
using Doozy.Editor.UIManager.Editors.Animators.Internal;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Animators;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Animators
{
    [CustomEditor(typeof(UIContainerColorAnimator), true)]
    [CanEditMultipleObjects]
    public class UIContainerColorAnimatorEditor : BaseUIContainerAnimatorEditor
    {
        public UIContainerColorAnimator castedTarget => (UIContainerColorAnimator)target;
        public List<UIContainerColorAnimator> castedTargets => targets.Cast<UIContainerColorAnimator>().ToList();

        private ColorAnimationTab showColorTab { get; set; }
        private ColorAnimationTab hideColorTab { get; set; }

        private ObjectField colorTargetObjectField { get; set; }
        private FluidField colorTargetFluidField { get; set; }
        private SerializedProperty propertyColorTarget { get; set; }
        private IVisualElementScheduledItem targetFinder { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            colorTargetFluidField?.Recycle();
            showColorTab?.Dispose();
            hideColorTab?.Dispose();
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
                    if (a.hasColorTarget)
                    {
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
                .SetComponentTypeText("Color Animator")
                .SetIcon(EditorSpriteSheets.Reactor.Icons.ColorAnimator)
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048739917/UIContainer+Color+Animator?atlOrigin=eyJpIjoiMmY2YTdiNjcwNGUzNDYzNWI0ZDBmNTAyYWI4YzVjZTEiLCJwIjoiYyJ9")
                .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.UIManager.Animators.UIContainerColorAnimator.html")
                .AddYouTubeButton();

            InitializeColorTarget();

            //search for color target
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                targetFinder = root.schedule.Execute(() =>
                {
                    if (castedTarget == null)
                        return;

                    if (castedTarget.colorTarget != null)
                    {
                        castedTarget.showAnimation.SetTarget(castedTarget.colorTarget);
                        castedTarget.hideAnimation.SetTarget(castedTarget.colorTarget);

                        targetFinder.Pause();
                        return;
                    }

                    castedTarget.FindTarget();

                }).Every(1000);

            //refresh tabs enabled indicator
            root.schedule.Execute(() =>
            {
                void UpdateIndicator(ColorAnimationTab tab, bool toggleOn, bool animateChange)
                {
                    if (tab == null) return;
                    if (tab.indicator.isOn != toggleOn)
                        tab.indicator.Toggle(toggleOn, animateChange);
                }

                //initial indicators state update (no animation)
                UpdateIndicator(showColorTab, castedTarget.showAnimation.animation.enabled, false);
                UpdateIndicator(hideColorTab, castedTarget.hideAnimation.animation.enabled, false);

                //subsequent indicators state update (animated)
                showColorTab.schedule.Execute(() => UpdateIndicator(showColorTab, castedTarget.showAnimation.animation.enabled, true)).Every(200);
                hideColorTab.schedule.Execute(() => UpdateIndicator(hideColorTab, castedTarget.hideAnimation.animation.enabled, true)).Every(200);
            });
        }

        private ColorAnimationTab GetColorTab(string labelText, UnityAction<bool> callback) =>
            ColorAnimationTab.Get()
                .SetLabelText(labelText)
                .SetElementSize(ElementSize.Small)
                .IndicatorSetEnabledColor(accentColor)
                .ButtonSetAccentColor(selectableAccentColor)
                .ButtonSetOnValueChanged(evt => callback?.Invoke(evt.newValue))
                .AddToToggleGroup(tabsGroup);

        protected override void InitializeShow()
        {
            base.InitializeShow();

            //remove the default show tab and replace it with the color tab
            showTab?.RemoveFromToggleGroup();
            showTab?.Recycle();

            showColorTab = GetColorTab("Show", value => showAnimatedContainer.Toggle(value))
                .SetIcon(EditorSpriteSheets.EditorUI.Icons.Show);

            //refresh colorTab reference color
            root.schedule.Execute(() =>
            {
                if (castedTarget == null) return;

                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                    if (castedTarget.hasColorTarget && !castedTarget.animatorInitialized)
                        HeartbeatCheck();

                showColorTab.RefreshTabReferenceColor(castedTarget.showAnimation);

            }).Every(200);
        }

        protected override void InitializeHide()
        {
            base.InitializeHide();

            //remove the default hide tab and replace it with the color tab
            hideTab?.RemoveFromToggleGroup();
            hideTab?.Recycle();

            hideColorTab = GetColorTab("Hide", value => hideAnimatedContainer.Toggle(value))
                .SetIcon(EditorSpriteSheets.EditorUI.Icons.Show);

            //refresh colorTab reference color
            root.schedule.Execute(() =>
            {
                if (castedTarget == null) return;

                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                    if (castedTarget.hasColorTarget && !castedTarget.animatorInitialized)
                        HeartbeatCheck();

                hideColorTab.RefreshTabReferenceColor(castedTarget.hideAnimation);

            }).Every(200);
        }

        private void InitializeColorTarget()
        {
            colorTargetObjectField =
                DesignUtils.NewObjectField(propertyColorTarget, typeof(ReactorColorTarget))
                    .SetTooltip("Animation color target")
                    .SetStyleFlexGrow(1);

            colorTargetFluidField =
                FluidField.Get()
                    .SetLabelText("Color Target")
                    .SetIcon(EditorSpriteSheets.Reactor.Icons.ColorTarget)
                    .SetStyleFlexShrink(0)
                    .AddFieldContent(colorTargetObjectField);
        }

        protected override VisualElement Toolbar()
        {
            return base.Toolbar()
                .RecycleAndClear()
                .AddChild(showColorTab)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(hideColorTab)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(DesignUtils.flexibleSpace);
        }

        protected override void Compose()
        {
            root
                .AddChild(reactionControls)
                .AddChild(componentHeader)
                .AddChild(Toolbar())
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(Content())
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(colorTargetFluidField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(GetController(propertyController))
                .AddChild(DesignUtils.endOfLineBlock);
        }
    }
}
