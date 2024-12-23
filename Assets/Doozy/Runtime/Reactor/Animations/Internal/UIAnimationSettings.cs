// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Reactor.Internal;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Animations
{
    /// <summary>
    /// Data class used to store settings for an <see cref="UIAnimation"/>.
    /// This class is also used for presets.
    /// </summary>
    [Serializable]
    public class UIAnimationSettings
    {
        [SerializeField] private UIAnimationType AnimationType;
        /// <summary> Type of UI animation </summary>
        public UIAnimationType animationType => AnimationType;

        /// <summary> Enabled state for the Move reaction </summary>
        public bool MoveEnabled;
        /// <summary> Setting for the 'From' reference value </summary>
        public ReferenceValue MoveFromReferenceValue;
        /// <summary> Setting for the 'To' reference value </summary>
        public ReferenceValue MoveToReferenceValue;
        /// <summary> Setting for the 'From' custom value </summary>
        public Vector3 MoveFromCustomValue;
        /// <summary> Setting for the 'To' custom value </summary>
        public Vector3 MoveToCustomValue;
        /// <summary> Setting for the 'From' offset </summary>
        public Vector3 MoveFromOffset;
        /// <summary> Setting for the 'To' offset </summary>
        public Vector3 MoveToOffset;
        /// <summary> Setting for the 'From' direction </summary>
        public MoveDirection MoveFromDirection;
        /// <summary> Setting for the 'To' direction </summary>
        public MoveDirection MoveToDirection;
        /// <summary> Reaction setting for the Move reaction </summary>
        public ReactionSettings MoveReactionSettings;

        /// <summary> Enabled state for the Rotate reaction </summary>
        public bool RotateEnabled;
        /// <summary> Setting for the 'From' reference value </summary>
        public ReferenceValue RotateFromReferenceValue;
        /// <summary> Setting for the 'To' reference value </summary>
        public ReferenceValue RotateToReferenceValue;
        /// <summary> Setting for the 'From' custom value </summary>
        public Vector3 RotateFromCustomValue;
        /// <summary> Setting for the 'To' custom value </summary>
        public Vector3 RotateToCustomValue;
        /// <summary> Setting for the 'From' offset </summary>
        public Vector3 RotateFromOffset;
        /// <summary> Setting for the 'To' offset </summary>
        public Vector3 RotateToOffset;
        /// <summary> Reaction setting for the Rotate reaction </summary>
        public ReactionSettings RotateReactionSettings;

        /// <summary> Enabled state for the Scale reaction </summary>
        public bool ScaleEnabled;
        /// <summary> Setting for the 'From' reference value </summary>
        public ReferenceValue ScaleFromReferenceValue;
        /// <summary> Setting for the 'To' reference value </summary>
        public ReferenceValue ScaleToReferenceValue;
        /// <summary> Setting for the 'From' custom value </summary>
        public Vector3 ScaleFromCustomValue;
        /// <summary> Setting for the 'To' custom value </summary>
        public Vector3 ScaleToCustomValue;
        /// <summary> Setting for the 'From' offset </summary>
        public Vector3 ScaleFromOffset;
        /// <summary> Setting for the 'To' offset </summary>
        public Vector3 ScaleToOffset;
        /// <summary> Reaction setting for the Scale reaction </summary>
        public ReactionSettings ScaleReactionSettings;

        /// <summary> Enabled state for the Fade reaction </summary>
        public bool FadeEnabled;
        /// <summary> Setting for the 'From' reference value </summary>
        public ReferenceValue FadeFromReferenceValue;
        /// <summary> Setting for the 'To' reference value </summary>
        public ReferenceValue FadeToReferenceValue;
        /// <summary> Setting for the 'From' custom value </summary>
        public float FadeFromCustomValue;
        /// <summary> Setting for the 'To' custom value </summary>
        public float FadeToCustomValue;
        /// <summary> Setting for the 'From' offset </summary>
        public float FadeFromOffset;
        /// <summary> Setting for the 'To' offset </summary>
        public float FadeToOffset;
        /// <summary> Reaction setting for the Fade reaction </summary>
        public ReactionSettings FadeReactionSettings;

        /// <summary> Construct a new UIAnimationSettings (with the default animation type set to custom) </summary>
        public UIAnimationSettings() : this(UIAnimationType.Custom) {}

        /// <summary> Construct a new UIAnimationSettings with the given animation type; </summary>
        /// <param name="animationType"> Animation type </param>
        public UIAnimationSettings(UIAnimationType animationType)
        {
            AnimationType = animationType;
            MoveReactionSettings = new ReactionSettings();
            RotateReactionSettings = new ReactionSettings();
            ScaleReactionSettings = new ReactionSettings();
            FadeReactionSettings = new ReactionSettings();
        }

        /// <summary> Construct a new UIAnimationSettings with the settings of the given source </summary>
        /// <param name="source"> Other UIAnimationSettings used as a source for the settings </param>
        public UIAnimationSettings(UIAnimation source) =>
            GetAnimationSettings(source);

        /// <summary> Apply the settings to the target UIAnimation (copy the setting to the target) </summary>
        /// <param name="target"> Target UIAnimation </param>
        public void SetAnimationSettings(UIAnimation target)
        {
            _ = target ?? throw new NullReferenceException(nameof(target));

            target.animationType = AnimationType;

            target.Move.enabled = MoveEnabled;
            target.Move.animationType = AnimationType;
            target.Move.fromReferenceValue = MoveFromReferenceValue;
            target.Move.toReferenceValue = MoveToReferenceValue;
            target.Move.fromCustomValue = MoveFromCustomValue;
            target.Move.toCustomValue = MoveToCustomValue;
            target.Move.fromOffset = MoveFromOffset;
            target.Move.toOffset = MoveToOffset;
            target.Move.fromDirection = target.animationType == UIAnimationType.Show ? MoveFromDirection : MoveDirection.CustomPosition;
            target.Move.toDirection = target.animationType == UIAnimationType.Hide ? MoveToDirection : MoveDirection.CustomPosition;
            target.Move.ApplyReactionSettings(MoveReactionSettings);

            target.Rotate.enabled = RotateEnabled;
            target.Rotate.fromReferenceValue = RotateFromReferenceValue;
            target.Rotate.toReferenceValue = RotateToReferenceValue;
            target.Rotate.fromCustomValue = RotateFromCustomValue;
            target.Rotate.toCustomValue = RotateToCustomValue;
            target.Rotate.fromOffset = RotateFromOffset;
            target.Rotate.toOffset = RotateToOffset;
            target.Rotate.ApplyReactionSettings(RotateReactionSettings);

            target.Scale.enabled = ScaleEnabled;
            target.Scale.fromReferenceValue = ScaleFromReferenceValue;
            target.Scale.toReferenceValue = ScaleToReferenceValue;
            target.Scale.fromCustomValue = ScaleFromCustomValue;
            target.Scale.toCustomValue = ScaleToCustomValue;
            target.Scale.fromOffset = ScaleFromOffset;
            target.Scale.toOffset = ScaleToOffset;
            target.Scale.ApplyReactionSettings(ScaleReactionSettings);

            target.Fade.enabled = FadeEnabled;
            target.Fade.fromReferenceValue = FadeFromReferenceValue;
            target.Fade.toReferenceValue = FadeToReferenceValue;
            target.Fade.fromCustomValue = FadeFromCustomValue;
            target.Fade.toCustomValue = FadeToCustomValue;
            target.Fade.fromOffset = FadeFromOffset;
            target.Fade.toOffset = FadeToOffset;
            target.Fade.ApplyReactionSettings(FadeReactionSettings);
        }

        /// <summary> Get the settings from a source UIAnimation (copy the settings from the source) </summary>
        /// <param name="source"> Source UIAnimation </param>
        public void GetAnimationSettings(UIAnimation source)
        {
            _ = source ?? throw new NullReferenceException(nameof(source));

            AnimationType = source.animationType;

            MoveEnabled = source.Move.enabled;
            MoveFromReferenceValue = source.Move.fromReferenceValue;
            MoveToReferenceValue = source.Move.toReferenceValue;
            MoveFromCustomValue = source.Move.fromCustomValue;
            MoveToCustomValue = source.Move.toCustomValue;
            MoveFromOffset = source.Move.fromOffset;
            MoveToOffset = source.Move.toOffset;
            MoveFromDirection = source.animationType == UIAnimationType.Show ? source.Move.fromDirection : MoveDirection.CustomPosition;
            MoveToDirection = source.animationType == UIAnimationType.Hide ?  source.Move.toDirection : MoveDirection.CustomPosition;;
            MoveReactionSettings = new ReactionSettings(source.Move.settings);

            RotateEnabled = source.Rotate.enabled;
            RotateFromReferenceValue = source.Rotate.fromReferenceValue;
            RotateToReferenceValue = source.Rotate.toReferenceValue;
            RotateFromCustomValue = source.Rotate.fromCustomValue;
            RotateToCustomValue = source.Rotate.toCustomValue;
            RotateFromOffset = source.Rotate.fromOffset;
            RotateToOffset = source.Rotate.toOffset;
            RotateReactionSettings = new ReactionSettings(source.Rotate.settings);

            ScaleEnabled = source.Scale.enabled;
            ScaleFromReferenceValue = source.Scale.fromReferenceValue;
            ScaleToReferenceValue = source.Scale.toReferenceValue;
            ScaleFromCustomValue = source.Scale.fromCustomValue;
            ScaleToCustomValue = source.Scale.toCustomValue;
            ScaleFromOffset = source.Scale.fromOffset;
            ScaleToOffset = source.Scale.toOffset;
            ScaleReactionSettings = new ReactionSettings(source.Scale.settings);

            FadeEnabled = source.Fade.enabled;
            FadeFromReferenceValue = source.Fade.fromReferenceValue;
            FadeToReferenceValue = source.Fade.toReferenceValue;
            FadeFromCustomValue = source.Fade.fromCustomValue;
            FadeToCustomValue = source.Fade.toCustomValue;
            FadeFromOffset = source.Fade.fromOffset;
            FadeToOffset = source.Fade.toOffset;
            FadeReactionSettings = new ReactionSettings(source.Fade.settings);
        }
        
        /// <summary>
        /// Apply the settings from the source UIAnimationSettings to the target UIAnimation
        /// (copy the settings from the source to the target)
        /// <para/> FROM UIAnimationSettings TO UIAnimation 
        /// </summary>
        /// <param name="source"> Source UIAnimationSettings </param>
        /// <param name="target"> Target UIAnimation </param>
        public static void SetAnimationSettings(UIAnimationSettings source, UIAnimation target)
        {
            _ = source ?? throw new NullReferenceException(nameof(source));
            _ = target ?? throw new NullReferenceException(nameof(target));
            source.SetAnimationSettings(target);
        }

        /// <summary>
        /// Get the settings from the source UIAnimation to the target UIAnimationSettings
        /// (copy the settings from the source to the target)
        /// <para/> FROM UIAnimation TO UIAnimationSettings 
        /// </summary>
        /// <param name="source"> Source UIAnimation </param>
        /// <param name="target"> Target UIAnimationSettings </param>
        public static void GetAnimationSettings(UIAnimation source, UIAnimationSettings target)
        {
            _ = source ?? throw new NullReferenceException(nameof(source));
            _ = target ?? throw new NullReferenceException(nameof(target));
            target.GetAnimationSettings(source);
        }
    }
}
