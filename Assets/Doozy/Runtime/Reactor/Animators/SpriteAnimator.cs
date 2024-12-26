// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Animators.Internal;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.Reactor.Ticker;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.Reactor.Animators
{
    /// <summary> Specialized animator component used to animate a set of Sprites for a SpriteTarget </summary>
    [AddComponentMenu("Reactor/Animators/Sprite Animator")]
    public class SpriteAnimator : ReactorAnimator
    {
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/Reactor/Animators/Sprite Animator", false, 6)]
        private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        {
            GameObjectUtils.AddToScene<SpriteAnimator>(false, true);
        }
        #endif

        /// <summary>
        /// Specialized animator component used to animate a the Sprite for a SpriteTarget
        /// </summary>
        [SerializeField] private ReactorSpriteTarget SpriteTarget;
        /// <summary> Reference to a sprite target component </summary>
        public ReactorSpriteTarget spriteTarget => SpriteTarget;

        /// <summary> Check if a sprite target is referenced or not </summary>
        public bool hasTarget => SpriteTarget != null;

        [SerializeField] private SpriteAnimation Animation;
        /// <summary> Sprite frame by frame Animation </summary>
        public new SpriteAnimation animation => Animation ?? (Animation = new SpriteAnimation(spriteTarget));

        #if UNITY_EDITOR
        private void Reset()
        {
            FindTarget();
            Animation ??= new SpriteAnimation(spriteTarget);
            ResetAnimation();
        }
        #endif

        /// <summary>
        /// Find a SpriteTarget on the GameObject this animator is attached to
        /// </summary>
        public void FindTarget()
        {
            if (SpriteTarget != null)
            {
                if (animation.spriteTarget != SpriteTarget)
                    animation.SetTarget(SpriteTarget);

                return;
            }

            SpriteTarget = ReactorSpriteTarget.FindTarget(gameObject);
            if (SpriteTarget != null) animation.SetTarget(SpriteTarget);
        }

        protected override void Awake()
        {
            if (!Application.isPlaying) return;
            base.Awake();
            animation.UpdateAnimationSprites();
            FindTarget();
        }

        /// <summary> Play the animation all the way in the given direction </summary>
        /// <param name="playDirection"> Play direction (Forward or Reverse) </param>
        public override void Play(PlayDirection playDirection) =>
            animation.Play(playDirection);

        /// <summary> Play the animation all the way </summary>
        /// <param name="inReverse"> Play the animation in reverse? </param>
        public override void Play(bool inReverse = false) =>
            animation.Play(inReverse);

        /// <summary> Set the animator target </summary>
        /// <param name="target"> Sprite target </param>
        public override void SetTarget(object target) =>
            SetTarget(target as ReactorSpriteTarget);

        /// <summary> Set the animator target </summary>
        /// <param name="target"> Sprite target </param>
        public void SetTarget(ReactorSpriteTarget target) =>
            animation.SetTarget(target);

        /// <summary> Reset all the reactions to their initial values (if the animation is enabled) </summary>
        /// <param name="forced"> If true, forced will ignore if the animation is enabled or not </param>
        public override void ResetToStartValues(bool forced = false)
        {
            if (animation.isActive) Stop();
            animation.ResetToStartValues(forced);

            if (spriteTarget == null)
                return;

            spriteTarget.sprite =
                animation.sprites != null &&                       //if the sprites list is not null
                animation.sprites.Count > 0 &&                     //and the sprites list count is greater than 0
                animation.startFrame >= 0 &&                       //and the start frame is greater than or equal to 0
                animation.startFrame < animation.sprites.Count - 1 //and the start frame is less than the sprites list count - 1
                    ? animation.sprites[animation.startFrame]      //then return the sprite at the start frame index
                    : null;                                        //else return null

            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(spriteTarget);
            UnityEditor.SceneView.RepaintAll();
            #endif
        }

        /// <summary> Refresh the set target and, if the animation is playing, update the calculated values </summary>
        public override void UpdateSettings()
        {
            SetTarget(spriteTarget);
            if (animation.isPlaying) UpdateValues();
        }

        /// <summary> Get the animation start delay. For random values it returns the max value. </summary>
        public override float GetStartDelay() =>
            animation.animation.isActive ? animation.animation.startDelay : animation.animation.settings.GetStartDelay();

        /// <summary> Get the animation duration. For random values it returns the max value. </summary>
        public override float GetDuration() =>
            animation.animation.isActive ? animation.animation.duration : animation.animation.settings.GetDuration();

        /// <summary> Get the animation start delay + duration. For random values it returns the max value. </summary>
        public override float GetTotalDuration() =>
            GetStartDelay() + GetDuration();

        /// <summary> Set animation heartbeat </summary>
        public override List<Heartbeat> SetHeartbeat<T>()
        {
            var list = new List<Heartbeat>() { new T() };
            animation.animation.SetHeartbeat(list[0]);
            return list;
        }

        /// <summary> Update From and To values by recalculating them </summary>
        public override void UpdateValues() =>
            animation.UpdateValues();

        /// <summary> Play the animation at the given progress value from the current value </summary>
        /// <param name="toProgress"> To progress [0,1] </param>
        public override void PlayToProgress(float toProgress) =>
            animation.PlayToProgress(toProgress);

        /// <summary> Play the animation from the given progress value to the current value </summary>
        /// <param name="fromProgress"> From progress [0,1] </param>
        public override void PlayFromProgress(float fromProgress) =>
            animation.PlayFromProgress(fromProgress);

        /// <summary> Play the animation from the given progress value to the given progress value </summary>
        /// <param name="fromProgress"> From progress [0,1] </param>
        /// <param name="toProgress"> To progress [0,1] </param>
        public override void PlayFromToProgress(float fromProgress, float toProgress) =>
            animation.PlayFromToProgress(fromProgress, toProgress);

        /// <summary>
        /// Stop the animation.
        /// Called every time the animation is stopped.
        /// Also called before calling Finish()
        /// </summary>
        public override void Stop() =>
            animation.Stop();

        /// <summary>
        /// Finish the animation.
        /// Called to mark that that animation completed playing.
        /// </summary>
        public override void Finish() =>
            animation.Finish();

        /// <summary>
        /// Reverse the animation's direction while playing.
        /// Works only if the animation is active (it either playing or paused)
        /// </summary>
        public override void Reverse() =>
            animation.Reverse();

        /// <summary>
        /// Rewind the animation to the start values
        /// </summary>
        public override void Rewind() =>
            animation.Rewind();

        /// <summary>
        /// Pause the animation.
        /// Works only if the animation is playing.
        /// </summary>
        public override void Pause() =>
            animation.Pause();

        /// <summary>
        /// Resume a paused animation.
        /// Works only if the animation is paused.
        /// </summary>
        public override void Resume() =>
            animation.Resume();

        /// <summary> Set the animation at 100% (at the end, or the 'To' value) </summary>
        public override void SetProgressAtOne() =>
            animation.SetProgressAtOne();

        /// <summary> Set the animation at 0% (at the start, or the 'From' value) </summary>
        public override void SetProgressAtZero() =>
            animation.SetProgressAtZero();

        /// <summary> Set the animation at the given progress value </summary>
        /// <param name="targetProgress"> Target progress [0,1] </param>
        public override void SetProgressAt(float targetProgress) =>
            animation.SetProgressAt(targetProgress);

        /// <summary>
        /// Recycle the reactions controlled by this animation.
        /// <para/> Reactions are pooled can (and should) be recycled to improve overall performance. 
        /// </summary>
        protected override void Recycle() =>
            animation?.Recycle();

        private void ResetAnimation()
        {
            var reaction = animation.animation;

            reaction.Reset();
            reaction.enabled = true;
            reaction.fromReferenceValue = FrameReferenceValue.FirstFrame;
            reaction.toReferenceValue = FrameReferenceValue.LastFrame;
            reaction.settings.duration = 1f;
        }

    }
}
