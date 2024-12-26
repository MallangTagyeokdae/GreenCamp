// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Runtime.Colors.Models;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.Reactor.Ticker;
using Doozy.Runtime.UIManager.Components;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Animators
{
    /// <summary>
    /// Specialized animator component used to animate the Color for a Reactor Color Target
    /// by listening to a UISelectable (controller) selection state changes.
    /// </summary>
    [AddComponentMenu("UI/Components/Animators/UISelectable Color Animator")]
    public class UISelectableColorAnimator : BaseUISelectableAnimator
    {
        [SerializeField] private ReactorColorTarget ColorTarget;
        /// <summary> Reference to a color target component </summary>
        public ReactorColorTarget colorTarget => ColorTarget;

        /// <summary> Check if a color target is referenced or not </summary>
        public bool hasColorTarget => ColorTarget != null;

        [SerializeField] private ColorAnimation NormalAnimation;
        /// <summary> Animation for the Normal selection state </summary>
        public ColorAnimation normalAnimation => NormalAnimation ?? (NormalAnimation = new ColorAnimation(colorTarget));

        [SerializeField] private ColorAnimation HighlightedAnimation;
        /// <summary> Animation for the Highlighted selection state </summary>
        public ColorAnimation highlightedAnimation => HighlightedAnimation ?? (HighlightedAnimation = new ColorAnimation(colorTarget));

        [SerializeField] private ColorAnimation PressedAnimation;
        /// <summary> Animation for the Pressed selection state </summary>
        public ColorAnimation pressedAnimation => PressedAnimation ?? (PressedAnimation = new ColorAnimation(colorTarget));

        [SerializeField] private ColorAnimation SelectedAnimation;
        /// <summary> Animation for the Selected selection state </summary>
        public ColorAnimation selectedAnimation => SelectedAnimation ?? (SelectedAnimation = new ColorAnimation(colorTarget));

        [SerializeField] private ColorAnimation DisabledAnimation;
        /// <summary> Animation for the Disabled selection state </summary>
        public ColorAnimation disabledAnimation => DisabledAnimation ?? (DisabledAnimation = new ColorAnimation(colorTarget));

        /// <summary> Get the animation triggered by the given selection state </summary>
        /// <param name="state"> Target selection state </param>
        public ColorAnimation GetAnimation(UISelectionState state) =>
            state switch
            {
                UISelectionState.Normal      => normalAnimation,
                UISelectionState.Highlighted => highlightedAnimation,
                UISelectionState.Pressed     => pressedAnimation,
                UISelectionState.Selected    => selectedAnimation,
                UISelectionState.Disabled    => disabledAnimation,
                _                            => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };

        #if UNITY_EDITOR
        protected override void Reset()
        {
            FindTarget();

            foreach (UISelectionState state in UISelectable.uiSelectionStates)
                ResetAnimation(state);

            base.Reset();
        }
        #endif

        public void FindTarget()
        {
            if (ColorTarget != null)
                return;

            ColorTarget = ReactorColorTarget.FindTarget(gameObject);
            UpdateSettings();
        }

        protected override void Awake()
        {
            FindTarget();
            UpdateSettings();
            base.Awake();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            foreach (UISelectionState state in UISelectable.uiSelectionStates)
                GetAnimation(state)?.Recycle();
        }

        /// <summary> Returns True if the givens selection state is enabled and the animation is not null. </summary>
        /// <param name="state"> Selection state </param>
        public override bool IsStateEnabled(UISelectionState state) =>
            GetAnimation(state).isEnabled;
        
        /// <summary> Update the animations settings (if a colorTarget is referenced) </summary>
        public override void UpdateSettings()
        {
            if (colorTarget == null)
                return;

            foreach (UISelectionState state in UISelectable.uiSelectionStates)
                GetAnimation(state)?.SetTarget(colorTarget);
        }

        /// <summary> Stop all animations </summary>
        public override void StopAllReactions()
        {
            foreach (UISelectionState state in UISelectable.uiSelectionStates)
                GetAnimation(state)?.Stop();
        }
        
        /// <summary> Reset all the reactions to their initial values (if the animation is enabled) </summary>
        /// <param name="forced"> If true, forced will ignore if the animation is enabled or not </param>
        public override void ResetToStartValues(bool forced = false)
        {
            if(normalAnimation.isActive) normalAnimation.Stop();
            if(highlightedAnimation.isActive) highlightedAnimation.Stop();
            if(pressedAnimation.isActive) pressedAnimation.Stop();
            if(selectedAnimation.isActive) selectedAnimation.Stop();
            if(disabledAnimation.isActive) disabledAnimation.Stop();
            
            normalAnimation.ResetToStartValues();
            highlightedAnimation.ResetToStartValues();
            pressedAnimation.ResetToStartValues();
            selectedAnimation.ResetToStartValues();
            disabledAnimation.ResetToStartValues();
            
            if (colorTarget == null)
                return;

            colorTarget.color = normalAnimation.startColor;
            
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(rectTransform);
            UnityEditor.SceneView.RepaintAll();
            #endif
        }
        
        /// <summary> Set animation heartbeat </summary>
        public override List<Heartbeat> SetHeartbeat<T>()
        {
            var list = new List<Heartbeat>();
            for (int i = 0; i < 5; i++) list.Add(new T());

            normalAnimation.animation.SetHeartbeat(list[0]);
            highlightedAnimation.animation.SetHeartbeat(list[1]);
            pressedAnimation.animation.SetHeartbeat(list[2]);
            selectedAnimation.animation.SetHeartbeat(list[3]);
            disabledAnimation.animation.SetHeartbeat(list[4]);
            
            return list;
        }

        /// <summary> Play the animation for the given selection state </summary>
        /// <param name="state"> Selection state </param>
        public override void Play(UISelectionState state) =>
            GetAnimation(state)?.Play();

        /// <summary> Reset the animation for the given selection state to its initial values </summary>
        /// <param name="state"> Selection state </param>
        private void ResetAnimation(UISelectionState state)
        {
            ColorAnimation a = GetAnimation(state);

            a.animation.Reset();
            a.animation.enabled = true;
            a.animation.fromReferenceValue = ReferenceValue.CurrentValue;
            a.animation.settings.duration = UISelectable.k_DefaultAnimationDuration;

            switch (state)
            {
                case UISelectionState.Normal:
                    a.animation.settings.duration = UISelectable.k_DefaultAnimationDuration * 0.5f;
                    break;
                case UISelectionState.Highlighted:
                    a.animation.toLightnessOffset = 0.1f;
                    break;
                case UISelectionState.Pressed:
                    a.animation.toLightnessOffset = -0.1f;
                    a.animation.settings.duration = UISelectable.k_DefaultAnimationDuration * 0.25f;
                    break;
                case UISelectionState.Selected:
                    a.animation.toHueOffset = -10f / HSL.H.F;
                    a.animation.toLightnessOffset = 0.2f;
                    break;
                case UISelectionState.Disabled:
                    a.animation.toSaturationOffset = -0.5f;
                    a.animation.toAlphaOffset = -0.5f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        /// <summary> Set the Start Color for all animations </summary>
        /// <param name="color"> New start color </param>
        public void SetStartColor(Color color)
        {
            foreach (UISelectionState state in UISelectable.uiSelectionStates)
            {
                ColorAnimation colorAnimation = GetAnimation(state);
                if (colorAnimation == null) continue;
                colorAnimation.customStartValue = color;
                // colorAnimation.startColor = color;
            }

            if (controller == null) return;
            controller.RefreshState();
        }

        /// <summary> Set the Start Color for the target selection state </summary>
        /// <param name="color"> New start color </param>
        /// <param name="selectionState"> Target selection state </param>
        public void SetStartColor(Color color, UISelectionState selectionState)
        {
            ColorAnimation colorAnimation = GetAnimation(selectionState);
            if (colorAnimation == null) return;
            colorAnimation.customStartValue = color;
            // colorAnimation.startColor = color;
            if (controller == null) return;
            if (controller.currentUISelectionState != selectionState) return;
            controller.RefreshState();
        }

    }
}
