// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Reflection;
using Doozy.Runtime.Reactor.Ticker;
using Doozy.Runtime.UIManager.Components;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.UIManager.Animators
{
    /// <summary>
    /// Specialized animator component used to animate a Vector2 value
    /// by listening to a target UISelectable (controller) selection state changes.
    /// </summary>
    [AddComponentMenu("UI/Components/Animators/UISelectable Vector2 Animator")]
    public class UISelectableVector2Animator : BaseUISelectableAnimator
    {
        /// <summary> Vector2 value target accessed via reflection </summary>
        public ReflectedVector2 ValueTarget = new ReflectedVector2();

        /// <summary> Check if the value target is set up correctly </summary>
        public bool isValid => ValueTarget.IsValid();

        [SerializeField] private Vector2Animation NormalAnimation;
        /// <summary> Animation for the Normal selection state </summary>
        public Vector2Animation normalAnimation => NormalAnimation ?? (NormalAnimation = new Vector2Animation(ValueTarget));

        [SerializeField] private Vector2Animation HighlightedAnimation;
        /// <summary> Animation for the Highlighted selection state </summary>
        public Vector2Animation highlightedAnimation => HighlightedAnimation ?? (HighlightedAnimation = new Vector2Animation(ValueTarget));

        [SerializeField] private Vector2Animation PressedAnimation;
        /// <summary> Animation for the Pressed selection state </summary>
        public Vector2Animation pressedAnimation => PressedAnimation ?? (PressedAnimation = new Vector2Animation(ValueTarget));

        [SerializeField] private Vector2Animation SelectedAnimation;
        /// <summary> Animation for the Selected selection state </summary>
        public Vector2Animation selectedAnimation => SelectedAnimation ?? (SelectedAnimation = new Vector2Animation(ValueTarget));

        [SerializeField] private Vector2Animation DisabledAnimation;
        /// <summary> Animation for the Disabled selection state </summary>
        public Vector2Animation disabledAnimation => DisabledAnimation ?? (DisabledAnimation = new Vector2Animation(ValueTarget));

        /// <summary> Returns TRUE if any animation is active </summary>
        public bool anyAnimationIsActive =>
            normalAnimation.isActive ||
            highlightedAnimation.isActive ||
            pressedAnimation.isActive ||
            selectedAnimation.isActive ||
            disabledAnimation.isActive;

        /// <summary> Get the animation triggered by the given selection state </summary>
        /// <param name="state"> Target selection state </param>
        public Vector2Animation GetAnimation(UISelectionState state) =>
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
            ValueTarget ??= new ReflectedVector2();
            base.Reset();
            foreach (UISelectionState state in UISelectable.uiSelectionStates)
                ResetAnimation(state);
        }
        #endif
        
        protected override void Awake()
        {
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
            foreach (UISelectionState state in UISelectable.uiSelectionStates)
            {
                var a = GetAnimation(state);
                if(a == null) continue;
                a.SetTarget(ValueTarget);
                if(a.isPlaying) a.UpdateValues();
            }
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
            
            if (ValueTarget == null || !ValueTarget.IsValid())
                return;

            ValueTarget.SetValue(normalAnimation.startValue);
            
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
        
        /// <summary> Reset the animation for the given selection state </summary>
        /// <param name="state"> Selection state </param>
        private void ResetAnimation(UISelectionState state)
        {
            var a = GetAnimation(state);

            a.animation.enabled = true;
            a.animation.fromReferenceValue = ReferenceValue.CurrentValue;
            a.animation.toReferenceValue = ReferenceValue.StartValue;
            a.animation.settings.duration = 0.2f;
        }
    }
}
