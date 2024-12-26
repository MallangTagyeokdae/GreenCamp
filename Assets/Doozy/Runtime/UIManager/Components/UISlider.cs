// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.Events;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Mody;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MoveDirection = UnityEngine.EventSystems.MoveDirection;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Runtime.UIManager.Components
{
    /// <summary>
    /// Slider component based on UISelectable with category/name id identifier.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("UI/Components/UISlider")]
    [SelectionBase]
    public partial class UISlider : UISelectable, IDragHandler, IInitializePotentialDragHandler
    {
        private const float TOLERANCE = 0.0001f;

        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/UI/Components/UISlider", false, 8)]
        private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        {
            GameObjectUtils.AddToScene<UISlider>("UISlider", false, true);
        }
        #endif

        /// <summary> UISliders database </summary>
        public static HashSet<UISlider> database { get; private set; } = new HashSet<UISlider>();

        [ExecuteOnReload]
        // ReSharper disable once UnusedMember.Local
        private static void OnReload()
        {
            database = new HashSet<UISlider>();
        }

        [ClearOnReload]
        private static SignalStream s_stream;
        /// <summary> UISlider signal stream </summary>
        public static SignalStream stream => s_stream ?? (s_stream = SignalsService.GetStream(k_StreamCategory, nameof(UISlider)));

        /// <summary> All sliders that are active and enabled </summary>
        public static IEnumerable<UISlider> availableSliders => database.Where(item => item.isActiveAndEnabled);

        /// <summary> TRUE is this selectable is selected by EventSystem.current, FALSE otherwise </summary>
        public bool isSelected => EventSystem.current.currentSelectedGameObject == gameObject;

        /// <summary> Type of selectable </summary>
        public override SelectableType selectableType => SelectableType.Button;

        /// <summary> UISlider identifier </summary>
        public UISliderId Id;

        /// <summary> Slider changed its value - executed when the slider changes its value </summary>
        [Obsolete("Use OnValueChanged instead")]
        public FloatEvent OnValueChangedCallback;

        /// <summary>
        /// Fired when the value changed.
        /// Returns the new value.
        /// </summary>
        public FloatEvent OnValueChanged = new FloatEvent();

        /// <summary>
        /// Fired when the value increases.
        /// Returns the difference between the new value and the previous value.
        /// <para/> Example: if the previous value was 0.5 and the new value is 0.7, the difference is 0.2
        /// </summary>
        public FloatEvent OnValueIncremented = new FloatEvent();

        /// <summary>
        /// Fired when the value decreases.
        /// Returns the difference between the new value and the previous value.
        /// <para/> Example: if the previous value was 10 and the new value is 5, the returned value will be -5
        /// </summary>
        public FloatEvent OnValueDecremented = new FloatEvent();

        /// <summary> Fired when the value was reset </summary>
        public ModyEvent OnValueReset = new ModyEvent();

        /// <summary> Fired when the value has reached the minimum value </summary>
        public ModyEvent OnValueReachedMin = new ModyEvent();

        /// <summary> Fired when the value has reached the maximum value </summary>
        public ModyEvent OnValueReachedMax = new ModyEvent();

        [SerializeField] private RectTransform FillRect;
        /// <summary> Optional RectTransform to use as fill for the slider </summary>
        public RectTransform fillRect
        {
            get => FillRect;
            set
            {
                if (value == FillRect)
                    return;

                FillRect = value;
                UpdateCachedReferences();
                UpdateVisuals();
            }
        }

        [SerializeField] private RectTransform HandleRect;
        /// <summary> Optional RectTransform to use as a handle for the slider </summary>
        public RectTransform handleRect
        {
            get => HandleRect;
            set
            {
                if (value == HandleRect)
                    return;

                HandleRect = value;
                UpdateCachedReferences();
                UpdateVisuals();
            }
        }

        [SerializeField] private SlideDirection Direction = SlideDirection.LeftToRight;
        /// <summary> The direction of the slider, from minimum to maximum value </summary>
        public SlideDirection direction
        {
            get => Direction;
            set
            {
                Direction = value;
                UpdateVisuals();
            }
        }

        [SerializeField] private float MinValue;
        /// <summary> The minimum allowed value of the slider </summary>
        public float minValue
        {
            get => MinValue;
            set
            {
                MinValue = value;
                Value.Clamp(MinValue, MaxValue);
                UpdateLabel(minValueLabel, MinValue);
                UpdateVisuals();
            }
        }

        [SerializeField] private float MaxValue = 1f;
        /// <summary> The maximum allowed value of the slider </summary>
        public float maxValue
        {
            get => MaxValue;
            set
            {
                MaxValue = value;
                Value.Clamp(MinValue, MaxValue);
                UpdateLabel(maxValueLabel, MaxValue);
                UpdateVisuals();
            }
        }

        [SerializeField] private bool WholeNumbers;
        /// <summary> Should the value only be allowed to be whole numbers? </summary>
        public bool wholeNumbers
        {
            get => WholeNumbers;
            set
            {
                WholeNumbers = value;
                if (!value)
                    return;
                MinValue = Mathf.Round(MinValue);
                MaxValue = Mathf.Round(MaxValue);
                Value.Clamp(MinValue, MaxValue);
                UpdateVisuals();
                UpdateLabel(minValueLabel, MinValue);
                UpdateLabel(maxValueLabel, MaxValue);
                UpdateLabel(valueLabel, Value);
            }
        }

        [SerializeField] protected float Value;
        /// <summary> The current value of the slider </summary>
        public virtual float value
        {
            get => wholeNumbers ? Mathf.Round(Value) : Value;
            set => SetValue(value);
        }

        [SerializeField] private float DefaultValue;
        /// <summary> Reset value for the slider </summary>
        public float defaultValue
        {
            get => DefaultValue;
            set => DefaultValue = Mathf.Clamp(value, minValue, maxValue);
        }

        [SerializeField] private TMP_Text ValueLabel;
        /// <summary> Reference to the value label that displays the current value of the slider </summary>
        public TMP_Text valueLabel
        {
            get => ValueLabel;
            private set
            {
                ValueLabel = value;
                UpdateLabel(ValueLabel, Value);
            }
        }

        [SerializeField] private TMP_Text MinValueLabel;
        /// <summary> Reference to the value label that displays the min value of the slider </summary>
        public TMP_Text minValueLabel
        {
            get => MinValueLabel;
            private set
            {
                MinValueLabel = value;
                UpdateLabel(MinValueLabel, minValue);
            }
        }

        [SerializeField] private TMP_Text MaxValueLabel;
        /// <summary> Reference to the value label that displays the max value of the slider </summary>
        public TMP_Text maxValueLabel
        {
            get => MaxValueLabel;
            private set
            {
                MaxValueLabel = value;
                UpdateLabel(MaxValueLabel, maxValue);
            }
        }

        [SerializeField] private Progressor TargetProgressor;
        /// <summary> Reference to a Progressor that will be updated when the slider value changes </summary>
        public Progressor targetProgressor
        {
            get => TargetProgressor;
            private set
            {
                TargetProgressor = value;
                if (value == null) return;
                UpdateTargetProgressorMinMax();
                UpdateTargetProgressorValue();
            }
        }

        /// <summary>
        /// When true, the slider will update the target progressor value with SetValueAt instead of PlayToValue.
        /// Basically, if true, the progressor will not animate when the slider value changes. 
        /// </summary>
        public bool InstantProgressorUpdate = true;

        /// <summary> Automatically resets the value to the default value when the stepper OnEnable </summary>
        public bool ResetValueOnEnable = true;

        /// <summary> The current value of the slider normalized into a value between 0 and 1 </summary>
        public float normalizedValue
        {
            get => Mathf.Approximately(minValue, maxValue) ? 0 : Mathf.InverseLerp(minValue, maxValue, value);
            set => this.value = Mathf.Lerp(minValue, maxValue, value);
        }

        private Axis axis => GetAxis(direction);

        private static Axis GetAxis(SlideDirection slideDirection)
        {
            switch (slideDirection)
            {
                case SlideDirection.LeftToRight:
                case SlideDirection.RightToLeft:
                    return Axis.Horizontal;
                case SlideDirection.BottomToTop:
                case SlideDirection.TopToBottom:
                    return Axis.Vertical;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool reverseValue => Direction == SlideDirection.RightToLeft || Direction == SlideDirection.TopToBottom;

        private Image m_FillImage;
        private Transform m_FillTransform;
        private RectTransform m_FillContainerRect;
        private Transform m_HandleTransform;
        private RectTransform m_HandleContainerRect;

        // The offset from handle position to mouse down position
        private Vector2 m_Offset = Vector2.zero;

        private DrivenRectTransformTracker m_Tracker;

        // This "delayed" mechanism is required for case 1037681.
        private bool m_DelayedUpdateVisuals;

        // Size of each step.
        private float stepSize => wholeNumbers ? 1 : (maxValue - minValue) * 0.1f;

        private UISlider()
        {
            Id = new UISliderId();
        }

        #if UNITY_EDITOR
        protected override void OnValidate()
        {

            MinValue = WholeNumbers ? MinValue.Round(0) : MinValue;
            MaxValue = WholeNumbers ? MaxValue.Round(0) : MaxValue;
            Value = WholeNumbers ? Value.Round(0) : Value;

            if (IsActive())
            {
                UpdateCachedReferences();
                m_DelayedUpdateVisuals = true;
            }

            base.OnValidate();
        }
        #endif //UNITY_EDITOR

        public override void Rebuild(CanvasUpdate executing)
        {
            base.Rebuild(executing);

            #if UNITY_EDITOR

            if (executing == CanvasUpdate.Prelayout)
            {
                #pragma warning disable CS0618
                OnValueChangedCallback?.Invoke(value);
                #pragma warning restore CS0618
                OnValueChanged?.Invoke(value);
            }

            #endif //UNITY_EDITOR
        }

        protected override void Awake()
        {
            database.Add(this);
            base.Awake();
            
            // OnValueChanged.AddListener(v => Debug.Log($"Slider {Id} value changed to {v}"));
            // OnValueIncremented.AddListener(v => Debug.Log($"Slider {Id} value incremented to {v}"));
            // OnValueDecremented.AddListener(v => Debug.Log($"Slider {Id} value decremented to {v}"));
            // OnValueReset.Event.AddListener(() => Debug.Log($"Slider {Id} value reset to {defaultValue}"));
            // OnValueReachedMin.Event.AddListener(() => Debug.Log($"Slider {Id} value reached min value {minValue}"));
            // OnValueReachedMax.Event.AddListener(() => Debug.Log($"Slider {Id} value reached max value {maxValue}"));
        }

        protected override void OnEnable()
        {
            database.Remove(null);
            base.OnEnable();
            if (!Application.isPlaying) return;
            UpdateCachedReferences();
            UpdateTargetProgressorMinMax();
            if (ResetValueOnEnable)
            {
                ResetValue();
            }
            else
            {
                UpdateTargetProgressorValue();
            }
            UpdateVisuals();
            UpdateLabel(minValueLabel, minValue);
            UpdateLabel(maxValueLabel, maxValue);
        }

        protected override void OnDisable()
        {
            database.Remove(null);
            m_Tracker.Clear();
            UpdateTargetProgressorValue();
            base.OnDisable();
        }

        protected override void OnDestroy()
        {
            database.Remove(null);
            database.Remove(this);
            base.OnDestroy();
        }

        private void Update()
        {
            if (!m_DelayedUpdateVisuals) return;
            m_DelayedUpdateVisuals = false;
            SetValue(Value, false);
            UpdateVisuals();
        }

        protected override void OnDidApplyAnimationProperties()
        {
            // Has value changed? Various elements of the slider have the old normalisedValue assigned, we can use this to perform a comparison.
            // We also need to ensure the value stays within min/max.
            Value = ClampValue(Value);
            float previousNormalizedValue = normalizedValue;
            if (m_FillContainerRect != null)
            {
                if (m_FillImage != null && m_FillImage.type == Image.Type.Filled)
                {
                    previousNormalizedValue = m_FillImage.fillAmount;
                }
                else
                {
                    previousNormalizedValue =
                        reverseValue
                            ? 1 - FillRect.anchorMin[(int)axis]
                            : FillRect.anchorMax[(int)axis];
                }
            }
            else if (m_HandleContainerRect != null)
            {
                previousNormalizedValue =
                    reverseValue
                        ? 1 - HandleRect.anchorMin[(int)axis]
                        : HandleRect.anchorMin[(int)axis];
            }

            UpdateVisuals();

            if (Mathf.Approximately(previousNormalizedValue, normalizedValue))
                return;

            UISystemProfilerApi.AddMarker("Slider.value", this);
            #pragma warning disable CS0618
            OnValueChangedCallback.Invoke(Value);
            #pragma warning restore CS0618
            OnValueChanged.Invoke(Value);
        }

        private void UpdateCachedReferences()
        {
            if (FillRect && FillRect != (RectTransform)transform)
            {
                m_FillTransform = FillRect.transform;
                m_FillImage = FillRect.GetComponent<Image>();
                if (m_FillTransform.parent != null)
                    m_FillContainerRect = m_FillTransform.parent.GetComponent<RectTransform>();
            }
            else
            {
                FillRect = null;
                m_FillContainerRect = null;
                m_FillImage = null;
            }

            if (HandleRect && HandleRect != (RectTransform)transform)
            {
                m_HandleTransform = HandleRect.transform;
                if (m_HandleTransform.parent != null)
                    m_HandleContainerRect = m_HandleTransform.parent.GetComponent<RectTransform>();
            }
            else
            {
                HandleRect = null;
                m_HandleContainerRect = null;
            }
        }

        /// <summary> Reset the current int or float value, depending on the stepper's value type, to the default value. </summary>
        public void ResetValue()
        {
            SetValue(defaultValue);
            OnValueReset.Execute();
        }

        private float ClampValue(float input) =>
            wholeNumbers
                ? input.Clamp(minValue, maxValue).Round(0)
                : input.Clamp(minValue, maxValue);

        /// <summary> Set the value of the slider without invoking OnValueChanged callback </summary>
        /// <param name="input"> The new value for the slider </param>
        public virtual void SetValueWithoutNotify(float input) =>
            SetValue(input, false);

        /// <summary> Set the value of the slider </summary>
        /// <param name="newValue"> The new value for the slider </param>
        /// <param name="sendCallback"> If the OnValueChanged callback should be invoked </param>
        /// <remarks>
        /// Process the input to ensure the value is between min and max value. If the input is different set the value and send the callback is required.
        /// </remarks>
        public void SetValue(float newValue, bool sendCallback = true)
        {
            bool valueChanged = Math.Abs(Value - newValue) > TOLERANCE; //check if the value has changed
            float previousValue = Value;
            Value = Mathf.Clamp(newValue, minValue, maxValue); //set the new value
            if (wholeNumbers) Value = Value.Round(0);          //round the value if wholeNumbers is true

            UpdateLabel(valueLabel, Value);
            
            UpdateVisuals();

            if (valueChanged)
            {
                if (sendCallback)
                {
                    UISystemProfilerApi.AddMarker($"{nameof(UISlider)}.{nameof(value)}", this);
                    #pragma warning disable CS0618
                    OnValueChangedCallback.Invoke(Value);
                    #pragma warning restore CS0618
                    OnValueChanged?.Invoke(Value);
                    stream.SendSignal(Value);

                    if (previousValue < Value)
                    {
                        OnValueIncremented?.Invoke(Value - previousValue);
                        stream.SendSignal(new UISliderSignalData(Id.Category, Id.Name, SliderState.ValueIncremented, this));
                    }
                    else if (previousValue > Value)
                    {
                        OnValueDecremented?.Invoke(previousValue - Value);
                        stream.SendSignal(new UISliderSignalData(Id.Category, Id.Name, SliderState.ValueDecremented, this));
                    }
                }

                if (InstantProgressorUpdate)
                {
                    UpdateTargetProgressorValue();
                }
                else
                {
                    PlayTargetProgressorValue();
                }
            }

            if (!sendCallback) return;

            if (Value <= minValue)
            {
                //value is equal to the min value
                //invoke the OnValueReachedMin event if the value is equal to the min value
                OnValueReachedMin.Execute();
                //stream.SendSignal(new UISliderSignalData(Id.Category, Id.Name, SliderState.ReachedMinValue, this));
            }

            if (Value >= maxValue)
            {
                //value is equal to the max value
                //invoke the OnValueReachedMax event if the value is equal to the max value
                OnValueReachedMax.Execute();
                //stream.SendSignal(new UISliderSignalData(Id.Category, Id.Name, SliderState.ReachedMaxValue, this));
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            //this can be invoked before OnEnabled is called
            //we shouldn't be accessing other objects, before OnEnable is called
            if (!IsActive()) return;

            UpdateVisuals();
        }

        /// <summary>
        /// Force-update the slider.
        /// Useful if the properties changed and a visual update is needed.
        /// </summary>
        public void UpdateVisuals()
        {
            #if UNITY_EDITOR
            if (!Application.isPlaying)
                UpdateCachedReferences();
            #endif //UNITY_EDITOR

            m_Tracker.Clear();

            if (m_FillContainerRect != null)
            {
                m_Tracker.Add(this, FillRect, DrivenTransformProperties.Anchors);
                Vector2 anchorMin = Vector2.zero;
                Vector2 anchorMax = Vector2.one;

                if (m_FillImage != null && m_FillImage.type == Image.Type.Filled)
                {
                    m_FillImage.fillAmount = normalizedValue;
                }
                else
                {
                    if (reverseValue)
                        anchorMin[(int)axis] = 1 - normalizedValue;
                    else
                        anchorMax[(int)axis] = normalizedValue;
                }

                FillRect.anchorMin = anchorMin;
                FillRect.anchorMax = anchorMax;
            }

            if (m_HandleContainerRect == null)
                return;
            {
                m_Tracker.Add(this, HandleRect, DrivenTransformProperties.Anchors);
                Vector2 anchorMin = Vector2.zero;
                Vector2 anchorMax = Vector2.one;
                anchorMin[(int)axis] = anchorMax[(int)axis] = (reverseValue ? (1 - normalizedValue) : normalizedValue);
                HandleRect.anchorMin = anchorMin;
                HandleRect.anchorMax = anchorMax;
            }
        }

        /// <summary> Update the slider's position based on the pointer event data </summary>
        /// <param name="eventData"> Data </param>
        /// <param name="cam"> Camera </param>
        private void UpdateDrag(PointerEventData eventData, Camera cam)
        {
            RectTransform clickRect = m_HandleContainerRect ? m_HandleContainerRect : m_FillContainerRect;

            if (clickRect == null)
                return;

            if (!(clickRect.rect.size[(int)axis] > 0))
                return;

            Vector2 position = Vector2.zero;
            if (!MultipleDisplayUtilities.GetRelativeMousePositionForDrag(eventData, ref position))
                return;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(clickRect, position, cam, out Vector2 localCursor))
                return;

            Rect rect = clickRect.rect;
            localCursor -= rect.position;

            float val = Mathf.Clamp01((localCursor - m_Offset)[(int)axis] / rect.size[(int)axis]);
            normalizedValue = reverseValue ? 1f - val : val;
        }

        private bool AllowDrag(PointerEventData eventData) =>
            IsActive() &&
            IsInteractable() &&
            eventData.button == PointerEventData.InputButton.Left;

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!AllowDrag(eventData))
                return;

            base.OnPointerDown(eventData);

            m_Offset = Vector2.zero;
            if (m_HandleContainerRect != null && RectTransformUtility.RectangleContainsScreenPoint(HandleRect, eventData.pointerPressRaycast.screenPosition, eventData.enterEventCamera))
            {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(HandleRect, eventData.pointerPressRaycast.screenPosition, eventData.pressEventCamera, out Vector2 localMousePos))
                {
                    m_Offset = localMousePos;
                }
                return;
            }

            // Outside the slider handle - jump to this point instead
            UpdateDrag(eventData, eventData.pressEventCamera);
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!AllowDrag(eventData))
                return;

            UpdateDrag(eventData, eventData.pressEventCamera);
        }

        public override void OnMove(AxisEventData eventData)
        {
            if (!IsActive() || !IsInteractable())
            {
                base.OnMove(eventData);
                return;
            }

            switch (eventData.moveDir)
            {
                case MoveDirection.Left:
                    if (axis == Axis.Horizontal && FindSelectableOnLeft() == null)
                        SetValue(reverseValue ? value + stepSize : value - stepSize);
                    else
                        base.OnMove(eventData);
                    break;
                case MoveDirection.Right:
                    if (axis == Axis.Horizontal && FindSelectableOnRight() == null)
                        SetValue(reverseValue ? value - stepSize : value + stepSize);
                    else
                        base.OnMove(eventData);
                    break;
                case MoveDirection.Up:
                    if (axis == Axis.Vertical && FindSelectableOnUp() == null)
                        SetValue(reverseValue ? value - stepSize : value + stepSize);
                    else
                        base.OnMove(eventData);
                    break;
                case MoveDirection.Down:
                    if (axis == Axis.Vertical && FindSelectableOnDown() == null)
                        SetValue(reverseValue ? value + stepSize : value - stepSize);
                    else
                        base.OnMove(eventData);
                    break;
            }
        }

        /// <summary>
        /// See Selectable.FindSelectableOnLeft
        /// </summary>
        public override Selectable FindSelectableOnLeft()
        {
            if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Horizontal)
                return null;
            return base.FindSelectableOnLeft();
        }

        /// <summary>
        /// See Selectable.FindSelectableOnRight
        /// </summary>
        public override Selectable FindSelectableOnRight()
        {
            if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Horizontal)
                return null;
            return base.FindSelectableOnRight();
        }

        /// <summary>
        /// See Selectable.FindSelectableOnUp
        /// </summary>
        public override Selectable FindSelectableOnUp()
        {
            if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Vertical)
                return null;
            return base.FindSelectableOnUp();
        }

        /// <summary>
        /// See Selectable.FindSelectableOnDown
        /// </summary>
        public override Selectable FindSelectableOnDown()
        {
            if (navigation.mode == Navigation.Mode.Automatic && axis == Axis.Vertical)
                return null;
            return base.FindSelectableOnDown();
        }

        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }

        /// <summary>
        /// Sets the direction of this slider, optionally changing the layout as well.
        /// </summary>
        /// <param name="previousDirection">The previous direction of the slider.</param>
        /// <param name="newDirection">The new direction of the slider.</param>
        /// <param name="includeRectLayouts">Should the layout be flipped together with the slider direction</param>
        public void SetDirection(SlideDirection previousDirection, SlideDirection newDirection, bool includeRectLayouts)
        {
            bool previousReverse = reverseValue;
            Axis previousAxis = GetAxis(previousDirection);
            direction = newDirection;

            if (!includeRectLayouts)
                return;

            if (axis != previousAxis)
                RectTransformUtility.FlipLayoutAxes(transform as RectTransform, true, true);

            if (reverseValue != previousReverse)
                RectTransformUtility.FlipLayoutOnAxis(transform as RectTransform, (int)axis, true, true);
        }

        private void UpdateTargetProgressorMinMax()
        {
            if (!targetProgressor) return;
            targetProgressor.fromValue = minValue;
            targetProgressor.toValue = maxValue;
            targetProgressor.SetValueAt(value);
        }

        private void UpdateTargetProgressorValue()
        {
            if (!targetProgressor) return;
            targetProgressor.SetValueAt(value);
        }

        private void PlayTargetProgressorValue()
        {
            if (!targetProgressor) return;
            targetProgressor.PlayToValue(value);
        }

        #region Chainable Methods

        /// <summary> Reference a new TMP_Text to the value label </summary>
        /// <param name="label"> The TMP_Text to reference </param>
        public T SetValueLabel<T>(TMP_Text label) where T : UISlider
        {
            valueLabel = label;
            return (T)this;
        }

        /// <summary> Reference a new TMP_Text to the min value label </summary>
        /// <param name="label"> The TMP_Text to reference </param>
        public T SetMinValueLabel<T>(TMP_Text label) where T : UISlider
        {
            minValueLabel = label;
            return (T)this;
        }

        /// <summary> Reference a new TMP_Text to the max value label </summary>
        /// <param name="label"> The TMP_Text to reference </param>
        public T SetMaxValueLabel<T>(TMP_Text label) where T : UISlider
        {
            maxValueLabel = label;
            return (T)this;
        }

        /// <summary> Set a new current value for the slider </summary>
        /// <param name="newValue"> The new value to set </param>
        public T SetValue<T>(float newValue) where T : UISlider
        {
            value = newValue;
            return (T)this;
        }

        /// <summary> Set a new min value </summary>
        /// <param name="newMinValue"> The new min value </param>
        public T SetMinValue<T>(float newMinValue) where T : UISlider
        {
            minValue = newMinValue;
            return (T)this;
        }

        /// <summary> Set a new max value </summary>
        /// <param name="newMaxValue"> The new max value </param>
        public T SetMaxValue<T>(float newMaxValue) where T : UISlider
        {
            maxValue = newMaxValue;
            return (T)this;
        }

        /// <summary> Set a new default value (reset value) </summary>
        /// <param name="newResetValue"> The new default value (reset value) </param>
        public T SetDefaultValue<T>(float newResetValue) where T : UISlider
        {
            defaultValue = newResetValue;
            return (T)this;
        }

        /// <summary> Set a new target progressor to update when the value changes </summary>
        /// <param name="progressor"> The new target progressor to update when the value changes </param>
        public T SetTargetProgressor<T>(Progressor progressor) where T : UISlider
        {
            targetProgressor = progressor;
            return (T)this;
        }

        #endregion

        #region Static Methods

        /// <summary> Get all the registered sliders with the given category and name </summary>
        /// <param name="category"> UISlider category </param>
        /// <param name="name"> UISlider name (from the given category) </param>
        public static IEnumerable<UISlider> GetSliders(string category, string name) =>
            database.Where(slider => slider.Id.Category.Equals(category)).Where(slider => slider.Id.Name.Equals(name));

        /// <summary> Get all the registered sliders with the given category </summary>
        /// <param name="category"> UISlider category </param>
        public static IEnumerable<UISlider> GetAllSlidersInCategory(string category) =>
            database.Where(slider => slider.Id.Category.Equals(category));

        /// <summary> Get all the sliders that are active and enabled (all the visible/available sliders) </summary>
        public static IEnumerable<UISlider> GetAvailableSliders() =>
            database.Where(slider => slider.isActiveAndEnabled);

        /// <summary> Get the selected slider (if a slider is not selected, this method returns null) </summary>
        public static UISlider GetSelectedSlider() =>
            database.FirstOrDefault(slider => slider.isSelected);

        /// <summary> Select the slider with the given category and name (if it is active and enabled) </summary>
        /// <param name="category"> UISlider category </param>
        /// <param name="name"> UISlider name (from the given category) </param>
        public static bool SelectSlider(string category, string name)
        {
            UISlider slider = availableSliders.FirstOrDefault(b => b.Id.Category.Equals(category) & b.Id.Name.Equals(name));
            if (slider == null) return false;
            slider.Select();
            return true;
        }

        /// <summary> Update a value label text with the given value </summary>
        /// <param name="targetLabel"> Label to update </param>
        /// <param name="displayValue"> Value to display </param>
        private static void UpdateLabel(TMP_Text targetLabel, float displayValue)
        {
            if (targetLabel == null) return;
            targetLabel.text = displayValue.ToString(CultureInfo.InvariantCulture);
        }

        #endregion
    }
}
