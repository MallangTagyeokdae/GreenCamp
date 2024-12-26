using System;
using System.Collections;
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
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable UnusedMember.Local

namespace Doozy.Runtime.UIManager.Components
{
    /// <summary>
    /// Stepper component that can be used to increment or decrement a value.
    /// Has category/name id identifier.
    /// </summary>
    [AddComponentMenu("UI/Components/UI Stepper")]
    public partial class UIStepper : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        private const float TOLERANCE = 0.0001f;
        private const float DRAG_DISTANCE = 20f;
        private const float WAIT_BEFORE_STARTING = 0.6f;
        private const float WAIT_TIME = 0.4f;
        private const float WAIT_TIME_MIN = 0.04f;
        private const float WAIT_TIME_REDUCTION = 0.4f;

        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/UI/Components/UIStepper", false, 8)]
        private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        {
            GameObjectUtils.AddToScene<UIStepper>("UIStepper", false, true);
        }
        #endif

        /// <summary> UISteppers database </summary>
        public static HashSet<UIStepper> database { get; private set; } = new HashSet<UIStepper>();

        [ExecuteOnReload]
        private static void OnReload()
        {
            database ??= new HashSet<UIStepper>();
        }

        [ClearOnReload]
        private static SignalStream s_stream;
        /// <summary> UIStepper signal stream </summary>
        public static SignalStream stream => s_stream ??= SignalsService.GetStream(UISelectable.k_StreamCategory, nameof(UIStepper));

        /// <summary> All steppers that are active and enabled </summary>
        public static IEnumerable<UIStepper> availableSteppers => database.Where(item => item.isActiveAndEnabled);

        /// <summary> UIStepper identifier </summary>
        public UIStepperId Id;

        /// <summary>
        /// Drag direction for a UIStepper
        /// </summary>
        public enum Direction
        {
            /// <summary>
            /// Value increases when dragging to the right and decreases when dragging to the left
            /// </summary>
            Horizontal,

            /// <summary>
            /// Value increases when dragging up and decreases when dragging down
            /// </summary>
            Vertical
        }

        /// <summary>
        /// The initial and maximum time in seconds to wait before the value starts to auto repeat (increment/decrement)
        /// </summary>
        public float AutoRepeatWaitTime = WAIT_TIME;

        /// <summary>
        /// When the stepper is auto-repeating, the wait time between each increase/decrease will be reduced by multiplying the remaining wait time with this value until it reaches AutoRepeatMinWaitTime limit.
        /// This reduction makes the stepper feel more responsive and less laggy.
        /// </summary>
        public float AutoRepeatWaitTimeReduction = WAIT_TIME_REDUCTION;

        /// <summary> The minimum wait time between each increase/decrease when the stepper is auto-repeating </summary>
        public float AutoRepeatMinWaitTime = WAIT_TIME_MIN;

        [SerializeField] private UIButton MinusButton;
        /// <summary> Reference to a UIButton that will be used to decrease the value of the stepper </summary>
        public UIButton minusButton
        {
            get => MinusButton;
            private set
            {
                if (MinusButton != null)
                {
                    MinusButton.pressedState.stateEvent.Event.RemoveListener(OnMinusButtonClicked);
                    MinusButton.onPointerDownEvent.RemoveListener(OnMinusButtonDown);
                    MinusButton.onPointerUpEvent.RemoveListener(OnMinusButtonUp);
                }
                if (value != null)
                {
                    value.pressedState.stateEvent.Event.AddListener(OnMinusButtonClicked);
                    value.onPointerDownEvent.AddListener(OnMinusButtonDown);
                    value.onPointerUpEvent.AddListener(OnMinusButtonUp);
                }
                MinusButton = value;
            }
        }

        [SerializeField] private UIButton PlusButton;
        /// <summary> Reference to a UIButton that will be used to increase the value of the stepper </summary>
        public UIButton plusButton
        {
            get => PlusButton;
            private set
            {
                if (PlusButton != null)
                {
                    PlusButton.pressedState.stateEvent.Event.RemoveListener(OnPlusButtonClicked);
                    PlusButton.onPointerDownEvent.RemoveListener(OnPlusButtonDown);
                    PlusButton.onPointerUpEvent.RemoveListener(OnPlusButtonUp);
                }
                if (value != null)
                {
                    value.pressedState.stateEvent.Event.AddListener(OnPlusButtonClicked);
                    value.onPointerDownEvent.AddListener(OnPlusButtonDown);
                    value.onPointerUpEvent.AddListener(OnPlusButtonUp);
                }
                PlusButton = value;
            }
        }

        [SerializeField] private UIButton ResetButton;
        /// <summary> Reference to a UIButton that will reset the stepper value to its default value </summary>
        public UIButton resetButton
        {
            get => ResetButton;
            private set
            {
                if (ResetButton != null)
                {
                    ResetButton.pressedState.stateEvent.Event.RemoveListener(OnResetButtonClicked);
                }
                if (value != null)
                {
                    value.pressedState.stateEvent.Event.AddListener(OnResetButtonClicked);
                }
                ResetButton = value;
            }
        }

        [SerializeField] private TMP_Text TargetLabel;
        /// <summary> Reference to the value label that displays the current value </summary>
        public TMP_Text targetLabel
        {
            get => TargetLabel;
            private set
            {
                TargetLabel = value;
                UpdateValueLabel();
            }
        }

        [SerializeField] private float MinValue;
        /// <summary> The minimum value that the stepper can have </summary>
        public float minValue
        {
            get => MinValue;
            private set
            {
                MinValue = value.Round(ValuePrecision);
                MaxValue = MinValue > MaxValue ? MinValue : MaxValue;

                if (Value < MinValue)
                {
                    SetValue(MinValue);
                }

                UpdateTargetProgressorMinMax();
                UpdateTargetProgressorValue();

                UpdateTargetProgressorMinMax();
                UpdateTargetSliderValue(Value);
            }
        }

        [SerializeField] private float MaxValue = 1f;
        /// <summary> The maximum value that the stepper can reach </summary>
        public float maxValue
        {
            get => MaxValue;
            private set
            {
                MaxValue = value.Round(ValuePrecision);
                MinValue = MaxValue < MinValue ? MaxValue : MinValue;

                if (Value > MaxValue)
                {
                    SetValue(MaxValue);
                }

                UpdateTargetProgressorMinMax();
                UpdateTargetProgressorValue();

                UpdateTargetProgressorMinMax();
                UpdateTargetSliderValue(Value);
            }
        }

        [SerializeField] private float Value;
        /// <summary> The current value of the stepper </summary>
        public float value
        {
            get => Value;
            set => SetValue(value);
        }

        [SerializeField] private float DefaultValue;
        /// <summary> Reset value for the stepper </summary>
        public float defaultValue
        {
            get => DefaultValue;
            set => DefaultValue = Mathf.Clamp(value, minValue, maxValue);
        }

        [SerializeField] private float Step = 0.1f;
        /// <summary> Value by which the stepper will increase or decrease when the value is changed </summary>
        public float step
        {
            get => Step;
            private set
            {
                Step = value;
                SetValue(NearestStep(value));
                UpdateTargetSliderValue(value);
                stepValueChanged = true;
            }
        }

        [SerializeField] private UISlider TargetSlider;
        /// <summary>
        /// Reference to a UISlider that will be updated when the stepper value changes.
        /// The slider value also updates the stepper value, when it changes.
        /// </summary>
        public UISlider targetSlider
        {
            get => TargetSlider;
            private set
            {
                if (TargetSlider != null)
                {
                    TargetSlider.OnValueChanged.RemoveListener(OnTargetSliderValueChanged);
                    OnValueChanged.RemoveListener(UpdateTargetSliderValue);
                }

                if (value != null)
                {
                    value.OnValueChanged.AddListener(OnTargetSliderValueChanged);
                    OnValueChanged.AddListener(UpdateTargetSliderValue);
                }

                TargetSlider = value;
                UpdateTargetSliderMinMax();
                UpdateTargetSliderValue(Value);
            }
        }

        [SerializeField] private Progressor TargetProgressor;
        /// <summary> Reference to a Progressor that will be updated when the stepper value changes </summary>
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
        /// When true, the stepper will update the target progressor value with SetValueAt instead of PlayToValue.
        /// Basically, if true, the progressor will not animate when the stepper value changes. 
        /// </summary>
        public bool InstantProgressorUpdate = true;

        /// <summary> Automatically resets the value to the default value when the stepper OnEnable </summary>
        public bool ResetValueOnEnable = true;

        /// <summary> Number of decimal places to round the value to, if the step is not a whole number </summary>
        public int ValuePrecision = 2;

        /// <summary>
        /// Enable or disable the drag functionality for this stepper.
        /// </summary>
        public bool DragEnabled;

        /// <summary>
        /// Reference to the RectTransform used as the drag handle.
        /// </summary>
        [SerializeField] private RectTransform DragHandle;
        public RectTransform dragHandle
        {
            get => DragHandle;
            set
            {
                DragHandle = value;
                if (DragHandle)
                {
                    m_DragHandleInitialPosition = DragHandle.anchoredPosition;
                }
            }
        }

        [SerializeField] private Direction DragDirection = Direction.Horizontal;
        /// <summary> Drag direction for the Drag Handle to increase or decrease the value </summary>
        public Direction dragDirection
        {
            get => DragDirection;
            private set => DragDirection = value;
        }

        /// <summary>
        /// During a drag operation, this is the maximum distance the drag handle can be moved from the initial position.
        /// This also affects the speed of the value change (inversely proportional).
        /// The longer the distance the finer the control the user has to change the value.
        /// </summary>
        public float MaxDragDistance = DRAG_DISTANCE;

        /// <summary>
        /// Fired when the value changed.
        /// Returns the new value.
        /// </summary>
        public FloatEvent OnValueChanged = new FloatEvent();

        /// <summary>
        /// Fired when the value increases.
        /// Returns the difference between the new and old value.
        /// <para/> Example: if the previous value was 0.5 and the new value is 0.7, the returned value will be 0.2
        /// </summary>
        public FloatEvent OnValueIncremented = new FloatEvent();

        /// <summary>
        /// Fired when the value decreases.
        /// Returns the difference between the new and old value.
        /// <para/> Example: if the previous value was 10 and the new value is 5, the returned value will be -5
        /// </summary>
        public FloatEvent OnValueDecremented = new FloatEvent();

        /// <summary> Fired when the value was reset </summary>
        public ModyEvent OnValueReset = new ModyEvent();

        /// <summary> Fired when the value has reached the minimum value </summary>
        public ModyEvent OnValueReachedMin = new ModyEvent();

        /// <summary> Fired when the value has reached the maximum value </summary>
        public ModyEvent OnValueReachedMax = new ModyEvent();

        /// <summary>
        /// Coroutine called when the user is holding down the plus button.
        /// It's used to auto-repeat the increment action.
        /// </summary>
        private Coroutine autoIncrementCoroutine { get; set; }

        /// <summary>
        /// Coroutine called when the user is holding down the minus button.
        /// It's used to auto-repeat the decrement action.
        /// </summary>
        private Coroutine autoDecrementCoroutine { get; set; }

        /// <summary>
        /// Flag to indicate if the stepper is currently auto-repeating an increment action.
        /// </summary>
        private bool autoIncrementing { get; set; }

        /// <summary>
        /// Flag to indicate if the stepper is currently auto-repeating the decrement action.
        /// </summary>
        private bool autoDecrementing { get; set; }

        /// <summary>
        /// During a drag operation, this is the distance travelled from the start of the drag.
        /// It is used to determine the direction of the drag and the speed of the value change (the further the drag, the faster the value change). 
        /// </summary>
        private float draggedDistance =>
            DragDirection switch
            {
                Direction.Horizontal => dragHandle.anchoredPosition.x - m_DragHandleInitialPosition.x,
                Direction.Vertical   => dragHandle.anchoredPosition.y - m_DragHandleInitialPosition.y,
                _                    => throw new ArgumentOutOfRangeException()
            };

        /// <summary>
        /// During a drag operation, this is the time in seconds that the stepper will wait before changing the value (incrementing or decrementing).
        /// The value will change depending on how far, from the initial position, the drag area has been dragged.
        /// The further the drag area is dragged, the faster the value will change (thus this value will be lower).
        /// </summary>
        private float dragWaitTime
        {
            get
            {
                float distance = Mathf.Abs(draggedDistance);
                distance = Mathf.Clamp(distance, 0, MaxDragDistance);
                float dragRatio = 1f - distance / MaxDragDistance;
                float waitTime = Mathf.Clamp(dragRatio * AutoRepeatWaitTime, AutoRepeatMinWaitTime, AutoRepeatWaitTime);
                return waitTime;
            }
        }

        /// <summary>
        /// Initial position of the drag handle when the drag operation starts.
        /// </summary>
        private Vector2 m_DragHandleInitialPosition;

        /// <summary>
        /// Coroutine called when the user is dragging the drag handle in the increment direction (right or up).
        /// It's used to drag-repeat the decrement action.
        /// </summary>
        private Coroutine dragIncrementCoroutine { get; set; }

        /// <summary>
        /// Coroutine called when the user is dragging the drag handle in the decrement direction (left or down).
        /// It's used to drag-repeat the decrement action.
        /// </summary>
        private Coroutine dragDecrementCoroutine { get; set; }

        /// <summary>
        /// Flag to indicate if the stepper is currently drag-repeating an increment action.
        /// </summary>
        private bool dragIncrementing { get; set; }

        /// <summary>
        /// Flag to indicate if the stepper is currently drag-repeating the decrement action.
        /// </summary>
        private bool dragDecrementing { get; set; }

        /// <summary>
        /// Flag to indicate if the stepper DragHandle can be dragged.
        /// </summary>
        private bool canDrag { get; set; }

        /// <summary>
        /// Flag to indicate if the stepper DragHandle is currently being dragged.
        /// </summary>
        private bool isDragging { get; set; }

        /// <summary>
        /// Flag to indicate that the stepper's step vale has changed.
        /// </summary>
        private bool stepValueChanged { get; set; }

        protected UIStepper()
        {
            Id = new UIStepperId();
        }

        protected virtual void OnValidate()
        {
            SetValue(value);
        }

        protected virtual void Awake()
        {
            database.Add(this);
        }

        protected virtual void Start()
        {
            UpdateValueLabel();
        }

        protected virtual void OnEnable()
        {
            database.Remove(null);
            if (!Application.isPlaying) return;

            if (minusButton != null)
            {
                minusButton.pressedState.stateEvent.Event.AddListener(OnMinusButtonClicked);
                minusButton.onPointerUpEvent.AddListener(OnMinusButtonUp);
                minusButton.onPointerDownEvent.AddListener(OnMinusButtonDown);
            }

            if (plusButton != null)
            {
                plusButton.pressedState.stateEvent.Event.AddListener(OnPlusButtonClicked);
                plusButton.onPointerUpEvent.AddListener(OnPlusButtonUp);
                plusButton.onPointerDownEvent.AddListener(OnPlusButtonDown);
            }

            if (resetButton != null)
            {
                resetButton.pressedState.stateEvent.Event.AddListener(OnResetButtonClicked);
            }

            if (TargetSlider)
            {
                TargetSlider.OnValueChanged.AddListener(OnTargetSliderValueChanged);
                OnValueChanged.AddListener(UpdateTargetSliderValue);
            }

            UpdateTargetProgressorMinMax();
            UpdateTargetSliderMinMax();
            if (ResetValueOnEnable)
            {
                ResetValue();
            }
            else
            {
                UpdateTargetProgressorValue();
                UpdateTargetSliderValue(value);
            }

            if (dragHandle)
            {
                m_DragHandleInitialPosition = dragHandle.anchoredPosition;
            }

            canDrag = true;
            isDragging = false;
        }

        protected virtual void OnDisable()
        {
            database.Remove(null);
            if (!Application.isPlaying) return;

            if (plusButton != null)
            {
                plusButton.pressedState.stateEvent.Event.RemoveListener(OnPlusButtonClicked);
                plusButton.onPointerUpEvent.RemoveListener(OnPlusButtonUp);
                plusButton.onPointerDownEvent.RemoveListener(OnPlusButtonDown);
            }

            if (minusButton != null)
            {
                minusButton.pressedState.stateEvent.Event.RemoveListener(OnMinusButtonClicked);
                minusButton.onPointerUpEvent.RemoveListener(OnMinusButtonUp);
                minusButton.onPointerDownEvent.RemoveListener(OnMinusButtonDown);
            }

            if (resetButton != null)
            {
                resetButton.pressedState.stateEvent.Event.RemoveListener(OnResetButtonClicked);
            }

            autoIncrementing = false;
            if (autoIncrementCoroutine != null)
            {
                StopCoroutine(autoIncrementCoroutine);
            }

            autoDecrementing = false;
            if (autoDecrementCoroutine != null)
            {
                StopCoroutine(autoDecrementCoroutine);
            }

            dragIncrementing = false;
            if (dragIncrementCoroutine != null)
            {
                StopCoroutine(dragIncrementCoroutine);
            }

            dragDecrementing = false;
            if (dragDecrementCoroutine != null)
            {
                StopCoroutine(dragDecrementCoroutine);
            }

            UpdateTargetProgressorValue();
            UpdateTargetSliderValue(value);

            if (TargetSlider)
            {
                TargetSlider.OnValueChanged.RemoveListener(OnTargetSliderValueChanged);
                OnValueChanged.RemoveListener(UpdateTargetSliderValue);
            }
        }

        protected virtual void OnDestroy()
        {
            database.Remove(null);
            database.Remove(this);
        }

        protected virtual void OnMinusButtonClicked()
        {
            DecrementValue();
            canDrag = true;
        }

        protected virtual void OnPlusButtonClicked()
        {
            IncrementValue();
            canDrag = true;
        }

        protected virtual void OnPlusButtonDown()
        {
            StopAutoIncrement();
            if (isDragging) return;
            canDrag = false;
            StartAutoIncrement();
        }

        protected virtual void OnPlusButtonUp()
        {
            StopAutoIncrement();
            canDrag = true;
        }

        protected virtual void OnMinusButtonDown()
        {
            StopAutoDecrement();
            if (isDragging) return;
            canDrag = false;
            StartAutoDecrement();
        }

        protected virtual void OnMinusButtonUp()
        {
            StopAutoDecrement();
            canDrag = true;
        }

        protected virtual void OnResetButtonClicked()
        {
            ResetValue();
        }

        protected virtual void OnTargetSliderValueChanged(float sliderValue)
        {
            float nearestStepValue = NearestStep(sliderValue);
            TargetSlider.SetValueWithoutNotify(nearestStepValue);
            SetValue(nearestStepValue);
        }

        /// <summary> Reset the current int or float value, depending on the stepper's value type, to the default value. </summary>
        public void ResetValue()
        {
            SetValue(defaultValue);
            OnValueReset.Execute();
        }

        /// <summary> Set the current float value to the given value </summary>
        /// <param name="newValue"> New value </param>
        public void SetValue(float newValue)
        {
            bool valueChanged = Math.Abs(Value - newValue) > TOLERANCE;              //check if the value has changed
            Value = Mathf.Clamp(newValue, minValue, maxValue).Round(ValuePrecision); //set the new value
            if (stepValueChanged)
            {
                Value = NearestStep(Value);
                stepValueChanged = false;
            }
            UpdateValueLabel(); //update the value label
            if (valueChanged)
            {
                OnValueChanged.Invoke(Value); //invoke the OnValueChanged event only if the value has changed

                if (InstantProgressorUpdate)
                {
                    UpdateTargetProgressorValue();
                }
                else
                {
                    PlayTargetProgressorValue();
                }

                UpdateTargetSliderValue(Value);
                stream.SendSignal(new UIStepperSignalData(Id.Category, Id.Name, StepperState.ValueChanged, this));
            }

            if (Value <= minValue)
            {
                //value is equal to the min value
                //invoke the OnValueReachedMin event if the value is equal to the min value
                //disable the minus button if the value is equal to the min value

                OnValueReachedMin.Execute();
                stream.SendSignal(new UIStepperSignalData(Id.Category, Id.Name, StepperState.ReachedMinValue, this));
                if (minusButton != null && minusButton.interactable)
                {
                    minusButton.interactable = false;
                }
            }
            else
            {
                //value is not equal to the min value
                //enable the minus button if the value is not equal to the min value

                if (minusButton != null && !minusButton.interactable)
                {
                    minusButton.interactable = true;
                }
            }

            if (Value >= maxValue)
            {
                //value is equal to the max value
                //invoke the OnValueReachedMax event if the value is equal to the max value
                //disable the plus button if the value is equal to the max value

                OnValueReachedMax.Execute();
                stream.SendSignal(new UIStepperSignalData(Id.Category, Id.Name, StepperState.ReachedMaxValue, this));
                if (plusButton != null && plusButton.interactable)
                {
                    plusButton.interactable = false;
                }
            }
            else
            {
                //value is not equal to the max value
                //enable the plus button if the value is not equal to the max value

                if (plusButton != null && !plusButton.interactable)
                {
                    plusButton.interactable = true;
                }
            }
        }

        /// <summary> Increment the value by the step value </summary>
        public void IncrementValue()
        {
            IncrementValue(step);
        }

        /// <summary> Increment the value </summary>
        /// <param name="increment"> Increment value </param>
        public void IncrementValue(float increment)
        {
            bool currentValueIsMax = Math.Abs(value - maxValue) < TOLERANCE; //we need to know this before we increment the value
            if (!currentValueIsMax)
            {
                OnValueIncremented.Invoke(increment); //invoke the OnValueIncremented event only if the value was not already at max
                //stream.SendSignal(new UIStepperSignalData(Id.Category, Id.Name, StepperState.ValueIncremented, this));
            }
            SetValue(value + increment); //increment value
        }

        /// <summary> Decrement the value by the step value </summary>
        public void DecrementValue()
        {
            DecrementValue(step);
        }

        /// <summary> Decrement the value </summary>
        /// <param name="decrement"> Decrement value </param>
        public void DecrementValue(float decrement)
        {
            bool currentValueIsMin = Math.Abs(value - minValue) < TOLERANCE; //we need to know this before we decrement the value
            if (!currentValueIsMin)
            {
                OnValueDecremented.Invoke(-decrement); //invoke the OnValueDecremented event only if the value was not already at min
                //stream.SendSignal(new UIStepperSignalData(Id.Category, Id.Name, StepperState.ValueDecremented, this));
            }
            SetValue(value - decrement); //decrement value
        }

        /// <summary> Update the value label text with the current value </summary>
        public void UpdateValueLabel()
        {
            if (targetLabel == null) return;
            targetLabel.text = value.ToString(CultureInfo.InvariantCulture);
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

        private void UpdateTargetSliderMinMax()
        {
            if (targetSlider == null) return;
            targetSlider.minValue = minValue;
            targetSlider.maxValue = maxValue;
        }

        private void UpdateTargetSliderValue(float newValue)
        {
            if (targetSlider == null) return;
            targetSlider.SetValueWithoutNotify(newValue);
        }

        #region Chainable Methods

        /// <summary> Reference a new UIButton to the minus button </summary>
        /// <param name="button"> The UIButton to reference </param>
        public T SetMinusButton<T>(UIButton button) where T : UIStepper
        {
            minusButton = button;
            return (T)this;
        }

        /// <summary> Reference a new UIButton to the plus button </summary>
        /// <param name="button"> The UIButton to reference </param>
        public T SetPlusButton<T>(UIButton button) where T : UIStepper
        {
            plusButton = button;
            return (T)this;
        }

        /// <summary> Reference a new TMP_Text to the value label </summary>
        /// <param name="label"> The TMP_Text to reference </param>
        public T SetValueLabel<T>(TMP_Text label) where T : UIStepper
        {
            targetLabel = label;
            return (T)this;
        }

        /// <summary> Reference a new UIButton to the reset button </summary>
        /// <param name="button"> The UIButton to reference </param>
        public T SetResetButton<T>(UIButton button) where T : UIStepper
        {
            resetButton = button;
            return (T)this;
        }

        /// <summary> Set a new min value </summary>
        /// <param name="newMinValue"> The new min value </param>
        public T SetMinValue<T>(float newMinValue) where T : UIStepper
        {
            minValue = newMinValue;
            return (T)this;
        }

        /// <summary> Set a new max value </summary>
        /// <param name="newMaxValue"> The new max value </param>
        public T SetMaxValue<T>(float newMaxValue) where T : UIStepper
        {
            maxValue = newMaxValue;
            return (T)this;
        }

        /// <summary> Set a new default value (reset value) </summary>
        /// <param name="newResetValue"> The new default value (reset value) </param>
        public T SetDefaultValue<T>(float newResetValue) where T : UIStepper
        {
            defaultValue = newResetValue;
            return (T)this;
        }

        /// <summary> Set a new target progressor to update when the value changes </summary>
        /// <param name="progressor"> The new target progressor to update when the value changes </param>
        public T SetTargetProgressor<T>(Progressor progressor) where T : UIStepper
        {
            targetProgressor = progressor;
            return (T)this;
        }

        /// <summary> Set a new direction for the stepper </summary>
        /// <param name="direction"> The new direction for the stepper </param>
        public T SetStepperDirection<T>(Direction direction) where T : UIStepper
        {
            DragDirection = direction;
            return (T)this;
        }

        #endregion

        /// <summary>
        /// Flag used to determine if a drag can occur
        /// </summary>
        private bool cannotDrag => !DragEnabled || !canDrag;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (cannotDrag) return;
            isDragging = true;
            // m_DragHandleInitialPosition = DragHandle.anchoredPosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (cannotDrag) return;
            isDragging = false;
            dragHandle.anchoredPosition = m_DragHandleInitialPosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (cannotDrag) return;
            if (!isDragging) return;

            float iX = m_DragHandleInitialPosition.x;
            float iY = m_DragHandleInitialPosition.y;
            switch (DragDirection)
            {
                case Direction.Horizontal:
                    float x = dragHandle.anchoredPosition.x + eventData.delta.x;
                    x = Mathf.Clamp(x, iX - MaxDragDistance, iX + MaxDragDistance);
                    dragHandle.anchoredPosition = new Vector2(x, m_DragHandleInitialPosition.y);
                    break;
                case Direction.Vertical:
                    float y = dragHandle.anchoredPosition.y + eventData.delta.y;
                    y = Mathf.Clamp(y, iY - MaxDragDistance, iY + MaxDragDistance);
                    dragHandle.anchoredPosition = new Vector2(m_DragHandleInitialPosition.x, y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void LateUpdate()
        {
            if (!isDragging)
            {
                if (dragIncrementing) StopDragIncrement();
                if (dragDecrementing) StopDragDecrement();
                return;
            }

            if (autoIncrementing) StopAutoIncrement();
            if (autoDecrementing) StopAutoDecrement();

            if (draggedDistance > 0)
            {
                if (dragDecrementing) StopDragDecrement();
                if (dragIncrementing) return;
                StartDragIncrement();
            }
            else
            {
                if (dragIncrementing) StopDragIncrement();
                if (dragDecrementing) return;
                StartDragDecrement();
            }
        }

        #region Increment

        private bool CanIncrementValue() =>
            value < maxValue;

        #region Auto Increment

        private void StartAutoIncrement()
        {
            StopAutoIncrement();
            autoIncrementCoroutine = StartCoroutine(AutoIncrementValue());
        }

        private void StopAutoIncrement()
        {
            autoIncrementing = false;
            if (autoIncrementCoroutine == null) return;
            StopCoroutine(autoIncrementCoroutine);
        }

        private IEnumerator AutoIncrementValue()
        {
            autoIncrementing = true;
            yield return new WaitForSecondsRealtime(WAIT_BEFORE_STARTING);
            float waitTime = AutoRepeatWaitTime;
            while (CanIncrementValue())
            {
                IncrementValue();
                yield return new WaitForSecondsRealtime(waitTime);
                waitTime = Mathf.Clamp(waitTime * AutoRepeatWaitTimeReduction, AutoRepeatMinWaitTime, AutoRepeatWaitTime);
            }
            autoIncrementing = false;
            autoIncrementCoroutine = null;
        }

        #endregion

        #region Drag Increment

        private void StartDragIncrement()
        {
            StopDragIncrement();
            dragIncrementCoroutine = StartCoroutine(DragIncrementValue());
        }

        private void StopDragIncrement()
        {
            dragIncrementing = false;
            if (dragIncrementCoroutine == null) return;
            StopCoroutine(dragIncrementCoroutine);
        }

        private IEnumerator DragIncrementValue()
        {
            dragIncrementing = true;
            while (CanIncrementValue())
            {
                IncrementValue();
                yield return new WaitForSecondsRealtime(dragWaitTime);
            }
            dragIncrementing = false;
            dragIncrementCoroutine = null;
        }

        #endregion

        #endregion

        #region Decrement

        private bool CanDecrementValue() =>
            value > minValue;

        #region Auto Decrement

        private void StartAutoDecrement()
        {
            StopAutoDecrement();
            autoDecrementCoroutine = StartCoroutine(AutoDecrementValue());
        }

        private void StopAutoDecrement()
        {
            autoDecrementing = false;
            if (autoDecrementCoroutine == null) return;
            StopCoroutine(autoDecrementCoroutine);
        }

        private IEnumerator AutoDecrementValue()
        {
            autoDecrementing = true;
            yield return new WaitForSecondsRealtime(WAIT_BEFORE_STARTING);
            float waitTime = AutoRepeatWaitTime;
            while (CanDecrementValue())
            {
                DecrementValue();
                yield return new WaitForSecondsRealtime(waitTime);
                waitTime = Mathf.Clamp(waitTime * AutoRepeatWaitTimeReduction, AutoRepeatMinWaitTime, AutoRepeatWaitTime);
            }
            autoDecrementing = false;
            autoDecrementCoroutine = null;
        }

        #endregion

        #region Drag Decrement

        private void StartDragDecrement()
        {
            StopDragDecrement();
            dragDecrementCoroutine = StartCoroutine(DragDecrementValue());
        }

        private void StopDragDecrement()
        {
            dragDecrementing = false;
            if (dragDecrementCoroutine == null) return;
            StopCoroutine(dragDecrementCoroutine);
        }

        private IEnumerator DragDecrementValue()
        {
            dragDecrementing = true;
            while (CanDecrementValue())
            {
                DecrementValue();
                yield return new WaitForSecondsRealtime(dragWaitTime);
            }
            dragDecrementing = false;
            dragDecrementCoroutine = null;
        }

        #endregion

        #endregion

        /// <summary> Get the nearest value to the given value that is a multiple of the step size. </summary>
        /// <param name="uncorrectedValue"> Value that has not been corrected. </param>
        private float NearestStep(float uncorrectedValue)
        {
            if (Step == 0) return uncorrectedValue;
            return (int)Math.Round(uncorrectedValue / (double)step, MidpointRounding.AwayFromZero) * step;
        }
    }
}
