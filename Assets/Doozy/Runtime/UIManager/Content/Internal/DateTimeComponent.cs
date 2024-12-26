// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections;
using System.Collections.Generic;
using Doozy.Runtime.Common;
using Doozy.Runtime.Global;
using Doozy.Runtime.Mody;
using UnityEngine;
using UnityEngine.Events;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global

namespace Doozy.Runtime.UIManager.Content.Internal
{
    public abstract class DateTimeComponent : MonoBehaviour
    {
        /// <summary> Minimum value for the update interval </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        protected const float k_MinimumUpdateInterval = 0.001f;

        /// <summary> The timescale mode to use when updating the time </summary>
        public Timescale TimescaleMode = Timescale.Independent;

        [Space(5)]
        [SerializeField] protected float UpdateInterval;
        /// <summary> The interval in seconds between each update </summary>
        public float updateInterval
        {
            get => UpdateInterval;
            set
            {
                UpdateInterval = Mathf.Max(k_MinimumUpdateInterval, value);
                waitRealtime = new WaitForSecondsRealtime(UpdateInterval);
                wait = new WaitForSeconds(UpdateInterval);
            }
        }

        /// <summary>
        /// Behaviour on Start
        /// <para/> What should happen when the component starts (Start method is called)
        /// </summary>
        public TimerBehaviour OnStartBehaviour = TimerBehaviour.Disabled;

        /// <summary>
        /// Behaviour OnEnable
        /// <para/> What should happen when the component is enabled (OnEnable method is called)
        /// </summary>
        public TimerBehaviour OnEnableBehaviour = TimerBehaviour.ResetAndStart;

        /// <summary>
        /// Behaviour OnDisable
        /// <para/> What should happen when the component is disabled (OnDisable method is called)
        /// </summary>
        public TimerBehaviour OnDisableBehaviour = TimerBehaviour.Finish;

        /// <summary>
        /// Behaviour OnDestroy
        /// <para/> What should happen when the component is destroyed (OnDestroy method is called)
        /// </summary>
        public TimerBehaviour OnDestroyBehaviour = TimerBehaviour.Cancel;

        [SerializeField] private List<FormattedLabel> Labels;
        /// <summary>
        /// List of labels that will be updated with relevant time information
        /// </summary>
        public List<FormattedLabel> labels => Labels ?? (Labels = new List<FormattedLabel>());

        /// <summary> Callback triggered when the timer starts </summary>
        public ModyEvent OnStart = new ModyEvent();

        /// <summary>
        /// Callback triggered when the timer starts.
        /// <para/> This is a quick access to the OnStart ModyEvent. </summary>
        public UnityEvent onStartEvent => OnStart.Event;

        /// <summary>
        /// Callback triggered when the timer stops
        /// </summary
        public ModyEvent OnStop = new ModyEvent();

        /// <summary>
        /// Callback triggered when the timer stops.
        /// <para/> This is a quick access to the OnStop ModyEvent. </summary>
        public UnityEvent onStopEvent => OnStop.Event;

        /// <summary>
        /// Callback triggered when the timer reaches the target time
        /// </summary>
        public ModyEvent OnFinish = new ModyEvent();

        /// <summary>
        /// Callback triggered when the timer reaches the target time.
        /// <para/>This is a quick access to the OnFinish ModyEvent.
        /// </summary>
        public UnityEvent onFinishEvent => OnFinish.Event;

        /// <summary>
        /// Callback triggered when the timer is canceled
        /// </summary>
        public ModyEvent OnCancel = new ModyEvent();

        /// <summary>
        /// Callback triggered when the timer is canceled.
        /// <para/>This is a quick access to the OnCancel ModyEvent.
        /// </summary>
        public UnityEvent onCancelEvent => OnFinish.Event;

        /// <summary>
        /// Callback triggered when the timer is paused
        /// </summary>
        public ModyEvent OnPause = new ModyEvent();

        /// <summary>
        /// Callback triggered when the timer is paused.
        /// <para/>This is a quick access to the OnPause ModyEvent.
        /// </summary>
        public UnityEvent onPauseEvent => OnPause.Event;

        /// <summary>
        /// Callback triggered when the timer is resumed
        /// </summary>
        public ModyEvent OnResume = new ModyEvent();

        /// <summary>
        /// Callback triggered when the timer is resumed.
        /// <para/>This is a quick access to the OnResume ModyEvent.
        /// </summary>
        public UnityEvent onResumeEvent => OnResume.Event;

        /// <summary>
        /// Callback triggered when the timer is reset
        /// </summary>
        public ModyEvent OnReset = new ModyEvent();

        /// <summary>
        /// Callback triggered when the timer is reset.
        /// <para/>This is a quick access to the OnReset ModyEvent.
        /// </summary>
        public UnityEvent onResetEvent => OnReset.Event;

        /// <summary>
        /// Callback triggered when the timer is updated
        /// </summary>
        public ModyEvent OnUpdate = new ModyEvent();

        /// <summary>
        /// Callback triggered when the timer is updated.
        /// <para/>This is a quick access to the OnUpdate ModyEvent.
        /// </summary>
        public UnityEvent onUpdateEvent => OnUpdate.Event;

        /// <summary> Returns TRUE if this timer has at least one callback assigned </summary>
        public bool hasCallbacks =>
            OnStart.hasCallbacks ||
            OnStop.hasCallbacks ||
            OnFinish.hasCallbacks ||
            OnCancel.hasCallbacks ||
            OnPause.hasCallbacks ||
            OnResume.hasCallbacks ||
            OnReset.hasCallbacks ||
            OnUpdate.hasCallbacks;

        [SerializeField] protected int Years;
        /// <summary> Years </summary>
        public int years => Years;

        [SerializeField] protected int Months;
        /// <summary> Months </summary>
        public int months => Months;

        [SerializeField] protected int Days;
        /// <summary> Days </summary>
        public int days => Days;

        [SerializeField] protected int Hours;
        /// <summary> Hours </summary>
        public int hours => Hours;

        [SerializeField] protected int Minutes;
        /// <summary> Minutes </summary>
        public int minutes => Minutes;

        [SerializeField] protected int Seconds;
        /// <summary> Seconds </summary>
        public int seconds => Seconds;

        [SerializeField] protected int Milliseconds;
        /// <summary> Milliseconds </summary>
        public int milliseconds => Milliseconds;

        /// <summary> Start time of the timer (when is running) </summary>
        public DateTime startTime { get; protected set; }

        /// <summary> Current time of the timer (when is running) </summary>
        public DateTime currentTime { get; protected set; }

        /// <summary> End time of the timer (when is running) </summary>
        public DateTime endTime { get; protected set; }

        /// <summary> Elapsed time since the timer started (when is running) </summary>
        public TimeSpan elapsedTime { get; protected set; }

        /// <summary> Remaining time until currentTime reaches endTime (when is running) </summary>
        public TimeSpan remainingTime { get; protected set; }

        /// <summary> Returns TRUE if the time update is running </summary>
        public bool isRunning
        {
            get;
            protected set;
        }

        /// <summary> Returns TRUE if the time update is running, but it's paused </summary>
        public bool isPaused { get; private set; }

        /// <summary> Returns TRUE if the time update has finished </summary>
        protected bool isFinished => remainingTime.TotalMilliseconds <= 0;

        /// <summary>
        /// Keeps track of the last time the time was updated,
        /// when the TimeScaleMode is set to 'Dependent'.
        /// This value is used to calculate the lastDeltaTime value.
        /// </summary>
        protected double lastTime { get; set; }
        /// <summary>
        /// Keeps track of the last time the time was updated,
        /// when the TimeScaleMode is set to 'Independent'.
        /// This value is used to calculate the lastUnscaledDeltaTime value.
        /// </summary>
        protected double lastUnscaledTime { get; set; }

        /// <summary>
        /// The time that has passed since the last time the time was updated,
        /// when the TimeScaleMode is set to 'Dependent'
        /// </summary>
        protected double lastDeltaTime => Time.timeAsDouble - lastTime;

        /// <summary>
        /// The time that has passed since the last time the time was updated,
        /// when the TimeScaleMode is set to 'Independent'
        /// </summary>
        protected double lastUnscaledDeltaTime => Time.realtimeSinceStartupAsDouble - lastUnscaledTime;

        /// <summary> Flag used to track if the update interval has changed when this component is running </summary>
        protected float previousUpdateInterval { get; set; }

        /// <summary>
        /// Custom YieldInstruction that waits for the specified amount of seconds (unscaled time)
        /// This is used to lower the GC allocation of the time update coroutine
        /// </summary>
        protected WaitForSecondsRealtime waitRealtime { get; set; }

        /// <summary>
        /// Custom YieldInstruction that waits for the specified amount of seconds (scaled by the time scale).
        /// This is used to lower the GC allocation of the time update coroutine
        /// </summary>
        protected WaitForSeconds wait { get; set; }

        /// <summary> Coroutine that updates the timer </summary>
        protected Coroutine updateCoroutine { get; set; }

        #if UNITY_EDITOR
        protected virtual void Reset()
        {
            TimescaleMode = Timescale.Independent;
            isRunning = false;
            isPaused = false;
            updateInterval = 0.1f;
        }
        #endif // UNITY_EDITOR

        protected virtual void Awake()
        {
            startTime = DateTime.Now;
            currentTime = DateTime.Now;
            endTime = DateTime.Now;

            isRunning = false;
            isPaused = false;
            updateInterval = UpdateInterval;
        }

        protected void Start()
        {
            RunBehaviour(OnStartBehaviour);
        }

        protected virtual void OnEnable()
        {
            //make sure the update interval is not 0 or less
            updateInterval = UpdateInterval;
            RunBehaviour(OnEnableBehaviour);
        }

        protected virtual void OnDisable()
        {
            switch (OnDisableBehaviour)
            {
                case TimerBehaviour.Disabled:
                case TimerBehaviour.Stop:
                case TimerBehaviour.StopAndReset:
                case TimerBehaviour.Pause:
                case TimerBehaviour.Reset:
                case TimerBehaviour.Finish:
                case TimerBehaviour.Cancel:
                    RunBehaviour(OnDisableBehaviour);
                    break;
                case TimerBehaviour.Start:
                case TimerBehaviour.Resume:
                case TimerBehaviour.ResetAndStart:
                    Debug.LogWarning
                    (
                        $"[{name}][{GetType().Name}] OnDisable Behaviour is set to '{OnDisableBehaviour}'. " +
                        $"This doesn't make sense. " +
                        $"Doing nothing."
                    );
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected virtual void OnDestroy()
        {
            switch (OnDisableBehaviour)
            {
                case TimerBehaviour.Disabled:
                case TimerBehaviour.Stop:
                case TimerBehaviour.Finish:
                case TimerBehaviour.Cancel:
                    RunBehaviour(OnDisableBehaviour);
                    break;
                case TimerBehaviour.StopAndReset:
                case TimerBehaviour.Pause:
                case TimerBehaviour.Reset:
                case TimerBehaviour.Start:
                case TimerBehaviour.Resume:
                case TimerBehaviour.ResetAndStart:
                    Debug.LogWarning
                    (
                        $"[{name}][{GetType().Name}] OnDisable Behaviour is set to '{OnDisableBehaviour}'. " +
                        $"This doesn't make sense. " +
                        $"Doing nothing."
                    );
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            RunBehaviour(OnDestroyBehaviour);
            StopUpdateCoroutine();
        }

        protected virtual void OnApplicationPause(bool pauseStatus)
        {
            RunBehaviour
            (
                pauseStatus
                    ? TimerBehaviour.Pause
                    : TimerBehaviour.Resume
            );
        }

        /// <summary>
        /// Coroutine responsible for updating the current time
        /// </summary>
        protected virtual IEnumerator TimeUpdateCoroutine()
        {
            waitRealtime ??= new WaitForSecondsRealtime(UpdateInterval);
            wait ??= new WaitForSeconds(UpdateInterval);
            previousUpdateInterval = UpdateInterval;

            while (isRunning)
            {
                if (isPaused)
                {
                    yield return null;
                    lastTime = Time.timeAsDouble;
                    lastUnscaledTime = (float)Time.realtimeSinceStartupAsDouble;
                    continue;
                }

                //check if the update interval has changed
                if (Math.Abs(previousUpdateInterval - UpdateInterval) > 0.001f)
                {
                    waitRealtime = new WaitForSecondsRealtime(UpdateInterval);
                    wait = new WaitForSeconds(UpdateInterval);
                    previousUpdateInterval = UpdateInterval;
                }

                switch (TimescaleMode)
                {
                    case Timescale.Independent:
                        yield return waitRealtime;
                        break;
                    case Timescale.Dependent:
                        yield return wait;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                UpdateCurrentTime();
                OnUpdate.Execute();
                if (currentTime < endTime) continue;
                isRunning = false;
                OnFinish?.Execute();
            }
        }

        /// <summary>
        /// Set the start time value for this timer
        /// </summary>
        protected virtual void SetStartTime()
        {
            startTime = DateTime.Now;
            UpdateLastTime();
        }


        /// <summary>
        /// Set the end time value for this timer
        /// </summary>
        protected virtual void SetEndTime()
        {
            endTime =
                startTime
                    .AddYears(Years)
                    .AddMonths(Months)
                    .AddDays(Days)
                    .AddHours(Hours)
                    .AddMinutes(Minutes)
                    .AddSeconds(Seconds)
                    .AddMilliseconds(Milliseconds);
        }

        /// <summary>
        /// Update the current time, the elapsed time and the remaining time
        /// </summary>
        protected virtual void UpdateCurrentTime()
        {
            switch (TimescaleMode)
            {
                case Timescale.Independent:
                    currentTime = currentTime.AddMilliseconds(lastUnscaledDeltaTime * 1000);
                    break;
                case Timescale.Dependent:
                    currentTime = currentTime.AddMilliseconds(lastDeltaTime * 1000);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            elapsedTime = currentTime.Subtract(startTime);
            remainingTime = endTime.Subtract(currentTime);

            UpdateLastTime();
        }

        public virtual void UpdateLabels()
        {
            for (int i = 0; i < labels.Count; i++)
            {
                if (labels[i].Label == null) continue;
                labels[i].SetText(currentTime);
            }
        }

        /// <summary>
        /// Reset the timer and call the OnReset event.
        /// <para/> If the timer is running, it will be stopped and the OnStop event will be called before the OnReset event.
        /// </summary>
        public virtual void ResetTimer()
        {
            updateInterval = UpdateInterval;

            StopUpdateCoroutine();

            isRunning = false;
            isPaused = false;

            OnReset?.Execute();

            SetStartTime();
            SetEndTime();
            currentTime = startTime;
            elapsedTime = TimeSpan.Zero;
            remainingTime = endTime - startTime;
        }

        /// <summary>
        /// Start the timer and call the OnStart event.
        /// <para/> If the timer is already running and is paused, it will resume the timer.
        /// <para/> It does nothing if the timer is already running and is not paused.
        /// </summary>
        public virtual void StartTimer()
        {
            if (isPaused)
            {
                ResumeTimer();
                return;
            }

            if (isRunning) return;
            SetStartTime();
            SetEndTime();
            currentTime = startTime;
            elapsedTime = TimeSpan.Zero;
            remainingTime = endTime - startTime;
            OnStart?.Execute();
            isRunning = true;
            UpdateCurrentTime();
            if (!isActiveAndEnabled) return;
            StartUpdateCoroutine();
        }

        /// <summary>
        /// Stop the timer and call the OnStop event.
        /// <para/> It does nothing if the timer is not running.
        /// </summary>
        public virtual void StopTimer()
        {
            StopUpdateCoroutine();
            if (!isRunning) return;
            OnStop?.Execute();
            isRunning = false;
            isPaused = false;
        }

        /// <summary>
        /// Pause the timer and call the OnPause event.
        /// <para/> It does nothing if the timer is not running or if it's already paused.
        /// </summary>
        public virtual void PauseTimer()
        {
            if (!isRunning || isPaused) return;
            OnPause?.Execute();
            isPaused = true;
        }

        /// <summary>
        /// Resume the timer, from a paused state, and call the OnResume event.
        /// <para/> It does nothing if the timer is not running and not paused.
        /// </summary>
        public virtual void ResumeTimer()
        {
            if (!isRunning) return;
            if (!isPaused) return;
            OnResume?.Execute();
            isPaused = false;
        }

        /// <summary>
        /// Stop the timer, call the OnStop event, and then call the OnFinish event.
        /// <para/> It does nothing if the timer is not running.
        /// </summary>
        public virtual void FinishTimer()
        {
            StopUpdateCoroutine();
            currentTime = endTime;
            UpdateCurrentTime();
            StopTimer();
            OnFinish?.Execute();
        }

        /// <summary>
        /// Cancel the timer and trigger the OnCancel event.
        /// <para/> It does nothing if the timer is not running.
        /// </summary>
        public virtual void CancelTimer()
        {
            StopUpdateCoroutine();
            if (isRunning) OnCancel?.Execute();
            isRunning = false;
            isPaused = false;
        }

        /// <summary> Runs the given timer behavior. </summary>
        /// <param name="behaviour"> The timer behavior to run. </param>
        protected virtual void RunBehaviour(TimerBehaviour behaviour)
        {
            // if (!isActiveAndEnabled) return;

            switch (behaviour)
            {
                case TimerBehaviour.Disabled:
                    //do nothing
                    break;
                case TimerBehaviour.Start:
                    StartTimer();
                    break;
                case TimerBehaviour.Stop:
                    StopTimer();
                    break;
                case TimerBehaviour.ResetAndStart:
                    ResetTimer();
                    StartTimer();
                    break;
                case TimerBehaviour.StopAndReset:
                    StopTimer();
                    ResetTimer();
                    break;
                case TimerBehaviour.Pause:
                    PauseTimer();
                    break;
                case TimerBehaviour.Resume:
                    ResumeTimer();
                    break;
                case TimerBehaviour.Reset:
                    ResetTimer();
                    break;
                case TimerBehaviour.Finish:
                    FinishTimer();
                    break;
                case TimerBehaviour.Cancel:
                    CancelTimer();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(behaviour), behaviour, null);
            }
        }

        /// <summary> Update the last time values </summary>
        protected void UpdateLastTime()
        {
            lastTime = Time.timeAsDouble;
            lastUnscaledTime = Time.realtimeSinceStartupAsDouble;
        }

        /// <summary> Start the update coroutine </summary>
        protected void StartUpdateCoroutine()
        {
            StopUpdateCoroutine();
            updateCoroutine = Coroutiner.Start(TimeUpdateCoroutine());
        }

        /// <summary> Stop the update coroutine </summary>
        protected void StopUpdateCoroutine()
        {
            if (updateCoroutine == null) return;
            Coroutiner.Stop(updateCoroutine);
        }

    }
}
