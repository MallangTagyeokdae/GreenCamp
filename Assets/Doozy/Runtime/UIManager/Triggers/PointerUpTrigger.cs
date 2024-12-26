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
    [AddComponentMenu("UI/Triggers/PointerUp")]
    public class PointerUpTrigger : SignalProvider, IPointerUpHandler
    {
        // #if UNITY_EDITOR
        // [UnityEditor.MenuItem("GameObject/UI/Triggers/PointerUp", false, 8)]
        // private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        // {
        //     GameObjectUtils.AddToScene<PointerUpTrigger>("PointerUp Trigger", false, true);
        // }
        // #endif
        
        /// <summary> Called when the pointer is released </summary>
        public PointerEventDataEvent OnTrigger = new PointerEventDataEvent();
        
        public PointerUpTrigger() : base(ProviderType.Local, "Pointer", "Up", typeof(PointerUpTrigger)) {}

        public void OnPointerUp(PointerEventData eventData)
        {
            if (UISettings.interactionsDisabled) return;
            SendSignal(eventData);
            OnTrigger?.Invoke(eventData);
        }
    }
}
