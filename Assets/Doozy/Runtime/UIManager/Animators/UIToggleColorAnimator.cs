// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.Reactor.Ticker;
using Doozy.Runtime.UIManager.Components;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.UIManager.Animators
{
    /// <summary>
    /// Specialized animator component used to animate the Color for a Rector Color Target
    /// by listening to a UIToggle (controller) onToggleValueChangedCallback
    /// </summary>
    [AddComponentMenu("UI/Components/Animators/UIToggle Color Animator")]
    public class UIToggleColorAnimator : BaseUIToggleAnimator
    {
        [SerializeField] private ReactorColorTarget ColorTarget;
        /// <summary> Reference to a color target component </summary>
        public ReactorColorTarget colorTarget => ColorTarget;

        /// <summary> Check if a color target is referenced or not </summary>
        public bool hasColorTarget => ColorTarget != null;

        [SerializeField] private ColorAnimation OnAnimation;
        /// <summary> Toggle On Animation </summary>
        public ColorAnimation onAnimation => OnAnimation ?? (OnAnimation = new ColorAnimation(colorTarget));

        [SerializeField] private ColorAnimation OffAnimation;
        /// <summary> Toggle Off Animation </summary>
        public ColorAnimation offAnimation => OffAnimation ?? (OffAnimation = new ColorAnimation(colorTarget));

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
        protected override UnityAction addResetToOnStateCallback => () => offAnimation.OnStopCallback.AddListener(ResetToOnState);
        protected override UnityAction removeResetToOnStateCallback => () => offAnimation.OnStopCallback.RemoveListener(ResetToOnState);
        protected override UnityAction addResetToOffStateCallback => () => onAnimation.OnStopCallback.AddListener(ResetToOffState);
        protected override UnityAction removeResetToOffStateCallback => () => onAnimation.OnStopCallback.RemoveListener(ResetToOffState);

        #if UNITY_EDITOR
        protected override void Reset()
        {
            FindTarget();

            ResetAnimation(onAnimation);
            ResetAnimation(offAnimation);

            base.Reset();
        }
        #endif

        /// <summary> Find ColorTarget </summary>
        public void FindTarget()
        {
            if (ColorTarget != null)
                return;

            ColorTarget = ReactorColorTarget.FindTarget(gameObject);
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

        /// <summary> Update the animations settings (if a colorTarget is referenced) </summary>
        public override void UpdateSettings()
        {
            if (colorTarget == null)
                return;

            onAnimation.SetTarget(colorTarget);
            offAnimation.SetTarget(colorTarget);
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

            if (colorTarget == null)
                return;

            colorTarget.color = onAnimation.startColor;

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

        private static void ResetAnimation(ColorAnimation target)
        {
            target.animation.Reset();
            target.animation.enabled = true;
            target.animation.fromReferenceValue = ReferenceValue.CurrentValue;
            target.animation.settings.duration = UISelectable.k_DefaultAnimationDuration;
        }

        /// <summary> Set the Start Color for all animations </summary>
        /// <param name="color"> New start color </param>
        public void SetStartColor(Color color)
        {
            SetStartColorForOn(color);
            SetStartColorForOff(color);
        }

        /// <summary> Set the Start Color for the ON animation </summary>
        /// <param name="color"> New start color </param>
        public void SetStartColorForOn(Color color)
        {
            onAnimation.customStartValue = color;
            if (controller == null) return;
            switch (controller.isOn)
            {
                case true:
                    onAnimation.SetProgressAtOne();
                    break;
                case false:
                    onAnimation.Play(PlayDirection.Forward);
                    break;
            }
        }

        /// <summary> Set the Start Color for the OFF animation </summary>
        /// <param name="color"> New start color </param>
        public void SetStartColorForOff(Color color)
        {
            offAnimation.customStartValue = color;
            if (controller == null) return;
            switch (controller.isOn)
            {
                case true:
                    offAnimation.Play(PlayDirection.Forward);
                    break;
                case false:
                    offAnimation.SetProgressAtOne();
                    break;
            }
        }
    }
}
