// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Common;
using Doozy.Runtime.UIManager.Content.Internal;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Content
{
    /// <summary>
    /// The UIClock component is used to display the current time in one or more text components.
    /// </summary>
    public class UIClock : DateTimeComponent
    {
        [SerializeField] private string TimeZoneId;
        /// <summary> Time zone ID for the clock </summary>
        public string timeZoneId
        {
            get => TimeZoneId;
            set
            {
                if (timeZoneInfo.Id.Equals(value)) return;
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(value);
                TimeZoneId = value;
                TimeZoneChanged();
             }
        }

        private TimeZoneInfo m_TimeZoneInfo = TimeZoneInfo.Local;
        /// <summary> The time zone info of the clock </summary>
        public TimeZoneInfo timeZoneInfo
        {
            get => m_TimeZoneInfo;
            set
            {
                m_TimeZoneInfo = value;
                TimeZoneId = value.Id;
                TimeZoneChanged();
            }
        }

        #if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            if(labels.Count == 0) labels.Add(new FormattedLabel(null));

            OnStartBehaviour = OnStartBehaviour = TimerBehaviour.Disabled;
            OnEnableBehaviour = OnEnableBehaviour = TimerBehaviour.ResetAndStart;
            OnDisableBehaviour = OnDisableBehaviour = TimerBehaviour.Stop;
            OnDisableBehaviour = OnDisableBehaviour = TimerBehaviour.Cancel;

            SetUtcTimeZone();
        }
        #endif // UNITY_EDITOR
        
        protected override void OnEnable()
        {
            base.OnEnable();
            StartTimer();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            StopTimer();
        }


        public override void StartTimer()
        {
            base.StartTimer();
            UpdateLabels();
        }

        public override void StopTimer()
        {
            base.StopTimer();
            UpdateLabels();
        }

        public override void ResetTimer()
        {
            base.ResetTimer();
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

        public override void CancelTimer()
        {
            base.CancelTimer();
            UpdateLabels();
        }

        protected override void SetStartTime()
        {
            //set start time according to the current time zone
            startTime = TimeZoneInfo.ConvertTimeFromUtc(GetDateTimeUtcNow(), timeZoneInfo);
            UpdateLastTime();
        }

        protected override void SetEndTime()
        {
            endTime =
                startTime
                    .AddYears(100);
        }

        public void TimeZoneChanged()
        {
            SetStartTime();
            SetEndTime();
            UpdateCurrentTime();
        }
        
        protected override void UpdateCurrentTime()
        {
            currentTime = TimeZoneInfo.ConvertTimeFromUtc(GetDateTimeUtcNow(), timeZoneInfo);
            Years = currentTime.Year;
            Months = currentTime.Month;
            Days = currentTime.Day;
            Hours = currentTime.Hour;
            Minutes = currentTime.Minute;
            Seconds = currentTime.Second;
            Milliseconds = currentTime.Millisecond;
            UpdateLabels();
        }

        public virtual DateTime GetDateTimeUtcNow()
        {
            return DateTime.UtcNow;
        }
        
        public void SetTimeZone(string zoneId)
        {
            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(zoneId);
            TimeZoneId = timeZoneInfo.Id;
            TimeZoneChanged();

        }
        
        public int GetTimeZoneUtcOffset() =>
            timeZoneInfo.BaseUtcOffset.Hours;
        

        public void SetTimeZone(TimeZoneInfo zoneInfo)
        {
            timeZoneInfo = zoneInfo;
            TimeZoneId = timeZoneInfo.Id;
            TimeZoneChanged();
        }
        
        /// <summary>
        /// Set the time zone for the clock by using the UTC offset
        /// </summary>
        /// <param name="utcOffset"></param>
        public void SetTimeZoneByUtcOffset(int utcOffset)
        {
            string zoneId = "UTC" + (utcOffset >= 0 ? "+" : "") + utcOffset;
            timeZoneInfo = TimeZoneInfo.CreateCustomTimeZone(zoneId, TimeSpan.FromHours(utcOffset), zoneId, zoneId);
            TimeZoneId = timeZoneInfo.Id;
            TimeZoneChanged();
        }
        
        /// <summary>
        /// Sets the local timezone as the clock's time zone
        /// </summary>
        public void SetLocalTimeZone() =>
            SetTimeZone(TimeZoneInfo.Local);

        /// <summary>
        /// Sets the UTC as the clock's time zone
        /// </summary>
        public void SetUtcTimeZone() =>
            SetTimeZone(TimeZoneInfo.Utc);
    }
}
