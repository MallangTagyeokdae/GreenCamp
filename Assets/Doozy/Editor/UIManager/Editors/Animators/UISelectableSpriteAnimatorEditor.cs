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
using Doozy.Runtime.UIManager;
using Doozy.Runtime.UIManager.Animators;
using Doozy.Runtime.UIManager.Components;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Animators
{
    [CustomEditor(typeof(UISelectableSpriteAnimator), true)]
    [CanEditMultipleObjects]
    public class UISelectableSpriteAnimatorEditor : BaseUISelectableAnimatorEditor
    {
        public UISelectableSpriteAnimator castedTarget => (UISelectableSpriteAnimator)target;
        public List<UISelectableSpriteAnimator> castedTargets => targets.Cast<UISelectableSpriteAnimator>().ToList();

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

        protected override void PlayNormal()
        {
            foreach (var a in castedTargets)
                a.GetAnimation(UISelectionState.Normal).Play();
        }

        protected override void PlayHighlighted()
        {
            foreach (var a in castedTargets)
                a.GetAnimation(UISelectionState.Highlighted).Play();
        }

        protected override void PlayPressed()
        {
            foreach (var a in castedTargets)
                a.GetAnimation(UISelectionState.Pressed).Play();
        }

        protected override void PlaySelected()
        {
            foreach (var a in castedTargets)
                a.GetAnimation(UISelectionState.Selected).Play();
        }

        protected override void PlayDisabled()
        {
            foreach (var a in castedTargets)
                a.GetAnimation(UISelectionState.Disabled).Play();
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
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048281113/UISelectable+Sprite+Animator?atlOrigin=eyJpIjoiNjQxNmZkOTY3NThmNDZiZmE5ODkwYzVkZDRhODUxZmQiLCJwIjoiYyJ9")
                .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.UIManager.Animators.UISelectableSpriteAnimator.html")
                .AddYouTubeButton();

            InitializeSpriteTarget();

            normalAnimatedContainer.AddOnShowCallback(() => normalAnimatedContainer.Bind(serializedObject));
            highlightedAnimatedContainer.AddOnShowCallback(() => highlightedAnimatedContainer.Bind(serializedObject));
            pressedAnimatedContainer.AddOnShowCallback(() => pressedAnimatedContainer.Bind(serializedObject));
            selectedAnimatedContainer.AddOnShowCallback(() => selectedAnimatedContainer.Bind(serializedObject));
            disabledAnimatedContainer.AddOnShowCallback(() => disabledAnimatedContainer.Bind(serializedObject));

            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                targetFinder = root.schedule.Execute(() =>
                {
                    if (castedTarget == null)
                        return;

                    if (castedTarget.spriteTarget != null)
                    {
                        foreach (UISelectionState state in UISelectable.uiSelectionStates)
                            castedTarget.GetAnimation(state).SetTarget(castedTarget.spriteTarget);

                        targetFinder.Pause();
                        return;
                    }

                    castedTarget.FindTarget();

                }).Every(1000);
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
                .AddChild(GetController(propertyController, propertyToggleCommand))
                .AddChild(DesignUtils.endOfLineBlock);
        }
    }
}
