// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Animators.Internal;
using Doozy.Runtime.Reactor.Reflection;
using Doozy.Runtime.Reactor.Ticker;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.Reactor.Animators
{
    /// <summary>
    /// Specialized Reactor class that allows you to animate any public Vector3 field or property of any object via Reactor.
    /// </summary>
    [AddComponentMenu("Reactor/Animators/Vector3 Animator")]
    public class Vector3Animator : ReflectedValueAnimator
    {
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/Reactor/Animators/Vector3 Animator", false, 6)]
        private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        {
            GameObjectUtils.AddToScene<Vector3Animator>(false, true);
        }
        #endif
        
        /// <summary> Vector3 value target accessed via reflection </summary>
        public ReflectedVector3 ValueTarget;

        /// <summary> Check if the value target is set up correctly </summary>
        public bool isValid => ValueTarget.IsValid();

        [SerializeField] private Vector3Animation Animation;
        /// <summary> Vector3 Animation </summary>
        public new Vector3Animation animation => Animation ?? (Animation = new Vector3Animation(ValueTarget));

        #if UNITY_EDITOR
        private void Reset()
        {
            Animation ??= new Vector3Animation(ValueTarget);
            ResetAnimation();
        }
        #endif
        
        /// <summary> Play the animation all the way in the given direction </summary>
        /// <param name="playDirection"> Play direction (Forward or Reverse) </param>
        public override void Play(PlayDirection playDirection) =>
            animation.Play(playDirection);

        /// <summary> Play the animation all the way </summary>
        /// <param name="inReverse"> Play the animation in reverse? </param>
        public override void Play(bool inReverse = false) =>
            animation.Play(inReverse);

        /// <summary> Set the value target </summary>
        /// <param name="reflectedValue"> Reflected value target </param>
        public override void SetTarget(object reflectedValue) =>
            SetTarget(reflectedValue as ReflectedVector3);

        /// <summary> Set the value target </summary>
        /// <param name="reflectedValue"> Reflected value target </param>
        public void SetTarget(ReflectedVector3 reflectedValue) =>
            animation.SetTarget(reflectedValue);

        /// <summary> Reset all the reactions to their initial values (if the animation is enabled) </summary>
        /// <param name="forced"> If true, forced will ignore if the animation is enabled or not </param>
        public override void ResetToStartValues(bool forced = false)
        {
            if (animation.isActive) Stop();
            animation.ResetToStartValues(forced);

            if (ValueTarget == null || !ValueTarget.IsValid())
                return;

            ValueTarget.SetValue(animation.startValue);

            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(ValueTarget.target);
            UnityEditor.SceneView.RepaintAll();
            #endif
        }

        /// <summary> Refresh the set target and, if the animation is playing, update the calculated values </summary>
        public override void UpdateSettings()
        {
            SetTarget(ValueTarget);
            if (animation.isPlaying) UpdateValues();
        }

        /// <summary> Get the animation start delay. For random values it returns the max value. </summary>
        public override float GetStartDelay() =>
            animation.animation.isActive ? animation.animation.startDelay : animation.animation.settings.GetStartDelay();

        /// <summary> Get the animation duration. For random values it returns the max value. </summary>
        public override float GetDuration() =>
            animation.animation.isActive ? animation.animation.duration : animation.animation.settings.GetDuration();

        /// <summary> Get the animation start delay + duration. For random values it returns the max value. </summary>
        public override float GetTotalDuration() =>
            GetStartDelay() + GetDuration();
        
        /// <summary> Set animation heartbeat </summary>
        public override List<Heartbeat> SetHeartbeat<T>()
        {
            var list = new List<Heartbeat>() { new T() };
            animation.animation.SetHeartbeat(list[0]);
            return list;
        }
        
        /// <summary> Update From and To values by recalculating them </summary>
        public override void UpdateValues() =>
            animation.UpdateValues();

        /// <summary> Play the animation at the given progress value from the current value </summary>
        /// <param name="toProgress"> To progress [0,1] </param>
        public override void PlayToProgress(float toProgress) =>
            animation.PlayToProgress(toProgress);

        /// <summary> Play the animation from the given progress value to the current value </summary>
        /// <param name="fromProgress"> From progress [0,1] </param>
        public override void PlayFromProgress(float fromProgress) =>
            animation.PlayFromProgress(fromProgress);

        /// <summary> Play the animation from the given progress value to the given progress value </summary>
        /// <param name="fromProgress"> From progress [0,1] </param>
        /// <param name="toProgress"> To progress [0,1] </param>
        public override void PlayFromToProgress(float fromProgress, float toProgress) =>
            animation.PlayFromToProgress(fromProgress, toProgress);

        /// <summary>
        /// Stop the animation.
        /// Called every time the animation is stopped.
        /// Also called before calling Finish()
        /// </summary>
        public override void Stop() =>
            animation.Stop();

        /// <summary>
        /// Finish the animation.
        /// Called to mark that that animation completed playing.
        /// </summary>
        public override void Finish() =>
            animation.Finish();

        /// <summary>
        /// Reverse the animation's direction while playing.
        /// Works only if the animation is active (it either playing or paused)
        /// </summary>
        public override void Reverse() =>
            animation.Reverse();

        /// <summary>
        /// Rewind the animation to the start values
        /// </summary>
        public override void Rewind() =>
            animation.Rewind();

        /// <summary>
        /// Pause the animation.
        /// Works only if the animation is playing.
        /// </summary>
        public override void Pause() =>
            animation.Pause();

        /// <summary>
        /// Resume a paused animation.
        /// Works only if the animation is paused.
        /// </summary>
        public override void Resume() =>
            animation.Resume();

        /// <summary> Set the animation at 100% (at the end, or the 'To' value) </summary>
        public override void SetProgressAtOne() =>
            animation.SetProgressAtOne();

        /// <summary> Set the animation at 0% (at the start, or the 'From' value) </summary>
        public override void SetProgressAtZero() =>
            animation.SetProgressAtZero();

        /// <summary> Set the animation at the given progress value </summary>
        /// <param name="targetProgress"> Target progress [0,1] </param>
        public override void SetProgressAt(float targetProgress) =>
            animation.SetProgressAt(targetProgress);

        /// <summary>
        /// Recycle the reactions controlled by this animation.
        /// <para/> Reactions are pooled can (and should) be recycled to improve overall performance. 
        /// </summary>
        protected override void Recycle() =>
            animation?.Recycle();

        private void ResetAnimation()
        {
            var reaction = animation.animation;

            reaction.Reset();
            reaction.enabled = true;
            reaction.fromReferenceValue = ReferenceValue.CustomValue;
            reaction.fromCustomValue = Vector3.zero;
            reaction.toReferenceValue = ReferenceValue.CustomValue;
            reaction.toCustomValue = Vector3.one;
            reaction.settings.duration = 1f;
        }
    }
}
