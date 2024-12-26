// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Doozy.Runtime.UIManager.Triggers
{
    [AddComponentMenu("UI/Triggers/UISubmit")]
    public class UISubmitTrigger : SignalProvider, ISubmitHandler
    {
        // #if UNITY_EDITOR
        // [UnityEditor.MenuItem("GameObject/UI/Triggers/UISubmit", false, 8)]
        // private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        // {
        //     GameObjectUtils.AddToScene<UISubmitTrigger>("UISubmit Trigger", false, true);
        // }
        // #endif
        
        /// <summary> Called when a 'Submit' event has been received </summary>
        public BaseEventDataEvent OnTrigger = new BaseEventDataEvent();
        
        public UISubmitTrigger() : base(ProviderType.Local, "UI", "Submit", typeof(UISubmitTrigger)) {}

        public void OnSubmit(BaseEventData eventData)
        {
            if (UISettings.interactionsDisabled) return;
            SendSignal(eventData);
            OnTrigger?.Invoke(eventData);
        }
    }
}
