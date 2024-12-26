// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.Reactor.Ticker;
using Doozy.Runtime.UIManager.Animators;
using UnityEngine;
using UnityEngine.Events;
namespace Doozy.Runtime.UIManager.Visual
{
    /// <summary>
    /// Specialized visual component used to swap a Sprites for a Reactor Sprite Target by listening to a UIToggle (controller) isOn state changes
    /// </summary>
    [AddComponentMenu("UI/Components/Addons/UIToggle SpriteSwapper")]
    public class UIToggleSpriteSwapper : BaseUIToggleAnimator
    {
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/UI/Components/Addons/UIToggle SpriteSwapper", false, 8)]
        private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        {
            GameObjectUtils.AddToScene<UIToggleSpriteSwapper>("UIToggle SpriteSwapper", false, true);
        }
        #endif

        [SerializeField] private ReactorSpriteTarget SpriteTarget;
        /// <summary> Reference to a sprite target component </summary>
        public ReactorSpriteTarget spriteTarget => SpriteTarget;

        /// <summary> Check if a sprite target is referenced or not </summary>
        public bool hasSpriteTarget => SpriteTarget != null;

        [SerializeField] private Sprite OnSprite;
        /// <summary> Sprite to set when the UIToggle isOn state transitions to TRUE </summary>
        public Sprite onSprite
        {
            get => OnSprite;
            set
            {
                OnSprite = value;
                if (controller.isOn && hasSpriteTarget) SpriteTarget.SetSprite(OnSprite);
            }
        }

        [SerializeField] private Sprite OffSprite;
        /// <summary> Sprite to set when the UIToggle isOn state transitions to FALSE </summary>
        public Sprite offSprite
        {
            get => OffSprite;
            set
            {
                OffSprite = value;
                if (!controller.isOn && hasSpriteTarget) SpriteTarget.SetSprite(OffSprite);
            }
        }

        protected override bool onAnimationIsActive => hasController && hasSpriteTarget && onSprite != null && spriteTarget.sprite == onSprite;
        protected override bool offAnimationIsActive => hasController && hasSpriteTarget && offSprite != null && spriteTarget.sprite == offSprite;
        protected override UnityAction playOnAnimation => () =>
        {
            if (!hasSpriteTarget) return;
            SpriteTarget.SetSprite(onSprite);
        };
        protected override UnityAction playOffAnimation => () =>
        {
            if (!hasSpriteTarget) return;
            SpriteTarget.SetSprite(offSprite);
        };
        protected override UnityAction reverseOnAnimation => () =>
        {
            if (!hasSpriteTarget) return;
            SpriteTarget.SetSprite(offSprite);
        };
        protected override UnityAction reverseOffAnimation => () =>
        {
            if (!hasSpriteTarget) return;
            SpriteTarget.SetSprite(onSprite);
        };
        protected override UnityAction instantPlayOnAnimation => () =>
        {
            if (!hasSpriteTarget) return;
            SpriteTarget.SetSprite(onSprite);
        };
        protected override UnityAction instantPlayOffAnimation => () =>
        {
            if (!hasSpriteTarget) return;
            SpriteTarget.SetSprite(offSprite);
        };
        protected override UnityAction stopOnAnimation => () => {};               //do nothing
        protected override UnityAction stopOffAnimation => () => {};              //do nothing
        protected override UnityAction addResetToOnStateCallback => () => {};     //do nothing
        protected override UnityAction removeResetToOnStateCallback => () => {};  //do nothing
        protected override UnityAction addResetToOffStateCallback => () => {};    //do nothing
        protected override UnityAction removeResetToOffStateCallback => () => {}; //do nothing

        #if UNITY_EDITOR
        protected override void Reset()
        {
            FindTarget();
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

        public override void UpdateSettings()
        {
            if (!hasSpriteTarget) return;
            if (!hasController) return;
            SpriteTarget.SetSprite(controller.isOn ? onSprite : offSprite);
        }

        public override void StopAllReactions() {}
        public override void ResetToStartValues(bool forced = false) {}
        public override List<Heartbeat> SetHeartbeat<Theartbeat>() => null;
    }
}
