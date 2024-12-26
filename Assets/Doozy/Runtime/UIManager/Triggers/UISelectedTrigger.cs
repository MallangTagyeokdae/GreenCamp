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
    [AddComponentMenu("UI/Triggers/UISelected")]
    public class UISelectedTrigger : SignalProvider, ISelectHandler
    {
        // #if UNITY_EDITOR
        // [UnityEditor.MenuItem("GameObject/UI/Triggers/UISelected", false, 8)]
        // private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        // {
        //     GameObjectUtils.AddToScene<UISelectedTrigger>("UISelected Trigger", false, true);
        // }
        // #endif
        
        /// <summary> Called when a Selectable is selected </summary>
        public BaseEventDataEvent OnTrigger = new BaseEventDataEvent();
        
        public UISelectedTrigger() : base(ProviderType.Local, "UI", "Selected", typeof(UISelectedTrigger)) {}

        public void OnSelect(BaseEventData eventData)
        {
            SendSignal(eventData);
            OnTrigger?.Invoke(eventData);
        }
    }
}
