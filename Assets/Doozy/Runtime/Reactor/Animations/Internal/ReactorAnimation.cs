// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine.Events;
// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Runtime.Reactor.Animations
{
    /// <summary>
    /// Base class for a Reactor animation.
    /// It contains all the methods and properties needed to work with an animation.
    /// An animation can contain any number of reactions.
    /// </summary>
    [Serializable]
    public abstract class ReactorAnimation
    {
        /// <summary> UnityEvent triggered when the animation starts playing </summary>
        public UnityEvent OnPlayCallback;
        /// <summary> UnityEvent triggered when the animation stops playing </summary>
        public UnityEvent OnStopCallback;
        /// <summary> UnityEvent triggered when the animation finished playing </summary>
        public UnityEvent OnFinishCallback;

        /// <summary> Is the animation enabled </summary>
        public abstract bool isEnabled { get; }
        /// <summary> Is the animation in the idle state (is enabled, but not active) </summary>
        public abstract bool isIdle { get; }
        /// <summary> Is the animation in the active state (is enabled and either playing or paused) </summary>
        public abstract bool isActive { get; }
        /// <summary> Is the animation in the paused state (is enabled, started playing and then paused) </summary>
        public abstract bool isPaused { get; }
        /// <summary> Is the animation in the playing state (is enabled and started playing) </summary>
        public abstract bool isPlaying { get; }
        /// <summary> Is the animation in the start delay state (is enabled, started playing and waiting to start running after the start delay duration has passed) </summary>
        public abstract bool inStartDelay { get; }
        /// <summary> Is the animation in the loop delay state (is enabled, started playing and is between loops waiting to continue running after the loop delay duration has passed) </summary>
        public abstract bool inLoopDelay { get; }

        protected int startedReactionsCount { get; set; }
        protected int stoppedReactionsCount { get; set; }
        protected int finishedReactionsCount { get; set; }

        protected bool onPlayInvoked { get; set; }

        protected void InvokeOnPlay()
        {
            if (startedReactionsCount <= 0) return;
            if (onPlayInvoked) return;
            OnPlayCallback?.Invoke();
            onPlayInvoked = true;
        }

        protected void InvokeOnStop()
        {
            if (startedReactionsCount <= 0) return;
            stoppedReactionsCount++;
            if (stoppedReactionsCount < startedReactionsCount) return;
            OnStopCallback?.Invoke();
        }

        protected void InvokeOnFinish()
        {
            if (startedReactionsCount <= 0) return;
            finishedReactionsCount++;
            if (finishedReactionsCount < startedReactionsCount) return;
            OnFinishCallback?.Invoke();
        }

        /// <summary>
        /// Called to recycle the reactions controlled by this animation.
        /// <para/> Reactions are pooled can (and should) be recycled to improve overall performance. 
        /// </summary>
        public abstract void Recycle();

        /// <summary>
        /// Called when the reactions controlled by this animation need to get their initial values updated
        /// </summary>
        public abstract void UpdateValues();

        /// <summary>
        /// Stop all the reactions controlled by this animation
        /// </summary>
        public abstract void StopAllReactionsOnTarget();

        /// <summary> Set the animation at 100% (at the end, or the 'To' value) </summary>
        public void SetProgressAtOne() =>
            SetProgressAt(1f);

        /// <summary> Set the animation at 0% (at the start, or the 'From' value) </summary>
        public void SetProgressAtZero() =>
            SetProgressAt(0f);

        /// <summary> Set the animation at the given progress value </summary>
        /// <param name="targetProgress"> Target progress [0,1] </param>
        public virtual void SetProgressAt(float targetProgress)
        {
            StopAllReactionsOnTarget();
            UpdateValues();
        }

        /// <summary> Play the animation at the given progress value from the current value </summary>
        /// <param name="toProgress"> To progress [0,1] </param>
        public virtual void PlayToProgress(float toProgress)
        {
            StopAllReactionsOnTarget();
            UpdateValues();
            RegisterCallbacks();
        }

        /// <summary> Play the animation from the given progress value to the current value </summary>
        /// <param name="fromProgress"> From progress [0,1] </param>
        public virtual void PlayFromProgress(float fromProgress)
        {
            StopAllReactionsOnTarget();
            UpdateValues();
            RegisterCallbacks();
        }

        /// <summary> Play the animation from the given progress value to the given progress value </summary>
        /// <param name="fromProgress"> From progress [0,1] </param>
        /// <param name="toProgress"> To progress [0,1] </param>
        public virtual void PlayFromToProgress(float fromProgress, float toProgress)
        {
            StopAllReactionsOnTarget();
            UpdateValues();
            RegisterCallbacks();
        }

        /// <summary> Play the animation all the way in the given direction </summary>
        /// <param name="playDirection"> Play direction (Forward or Reverse) </param>
        public void Play(PlayDirection playDirection) =>
            Play(playDirection == PlayDirection.Reverse);

        /// <summary> Play the animation all the way </summary>
        /// <param name="inReverse"> Play the animation in reverse? </param>
        public abstract void Play(bool inReverse = false);
        
        /// <summary> Reset all the reactions to their initial values (if the animation is enabled) </summary>
        /// <param name="forced"> If true, forced will ignore if the animation is enabled or not </param>
        public abstract void ResetToStartValues(bool forced = false);

        /// <summary>
        /// Stop the animation.
        /// Called every time the animation is stopped. Also called before calling Finish()
        /// </summary>
        public virtual void Stop()
        {
            UnregisterOnPlayCallbacks();
            UnregisterOnStopCallbacks();
        }

        /// <summary>
        /// Finish the animation.
        /// Called to mark that that animation completed playing.
        /// </summary>
        public virtual void Finish()
        {
            UnregisterCallbacks();
        }

        /// <summary>
        /// Reverse the animation's direction while playing.
        /// Works only if the animation is active (it either playing or paused)
        /// </summary>
        public abstract void Reverse();
        
        /// <summary>
        /// Rewind the animation to the start values
        /// </summary>
        public abstract void Rewind();
        
        /// <summary>
        /// Pause the animation.
        /// Works only if the animation is playing.
        /// </summary>
        public abstract void Pause();
        
        /// <summary>
        /// Resume a paused animation.
        /// Works only if the animation is paused.
        /// </summary>
        public abstract void Resume();

        /// <summary> Register all callbacks to the animation </summary>
        protected virtual void RegisterCallbacks()
        {
            UnregisterCallbacks();

            onPlayInvoked = false;
            startedReactionsCount = 0;
            stoppedReactionsCount = 0;
            finishedReactionsCount = 0;
        }

        /// <summary> Unregister the callbacks from the animation </summary>
        protected void UnregisterCallbacks()
        {
            UnregisterOnPlayCallbacks();
            UnregisterOnStopCallbacks();
            UnregisterOnFinishCallbacks();
        }

        /// <summary> Unregister OnPlayCallback </summary>
        protected abstract void UnregisterOnPlayCallbacks();
        /// <summary> Unregister OnStopCallback </summary>
        protected abstract void UnregisterOnStopCallbacks();
        /// <summary> Unregister OnFinishCallback </summary>
        protected abstract void UnregisterOnFinishCallbacks();
    }
}
