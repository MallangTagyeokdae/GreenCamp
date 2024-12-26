// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.Events;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Mody;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBeProtected.Global

namespace Doozy.Runtime.UIManager.Components
{
    /// <summary>
    /// Toggle component based on UISelectable with category/name id identifier.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("UI/Components/UIToggle")]
    [SelectionBase]
    public partial class UIToggle : UISelectable, IPointerClickHandler, ISubmitHandler
    {
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/UI/Components/UIToggle", false, 8)]
        private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        {
            GameObjectUtils.AddToScene<UIToggle>("UIToggle", false, true);
        }
        #endif

        /// <summary> UIToggles database </summary>
        public static HashSet<UIToggle> database { get; private set; } = new HashSet<UIToggle>();

        [ExecuteOnReload]
        private static void OnReload()
        {
            database = new HashSet<UIToggle>();
        }

        [ClearOnReload]
        private static SignalStream s_stream;
        /// <summary> UIToggle signal stream </summary>
        public static SignalStream stream => s_stream ?? (s_stream = SignalsService.GetStream(k_StreamCategory, nameof(UIToggle)));

        /// <summary> All toggles that are active and enabled </summary>
        public static IEnumerable<UIToggle> availableToggles => database.Where(item => item.isActiveAndEnabled);

        /// <summary> TRUE is this selectable is selected by EventSystem.current, FALSE otherwise </summary>
        public bool isSelected => EventSystem.current.currentSelectedGameObject == gameObject;

        /// <summary> Type of selectable </summary>
        public override SelectableType selectableType => SelectableType.Toggle;

        /// <summary> UIToggle identifier </summary>
        public UIToggleId Id = new UIToggleId();

        /// <summary>
        /// Cooldown time in seconds before the toggle can be clicked again.
        /// <para/> This is useful when you want to prevent the toggle from being clicked multiple times in a short period of time.
        /// </summary>
        public float Cooldown;

        /// <summary>
        /// Set the toggle's interactable state to false during the cooldown time.
        /// </summary>
        public bool DisableWhenInCooldown;
        
        /// <summary> Internal coroutine to handle the cooldown. </summary>
        private Coroutine cooldownRoutine { get; set; }

        /// <summary> Internal flag to determine if the toggle is currently in cooldown </summary>
        private bool inCooldown => cooldownRoutine != null;

        /// <summary> Toggle became ON - executed when isOn becomes TRUE </summary>
        public ModyEvent OnToggleOnCallback = new ModyEvent(nameof(OnToggleOnCallback));

        // <summary> Toggle became ON with instant animations - executed when isOn becomes TRUE </summary>
        public ModyEvent OnInstantToggleOnCallback = new ModyEvent(nameof(OnInstantToggleOnCallback));

        /// <summary> Toggle became OFF - executed when isOn becomes FALSE </summary>
        public ModyEvent OnToggleOffCallback = new ModyEvent(nameof(OnToggleOffCallback));

        /// <summary> Toggle became OFF with instant animations - executed when isOn becomes FALSE </summary>
        public ModyEvent OnInstantToggleOffCallback = new ModyEvent(nameof(OnInstantToggleOffCallback));

        /// <summary> Toggle changed its value - executed when isOn changes its value </summary>
        public BoolEvent OnValueChangedCallback = new BoolEvent();

        /// <summary> Toggle value changed callback. This special callback also sends when the event happened, the previousValue and the newValue </summary>
        public UnityAction<ToggleValueChangedEvent> onToggleValueChangedCallback { get; set; }

        /// <summary> Returns TRUE if this toggle has a toggle group reference </summary>
        public bool inToggleGroup => ToggleGroup != null && ToggleGroup.toggles.Contains(this);

        [SerializeField] private UIToggleGroup ToggleGroup;
        /// <summary> Reference to the toggle group that this toggle belongs to </summary>
        public UIToggleGroup toggleGroup
        {
            get => ToggleGroup;
            internal set => ToggleGroup = value;
        }

        /// <summary> TRUE if the toggle is on, FALSE otherwise </summary>
        public override bool isOn
        {
            get => IsOn;
            set
            {
                bool previousValue = IsOn;
                IsOn = value;

                if (inToggleGroup)
                {
                    toggleGroup.ToggleChangedValue(toggle: this, animateChange: true);
                    return;
                }

                ValueChanged(previousValue: previousValue, newValue: value, animateChange: true, triggerValueChanged: true);
            }
        }

        /// <summary> Internal flag to track if the toggle has been initialized </summary>
        protected bool toggleInitialized { get; set; }

        protected override void Awake()
        {
            toggleInitialized = false;
            if (!Application.isPlaying) return;
            database.Add(this);
            base.Awake();
        }

        protected override void OnEnable()
        {
            if (!Application.isPlaying) return;
            StopCooldown();
            database.Remove(null);
            base.OnEnable();
            InitializeToggle();
        }

        protected override void OnDisable()
        {
            StopCooldown();
            database.Remove(null);
            base.OnDisable();
        }

        protected override void OnDestroy()
        {
            database.Remove(null);
            database.Remove(this);
            base.OnDestroy();
        }

        /// <summary> Initializes the toggle </summary>
        protected virtual void InitializeToggle()
        {
            if (toggleInitialized) return;
            AddToToggleGroup(toggleGroup);
            toggleInitialized = true;
            if (inToggleGroup) return;
            ValueChanged(isOn, isOn, false, false);
        }

        /// <summary> Called when on pointer click event is sent by the IPointerClickHandler </summary>
        /// <param name="eventData"> Pointer event data </param>
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (inCooldown) return;

            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (!IsActive() || !IsInteractable())
                return;

            ToggleValue();
        }

        /// <summary> Called when on submit event is sent by the ISubmitHandler </summary>
        /// <param name="eventData"> Event data </param>
        public virtual void OnSubmit(BaseEventData eventData)
        {
            if (inCooldown) return;

            if (!IsActive() || !IsInteractable())
                return;

            ToggleValue();

            if (!inputSettings.submitTriggersPointerClick) return;
            behaviours.GetBehaviour(UIBehaviour.Name.PointerClick)?.Execute();
            behaviours.GetBehaviour(UIBehaviour.Name.PointerLeftClick)?.Execute();
        }

        /// <summary> Toggle the toggle's value </summary>
        protected virtual void ToggleValue()
        {
            isOn = !isOn;
            StartCooldown();
        }

        /// <summary> Adds this toggle to the specified toggle group </summary>
        /// <param name="targetToggleGroup"> Target toggle group </param>
        public void AddToToggleGroup(UIToggleGroup targetToggleGroup)
        {
            if (targetToggleGroup == null)
                return;
            if (inToggleGroup && targetToggleGroup != toggleGroup)
                RemoveFromToggleGroup();
            targetToggleGroup.AddToggle(this);
        }

        /// <summary> Removes this toggle from its assigned toggle group </summary>
        public void RemoveFromToggleGroup()
        {
            if (toggleGroup == null)
                return;
            toggleGroup.RemoveToggle(this);
        }

        /// <summary> Called when the value of the toggle is changed by a toggle group </summary>
        /// <param name="newValue"> New value of the toggle </param>
        /// <param name="animateChange"> TRUE if the change should be animated, FALSE otherwise </param>
        /// <param name="triggerValueChanged"> TRUE if the value changed callback should be triggered, FALSE otherwise </param>
        protected internal virtual void UpdateValueFromGroup(bool newValue, bool animateChange, bool triggerValueChanged = true)
        {
            bool previousValue = IsOn;
            IsOn = newValue;
            ValueChanged(previousValue, newValue, animateChange, triggerValueChanged);
        }

        /// <summary> Send a signal to the toggle signal stream with the new value for this toggle </summary>
        /// <param name="newValue"> New value of the toggle </param>
        internal void SendSignal(bool newValue)
        {
            stream.SendSignal(new UIToggleSignalData(Id.Category, Id.Name, newValue ? CommandToggle.On : CommandToggle.Off, playerIndex, this));
        }

        /// <summary> Called when the value of the toggle changes </summary>
        /// <param name="previousValue"> Previous value of the toggle </param>
        /// <param name="newValue"> New value of the toggle </param>
        /// <param name="animateChange"> TRUE if the change should be animated, FALSE otherwise </param>
        /// <param name="triggerValueChanged"> TRUE if the value changed callback should be triggered, FALSE otherwise </param>
        internal virtual void ValueChanged(bool previousValue, bool newValue, bool animateChange, bool triggerValueChanged)
        {
            RefreshState();

            switch (newValue)
            {
                case true:
                    switch (animateChange)
                    {
                        case true:
                            OnToggleOnCallback?.Execute();
                            break;
                        default:
                            OnInstantToggleOnCallback?.Execute();
                            break;
                    }
                    break;

                case false:
                    switch (animateChange)
                    {
                        case true:
                            OnToggleOffCallback?.Execute();
                            break;
                        default:
                            OnInstantToggleOffCallback?.Execute();
                            break;
                    }
                    break;
            }

            if (!triggerValueChanged)
                return;

            SendSignal(newValue);
            OnValueChangedCallback?.Invoke(newValue);
            onToggleValueChangedCallback?.Invoke(new ToggleValueChangedEvent(previousValue, newValue, animateChange));
        }

        #region Private Methods

        /// <summary> Start the cooldown </summary>
        private void StartCooldown()
        {
            StopCooldown();
            if (Cooldown <= 0) return;
            cooldownRoutine = StartCoroutine(CooldownRoutine());
        }

        /// <summary> Stop the cooldown </summary>
        private void StopCooldown()
        {
            if (DisableWhenInCooldown) interactable = true;
            if (cooldownRoutine == null) return;
            StopCoroutine(cooldownRoutine);
            cooldownRoutine = null;
        }

        /// <summary> Internal coroutine to handle the cooldown. </summary>
        private IEnumerator CooldownRoutine()
        {
            if (DisableWhenInCooldown) interactable = false;
            yield return new WaitForSecondsRealtime(Cooldown);
            if (DisableWhenInCooldown) interactable = true;
            cooldownRoutine = null;
        }

        #endregion


        #region Static Methods

        /// <summary> Get all the registered toggles with the given category and name </summary>
        /// <param name="category"> UIToggle category </param>
        /// <param name="name"> UIToggle name (from the given category) </param>
        public static IEnumerable<UIToggle> GetToggles(string category, string name) =>
            database.Where(toggle => toggle.Id.Category.Equals(category)).Where(toggle => toggle.Id.Name.Equals(name));

        /// <summary> Get all the registered toggles with the given category </summary>
        /// <param name="category"> UIToggle category </param>
        public static IEnumerable<UIToggle> GetAllTogglesInCategory(string category) =>
            database.Where(toggle => toggle.Id.Category.Equals(category));

        /// <summary> Get all the toggles that are active and enabled (all the visible/available toggles) </summary>
        public static IEnumerable<UIToggle> GetAvailableToggles() =>
            database.Where(toggle => toggle.isActiveAndEnabled);

        /// <summary> Get the selected toggle (if a toggle is not selected, this method returns null) </summary>
        public static UIToggle GetSelectedToggle() =>
            database.FirstOrDefault(toggle => toggle.isSelected);

        /// <summary> Select the toggle with the given category and name (if it is active and enabled) </summary>
        /// <param name="category"> UIToggle category </param>
        /// <param name="name"> UIToggle name (from the given category) </param>
        public static bool SelectToggle(string category, string name)
        {
            UIToggle toggle = availableToggles.FirstOrDefault(b => b.Id.Category.Equals(category) & b.Id.Name.Equals(name));
            if (toggle == null) return false;
            toggle.Select();
            return true;
        }

        #endregion
    }

    public static class UIToggleExtensions
    {
        /// <summary> Set the toggle value to the given value </summary>
        /// <param name="target"> Target toggle </param>
        /// <param name="newValue"> New value of the toggle </param>
        /// <param name="animateChange"> TRUE if the change should be animated, FALSE otherwise </param>
        /// <param name="triggerValueChanged"> TRUE if the value changed callback should be triggered, FALSE otherwise </param>
        public static T SetIsOn<T>(this T target, bool newValue, bool animateChange = true, bool triggerValueChanged = true) where T : UIToggle
        {
            bool previousValue = target.isOn;
            target.IsOn = newValue;
            if (target.inToggleGroup)
            {
                target.toggleGroup.ToggleChangedValue(target, animateChange, triggerValueChanged);
                return target;
            }
            target.ValueChanged(previousValue, newValue, animateChange, triggerValueChanged);
            return target;
        }
    }
}
