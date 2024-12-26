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
using UnityEngine.Events;
using UnityEngine.UI;
namespace Doozy.Runtime.UIManager.Animators
{
    /// <summary>
    /// Specialized animator component used to animate a RectTransform’s position, rotation, scale and alpha
    /// by listening to a UIToggle (controller) onToggleValueChangedCallback.
    /// </summary>
    [AddComponentMenu("UI/Components/Animators/UIToggle UIAnimator")]
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(RectTransform))]
    public class UIToggleUIAnimator : BaseUIToggleAnimator
    {
        private CanvasGroup m_CanvasGroup;
        /// <summary> Reference to the CanvasGroup component </summary>
        public CanvasGroup canvasGroup => m_CanvasGroup ? m_CanvasGroup : m_CanvasGroup = GetComponent<CanvasGroup>();

        [SerializeField] private UIAnimation OnAnimation;
        /// <summary> Container Show Animation </summary>
        public UIAnimation onAnimation => OnAnimation ?? (OnAnimation = new UIAnimation(rectTransform));

        [SerializeField] private UIAnimation OffAnimation;
        /// <summary> Container Hide Animation </summary>
        public UIAnimation offAnimation => OffAnimation ?? (OffAnimation = new UIAnimation(rectTransform));

        public bool anyAnimationIsActive => onAnimation.isActive || offAnimation.isActive;
        private bool isInLayoutGroup { get; set; }
        private Vector3 localPosition { get; set; }
        private UIBehaviourHandler uiBehaviourHandler { get; set; }
        private bool updateStartPositionInLateUpdate { get; set; }

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
        protected override UnityAction addResetToOnStateCallback => () => offAnimation.OnFinishCallback.AddListener(ResetToOnState);
        protected override UnityAction removeResetToOnStateCallback => () => offAnimation.OnFinishCallback.RemoveListener(ResetToOnState);
        protected override UnityAction addResetToOffStateCallback => () => onAnimation.OnFinishCallback.AddListener(ResetToOffState);
        protected override UnityAction removeResetToOffStateCallback => () => onAnimation.OnFinishCallback.RemoveListener(ResetToOffState);
        
        #if UNITY_EDITOR
        protected override void Reset()
        {
            ResetAnimation(onAnimation);
            ResetAnimation(offAnimation);

            onAnimation.animationType = UIAnimationType.Show;
            onAnimation.Move.enabled = true;
            onAnimation.Move.fromDirection = MoveDirection.Left;

            offAnimation.animationType = UIAnimationType.Hide;
            offAnimation.Move.enabled = true;
            offAnimation.Move.toDirection = MoveDirection.Right;

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
            if (onAnimation.isPlaying) onAnimation.SetProgressAtOne();
            if (offAnimation.isPlaying) offAnimation.SetProgressAtOne();
            RefreshLayout();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnAnimation?.Recycle();
            OffAnimation?.Recycle();
        }

        private void LateUpdate()
        {
            if (!animatorInitialized) return;
            if (!isInLayoutGroup) return;
            if (!isConnected) return;
            if (!controller.isActiveAndEnabled) return;
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
            onAnimation.startPosition = anchoredPosition3D;
            offAnimation.startPosition = anchoredPosition3D;
            if (onAnimation.Move.isPlaying) onAnimation.Move.UpdateValues();
            if (offAnimation.Move.isPlaying) offAnimation.Move.UpdateValues();
            localPosition = rectTransform.localPosition;
            updateStartPositionInLateUpdate = false;
        }

        /// <summary> Refresh the layout and then the start position of the container </summary>
        private void RefreshStartPosition()
        {
            if (!isConnected) return;
            if (anyAnimationIsActive) return;
            if (!controller.isActiveAndEnabled) return;
            RefreshLayout();
            UpdateStartPosition();
        }
        
         /// <summary> Update the animations settings (if a colorTarget is referenced) </summary>
        public override void UpdateSettings()
        {
            onAnimation.SetTarget(rectTransform, canvasGroup);
            offAnimation.SetTarget(rectTransform, canvasGroup);
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
            if (m_RectTransform == null) return;

            if (onAnimation.isActive) onAnimation.Stop();
            if (offAnimation.isActive) offAnimation.Stop();

            onAnimation.ResetToStartValues(forced);
            offAnimation.ResetToStartValues(forced);

            rectTransform.anchoredPosition3D = onAnimation.startPosition;
            rectTransform.localEulerAngles = onAnimation.startRotation;
            rectTransform.localScale = onAnimation.startScale;
            canvasGroup.alpha = onAnimation.startAlpha;

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

            onAnimation.Move.SetHeartbeat(list[0]);
            onAnimation.Rotate.SetHeartbeat(list[1]);
            onAnimation.Scale.SetHeartbeat(list[2]);
            onAnimation.Fade.SetHeartbeat(list[3]);

            offAnimation.Move.SetHeartbeat(list[4]);
            offAnimation.Rotate.SetHeartbeat(list[5]);
            offAnimation.Scale.SetHeartbeat(list[6]);
            offAnimation.Fade.SetHeartbeat(list[7]);

            return list;
        }

        /// <summary> Set a new start position value (RectTransform.anchoredPosition3D) for all animations </summary>
        /// <param name="value"> New start position </param>
        public void SetStartPosition(Vector3 value)
        {
            onAnimation.startPosition = value;
            offAnimation.startPosition = value;
        }

        /// <summary> Set a new start rotation value (RectTransform.localEulerAngles) for both show and hide animations </summary>
        /// <param name="value"> New start rotation </param>
        public void SetStartRotation(Vector3 value)
        {
            onAnimation.startRotation = value;
            offAnimation.startRotation = value;
        }

        /// <summary> Set a new start scale value (RectTransform.localScale) for both show and hide animations </summary>
        /// <param name="value"> New start scale </param>
        public void SetStartScale(Vector3 value)
        {
            onAnimation.startScale = value;
            offAnimation.startScale = value;
        }

        /// <summary> Set a new start alpha value (CanvasGroup.alpha) for both show and hide animations </summary>
        /// <param name="value"> New start scale </param>
        public void SetStartAlpha(float value)
        {
            onAnimation.startAlpha = value;
            offAnimation.startAlpha = value;
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
