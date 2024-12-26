// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Reactor.Ticker;
using Doozy.Runtime.UIManager.Animators;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.UIManager.Audio
{
    /// <summary>
    /// Specialized audio component used to play a set AudioClip by listening to a UISelectable (controller) selection state changes.
    /// </summary>
    [AddComponentMenu("UI/Components/Addons/UISelectable Audio")]
    public class UISelectableAudio : BaseUISelectableAnimator
    {
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/UI/Components/Addons/UISelectable Audio", false, 8)]
        private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        {
            GameObjectUtils.AddToScene<UISelectableAudio>("UISelectable Audio", false, true);
        }
        #endif

        [SerializeField] private AudioSource AudioSource;
        /// <summary> Reference to a target Audio source </summary>
        public AudioSource audioSource => AudioSource;

        /// <summary> Check if a AudioSource is referenced or not </summary>
        public bool hasAudioSource => AudioSource != null;

        [SerializeField] private AudioClip NormalAudioClip;
        /// <summary> AudioClip for the Selectable Normal state </summary>
        public AudioClip normalAudioClip => NormalAudioClip;

        [SerializeField] private AudioClip HighlightedAudioClip;
        /// <summary> AudioClip for the Selectable Highlighted state </summary>
        public AudioClip highlightedAudioClip => HighlightedAudioClip;

        [SerializeField] private AudioClip PressedAudioClip;
        /// <summary> AudioClip for the Selectable Pressed state </summary>
        public AudioClip pressedAudioClip => PressedAudioClip;

        [SerializeField] private AudioClip SelectedAudioClip;
        /// <summary> AudioClip for the Selectable Selected state </summary>
        public AudioClip selectedAudioClip => SelectedAudioClip;

        [SerializeField] private AudioClip DisabledAudioClip;
        /// <summary> AudioClip for the Selectable Disabled state </summary>
        public AudioClip disabledAudioClip => DisabledAudioClip;

        private bool initialized { get; set; }

        protected override void OnEnable()
        {
            initialized = false;
            base.OnEnable();
        }

        public override void StopAllReactions()
        {
            if (!hasAudioSource)
                return;

            audioSource.Stop();
        }

        public override bool IsStateEnabled(UISelectionState state) =>
            true;

        public override void Play(UISelectionState state)
        {
            if (!initialized)
            {
                initialized = true;
                if (state == UISelectionState.Normal)
                    return;
            }

            if (!hasAudioSource)
                return;

            audioSource.Stop();

            switch (state)
            {
                case UISelectionState.Normal:
                    audioSource.clip = normalAudioClip;
                    break;
                case UISelectionState.Highlighted:
                    audioSource.clip = highlightedAudioClip;
                    break;
                case UISelectionState.Pressed:
                    audioSource.clip = pressedAudioClip;
                    break;
                case UISelectionState.Selected:
                    audioSource.clip = selectedAudioClip;
                    break;
                case UISelectionState.Disabled:
                    audioSource.clip = disabledAudioClip;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

            if (audioSource.clip != null)
                audioSource.Play();
        }

        public override void UpdateSettings() {}                           //ignored
        public override void ResetToStartValues(bool forced = false) {}    //ignored
        public override List<Heartbeat> SetHeartbeat<T>() { return null; } //ignored
    }
}
