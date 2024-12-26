// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Doozy.Runtime.UIManager.Triggers
{
    [AddComponentMenu("UI/Triggers/PointerLongClick")]
    public class PointerLongClickTrigger : SignalProvider, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        // #if UNITY_EDITOR
        // [UnityEditor.MenuItem("GameObject/UI/Triggers/PointerLongClick", false, 8)]
        // private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        // {
        //     GameObjectUtils.AddToScene<PointerLongClickTrigger>("PointerLongClick Trigger", false, true);
        // }
        // #endif
        
        /// <summary> Called when the pointer pressed down for a set interval of time over the trigger </summary>
        public PointerEventDataEvent OnTrigger = new PointerEventDataEvent();
        
        public const float k_LongClickRegisterInterval = 0.5f;

        private float m_LongClickTriggerTime;
        private Coroutine run { get; set; }
        
        public PointerLongClickTrigger() : base(ProviderType.Local, "Pointer", "Long Click", typeof(PointerLongClickTrigger))
        {
            Reset();
        }

        public void Reset()
        {
            if (run != null)
            {
                StopCoroutine(run);
                run = null;
            }
            
            m_LongClickTriggerTime = 0;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            Reset();
            m_LongClickTriggerTime = Time.unscaledTime + k_LongClickRegisterInterval;
            run = StartCoroutine(Run(eventData));
        }
        
        public void OnPointerUp(PointerEventData eventData) =>
            Reset();

        public void OnPointerExit(PointerEventData eventData) =>
            Reset();

        private IEnumerator Run(PointerEventData eventData)
        {
            while (m_LongClickTriggerTime > Time.unscaledTime)
                yield return null;

            if (UISettings.interactionsDisabled) yield break;
            SendSignal(eventData);
            OnTrigger?.Invoke(eventData);
            Reset();
        }
    }
}
