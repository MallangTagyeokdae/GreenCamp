// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Runtime.Common;
using Doozy.Runtime.Mody;
using Doozy.Runtime.UIManager.Content.Internal;
using UnityEngine.Events;

namespace Doozy.Runtime.UIManager.Content
{
    /// <summary>
    /// The UIStopwatch component can be used to measure elapsed time and trigger events at specific intervals (laps).
    /// It works like a stopwatch, and it can be paused and resumed at any time.
    /// It works with both real time (unscaled time) and game time (scaled time).
    /// </summary>
    public class UIStopwatch : DateTimeComponent
    {
        [Serializable]
        public struct Lap
        {
            public int lapNumber { get; private set; }
            public TimeSpan lapTime { get; private set; }
            public TimeSpan lapDuration { get; private set; }

            public Lap(int lapNumber, TimeSpan lapTime, TimeSpan lapDuration)
            {
                this.lapNumber = lapNumber;
                this.lapTime = lapTime;
                this.lapDuration = lapDuration;
            }
        }

        /// <summary>
        /// Current lap index (starts at 0 for the first lap)
        /// </summary>
        public int currentLapIndex { get; protected set; }

        /// <summary>
        /// Current lap number (starts at 1 for the first lap)
        /// </summary>
        public int currentLapNumber => currentLapIndex + 1;

        /// <summary> Keeps track of the added laps </summary>
        public List<Lap> laps { get; protected set; }

        /// <summary> Callback triggered when a new lap is added </summary>
        public ModyEvent OnLap = new ModyEvent();

        /// <summary>
        /// Callback triggered when a new lap is added
        /// <para/> This is a quick access to the OnLap ModyEvent
        /// </summary>
        public UnityEvent onLapEvent => OnLap.Event;

        /// <summary> Keeps track of the last time a lap was added </summary>
        private TimeSpan previousLapTime { get; set; }

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
            laps ??= new List<Lap>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            StartTimer();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CancelTimer();
        }
        
        protected override void UpdateCurrentTime()
        {
            base.UpdateCurrentTime();
            Years = elapsedTime.Days / 365;
            Months = elapsedTime.Days / 30;
            Days = elapsedTime.Days;
            Hours = elapsedTime.Hours;
            Minutes = elapsedTime.Minutes;
            Seconds = elapsedTime.Seconds;
            Milliseconds = elapsedTime.Milliseconds;
            UpdateLabels();
        }

        public override void UpdateLabels()
        {
            for (int i = 0; i < labels.Count; i++)
            {
                if (labels[i].Label == null) continue;
                labels[i].SetText(elapsedTime);
            }
        }
        
        public override void ResetTimer()
        {
            base.ResetTimer();
            ClearLaps();
            UpdateLabels();
        }

        public override void StartTimer()
        {
            base.StartTimer();
            ClearLaps();
            UpdateLabels();
        }

        public override void StopTimer()
        {
            base.StopTimer();
            UpdateLabels();
        }

        public override void PauseTimer()
        {
            base.PauseTimer();
            UpdateLabels();
        }

        public override void ResumeTimer()
        {
            base.ResumeTimer();
            UpdateLabels();
        }

        public override void FinishTimer()
        {
            StartUpdateCoroutine();
            UpdateCurrentTime();
            StopTimer();
            OnFinish?.Execute();
            UpdateLabels();
        }

        public override void CancelTimer()
        {
            base.CancelTimer();
            UpdateLabels();
        }

        /// <summary>
        /// Add a new lap time and duration to the lapTimes and lapDurations dictionaries
        /// </summary>
        public void AddLap()
        {
            laps.Add
            (
                new Lap
                (
                    currentLapNumber,             //lap number
                    elapsedTime,                  //lap time
                    elapsedTime - previousLapTime //lap duration
                )
            );
            OnLap?.Execute();              //trigger the OnLap event
            UpdateLabels();                //update the labels
            currentLapIndex++;             //increment the current lap index
            previousLapTime = elapsedTime; //update the previous lap time
        }

        /// <summary>
        /// Get the lap time for the given lap number (starts at 1 for the first lap)
        /// </summary>
        /// <param name="lapNumber"> Lap number (starts at 1 for the first lap) </param>
        /// <returns> Lap time for the given lap number </returns>
        public Lap GetLap(int lapNumber)
        {
            if (lapNumber < 1 || lapNumber > currentLapNumber) return new Lap();
            return laps[lapNumber - 1];
        }
        
        /// <summary> Clear the laps list and reset the current lap index </summary>
        public void ClearLaps()
        {
            laps.Clear();
            currentLapIndex = 0;
            previousLapTime = TimeSpan.Zero; 
        }

        protected override void SetEndTime()
        {
            endTime =
                startTime
                    .AddYears(100);
        }
    }
}
