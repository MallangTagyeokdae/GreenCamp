// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.Reactor.Targets;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Animations
{
    /// <summary>
    /// Reactor animation that works with a list of sprites.
    /// It works like a frame by frame animation, each sprite counting as a frame, for a sprite target.
    /// </summary>
    [Serializable]
    public class SpriteAnimation : ReactorAnimation
    {
        /// <summary> Reference to a sprite target component </summary>
        public ReactorSpriteTarget spriteTarget { get; private set; }

        [SerializeField] private List<Sprite> Sprites = new List<Sprite>();
        /// <summary> List of sprites, each sprite counted as a frame for the animation </summary>
        public List<Sprite> sprites => Sprites;

        /// <summary> Check if a sprite target is referenced or not </summary>
        public bool hasTarget => spriteTarget != null;

        [SerializeField] private SpriteTargetReaction Animation;
        /// <summary> Sprite reaction designed to change (via a frame by frame animation) the sprite of a sprite target </summary>        
        public SpriteTargetReaction animation => Animation ?? (Animation = Reaction.Get<SpriteTargetReaction>());

        /// <summary> Animation start frame </summary>
        public int startFrame
        {
            get => animation.startFrame;
            set => animation.startFrame = value;
        }

        /// <summary> Is the animation enabled </summary>
        public override bool isEnabled => animation.enabled;
        /// <summary> Is the animation in the idle state (is enabled, but not active) </summary>
        public override bool isIdle => animation.isIdle;
        /// <summary> Is the animation in the active state (is enabled and either playing or paused) </summary>
        public override bool isActive => animation.isActive;
        /// <summary> Is the animation in the paused state (is enabled, started playing and then paused) </summary>
        public override bool isPaused => animation.isPaused;
        /// <summary> Is the animation in the playing state (is enabled and started playing) </summary>
        public override bool isPlaying => animation.isPlaying;
        /// <summary> Is the animation in the start delay state (is enabled, started playing and waiting to start running after the start delay duration has passed) </summary>
        public override bool inStartDelay => animation.inStartDelay;
        /// <summary> Is the animation in the loop delay state (is enabled, started playing and is between loops waiting to continue running after the loop delay duration has passed) </summary>
        public override bool inLoopDelay => animation.inLoopDelay;

        /// <summary> Construct a new SpriteAnimation with the given sprite target </summary>
        /// <param name="target"> Sprite target</param>
        public SpriteAnimation(ReactorSpriteTarget target = null)
        {
            if (target == null)
                return;

            SetTarget(target);
        }

        /// <summary> Load a set of sprites to the animation. Each sprite is considered a frame. </summary>
        /// <param name="spriteEnumerable"> Collection of sprites </param>
        public SpriteAnimation SetSprites(IEnumerable<Sprite> spriteEnumerable)
        {
            if (spriteEnumerable == null) return this;
            Sprites.Clear();
            Sprites.AddRange(spriteEnumerable);
            return UpdateAnimationSprites();
        }
        
        /// <summary> Updates the list of sprites referenced in the animation </summary>
        public SpriteAnimation UpdateAnimationSprites()
        {
            animation.SetSprites(Sprites, false);
            return this;
        }

        /// <summary> Sorts the sprites in an alphabetical order from A to Z </summary>
        public SpriteAnimation SortSpritesAz()
        {
            Sprites = Sprites.OrderBy(item => item.name).ToList();
            return UpdateAnimationSprites();
        }

        /// <summary> Sorts the sprites in reverse alphabetical order from Z to A </summary>
        public SpriteAnimation SortSpritesZa()
        {
            Sprites = Sprites.OrderByDescending(item => item.name).ToList();
            return UpdateAnimationSprites();
        }

        /// <summary> Set the sprite target </summary>
        /// <param name="target"> Sprite target </param>
        public void SetTarget(ReactorSpriteTarget target)
        {
            spriteTarget = null;
            _ = target ? target : throw new NullReferenceException(nameof(target));
            spriteTarget = target;

            Initialize();
        }

        /// <summary> Initialize de animation </summary>
        public void Initialize()
        {
            animation?.Stop(true);
            Animation ??= Reaction.Get<SpriteTargetReaction>();
            animation?.SetTarget(spriteTarget);

            UpdateValues();
        }

        /// <summary>
        /// Recycle the reactions controlled by this animation.
        /// <para/> Reactions are pooled can (and should) be recycled to improve overall performance. 
        /// </summary>
        public override void Recycle() =>
            animation?.Recycle();

        /// <summary>
        /// Update the initial values for the reactions controlled by this animation
        /// </summary>
        public override void UpdateValues()
        {
            UpdateAnimationSprites();
            animation.UpdateValues();
        }

        /// <summary>
        /// Stop all the reactions controlled by this animation
        /// </summary>
        public override void StopAllReactionsOnTarget() =>
            Reaction.StopAllReactionsByTargetObject(spriteTarget, true);


        /// <summary> Set the animation at the given progress value </summary>
        /// <param name="targetProgress"> Target progress [0,1] </param>
        public override void SetProgressAt(float targetProgress)
        {
            base.SetProgressAt(targetProgress);
            if (!animation.enabled)
                return;
            UpdateAnimationSprites();
            animation.SetProgressAt(targetProgress);
        }

        /// <summary> Play the animation at the given progress value from the current value </summary>
        /// <param name="toProgress"> To progress [0,1] </param>
        public override void PlayToProgress(float toProgress)
        {
            base.PlayToProgress(toProgress);
            if (!animation.enabled)
                return;
            UpdateAnimationSprites();
            animation.PlayToProgress(toProgress);
        }

        /// <summary> Play the animation from the given progress value to the current value </summary>
        /// <param name="fromProgress"> From progress [0,1] </param>
        public override void PlayFromProgress(float fromProgress)
        {
            base.PlayFromProgress(fromProgress);
            if (!animation.enabled)
                return;
            UpdateAnimationSprites();
            animation.PlayFromProgress(fromProgress);
        }

        /// <summary> Play the animation from the given progress value to the given progress value </summary>
        /// <param name="fromProgress"> From progress [0,1] </param>
        /// <param name="toProgress"> To progress [0,1] </param>
        public override void PlayFromToProgress(float fromProgress, float toProgress)
        {
            base.PlayFromToProgress(fromProgress, toProgress);
            if (!animation.enabled)
                return;
            UpdateAnimationSprites();
            animation.PlayFromToProgress(fromProgress, toProgress);
        }

        /// <summary> Play the animation all the way </summary>
        /// <param name="inReverse"> Play the animation in reverse? </param>
        public override void Play(bool inReverse = false)
        {
            if (spriteTarget == null)
                return;

            RegisterCallbacks();
            if (!isActive)
            {
                StopAllReactionsOnTarget();
                // ResetToStartValues();
            }

            if (!animation.enabled)
                return;
            UpdateAnimationSprites();
            animation.Play(inReverse);
        }

        /// <summary> Reset all the reactions to their initial values (if the animation is enabled) </summary>
        /// <param name="forced"> If true, forced will ignore if the animation is enabled or not </param>
        public override void ResetToStartValues(bool forced = false)
        {
            if(spriteTarget == null) return;
            if (forced || animation.enabled)
            {
                UpdateAnimationSprites();
                animation.SetValue(startFrame);
            }
        }

        /// <summary>
        /// Stop the animation.
        /// Called every time the animation is stopped. Also called before calling Finish()
        /// </summary>
        public override void Stop()
        {
            if (animation.isActive || animation.enabled) animation.Stop();
            base.Stop();
        }

        /// <summary>
        /// Finish the animation.
        /// Called to mark that that animation completed playing.
        /// </summary>
        public override void Finish()
        {
            if (animation.isActive || animation.enabled) animation.Finish();
            base.Finish();
        }

        /// <summary>
        /// Reverse the animation's direction while playing.
        /// Works only if the animation is active (it either playing or paused)
        /// </summary>
        public override void Reverse()
        {
            if (animation.isActive) animation.Reverse();
            else if (animation.enabled) animation.Play(PlayDirection.Reverse);
        }

        /// <summary>
        /// Rewind the animation to the start values
        /// </summary>
        public override void Rewind()
        {
            if (!animation.enabled)
                return;
            UpdateAnimationSprites();
            animation.Rewind();
        }

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

        /// <summary> Register all callbacks to the animation </summary>
        protected override void RegisterCallbacks()
        {
            base.RegisterCallbacks();

            if (!animation.enabled)
                return;

            startedReactionsCount++;
            animation.OnPlayCallback += InvokeOnPlay;
            animation.OnStopCallback += InvokeOnStop;
            animation.OnFinishCallback += InvokeOnFinish;
        }

        /// <summary> Unregister OnPlayCallback </summary>
        protected override void UnregisterOnPlayCallbacks()
        {
            if (!animation.enabled)
                return;

            animation.OnPlayCallback -= InvokeOnPlay;
        }

        /// <summary> Unregister OnStopCallback </summary>
        protected override void UnregisterOnStopCallbacks()
        {
            if (!animation.enabled)
                return;

            animation.OnStopCallback -= InvokeOnStop;
        }

        /// <summary> Unregister OnFinishCallback </summary>
        protected override void UnregisterOnFinishCallbacks()
        {
            if (!animation.enabled)
                return;

            animation.OnFinishCallback -= InvokeOnFinish;
        }
    }
}
