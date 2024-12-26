// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Ticker;
using Doozy.Runtime.UIManager.Containers;
using Doozy.Runtime.UIManager.Layouts;
using Doozy.Runtime.UIManager.Utils;
using UnityEngine;
using UnityEngine.UI;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.UIManager.Animators
{
    /// <summary>
    /// Specialized animator component used to animate a RectTransform’s position, rotation, scale and alpha
    /// by listening to a target UIContainer (controller) show/hide commands.
    /// </summary>
    [AddComponentMenu("UI/Containers/Animators/UIContainer UIAnimator")]
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(RectTransform))]
    public class UIContainerUIAnimator : BaseUIContainerAnimator
    {
        private CanvasGroup m_CanvasGroup;
        /// <summary> Reference to the CanvasGroup component </summary>
        public CanvasGroup canvasGroup => m_CanvasGroup ? m_CanvasGroup : m_CanvasGroup = GetComponent<CanvasGroup>();

        [SerializeField] private UIAnimation ShowAnimation;
        /// <summary> Container Show Animation </summary>
        public UIAnimation showAnimation => ShowAnimation ?? (ShowAnimation = new UIAnimation(rectTransform));

        [SerializeField] private UIAnimation HideAnimation;
        /// <summary> Container Hide Animation </summary>
        public UIAnimation hideAnimation => HideAnimation ?? (HideAnimation = new UIAnimation(rectTransform));

        public bool anyAnimationIsActive => showAnimation.isActive || hideAnimation.isActive;
        private bool isInLayoutGroup { get; set; }
        private Vector3 localPosition { get; set; }
        private UIBehaviourHandler uiBehaviourHandler { get; set; }
        private bool updateStartPositionInLateUpdate { get; set; }

        #if UNITY_EDITOR
        protected override void Reset()
        {
            ResetAnimation(showAnimation);
            ResetAnimation(hideAnimation);

            showAnimation.animationType = UIAnimationType.Show;
            showAnimation.Move.enabled = true;
            showAnimation.Move.fromDirection = MoveDirection.Left;

            hideAnimation.animationType = UIAnimationType.Hide;
            hideAnimation.Move.enabled = true;
            hideAnimation.Move.toDirection = MoveDirection.Right;

            base.Reset();
        }
        #endif

        protected override void Awake()
        {
            if (!Application.isPlaying) return;
            animatorInitialized = false;
            m_RectTransform = GetComponent<RectTransform>();
            m_CanvasGroup = GetComponent<CanvasGroup>();
            UpdateLayout();
        }

        protected override void OnEnable()
        {
            if (!Application.isPlaying) return;
            base.OnEnable();
            updateStartPositionInLateUpdate = true;
        }

        protected override void OnDisable()
        {
            if (!Application.isPlaying) return;
            base.OnDisable();
            if (showAnimation.isPlaying) showAnimation.SetProgressAtOne();
            if (hideAnimation.isPlaying) hideAnimation.SetProgressAtOne();
            RefreshLayout();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ShowAnimation?.Recycle();
            HideAnimation?.Recycle();
        }

        private void LateUpdate()
        {
            if (!animatorInitialized) return;
            if (!isInLayoutGroup) return;
            if (!isConnected) return;
            if (controller.visibilityState != VisibilityState.Visible) return;
            if (anyAnimationIsActive) return;
            if (!updateStartPositionInLateUpdate && localPosition == rectTransform.localPosition) return;
            if (CanvasUpdateRegistry.IsRebuildingLayout()) return;
            RefreshLayout();
            UpdateStartPosition();
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

        /// <summary> Refresh the layout the container is in </summary>
        private void RefreshLayout()
        {
            if (uiBehaviourHandler == null) return;
            uiBehaviourHandler.RefreshLayout();
        }

        /// <summary> Update the start position of the container </summary>
        public void UpdateStartPosition()
        {
            // if (name.Contains("#")) Debug.Log($"({Time.frameCount}) [{name}] {nameof(UpdateStartPosition)}");
            Vector3 anchoredPosition3D = rectTransform.anchoredPosition3D;
            showAnimation.startPosition = anchoredPosition3D;
            hideAnimation.startPosition = anchoredPosition3D;
            if (showAnimation.Move.isPlaying) showAnimation.Move.UpdateValues();
            if (hideAnimation.Move.isPlaying) hideAnimation.Move.UpdateValues();
            localPosition = rectTransform.localPosition;
            updateStartPositionInLateUpdate = false;
        }

        /// <summary> Refresh the layout and then the start position of the container </summary>
        private void RefreshStartPosition()
        {
            if (!isConnected) return;
            if (anyAnimationIsActive) return;
            if (controller.visibilityState != VisibilityState.Visible) return;
            RefreshLayout();
            UpdateStartPosition();
        }

        /// <summary> Connect to Controller </summary>
        protected override void ConnectToController()
        {
            base.ConnectToController();
            if (!controller) return;

            controller.showReactions.Add(showAnimation.Move);
            controller.showReactions.Add(showAnimation.Rotate);
            controller.showReactions.Add(showAnimation.Scale);
            controller.showReactions.Add(showAnimation.Fade);

            controller.hideReactions.Add(hideAnimation.Move);
            controller.hideReactions.Add(hideAnimation.Rotate);
            controller.hideReactions.Add(hideAnimation.Scale);
            controller.hideReactions.Add(hideAnimation.Fade);
        }

        /// <summary> Disconnect from Controller </summary>
        protected override void DisconnectFromController()
        {
            base.DisconnectFromController();
            if (!controller) return;

            controller.showReactions.Remove(showAnimation.Move);
            controller.showReactions.Remove(showAnimation.Rotate);
            controller.showReactions.Remove(showAnimation.Scale);
            controller.showReactions.Remove(showAnimation.Fade);

            controller.hideReactions.Remove(hideAnimation.Move);
            controller.hideReactions.Remove(hideAnimation.Rotate);
            controller.hideReactions.Remove(hideAnimation.Scale);
            controller.hideReactions.Remove(hideAnimation.Fade);
        }

        /// <summary> Play the show animation </summary>
        public override void Show()
        {
            showAnimation.Play(PlayDirection.Forward);
            if (animatorInitialized && isInLayoutGroup) updateStartPositionInLateUpdate = true;
        }

        /// <summary> Reverse the show animation (if playing) </summary>
        public override void ReverseShow()
        {
            if (showAnimation.isPlaying)
            {
                showAnimation.OnFinishCallback.AddListener(OnReverseShowComplete);
                void OnReverseShowComplete()
                {
                    InstantHide();
                    showAnimation.OnFinishCallback.RemoveListener(OnReverseShowComplete);
                }
                showAnimation.Reverse();
                //Hide();
                return;
            }
            Hide();
        }

        /// <summary> Play the hide animation </summary>
        public override void Hide()
        {
            if (animatorInitialized && isInLayoutGroup) RefreshStartPosition();
            hideAnimation.Play(PlayDirection.Forward);
        }

        /// <summary> Reverse the hide animation (if playing) </summary>
        public override void ReverseHide()
        {
            if(hideAnimation.isPlaying)
            {
                hideAnimation.OnFinishCallback.AddListener(OnReverseHideComplete);
                void OnReverseHideComplete()
                {
                    InstantShow();
                    updateStartPositionInLateUpdate = true;
                    hideAnimation.OnFinishCallback.RemoveListener(OnReverseHideComplete);
                }
                hideAnimation.Reverse();
                //Show();
                return;
            }
            Show();
        }

        /// <summary> Set show animation's progress at one </summary>
        public override void InstantShow()
        {
            showAnimation.SetProgressAtOne();
            if (animatorInitialized && isInLayoutGroup) updateStartPositionInLateUpdate = true;
        }

        /// <summary> Set hide animation's progress at one </summary>
        public override void InstantHide()
        {
            if (animatorInitialized && isInLayoutGroup) RefreshStartPosition();
            hideAnimation.SetProgressAtOne();
        }

        /// <summary> Update the animations settings (if a colorTarget is referenced) </summary>
        public override void UpdateSettings()
        {
            showAnimation.SetTarget(rectTransform, canvasGroup);
            hideAnimation.SetTarget(rectTransform, canvasGroup);
        }

        /// <summary> Stop all animations </summary>
        public override void StopAllReactions()
        {
            showAnimation.Stop();
            hideAnimation.Stop();
        }

        /// <summary> Reset all the reactions to their initial values (if the animation is enabled) </summary>
        /// <param name="forced"> If true, forced will ignore if the animation is enabled or not </param>
        public override void ResetToStartValues(bool forced = false)
        {
            if (m_RectTransform == null) return;

            if (showAnimation.isActive) showAnimation.Stop();
            if (hideAnimation.isActive) hideAnimation.Stop();

            showAnimation.ResetToStartValues(forced);
            hideAnimation.ResetToStartValues(forced);

            rectTransform.anchoredPosition3D = showAnimation.startPosition;
            rectTransform.localEulerAngles = showAnimation.startRotation;
            rectTransform.localScale = showAnimation.startScale;
            canvasGroup.alpha = showAnimation.startAlpha;

            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(rectTransform);
            UnityEditor.SceneView.RepaintAll();
            #endif
        }

        /// <summary> Set animation heartbeat </summary>
        public override List<Heartbeat> SetHeartbeat<T>()
        {
            var list = new List<Heartbeat>();
            for (int i = 0; i < 8; i++) list.Add(new T());

            showAnimation.Move.SetHeartbeat(list[0]);
            showAnimation.Rotate.SetHeartbeat(list[1]);
            showAnimation.Scale.SetHeartbeat(list[2]);
            showAnimation.Fade.SetHeartbeat(list[3]);

            hideAnimation.Move.SetHeartbeat(list[4]);
            hideAnimation.Rotate.SetHeartbeat(list[5]);
            hideAnimation.Scale.SetHeartbeat(list[6]);
            hideAnimation.Fade.SetHeartbeat(list[7]);

            return list;
        }

        /// <summary> Set a new start position value (RectTransform.anchoredPosition3D) for all animations </summary>
        /// <param name="value"> New start position </param>
        public void SetStartPosition(Vector3 value)
        {
            showAnimation.startPosition = value;
            hideAnimation.startPosition = value;
        }

        /// <summary> Set a new start rotation value (RectTransform.localEulerAngles) for both show and hide animations </summary>
        /// <param name="value"> New start rotation </param>
        public void SetStartRotation(Vector3 value)
        {
            showAnimation.startRotation = value;
            hideAnimation.startRotation = value;
        }

        /// <summary> Set a new start scale value (RectTransform.localScale) for both show and hide animations </summary>
        /// <param name="value"> New start scale </param>
        public void SetStartScale(Vector3 value)
        {
            showAnimation.startScale = value;
            hideAnimation.startScale = value;
        }

        /// <summary> Set a new start alpha value (CanvasGroup.alpha) for both show and hide animations </summary>
        /// <param name="value"> New start scale </param>
        public void SetStartAlpha(float value)
        {
            showAnimation.startAlpha = value;
            hideAnimation.startAlpha = value;
        }

        private static void ResetAnimation(UIAnimation target)
        {
            target.Move.Reset();
            target.Rotate.Reset();
            target.Scale.Reset();
            target.Fade.Reset();

            target.Move.fromReferenceValue = ReferenceValue.StartValue;
            target.Rotate.fromReferenceValue = ReferenceValue.StartValue;
            target.Scale.fromReferenceValue = ReferenceValue.StartValue;
            target.Fade.fromReferenceValue = ReferenceValue.StartValue;

            target.Move.settings.duration = UIContainer.k_DefaultAnimationDuration;
            target.Rotate.settings.duration = UIContainer.k_DefaultAnimationDuration;
            target.Scale.settings.duration = UIContainer.k_DefaultAnimationDuration;
            target.Fade.settings.duration = UIContainer.k_DefaultAnimationDuration;
        }
    }
}
