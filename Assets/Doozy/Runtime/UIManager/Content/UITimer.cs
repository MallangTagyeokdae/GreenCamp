// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Events;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Mody;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.UIManager.Content.Internal;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Mathf;

namespace Doozy.Runtime.UIManager.Content
{
    /// <summary>
    /// The UITimer component can be used to trigger a callback after a certain amount of time has passed.
    /// It works like a countdown timer, where the timer starts at the given duration and counts down to zero.
    /// It's also possible to set the timer to loop, so that it will trigger the callback every time the timer reaches the target time.
    /// It works with both real time (unscaled time) and game time (scaled time).
    /// </summary>
    [AddComponentMenu("UI/DateTime/UI Timer")]
    public class UITimer : DateTimeComponent
    {
        /// <summary>
        /// Callback triggered when the timer loops.
        /// <para/> This means that the timer has reached the target time and it will start counting down again.
        /// </summary>
        public ModyEvent OnLoop = new ModyEvent();

        /// <summary>
        /// Callback triggered when the timer loops.
        /// <para/> This means that the timer has reached the target time and it will start counting down again.
        /// <para/> This is a quick access to the OnLoop ModyEvent.
        /// </summary>
        public UnityEvent onLoopEvent => OnLoop.Event;

        /// <summary> Callback triggered when the timer is updated </summary>
        public FloatEvent OnProgressChanged = new FloatEvent();

        [SerializeField] private Progressor TargetProgressor;
        /// <summary> Reference to a Progressor that will be updated when the timer is updated </summary>
        public Progressor targetProgressor
        {
            get => TargetProgressor;
            set => TargetProgressor = value;
        }

        /// <summary>
        /// When true, the timer will update the target progressor value with SetProgressAt instead of PlayToProgress.
        /// <para/> Basically, if true, the progressor will not animate to the new value when the timer is updated. 
        /// </summary>
        public bool InstantProgressorUpdate = true;

        [SerializeField] private int Loops;
        /// <summary>
        /// Number of times the timer restarts after it reaches the target time before it stops and completes.
        /// <para/> -1 - infinite loops
        /// <para/> 0 - no loops (plays once)
        /// <para/> > 0 - replays (restarts) for the given number of loops
        /// </summary>
        public int loops
        {
            get => Loops;
            set => Loops = Max(-1, value);
        }

        [SerializeField] private float LoopDelay;
        /// <summary>
        /// Delay between loops time interval
        /// <para/> How long to wait before the timer starts counting down again after it reaches the target time
        /// </summary>
        public float loopDelay
        {
            get => LoopDelay;
            set => LoopDelay = Max(0, value);
        }

        /// <summary>
        /// Flag that indicates if the timer is currently waiting for the loop delay to end before it starts counting down again.
        /// </summary>
        public bool inLoopDelay { get; protected set; }

        /// <summary>
        /// Internal DateTime used to keep track of the time the timer needs to wait before it starts counting down again. 
        /// </summary>
        public DateTime loopDelayEndTime { get; protected set; }

        /// <summary> Timer progress value (0-1) </summary>
        public float progress =>
            isFinished
                ? 1f
                : ((float)elapsedTime.TotalMilliseconds / (float)endTime.Subtract(startTime).TotalMilliseconds).Round(4);

        #if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            if (labels.Count == 0) labels.Add(new FormattedLabel(null, @"mm\:ss\.f"));
        }
        #endif // UNITY_EDITOR

        protected override void Awake()
        {
            base.Awake();
            inLoopDelay = false;
        }

        protected override void UpdateCurrentTime()
        {
            base.UpdateCurrentTime();
            if (inLoopDelay) return;
            UpdateProgress(progress);
            UpdateLabels();
        }

        public override void UpdateLabels()
        {
            for (int i = 0; i < labels.Count; i++)
            {
                if (labels[i].Label == null) continue;
                labels[i].SetText(remainingTime);
            }
        }

        public override void ResetTimer()
        {
            base.ResetTimer();
            inLoopDelay = false;
            UpdateProgress(0f);
            UpdateLabels();
        }

        public override void StartTimer()
        {
            inLoopDelay = false;
            base.StartTimer();
            UpdateProgress(0f);
            UpdateLabels();
        }

        public override void StopTimer()
        {
            inLoopDelay = false;
            base.StopTimer();
            UpdateProgress(progress);
            UpdateLabels();
        }

        public override void PauseTimer()
        {
            base.PauseTimer();
            UpdateProgress(progress);
            UpdateLabels();
        }

        public override void ResumeTimer()
        {
            base.ResumeTimer();
            UpdateProgress(progress);
            UpdateLabels();
        }

        public override void FinishTimer()
        {
            base.FinishTimer();
            UpdateProgress(1f);
            UpdateLabels();
        }

        public override void CancelTimer()
        {
            inLoopDelay = false;
            base.CancelTimer();
            UpdateLabels();
        }

        private void UpdateProgress(float newProgress)
        {
            OnProgressChanged?.Invoke(newProgress);

            if (targetProgressor == null) return;
            if (InstantProgressorUpdate)
                targetProgressor.SetProgressAt(newProgress);
            else
                targetProgressor.PlayToProgress(newProgress);
        }

        protected override IEnumerator TimeUpdateCoroutine()
        {
            waitRealtime ??= new WaitForSecondsRealtime(UpdateInterval); //create a new wait object if it's null
            wait ??= new WaitForSeconds(UpdateInterval);                 //create a new wait object if it's null
            previousUpdateInterval = UpdateInterval;                     //set the previous update interval to the current update interval

            int loopCount = loops;

            while (isRunning)
            {
                if (isPaused) //wait for the timer to be resumed
                {
                    yield return null;                                           //wait for the next frame
                    lastTime = Time.timeAsDouble;                                //reset the last time
                    lastUnscaledTime = (float)Time.realtimeSinceStartupAsDouble; //reset the last unscaled time
                    continue;                                                    //skip the rest of the loop
                }

                //check if the update interval has changed
                if (Math.Abs(previousUpdateInterval - UpdateInterval) > 0.001f)
                {
                    waitRealtime = new WaitForSecondsRealtime(UpdateInterval); //create a new WaitForSecondsRealtime with the new update interval
                    wait = new WaitForSeconds(UpdateInterval);                 //create a new WaitForSeconds with the new update interval
                    previousUpdateInterval = UpdateInterval;                   //update the previous update interval
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

                UpdateCurrentTime(); //update the current time

                if (!inLoopDelay)       //if the timer is not in loop delay
                    OnUpdate.Execute(); //trigger the OnUpdate event

                if (currentTime < endTime)
                    continue; //if the current time is less than the end time, skip the rest of the loop

                if (loops < 0 | (loops > 0 && loopCount > 0)) //if the timer is set to loop
                {
                    if (loopDelay > 0)
                    {
                        if (!inLoopDelay)
                        {
                            loopDelayEndTime = endTime.AddSeconds(loopDelay);
                        }

                        if (currentTime < loopDelayEndTime)
                        {
                            inLoopDelay = true;
                            continue;
                        }

                        inLoopDelay = false;
                    }

                    loopCount--;                         //count the loop
                    OnLoop?.Execute();                   //trigger the loop event
                    SetStartTime();                      //reset the start time
                    SetEndTime();                        //reset the end time
                    currentTime = startTime;             //reset the current time
                    elapsedTime = TimeSpan.Zero;         //reset the elapsed time
                    remainingTime = endTime - startTime; //reset the remaining time
                    UpdateCurrentTime();                 //update the current time
                    continue;                            //continue the loop
                }

                isRunning = false;   //stop the timer
                OnFinish?.Execute(); //trigger the OnComplete event
            }
        }
    }
}
