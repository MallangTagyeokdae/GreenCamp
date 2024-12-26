// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.Reactor.Ticker;
using UnityEngine;
using UnityEngine.Events;
namespace Doozy.Runtime.UIManager.Animators
{
    /// <summary>
    /// Specialized animator component used to animate a set of Sprites for a Reactor Sprite Target
    /// by listening to a UIToggle (controller) onToggleValueChangedCallback.
    /// </summary>
    [AddComponentMenu("UI/Components/Animators/UIToggle Sprite Animator")]
    public class UIToggleSpriteAnimator : BaseUIToggleAnimator
    {
        [SerializeField] private ReactorSpriteTarget SpriteTarget;
        /// <summary> Reference to a sprite target component </summary>
        public ReactorSpriteTarget spriteTarget => SpriteTarget;
        
        /// <summary> Check if a sprite target is referenced or not </summary>
        public bool hasSpriteTarget => SpriteTarget != null;
        
        [SerializeField] private SpriteAnimation OnAnimation;
        /// <summary> Toggle On Animation </summary>
        public SpriteAnimation onAnimation => OnAnimation ?? (OnAnimation = new SpriteAnimation(spriteTarget));

        [SerializeField] private SpriteAnimation OffAnimation;
        /// <summary> Toggle Off Animation </summary>
        public SpriteAnimation offAnimation => OffAnimation ?? (OffAnimation = new SpriteAnimation(spriteTarget));
        
        protected override bool onAnimationIsActive => onAnimation.isActive;
        protected override bool offAnimationIsActive => offAnimation.isActive;
        protected override UnityAction playOnAnimation => () => onAnimation.Play();
        protected override UnityAction playOffAnimation => () => offAnimation.Play();
        protected override UnityAction reverseOnAnimation => () => onAnimation.Reverse();
        protected override UnityAction reverseOffAnimation => () => offAnimation.Reverse();
        protected override UnityAction instantPlayOnAnimation => () => onAnimation.SetProgressAtOne();
        protected override UnityAction instantPlayOffAnimation => () => offAnimation.SetProgressAtOne();
        protected override UnityAction stopOnAnimation => () => onAnimation.Stop();
        protected override UnityAction stopOffAnimation => () => offAnimation.Stop();
        protected override UnityAction addResetToOnStateCallback => () => offAnimation.OnFinishCallback.AddListener(ResetToOnState);
        protected override UnityAction removeResetToOnStateCallback => () => offAnimation.OnFinishCallback.RemoveListener(ResetToOnState);
        protected override UnityAction addResetToOffStateCallback => () => onAnimation.OnFinishCallback.AddListener(ResetToOffState);
        protected override UnityAction removeResetToOffStateCallback => () => onAnimation.OnFinishCallback.RemoveListener(ResetToOffState);
        
        #if UNITY_EDITOR
        protected override void Reset()
        {
            FindTarget();
            ResetAnimation(onAnimation);
            ResetAnimation(offAnimation);
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
            UpdateSettings();
            FindTarget();
            base.Awake();
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnAnimation?.Recycle();
            OffAnimation?.Recycle();
        }
        
        /// <summary> Update the animations settings (if a spriteTarget is referenced) </summary>
        public override void UpdateSettings()
        {
            if(spriteTarget == null)
                return;

            onAnimation.SetTarget(spriteTarget);
            offAnimation.SetTarget(spriteTarget);
        }
        
        /// <summary> Stop all animations </summary>
        public override void StopAllReactions()
        {
            onAnimation.Stop();
            offAnimation.Stop();
        }
        
        /// <summary> Reset all the reactions to their initial values (if the animation is enabled) </summary>
        /// <param name="forced"> If true, forced will ignore if the animation is enabled or not </param>
        public override void ResetToStartValues(bool forced = false)
        {
            if (onAnimation.isActive) onAnimation.Stop();
            if (offAnimation.isActive) offAnimation.Stop();
            
            onAnimation.ResetToStartValues(forced);
            offAnimation.ResetToStartValues(forced);

            if (spriteTarget == null)
                return;
            
            spriteTarget.sprite = onAnimation.sprites[onAnimation.startFrame];

            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(rectTransform);
            UnityEditor.SceneView.RepaintAll();
            #endif
        }
        
        
        /// <summary> Set animation heartbeat </summary>
        public override List<Heartbeat> SetHeartbeat<T>()
        {
            var list = new List<Heartbeat>();
            for (int i = 0; i < 2; i++) list.Add(new T());
            
            onAnimation.animation.SetHeartbeat(list[0]);
            offAnimation.animation.SetHeartbeat(list[1]);
            
            return list;
        }

        private static void ResetAnimation(SpriteAnimation target)
        {
            target.animation.Reset();
            target.animation.enabled = false;
            target.animation.settings.duration = 1f;
        }
    }
}
