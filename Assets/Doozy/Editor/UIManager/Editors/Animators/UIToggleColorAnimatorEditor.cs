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

namespace Doozy.Editor.UIManager.Editors.Animators
{
    [CustomEditor(typeof(UIToggleColorAnimator))]
    [CanEditMultipleObjects]
    public class UIToggleColorAnimatorEditor : BaseUIToggleAnimatorEditor
    {
        public UIToggleColorAnimator castedTarget => (UIToggleColorAnimator) target;
        public List<UIToggleColorAnimator> castedTargets => targets.Cast<UIToggleColorAnimator>().ToList();

        private ColorAnimationTab onColorTab { get; set; }
        private ColorAnimationTab offColorTab { get; set; }
        
        private ObjectField colorTargetObjectField { get; set; }
        private FluidField colorTargetFluidField { get; set; }
        private SerializedProperty propertyColorTarget { get; set; }
        private IVisualElementScheduledItem targetFinder { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            colorTargetFluidField?.Recycle();
            onColorTab?.Dispose();
            offColorTab?.Dispose();
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
                .AddManualButton()
                .AddApiButton()
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
                        castedTarget.onAnimation.SetTarget(castedTarget.colorTarget);
                        castedTarget.offAnimation.SetTarget(castedTarget.colorTarget);

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
                UpdateIndicator(onColorTab, castedTarget.onAnimation.animation.enabled, false);
                UpdateIndicator(offColorTab, castedTarget.offAnimation.animation.enabled, false);

                //subsequent indicators state update (animated)
                onColorTab.schedule.Execute(() => UpdateIndicator(onColorTab, castedTarget.onAnimation.animation.enabled, true)).Every(200);
                offColorTab.schedule.Execute(() => UpdateIndicator(offColorTab, castedTarget.offAnimation.animation.enabled, true)).Every(200);
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

        protected override void InitializeOn()
        {
            base.InitializeOn();
            
            //remove the default on tab and replace it with the color tab
            onTab?.RemoveFromToggleGroup();
            onTab?.Recycle();
            
            onColorTab = GetColorTab("On", value => onAnimatedContainer.Toggle(value))
                .SetIcon(EditorSpriteSheets.EditorUI.Icons.ToggleON);
            
            //refresh colorTab reference color
            root.schedule.Execute(() =>
            {
                if (castedTarget == null) return;

                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                    if (castedTarget.hasColorTarget && !castedTarget.animatorInitialized)
                        HeartbeatCheck();

                onColorTab.RefreshTabReferenceColor(castedTarget.onAnimation);

            }).Every(200);
        }
        
        protected override void InitializeOff()
        {
            base.InitializeOff();
            
            //remove the default off tab and replace it with the color tab
            offTab?.RemoveFromToggleGroup();
            offTab?.Recycle();
            
            offColorTab = GetColorTab("Off", value => offAnimatedContainer.Toggle(value))
                .SetIcon(EditorSpriteSheets.EditorUI.Icons.ToggleOFF);
            
            //refresh colorTab reference color
            root.schedule.Execute(() =>
            {
                if (castedTarget == null) return;

                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                    if (castedTarget.hasColorTarget && !castedTarget.animatorInitialized)
                        HeartbeatCheck();

                offColorTab.RefreshTabReferenceColor(castedTarget.offAnimation);

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
                .AddChild(onColorTab)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(offColorTab)
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
