// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Global;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers.Internal;
using Doozy.Runtime.UIManager.Input;
using Doozy.Runtime.UIManager.ScriptableObjects;
using Doozy.Runtime.UIManager.Triggers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if INPUT_SYSTEM_PACKAGE
using UnityEngine.InputSystem.UI;
#endif

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable PartialTypeWithSinglePart

namespace Doozy.Runtime.UIManager.Containers
{
    /// <summary>
    /// Specialized container that behaves like a tooltip,
    /// with parenting, tracking and positioning options.
    /// </summary>
    [AddComponentMenu("UI/Containers/UITooltip")]
    [DisallowMultipleComponent]
    public partial class UITooltip : UIContainerComponent<UITooltip>
    {
        /// <summary> Describes where a tooltip can be instantiated under </summary>
        public enum Parenting
        {
            /// <summary> The parent of the tooltip will be the TooltipCanvas </summary>
            TooltipsCanvas = 0,

            /// <summary> The parent of the tooltip will be the RectTransform of UITooltipTrigger that triggered the tooltip </summary>
            TooltipTrigger = 1,

            /// <summary> The parent of the tooltip will be the RectTransform of the GameObject that has the given UITagId </summary>
            UITag = 2
        }

        /// <summary> Describes the how the tooltip behaves when it is shown </summary>
        public enum Tracking
        {
            /// <summary> The tooltip will be shown at a predefined position and it will stay there until it is hidden </summary>
            Disabled = 0,

            /// <summary> The tooltip will follow the pointer until it is hidden </summary>
            FollowPointer = 1,

            /// <summary> The tooltip will follow the GameObject of the trigger until it is hidden </summary>
            FollowTrigger = 2,

            /// <summary> The tooltip will follow the GameObject of the given UITag until it is hidden </summary>
            FollowTarget = 3
        }

        /// <summary> Describes where the tooltip should be positioned relative to the tracked target </summary>
        public enum Positioning
        {
            TopLeft = 0,
            TopCenter = 1,
            TopRight = 2,

            MiddleLeft = 3,
            MiddleCenter = 4,
            MiddleRight = 5,

            BottomLeft = 6,
            BottomCenter = 7,
            BottomRight = 8
        }

        /// <summary> Maximum sorting order value for tooltips </summary>
        public const int k_MaxSortingOrder = 32767;
        /// <summary> Default tooltip name </summary>
        public const string k_DefaultTooltipName = "None";
        /// <summary> Default category used by the UITagId to identify the default Tooltip Canvas </summary>
        public const string k_DefaultTooltipCanvasUITagCategory = "UITooltip";
        /// <summary> Default name used by the UITagId to identify the default Tooltip Canvas </summary>
        public const string k_DefaultTooltipCanvasUITagName = "Canvas";

        /// <summary> Get all the visible tooltips (returns all tooltips that are either in the isVisible or isShowing state) </summary>
        public static IEnumerable<UITooltip> visibleTooltips =>
            database.Where(item => item.isVisible || item.isShowing);

        private LayoutElement m_LayoutElement;
        /// <summary> Reference to the LayoutElement attached to this GameObject </summary>
        public LayoutElement layoutElement =>
            m_LayoutElement
                ? m_LayoutElement
                : m_LayoutElement = GetComponent<LayoutElement>() ?? gameObject.AddComponent<LayoutElement>();

        /// <summary> Internal static reference to the default canvas used to display tooltips </summary>
        [ClearOnReload]
        private static Canvas s_tooltipsCanvas;
        /// <summary>
        /// Static reference to the default canvas used to display tooltips (tooltips get parented to this canvas).
        /// <para/> You can override the default canvas by referencing another canvas (at runtime) to be used as the default canvas.
        /// </summary>
        public static Canvas tooltipsCanvas
        {
            get
            {
                if (s_tooltipsCanvas != null) return s_tooltipsCanvas;
                //look for UITag
                UITag uiTag = UITag.GetTags(k_DefaultTooltipCanvasUITagCategory, k_DefaultTooltipCanvasUITagName).FirstOrDefault();
                if (uiTag != null)
                {
                    s_tooltipsCanvas = uiTag.GetComponent<Canvas>();
                    if (s_tooltipsCanvas != null)
                        return s_tooltipsCanvas;
                }
                //create Tooltip Canvas
                s_tooltipsCanvas = new GameObject("Tooltips Canvas").AddComponent<Canvas>();
                s_tooltipsCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                s_tooltipsCanvas.overrideSorting = true;
                s_tooltipsCanvas.sortingOrder = k_MaxSortingOrder;
                //add the default tag
                uiTag = s_tooltipsCanvas.gameObject.AddComponent<UITag>();
                uiTag.Id.Category = k_DefaultTooltipCanvasUITagCategory;
                uiTag.Id.Name = k_DefaultTooltipCanvasUITagName;
                return s_tooltipsCanvas;
            }
            set => s_tooltipsCanvas = value;
        }

        #region Tooltip rootCanvas rootCanvasRectTransform

        private Canvas m_TooltipRootCanvas;
        /// <summary> Internal reference to the root canvas of this tooltip </summary>
        internal Canvas tooltipRootCanvas
        {
            get => m_TooltipRootCanvas;
            set
            {
                tooltipRootCanvasRectTransform = null;
                m_TooltipRootCanvas = value;
                if (value == null) return;
                tooltipRootCanvasRectTransform = value.GetComponent<RectTransform>();
            }
        }

        /// <summary> Internal reference to the RectTransform of the root canvas of this tooltip </summary>
        internal RectTransform tooltipRootCanvasRectTransform { get; private set; }

        #endregion

        #region Target RectTransform rootCanvas rootCanvasRectTransform

        private RectTransform m_TargetRectTransform;
        /// <summary> Internal reference to the current target RectTransform </summary>
        internal RectTransform targetRectTransform
        {
            get => m_TargetRectTransform;
            set
            {
                targetRootCanvas = null;
                targetRootCanvasRectTransform = null;
                m_TargetRectTransform = value;
                if (value == null) return;
                targetRootCanvas = value.GetComponentInParent<Canvas>().rootCanvas;
                targetRootCanvasRectTransform = targetRootCanvas.GetComponent<RectTransform>();
            }
        }

        /// <summary> Internal reference to the current target root canvas </summary>
        internal Canvas targetRootCanvas { get; private set; }

        /// <summary> Internal reference to the current target root canvas RectTransform </summary>
        internal RectTransform targetRootCanvasRectTransform { get; private set; }

        #endregion

        /// <summary> List of all the labels this UITooltip has </summary>
        public List<TextMeshProUGUI> Labels = new List<TextMeshProUGUI>();

        /// <summary> TRUE if the tooltip has at least one TextMeshProUGUI label reference </summary>
        public bool hasLabels => Labels.RemoveNulls().Count > 0;

        /// <summary> List of all the images this UITooltip has </summary>
        public List<Image> Images = new List<Image>();

        /// <summary> TRUE if the tooltip has at least one Image reference </summary>
        public bool hasImages => Images.RemoveNulls().Count > 0;

        /// <summary> List of all the buttons this UITooltip has </summary>
        public List<UIButton> Buttons = new List<UIButton>();

        /// <summary> TRUE if the tooltip has at least one UIButton reference </summary>
        public bool hasButtons => Buttons.RemoveNulls().Count > 0;

        /// <summary> Tooltip rectTransform.rect </summary>
        public Rect rect => rectTransform.rect;

        /// <summary> Tooltip rectTransform.rect.width </summary>
        public float width => rect.width;

        /// <summary> Tooltip rectTransform.rect.height </summary>
        public float height => rect.height;

        /// <summary> Tooltip rectTransform.pivot.x </summary>
        public float pivotX => rectTransform.pivot.x;

        /// <summary> Tooltip rectTransform.pivot.y </summary>
        public float pivotY => rectTransform.pivot.y;

        /// <summary> Set where a tooltip should be instantiated under </summary>
        public Parenting ParentMode = Parenting.TooltipsCanvas;

        /// <summary> Set how the tooltip behaves when it is shown </summary>
        public Tracking TrackingMode = Tracking.Disabled;

        /// <summary> Set where the tooltip should be positioned relative to the tracked target </summary>
        public Positioning PositionMode = Positioning.MiddleCenter;

        /// <summary>
        /// Id used to identify the designated parent where the tooltip should be parented under,
        /// after is has been instantiated
        /// </summary>
        public UITagId ParentTag;

        /// <summary>
        /// Id used to identify the follow target when the tracking mode is set to FollowTarget
        /// </summary>
        public UITagId FollowTag;

        /// <summary>
        /// Set the offset applied to the tooltip position,
        /// after all the positioning has been applied
        /// </summary>
        public Vector3 PositionOffset = Vector3.zero;

        /// <summary>
        /// Set a maximum width for the tooltip.
        /// If the value is 0, the tooltip will be automatically sized to fit the content")
        /// </summary>
        public float MaximumWidth;

        /// <summary>
        /// Keep the tooltip in screen at all times, while it is shown
        /// </summary>
        public bool KeepInScreen = true;

        /// <summary>
        /// Enable override sorting and set the sorting order to the maximum value,
        /// for the Canvas component attached to this tooltip
        /// </summary>
        public bool OverrideSorting = true;

        /// <summary>
        /// Hide (close) the tooltip when the user clicks on any of the UIButton references.
        /// <para/> At runtime, a 'hide tooltip' event will be added to all the referenced UIButtons (if any).
        /// </summary>
        public bool HideOnAnyButton = true;

        /// <summary> Set the next 'Back' button event to hide (close) this tooltip </summary>
        public bool HideOnBackButton = true;

        #if INPUT_SYSTEM_PACKAGE
        private static InputSystemUIInputModule s_inputSystemUIInputModule;
        public static InputSystemUIInputModule inputModule
        {
            get
            {
                if (s_inputSystemUIInputModule != null) return s_inputSystemUIInputModule;
                if (EventSystem.current == null)
                    return null;
                s_inputSystemUIInputModule = EventSystem.current.GetComponent<InputSystemUIInputModule>();
                return s_inputSystemUIInputModule;
            }
        }
        #endif

        /// <summary> Pointer's current position </summary>
        public static Vector2 pointerPosition
        {
            get
            {
                #pragma warning disable CS0162
                #if LEGACY_INPUT_MANAGER
                {
                    return UnityEngine.Input.mousePosition;
                }
                #elif INPUT_SYSTEM_PACKAGE
                {
                    return inputModule.point.action.ReadValue<Vector2>();
                }
                #endif
                // ReSharper disable once HeuristicUnreachableCode
                return Vector2.zero;
                #pragma warning restore CS0162
            }
        }

        /// <summary> Internal flag to track if show has been called </summary>
        private bool showHasBeenCalled { get; set; }
        /// <summary> Internal flag to track if hide has been called </summary>
        private bool hideHasBeenCalled { get; set; }
        /// <summary> Internal flag to check it the tooltip has a move animator for the show animation </summary>
        private bool showHasMovement { get; set; }
        /// <summary> Internal flag to check it the tooltip has a move animator for the hide animation </summary>
        private bool hideHasMovement { get; set; }
        /// <summary> Internal reference to the tooltip's show move reaction (if any) </summary>        
        private UIMoveReaction showMoveReaction { get; set; }
        /// <summary> Internal reference to the tooltip's hide move reaction (if any) </summary>
        private UIMoveReaction hideMoveReaction { get; set; }

        /// <summary>
        /// Internal flag used to keep track if the hide event was added to the buttons.
        /// This is needed to avoid adding the event multiple times to the same button.
        /// </summary>
        private bool addedHideEventToButtons { get; set; }

        /// <summary> Internal flag used to keep track if this tooltip added a PointerClickTrigger to it </summary>
        private bool addedHideOnClick { get; set; }

        #region Parent Info

        public RectTransform parentRectTransform { get; internal set; }

        #endregion

        #region Trigger

        private UITooltipTrigger m_Trigger;
        public UITooltipTrigger trigger
        {
            get => m_Trigger;
            set
            {
                triggerRectTransform = null;
                m_Trigger = value;
                if (value == null) return;
                triggerRectTransform = value.GetComponent<RectTransform>();
            }
        }

        /// <summary> RectTransform of the trigger to track when the tooltip is shown and tracking is set to FollowTrigger </summary>
        public RectTransform triggerRectTransform { get; private set; }

        /// <summary> TRUE if the tooltip has a trigger reference </summary>
        public bool hasTrigger => trigger != null;

        /// <summary> TRUE if the trigger transform has a RectTransform </summary>
        public bool hasTriggerRectTransform => triggerRectTransform != null;

        #endregion

        #region Follow Target

        private GameObject m_FollowTarget;
        /// <summary> GameObject to track when the tooltip is shown and tracking is set to FollowTarget </summary>
        public GameObject followTarget
        {
            get => m_FollowTarget;
            set
            {
                followTargetRectTransform = null;
                m_FollowTarget = value;
                if (value == null) return;
                followTargetRectTransform = value.GetComponent<RectTransform>();
            }
        }

        /// <summary> RectTransform of the follow target to track when the tooltip is shown and tracking is set to FollowTarget </summary>
        public RectTransform followTargetRectTransform { get; private set; }

        /// <summary> TRUE if the tooltip has a follow target reference </summary>
        public bool hasFollowTarget => followTarget != null;

        /// <summary> TRUE if the follow target transform has a RectTransform </summary>
        public bool hasFollowTargetRectTransform => followTargetRectTransform != null;

        #endregion

        /// <summary> 'Back' button signal receiver used to trigger the hiding of this UITooltip if HideOnBackButton is TRUE </summary>
        public SignalReceiver backButtonReceiver { get; set; }

        protected override void Awake()
        {
            base.Awake();
            addedHideEventToButtons = false;

            //initialize the 'Back' button signal receiver
            backButtonReceiver = new SignalReceiver().SetOnSignalCallback(signal =>
            {
                if (!HideOnBackButton) return;
                if (isHidden || isHiding) return;
                Hide();
            });
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            //connect to the 'Back' button secondary signal stream
            //(this stream ignores the disabled state for the 'Back' button)
            BackButton.streamIgnoreDisabled.ConnectReceiver(backButtonReceiver);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            //disconnect from the 'Back' button secondary signal stream
            //(this stream ignores the disabled state for the 'Back' button)
            BackButton.streamIgnoreDisabled.DisconnectReceiver(backButtonReceiver);
        }

        private void LateUpdate()
        {
            CheckIfShowOrHideHaveMoveReactions();

            if (isShowing && showHasMovement || isHiding && hideHasMovement)
            {
                ApplyPositioning();
                return;
            }

            ApplyTracking();
            ApplyPositioning();
            ApplyKeepInScreen();
            SetCustomStartPosition(rectTransform.anchoredPosition3D, false);
        }

        /// <summary> Validate the tooltip's settings </summary>
        public virtual void Validate()
        {
            Labels.RemoveNulls();
            Images.RemoveNulls();
            Buttons.RemoveNulls();
        }

        public override void Show()
        {
            UpdateTarget();
            ApplyMaximumWidth();
            ApplyHideOnAnyButton();
            base.Show();
            Coroutiner.ExecuteLater
            (
                () =>
                {
                    if (this == null) return;
                    this.ApplyOverrideSorting();
                },
                3 //number of frames to wait
            );
        }

        public override void InstantShow(bool triggerCallbacks)
        {
            UpdateTarget();
            ApplyMaximumWidth();
            ApplyHideOnAnyButton();
            base.InstantShow(triggerCallbacks);
            this.ApplyOverrideSorting();
        }

        /// <summary>
        /// Update the reference to the root canvas and RectTransform of the root canvas of this tooltip's current target, if any. 
        /// </summary>
        public void UpdateTarget()
        {
            updateTarget = false;
            targetRectTransform = null;

            switch (TrackingMode)
            {
                case Tracking.Disabled:
                case Tracking.FollowPointer:
                    return;
                case Tracking.FollowTrigger:
                    targetRectTransform = triggerRectTransform;
                    break;
                case Tracking.FollowTarget:
                    targetRectTransform = followTargetRectTransform;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #region Public Methods

        /// <summary>
        /// Get a parent reference for the tooltip according to the tooltip's current ParentMode setting.
        /// <para/> This is not the same as the tooltip's transform.parent, which is the parent of the tooltip's GameObject.
        /// </summary>
        public RectTransform GetParent()
        {
            RectTransform parent;

            switch (ParentMode)
            {
                case Parenting.TooltipsCanvas:
                    parent = tooltipsCanvas.GetComponent<RectTransform>();
                    break;
                case Parenting.TooltipTrigger:
                    if (trigger == null)
                    {
                        Debug.Log
                        (
                            "[Tooltip] Parenting mode set to 'Tooltip Trigger' but no tooltip trigger is set." +
                            "Used the TooltipCanvas as parent instead."
                        );
                        parent = tooltipsCanvas.GetComponent<RectTransform>();
                        break;
                    }
                    parent = trigger.GetComponent<RectTransform>();
                    if (parent == null)
                    {
                        Debug.Log
                        (
                            "[Tooltip] Parenting mode set to 'Tooltip Trigger' but the tooltip trigger has no RectTransform component." +
                            "Used the TooltipCanvas as parent instead."
                        );
                        parent = tooltipsCanvas.GetComponent<RectTransform>();
                    }
                    break;
                case Parenting.UITag:
                    if (ParentTag == null)
                    {
                        Debug.Log
                        (
                            "[Tooltip] Parenting mode set to 'UITag' but no UITag is set." +
                            "Used the TooltipCanvas as parent instead."
                        );
                        parent = tooltipsCanvas.GetComponent<RectTransform>();
                        break;
                    }
                    var uiTag = UITag.GetFirstTag(ParentTag.Category, ParentTag.Name);
                    if (uiTag == null)
                    {
                        Debug.Log
                        (
                            "[Tooltip] Parenting mode set to 'UITag' but the UITag is not found." +
                            "Used the TooltipCanvas as parent instead."
                        );
                        parent = tooltipsCanvas.GetComponent<RectTransform>();
                        break;
                    }
                    parent = uiTag.GetComponent<RectTransform>();
                    if (parent == null)
                    {
                        Debug.Log
                        (
                            "[Tooltip] Parenting mode set to 'UITag' but the UITag has no RectTransform component." +
                            "Used the TooltipCanvas as parent instead."
                        );
                        parent = tooltipsCanvas.GetComponent<RectTransform>();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return parent;
        }

        #endregion

        #region Private Methods

        /// <summary> Set all the referenced buttons to hide the tooltip is HideOnAnyButton is enabled </summary>
        private void ApplyHideOnAnyButton()
        {
            if (addedHideEventToButtons) return;
            if (!hasButtons) return;
            if (!HideOnAnyButton) return;

            foreach (UIButton button in Buttons)
            {
                button.onClickBehaviour.Event.AddListener(Hide);
                button.onSubmitBehaviour.Event.AddListener(Hide);
            }

            addedHideEventToButtons = true; //only add the event once
        }

        /// <summary> Check is the tooltip is moved by a show/hide animation </summary>
        private void CheckIfShowOrHideHaveMoveReactions()
        {
            if (isShowing && !showHasBeenCalled)
            {
                showHasBeenCalled = true;
                showHasMovement = showReactions.Any(r => r.GetType() == typeof(UIMoveReaction) && ((UIMoveReaction)r).rectTransform == rectTransform && ((UIMoveReaction)r).enabled);
                if (showHasMovement)
                    showMoveReaction = showReactions.First(r => r.GetType() == typeof(UIMoveReaction) && ((UIMoveReaction)r).rectTransform == rectTransform) as UIMoveReaction;
                return;
            }

            if (isHiding && !hideHasBeenCalled)
            {
                hideHasBeenCalled = true;
                hideHasMovement = hideReactions.Any(r => r.GetType() == typeof(UIMoveReaction) && ((UIMoveReaction)r).rectTransform == rectTransform && ((UIMoveReaction)r).enabled);
                if (hideHasMovement)
                    hideMoveReaction = hideReactions.First(r => r.GetType() == typeof(UIMoveReaction) && ((UIMoveReaction)r).rectTransform == rectTransform) as UIMoveReaction;
            }
        }

        /// <summary> Update the tooltip position according to the tracking mode </summary>
        private void ApplyTracking()
        {
            Vector3 targetPosition;

            switch (TrackingMode)
            {
                case Tracking.Disabled:
                    return;
                case Tracking.FollowPointer:
                    targetPosition = pointerPosition;
                    break;
                case Tracking.FollowTrigger:
                    if (!hasTrigger)
                    {
                        TrackingMode = Tracking.Disabled;
                        return;
                    }
                    targetPosition = trigger.transform.position;
                    break;
                case Tracking.FollowTarget:
                    if (!hasFollowTarget)
                    {
                        this.SetFollowTargetFromUITag(FollowTag.Category, FollowTag.Name);
                        if (!hasFollowTarget)
                        {
                            TrackingMode = Tracking.Disabled;
                            return;
                        }
                        UpdateTarget();
                    }
                    targetPosition = followTarget.transform.position;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            transform.position = targetPosition;
        }

        /// <summary> Update the tooltip position according to the positioning mode </summary>
        private void ApplyPositioning()
        {
            if (transform.parent == null) return; //cannot calculate the position if the tooltip doesn't have a parent
            Vector3 calculatedTargetPosition;
            switch (TrackingMode)
            {
                case Tracking.Disabled:
                    calculatedTargetPosition = CalculatePositioningWhenTrackingIsDisabled();
                    break;
                case Tracking.FollowPointer:
                    calculatedTargetPosition = CalculatePositioningWhenTrackingIsFollowPointer();
                    break;
                case Tracking.FollowTrigger:
                case Tracking.FollowTarget:
                    calculatedTargetPosition = CalculatePositioningWhenTrackingIsEnabled();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Debug.Log($"[{TrackingMode}] calculatedTargetPosition: {calculatedTargetPosition}");

            //apply the position offset (set by the developer)
            calculatedTargetPosition += PositionOffset;
            
            //check for NaN values
            calculatedTargetPosition.x = float.IsNaN(calculatedTargetPosition.x) ? 0 : calculatedTargetPosition.x;
            calculatedTargetPosition.y = float.IsNaN(calculatedTargetPosition.y) ? 0 : calculatedTargetPosition.y;
            calculatedTargetPosition.z = float.IsNaN(calculatedTargetPosition.z) ? 0 : calculatedTargetPosition.z;
            
            //check for infinite values
            calculatedTargetPosition.x = float.IsInfinity(calculatedTargetPosition.x) ? 0 : calculatedTargetPosition.x;
            calculatedTargetPosition.y = float.IsInfinity(calculatedTargetPosition.y) ? 0 : calculatedTargetPosition.y;
            calculatedTargetPosition.z = float.IsInfinity(calculatedTargetPosition.z) ? 0 : calculatedTargetPosition.z;

            //if the tooltip is showing and the show animation has a move reaction -> update the reaction's To value to the calculated position
            if (isShowing & showHasMovement)
            {
                showMoveReaction.SetTo(calculatedTargetPosition);
                return;
            }

            //if the tooltip is hiding and the hide animation has a move reaction -> update the reaction's From value to the calculated position
            if (isHiding & hideHasMovement)
            {
                hideMoveReaction.SetFrom(calculatedTargetPosition);
                return;
            }

            //apply the calculated anchored position
            rectTransform.anchoredPosition3D = calculatedTargetPosition; //update the tooltip anchored position with the calculated position
        }

        /// <summary> Update the tooltip position to keep it in screen, if KeepInScreen is enabled </summary>
        private void ApplyKeepInScreen()
        {
            if (!KeepInScreen) return;

            var cCorners = new Vector3[4];
            tooltipRootCanvasRectTransform.GetWorldCorners(cCorners);
            Vector3 cBottomLeft = cCorners[0];
            Vector3 cTopRight = cCorners[2];
            Vector3 cSize = cTopRight - cBottomLeft;
            rectTransform.GetWorldCorners(cCorners);
            Vector3 tBottomLEft = cCorners[0];
            Vector3 tTopRight = cCorners[2];
            Vector3 tSize = tTopRight - tBottomLEft;
            Vector3 tPosition = rectTransform.position;
            Vector3 deltaBottomLeft = tPosition - tBottomLEft;
            Vector3 deltaTopRight = tTopRight - tPosition;
            tPosition.x = tSize.x < cSize.x
                ? Mathf.Clamp(tPosition.x, cBottomLeft.x + deltaBottomLeft.x, cTopRight.x - deltaTopRight.x)
                : Mathf.Clamp(tPosition.x, cTopRight.x - deltaTopRight.x, cBottomLeft.x + deltaBottomLeft.x);
            tPosition.y = tSize.y < cSize.y
                ? Mathf.Clamp(tPosition.y, cBottomLeft.y + deltaBottomLeft.y, cTopRight.y - deltaTopRight.y)
                : Mathf.Clamp(tPosition.y, cTopRight.y - deltaTopRight.y, cBottomLeft.y + deltaBottomLeft.y);
            rectTransform.position = tPosition;
        }

        /// <summary> Apply a maximum width constraint to the tooltip if one is set </summary>
        private void ApplyMaximumWidth()
        {
            if (MaximumWidth <= 0) return;                                     //no maximum width set
            layoutElement.preferredWidth = -1;                                 //reset the preferred width to -1 so it can be recalculated
            layoutElement.enabled = false;                                     //disable the layout element so it doesn't influence the preferred width
            rectTransform.ForceUpdateRectTransforms();                         //force the rect transform to update
            foreach (TextMeshProUGUI label in Labels) label.ForceMeshUpdate(); //force mesh updates on all labels
            float maxLabelWidth = Labels.Max(label => label.preferredWidth);   //get the maximum width from all labels
            if (maxLabelWidth < MaximumWidth) return;                          //if the maximum width is smaller than the maximum width set by the user, no need to do anything
            layoutElement.enabled = true;                                      //enable the layout element
            layoutElement.preferredWidth = MaximumWidth;                       //set the preferred width to the maximum width constraint
            rectTransform.ForceUpdateRectTransforms();                         //force an update of the rect transform
        }

        /// <summary> Internal method used if tracking is disabled to calculate the position according to the positioning mode inside its parent </summary>
        /// <returns> The anchoredPosition3D to apply to the tooltip </returns>
        private Vector3 CalculatePositioningWhenTrackingIsDisabled()
        {
            if (transform.parent == null) return rectTransform.anchoredPosition3D; //if the tooltip is not parented, just return the initial anchored position
            float z = rectTransform.anchoredPosition3D.z;                          //save the z value of the anchored position
            Rect parentRect = parentRectTransform.rect;                            //get the parent rect
            float parentWidth = parentRect.width;                                  //get the parent width
            float parentHeight = parentRect.height;                                //get the parent height

            return PositionMode switch
                   {
                       Positioning.TopLeft      => new Vector3(-parentWidth / 2f + width * pivotX, parentHeight / 2f - height * pivotY, z),
                       Positioning.TopCenter    => new Vector3(0f, parentHeight / 2f - height * pivotY, z),
                       Positioning.TopRight     => new Vector3(parentWidth / 2f - width * (1 - pivotX), parentHeight / 2f - height * pivotY, z),
                       Positioning.MiddleLeft   => new Vector3(-parentWidth / 2f + width * pivotX, 0f, z),
                       Positioning.MiddleCenter => new Vector3(0f, 0f, z),
                       Positioning.MiddleRight  => new Vector3(parentWidth / 2f - width * (1 - pivotX), 0f, z),
                       Positioning.BottomLeft   => new Vector3(-parentWidth / 2f + width * pivotX, -parentHeight / 2f + height * (1 - pivotY), z),
                       Positioning.BottomCenter => new Vector3(0f, -parentHeight / 2f + height * (1 - pivotY), z),
                       Positioning.BottomRight  => new Vector3(parentWidth / 2f - width * (1 - pivotX), -parentHeight / 2f + height * (1 - pivotY), z),
                       _                        => throw new ArgumentOutOfRangeException()
                   };
        }

        /// <summary> Internal method used to calculate the position of the tooltip according to the positioning mode and the current pointer position </summary>
        /// <returns> The anchoredPosition3D to apply to the tooltip </returns>
        private Vector3 CalculatePositioningWhenTrackingIsFollowPointer()
        {
            Vector2 point;
            const float z = 0;

            switch (tooltipRootCanvas.renderMode)
            {
                case RenderMode.ScreenSpaceOverlay:
                    point = parentRectTransform.InverseTransformPoint(pointerPosition);
                    break;
                case RenderMode.ScreenSpaceCamera:
                case RenderMode.WorldSpace:
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, pointerPosition, tooltipRootCanvas.worldCamera, out point);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            point -= parentRectTransform.rect.center;

            //adjust the point according to the positioning mode and the tooltip size and pivot
            point = PositionMode switch
                    {
                        Positioning.TopLeft      => new Vector3(point.x - width * pivotX, point.y + height * pivotY, z),
                        Positioning.TopCenter    => new Vector3(point.x, point.y + height * pivotY, z),
                        Positioning.TopRight     => new Vector3(point.x + width * (1 - pivotX), point.y + height * pivotY, z),
                        Positioning.MiddleLeft   => new Vector3(point.x - width * pivotX, point.y, z),
                        Positioning.MiddleCenter => new Vector3(point.x, point.y, z),
                        Positioning.MiddleRight  => new Vector3(point.x + width * (1 - pivotX), point.y, z),
                        Positioning.BottomLeft   => new Vector3(point.x - width * pivotX, point.y - height * (1 - pivotY), z),
                        Positioning.BottomCenter => new Vector3(point.x, point.y - height * (1 - pivotY), z),
                        Positioning.BottomRight  => new Vector3(point.x + width * (1 - pivotX), point.y - height * (1 - pivotY), z),
                        _                        => throw new ArgumentOutOfRangeException()
                    };

            return point;
        }

        internal bool updateTarget { get; set; }

        /// <summary> Internal method used to calculate the position of the tooltip according to the positioning mode inside its parent </summary>
        /// <returns> The anchoredPosition3D to apply to the tooltip </returns>
        private Vector3 CalculatePositioningWhenTrackingIsEnabled()
        {
            if (updateTarget) UpdateTarget();
            Vector3 point = parentRectTransform.InverseTransformPoint(targetRectTransform.position);
            //calculate the target offset according to the positioning mode
            Vector3 targetOffset = GetPositionOffset(targetRectTransform, PositionMode);
            //fix the target offset value if the rectTransform has a scale applied
            Vector3 localScale = targetRectTransform.localScale;
            targetOffset.x *= localScale.x;
            targetOffset.y *= localScale.y;
            //calculate the scale difference between the tooltip root canvas and the target root canvas
            Vector3 targetCanvasScale = targetRectTransform.lossyScale;
            Vector3 tooltipCanvasScale = rectTransform.lossyScale;
            var scaleDiff = new Vector3(targetCanvasScale.x / tooltipCanvasScale.x, targetCanvasScale.y / tooltipCanvasScale.y, targetCanvasScale.z / tooltipCanvasScale.z);
            //apply the scale difference to the target offset for better positioning
            targetOffset.x *= scaleDiff.x;
            targetOffset.y *= scaleDiff.y;
            targetOffset.z *= scaleDiff.z;
            //apply the calculated target offset 
            point += targetOffset;
            //calculate the tooltip offset according to the positioning mode
            Vector3 tooltipOffset = GetPositionOffset(rectTransform, PositionMode);
            //apply the calculated tooltip offset`
            point += tooltipOffset;
            //return the calculated anchored position
            return point;
        }

        private static Vector3 GetPositionOffset(RectTransform rectTransform, Positioning positionMode)
        {
            Rect rect = rectTransform.rect;
            return positionMode switch
                   {
                       Positioning.TopLeft      => new Vector3(rect.xMin, rect.yMax, 0),
                       Positioning.TopCenter    => new Vector3(rect.center.x, rect.yMax, 0),
                       Positioning.TopRight     => new Vector3(rect.xMax, rect.yMax, 0),
                       Positioning.MiddleLeft   => new Vector3(rect.xMin, rect.center.y, 0),
                       Positioning.MiddleCenter => new Vector3(rect.center.x, rect.center.y, 0),
                       Positioning.MiddleRight  => new Vector3(rect.xMax, rect.center.y, 0),
                       Positioning.BottomLeft   => new Vector3(rect.xMin, rect.yMin, 0),
                       Positioning.BottomCenter => new Vector3(rect.center.x, rect.yMin, 0),
                       Positioning.BottomRight  => new Vector3(rect.xMax, rect.yMin, 0),
                       _                        => throw new ArgumentOutOfRangeException()
                   };
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Instantiate a new tooltip from the prefab that is registered in the database.
        /// If a prefab with the given tooltip name is not found, null will be returned.
        /// </summary>
        /// <param name="tooltipName"> The name of the tooltip prefab in the database </param>
        /// <returns> The tooltip instance. Null if the prefab is not found. </returns>
        public static UITooltip Get(string tooltipName)
        {
            if (string.IsNullOrEmpty(tooltipName)) return null;
            GameObject prefab = UITooltipDatabase.instance.GetPrefab(tooltipName);
            if (prefab == null)
            {
                Debug.LogWarning($"UITooltip.Get({tooltipName}) - prefab not found in the database");
                return null;
            }

            UITooltip tooltip =
                Instantiate(prefab)
                    .GetComponent<UITooltip>()
                    .Reset();

            //destroy the tooltip when it is hidden
            tooltip.OnHiddenCallback.Event.AddListener(() =>
            {
                //sanity check to make sure the tooltip is not already destroyed
                if (tooltip == null) return;
                Destroy(tooltip.gameObject);
                tooltip = null;
            });

            return tooltip;
        }

        /// <summary> Calls ShowTooltip on all UITooltipTriggers in the scene (that are active and enabled) </summary>
        public static void ShowAllTooltips()
        {
            foreach (UITooltipTrigger tooltipTrigger in UITooltipTrigger.database)
            {
                if (!tooltipTrigger.isActiveAndEnabled) return;
                tooltipTrigger.ShowTooltip();
            }
        }

        /// <summary> Calls Hide on all UITooltips in the scene (that are active and enabled) </summary>
        public void HideAllTooltips()
        {
            foreach (UITooltip tooltip in database)
            {
                if (!tooltip.isActiveAndEnabled) return;
                tooltip.Hide();
            }
        }

        #endregion
    }

    public static class UITooltipExtensions
    {
        /// <summary> Reset the tooltip to its initial state </summary>
        /// <param name="tooltip"> Target tooltip </param>
        /// <returns> The tooltip instance </returns>
        public static T Reset<T>(this T tooltip) where T : UITooltip
        {
            tooltip.updateTarget = true;
            tooltip.tooltipRootCanvas = null;
            tooltip.targetRectTransform = null;
            tooltip.parentRectTransform = null;
            tooltip.trigger = null;
            tooltip.followTarget = null;
            return tooltip;
        }

        /// <summary> Reparent the tooltip to a new parent </summary>
        /// <param name="tooltip"> Target tooltip </param>
        /// <param name="parent"> The new parent </param>
        /// <returns> The tooltip instance </returns>
        public static T SetParent<T>(this T tooltip, RectTransform parent) where T : UITooltip
        {
            tooltip.tooltipRootCanvas = null;
            tooltip.parentRectTransform = parent;
            if (parent == null) return tooltip;
            tooltip.rectTransform.SetParent(parent, true);
            tooltip.rectTransform.CenterPivot();
            tooltip.rectTransform.localScale = Vector3.one;
            tooltip.rectTransform.anchoredPosition3D = tooltip.CustomStartPosition;
            tooltip.tooltipRootCanvas = parent.GetComponentInParent<Canvas>().rootCanvas;
            return tooltip;
        }

        /// <summary> Set the follow target for when the tooltip is visible and tracking is enabled to follow a target </summary>
        /// <param name="tooltip"> Target tooltip </param>
        /// <param name="target"> Target trigger </param>
        /// <returns> The tooltip instance </returns>
        public static T SetTrigger<T>(this T tooltip, UITooltipTrigger target) where T : UITooltip
        {
            tooltip.trigger = target;
            return tooltip;
        }

        /// <summary> Set the follow target for when the tooltip is visible and tracking is enabled to follow a target </summary>
        /// <param name="tooltip"> Target tooltip </param>
        /// <param name="target"> Target game object </param>
        /// <returns> The tooltip instance </returns>
        public static T SetFollowTarget<T>(this T tooltip, GameObject target) where T : UITooltip
        {
            tooltip.followTarget = target;
            return tooltip;
        }

        /// <summary> Set the follow target for when the tooltip is visible and tracking is enabled to follow a target </summary>
        /// <param name="tooltip"> Target tooltip </param>
        /// <param name="category"> Category of the tag </param>
        /// <param name="name"> Name of the tag </param>
        /// <returns> The tooltip instance </returns>
        public static T SetFollowTargetFromUITag<T>(this T tooltip, string category, string name) where T : UITooltip
        {
            tooltip.followTarget = null;
            UITag tag = UITag.GetTags(category, name).FirstOrDefault();
            if (tag != null) tooltip.followTarget = tag.gameObject;
            return tooltip;
        }

        /// <summary> Update the keep in screen setting </summary>
        /// <param name="tooltip"> Target tooltip </param>
        /// <param name="keepInScreen"> TRUE to keep the tooltip in screen at all times, while it is visible </param>
        /// <returns> The tooltip instance </returns>
        public static T SetKeepInScreen<T>(this T tooltip, bool keepInScreen) where T : UITooltip
        {
            tooltip.KeepInScreen = keepInScreen;
            return tooltip;
        }

        /// <summary> Update the override sorting order setting </summary>
        /// <param name="target"> Target tooltip </param>
        /// <param name="overrideSortingOrder"> New override sorting order value </param>
        /// <param name="apply"> TRUE to apply the new value, FALSE to only update the setting </param>
        /// <returns> The tooltip instance </returns>
        public static T SetOverrideSorting<T>(this T target, bool overrideSortingOrder, bool apply = false) where T : UITooltip
        {
            target.OverrideSorting = overrideSortingOrder;
            if (apply) target.ApplyOverrideSorting();
            return target;
        }

        /// <summary> Apply the override sorting order setting (if enabled) </summary>
        /// <param name="target"> Target tooltip </param>
        /// <returns> The tooltip instance </returns>
        public static T ApplyOverrideSorting<T>(this T target) where T : UITooltip
        {
            if (!target.OverrideSorting)
                return target;

            target.canvas.overrideSorting = true;
            target.canvas.sortingOrder = UITooltip.k_MaxSortingOrder;

            if (!target.canvas.gameObject.activeInHierarchy)
                Debug.Log($"Cannot apply override sorting order to tooltip {target.name} because it is not active in the scene");

            if (!target.canvas.enabled)
                Debug.Log($"Cannot apply override sorting order to tooltip {target.name} because its canvas is not enabled");

            return target;
        }

        /// <summary>
        /// Set the text values for all the text mesh pro labels this tooltip has references to.
        /// <para/> Each string value will be set to the TextMeshProUI label with the same index in the Labels list.
        /// </summary>
        /// <param name="tooltip"> Target tooltip </param>
        /// <param name="texts"> Text values to set </param>
        /// <returns> The tooltip instance </returns>
        public static T SetTexts<T>(this T tooltip, params string[] texts) where T : UITooltip
        {
            int textsCount = texts.Length;
            if (textsCount == 0) return tooltip;
            for (int i = 0; i < tooltip.Labels.Count; i++)
            {
                TextMeshProUGUI label = tooltip.Labels[i];
                if (label == null) continue;
                label.SetText(i < textsCount ? texts[i] : string.Empty);
                label.ForceMeshUpdate();
            }
            return tooltip;
        }

        /// <summary>
        /// Set the sprite references for all the Images this tooltip has references to.
        /// <para/> Each Sprite will be referenced to the Image with the same index in the Images list.
        /// </summary>
        /// <param name="tooltip"> Target tooltip </param>
        /// <param name="sprites"> Sprite references to set </param>
        /// <returns> The tooltip instance </returns>
        public static T SetSprites<T>(this T tooltip, params Sprite[] sprites) where T : UITooltip
        {
            int spritesCount = sprites.Length;
            if (spritesCount == 0) return tooltip;
            for (int i = 0; i < tooltip.Images.Count; i++)
            {
                Image image = tooltip.Images[i];
                if (image == null) continue;
                image.sprite = i < spritesCount ? sprites[i] : null;
            }
            return tooltip;
        }

        /// <summary>
        /// Set the UnityEvents to invoke for all the UIButtons this tooltip has references to
        /// <para/> Each UnityEvent will be assigned to the UIButton with the same index in the Buttons list.
        /// <para/> The UnityEvent will be invoked when the UIButton's either on click or submit behaviour is invoked.
        /// </summary>
        /// <param name="tooltip"> Target tooltip </param>
        /// <param name="events"> UnityEvents to invoke </param>
        /// <returns> The tooltip instance </returns>
        public static T SetEvents<T>(this T tooltip, params UnityEvent[] events) where T : UITooltip
        {
            int eventsCount = events.Length;
            if (eventsCount == 0) return tooltip;
            bool hasGraphicRaycaster = tooltip.GetComponentInChildren<GraphicRaycaster>();
            for (int i = 0; i < tooltip.Buttons.Count; i++)
            {
                UIButton button = tooltip.Buttons[i];                       //get the button
                if (button == null) continue;                               //if the button is null, continue
                if (!hasGraphicRaycaster)                                   //if the tooltip doesn't have a graphic raycaster, check if the button has one
                    if (!button.GetComponent<GraphicRaycaster>())           //if the button doesn't have a graphic raycaster
                        button.gameObject.AddComponent<GraphicRaycaster>(); //add a graphic raycaster to the button
                if (eventsCount <= i) continue;                             //if the event count is less than the index, continue
                UnityEvent evt = events[i];                                 //get the event
                if (evt == null) continue;                                  //if the event is null, continue
                button.onClickBehaviour.Event.AddListener(evt.Invoke);      //add the event to the button on click behaviour
                button.onSubmitBehaviour.Event.AddListener(evt.Invoke);     //add the event to the button on submit behaviour
            }

            return tooltip;
        }

        /// <summary>
        /// Set the UnityActions to invoke for all the UIButtons this tooltip has references to.
        /// <para/> Each UnityAction will be assigned to the UIButton with the same index in the Buttons list.
        /// <para/> The UnityAction will be invoked when the UIButton's either on click or submit behaviour is invoked.
        /// </summary>
        /// <param name="tooltip"> Target tooltip </param>
        /// <param name="actions"> UnityActions to invoke </param>
        /// <returns> The tooltip instance </returns>
        public static T SetEvents<T>(this T tooltip, params UnityAction[] actions) where T : UITooltip
        {
            int actionsCount = actions.Length;
            if (actionsCount == 0) return tooltip;
            bool hasGraphicRaycaster = tooltip.GetComponentInChildren<GraphicRaycaster>();
            for (int i = 0; i < tooltip.Buttons.Count; i++)
            {
                UIButton button = tooltip.Buttons[i];                       //get the button
                if (button == null) continue;                               //if the button is null, continue
                if (!hasGraphicRaycaster)                                   //if the tooltip doesn't have a graphic raycaster, check if the button has one
                    if (!button.GetComponent<GraphicRaycaster>())           //if the button doesn't have a graphic raycaster
                        button.gameObject.AddComponent<GraphicRaycaster>(); //add a graphic raycaster to the button
                if (actionsCount <= i) continue;                            //if the event count is less than the index, continue
                UnityAction action = actions[i];                            //get the action
                if (action == null) continue;                               //if the action is null, continue
                button.onClickBehaviour.Event.AddListener(action.Invoke);   //add the action to the button on click behaviour
                button.onSubmitBehaviour.Event.AddListener(action.Invoke);  //add the action to the button on submit behaviour
            }

            return tooltip;
        }
    }
}
