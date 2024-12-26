// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections;
using System.Collections.Generic;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Ticker;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.Reactor.Animators.Internal
{
    /// <summary> Base class for Reactor animators </summary>
    [Serializable]
    public abstract class ReactorAnimator : MonoBehaviour
    {
        /// <summary> Animator name </summary>
        public string AnimatorName;

        /// <summary> Animator behaviour on Start </summary>
        public AnimatorBehaviour OnStartBehaviour = AnimatorBehaviour.Disabled;

        /// <summary> Animator behaviour on Enable </summary>
        public AnimatorBehaviour OnEnableBehaviour = AnimatorBehaviour.Disabled;

        /// <summary> Initialize with a delay </summary>
        protected Coroutine initializeLater { get; set; }

        /// <summary> Flag used to mark when the animation has been initialized </summary>
        public bool animatorInitialized { get; set; }

        protected virtual void Awake()
        {
            if (!Application.isPlaying) return;
            animatorInitialized = false;
        }

        protected virtual void OnEnable()
        {
            if (!Application.isPlaying) return;
            Initialize();
            RunBehaviour(OnEnableBehaviour);
        }

        protected virtual void Start()
        {
            if (!Application.isPlaying) return;
            RunBehaviour(OnStartBehaviour);
        }

        protected virtual void OnDestroy()
        {
            if (!Application.isPlaying) return;
            Recycle();
        }

        /// <summary> Start the animator initialization process </summary>
        public virtual void Initialize()
        {
            if (animatorInitialized) return;
            if (initializeLater != null)
            {
                StopCoroutine(initializeLater);
                initializeLater = null;
            }
            initializeLater = StartCoroutine(InitializeLater());
        }

        /// <summary> Initialize the animator with a delay (at the end of frame) </summary>
        protected IEnumerator InitializeLater()
        {
            yield return new WaitForEndOfFrame();
            InitializeAnimator();
        }

        /// <summary> Initialize the animator (update settings and set the initialized flag) </summary>
        public virtual void InitializeAnimator()
        {
            UpdateSettings();
            animatorInitialized = true;
        }

        /// <summary> Execute the given behaviour </summary>
        /// <param name="behaviour"> Animator behaviour </param>
        protected void RunBehaviour(AnimatorBehaviour behaviour)
        {
            if (behaviour == AnimatorBehaviour.Disabled)
                return;

            if (!animatorInitialized)
            {
                DelayExecution(() => RunBehaviour(behaviour));
                return;
            }

            switch (behaviour)
            {
                case AnimatorBehaviour.PlayForward:
                    Play(PlayDirection.Forward);
                    return;

                case AnimatorBehaviour.PlayReverse:
                    Play(PlayDirection.Reverse);
                    return;

                case AnimatorBehaviour.SetFromValue:
                    SetProgressAtZero();
                    return;

                case AnimatorBehaviour.SetToValue:
                    SetProgressAtOne();
                    return;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary> Delay any execution until the animator has been initialized </summary>
        /// <param name="callback"> Unity action callback </param>
        protected void DelayExecution(UnityAction callback) =>
            StartCoroutine(ExecuteAfterAnimatorInitialized(callback));

        /// <summary> Invoke the given callback after the animator has been initialized </summary>
        /// <param name="callback"> Unity action callback </param>
        protected IEnumerator ExecuteAfterAnimatorInitialized(UnityAction callback)
        {
            yield return new WaitUntil(() => animatorInitialized);
            callback?.Invoke();
        }

        /// <summary>
        /// Recycle the reactions controlled by this animation.
        /// <para/> Reactions are pooled can (and should) be recycled to improve overall performance. 
        /// </summary>
        protected abstract void Recycle();

        /// <summary> Update the initial values for the reactions controlled by this animation </summary>
        public abstract void UpdateValues();

        /// <summary> Set the animation at 100% (at the end, or the 'To' value) </summary>
        public abstract void SetProgressAtOne();

        /// <summary> Set the animation at 0% (at the start, or the 'From' value) </summary>
        public abstract void SetProgressAtZero();

        /// <summary> Set the animation at the given progress value </summary>
        /// <param name="targetProgress"> Target progress [0,1] </param>
        public abstract void SetProgressAt(float targetProgress);

        /// <summary> Play the animation at the given progress value from the current value </summary>
        /// <param name="toProgress"> To progress [0,1] </param>
        public abstract void PlayToProgress(float toProgress);

        /// <summary> Play the animation from the given progress value to the current value </summary>
        /// <param name="fromProgress"> From progress [0,1] </param>
        public abstract void PlayFromProgress(float fromProgress);

        /// <summary> Play the animation from the given progress value to the given progress value </summary>
        /// <param name="fromProgress"> From progress [0,1] </param>
        /// <param name="toProgress"> To progress [0,1] </param>
        public abstract void PlayFromToProgress(float fromProgress, float toProgress);

        /// <summary> Play the animation all the way in the given direction </summary>
        /// <param name="playDirection"> Play direction (Forward or Reverse) </param>
        public abstract void Play(PlayDirection playDirection);

        /// <summary> Play the animation all the way </summary>
        /// <param name="inReverse"> Play the animation in reverse? </param>
        public abstract void Play(bool inReverse = false);

        /// <summary> Reset all the reactions to their initial values (if the animation is enabled) </summary>
        /// <param name="forced"> If true, forced will ignore if the animation is enabled or not </param>
        public abstract void ResetToStartValues(bool forced = false);

        /// <summary>
        /// Stop the animation. Called every time the animation is stopped. Also called before calling Finish()
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Finish the animation. Called to mark that that animation completed playing.
        /// </summary>
        public abstract void Finish();

        /// <summary>
        /// Reverse the animation's direction while playing.
        /// Works only if the animation is active (either playing or paused)
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

        /// <summary> Set the animation target </summary>
        /// <param name="target"> Animation target</param>
        public abstract void SetTarget(object target);

        /// <summary> Refresh the animation target and update values </summary>
        public abstract void UpdateSettings();

        /// <summary> Animation start delay </summary>
        public abstract float GetStartDelay();

        /// <summary> Animation duration (without start delay) </summary>
        public abstract float GetDuration();

        /// <summary> Animation duration (with start delay) </summary>
        public abstract float GetTotalDuration();

        /// <summary> Set animation heartbeat </summary>
        public abstract List<Heartbeat> SetHeartbeat<T>() where T : Heartbeat, new();
    }
}
