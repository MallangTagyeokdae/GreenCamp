// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections;
using System.Collections.Generic;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Mody;
using Doozy.Runtime.UIManager.Containers;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable PartialTypeWithSinglePart

namespace Doozy.Runtime.UIManager.Triggers
{
    /// <summary>
    /// Specialized trigger used to show a UITooltip
    /// </summary>
    [DisallowMultipleComponent]
    public partial class UITooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [ClearOnReload]
        public static HashSet<UITooltipTrigger> database { get; } = new HashSet<UITooltipTrigger>();

        /// <summary>
        /// Name of the UITooltip that will be triggered by this trigger
        /// </summary>
        public string UITooltipName = UITooltip.k_DefaultTooltipName;

        /// <summary>
        /// Show the tooltip when the pointer enters the trigger
        /// </summary>
        public bool ShowOnPointerEnter = true;

        /// <summary>
        /// Hide the tooltip when the pointer exits the trigger (if the tooltip is shown)
        /// </summary>
        public bool HideOnPointerExit = true;

        /// <summary>
        /// Show the tooltip when the pointer clicks the trigger
        /// </summary>
        public bool ShowOnPointerClick;

        /// <summary>
        /// Hide the tooltip when the pointer clicks the trigger (if the tooltip is shown)
        /// </summary>
        public bool HideOnPointerClick;

        /// <summary>
        /// Override tooltip's ParentMode to use the value from this trigger instead of the one set in the tooltip
        /// </summary>
        public bool OverrideParentMode;

        /// <summary>
        /// Set where a tooltip should be instantiated under.
        /// Only used when OverrideParentMode is true.
        /// </summary>
        public UITooltip.Parenting ParentMode = UITooltip.Parenting.TooltipsCanvas;

        /// <summary>
        /// Override tooltip's TrackingMode to use the value from this trigger instead of the one set in the tooltip
        /// </summary>
        public bool OverrideTrackingMode;

        /// <summary>
        /// Set how the tooltip behaves when it is shown.
        /// Only used when OverrideTrackingMode is true.
        /// </summary>
        public UITooltip.Tracking TrackingMode = UITooltip.Tracking.Disabled;

        /// <summary>
        /// Override tooltip's PositionMode to use the value from this trigger instead of the one set in the tooltip
        /// </summary>
        public bool OverridePositionMode;

        /// <summary>
        /// Set where the tooltip should be positioned relative to the tracked target.
        /// Only used when OverridePositionMode is true.
        /// </summary>
        public UITooltip.Positioning PositionMode = UITooltip.Positioning.MiddleCenter;

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
        /// Override tooltip's PositionOffset to use the value from this trigger instead of the one set in the tooltip
        /// </summary>
        public bool OverridePositionOffset;

        /// <summary>
        /// Set the offset applied to the tooltip position.
        /// Only used when OverridePositionOffset is true.
        /// </summary>
        public Vector3 PositionOffset = Vector3.zero;

        /// <summary>
        /// Override tooltip's MaximumWidth to use the value from this trigger instead of the one set in the tooltip
        /// </summary>
        public bool OverrideMaximumWidth;

        /// <summary>
        /// Set the maximum width of the tooltip.
        /// Only used when OverrideMaximumWidth is true.
        /// </summary>
        public float MaximumWidth;

        [SerializeField] private float ShowDelay;
        /// <summary> How long (in seconds), after the pointer enters the trigger, should the tooltip be shown </summary>
        public float showDelay
        {
            get => ShowDelay;
            set => ShowDelay = Mathf.Max(0, value);
        }

        /// <summary>
        /// Texts set to the TextMeshProUGUI labels attached to the target UITooltip
        /// </summary>
        public List<string> Texts = new List<string>();

        /// <summary>
        /// Sprites set to the Images attached to the target UITooltip
        /// </summary>
        public List<Sprite> Sprites = new List<Sprite>();

        /// <summary>
        /// UnityEvents triggered by clicking the UIButtons attached to the target UITooltip
        /// </summary>
        public List<UnityEvent> Events = new List<UnityEvent>();

        /// <summary> Callback invoked when the trigger calls for the tooltip to be shown </summary>
        public ModyEvent OnShowCallback = new ModyEvent();

        /// <summary> Callback invoked when the trigger calls for the tooltip to be hidden </summary>
        public ModyEvent OnHideCallback = new ModyEvent();

        /// <summary>
        /// Internal variable used to store the UITooltip that is currently being triggered by this trigger
        /// </summary>
        public UITooltip tooltip { get; private set; }

        /// <summary> Internal flag used to determine if the tooltip name is valid </summary>
        private bool isValid { get; set; }

        /// <summary> Internal coroutine used to show the tooltip after the ShowDelay has passed </summary>
        private Coroutine showDelayCoroutine { get; set; }

        /// <summary> Flag used to keep track if the tooltip trigger is currently waiting to show its target tooltip </summary>
        public bool isWaitingToShow => showDelayCoroutine != null;

        private void Validate()
        {
            isValid = UITooltipDatabase.instance.GetPrefab(UITooltipName) != null;
        }

        private void Awake()
        {
            database.Add(this);
        }

        private void OnEnable()
        {
            database.Remove(null);
            Validate();
            if (isValid) return;
            Debug.LogWarning($"UITooltipTrigger - {name} - The UITooltip name '{UITooltipName}' is not valid. Please make sure it is spelled correctly in the UITooltipDatabase.", this);
        }

        private void OnDisable()
        {
            StopShowDelayCoroutine();
        }

        private void OnDestroy()
        {
            database.Remove(this);
            database.Remove(null);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            StopShowDelayCoroutine();
            if (!isValid) return;
            if (!ShowOnPointerEnter) return;
            if (showDelay > 0)
            {
                showDelayCoroutine = StartCoroutine(ShowDelayCoroutine());
                return;
            }
            ShowTooltip();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StopShowDelayCoroutine();
            if (!isValid) return;
            if (HideOnPointerExit)
                HideTooltip();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            StopShowDelayCoroutine();
            if (!isValid) return;
            if (ShowOnPointerClick && tooltip == null)
            {
                ShowTooltip();
                return;
            }

            if (HideOnPointerClick)
                HideTooltip();
        }

        private IEnumerator ShowDelayCoroutine()
        {
            yield return new WaitForSecondsRealtime(showDelay);
            ShowTooltip();
            StopShowDelayCoroutine();
        }

        private void StopShowDelayCoroutine()
        {
            if (showDelayCoroutine == null) return;
            StopCoroutine(showDelayCoroutine);
            showDelayCoroutine = null;
        }

        /// <summary> Show the tooltip </summary>
        public virtual void ShowTooltip()
        {
            if (tooltip != null)
            {
                if (tooltip.isVisible || tooltip.isShowing)
                    return;

                tooltip.InstantHide();
                if (tooltip != null)
                {
                    Destroy(tooltip.gameObject);
                    tooltip = null;
                }
            }

            tooltip = UITooltip.Get(UITooltipName); // Get the tooltip from the database
            if (tooltip == null) return;            //Tooltip not found in the database
            tooltip.SetTrigger(this);               //Set the trigger to the tooltip so it can access the trigger's properties
            //Parent Mode
            if (OverrideParentMode)
            {
                tooltip.ParentMode = ParentMode;
                tooltip.ParentTag = ParentTag;
            }
            tooltip.SetParent(tooltip.GetParent()); //Reparent the tooltip
            //Tracking Mode
            if (OverrideTrackingMode)
            {
                tooltip.TrackingMode = TrackingMode;
                tooltip.FollowTag = FollowTag;
            }
            //Position Mode
            if (OverridePositionMode) tooltip.PositionMode = PositionMode;
            //Position Offset
            if (OverridePositionOffset) tooltip.PositionOffset = PositionOffset;
            //Maximum Width
            if (OverrideMaximumWidth) tooltip.MaximumWidth = MaximumWidth;
            tooltip.InstantHide(false);                          //Hide instantly to prevent the tooltip from showing up in the wrong position
            tooltip.SetTexts(Texts.RemoveNulls().ToArray());     //Set the texts
            tooltip.SetSprites(Sprites.RemoveNulls().ToArray()); //Set the sprites
            tooltip.SetEvents(Events.RemoveNulls().ToArray());   //Set the events
            tooltip.Show();                                      //Show the tooltip
            OnShowCallback?.Execute();                           //Invoke the OnShowCallback
        }

        /// <summary> Hide the tooltip </summary>
        public virtual void HideTooltip()
        {
            if (tooltip == null) return; //tooltip might have been destroyed by another script

            tooltip.OnHiddenCallback.Event.AddListener(() =>
            {
                if (tooltip == null) return; //If the tooltip is null, it means it was destroyed by another script, so we don't need to destroy it again
                Destroy(tooltip.gameObject); //Destroy the tooltip
                tooltip = null;              //Set the tooltip to null to prevent it from being destroyed on the next frame
                OnHideCallback?.Execute();   //Invoke the OnHideCallback
            });

            tooltip.Hide(); //Hide the tooltip
        }

    }
}
