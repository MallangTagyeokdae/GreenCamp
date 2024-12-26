// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.Events;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Mody;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.Reactor.Ticker;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global

namespace Doozy.Runtime.Reactor
{
    /// <summary>
    /// Specialized Reactor class that allows you animate a float value over time to be used as a progress bar or indicator.
    /// </summary>
    [AddComponentMenu("Reactor/Progressor")]
    public partial class Progressor : MonoBehaviour
    {
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/Reactor/Progressor", false, 6)]
        private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        {
            GameObjectUtils.AddToScene<Progressor>(false, true);
        }
        #endif

        public static HashSet<Progressor> database { get; private set; } = new HashSet<Progressor>();

        [ExecuteOnReload]
        private static void OnReload()
        {
            database = new HashSet<Progressor>();
        }

        [SerializeField] private List<ProgressTarget> ProgressTargets;
        /// <summary> Progress targets that get updated by this Progressor when activated </summary>
        public List<ProgressTarget> progressTargets
        {
            get
            {
                Initialize();
                return ProgressTargets;
            }
        }

        [SerializeField] private List<Progressor> ProgressorTargets;
        /// <summary> Other Progressors that get updated by this Progressor when activated </summary>
        public List<Progressor> progressorTargets
        {
            get
            {
                Initialize();
                return ProgressorTargets;
            }
        }

        [SerializeField] protected float FromValue = 0f;
        /// <summary> Start Value </summary>
        public float fromValue
        {
            get => FromValue;
            set
            {
                FromValue = value;
                if (reaction.isActive) reaction.SetFrom(fromValue);
            }
        }

        [SerializeField] protected float ToValue = 1f;
        /// <summary> End Value </summary>
        public float toValue
        {
            get => ToValue;
            set
            {
                ToValue = value;
                if (reaction.isActive) reaction.SetTo(toValue);
            }
        }

        [SerializeField] protected float CurrentValue;
        /// <summary> Current Value </summary>
        public float currentValue => CurrentValue;

        [SerializeField] protected float Progress;
        /// <summary> Current Progress </summary>
        public float progress => Progress;

        [SerializeField] protected float CustomResetValue;
        /// <summary> Custom Reset Value </summary>
        public float customResetValue
        {
            get => CustomResetValue;
            set => CustomResetValue = Mathf.Clamp(value, FromValue, ToValue);
        }

        [SerializeField] protected FloatReaction Reaction;
        /// <summary> Reaction that runs this progressor </summary>
        public FloatReaction reaction
        {
            get
            {
                Initialize();
                return Reaction;
            }
        }

        /// <summary> OnEnable behaviour for this Progressor </summary>
        public ResetValue ResetValueOnEnable = ResetValue.Disabled;

        /// <summary>
        /// Fired when the value changed.
        /// Returns the current value.
        /// </summary>
        public FloatEvent OnValueChanged;

        /// <summary>
        /// Fired when the progress changed.
        /// Returns the current progress.
        /// </summary>
        public FloatEvent OnProgressChanged;

        /// <summary>
        /// Fired when the value increases.
        /// Returns the difference between the new value and the previous value.
        /// <para/> Example: if the previous value was 0.5 and the new value is 0.7, the returned value will be 0.2
        /// </summary>
        public FloatEvent OnValueIncremented = new FloatEvent();

        /// <summary>
        /// Fired when the value decreases.
        /// Returns the difference between the new value and the previous value.
        /// <para/> Example: if the previous value was 10 and the new value is 5, the returned value will be -5
        /// </summary>
        public FloatEvent OnValueDecremented = new FloatEvent();

        /// <summary> Fired when the value was reset </summary>
        public ModyEvent OnValueReset = new ModyEvent();

        /// <summary> Fired when the value has reached the fromValue </summary>
        public ModyEvent OnValueReachedFromValue = new ModyEvent();

        /// <summary> Fired when the value has reached the toValue </summary>
        public ModyEvent OnValueReachedToValue = new ModyEvent();

        public bool initialized
        {
            get;
            set;
        }

        public ProgressorId Id;

        protected Progressor()
        {
            Id = new ProgressorId();
        }

        public virtual void Initialize()
        {
            if (initialized) return;
            ProgressTargets ??= new List<ProgressTarget>();
            ProgressorTargets ??= new List<Progressor>();
            Reaction = Reaction ?? ReactionPool.Get<FloatReaction>();
            Reaction.SetFrom(fromValue);
            Reaction.SetTo(toValue);
            Reaction.SetValue(fromValue);
            Reaction.OnUpdateCallback = UpdateProgressor;
            initialized = true;
        }

        protected virtual void Awake()
        {
            if (!Application.isPlaying)
                return;

            database.Add(this);
            Initialize();
            
            // OnValueChanged.AddListener(newValue => Debug.Log($"[{name}] OnValueChanged: {newValue}"));
            // OnProgressChanged.AddListener(newProgress => Debug.Log($"[{name}] OnProgressChanged: {newProgress}"));
            // OnValueIncremented.AddListener(increment => Debug.Log($"[{name}] OnValueIncremented: {increment}"));
            // OnValueDecremented.AddListener(decrement => Debug.Log($"[{name}] OnValueDecremented: {decrement}"));
            // OnValueReset.Event.AddListener(() => Debug.Log($"[{name}] OnValueReset"));
            // OnValueReachedFromValue.Event.AddListener(() => Debug.Log($"[{name}] OnValueReachedFromValue"));
            // OnValueReachedToValue.Event.AddListener(() => Debug.Log($"[{name}] OnValueReachedToValue"));
        }

        protected virtual void OnEnable()
        {
            if (!Application.isPlaying)
                return;

            CleanDatabase();
            ValidateTargets();
            ResetCurrentValue(ResetValueOnEnable);
        }

        protected virtual void OnDisable()
        {
            if (!Application.isPlaying)
                return;

            CleanDatabase();
            ValidateTargets();
            reaction.Stop();
        }

        protected void OnDestroy()
        {
            if (!Application.isPlaying)
                return;

            database.Remove(this);
            CleanDatabase();

            Reaction?.Recycle();
        }

        /// <summary> Remove null and duplicate targets </summary>
        private void ValidateTargets() =>
            ProgressTargets = progressTargets.Where(t => t != null).Distinct().ToList();

        /// <summary> Reset the Progressor to the given reset value </summary>
        /// <param name="resetValue"> Value to reset to </param>
        protected void ResetCurrentValue(ResetValue resetValue)
        {
            if (resetValue == ResetValue.Disabled) return;
            reaction.SetFrom(FromValue);
            reaction.SetTo(ToValue);
            switch (resetValue)
            {
                case ResetValue.FromValue:
                    SetProgressAtZero();
                    OnValueReset?.Execute();
                    break;
                case ResetValue.EndValue:
                    SetProgressAtOne();
                    OnValueReset?.Execute();
                    break;
                case ResetValue.CustomValue:
                    SetProgressAt(reaction.GetProgressAtValue(CustomResetValue));
                    OnValueReset?.Execute();
                    break;
                case ResetValue.Disabled:
                default:
                    throw new ArgumentOutOfRangeException(nameof(resetValue), resetValue, null);
            }
        }

        /// <summary> Reset all the reactions to their initial values (if the animation is enabled) </summary>
        public void ResetToStartValues()
        {
            if (reaction.isActive) Stop();

            ResetCurrentValue(ResetValueOnEnable);

            #if UNITY_EDITOR
            if (this == null) return;
            foreach (ProgressTarget progressTarget in progressTargets)
            {
                if (progressTarget == null) continue;
                UnityEditor.EditorUtility.SetDirty(progressTarget);
            }
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.SceneView.RepaintAll();
            #endif
        }

        /// <summary> Update current value and trigger callbacks </summary>
        public virtual void UpdateProgressor()
        {
            float previousValue = CurrentValue;
            
            CurrentValue = reaction.currentValue;
            Progress = Mathf.InverseLerp(fromValue, toValue, currentValue);

            if (previousValue < CurrentValue)
            {
                OnValueIncremented?.Invoke(CurrentValue - previousValue);
            }
            else if (previousValue > CurrentValue)
            {
                OnValueDecremented?.Invoke(previousValue - CurrentValue);
            }
            
            OnValueChanged?.Invoke(CurrentValue);
            OnProgressChanged?.Invoke(Progress);
            
            if (currentValue.Approximately(fromValue))
            {
                //value reached fromValue -> invoke the OnValueReachedFromValue event
                OnValueReachedFromValue?.Execute();
            }
            
            if (currentValue.Approximately(toValue))
            {
                //value reached toValue -> invoke the OnValueReachedToValue event
                OnValueReachedToValue?.Execute();
            }

            ProgressTargets.RemoveNulls();
            ProgressTargets.ForEach(t => t.UpdateTarget(this));

            ProgressorTargets.RemoveNulls();
            for (int i = ProgressorTargets.Count - 1; i >= 0; i--)
            {
                if (progressorTargets[i] == this)
                    ProgressorTargets.RemoveAt(i);
            }
            ProgressorTargets.ForEach(p => p.SetProgressAt(Progress));
        }

        /// <summary> Set the Progressor's current value to the given target value </summary>
        /// <param name="value"> Target value </param>
        public void SetValueAt(float value)
        {
            SetProgressAt(reaction.GetProgressAtValue(Mathf.Clamp(value, fromValue, toValue)));
        }

        /// <summary> Set the Progressor's current progress to the given target progress </summary>
        /// <param name="targetProgress"> Target progress </param>
        public void SetProgressAt(float targetProgress)
        {
            reaction.SetFrom(FromValue);
            reaction.SetTo(ToValue);
            reaction.SetProgressAt(targetProgress);
            UpdateProgressor();
        }

        /// <summary> Set the Progressor's current progress to 1 (100%) </summary>
        public void SetProgressAtOne() =>
            SetProgressAt(1f);

        /// <summary> Set the Progressor's current progress to 0 (0%) </summary>
        public void SetProgressAtZero() =>
            SetProgressAt(0f);

        /// <summary> Play from start (from value) to end (to value), depending on the given PlayDirection </summary>
        /// <param name="direction"> Play direction </param>
        public void Play(PlayDirection direction) =>
            Play(direction == PlayDirection.Reverse);

        /// <summary> Play from the given start (from value) to end (to value), or in reverse </summary>
        /// <param name="inReverse"> Play in reverse? </param>
        public void Play(bool inReverse = false)
        {
            reaction.SetValue(inReverse ? ToValue : FromValue);
            reaction.Play(FromValue, ToValue, inReverse);
        }

        /// <summary> Play from the current value to the given target value </summary>
        /// <param name="value"> Target value </param>
        public void PlayToValue(float value)
        {
            value = Mathf.Clamp(value, fromValue, toValue); //clamp the value

            if (Math.Abs(value - fromValue) < 0.001f)
            {
                PlayToProgress(0f);
                return;
            }

            if (Math.Abs(value - toValue) < 0.001f)
            {
                PlayToProgress(1f);
                return;
            }

            PlayToProgress(Mathf.InverseLerp(fromValue, toValue, value));
        }

        /// <summary> Play from the current progress to the given target progress </summary>
        /// <param name="toProgress"> Target progress </param>
        public void PlayToProgress(float toProgress)
        {
            float p = Mathf.Clamp01(toProgress); //clamp the progress

            switch (p)
            {
                case 0:
                    reaction.Play(currentValue, fromValue);
                    break;
                case 1:
                    reaction.Play(currentValue, toValue);
                    break;
                default:
                    reaction.Play(currentValue, Mathf.Lerp(fromValue, toValue, p));
                    break;
            }
        }

        /// <summary> Stop the Progressor from playing </summary>
        public void Stop() =>
            reaction.Stop();

        /// <summary> Reverse the Progressor's playing direction (works only if the Progressor is playing) </summary>
        public void Reverse() =>
            reaction.Reverse();

        /// <summary> Rewind the Progressor to 0% if playing forward or to 100% if playing in reverse </summary>
        public void Rewind() =>
            reaction.Rewind();

        /// <summary>
        /// Progressor start delay.
        /// <para/>
        /// If the Progressor is active, it will return the current start delay, otherwise it returns the value from settings.
        /// <para/>
        /// For random, it returns the maximum value from settings.
        /// </summary>
        public float GetStartDelay() =>
            reaction.isActive ? reaction.startDelay : reaction.settings.GetStartDelay();

        /// <summary>
        /// Progressor duration.
        /// <para/>
        /// If the Progressor is active, it will return the current duration, otherwise it returns the value from settings.
        /// <para/>
        /// For random, it returns the maximum value from settings.
        /// </summary>
        public float GetDuration() =>
            reaction.isActive ? reaction.duration : reaction.settings.GetDuration();

        /// <summary>
        /// Progressor start delay + duration.
        /// <para/>
        /// If the Progressor is active, it will return the current start delay + current duration, otherwise it returns the summed values from settings.
        /// <para/>
        /// For random, it returns the maximum value from settings.
        /// </summary>
        public float GetTotalDuration() =>
            GetStartDelay() + GetDuration();

        /// <summary> Set animation heartbeat </summary>
        public List<Heartbeat> SetHeartbeat<T>() where T : Heartbeat, new()
        {
            var list = new List<Heartbeat>() { new T() };
            reaction.SetHeartbeat(list[0]);
            return list;
        }

        #region Static Methods

        /// <summary> Remove all null references from the database </summary>
        protected static void CleanDatabase() =>
            database.Remove(null);

        /// <summary> Get all the registered progressors with the given category and name </summary>
        /// <param name="category"> Progressor category </param>
        /// <param name="name"> Progressor name (from the given category) </param>
        public static IEnumerable<Progressor> GetProgressors(string category, string name) =>
            database.Where(p => p.Id.Category.Equals(category)).Where(button => button.Id.Name.Equals(name));

        /// <summary> Get all the registered progressors with the given category </summary>
        /// <param name="name"> Progressor category </param>
        public static IEnumerable<Progressor> GetAllProgressorsInCategory(string name) =>
            database.Where(p => p.Id.Category.Equals(name));

        /// <summary> Get all the registered progressors with the given name (regardless of their category) </summary>
        /// <param name="name"> Progressor name (from the given category) </param>
        public static IEnumerable<Progressor> GetAllProgressorsByName(string name) =>
            database.Where(p => p.Id.Name.Equals(name));

        #endregion
    }
}
