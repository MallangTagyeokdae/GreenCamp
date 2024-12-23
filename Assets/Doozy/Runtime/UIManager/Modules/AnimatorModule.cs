// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Mody;
using Doozy.Runtime.Mody.Actions;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animators.Internal;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.UIManager.Modules
{
    /// <summary> Mody module used to control a list of Reactor animators </summary>
    [AddComponentMenu("Mody/Animator Module")]
    public class AnimatorModule : ModyModule
    {
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/Mody/Animator Module", false, 8)]
        private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        {
            GameObjectUtils.AddToScene<AnimatorModule>("Animator Module", false, true);
        }
        #endif
        
        /// <summary> Default module name </summary>
        public const string k_DefaultModuleName = "Animator";

        /// <summary> Construct an Animator Module with the default name </summary>
        public AnimatorModule() : this(k_DefaultModuleName) {}

        /// <summary> Construct an Animator Module with the given name </summary>
        /// <param name="moduleName"> Module name </param>
        public AnimatorModule(string moduleName) : base(moduleName.IsNullOrEmpty() ? k_DefaultModuleName : moduleName) {}

        /// <summary> All the animators controlled by this module </summary>
        public List<ReactorAnimator> Animators = new List<ReactorAnimator>();

        /// <summary> Simple action to Play forward for all animations </summary>
        public SimpleModyAction PlayForward;
        
        /// <summary> Simple action to Play in reverse for all animations </summary>
        public SimpleModyAction PlayReverse;
        
        /// <summary> Simple action to Stop playing for all animations (does not call Finish) </summary>
        public SimpleModyAction Stop;
        
        /// <summary> Simple action to Finish playing for all animations(also calls Stop) </summary>
        public SimpleModyAction Finish;
        
        /// <summary> Simple action to Reverse all playing animations </summary>
        public SimpleModyAction Reverse;
        
        /// <summary> Simple action to Rewind all animations </summary>
        public SimpleModyAction Rewind;
        
        /// <summary> Simple action to Pause all playing animations </summary>
        public SimpleModyAction Pause;
        
        /// <summary> Simple action to Resume all paused animations </summary>
        public SimpleModyAction Resume;
        
        /// <summary> Float action to set the progress of all animations to a given value </summary>
        public FloatModyAction SetProgressAt;
        
        /// <summary> Simple action to set the progress of all animations to zero </summary>
        public SimpleModyAction SetProgressAtZero;
        
        /// <summary> Simple action to set the progress of all animations to one </summary>
        public SimpleModyAction SetProgressAtOne;
        
        /// <summary> Float action to Play all the animations to a given progress value (from the current value) </summary>
        public FloatModyAction PlayToProgress;
        
        /// <summary> Float action to Play all the animations from a given progress value (to the current value) </summary>
        public FloatModyAction PlayFromProgress;
        
        /// <summary> Simple action to Update the values for all the animations </summary>
        public SimpleModyAction UpdateValues;

        /// <summary> Initialize the actions </summary>
        protected override void SetupActions()
        {
            this.AddAction(PlayForward ??= new SimpleModyAction(this, nameof(PlayForward), ExecutePlayForward));
            this.AddAction(PlayReverse ??= new SimpleModyAction(this, nameof(PlayReverse), ExecutePlayReverse));
            this.AddAction(Stop ??= new SimpleModyAction(this, nameof(Stop), ExecuteStop));
            this.AddAction(Finish ??= new SimpleModyAction(this, nameof(Finish), ExecuteFinish));
            this.AddAction(Reverse ??= new SimpleModyAction(this, nameof(Reverse), ExecuteReverse));
            this.AddAction(Rewind ??= new SimpleModyAction(this, nameof(Rewind), ExecuteRewind));
            this.AddAction(Pause ??= new SimpleModyAction(this, nameof(Pause), ExecutePause));
            this.AddAction(Resume ??= new SimpleModyAction(this, nameof(Resume), ExecuteResume));
            this.AddAction(SetProgressAt ??= new FloatModyAction(this, nameof(SetProgressAt), ExecuteSetProgressAt));
            this.AddAction(SetProgressAtZero ??= new SimpleModyAction(this, nameof(SetProgressAtZero), ExecuteSetProgressAtZero));
            this.AddAction(SetProgressAtOne ??= new SimpleModyAction(this, nameof(SetProgressAtOne), ExecuteSetProgressAtOne));
            this.AddAction(PlayToProgress ??= new FloatModyAction(this, nameof(PlayToProgress), ExecutePlayToProgress));
            this.AddAction(PlayFromProgress ??= new FloatModyAction(this, nameof(PlayFromProgress), ExecutePlayFromProgress));
            this.AddAction(UpdateValues ??= new SimpleModyAction(this, nameof(UpdateValues), ExecuteUpdateValues));
        }

        /// <summary> Remove any null animator references </summary>
        public void CleanAnimatorsList()
        {
            for (int i = Animators.Count - 1; i >= 0; i--)
                if (Animators[i] == null)
                    Animators.RemoveAt(i);
        }

        /// <summary> Execute Play forward for all animations </summary>
        public void ExecutePlayForward()
        {
            CleanAnimatorsList();
            foreach (ReactorAnimator animator in Animators)
                animator.Play(PlayDirection.Forward);
        }

        /// <summary> Execute Play in reverse for all animations </summary>
        public void ExecutePlayReverse()
        {
            CleanAnimatorsList();
            foreach (ReactorAnimator animator in Animators)
                animator.Play(PlayDirection.Reverse);
        }

        /// <summary> Execute Stop for all animations </summary>
        public void ExecuteStop()
        {
            CleanAnimatorsList();
            foreach (ReactorAnimator animator in Animators)
                animator.Stop();
        }

        /// <summary> Execute Finish for all animations </summary>
        public void ExecuteFinish()
        {
            CleanAnimatorsList();
            foreach (ReactorAnimator animator in Animators)
                animator.Finish();
        }

        /// <summary> Execute Reverse for all animations </summary>
        public void ExecuteReverse()
        {
            CleanAnimatorsList();
            foreach (ReactorAnimator animator in Animators)
                animator.Reverse();
        }

        /// <summary> Execute Rewind for all animations </summary>
        public void ExecuteRewind()
        {
            CleanAnimatorsList();
            foreach (ReactorAnimator animator in Animators)
                animator.Rewind();
        }
        
        /// <summary> Execute Pause for all animations </summary>
        public void ExecutePause()
        {
            CleanAnimatorsList();
            foreach (ReactorAnimator animator in Animators)
                animator.Pause();
        }
        
        /// <summary> Execute Resume for all animations </summary>
        public void ExecuteResume()
        {
            CleanAnimatorsList();
            foreach (ReactorAnimator animator in Animators)
                animator.Resume();
        }
        
        /// <summary> Execute SetProgressAt the given value for all animations </summary>
        public void ExecuteSetProgressAt(float value)
        {
            CleanAnimatorsList();
            foreach (ReactorAnimator animator in Animators)
                animator.SetProgressAt(value);
        }
        
        /// <summary> Execute SetProgressAtZero for all animations </summary>
        public void ExecuteSetProgressAtZero()
        {
            CleanAnimatorsList();
            foreach (ReactorAnimator animator in Animators)
                animator.SetProgressAtZero();
        }
        
        /// <summary> Execute SetProgressAtOne for all animations </summary>
        public void ExecuteSetProgressAtOne()
        {
            CleanAnimatorsList();
            foreach (ReactorAnimator animator in Animators)
                animator.SetProgressAtOne();
        }
        
        /// <summary> Execute PlayToProgress to the given value for all animations </summary>
        public void ExecutePlayToProgress(float value)
        {
            CleanAnimatorsList();
            foreach (ReactorAnimator animator in Animators)
                animator.PlayToProgress(value);
        }
        
        /// <summary> Execute PlayFromProgress from the given value for all animations </summary>
        public void ExecutePlayFromProgress(float value)
        {
            CleanAnimatorsList();
            foreach (ReactorAnimator animator in Animators)
                animator.PlayFromProgress(value);
        }
        
        /// <summary> Execute UpdateValues for all animations </summary>
        public void ExecuteUpdateValues()
        {
            CleanAnimatorsList();
            foreach (ReactorAnimator animator in Animators)
                animator.UpdateValues();
        }
    }
}
