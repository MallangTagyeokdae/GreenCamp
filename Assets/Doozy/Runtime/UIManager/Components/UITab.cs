// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.UIManager.Containers;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Local

namespace Doozy.Runtime.UIManager.Components
{
    /// <summary>
    /// Toggle component, based on UIToggle, that can sync its state with a UIContainer. 
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("UI/Components/UITab")]
    [SelectionBase]
    public partial class UITab : UIToggle
    {
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/UI/Components/UITab", false, 8)]
        private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        {
            GameObjectUtils.AddToScene<UITab>("UITab", false, true);
        }
        #endif

        [SerializeField] private UIContainer TargetContainer;
        /// <summary> Reference to the target container controlled by this UITab </summary>
        public UIContainer targetContainer
        {
            get => TargetContainer;
            set
            {
                if (TargetContainer == value) return;
                DisconnectFromContainer();
                TargetContainer = value;
                ConnectToContainer(true);
            }
        }

        /// <summary>
        /// Internal flag used to keep track if the UITab is currently connected to a container
        /// </summary>
        private bool isConnectedToContainer { get; set; }
        
        /// <summary> Connects the UITab to the referenced target container </summary>
        /// <param name="updateIsOn"> If TRUE, the UITab will update its isOn value to match the target container's isOn value </param>
        private void ConnectToContainer(bool updateIsOn)
        {
            // Debug.Log($"[UITab] ConnectToContainer - updateIsOn: {updateIsOn}");
            if (TargetContainer == null)
            {
                isConnectedToContainer = false;
                return;
            }
            if (isConnectedToContainer) return;
            TargetContainer.OnShowCallback.Event.AddListener(UpdateIsOnFromContainer);
            TargetContainer.OnHideCallback.Event.AddListener(UpdateIsOnFromContainer);
            if (updateIsOn) UpdateIsOnFromContainer();
            isConnectedToContainer = true;
            // Debug.Log($"[UITab] '{name}' Connected to container '{TargetContainer.name}'");
        }

        /// <summary> Disconnects the UITab from the referenced target container </summary>
        private void DisconnectFromContainer()
        {
            if (TargetContainer == null) return;
            TargetContainer.OnShowCallback.Event.RemoveListener(UpdateIsOnFromContainer);
            TargetContainer.OnHideCallback.Event.RemoveListener(UpdateIsOnFromContainer);
            // Debug.Log($"[UITab] '{name}' Disconnected from container '{TargetContainer.name}'");
            TargetContainer = null;
            isOn = false;
            isConnectedToContainer = false;
        }

        /// <summary> Updates the UITab's isOn value to match the target container's isOn value </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void UpdateIsOnFromContainer()
        {
            // Debug.Log($"[UITab] '{name}' UpdateIsOnFromContainer {TargetContainer.visibilityState} --> isOn = {isOn}");
            switch (TargetContainer.visibilityState)
            {
                case VisibilityState.Visible:
                case VisibilityState.IsShowing:
                    if (isOn) break;
                    isOn = true;
                    break;
                case VisibilityState.Hidden:
                case VisibilityState.IsHiding:
                    if(!isOn) break;
                    isOn = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void Awake()
        {
            isConnectedToContainer = false;
            base.Awake();
            ConnectToContainer(false);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            StartCoroutine($"UpdateConnection");
        }

        internal override void ValueChanged(bool previousValue, bool newValue, bool animateChange, bool triggerValueChanged)
        {

            base.ValueChanged(previousValue, newValue, animateChange, triggerValueChanged);
            if (!isConnectedToContainer) return;
            switch (isOn)
            {
                case true:
                    if (animateChange)
                    {
                        TargetContainer.Show();
                        break;
                    }
                    TargetContainer.InstantShow();
                    break;
                case false:
                    if (animateChange)
                    {
                        TargetContainer.Hide();
                        break;
                    }
                    TargetContainer.InstantHide();
                    break;
            }
        }

        private IEnumerable UpdateConnection()
        {
            yield return null;

            ConnectToContainer(false);

            if (inToggleGroup)
            {
                ValueChanged(isOn, isConnectedToContainer && isOn, false, false);
                yield break;
            }

            UpdateIsOnFromContainer();
        }
    }
}
