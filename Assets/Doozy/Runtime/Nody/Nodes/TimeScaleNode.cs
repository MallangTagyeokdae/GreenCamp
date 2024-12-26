// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Nody.Nodes.Internal;
using Doozy.Runtime.Reactor.Easings;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using UnityEngine;
// ReSharper disable RedundantOverriddenMember

namespace Doozy.Runtime.Nody.Nodes
{
    /// <summary>
    /// TimeScale Node sets the scale at which the time is passing (it updates Time.timeScale). This can be used for slow motion effects.
    /// It can do that that either instantly or over a set duration (animated).
    /// The node can also wait until the current Time.timeScale value has reached the target value, before activating the next node in the Graph.
    /// </summary>
    [Serializable]
    [NodyMenuPath("Time", "TimeScale")]
    public sealed class TimeScaleNode : SimpleNode
    {
        public static string timescaleAnimationId => $"{nameof(TimeScaleNode)} TimeScale Animation";

        public FloatReaction timeScaleReaction { get; private set; }

        public float TargetValue;
        public bool AnimateValue;
        public float AnimationDuration;
        public Ease AnimationEase;
        public bool WaitForAnimationToFinish;

        public TimeScaleNode()
        {
            TargetValue = 1f;
            AnimateValue = false;
            AnimationDuration = 1f;
            AnimationEase = Ease.Linear;
            WaitForAnimationToFinish = false;

            AddInputPort()
                .SetCanBeDeleted(false)
                .SetCanBeReordered(false);

            AddOutputPort()
                .SetCanBeDeleted(false)
                .SetCanBeReordered(false);
        }

        public override void OnEnter(FlowNode previousNode = null, FlowPort previousPort = null)
        {
            base.OnEnter(previousNode, previousPort);
            StartTimer();
        }

        private void StartTimer()
        {
            if (Math.Abs(Time.timeScale - TargetValue) < 0.01f)
            {
                Time.timeScale = TargetValue;
                GoToNextNode(firstOutputPort);
                return;
            }

            timeScaleReaction?.Recycle();
            timeScaleReaction =
                Reaction
                    .Get<FloatReaction>()
                    .SetStringId(timescaleAnimationId)
                    .SetSetter(value => Time.timeScale = value)
                    .SetGetter(() => Time.timeScale);

            timeScaleReaction.SetEase(AnimationEase);
            if (AnimateValue && AnimationDuration > 0)
            {
                timeScaleReaction.settings.duration = AnimationDuration;
                timeScaleReaction.SetFrom(Time.timeScale);
                timeScaleReaction.SetTo(TargetValue);
                timeScaleReaction.ClearOnFinishCallback();
                if (WaitForAnimationToFinish)
                {
                    timeScaleReaction.AddOnFinishCallback(() =>
                    {
                        StopTimer();
                        GoToNextNode(firstOutputPort);
                        timeScaleReaction?.Recycle();
                    });
                    timeScaleReaction.Play();
                    return;
                }
                timeScaleReaction.AddOnFinishCallback(StopTimer);
                timeScaleReaction.Play();
                GoToNextNode(firstOutputPort);
                timeScaleReaction?.Recycle();
                return;
            }
            Time.timeScale = TargetValue;
            GoToNextNode(firstOutputPort);
        }

        private void StopTimer()
        {
            if (timeScaleReaction == null)
                return;

            timeScaleReaction.Finish();
            timeScaleReaction.ClearOnFinishCallback();
            timeScaleReaction.Recycle();
            timeScaleReaction = null;
        }


    }
}
