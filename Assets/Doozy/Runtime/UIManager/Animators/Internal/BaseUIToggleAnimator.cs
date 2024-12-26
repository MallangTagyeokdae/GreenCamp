// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Events;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.UIManager.Animators
{
    /// <summary> Base class for all UIToggle animators for On and Off states </summary>
    public abstract class BaseUIToggleAnimator : BaseTargetComponentAnimator<UIToggle>
    {
        private Coroutine initializeCoroutine { get; set; }

        /// <summary> Is the on animation in the active state (is enabled and either playing or paused) </summary>
        protected abstract bool onAnimationIsActive { get; }

        /// <summary> Is the off animation in the active state (is enabled and either playing or paused) </summary>
        protected abstract bool offAnimationIsActive { get; }

        /// <summary> Called when the On animation needs to be played </summary>
        protected abstract UnityAction playOnAnimation { get; }

        /// <summary> Called when the Off animation needs to be played </summary>
        protected abstract UnityAction playOffAnimation { get; }

        /// <summary> Called when the On animation needs to be played in reverse </summary>
        protected abstract UnityAction reverseOnAnimation { get; }

        /// <summary> Called when the Off animation needs to be played in reverse </summary>
        protected abstract UnityAction reverseOffAnimation { get; }

        /// <summary> Called when the On animation needs to have it's progress set to 1 </summary>
        protected abstract UnityAction instantPlayOnAnimation { get; }

        /// <summary> Called when the Off animation needs to have it's progress set to 1 </summary>
        protected abstract UnityAction instantPlayOffAnimation { get; }

        /// <summary> Called when the On animation needs to be stopped </summary>
        protected abstract UnityAction stopOnAnimation { get; }

        /// <summary> Called when the Off animation needs to be stopped </summary>
        protected abstract UnityAction stopOffAnimation { get; }

        /// <summary>
        /// Called to add ResetToOnState callback to the Off animation when it's stopped.
        /// This is used when playing the Off animation in reverse.
        /// </summary>
        protected abstract UnityAction addResetToOnStateCallback { get; }

        /// <summary>
        /// Called to remove the ResetToOnState callback from the Off animation when it's stopped.
        /// This is used when the Off animation, that was played in reverse, is stopped.
        /// </summary>
        protected abstract UnityAction removeResetToOnStateCallback { get; }

        /// <summary>
        /// Called to add ResetToOffState callback to the On animation when it's stopped.
        /// This is used when playing the On animation in reverse.
        /// </summary>
        protected abstract UnityAction addResetToOffStateCallback { get; }

        /// <summary>
        /// Called to remove the ResetToOffState callback from the On animation when it's stopped.
        /// This is used when the On animation, that was played in reverse, is stopped.
        /// </summary>
        protected abstract UnityAction removeResetToOffStateCallback { get; }


        /// <summary> Connect to Controller </summary>
        protected override void ConnectToController()
        {
            if (controller == null) return;
            controller.onToggleValueChangedCallback -= OnValueChanged;
            controller.onToggleValueChangedCallback += OnValueChanged;
            OnValueChanged(new ToggleValueChangedEvent(controller.isOn, controller.isOn, false));
        }

        /// <summary> Disconnect from Controller </summary>
        protected override void DisconnectFromController()
        {
            if (controller == null) return;
            controller.onToggleValueChangedCallback -= OnValueChanged;
        }

        /// <summary> Called when the value of the toggle changes </summary>
        /// <param name="evt"> Event data </param>
        protected virtual void OnValueChanged(ToggleValueChangedEvent evt)
        {
            if (controller == null) return;

            if (initializeCoroutine != null)
            {
                StopCoroutine(initializeCoroutine);
                initializeCoroutine = null;
            }

            if (!animatorInitialized)
            {
                initializeCoroutine = StartCoroutine(InitializeAfterAnimatorInitialized());
                return;
            }

            if (evt.newValue == evt.previousValue & !controller.inToggleGroup) //the value didn't change and the controller is not in a toggle group
            {
                switch (evt.newValue)
                {
                    case true:
                        InstantPlayOnAnimation();
                        return;
                    case false:
                        InstantPlayOffAnimation();
                        return;
                }
            }

            switch (evt.newValue)
            {
                case true:
                    if (offAnimationIsActive)
                    {
                        ReverseOffAnimation();
                        return;
                    }

                    switch (evt.animateChange)
                    {
                        case true:
                            PlayOnAnimation();
                            break;
                        default:
                            InstantPlayOnAnimation();
                            break;
                    }
                    return;
                case false:
                    if (onAnimationIsActive)
                    {
                        ReverseOnAnimation();
                        return;
                    }

                    switch (evt.animateChange)
                    {
                        case true:
                            PlayOffAnimation();
                            break;
                        default:
                            InstantPlayOffAnimation();
                            break;
                    }
                    return;
            }
        }

        /// <summary> Update the animation state to match the current value of the toggle </summary>
        private IEnumerator InitializeAfterAnimatorInitialized()
        {
            yield return new WaitUntil(() => animatorInitialized);
            yield return new WaitForEndOfFrame();
            OnValueChanged(new ToggleValueChangedEvent(controller.isOn, controller.isOn, false));
            initializeCoroutine = null;
        }

        /// <summary> Play the animation for the On state when the toggle value changes to true </summary>
        public virtual void PlayOnAnimation()
        {
            if (!animatorInitialized) return;
            if (offAnimationIsActive) stopOffAnimation?.Invoke();
            playOnAnimation?.Invoke();
        }

        /// <summary> Play the animation for the Off state when the toggle value changes to false </summary>
        public virtual void PlayOffAnimation()
        {
            if (!animatorInitialized) return;
            if (onAnimationIsActive) stopOnAnimation?.Invoke();
            playOffAnimation?.Invoke();
        }

        /// <summary> Set the On state when the toggle value changes to true, by setting the animation's progress to 1 </summary>
        public virtual void InstantPlayOnAnimation()
        {
            if (!animatorInitialized) return;
            if (offAnimationIsActive) stopOffAnimation?.Invoke();
            instantPlayOnAnimation?.Invoke();
        }

        /// <summary> Set the Off state when the toggle value changes to false, by setting the animation's progress to 0 </summary>
        public virtual void InstantPlayOffAnimation()
        {
            if (!animatorInitialized) return;
            if (onAnimationIsActive) stopOnAnimation?.Invoke();
            instantPlayOffAnimation?.Invoke();
        }

        /// <summary> Reverse the animation for the On state when the toggle value changes to false (if the animation is playing) </summary>
        public virtual void ReverseOnAnimation()
        {
            if (!animatorInitialized) return;
            if (offAnimationIsActive) StopOffAnimation();
            addResetToOffStateCallback?.Invoke();
            reverseOnAnimation?.Invoke();
        }

        /// <summary> Reverse the animation for the Off state when the toggle value changes to true (if the animation is playing) </summary>
        public virtual void ReverseOffAnimation()
        {
            if (!animatorInitialized) return;
            if (onAnimationIsActive) StopOnAnimation();
            addResetToOnStateCallback?.Invoke();
            reverseOffAnimation?.Invoke();
        }

        /// <summary> Stop the On animation if it is playing </summary>
        public virtual void StopOnAnimation()
        {
            if (!animatorInitialized) return;
            stopOnAnimation?.Invoke();
        }

        /// <summary> Stop the Off animation if it is playing </summary>
        public virtual void StopOffAnimation()
        {
            if (!animatorInitialized) return;
            stopOffAnimation?.Invoke();
        }

        /// <summary> Reset the On state when the toggle value changes to false, by setting the animation's progress to 0 </summary>
        public virtual void ResetToOnState()
        {
            removeResetToOnStateCallback?.Invoke();
            if (controller.isOn) instantPlayOnAnimation?.Invoke();
        }

        /// <summary> Reset the Off state when the toggle value changes to true, by setting the animation's progress to 1 </summary>
        public virtual void ResetToOffState()
        {
            removeResetToOffStateCallback?.Invoke();
            if (!controller.isOn) instantPlayOffAnimation?.Invoke();
        }
    }
}
