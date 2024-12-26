// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.Reactor.Ticker;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Animators
{
    /// <summary>
    /// Specialized animator component used to animate a set of Sprites for a Reactor Sprite Target
    /// by listening to a UIContainer (controller) show/hide commands.
    /// </summary>
    [AddComponentMenu("UI/Containers/Animators/UIContainer Sprite Animator")]
    public class UIContainerSpriteAnimator : BaseUIContainerAnimator
    {
        [SerializeField] private ReactorSpriteTarget SpriteTarget;
        /// <summary> Reference to a sprite target component </summary>
        public ReactorSpriteTarget spriteTarget => SpriteTarget;
        
        /// <summary> Check if a sprite target is referenced or not </summary>
        public bool hasSpriteTarget => SpriteTarget != null;
        
        [SerializeField] private SpriteAnimation ShowAnimation;
        /// <summary> Container Show Animation </summary>
        public SpriteAnimation showAnimation => ShowAnimation ?? (ShowAnimation = new SpriteAnimation(spriteTarget));

        [SerializeField] private SpriteAnimation HideAnimation;
        /// <summary> Container Hide Animation </summary>
        public SpriteAnimation hideAnimation => HideAnimation ?? (HideAnimation = new SpriteAnimation(spriteTarget));
        
        #if UNITY_EDITOR
        protected override void Reset()
        {
            FindTarget();
            ResetAnimation(showAnimation);
            ResetAnimation(hideAnimation);
            base.Reset();
        }
        #endif
        
        public void FindTarget()
        {
            if (SpriteTarget != null)
                return;
            
            SpriteTarget = ReactorSpriteTarget.FindTarget(gameObject);
            UpdateSettings();
        }
        
        protected override void Awake()
        {
            FindTarget();
            UpdateSettings();
            base.Awake();
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            ShowAnimation?.Recycle();
            HideAnimation?.Recycle();
        }
        
        /// <summary> Connect to Controller </summary>
        protected override void ConnectToController()
        {
            base.ConnectToController();
            if (!controller) return;
            
            controller.showReactions.Add(showAnimation.animation);
            controller.hideReactions.Add(hideAnimation.animation);
        }
        
        /// <summary> Disconnect from Controller </summary>
        protected override void DisconnectFromController()
        {
            base.DisconnectFromController();
            if (!controller) return;
            
            controller.showReactions.Remove(showAnimation.animation);
            controller.hideReactions.Remove(hideAnimation.animation);
        }
        
        /// <summary> Play the show animation </summary>
        public override void Show() =>
            showAnimation.Play(PlayDirection.Forward);

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
                return;
            }
            Hide();
        }
        
        /// <summary> Play the hide animation </summary>
        public override void Hide() =>
            hideAnimation.Play(PlayDirection.Forward);

        /// <summary> Reverse the hide animation (if playing) </summary>
        public override void ReverseHide()
        {
            if(hideAnimation.isPlaying)
            {
                hideAnimation.OnFinishCallback.AddListener(OnReverseHideComplete);
                void OnReverseHideComplete()
                {
                    InstantShow();
                    hideAnimation.OnFinishCallback.RemoveListener(OnReverseHideComplete);
                }
                hideAnimation.Reverse();
                return;
            }
            Show();
        }
        
        /// <summary> Set show animation's progress at one </summary>
        public override void InstantShow() =>
            showAnimation.SetProgressAtOne();

        /// <summary> Set hide animation's progress at one </summary>
        public override void InstantHide() =>
            hideAnimation.SetProgressAtOne();
       
        /// <summary> Update the animations settings (if a spriteTarget is referenced) </summary>
        public override void UpdateSettings()
        {
            if(spriteTarget == null)
                return;

            showAnimation.SetTarget(spriteTarget);
            hideAnimation.SetTarget(spriteTarget);
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
            if (showAnimation.isActive) showAnimation.Stop();
            if (hideAnimation.isActive) hideAnimation.Stop();
            
            showAnimation.ResetToStartValues(forced);
            hideAnimation.ResetToStartValues(forced);

            if (spriteTarget == null)
                return;
            
            spriteTarget.sprite = showAnimation.sprites[showAnimation.startFrame];

            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(rectTransform);
            UnityEditor.SceneView.RepaintAll();
            #endif
        }
        
        
        /// <summary> Set animation heartbeat </summary>
        public override List<Heartbeat> SetHeartbeat<T>()
        {
            var list = new List<Heartbeat>();
            for (int i = 0; i < 2; i++) list.Add(new T());
            
            showAnimation.animation.SetHeartbeat(list[0]);
            hideAnimation.animation.SetHeartbeat(list[1]);
            
            return list;
        }

        private static void ResetAnimation(SpriteAnimation target)
        {
            target.animation.Reset();
            target.animation.enabled = false;
            target.animation.settings.duration = 1f;
        }
    }
}
