// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.Reactor.Ticker;
using Doozy.Runtime.UIManager.Components;
using UnityEngine;
// ReSharper disable UnusedMember.Global

namespace Doozy.Runtime.UIManager.Animators
{
    /// <summary>
    /// Specialized animator component used to animate a set of Sprites for a Reactor Sprite Target
    /// by listening to a UISelectable (controller) selection state changes.
    /// </summary>
    [AddComponentMenu("UI/Components/Animators/UISelectable Sprite Animator")]
    public class UISelectableSpriteAnimator : BaseUISelectableAnimator
    {
        [SerializeField] private ReactorSpriteTarget SpriteTarget;
        /// <summary> Reference to a sprite target component </summary>
        public ReactorSpriteTarget spriteTarget => SpriteTarget;

        /// <summary> Check if a sprite target is referenced or not </summary>
        public bool hasSpriteTarget => SpriteTarget != null;

        [SerializeField] private SpriteAnimation NormalAnimation;
        /// <summary> Animation for the Normal selection state </summary>
        public SpriteAnimation normalAnimation => NormalAnimation ?? (NormalAnimation = new SpriteAnimation(spriteTarget));

        [SerializeField] private SpriteAnimation HighlightedAnimation;
        /// <summary> Animation for the Highlighted selection state </summary>
        public SpriteAnimation highlightedAnimation => HighlightedAnimation ?? (HighlightedAnimation = new SpriteAnimation(spriteTarget));

        [SerializeField] private SpriteAnimation PressedAnimation;
        /// <summary> Animation for the Pressed selection state </summary>
        public SpriteAnimation pressedAnimation => PressedAnimation ?? (PressedAnimation = new SpriteAnimation(spriteTarget));

        [SerializeField] private SpriteAnimation SelectedAnimation;
        /// <summary> Animation for the Selected selection state </summary>
        public SpriteAnimation selectedAnimation => SelectedAnimation ?? (SelectedAnimation = new SpriteAnimation(spriteTarget));

        [SerializeField] private SpriteAnimation DisabledAnimation;
        /// <summary> Animation for the Disabled selection state </summary>
        public SpriteAnimation disabledAnimation => DisabledAnimation ?? (DisabledAnimation = new SpriteAnimation(spriteTarget));

        /// <summary> Get the animation triggered by the given selection state </summary>
        /// <param name="state"> Target selection state </param>
        public SpriteAnimation GetAnimation(UISelectionState state) =>
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
            if (SpriteTarget != null)
                return;

            SpriteTarget = ReactorSpriteTarget.FindTarget(gameObject);
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
            if (spriteTarget == null)
                return;

            foreach (UISelectionState state in UISelectable.uiSelectionStates)
                GetAnimation(state)?.SetTarget(spriteTarget);
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
            
            if (spriteTarget == null)
                return;

            spriteTarget.sprite = normalAnimation.sprites[normalAnimation.startFrame];
            
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
            SpriteAnimation a = GetAnimation(state);

            a.animation.Reset();
            a.animation.enabled = false;
            a.animation.settings.duration = 0.5f;
        }
    }
}
