// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Events;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global

namespace Doozy.Runtime.UIManager.Animators
{
    /// <summary> Base class for all UISelectable animators for Normal, Highlighted, Pressed and Disabled states </summary>
    public abstract class BaseUISelectableAnimator : BaseTargetComponentAnimator<UISelectable>
    {
        /// <summary> Filter used when the referenced selectable is a Toggle </summary>
        public bool IsOn;

        /// <summary> Filter used when the referenced selectable is a Toggle </summary>
        public CommandToggle ToggleCommand = CommandToggle.Any;

        /// <summary> Returns TRUE if the controller selectable type is Button </summary>
        public bool controllerIsButton => hasController && controller.isButton;

        /// <summary> Returns TRUE if the controller selectable type is Toggle </summary>
        public bool controllerIsToggle => hasController && controller.isToggle;

        /// <summary> Connect to Controller </summary>
        protected override void ConnectToController()
        {
            if (controller == null) return;
            controller.OnSelectionStateChangedCallback ??= new UISelectionStateEvent();
            controller.OnSelectionStateChangedCallback.AddListener(OnSelectionStateChanged);
            StartCoroutine(UpdateStateLater());
        }

        /// <summary> Disconnect from Controller </summary>
        protected override void DisconnectFromController()
        {
            if (controller == null) return;
            controller.OnSelectionStateChangedCallback ??= new UISelectionStateEvent();
            controller.OnSelectionStateChangedCallback.RemoveListener(OnSelectionStateChanged);
        }

        /// <summary> Do on selection state changed </summary>
        /// <param name="state"> New state </param>
        protected virtual void OnSelectionStateChanged(UISelectionState state)
        {
            if (controller == null) return;

            if (controllerIsToggle)
            {
                switch (ToggleCommand)
                {
                    case CommandToggle.On when !controller.isOn:
                    case CommandToggle.Off when controller.isOn:
                        return;
                }
            }

            if (!IsStateEnabled(state)) return;
            if (!Application.isPlaying) return;

            StopAllReactions();
            Play(state);
        }

        /// <summary> Returns TRUE if the state is enabled </summary>
        public abstract bool IsStateEnabled(UISelectionState state);
        
        /// <summary> Play the animation for the given state </summary>
        public abstract void Play(UISelectionState state);

        /// <summary> Update the state at the end of the frame </summary>
        private IEnumerator UpdateStateLater()
        {
            yield return new WaitForEndOfFrame();
            OnSelectionStateChanged(controller.currentUISelectionState);
        }
    }
}
