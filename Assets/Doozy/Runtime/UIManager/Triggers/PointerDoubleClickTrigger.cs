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
    [AddComponentMenu("UI/Triggers/PointerDoubleClick")]
    public class PointerDoubleClickTrigger : SignalProvider, IPointerDownHandler, IPointerUpHandler
    {
        // #if UNITY_EDITOR
        // [UnityEditor.MenuItem("GameObject/UI/Triggers/PointerDoubleClick", false, 8)]
        // private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        // {
        //     GameObjectUtils.AddToScene<PointerDoubleClickTrigger>("PointerDoubleClick Trigger", false, true);
        // }
        // #endif
        
        /// <summary> Called when a pointer double click event happens </summary>
        public PointerEventDataEvent OnTrigger = new PointerEventDataEvent();
        
        public const float k_DoubleClickRegisterInterval = 0.2f;
        
        private bool m_ClickedOnce;
        private float m_ClickTime;

        public PointerDoubleClickTrigger() : base(ProviderType.Local, "Pointer", "Double Click", typeof(PointerDoubleClickTrigger))
        {
            Reset();
        }

        public void Reset()
        {
            m_ClickedOnce = false;
            m_ClickTime = 0;
        }

        public void OnPointerDown(PointerEventData eventData) {}

        public void OnPointerUp(PointerEventData eventData)
        {
            if (m_ClickedOnce == false)
            {
                // Debug.Log($"clicked once");
                m_ClickedOnce = true;
                m_ClickTime = Time.unscaledTime;
                return;
            }

            if (Time.unscaledTime - m_ClickTime > k_DoubleClickRegisterInterval) //interval passed
            {
                // Debug.Log($"clicked once - after interval passed");
                m_ClickedOnce = true;
                m_ClickTime = Time.unscaledTime;
                return;
            }

            Reset();
            if(UISettings.interactionsDisabled) return;
            SendSignal(eventData);
            OnTrigger?.Invoke(eventData);
            // Debug.Log($"clicked twice - SIGNAL");
        }
    }
}
