// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Reflection;
using Doozy.Runtime.Reactor.Ticker;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.UIManager.Animators
{
    /// <summary>
    /// Specialized animator component used to animate a float value
    /// by listening to a UIToggle (controller) onToggleValueChangedCallback
    /// </summary>
    [AddComponentMenu("UI/Components/Animators/UIToggle Float Animator")]
    public class UIToggleFloatAnimator : BaseUIToggleAnimator
    {
        /// <summary> Float value target accessed via reflection </summary>
        public ReflectedFloat ValueTarget = new ReflectedFloat();

        /// <summary> Check if the value target is set up correctly </summary>
        public bool isValid => ValueTarget.IsValid();

        [SerializeField] private FloatAnimation OnAnimation;
        /// <summary> Toggle On Animation </summary>
        public FloatAnimation onAnimation => OnAnimation ?? (OnAnimation = new FloatAnimation(ValueTarget));

        [SerializeField] private FloatAnimation OffAnimation;
        /// <summary> Toggle Off Animation </summary>
        public FloatAnimation offAnimation => OffAnimation ?? (OffAnimation = new FloatAnimation(ValueTarget));

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
            ValueTarget ??= new ReflectedFloat();
            base.Reset();
            
            ResetAnimation(onAnimation);
            onAnimation.animation.fromCustomValue = 0f;
            onAnimation.animation.toCustomValue = 1f;
            
            ResetAnimation(offAnimation);
            offAnimation.animation.fromCustomValue = 1f;
            offAnimation.animation.toCustomValue = 0f;
        }
        #endif

        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnAnimation?.Recycle();
            OffAnimation?.Recycle();
        }

        /// <summary> Set the value target </summary>
        /// <param name="reflectedValue"> Reflected value target </param>
        private void SetTarget(object reflectedValue) =>
            SetTarget(reflectedValue as ReflectedFloat);

        /// <summary> Set the value target </summary>
        /// <param name="reflectedValue"> Reflected value target </param>
        private void SetTarget(ReflectedFloat reflectedValue)
        {
            onAnimation.SetTarget(reflectedValue);
            offAnimation.SetTarget(reflectedValue);
        }

        /// <summary> Refresh the set target and, if the animation is playing, update the calculated values </summary>
        public override void UpdateSettings()
        {
            SetTarget(ValueTarget);
            if (onAnimation.isPlaying) onAnimation.UpdateValues();
            if (offAnimation.isPlaying) offAnimation.UpdateValues();
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

            if (ValueTarget == null || !ValueTarget.IsValid())
                return;

            ValueTarget.SetValue(onAnimation.startValue);

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

        private static void ResetAnimation(FloatAnimation animation)
        {
            var reaction = animation.animation;

            reaction.Reset();
            reaction.enabled = true;
            reaction.fromReferenceValue = ReferenceValue.CustomValue;
            reaction.fromCustomValue = 0f;
            reaction.toReferenceValue = ReferenceValue.CustomValue;
            reaction.toCustomValue = 1f;
            reaction.settings.duration = 0.5f;
        }
    }
}
