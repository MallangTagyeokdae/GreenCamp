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
using Doozy.Runtime.UIManager;
using Doozy.Runtime.UIManager.Animators;
using Doozy.Runtime.UIManager.Components;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Animators
{
    [CustomEditor(typeof(UISelectableColorAnimator), true)]
    [CanEditMultipleObjects]
    public class UISelectableColorAnimatorEditor : BaseUISelectableAnimatorEditor
    {
        public UISelectableColorAnimator castedTarget => (UISelectableColorAnimator)target;
        public IEnumerable<UISelectableColorAnimator> castedTargets => targets.Cast<UISelectableColorAnimator>();

        private ColorAnimationTab normalColorTab { get; set; }
        private ColorAnimationTab highlightedColorTab { get; set; }
        private ColorAnimationTab pressedColorTab { get; set; }
        private ColorAnimationTab selectedColorTab { get; set; }
        private ColorAnimationTab disabledColorTab { get; set; }

        private ObjectField colorTargetObjectField { get; set; }
        private FluidField colorTargetFluidField { get; set; }
        private SerializedProperty propertyColorTarget { get; set; }
        private IVisualElementScheduledItem targetFinder { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            normalColorTab?.Recycle();
            highlightedColorTab?.Recycle();
            pressedColorTab?.Recycle();
            selectedColorTab?.Recycle();
            disabledColorTab?.Recycle();

            colorTargetFluidField?.Recycle();
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
                .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1046577372/UISelectable+Color+Animator?atlOrigin=eyJpIjoiODkxNmI1NTFiMWI5NGNmMDg1MTczZDQxYzU1ZmE4OTgiLCJwIjoiYyJ9")
                .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.UIManager.Animators.UISelectableColorAnimator.html")
                .AddYouTubeButton();

            InitializeColorTarget();

            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                targetFinder = root.schedule.Execute(() =>
                {
                    if (castedTarget == null)
                        return;

                    if (castedTarget.colorTarget != null)
                    {
                        foreach (UISelectionState state in UISelectable.uiSelectionStates)
                            castedTarget.GetAnimation(state).SetTarget(castedTarget.colorTarget);

                        targetFinder.Pause();
                        return;
                    }

                    castedTarget.FindTarget();

                }).Every(1000);
        }

        private ColorAnimationTab GetColorTab(string labelText, UnityAction<bool> callback) =>
            ColorAnimationTab.Get()
                .SetLabelText(labelText)
                .SetElementSize(ElementSize.Small)
                .IndicatorSetEnabledColor(accentColor)
                .ButtonSetAccentColor(selectableAccentColor)
                .ButtonSetOnValueChanged(evt => callback?.Invoke(evt.newValue))
                .AddToToggleGroup(tabsGroup);

        protected override void InitializeNormal()
        {
            base.InitializeNormal();

            //remove the default normal tab and replace it with the color tab
            normalTab?.RemoveFromToggleGroup();
            normalTab?.Recycle();

            normalColorTab = GetColorTab("Normal", value => normalAnimatedContainer.Toggle(value));

            //refresh colorTab reference color
            root.schedule.Execute(() =>
            {
                if (castedTarget == null) return;

                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                    if (castedTarget.hasColorTarget && !castedTarget.animatorInitialized)
                        HeartbeatCheck();

                normalColorTab.RefreshTabReferenceColor(castedTarget.GetAnimation(UISelectionState.Normal));

            }).Every(200);
        }

        protected override void InitializeHighlighted()
        {
            base.InitializeHighlighted();

            //remove the default highlighted tab and replace it with the color tab
            highlightedTab?.RemoveFromToggleGroup();
            highlightedTab?.Recycle();

            highlightedColorTab = GetColorTab("Highlighted", value => highlightedAnimatedContainer.Toggle(value));

            //refresh colorTab reference color
            root.schedule.Execute(() =>
            {
                if (castedTarget == null) return;

                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                    if (castedTarget.hasColorTarget && !castedTarget.animatorInitialized)
                        HeartbeatCheck();

                highlightedColorTab.RefreshTabReferenceColor(castedTarget.GetAnimation(UISelectionState.Highlighted));

            }).Every(200);
        }

        protected override void InitializePressed()
        {
            base.InitializePressed();

            //remove the default pressed tab and replace it with the color tab
            pressedTab?.RemoveFromToggleGroup();
            pressedTab?.Recycle();

            pressedColorTab = GetColorTab("Pressed", value => pressedAnimatedContainer.Toggle(value));

            //refresh colorTab reference color
            root.schedule.Execute(() =>
            {
                if (castedTarget == null) return;

                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                    if (castedTarget.hasColorTarget && !castedTarget.animatorInitialized)
                        HeartbeatCheck();

                pressedColorTab.RefreshTabReferenceColor(castedTarget.GetAnimation(UISelectionState.Pressed));

            }).Every(200);
        }

        protected override void InitializeSelected()
        {
            base.InitializeSelected();

            //remove the default selected tab and replace it with the color tab
            selectedTab?.RemoveFromToggleGroup();
            selectedTab?.Recycle();

            selectedColorTab = GetColorTab("Selected", value => selectedAnimatedContainer.Toggle(value));

            //refresh colorTab reference color
            root.schedule.Execute(() =>
            {
                if (castedTarget == null) return;

                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                    if (castedTarget.hasColorTarget && !castedTarget.animatorInitialized)
                        HeartbeatCheck();

                selectedColorTab.RefreshTabReferenceColor(castedTarget.GetAnimation(UISelectionState.Selected));

            }).Every(200);
        }

        protected override void InitializeDisabled()
        {
            base.InitializeDisabled();

            //remove the default disabled tab and replace it with the color tab
            disabledTab?.RemoveFromToggleGroup();
            disabledTab?.Recycle();

            disabledColorTab = GetColorTab("Disabled", value => disabledAnimatedContainer.Toggle(value));

            //refresh colorTab reference color
            root.schedule.Execute(() =>
            {
                if (castedTarget == null) return;

                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                    if (castedTarget.hasColorTarget && !castedTarget.animatorInitialized)
                        HeartbeatCheck();

                disabledColorTab.RefreshTabReferenceColor(castedTarget.GetAnimation(UISelectionState.Disabled));

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
                .AddChild(normalColorTab)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(highlightedColorTab)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(pressedColorTab)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(selectedColorTab)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(disabledColorTab)
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
                .AddChild(GetController(propertyController, propertyToggleCommand))
                .AddChild(DesignUtils.endOfLineBlock);
        }
    }
}
