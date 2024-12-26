// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Reactor.Ticker;
using Doozy.Runtime.UIManager.Animators;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.UIManager.Audio
{
    /// <summary>
    /// Specialized audio component used to play a set AudioClip by listening to a UIContainer (controller) show/hide commands.
    /// </summary>
    [AddComponentMenu("UI/Containers/Addons/UIContainer Audio")]
    public class UIContainerAudio : BaseUIContainerAnimator
    {
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/UI/Containers/Addons/UIContainer Audio", false, 8)]
        private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        {
            GameObjectUtils.AddToScene<UIContainerAudio>("UIContainer Audio", false, true);
        }
        #endif

        [SerializeField] private AudioSource AudioSource;
        /// <summary> Reference to a target Audio source </summary>
        public AudioSource audioSource => AudioSource;

        /// <summary> Check if a AudioSource is referenced or not </summary>
        public bool hasAudioSource => AudioSource != null;

        [SerializeField] private AudioClip ShowAudioClip;
        /// <summary> Container Show AudioClip </summary>
        public AudioClip showAudioClip => ShowAudioClip;

        [SerializeField] private AudioClip HideAudioClip;
        /// <summary> Container Hide AudioClip </summary>
        public AudioClip hideAudioClip => HideAudioClip;

        /// <summary> Stop the currently playing sound, if any. </summary>
        public override void StopAllReactions()
        {
            if (!hasAudioSource) return;
            audioSource.Stop();
        }

        public override void Show()
        {
            if (!hasAudioSource) return;
            if (showAudioClip == null) return;
            audioSource.Stop();
            audioSource.clip = showAudioClip;
            audioSource.Play();
        }

        public override void ReverseShow() =>
            Hide();

        public override void Hide()
        {
            if (!hasAudioSource) return;
            if (hideAudioClip == null) return;
            audioSource.Stop();
            audioSource.clip = hideAudioClip;
            audioSource.Play();
        }

        public override void ReverseHide() =>
            Show();

        /// <summary> Ignored </summary>
        public override void UpdateSettings() {}
        /// <summary> Ignored </summary>
        public override void InstantShow() {}
        /// <summary> Ignored </summary>
        public override void InstantHide() {}
        /// <summary> Ignored </summary>
        public override void ResetToStartValues(bool forced = false) {}
        /// <summary> Ignored </summary>
        public override List<Heartbeat> SetHeartbeat<T>() { return null; }
    }
}
