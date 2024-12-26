// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Mody;
using Doozy.Runtime.Signals;
// ReSharper disable UnassignedGetOnlyAutoProperty
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.UIManager.Listeners.Internal
{
    public abstract class BaseListener : BaseStreamListener
    {
        /// <summary> Callback invoked every time the listener is triggered </summary>
        public ModyEvent Callback;

        protected BaseListener()
        {
            Callback = new ModyEvent("Callback invoked every time the listener is triggered");
        }

        protected override void ProcessSignal(Signal signal)
        {
            Callback?.Execute(signal);
        }
    }
}
