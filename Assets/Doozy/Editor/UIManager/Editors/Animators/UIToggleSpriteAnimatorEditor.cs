// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Ticker;
using Doozy.Editor.UIManager.Editors.Animators.Internal;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Animators;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Editors.Animators
{
    [CustomEditor(typeof(UIToggleSpriteAnimator), true)]
    [CanEditMultipleObjects]
    public class UIToggleSpriteAnimatorEditor : BaseUIToggleAnimatorEditor
    {
        public UIToggleSpriteAnimator castedTarget => (UIToggleSpriteAnimator) target;
        public List<UIToggleSpriteAnimator> castedTargets => targets.Cast<UIToggleSpriteAnimator>().ToList();
        
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
                    if (a.hasSpriteTarget)
                    {
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
                .SetComponentTypeText("Sprite Animator")
                .SetIcon(EditorSpriteSheets.Reactor.Icons.SpriteAnimator)
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048903720/UIContainer+Sprite+Animator?atlOrigin=eyJpIjoiOTE2YzJjMjZjMjQwNDhjNWIzY2I3NTEzYzUwNjRkOGUiLCJwIjoiYyJ9")
                .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.UIManager.Animators.UIContainerSpriteAnimator.html")
                .AddYouTubeButton();

            InitializeSpriteTarget();

            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                targetFinder = root.schedule.Execute(() =>
                {
                    if (castedTarget == null)
                        return;

                    if (castedTarget.spriteTarget != null)
                    {
                        castedTarget.onAnimation.SetTarget(castedTarget.spriteTarget);
                        castedTarget.offAnimation.SetTarget(castedTarget.spriteTarget);

                        targetFinder.Pause();
                        return;
                    }

                    castedTarget.FindTarget();

                }).Every(1000);

            //refresh tabs enabled indicator
            root.schedule.Execute(() =>
            {
                void UpdateIndicator(FluidTab tab, bool toggleOn, bool animateChange)
                {
                    if (tab == null) return;
                    if (tab.indicator.isOn != toggleOn)
                        tab.indicator.Toggle(toggleOn, animateChange);
                }

                //initial indicators state update (no animation)
                UpdateIndicator(onTab, castedTarget.onAnimation.animation.enabled, false);
                UpdateIndicator(offTab, castedTarget.offAnimation.animation.enabled, false);

                //subsequent indicators state update (animated)
                onTab.schedule.Execute(() => UpdateIndicator(onTab, castedTarget.onAnimation.animation.enabled, true)).Every(200);
                offTab.schedule.Execute(() => UpdateIndicator(offTab, castedTarget.offAnimation.animation.enabled, true)).Every(200);
            });
        }

        private void InitializeSpriteTarget()
        {
            spriteTargetObjectField =
                DesignUtils.NewObjectField(propertySpriteTarget, typeof(ReactorSpriteTarget))
                    .SetStyleFlexGrow(1)
                    .SetTooltip("Animation sprite target");
            spriteTargetFluidField =
                FluidField.Get()
                    .SetLabelText("Sprite Target")
                    .SetIcon(EditorSpriteSheets.Reactor.Icons.SpriteTarget)
                    .SetStyleFlexShrink(0)
                    .AddFieldContent(spriteTargetObjectField);
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
                .AddChild(spriteTargetFluidField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(GetController(propertyController))
                .AddChild(DesignUtils.endOfLineBlock);
        }
    }
}
