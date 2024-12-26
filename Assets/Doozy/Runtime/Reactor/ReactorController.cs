// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Reactor.Animators.Internal;
using UnityEngine;
using UnityEngine.Events;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.Reactor
{
    /// <summary> Specialized controller that controls multiple ReactorAnimators at the same time </summary>
    [AddComponentMenu("Reactor/Reactor Controller")]
    public class ReactorController : MonoBehaviour
    {
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/Reactor/Reactor Controller", false, 6)]
        private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        {
            GameObjectUtils.AddToScene<ReactorController>(false, true);
        }
        #endif
        
        /// <summary>
        /// Describes the controller working mode
        /// </summary>
        public enum Mode
        {
            /// <summary> Set animators manually without any logic in Awake() </summary>
            Manual,

            /// <summary> Automatically search and populate the controlled animators list in Awake() </summary>
            Automatic
        }

        /// <summary> Controller name </summary>
        public string ControllerName;

        /// <summary> Controller animators behaviour on Start </summary>
        public AnimatorBehaviour OnStartBehaviour = AnimatorBehaviour.Disabled;

        /// <summary> Controller animators behaviour on Enable </summary>
        public AnimatorBehaviour OnEnableBehaviour = AnimatorBehaviour.Disabled;

        /// <summary> Override the OnStart and OnEnable animator behaviours for all connected animators </summary>
        public bool OverrideAnimatorsBehaviors;

        /// <summary> Determines if the controller should automatically search for animators or not </summary>
        public Mode ControllerMode = Mode.Manual;

        [SerializeField] private List<ReactorAnimator> Animators;
        /// <summary> All the animators controlled by this controller </summary>
        public List<ReactorAnimator> animators
        {
            get => Animators ?? (Animators = new List<ReactorAnimator>());
            private set => Animators = value;
        }

        /// <summary> Check if this controller controls any animators </summary>
        public bool hasAnimators => animators.Count > 0;

        /// <summary> Flag used to mark when the controller has been initialized </summary>
        public bool initialized { get; private set; }

        protected virtual void Awake()
        {
            if (!Application.isPlaying) return;
            initialized = false;
            Initialize();
        }

        protected virtual void OnEnable()
        {
            if (!Application.isPlaying) return;
            RunBehaviour(OnEnableBehaviour);
        }

        protected virtual void Start()
        {
            if (!Application.isPlaying) return;
            RunBehaviour(OnStartBehaviour);
        }

        /// <summary> Start the controller initialization process </summary>
        protected virtual void Initialize()
        {
            if (initialized) return;
            switch (ControllerMode)
            {
                case Mode.Automatic:
                    animators = GetComponentsInChildren<ReactorAnimator>().Where(c => c.isActiveAndEnabled).ToList();
                    break;
                case Mode.Manual:
                    //ignored
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!hasAnimators)
            {
                initialized = true;
                return;
            }

            if (OverrideAnimatorsBehaviors)
            {
                foreach (ReactorAnimator animator in animators.RemoveNulls())
                {
                    animator.OnStartBehaviour = AnimatorBehaviour.Disabled;
                    animator.OnEnableBehaviour = AnimatorBehaviour.Disabled;
                }
            }

            initialized = true;
        }

        // <summary> Execute the given behaviour </summary>
        /// <param name="behaviour"> Controller behaviour </param>
        protected virtual void RunBehaviour(AnimatorBehaviour behaviour)
        {
            if (behaviour == AnimatorBehaviour.Disabled)
                return;

            bool animatorsInitialized = true;
            foreach (ReactorAnimator a in animators.RemoveNulls())
                if (!a.animatorInitialized)
                    animatorsInitialized = false;

            if (!initialized || !animatorsInitialized)
            {
                DelayExecution(() => RunBehaviour(behaviour));
                return;
            }
            
            InitializeAnimators();

            switch (behaviour)
            {
                case AnimatorBehaviour.PlayForward:
                    Play(PlayDirection.Forward);
                    break;

                case AnimatorBehaviour.PlayReverse:
                    Play(PlayDirection.Reverse);
                    break;

                case AnimatorBehaviour.SetFromValue:
                    SetProgressAtZero();
                    break;

                case AnimatorBehaviour.SetToValue:
                    SetProgressAtOne();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(behaviour), behaviour, null);
            }
        }

        /// <summary> Delay any execution until the controller has been initialized </summary>
        /// <param name="callback"> Unity action callback </param>
        protected void DelayExecution(UnityAction callback) =>
            StartCoroutine(ExecuteAfterControllerInitialized(callback));

        /// <summary> Invoke the given callback after the controller has been initialized </summary>
        /// <param name="callback"> Unity action callback </param>
        protected IEnumerator ExecuteAfterControllerInitialized(UnityAction callback)
        {
            yield return new WaitUntil(() => initialized);
            callback?.Invoke();
        }

        /// <summary> Initialize all the controlled animators (update settings and set the initialized flag) </summary>
        public void InitializeAnimators()
        {
            foreach (var a in animators.RemoveNulls())
                a.Initialize();
        }

        /// <summary> Update the initial values for the animators controlled by this controller </summary>
        public void UpdateValues()
        {
            foreach (var a in animators.RemoveNulls())
                a.UpdateValues();
        }

        /// <summary> Set all the controlled animators animations at 100% (at the end, or the 'To' value) </summary>
        public void SetProgressAtOne()
        {
            foreach (var a in animators.RemoveNulls())
                a.SetProgressAtOne();
        }

        /// <summary> Set all the controlled animators animations at 0% (at the start, or the 'From' value) </summary>
        public void SetProgressAtZero()
        {
            foreach (var a in animators.RemoveNulls())
                a.SetProgressAtZero();
        }

        /// <summary> Set all the controlled animators animations at the given progress value </summary>
        /// <param name="targetProgress"> Target progress [0,1] </param>
        public void SetProgressAt(float targetProgress)
        {
            foreach (var a in animators.RemoveNulls())
                a.SetProgressAt(targetProgress);
        }

        /// <summary> Play all the controlled animators animations at the given progress value from the current value </summary>
        /// <param name="toProgress"> To progress [0,1] </param>
        public void PlayToProgress(float toProgress)
        {
            foreach (var a in animators.RemoveNulls())
                a.PlayToProgress(toProgress);
        }

        /// <summary> Play all the controlled animators animations from the given progress value to the current value </summary>
        /// <param name="fromProgress"> From progress [0,1] </param>
        public void PlayFromProgress(float fromProgress)
        {
            foreach (var a in animators.RemoveNulls())
                a.PlayFromProgress(fromProgress);
        }

        /// <summary> Play all the controlled animators animations from the given progress value to the given progress value </summary>
        /// <param name="fromProgress"> From progress [0,1] </param>
        /// <param name="toProgress"> To progress [0,1] </param>
        public void PlayFromToProgress(float fromProgress, float toProgress)
        {
            foreach (var a in animators.RemoveNulls())
                a.PlayFromToProgress(fromProgress, toProgress);
        }

        /// <summary> Play all the controlled animators animations all the way in the given direction </summary>
        /// <param name="playDirection"> Play direction (Forward or Reverse) </param>
        public void Play(PlayDirection playDirection)
        {
            foreach (var a in animators.RemoveNulls())
                a.Play(playDirection);
        }

        /// <summary> Play all the controlled animators animations all the way </summary>
        /// <param name="inReverse"> Play the animation in reverse? </param>
        public void Play(bool inReverse = false)
        {
            foreach (var a in animators.RemoveNulls())
                a.Play(inReverse);
        }

        /// <summary> Reset all the controlled animators animations to their initial values (if the animations are enabled) </summary>
        /// <param name="forced"> If true, forced will ignore if the animation is enabled or not </param>
        public void ResetToStartValues(bool forced = false)
        {
            foreach (var a in animators.RemoveNulls())
                a.ResetToStartValues(forced);
        }

        /// <summary> Stop all the controlled animators animations </summary>
        public void Stop()
        {
            foreach (var a in animators.RemoveNulls())
                a.Stop();
        }

        /// <summary> Finish all the controlled animators animations </summary>
        public void Finish()
        {
            foreach (var a in animators.RemoveNulls())
                a.Finish();
        }

        /// <summary>
        /// Reverse all the controlled animators animations direction while playing.
        /// Works only if the animations are active (either playing or paused)
        /// </summary>
        public void Reverse()
        {
            foreach (var a in animators.RemoveNulls())
                a.Reverse();
        }

        /// <summary> Rewind all the controlled animators animations to the start values </summary>
        public void Rewind()
        {
            foreach (var a in animators.RemoveNulls())
                a.Rewind();
        }

        /// <summary>
        /// Pause all the controlled animators animations.
        /// Works only if the animations are playing.
        /// </summary>
        public void Pause()
        {
            foreach (var a in animators.RemoveNulls())
                a.Pause();
        }

        /// <summary>
        /// Resume all the controlled animators animations.
        /// Works only if the animations are paused.
        /// </summary>
        public void Resume()
        {
            foreach (var a in animators.RemoveNulls())
                a.Resume();
        }

        /// <summary> Refresh all the controlled animators animations target and update values </summary>
        public void UpdateSettings()
        {
            foreach (var a in animators.RemoveNulls())
                a.UpdateSettings();
        }

        /// <summary>
        /// Get the max start delay of all the controlled animators animations.
        /// For random values it returns the max value.
        /// </summary>
        public float GetStartDelay()
        {
            float result = 0f;
            foreach (ReactorAnimator animator in animators.RemoveNulls())
                result = Mathf.Max(result, animator.GetStartDelay());
            return result;
        }

        /// <summary>
        /// Get the maximum duration (without start delay) of all the controlled animators animations.
        /// For random values it returns the max value.
        /// </summary>
        public float GetDuration()
        {
            float result = 0f;
            foreach (ReactorAnimator animator in animators.RemoveNulls())
                result = Mathf.Max(result, animator.GetDuration());
            return result;
        }

        /// <summary> Get the maxi start delay + duration of all the controlled animators animations. </summary>
        public float GetTotalDuration()
        {
            return GetStartDelay() + GetDuration();
        }
    }
}
