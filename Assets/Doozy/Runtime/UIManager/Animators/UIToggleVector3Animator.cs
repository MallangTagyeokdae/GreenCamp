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
    /// Specialized animator component used to animate a Vector3 value
    /// by listening to a UIToggle (controller) onToggleValueChangedCallback
    /// </summary>
    [AddComponentMenu("UI/Components/Animators/UIToggle Vector3 Animator")]
    public class UIToggleVector3Animator : BaseUIToggleAnimator
    {
        /// <summary> Vector3 value target accessed via reflection </summary>
        public ReflectedVector3 ValueTarget = new ReflectedVector3();

        /// <summary> Check if the value target is set up correctly </summary>
        public bool isValid => ValueTarget.IsValid();

        [SerializeField] private Vector3Animation OnAnimation;
        /// <summary> Toggle On Animation </summary>
        public Vector3Animation onAnimation => OnAnimation ?? (OnAnimation = new Vector3Animation(ValueTarget));

        [SerializeField] private Vector3Animation OffAnimation;
        /// <summary> Toggle Off Animation </summary>
        public Vector3Animation offAnimation => OffAnimation ?? (OffAnimation = new Vector3Animation(ValueTarget));

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
            ValueTarget ??= new ReflectedVector3();
            base.Reset();
            
            ResetAnimation(onAnimation);
            onAnimation.animation.fromCustomValue = Vector3.zero;
            onAnimation.animation.toCustomValue = Vector3.one;
            
            ResetAnimation(offAnimation);
            offAnimation.animation.fromCustomValue = Vector3.one;
            offAnimation.animation.toCustomValue = Vector3.zero;
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
            SetTarget(reflectedValue as ReflectedVector3);

        /// <summary> Set the value target </summary>
        /// <param name="reflectedValue"> Reflected value target </param>
        private void SetTarget(ReflectedVector3 reflectedValue)
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

        private static void ResetAnimation(Vector3Animation animation)
        {
            var reaction = animation.animation;

            reaction.Reset();
            reaction.enabled = true;
            reaction.fromReferenceValue = ReferenceValue.CustomValue;
            reaction.fromCustomValue = Vector3.zero;
            reaction.toReferenceValue = ReferenceValue.CustomValue;
            reaction.toCustomValue = Vector3.one;
            reaction.settings.duration = 0.5f;
        }
    }
}
