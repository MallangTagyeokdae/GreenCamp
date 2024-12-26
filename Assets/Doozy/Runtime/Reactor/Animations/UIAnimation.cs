// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Animations
{
    /// <summary>
    /// Reactor animation that works with a RectTransform.
    /// It works as a combined animation for position, rotation, scale and alpha (transparency).
    /// </summary>
    [Serializable]
    public class UIAnimation : ReactorAnimation
    {
        /// <summary>
        /// Target RectTransform for the animation
        /// (animates the position, rotation and scale via the Move, Rotate and Scale animations)
        /// </summary>
        public RectTransform rectTransform
        {
            get;
            internal set;
        }

        /// <summary>
        /// Target CanvasGroup for the animation
        /// (animates group alpha via the Fade animation)
        /// </summary>
        public CanvasGroup canvasGroup
        {
            get;
            internal set;
        }

        [SerializeField] private UIAnimationType AnimationType;
        /// <summary> Type of animation. Used to show specialized settings and to sava/load dedicated presets </summary>
        public UIAnimationType animationType
        {
            get => AnimationType;
            set
            {
                AnimationType = value;
                Move.animationType = value;
            }
        }

        /// <summary> Move reaction designed to change the position of a target RectTransform </summary>
        public UIMoveReaction Move;
        /// <summary> Rotate reaction designed to change the rotation of a target RectTransform </summary>
        public UIRotateReaction Rotate;
        /// <summary> Scale reaction designed to change the scale of a target RectTransform </summary>
        public UIScaleReaction Scale;
        /// <summary> Fade reaction designed to change the alpha value of a target CanvasGroup </summary>
        public UIFadeReaction Fade;

        /// <summary> Move start position value (RectTransform.anchoredPosition3D) </summary>
        public Vector3 startPosition
        {
            get => Move.startPosition;
            set => Move.startPosition = value;
        }

        /// <summary> Rotate start rotation value (RectTransform.localEulerAngles) </summary>
        public Vector3 startRotation
        {
            get => Rotate.startRotation;
            set => Rotate.startRotation = value;
        }

        /// <summary> Scale start scale value (RectTransform.localScale) </summary>
        public Vector3 startScale
        {
            get => Scale.startScale;
            set => Scale.startScale = value;
        }

        /// <summary> Fade start alpha value (CanvasGroup.alpha) </summary>
        public float startAlpha
        {
            get => Fade.startAlpha;
            set => Fade.startAlpha = value;
        }

        /// <summary> Is the animation enabled </summary>
        public override bool isEnabled => Move.enabled | Rotate.enabled | Scale.enabled | Fade.enabled;
        /// <summary> Is the animation in the idle state (is enabled, but not active) </summary>
        public override bool isIdle => Move.isIdle | Rotate.isIdle | Scale.isIdle | Fade.isIdle;
        /// <summary> Is the animation in the active state (is enabled and either playing or paused) </summary>
        public override bool isActive => Move.isActive | Rotate.isActive | Scale.isActive | Fade.isActive;
        /// <summary> Is the animation in the paused state (is enabled, started playing and then paused) </summary>
        public override bool isPaused => Move.isPaused | Rotate.isPaused | Scale.isPaused | Fade.isPaused;
        /// <summary> Is the animation in the playing state (is enabled and started playing) </summary>
        public override bool isPlaying => Move.isPlaying || Rotate.isPlaying || Scale.isPlaying || Fade.isPlaying;
        /// <summary> Is the animation in the start delay state (is enabled, started playing and waiting to start running after the start delay duration has passed) </summary>
        public override bool inStartDelay => Move.inStartDelay | Rotate.inStartDelay | Scale.inStartDelay | Fade.inStartDelay;
        /// <summary> Is the animation in the loop delay state (is enabled, started playing and is between loops waiting to continue running after the loop delay duration has passed) </summary>
        public override bool inLoopDelay => Move.inLoopDelay | Rotate.inLoopDelay | Scale.inLoopDelay | Fade.inLoopDelay;

        /// <summary> Construct a new UIAnimation with the given RectTransform target and CanvasGroup target </summary>
        /// <param name="targetRectTransform"> Target RectTransform </param>
        /// <param name="targetCanvasGroup"> Target CanvasGroup </param>
        public UIAnimation(RectTransform targetRectTransform, CanvasGroup targetCanvasGroup = null) =>
            SetTarget(targetRectTransform, targetCanvasGroup);

        /// <summary>
        /// Set the RectTransform target and CanvasGroup target.
        /// <para/> If a CanvasGroup reference is not set, this method will try to get the reference by trying to get the component from the RectTransform.
        /// <para/> If the CanvasGroup component is not found, one will be added and used as a reference.
        /// </summary>
        /// <param name="targetRectTransform"> RectTransform target </param>
        /// <param name="targetCanvasGroup"> CanvasGroup target </param>
        public void SetTarget(RectTransform targetRectTransform, CanvasGroup targetCanvasGroup = null)
        {
            rectTransform = null;
            canvasGroup = null;

            _ = targetRectTransform ? targetRectTransform : throw new NullReferenceException(nameof(targetRectTransform));
            rectTransform = targetRectTransform;
            if (targetCanvasGroup == null) targetCanvasGroup = targetRectTransform.gameObject.GetComponent<CanvasGroup>();
            canvasGroup = targetCanvasGroup == null ? targetRectTransform.gameObject.AddComponent<CanvasGroup>() : targetCanvasGroup;

            Initialize();
        }

        /// <summary> Initialize the animation </summary>
        public void Initialize()
        {
            Move?.Stop(true);
            Move ??= Reaction.Get<UIMoveReaction>();
            Move.SetTarget(rectTransform);
            Move.animationType = animationType;

            Rotate?.Stop(true);
            Rotate ??= Reaction.Get<UIRotateReaction>();
            Rotate.SetTarget(rectTransform);

            Scale?.Stop(true);
            Scale ??= Reaction.Get<UIScaleReaction>();
            Scale.SetTarget(rectTransform);

            Fade?.Stop(true);
            Fade ??= Reaction.Get<UIFadeReaction>();
            Fade.SetTarget(rectTransform, canvasGroup);

            UpdateValues();
        }

        /// <summary>
        /// Recycle the reactions controlled by this animation.
        /// <para/> Reactions are pooled can (and should) be recycled to improve overall performance. 
        /// </summary>
        public override void Recycle()
        {
            Move?.Recycle();
            Rotate?.Recycle();
            Scale?.Recycle();
            Fade?.Recycle();
        }

        /// <summary>
        /// Update the initial values for the reactions controlled by this animation
        /// </summary>
        public override void UpdateValues()
        {
            if (canvasGroup != null)
                Fade.UpdateValues(); //calculate fade
            Scale.UpdateValues();    //calculate scale
            Rotate.UpdateValues();   // calculate rotation

            //update move settings after calculating scale
            {
                Move.UseCustomLocalScale = Scale.enabled;
                Move.CustomFromLocalScale = Scale.enabled ? Scale.fromValue : startScale;
                Move.CustomToLocalScale = Scale.enabled ? Scale.toValue : startScale;
            }

            //update move settings after rotation
            {
                Move.UseCustomLocalRotation = Rotate.enabled;
                Move.CustomFromLocalRotation = Rotate.enabled ? Rotate.fromValue : startRotation;
                Move.CustomToLocalRotation = Rotate.enabled ? Rotate.toValue : startRotation;
            }

            Move.animationType = animationType; //enforce animation type
            Move.UpdateValues();                //calculate position
        }

        /// <summary>
        /// Stop all the reactions controlled by this animation
        /// </summary>
        public override void StopAllReactionsOnTarget() =>
            Reaction.StopAllReactionsByTargetObject(rectTransform, true);

        /// <summary> Set the animation at the given progress value </summary>
        /// <param name="targetProgress"> Target progress [0,1] </param>
        public override void SetProgressAt(float targetProgress)
        {
            base.SetProgressAt(targetProgress);
            if (Fade.enabled) Fade.SetProgressAt(targetProgress);
            if (Scale.enabled) Scale.SetProgressAt(targetProgress);
            if (Rotate.enabled) Rotate.SetProgressAt(targetProgress);
            if (Move.enabled) Move.SetProgressAt(targetProgress);

            if (animationType != UIAnimationType.Custom)
                ResetToStartValues();
        }

        /// <summary> Play the animation at the given progress value from the current value </summary>
        /// <param name="toProgress"> To progress [0,1] </param>
        public override void PlayToProgress(float toProgress)
        {
            base.PlayToProgress(toProgress);
            if (Fade.enabled) Fade.PlayToProgress(toProgress);
            if (Scale.enabled) Scale.PlayToProgress(toProgress);
            if (Rotate.enabled) Rotate.PlayToProgress(toProgress);
            if (Move.enabled) Move.PlayToProgress(toProgress);

            if (animationType != UIAnimationType.Custom)
                ResetToStartValues();
        }

        /// <summary> Play the animation from the given progress value to the current value </summary>
        /// <param name="fromProgress"> From progress [0,1] </param>
        public override void PlayFromProgress(float fromProgress)
        {
            base.PlayFromProgress(fromProgress);
            if (Move.enabled) Move.PlayFromProgress(fromProgress);
            if (Rotate.enabled) Rotate.PlayFromProgress(fromProgress);
            if (Scale.enabled) Scale.PlayFromProgress(fromProgress);
            if (Fade.enabled) Fade.PlayFromProgress(fromProgress);

            if (animationType != UIAnimationType.Custom)
                ResetToStartValues();
        }

        /// <summary> Play the animation from the given progress value to the given progress value </summary>
        /// <param name="fromProgress"> From progress [0,1] </param>
        /// <param name="toProgress"> To progress [0,1] </param>
        public override void PlayFromToProgress(float fromProgress, float toProgress)
        {
            base.PlayFromToProgress(fromProgress, toProgress);
            if (Move.enabled) Move.PlayFromToProgress(fromProgress, toProgress);
            if (Rotate.enabled) Rotate.PlayFromToProgress(fromProgress, toProgress);
            if (Scale.enabled) Scale.PlayFromToProgress(fromProgress, toProgress);
            if (Fade.enabled) Fade.PlayFromToProgress(fromProgress, toProgress);

            if (animationType != UIAnimationType.Custom)
                ResetToStartValues();
        }

        /// <summary> Play the animation all the way </summary>
        /// <param name="inReverse"> Play the animation in reverse? </param>
        public override void Play(bool inReverse = false)
        {
            if (rectTransform == null)
                return;

            RegisterCallbacks();
            if (!isActive)
            {
                StopAllReactionsOnTarget();
                ResetToStartValues();
            }

            if (Move.enabled) Move.Play(inReverse);
            if (Rotate.enabled) Rotate.Play(inReverse);
            if (Scale.enabled) Scale.Play(inReverse);
            if (Fade.enabled) Fade.Play(inReverse);

            // if (!isPlaying && animationType != UIAnimationType.Custom)
            //     ResetToStartValues();
        }

        /// <summary> Reset all the reactions to their initial values (if the animation is enabled) </summary>
        /// <param name="forced"> If true, forced will ignore if the animation is enabled or not </param>
        public override void ResetToStartValues(bool forced = false)
        {
            if (rectTransform == null) return;
            if (forced || !Move.enabled) Move.SetValue(startPosition);
            if (forced || !Rotate.enabled) Rotate.SetValue(startRotation);
            if (forced || !Scale.enabled) Scale.SetValue(startScale);
            if (forced || !Fade.enabled) Fade.SetValue(startAlpha);
        }

        /// <summary>
        /// Stop the animation.
        /// Called every time the animation is stopped. Also called before calling Finish()
        /// </summary>
        public override void Stop()
        {
            if (Move.isActive || Move.enabled) Move.Stop();
            if (Rotate.isActive || Rotate.enabled) Rotate.Stop();
            if (Scale.isActive || Scale.enabled) Scale.Stop();
            if (Fade.isActive || Fade.enabled) Fade.Stop();
            base.Stop();
        }

        /// <summary>
        /// Finish the animation.
        /// Called to mark that that animation completed playing.
        /// </summary>
        public override void Finish()
        {
            if (Move.isActive || Move.enabled) Move.Finish();
            if (Rotate.isActive || Rotate.enabled) Rotate.Finish();
            if (Scale.isActive || Scale.enabled) Scale.Finish();
            if (Fade.isActive || Fade.enabled) Fade.Finish();
            base.Finish();
        }

        /// <summary>
        /// Reverse the animation's direction while playing.
        /// Works only if the animation is active (it either playing or paused)
        /// </summary>
        public override void Reverse()
        {
            if (Move.isActive) Move.Reverse();
            else if (Move.enabled) Move.Play(PlayDirection.Reverse);

            if (Rotate.isActive) Rotate.Reverse();
            else if (Rotate.enabled) Rotate.Play(PlayDirection.Reverse);

            if (Scale.isActive) Scale.Reverse();
            else if (Scale.enabled) Scale.Play(PlayDirection.Reverse);

            if (Fade.isActive) Fade.Reverse();
            else if (Fade.enabled) Fade.Play(PlayDirection.Reverse);
        }

        /// <summary>
        /// Rewind the animation to the start values
        /// </summary>
        public override void Rewind()
        {
            if (Move.enabled) Move.Rewind();
            if (Rotate.enabled) Rotate.Rewind();
            if (Scale.enabled) Scale.Rewind();
            if (Fade.enabled) Fade.Rewind();
        }

        /// <summary>
        /// Pause the animation.
        /// Works only if the animation is playing.
        /// </summary>
        public override void Pause()
        {
            Move.Pause();
            Rotate.Pause();
            Scale.Pause();
            Fade.Pause();
        }

        /// <summary>
        /// Resume a paused animation.
        /// Works only if the animation is paused.
        /// </summary>
        public override void Resume()
        {
            Move.Resume();
            Rotate.Resume();
            Scale.Resume();
            Fade.Resume();
        }

        public float GetStartDelay()
        {
            float move = Move.enabled ? Move.isActive ? Move.startDelay : Move.settings.GetStartDelay() : 0;
            float rotate = Rotate.enabled ? Rotate.isActive ? Rotate.startDelay : Rotate.settings.GetStartDelay() : 0;
            float scale = Scale.enabled ? Scale.isActive ? Scale.startDelay : Scale.settings.GetStartDelay() : 0;
            float fade = Fade.enabled ? Fade.isActive ? Fade.startDelay : Fade.settings.GetStartDelay() : 0;
            return move + rotate + scale + fade;
        }

        public float GetDuration()
        {
            float move = Move.enabled ? Move.isActive ? Move.duration : Move.settings.GetDuration() : 0;
            float rotate = Rotate.enabled ? Rotate.isActive ? Rotate.duration : Rotate.settings.GetDuration() : 0;
            float scale = Scale.enabled ? Scale.isActive ? Scale.duration : Scale.settings.GetDuration() : 0;
            float fade = Fade.enabled ? Fade.isActive ? Fade.duration : Fade.settings.GetDuration() : 0;
            return move + rotate + scale + fade;
        }

        public float GetTotalDuration() =>
            GetStartDelay() + GetDuration();

        /// <summary>
        /// Register all callbacks to the animation
        /// </summary>
        protected override void RegisterCallbacks()
        {
            base.RegisterCallbacks();

            if (Move.enabled)
            {
                startedReactionsCount++;
                Move.OnPlayCallback += InvokeOnPlay;
                Move.OnStopCallback += InvokeOnStop;
                Move.OnFinishCallback += InvokeOnFinish;
            }

            if (Rotate.enabled)
            {
                startedReactionsCount++;
                Rotate.OnPlayCallback += InvokeOnPlay;
                Rotate.OnStopCallback += InvokeOnStop;
                Rotate.OnFinishCallback += InvokeOnFinish;
            }

            if (Scale.enabled)
            {
                startedReactionsCount++;
                Scale.OnPlayCallback += InvokeOnPlay;
                Scale.OnStopCallback += InvokeOnStop;
                Scale.OnFinishCallback += InvokeOnFinish;
            }

            if (Fade.enabled)
            {
                startedReactionsCount++;
                Fade.OnPlayCallback += InvokeOnPlay;
                Fade.OnStopCallback += InvokeOnStop;
                Fade.OnFinishCallback += InvokeOnFinish;
            }
        }

        /// <summary> Unregister OnPlayCallback </summary>
        protected override void UnregisterOnPlayCallbacks()
        {
            if (Move.enabled) Move.OnPlayCallback -= InvokeOnPlay;
            if (Rotate.enabled) Rotate.OnPlayCallback -= InvokeOnPlay;
            if (Scale.enabled) Scale.OnPlayCallback -= InvokeOnPlay;
            if (Fade.enabled) Fade.OnPlayCallback -= InvokeOnPlay;
        }

        /// <summary> Unregister OnStopCallback </summary>
        protected override void UnregisterOnStopCallbacks()
        {
            if (Move.enabled) Move.OnStopCallback -= InvokeOnStop;
            if (Rotate.enabled) Rotate.OnStopCallback -= InvokeOnStop;
            if (Scale.enabled) Scale.OnStopCallback -= InvokeOnStop;
            if (Fade.enabled) Fade.OnStopCallback -= InvokeOnStop;
        }

        /// <summary> Unregister OnFinishCallback </summary>
        protected override void UnregisterOnFinishCallbacks()
        {
            if (Move.enabled) Move.OnFinishCallback -= InvokeOnFinish;
            if (Rotate.enabled) Rotate.OnFinishCallback -= InvokeOnFinish;
            if (Scale.enabled) Scale.OnFinishCallback -= InvokeOnFinish;
            if (Fade.enabled) Fade.OnFinishCallback -= InvokeOnFinish;
        }
    }
}
