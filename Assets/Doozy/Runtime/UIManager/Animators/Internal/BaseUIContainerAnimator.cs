// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections;
using Doozy.Runtime.UIManager.Containers;
using UnityEngine;
// ReSharper disable MemberCanBeProtected.Global

namespace Doozy.Runtime.UIManager.Animators
{
    /// <summary> Base class for all UI Container animators for Show and Hide states </summary>
    public abstract class BaseUIContainerAnimator : BaseTargetComponentAnimator<UIContainer>
    {
        private Coroutine executeCommandCoroutine { get; set; }

        /// <summary> Connect to Controller </summary>
        protected override void ConnectToController()
        {
            if (controller == null) return;
            controller.showHideExecute -= Execute;
            controller.showHideExecute += Execute;
            if (controller.executedFirstCommand)
                Execute(controller.previouslyExecutedCommand);
        }

        /// <summary> Disconnect from Controller </summary>
        protected override void DisconnectFromController()
        {
            if (controller == null) return;
            controller.showHideExecute -= Execute;
        }

        /// <summary> Execute the given ShowHide command </summary>
        protected virtual void Execute(ShowHideExecute execute)
        {
            if (executeCommandCoroutine != null)
            {
                StopCoroutine(executeCommandCoroutine);
                executeCommandCoroutine = null;
            }

            if (!animatorInitialized)
            {
                executeCommandCoroutine = StartCoroutine(ExecuteCommandAfterAnimatorInitialized(execute));
                return;
            }

            switch (execute)
            {
                case ShowHideExecute.Show:
                    Show();
                    return;

                case ShowHideExecute.Hide:
                    Hide();
                    return;

                case ShowHideExecute.InstantShow:
                    InstantShow();
                    return;

                case ShowHideExecute.InstantHide:
                    InstantHide();
                    return;

                case ShowHideExecute.ReverseShow:
                    ReverseShow();
                    return;

                case ShowHideExecute.ReverseHide:
                    ReverseHide();
                    return;

                default:
                    throw new ArgumentOutOfRangeException(nameof(execute), execute, null);
            }
        }

        /// <summary> Execute the given ShowHide command after the animator has been initialized </summary>
        private IEnumerator ExecuteCommandAfterAnimatorInitialized(ShowHideExecute execute)
        {
            yield return new WaitUntil(() => animatorInitialized);
            Execute(execute);
            executeCommandCoroutine = null;
        }
        
        /// <summary> Play the show animation </summary>
        public abstract void Show();
        
        /// <summary> Reverse the show animation (if playing) </summary>
        public abstract void ReverseShow();

        /// <summary> Play the hide animation </summary>
        public abstract void Hide();
        
        /// <summary> Reverse the hide animation (if playing) </summary>
        public abstract void ReverseHide();

        /// <summary> Set show animation's progress at one </summary>
        public abstract void InstantShow();
        
        /// <summary> Set hide animation's progress at one </summary>
        public abstract void InstantHide();
    }
}
