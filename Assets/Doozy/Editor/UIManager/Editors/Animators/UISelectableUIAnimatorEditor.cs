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
using Doozy.Runtime.UIManager;
using Doozy.Runtime.UIManager.Animators;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Animators
{
    [CustomEditor(typeof(UISelectableUIAnimator), true)]
    [CanEditMultipleObjects]
    public class UISelectableUIAnimatorEditor : BaseUISelectableAnimatorEditor
    {
        public UISelectableUIAnimator castedTarget => (UISelectableUIAnimator)target;
        public IEnumerable<UISelectableUIAnimator> castedTargets => targets.Cast<UISelectableUIAnimator>();

        private UIAnimationTab normalUIAnimationTab { get; set; }
        private UIAnimationTab highlightedUIAnimationTab { get; set; }
        private UIAnimationTab pressedUIAnimationTab { get; set; }
        private UIAnimationTab selectedUIAnimationTab { get; set; }
        private UIAnimationTab disabledUIAnimationTab { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            normalUIAnimationTab?.Dispose();
            highlightedUIAnimationTab?.Dispose();
            pressedUIAnimationTab?.Dispose();
            selectedUIAnimationTab?.Dispose();
            disabledUIAnimationTab?.Dispose();
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
                    .SetComponentTypeText("UI Animator")
                    .SetIcon(EditorSpriteSheets.Reactor.Icons.UIAnimator)
                    .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048543277/UISelectable+UI+Animator?atlOrigin=eyJpIjoiMDNjZTc2YWZhZjlhNDU0N2E1YzdmOThjMzQwODJmMWIiLCJwIjoiYyJ9")
                    .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.UIManager.Animators.UISelectableUIAnimator.html")
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
                UpdateIndicator(normalUIAnimationTab, castedTarget.normalAnimation, false);
                UpdateIndicator(highlightedUIAnimationTab, castedTarget.highlightedAnimation, false);
                UpdateIndicator(pressedUIAnimationTab, castedTarget.pressedAnimation, false);
                UpdateIndicator(selectedUIAnimationTab, castedTarget.selectedAnimation, false);
                UpdateIndicator(disabledUIAnimationTab, castedTarget.disabledAnimation, false);

                //subsequent indicators state update (animated)
                root.schedule.Execute(() =>
                {
                    UpdateIndicator(normalUIAnimationTab, castedTarget.normalAnimation, true);
                    UpdateIndicator(highlightedUIAnimationTab, castedTarget.highlightedAnimation, true);
                    UpdateIndicator(pressedUIAnimationTab, castedTarget.pressedAnimation, true);
                    UpdateIndicator(selectedUIAnimationTab, castedTarget.selectedAnimation, true);
                    UpdateIndicator(disabledUIAnimationTab, castedTarget.disabledAnimation, true);

                }).Every(200);
            });
        }

        protected override void InitializeNormal()
        {
            base.InitializeNormal();
            
            //remove the default tab and replace it with the ui animation tab
            normalTab?.RemoveFromToggleGroup();
            normalTab?.Recycle();

            normalUIAnimationTab =
                UIAnimationTab.Get()
                    .SetLabelText("Normal")
                    .ButtonSetOnValueChanged(evt => normalAnimatedContainer.Toggle(evt.newValue, evt.animateChange))
                    .AddToToggleGroup(tabsGroup);
        }
        
        protected override void InitializeHighlighted()
        {
            base.InitializeHighlighted();
            
            //remove the default tab and replace it with the ui animation tab
            highlightedTab?.RemoveFromToggleGroup();
            highlightedTab?.Recycle();

            highlightedUIAnimationTab =
                UIAnimationTab.Get()
                    .SetLabelText("Highlight")
                    .ButtonSetOnValueChanged(evt => highlightedAnimatedContainer.Toggle(evt.newValue, evt.animateChange))
                    .AddToToggleGroup(tabsGroup);
        }
        
        protected override void InitializePressed()
        {
            base.InitializePressed();
            
            //remove the default tab and replace it with the ui animation tab
            pressedTab?.RemoveFromToggleGroup();
            pressedTab?.Recycle();

            pressedUIAnimationTab =
                UIAnimationTab.Get()
                    .SetLabelText("Press")
                    .ButtonSetOnValueChanged(evt => pressedAnimatedContainer.Toggle(evt.newValue, evt.animateChange))
                    .AddToToggleGroup(tabsGroup);
        }
        
        protected override void InitializeSelected()
        {
            base.InitializeSelected();
            
            //remove the default tab and replace it with the ui animation tab
            selectedTab?.RemoveFromToggleGroup();
            selectedTab?.Recycle();

            selectedUIAnimationTab =
                UIAnimationTab.Get()
                    .SetLabelText("Select")
                    .ButtonSetOnValueChanged(evt => selectedAnimatedContainer.Toggle(evt.newValue, evt.animateChange))
                    .AddToToggleGroup(tabsGroup);
        }
        
        protected override void InitializeDisabled()
        {
            base.InitializeDisabled();
            
            //remove the default tab and replace it with the ui animation tab
            disabledTab?.RemoveFromToggleGroup();
            disabledTab?.Recycle();

            disabledUIAnimationTab =
                UIAnimationTab.Get()
                    .SetLabelText("Disable")
                    .ButtonSetOnValueChanged(evt => disabledAnimatedContainer.Toggle(evt.newValue, evt.animateChange))
                    .AddToToggleGroup(tabsGroup);
        }
        
        protected override VisualElement Toolbar()
        {
            return base.Toolbar()
                .RecycleAndClear()
                .AddChild(normalUIAnimationTab)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(highlightedUIAnimationTab)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(pressedUIAnimationTab)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(selectedUIAnimationTab)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(disabledUIAnimationTab)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(DesignUtils.flexibleSpace);
        }
    }
}
