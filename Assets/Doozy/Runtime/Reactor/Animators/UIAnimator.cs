// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Animators.Internal;
using Doozy.Runtime.Reactor.Ticker;
using Doozy.Runtime.UIManager.Layouts;
using Doozy.Runtime.UIManager.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Runtime.Reactor.Animators
{
    /// <summary>
    /// Specialized animator component used to animate a RectTransform’s position, rotation, scale and alpha.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("Reactor/Animators/UI Animator")]
    public class UIAnimator : ReactorAnimator
    {
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/Reactor/Animators/UI Animator", false, 6)]
        private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        {
            GameObjectUtils.AddToScene<UIAnimator>(false, true);
        }
        #endif

        private CanvasGroup m_CanvasGroup;
        /// <summary> Reference to the CanvasGroup component </summary>
        public CanvasGroup canvasGroup => m_CanvasGroup ? m_CanvasGroup : m_CanvasGroup = GetComponent<CanvasGroup>();

        private RectTransform m_RectTransform;
        /// <summary> Reference to the RectTransform component </summary>
        public RectTransform rectTransform => m_RectTransform ? m_RectTransform : m_RectTransform = GetComponent<RectTransform>();

        [SerializeField] private UIAnimation Animation;
        /// <summary> UI Animation </summary>
        public new UIAnimation animation => Animation ??= new UIAnimation(rectTransform, canvasGroup);

        private bool isInLayoutGroup { get; set; }
        private Vector3 localPosition { get; set; }
        private UIBehaviourHandler uiBehaviourHandler { get; set; }
        private bool updateStartPositionInLateUpdate { get; set; }
        private float lastMoveAnimationProgress { get; set; }

        #if UNITY_EDITOR
        private void Reset()
        {
            Animation ??= new UIAnimation(rectTransform, canvasGroup);
            ResetAnimation();
        }
        #endif

        protected override void Awake()
        {
            if (!Application.isPlaying) return;
            animatorInitialized = false;
            m_CanvasGroup = GetComponent<CanvasGroup>();
            m_RectTransform = GetComponent<RectTransform>();
            UpdateLayout();
        }

        protected override void OnEnable()
        {
            if (!Application.isPlaying) return;
            base.OnEnable();
            UpdateLayout();
            updateStartPositionInLateUpdate = true;
        }

        private void OnDisable()
        {
            RefreshLayout();
        }

        private void OnRectTransformDimensionsChange()
        {
            if (!animatorInitialized) return;
            if (!isInLayoutGroup) return;
            updateStartPositionInLateUpdate = true;
        }

        private void LateUpdate()
        {
            if (!animatorInitialized) return;
            if (!isInLayoutGroup) return;
            if (animation.isActive)
            {
                lastMoveAnimationProgress = animation.Move.progress;
                return;
            }
            if (localPosition != rectTransform.localPosition) updateStartPositionInLateUpdate = true;
            if (!updateStartPositionInLateUpdate) return;
            if (CanvasUpdateRegistry.IsRebuildingLayout()) return;
            UpdateStartPosition();
            RefreshLayout();
        }

        private void UpdateLayout()
        {
            isInLayoutGroup = rectTransform.IsInLayoutGroup();
            uiBehaviourHandler = null;
            if (!isInLayoutGroup) return;
            LayoutGroup layout = rectTransform.GetLayoutGroupInParent();
            if (layout == null) return;
            uiBehaviourHandler = layout.GetUIBehaviourHandler();
            System.Diagnostics.Debug.Assert(uiBehaviourHandler != null, nameof(uiBehaviourHandler) + " != null");
            uiBehaviourHandler.SetDirty();
        }

        private void RefreshLayout()
        {
            if (uiBehaviourHandler == null) return;
            uiBehaviourHandler.RefreshLayout();
        }

        /// <summary>
        /// Update the StartPosition from the current value of rectTransform.anchoredPosition3D
        /// </summary>
        public void UpdateStartPosition()
        {
            // if (name.Contains("#")) Debug.Log($"({Time.frameCount}) [{name}] {nameof(UpdateStartPosition)} sp:({animation.startPosition}) rp:({rectTransform.anchoredPosition}) lp:({rectTransform.localPosition})");
            animation.startPosition = rectTransform.anchoredPosition3D;
            if (animation.isPlaying) animation.UpdateValues();
            localPosition = rectTransform.localPosition;
            updateStartPositionInLateUpdate = false;
        }

        /// <summary> Play the animation all the way in the given direction </summary>
        /// <param name="playDirection"> Play direction (Forward or Reverse) </param>
        public override void Play(PlayDirection playDirection)
        {
            if (!animatorInitialized)
            {
                DelayExecution(() => animation.Play(playDirection));
                return;
            }
            animation.Play(playDirection);
        }

        /// <summary> Play the animation all the way </summary>
        /// <param name="inReverse"> Play the animation in reverse? </param>
        public override void Play(bool inReverse = false)
        {
            if (!animatorInitialized)
            {
                DelayExecution(() => animation.Play(inReverse));
                return;
            }
            animation.Play(inReverse);
        }

        /// <summary> Set the animator target </summary>
        /// <param name="target"> RectTransform target </param>
        public override void SetTarget(object target) =>
            SetTarget(target as RectTransform);

        /// <summary> Set the animator target </summary>
        /// <param name="targetRectTransform"> RectTransform target </param>
        /// <param name="targetCanvasGroup"> CanvasGroup target </param>
        public void SetTarget(RectTransform targetRectTransform, CanvasGroup targetCanvasGroup = null) =>
            animation.SetTarget(targetRectTransform, targetCanvasGroup);

        /// <summary> Reset all the reactions to their initial values (if the animation is enabled) </summary>
        /// <param name="forced"> If true, forced will ignore if the animation is enabled or not </param>
        public override void ResetToStartValues(bool forced = false)
        {
            if (animation.isActive) Stop();
            animation.ResetToStartValues(forced);

            if (this == null) return;

            rectTransform.anchoredPosition3D = animation.startPosition;
            rectTransform.localEulerAngles = animation.startRotation;
            rectTransform.localScale = animation.startScale;
            canvasGroup.alpha = animation.startAlpha;

            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(rectTransform);
            UnityEditor.SceneView.RepaintAll();
            #endif
        }

        /// <summary> Refresh the set target and, if the animation is playing, update the calculated values </summary>
        public override void UpdateSettings()
        {
            SetTarget(rectTransform, canvasGroup);
            if (animation.isPlaying) UpdateValues();
        }

        /// <summary> Get the animation start delay. For random values it returns the max value. </summary>
        public override float GetStartDelay() =>
            animation.GetStartDelay();

        /// <summary> Get the animation duration. For random values it returns the max value. </summary>
        public override float GetDuration() =>
            animation.GetDuration();

        /// <summary> Get the animation start delay + duration. For random values it returns the max value. </summary>
        public override float GetTotalDuration() =>
            GetStartDelay() + GetDuration();

        /// <summary> Set animation heartbeat </summary>
        public override List<Heartbeat> SetHeartbeat<T>()
        {
            var list = new List<Heartbeat>();
            for (int i = 0; i < 4; i++) list.Add(new T());

            animation.Move.SetHeartbeat(list[0]);
            animation.Rotate.SetHeartbeat(list[1]);
            animation.Scale.SetHeartbeat(list[2]);
            animation.Fade.SetHeartbeat(list[3]);

            return list;
        }

        /// <summary> Update From and To values by recalculating them </summary>
        public override void UpdateValues() =>
            animation.UpdateValues();

        /// <summary> Play the animation at the given progress value from the current value </summary>
        /// <param name="toProgress"> To progress [0,1] </param>
        public override void PlayToProgress(float toProgress)
        {
            if (!animatorInitialized)
            {
                DelayExecution(() => animation.PlayToProgress(toProgress));
                return;
            }
            animation.PlayToProgress(toProgress);
        }

        /// <summary> Play the animation from the given progress value to the current value </summary>
        /// <param name="fromProgress"> From progress [0,1] </param>
        public override void PlayFromProgress(float fromProgress)
        {
            if (!animatorInitialized)
            {
                DelayExecution(() => animation.PlayFromProgress(fromProgress));
                return;
            }
            animation.PlayFromProgress(fromProgress);
        }

        /// <summary> Play the animation from the given progress value to the given progress value </summary>
        /// <param name="fromProgress"> From progress [0,1] </param>
        /// <param name="toProgress"> To progress [0,1] </param>
        public override void PlayFromToProgress(float fromProgress, float toProgress)
        {
            if (!animatorInitialized)
            {
                DelayExecution(() => animation.PlayFromToProgress(fromProgress, toProgress));
                return;
            }
            animation.PlayFromToProgress(fromProgress, toProgress);
        }

        /// <summary>
        /// Stop the animation.
        /// Called every time the animation is stopped.
        /// Also called before calling Finish()
        /// </summary>
        public override void Stop()
        {
            if (!animatorInitialized)
            {
                DelayExecution(() => animation.Stop());
                return;
            }
            animation.Stop();
        }

        /// <summary>
        /// Finish the animation.
        /// Called to mark that that animation completed playing.
        /// </summary>
        public override void Finish()
        {
            if (!animatorInitialized)
            {
                DelayExecution(() => animation.Finish());
                return;
            }
            animation.Finish();
        }

        /// <summary>
        /// Reverse the animation's direction while playing.
        /// Works only if the animation is active (either playing or paused)
        /// </summary>
        public override void Reverse()
        {
            if (!animatorInitialized)
            {
                DelayExecution(() => animation.Reverse());
                return;
            }
            animation.Reverse();
        }

        /// <summary>
        /// Rewind the animation to the start values
        /// </summary>
        public override void Rewind()
        {
            if (!animatorInitialized)
            {
                DelayExecution(() => animation.Rewind());
                return;
            }
            animation.Rewind();
        }

        /// <summary>
        /// Pause the animation.
        /// Works only if the animation is playing.
        /// </summary>
        public override void Pause()
        {
            if (!animatorInitialized)
            {
                DelayExecution(() => animation.Pause());
                return;
            }
            animation.Pause();
        }

        /// <summary>
        /// Resume a paused animation.
        /// Works only if the animation is paused.
        /// </summary>
        public override void Resume()
        {
            if (!animatorInitialized)
            {
                DelayExecution(() => animation.Resume());
                return;
            }
            animation.Resume();
        }

        /// <summary> Set the animation at 100% (at the end, or the 'To' value) </summary>
        public override void SetProgressAtOne()
        {
            if (!animatorInitialized)
            {
                DelayExecution(() => animation.SetProgressAtOne());
                return;
            }
            animation.SetProgressAtOne();
        }

        /// <summary> Set the animation at 0% (at the start, or the 'From' value) </summary>
        public override void SetProgressAtZero()
        {
            if (!animatorInitialized)
            {
                DelayExecution(() => animation.SetProgressAtZero());
                return;
            }
            animation.SetProgressAtZero();
        }

        /// <summary> Set the animation at the given progress value </summary>
        /// <param name="targetProgress"> Target progress [0,1] </param>
        public override void SetProgressAt(float targetProgress)
        {
            if (!animatorInitialized)
            {
                DelayExecution(() => SetProgressAt(targetProgress));
                return;
            }
            animation.SetProgressAt(targetProgress);
        }

        /// <summary>
        /// Recycle the reactions controlled by this animation.
        /// <para/> Reactions are pooled can (and should) be recycled to improve overall performance. 
        /// </summary>
        protected override void Recycle() =>
            animation?.Recycle();

        /// <summary> Set a new start position value (RectTransform.anchoredPosition3D) for the animation </summary>
        /// <param name="value"> New start position </param>
        public void SetStartPosition(Vector3 value)
        {
            animation.startPosition = value;
        }

        /// <summary> Set a new start rotation value (RectTransform.localEulerAngles) for the animation </summary>
        /// <param name="value"> New start rotation </param>
        public void SetStartRotation(Vector3 value)
        {
            animation.startRotation = value;
        }

        /// <summary> Set a new start scale value (RectTransform.localScale) for the animation </summary>
        /// <param name="value"> New start scale </param>
        public void SetStartScale(Vector3 value)
        {
            animation.startScale = value;
        }

        /// <summary> Set a new start alpha value (CanvasGroup.alpha) for the animation </summary>
        /// <param name="value"> New start scale </param>
        public void SetStartAlpha(float value)
        {
            animation.startAlpha = value;
        }

        private void ResetAnimation()
        {
            animation.animationType = UIAnimationType.Show;
            animation.Move.Reset();
            animation.Move.enabled = true;
            animation.Rotate.Reset();
            animation.Scale.Reset();
            animation.Fade.Reset();
        }
    }
}
