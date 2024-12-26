// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.Reactor.Reflection;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Animations
{
    [Serializable]
    public class IntAnimation : ReactorAnimation
    {
        /// <summary> Int value target accessed via reflection  </summary>
        public ReflectedInt valueTarget { get; private set; }

        [SerializeField] private ReflectedIntReaction Animation;
        /// <summary> Int reaction designed to animate a integer value over time </summary>
        public ReflectedIntReaction animation => Animation ?? (Animation = Reaction.Get<ReflectedIntReaction>());

        /// <summary> Animation start value </summary>
        public int startValue
        {
            get => animation.startValue;
            set => animation.startValue = value;
        }

        /// <summary> Is the animation enabled </summary>
        public override bool isEnabled => animation.enabled;
        /// <summary> Is the animation in the idle state (is enabled, but not active) </summary>
        public override bool isIdle => animation.isIdle;
        /// <summary> Is the animation in the active state (is enabled and either playing or paused) </summary>
        public override bool isActive => animation.isActive;
        /// <summary> Is the animation in the paused state (is enabled, started playing and then paused) </summary>
        public override bool isPaused => animation.isPaused;
        /// <summary> Is the animation in the playing state (is enabled and started playing) </summary>
        public override bool isPlaying => animation.isPlaying;
        /// <summary> Is the animation in the start delay state (is enabled, started playing and waiting to start running after the start delay duration has passed) </summary>
        public override bool inStartDelay => animation.inStartDelay;
        /// <summary> Is the animation in the loop delay state (is enabled, started playing and is between loops waiting to continue running after the loop delay duration has passed) </summary>
        public override bool inLoopDelay => animation.inLoopDelay;

        /// <summary> Construct a new animation </summary>
        public IntAnimation(ReflectedInt reflectedInt)
        {
            valueTarget = reflectedInt;
        }
        
        /// <summary> Set the value target </summary>
        /// <param name="target"> Value target </param>
        public void SetTarget(ReflectedInt target)
        {
            valueTarget = null;
            _ = target ?? throw new NullReferenceException(nameof(target));
            valueTarget = target;

            Initialize();
        }
        
        /// <summary> Initialize the animation </summary>
        public void Initialize()
        {
            animation?.Stop(true);
            Animation ??= Reaction.Get<ReflectedIntReaction>();

            valueTarget.Initialize();
            animation?.SetTarget(valueTarget);

            UpdateValues();
        }
        
        /// <summary>
        /// Recycle the reactions controlled by this animation.
        /// <para/> Reactions are pooled can (and should) be recycled to improve overall performance. 
        /// </summary>
        public override void Recycle() =>
            animation?.Recycle();

        /// <summary> Update From and To values by recalculating them </summary>
        public override void UpdateValues() =>
            animation.UpdateValues();
        
        /// <summary>
        /// Stop all the reactions controlled by this animation
        /// </summary>
        public override void StopAllReactionsOnTarget() =>
            Reaction.StopAllReactionsByTargetObject(valueTarget.target, true);

        /// <summary> Set the animation at the given progress value </summary>
        /// <param name="targetProgress"> Target progress [0,1] </param>
        public override void SetProgressAt(float targetProgress)
        {
            base.SetProgressAt(targetProgress);
            if (animation.enabled) animation.SetProgressAt(targetProgress);
        }

        /// <summary> Play the animation at the given progress value from the current value </summary>
        /// <param name="toProgress"> To progress [0,1] </param>
        public override void PlayToProgress(float toProgress)
        {
            base.PlayToProgress(toProgress);
            if (animation.enabled) animation.PlayToProgress(toProgress);
        }

        /// <summary> Play the animation from the given progress value to the current value </summary>
        /// <param name="fromProgress"> From progress [0,1] </param>
        public override void PlayFromProgress(float fromProgress)
        {
            base.PlayFromProgress(fromProgress);
            if (animation.enabled) animation.PlayFromProgress(fromProgress);
        }

        /// <summary> Play the animation from the given progress value to the given progress value </summary>
        /// <param name="fromProgress"> From progress [0,1] </param>
        /// <param name="toProgress"> To progress [0,1] </param>
        public override void PlayFromToProgress(float fromProgress, float toProgress)
        {
            base.PlayFromToProgress(fromProgress, toProgress);
            if (animation.enabled) animation.PlayFromToProgress(fromProgress, toProgress);
        }

        /// <summary> Play the animation all the way </summary>
        /// <param name="inReverse"> Play the animation in reverse? </param>
        public override void Play(bool inReverse = false)
        {
            if (valueTarget == null)
                return;
            
            valueTarget.Initialize();
            
            if (!valueTarget.IsValid())
                return;

            RegisterCallbacks();
            if (!isActive)
            {
                StopAllReactionsOnTarget();
                // ResetToStartValues();
            }

            if (animation.enabled) animation.Play(inReverse);
        }

        /// <summary> Reset all the reactions to their initial values (if the animation is enabled) </summary>
        /// <param name="forced"> If true, forced will ignore if the animation is enabled or not </param>
        public override void ResetToStartValues(bool forced = false)
        {
            if (valueTarget == null || valueTarget.target == null) return;
            if (forced || animation.enabled)
            {
                animation.SetValue(startValue);
            }
        }

        /// <summary>
        /// Stop the animation.
        /// Called every time the animation is stopped.
        /// Also called before calling Finish()
        /// </summary>
        public override void Stop()
        {
            if (animation.isActive || animation.enabled) animation.Stop();
            base.Stop();
        }

        /// <summary>
        /// Finish the animation.
        /// Called to mark that that animation completed playing.
        /// </summary>
        public override void Finish()
        {
            if (animation.isActive || animation.enabled) animation.Finish();
            base.Finish();
        }

        /// <summary>
        /// Reverse the animation's direction while playing.
        /// Works only if the animation is active (it either playing or paused)
        /// </summary>
        public override void Reverse()
        {
            if (animation.isActive) animation.Reverse();
            else if (animation.enabled) animation.Play(PlayDirection.Reverse);
        }

        /// <summary>
        /// Rewind the animation to the start values
        /// </summary>
        public override void Rewind()
        {
            if (animation.enabled) animation.Rewind();
        }

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

        /// <summary> Register all callbacks to the animation </summary>
        protected override void RegisterCallbacks()
        {
            base.RegisterCallbacks();

            if (!animation.enabled)
                return;

            startedReactionsCount++;
            animation.OnPlayCallback += InvokeOnPlay;
            animation.OnStopCallback += InvokeOnStop;
            animation.OnFinishCallback += InvokeOnFinish;
        }

        /// <summary> Unregister OnPlayCallback </summary>
        protected override void UnregisterOnPlayCallbacks()
        {
            if (!animation.enabled)
                return;

            animation.OnPlayCallback -= InvokeOnPlay;
        }

        /// <summary> Unregister OnStopCallback </summary>
        protected override void UnregisterOnStopCallbacks()
        {
            if (!animation.enabled)
                return;

            animation.OnStopCallback -= InvokeOnStop;
        }

        /// <summary> Unregister OnFinishCallback </summary>
        protected override void UnregisterOnFinishCallbacks()
        {
            if (!animation.enabled)
                return;

            animation.OnFinishCallback -= InvokeOnFinish;
        }
    }
}