// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Ticker;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Layouts;
using Doozy.Runtime.UIManager.Utils;
using UnityEngine;
using UnityEngine.UI;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.UIManager.Animators
{
    /// <summary>
    /// Specialized animator component used to animate a RectTransform’s position, rotation, scale and alpha
    /// by listening to a target UISelectable (controller) selection state changes.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("UI/Components/Animators/UISelectable UIAnimator")]
    public class UISelectableUIAnimator : BaseUISelectableAnimator
    {
        private CanvasGroup m_CanvasGroup;
        /// <summary> Reference to the CanvasGroup component </summary>
        public CanvasGroup canvasGroup => m_CanvasGroup ? m_CanvasGroup : m_CanvasGroup = GetComponent<CanvasGroup>();

        [SerializeField] private UIAnimation NormalAnimation;
        /// <summary> Animation for the Normal selection state </summary>
        public UIAnimation normalAnimation => NormalAnimation ?? (NormalAnimation = new UIAnimation(rectTransform));

        [SerializeField] private UIAnimation HighlightedAnimation;
        /// <summary> Animation for the Highlighted selection state </summary>
        public UIAnimation highlightedAnimation => HighlightedAnimation ?? (HighlightedAnimation = new UIAnimation(rectTransform));

        [SerializeField] private UIAnimation PressedAnimation;
        /// <summary> Animation for the Pressed selection state </summary>
        public UIAnimation pressedAnimation => PressedAnimation ?? (PressedAnimation = new UIAnimation(rectTransform));

        [SerializeField] private UIAnimation SelectedAnimation;
        /// <summary> Animation for the Selected selection state </summary>
        public UIAnimation selectedAnimation => SelectedAnimation ?? (SelectedAnimation = new UIAnimation(rectTransform));

        [SerializeField] private UIAnimation DisabledAnimation;
        /// <summary> Animation for the Disabled selection state </summary>
        public UIAnimation disabledAnimation => DisabledAnimation ?? (DisabledAnimation = new UIAnimation(rectTransform));

        /// <summary> Returns TRUE if any animation is active </summary>
        public bool anyAnimationIsActive =>
            normalAnimation.isActive ||
            highlightedAnimation.isActive ||
            pressedAnimation.isActive ||
            selectedAnimation.isActive ||
            disabledAnimation.isActive;

        private bool isInLayoutGroup { get; set; }
        private Vector3 localPosition { get; set; }
        private UIBehaviourHandler uiBehaviourHandler { get; set; }
        private bool updateStartPositionInLateUpdate { get; set; }
        private bool playStateAnimationFromLateUpdate { get; set; }

        /// <summary> Get the Animation triggered by the given selection state </summary>
        /// <param name="state"> Target selection state </param>
        public UIAnimation GetAnimation(UISelectionState state) =>
            state switch
            {
                UISelectionState.Normal      => normalAnimation,
                UISelectionState.Highlighted => highlightedAnimation,
                UISelectionState.Pressed     => pressedAnimation,
                UISelectionState.Selected    => selectedAnimation,
                UISelectionState.Disabled    => disabledAnimation,
                _                            => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };

        #if UNITY_EDITOR
        protected override void Reset()
        {
            foreach (UISelectionState state in UISelectable.uiSelectionStates)
                ResetAnimation(GetAnimation(state));

            NormalAnimation.animationType = UIAnimationType.Reset;
            NormalAnimation.Move.enabled = true;
            NormalAnimation.Rotate.enabled = true;
            NormalAnimation.Scale.enabled = true;
            NormalAnimation.Fade.enabled = true;

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
            playStateAnimationFromLateUpdate = true;
            base.OnEnable();
            UpdateLayout();
            updateStartPositionInLateUpdate = true;
        }

        protected override void OnDisable()
        {
            if (!Application.isPlaying) return;
            base.OnDisable();
            RefreshLayout();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            foreach (UISelectionState state in UISelectable.uiSelectionStates)
                GetAnimation(state)?.Recycle();
        }

        private void OnRectTransformDimensionsChange()
        {
            if (!isConnected) return;
            if (!isInLayoutGroup) return;
            updateStartPositionInLateUpdate = true;
        }

        private void LateUpdate()
        {
            if (!animatorInitialized) return;

            if (playStateAnimationFromLateUpdate)
            {
                if (isConnected)
                {
                    Play(controller.currentUISelectionState);
                    playStateAnimationFromLateUpdate = false;
                }
            }

            if (!isInLayoutGroup) return;
            if (!isConnected) return;
            if (anyAnimationIsActive) return;
            if (!updateStartPositionInLateUpdate && localPosition == rectTransform.localPosition) return;
            if (controller.currentUISelectionState != UISelectionState.Normal) return;
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

        /// <summary> Refresh the layout the selectable is in </summary>
        private void RefreshLayout()
        {
            if (uiBehaviourHandler == null) return;
            uiBehaviourHandler.RefreshLayout();
        }

        /// <summary> Update the start position of the selectable </summary>
        public void UpdateStartPosition()
        {
            foreach (UISelectionState state in UISelectable.uiSelectionStates)
            {
                UIAnimation uiAnimation = GetAnimation(state);
                uiAnimation.startPosition = rectTransform.anchoredPosition3D;
                if (uiAnimation.Move.isPlaying) uiAnimation.Move.UpdateValues();
            }
            localPosition = rectTransform.localPosition;
            updateStartPositionInLateUpdate = false;
        }

        /// <summary> Returns True if the givens selection state is enabled and the animation is not null. </summary>
        /// <param name="state"> Selection state </param>
        public override bool IsStateEnabled(UISelectionState state) =>
            GetAnimation(state).isEnabled;

        /// <summary> Update the animations settings </summary>
        public override void UpdateSettings()
        {
            foreach (UISelectionState state in UISelectable.uiSelectionStates)
                GetAnimation(state).SetTarget(rectTransform, canvasGroup);
        }

        /// <summary> Stop all animations </summary>
        public override void StopAllReactions()
        {
            foreach (UISelectionState state in UISelectable.uiSelectionStates)
                GetAnimation(state)?.Stop();
        }

        /// <summary> Reset all the reactions to their initial values (if the animation is enabled) </summary>
        /// <param name="forced"> If true, forced will ignore if the animation is enabled or not </param>
        public override void ResetToStartValues(bool forced = false)
        {
            if (normalAnimation.isActive) normalAnimation.Stop();
            if (highlightedAnimation.isActive) highlightedAnimation.Stop();
            if (pressedAnimation.isActive) pressedAnimation.Stop();
            if (selectedAnimation.isActive) selectedAnimation.Stop();
            if (disabledAnimation.isActive) disabledAnimation.Stop();

            normalAnimation.ResetToStartValues();
            highlightedAnimation.ResetToStartValues();
            pressedAnimation.ResetToStartValues();
            selectedAnimation.ResetToStartValues();
            disabledAnimation.ResetToStartValues();

            if (m_RectTransform == null) 
                return;

            rectTransform.anchoredPosition3D = normalAnimation.startPosition;
            rectTransform.localEulerAngles = normalAnimation.startRotation;
            rectTransform.localScale = normalAnimation.startScale;
            canvasGroup.alpha = normalAnimation.startAlpha;

            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(rectTransform);
            UnityEditor.SceneView.RepaintAll();
            #endif
        }

        /// <summary> Set animation heartbeat </summary>*
        public override List<Heartbeat> SetHeartbeat<T>()
        {
            var list = new List<Heartbeat>();
            for (int i = 0; i < 20; i++) list.Add(new T());

            normalAnimation.Move.SetHeartbeat(list[0]);
            normalAnimation.Rotate.SetHeartbeat(list[1]);
            normalAnimation.Scale.SetHeartbeat(list[2]);
            normalAnimation.Fade.SetHeartbeat(list[3]);

            highlightedAnimation.Move.SetHeartbeat(list[4]);
            highlightedAnimation.Rotate.SetHeartbeat(list[5]);
            highlightedAnimation.Scale.SetHeartbeat(list[6]);
            highlightedAnimation.Fade.SetHeartbeat(list[7]);

            pressedAnimation.Move.SetHeartbeat(list[8]);
            pressedAnimation.Rotate.SetHeartbeat(list[9]);
            pressedAnimation.Scale.SetHeartbeat(list[10]);
            pressedAnimation.Fade.SetHeartbeat(list[11]);

            selectedAnimation.Move.SetHeartbeat(list[12]);
            selectedAnimation.Rotate.SetHeartbeat(list[13]);
            selectedAnimation.Scale.SetHeartbeat(list[14]);
            selectedAnimation.Fade.SetHeartbeat(list[15]);

            disabledAnimation.Move.SetHeartbeat(list[16]);
            disabledAnimation.Rotate.SetHeartbeat(list[17]);
            disabledAnimation.Scale.SetHeartbeat(list[18]);
            disabledAnimation.Fade.SetHeartbeat(list[19]);

            return list;
        }

        /// <summary> Play the animation for the given selection state </summary>
        /// <param name="state"> Selection state </param>
        public override void Play(UISelectionState state)
        {
            if (playStateAnimationFromLateUpdate)
            {
                GetAnimation(state)?.SetProgressAtOne();
                return;
            }
            GetAnimation(state)?.Play();
        }

        /// <summary> Set a new start position value (RectTransform.anchoredPosition3D) for all state animations </summary>
        /// <param name="value"> New start position </param>
        public void SetStartPosition(Vector3 value)
        {
            normalAnimation.startPosition = value;
            highlightedAnimation.startPosition = value;
            pressedAnimation.startPosition = value;
            selectedAnimation.startPosition = value;
            disabledAnimation.startPosition = value;
        }

        /// <summary> Set a new start rotation value (RectTransform.localEulerAngles) for all state animations </summary>
        /// <param name="value"> New start rotation </param>
        public void SetStartRotation(Vector3 value)
        {
            normalAnimation.startRotation = value;
            highlightedAnimation.startRotation = value;
            pressedAnimation.startRotation = value;
            selectedAnimation.startRotation = value;
            disabledAnimation.startRotation = value;
        }

        /// <summary> Set a new start scale value (RectTransform.localScale) for all state animations </summary>
        /// <param name="value"> New start scale </param>
        public void SetStartScale(Vector3 value)
        {
            normalAnimation.startScale = value;
            highlightedAnimation.startScale = value;
            pressedAnimation.startScale = value;
            selectedAnimation.startScale = value;
            disabledAnimation.startScale = value;
        }

        /// <summary> Set a new start alpha value (CanvasGroup.alpha) for all state animations </summary>
        /// <param name="value"> New start scale </param>
        public void SetStartAlpha(float value)
        {
            normalAnimation.startAlpha = value;
            highlightedAnimation.startAlpha = value;
            pressedAnimation.startAlpha = value;
            selectedAnimation.startAlpha = value;
            disabledAnimation.startAlpha = value;
        }

        /// <summary> Reset the give animation to a set of default values </summary>
        /// <param name="target"> Target animation </param>
        private static void ResetAnimation(UIAnimation target)
        {
            target.Move.Reset();
            target.Rotate.Reset();
            target.Scale.Reset();
            target.Fade.Reset();

            target.animationType = UIAnimationType.State;

            target.Move.fromReferenceValue = ReferenceValue.CurrentValue;
            target.Rotate.fromReferenceValue = ReferenceValue.CurrentValue;
            target.Scale.fromReferenceValue = ReferenceValue.CurrentValue;
            target.Fade.fromReferenceValue = ReferenceValue.CurrentValue;

            target.Move.settings.duration = UISelectable.k_DefaultAnimationDuration;
            target.Rotate.settings.duration = UISelectable.k_DefaultAnimationDuration;
            target.Scale.settings.duration = UISelectable.k_DefaultAnimationDuration;
            target.Fade.settings.duration = UISelectable.k_DefaultAnimationDuration;
        }
    }
}
