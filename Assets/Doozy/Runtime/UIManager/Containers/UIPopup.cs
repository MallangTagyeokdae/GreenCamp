// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Global;
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
// ReSharper disable PartialTypeWithSinglePart

// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.UIManager.Containers
{
    /// <summary>
    /// Specialized container that behaves like a popup (modal window),
    /// with parenting and positioning options.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    [AddComponentMenu("UI/Containers/UIPopup")]
    [DisallowMultipleComponent]
    public partial class UIPopup : UIContainerComponent<UIPopup>
    {
        /// <summary> Describes where a popup can be instantiated under </summary>
        public enum Parenting
        {
            /// <summary> The parent of the popup will be the PopupCanvas </summary>
            PopupsCanvas = 0,

            /// <summary> The parent of the popup will be the RectTransform of the GameObject that has the given UITagId </summary>
            UITag = 1
        }

        /// <summary> Maximum sorting order value for popups (it's 1 level lower than popups) </summary>
        public const int k_MaxSortingOrder = 32766;

        /// <summary> Default popup name </summary>
        public const string k_DefaultPopupName = "None";

        /// <summary> Default category used by the UITagId to identify the default Popup Canvas </summary>
        public const string k_DefaultPopupCanvasUITagCategory = "UIPopup";

        /// <summary> Default name used by the UITagId to identify the default Popup Canvas </summary>
        public const string k_DefaultPopupCanvasUITagName = "Canvas";

        /// <summary> Default name for the popups queue. </summary>
        public const string k_DefaultQueueName = "Default";

        /// <summary> Get all the visible popups (returns all popups that are either in the isVisible or isShowing state) </summary>
        public static IEnumerable<UIPopup> visiblePopups =>
            database.Where(item => item.isVisible || item.isShowing);

        /// <summary> Internal static reference to the default canvas used to display popups </summary>
        [ClearOnReload]
        private static Canvas s_popupsCanvas;
        /// <summary>
        /// Static reference to the default canvas used to display popups (popups get parented to this canvas).
        /// <para/> You can override the default canvas by referencing another canvas (at runtime) to be used as the default canvas.
        /// </summary>
        public static Canvas popupsCanvas
        {
            get
            {
                if (s_popupsCanvas != null) return s_popupsCanvas;
                //look for UITag
                UITag uiTag = UITag.GetTags(k_DefaultPopupCanvasUITagCategory, k_DefaultPopupCanvasUITagName).FirstOrDefault();
                if (uiTag != null)
                {
                    s_popupsCanvas = uiTag.GetComponent<Canvas>();
                    if (s_popupsCanvas != null)
                        return s_popupsCanvas;
                }
                //create Popup Canvas
                s_popupsCanvas = new GameObject("Popups Canvas").AddComponent<Canvas>();
                s_popupsCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                s_popupsCanvas.overrideSorting = true;
                s_popupsCanvas.sortingOrder = k_MaxSortingOrder;
                //add the default tag
                uiTag = s_popupsCanvas.gameObject.AddComponent<UITag>();
                uiTag.Id.Category = k_DefaultPopupCanvasUITagCategory;
                uiTag.Id.Name = k_DefaultPopupCanvasUITagName;
                return s_popupsCanvas;
            }
            set => s_popupsCanvas = value;
        }

        #region Popup Queue

        [ClearOnReload]
        private static Dictionary<string, List<UIPopup>> s_queues;
        /// <summary> Popup queues </summary>
        public static Dictionary<string, List<UIPopup>> queues => s_queues ?? (s_queues = new Dictionary<string, List<UIPopup>>());

        /// <summary> Get the queue with the given name </summary>
        /// <param name="queueName"> Queue name </param>
        /// <returns> The queue with the given name </returns>
        public static List<UIPopup> GetQueue(string queueName = k_DefaultQueueName)
        {
            if (string.IsNullOrEmpty(queueName)) return null;
            return queues.TryGetValue(queueName, out List<UIPopup> queue) ? queue.RemoveNulls() : null;
        }

        /// <summary> Get the first popup in the queue with the given name </summary>
        /// <param name="queueName"> Queue name </param>
        /// <returns> The first popup in the queue with the given name </returns>
        public static UIPopup GetFirstPopupInQueue(string queueName = k_DefaultQueueName) =>
            GetQueue(queueName)?.FirstOrDefault();

        /// <summary> Show the first popup in the queue with the given name </summary>
        /// <param name="queueName"> Queue name </param>
        /// <returns>
        /// The first popup in the queue with the given name.
        /// <para/> Returns null if the queue is empty.
        /// </returns>
        public static UIPopup ShowNextPopupInQueue(string queueName = k_DefaultQueueName)
        {
            List<UIPopup> queue = GetQueue(queueName);
            if (queue == null) return null;
            if (queue.Count == 0)
            {
                queues.Remove(queueName);
                return null;
            }
            UIPopup popup = queue.FirstOrDefault();
            if (popup == null) return null;
            popup.OnHiddenCallback.Event.AddListener(() =>
            {
                RemovePopupFromQueue(popup);
                ShowNextPopupInQueue(queueName);
            });
            popup.Show();
            return popup;
        }

        /// <summary>
        /// Add a popup to the queue with the given name.
        /// If the queue doesn't exist, it will be created and the popup will be shown.
        /// </summary>
        /// <param name="popup"> Popup to add to the queue </param>
        /// <param name="queueName"> Queue name </param>
        public static void AddPopupToQueue(UIPopup popup, string queueName = k_DefaultQueueName)
        {
            if (popup == null) return;
            List<UIPopup> queue = GetQueue(queueName);
            
            //check if the queue already exists and if it's empty
            bool showPopup = queue == null || queue.Count == 0;
            
            //create queue if it doesn't exist
            if (queue == null)
            {
                queue = new List<UIPopup>();
                queues.Add(queueName, queue);
            }
            
            //don't add the popup if it's already in the queue
            if(!showPopup && queue.Contains(popup))
                return;
            
            //add the popup to the queue
            queue.Add(popup);
            
            //instantly hide the popup if it's not hidden
            if (!popup.isHidden) popup.InstantHide(false);
            
            //show the popup if it's the first one in the queue
            if (showPopup) ShowNextPopupInQueue(queueName);
        }

        /// <summary>
        /// Remove the popup from the queue with the given name.
        /// If the popup is currently showing, it will be hidden and removed from the queue.
        /// </summary>
        /// <param name="popup"> Popup to remove from the queue </param>
        /// <param name="queueName"> Queue name </param>
        public static void RemovePopupFromQueue(UIPopup popup, string queueName = k_DefaultQueueName)
        {
            if (popup == null) return;
            List<UIPopup> queue = GetQueue(queueName);
            queue?.Remove(popup);
            if (popup.isVisible || popup.isShowing)
                popup.Hide();
        }

        /// <summary>
        /// Clear the queue with the given name by hiding all the popups in the queue and removing them from the queue.
        /// </summary>
        /// <param name="queueName"> Queue name </param>
        public static void ClearQueue(string queueName = k_DefaultQueueName)
        {
            List<UIPopup> queue = GetQueue(queueName);
            if (queue == null) return;
            foreach (UIPopup popup in queue)
                popup.Hide();
            queue.Clear();
            queues.Remove(queueName);
        }

        #endregion

        /// <summary> Set where a popup should be instantiated under </summary>
        public Parenting ParentMode = Parenting.PopupsCanvas;

        /// <summary>
        /// Id used to identify the designated parent where the popup should be parented under,
        /// after is has been instantiated
        /// </summary>
        public UITagId ParentTag;

        /// <summary>
        /// Enable override sorting and set the sorting order to the maximum value,
        /// for the Canvas component attached to this popup
        /// </summary>
        public bool OverrideSorting = true;

        /// <summary> Block the 'Back' button when the popup is visible </summary>
        public bool BlockBackButton = true;

        /// <summary>
        /// Reselect the previously selected GameObject when the popup is hidden.
        /// <para/> This is useful when the popup is hidden from a button that was selected before the popup was shown.
        /// <para/> EventSystem.current is used to determine the currently selected GameObject.
        /// </summary>
        public bool RestoreSelectedAfterHide = true;

        /// <summary>
        /// Hide (close) the popup when the user clicks on any of the UIButton references.
        /// <para/> At runtime, a 'hide popup' event will be added to all the referenced UIButtons (if any).
        /// </summary>
        public bool HideOnAnyButton = true;

        /// <summary> Set the next 'Back' button event to hide (close) this UIPopup </summary>
        public bool HideOnBackButton;

        /// <summary> Set the next click (on the Container) to hide (close) this UIPopup </summary>
        public bool HideOnClickContainer = true;

        /// <summary> Set the next click (on the Overlay) to hide (close) this UIPopup </summary>
        public bool HideOnClickOverlay = true;

        /// <summary> Reference to the popup's Overlay RectTransform </summary>
        public RectTransform Overlay;

        /// <summary> TRUE if the popup has an Overlay RectTransform reference </summary>
        public bool hasOverlay => Overlay != null;

        /// <summary> Reference to the popup's Container RectTransform </summary>
        public RectTransform Container;

        /// <summary> TRUE if the popup has a Content RectTransform reference </summary>
        public bool hasContainer => Container != null;

        /// <summary> List of all the labels this UIPopup has </summary>
        public List<TextMeshProUGUI> Labels = new List<TextMeshProUGUI>();

        /// <summary> TRUE if the popup has at least one TextMeshProUGUI label reference </summary>
        public bool hasLabels => Labels.RemoveNulls().Count > 0;

        /// <summary> List of all the images this UIPopup has </summary>
        public List<Image> Images = new List<Image>();

        /// <summary> TRUE if the popup has at least one Image reference </summary>
        public bool hasImages => Images.RemoveNulls().Count > 0;

        /// <summary> List of all the buttons this UIPopup has </summary>
        public List<UIButton> Buttons = new List<UIButton>();

        /// <summary> TRUE if the popup has at least one UIButton reference </summary>
        public bool hasButtons => Buttons.RemoveNulls().Count > 0;

        /// <summary>
        /// Reference to the RectTransform of the popup's parent.
        /// This value is updated after SetParent() is called.
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public RectTransform parentRectTransform { get; internal set; }

        /// <summary> 'Back' button signal receiver used to trigger the hiding of this UIPopup if HideOnBackButton is TRUE </summary>
        public SignalReceiver backButtonReceiver { get; set; }

        /// <summary> Internal flag used to keep track if this popup disabled the 'Back' button, used to restore the previous state </summary>
        private bool unblockBackButton { get; set; }

        /// <summary> Internal flag used to keep track if this popup added a PointerClickTrigger to the Overlay RectTransform </summary>
        private bool addedHideOnClickOverlay { get; set; }

        /// <summary> Internal flag used to keep track if this popup added a PointerClickTrigger to the Container RectTransform </summary>
        private bool addedHideOnClickContainer { get; set; }

        /// <summary>
        /// Internal flag used to keep track if the hide event was added to the buttons.
        /// This is needed to avoid adding the event multiple times to the same button.
        /// </summary>
        private bool addedHideEventToButtons { get; set; }

        /// <summary> Internal reference to the previously selected GameObject. </summary>
        private GameObject previouslySelectedGameObject { get; set; }

        /// <summary> Validate the popup's settings </summary>
        public virtual void Validate()
        {
            Labels.RemoveNulls();
            Images.RemoveNulls();
            Buttons.RemoveNulls();
        }

        protected override void Awake()
        {
            base.Awake();

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

        protected override void OnDestroy()
        {
            base.OnDestroy();

            //sanity check to make sure the 'Back' button is enabled back again
            EnableBackButton();
        }

        public override void Show()
        {
            SavePreviouslySelectedGameObject();
            DisableBackButton();
            AddOnClickToOverlay();
            AddOnClickToContainer();
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

        public override void InstantShow()
        {
            SavePreviouslySelectedGameObject();
            DisableBackButton();
            AddOnClickToOverlay();
            AddOnClickToContainer();
            base.InstantShow();
        }

        public override void Hide()
        {
            EnableBackButton();
            base.Hide();
        }

        public override void InstantHide()
        {
            EnableBackButton();
            base.InstantHide();
        }

        #region Public Methods

        /// <summary>
        /// Get a parent reference for the popup according to the popup's current ParentMode setting.
        /// <para/> This is not the same as the popup's transform.parent, which is the parent of the popup's GameObject.
        /// </summary>
        public RectTransform GetParent()
        {
            RectTransform parent;

            switch (ParentMode)
            {
                case Parenting.PopupsCanvas:
                    parent = popupsCanvas.GetComponent<RectTransform>();
                    break;
                case Parenting.UITag:
                    if (ParentTag == null)
                    {
                        Debug.Log
                        (
                            "[Popup] Parenting mode set to 'UITag' but no UITag is set." +
                            "Used the PopupsCanvas as parent instead."
                        );
                        parent = popupsCanvas.GetComponent<RectTransform>();
                        break;
                    }
                    var uiTag = UITag.GetFirstTag(ParentTag.Category, ParentTag.Name);
                    if (uiTag == null)
                    {
                        Debug.Log
                        (
                            "[Popup] Parenting mode set to 'UITag' but the UITag is not found." +
                            "Used the PopupsCanvas as parent instead."
                        );
                        parent = popupsCanvas.GetComponent<RectTransform>();
                        break;
                    }
                    parent = uiTag.GetComponent<RectTransform>();
                    if (parent == null)
                    {
                        Debug.Log
                        (
                            "[Popup] Parenting mode set to 'UITag' but the UITag has no RectTransform component." +
                            "Used the PopupsCanvas as parent instead."
                        );
                        parent = popupsCanvas.GetComponent<RectTransform>();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return parent;
        }

        #endregion

        #region Private Methods

        /// <summary> Disable the 'Back' button if it is enabled and the popup is visible </summary>
        private void DisableBackButton()
        {
            //sanity check to make sure the 'Back' button was not already disabled by this UIPopup
            if (unblockBackButton) return;

            //check that block 'Back' button option is enabled
            if (!BlockBackButton) return;

            BackButton.Disable();
            unblockBackButton = true;
        }

        private void EnableBackButton()
        {
            //sanity check to make sure the 'Back' button was not already enabled by this UIPopup
            if (!unblockBackButton) return;

            //check that block 'Back' button option is enabled
            if (!BlockBackButton) return;

            BackButton.Enable();
            unblockBackButton = false;
        }

        /// <summary>
        /// Add on pointer click trigger on the Overlay RectTransform to hide (close) this UIPopup.
        /// If the Overlay RectTransform is not assigned, nothing will happen.
        /// Calling this method multiple time will not add multiple triggers (nor events) to the Overlay RectTransform.
        /// </summary>
        private void AddOnClickToOverlay()
        {
            if (!hasOverlay) return;
            if (!HideOnClickOverlay) return;
            if (addedHideOnClickOverlay) return;
            PointerClickTrigger clickTrigger = Overlay.GetComponent<PointerClickTrigger>() ?? Overlay.gameObject.AddComponent<PointerClickTrigger>();
            clickTrigger.OnTrigger.AddListener(evt => Hide());
            addedHideOnClickOverlay = true;
        }

        /// <summary>
        /// Add on pointer click trigger on the Container RectTransform to hide (close) this UIPopup.
        /// If the Container RectTransform is not assigned, nothing will happen.
        /// Calling this method multiple time will not add multiple triggers (nor events) to the Container RectTransform.
        /// </summary>
        private void AddOnClickToContainer()
        {
            if (!hasContainer) return;
            if (!HideOnClickContainer) return;
            if (addedHideOnClickContainer) return;
            PointerClickTrigger clickTrigger = Container.GetComponent<PointerClickTrigger>() ?? Container.gameObject.AddComponent<PointerClickTrigger>();
            clickTrigger.OnTrigger.AddListener(evt => Hide());
            addedHideOnClickContainer = true;
        }

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

        /// <summary>
        /// Save the currently selected GameObject in the EventSystem.current.
        /// </summary>
        private void SavePreviouslySelectedGameObject()
        {
            if (EventSystem.current == null) return;
            previouslySelectedGameObject = EventSystem.current.currentSelectedGameObject;
        }

        /// <summary>
        /// Restore the previously selected GameObject when the popup is hidden.
        /// </summary>
        private void RestorePreviouslySelectedGameObject()
        {
            if (!RestoreSelectedAfterHide) return;
            if (EventSystem.current == null) return;
            if (previouslySelectedGameObject == null) return;
            EventSystem.current.SetSelectedGameObject(previouslySelectedGameObject);
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Instantiate a new popup from the prefab that is registered in the database.
        /// If a prefab with the given popup name is not found, null will be returned.
        /// </summary>
        /// <param name="popupName"> The name of the popup prefab in the database. </param>
        /// <returns> The popup instance. Null if the prefab is not found. </returns>
        public static UIPopup Get(string popupName)
        {
            if (string.IsNullOrEmpty(popupName)) return null;
            GameObject prefab = UIPopupDatabase.instance.GetPrefab(popupName);
            if (prefab == null)
            {
                Debug.LogWarning($"UIPopup.Get({popupName}) - prefab not found in the database");
                return null;
            }

            UIPopup popup =
                Instantiate(prefab)
                    .GetComponent<UIPopup>()
                    .Reset();

            popup.Validate();
            popup.ApplyHideOnAnyButton();
            popup.SetParent(popup.GetParent());
            popup.InstantHide(false);

            //destroy the popup when it is hidden
            popup.OnHiddenCallback.Event.AddListener(() =>
            {
                //sanity check to make sure the popup is not already destroyed
                if (popup == null) return;
                popup.RestorePreviouslySelectedGameObject();
                Destroy(popup.gameObject);
                popup = null;
            });

            return popup;
        }

        #endregion
    }

    public static class UIPopupExtensions
    {
        /// <summary> Reset the popup to its initial state </summary>
        /// <param name="popup"> Target popup </param>
        /// <returns> The popup instance </returns>
        public static T Reset<T>(this T popup) where T : UIPopup
        {
            popup.parentRectTransform = null;
            return popup;
        }

        /// <summary> Reparent the popup to a new parent </summary>
        /// <param name="popup"> Target popup </param>
        /// <param name="parent"> The new parent </param>
        /// <returns> The popup instance </returns>
        public static T SetParent<T>(this T popup, RectTransform parent) where T : UIPopup
        {
            popup.parentRectTransform = parent;
            if (parent == null) return popup;
            popup.rectTransform.SetParent(parent, true);
            popup.rectTransform.CenterPivot().ExpandToParentSize(true);
            return popup;
        }

        /// <summary> Update the override sorting order setting </summary>
        /// <param name="popup"> Target popup </param>
        /// <param name="overrideSortingOrder"> New override sorting order value </param>
        /// <param name="apply"> TRUE to apply the new value, FALSE to only update the setting </param>
        /// <returns> The popup instance </returns>
        public static T SetOverrideSorting<T>(this T popup, bool overrideSortingOrder, bool apply = false) where T : UIPopup
        {
            popup.OverrideSorting = overrideSortingOrder;
            if (apply) popup.ApplyOverrideSorting();
            return popup;
        }

        /// <summary> Apply the override sorting order setting (if enabled) </summary>
        /// <param name="popup"> Target popup </param>
        /// <returns> The popup instance </returns>
        public static T ApplyOverrideSorting<T>(this T popup) where T : UIPopup
        {
            if (!popup.OverrideSorting)
                return popup;

            popup.canvas.overrideSorting = true;
            popup.canvas.sortingOrder = UIPopup.k_MaxSortingOrder;

            if (!popup.canvas.gameObject.activeInHierarchy)
                Debug.Log($"Cannot apply override sorting order to popup {popup.name} because it is not active in the scene");

            if (!popup.canvas.enabled)
                Debug.Log($"Cannot apply override sorting order to popup {popup.name} because its canvas is not enabled");

            return popup;
        }

        /// <summary>
        /// Set the text values for all the text mesh pro labels this popup has references to.
        /// <para/> Each string value will be set to the TextMeshProUI label with the same index in the Labels list.
        /// </summary>
        /// <param name="popup"> Target popup </param>
        /// <param name="texts"> Text values to set </param>
        /// <returns> The popup instance </returns>
        public static T SetTexts<T>(this T popup, params string[] texts) where T : UIPopup
        {
            int textsCount = texts.Length;
            if (textsCount == 0) return popup;
            for (int i = 0; i < popup.Labels.Count; i++)
            {
                TextMeshProUGUI label = popup.Labels[i];
                if (label == null) continue;
                label.SetText(i < textsCount ? texts[i] : string.Empty);
                label.ForceMeshUpdate();
            }
            return popup;
        }

        /// <summary>
        /// Set the sprite references for all the Images this popup has references to.
        /// <para/> Each Sprite will be referenced to the Image with the same index in the Images list.
        /// </summary>
        /// <param name="popup"> Target popup </param>
        /// <param name="sprites"> Sprite references to set </param>
        /// <returns> The popup instance </returns>
        public static T SetSprites<T>(this T popup, params Sprite[] sprites) where T : UIPopup
        {
            int spritesCount = sprites.Length;
            if (spritesCount == 0) return popup;
            for (int i = 0; i < popup.Images.Count; i++)
            {
                Image image = popup.Images[i];
                if (image == null) continue;
                image.sprite = i < spritesCount ? sprites[i] : null;
            }
            return popup;
        }

        /// <summary>
        /// Set the UnityEvents to invoke for all the UIButtons this popup has references to
        /// <para/> Each UnityEvent will be assigned to the UIButton with the same index in the Buttons list.
        /// <para/> The UnityEvent will be invoked when the UIButton's either on click or submit behaviour is invoked.
        /// </summary>
        /// <param name="popup"> Target popup </param>
        /// <param name="events"> UnityEvents to invoke </param>
        /// <returns> The popup instance </returns>
        public static T SetEvents<T>(this T popup, params UnityEvent[] events) where T : UIPopup
        {
            int eventsCount = events.Length;
            if (eventsCount == 0) return popup;
            bool hasGraphicRaycaster = popup.GetComponentInChildren<GraphicRaycaster>();
            for (int i = 0; i < popup.Buttons.Count; i++)
            {
                UIButton button = popup.Buttons[i];                         //get the button
                if (button == null) continue;                               //if the button is null, continue
                if (!hasGraphicRaycaster)                                   //if the popup doesn't have a graphic raycaster, check if the button has one
                    if (!button.GetComponent<GraphicRaycaster>())           //if the button doesn't have a graphic raycaster
                        button.gameObject.AddComponent<GraphicRaycaster>(); //add a graphic raycaster to the button
                if (eventsCount <= i) continue;                             //if the event count is less than the index, continue
                UnityEvent evt = events[i];                                 //get the event
                if (evt == null) continue;                                  //if the event is null, continue
                button.onClickBehaviour.Event.AddListener(evt.Invoke);      //add the event to the button on click behaviour
                button.onSubmitBehaviour.Event.AddListener(evt.Invoke);     //add the event to the button on submit behaviour
            }

            return popup;
        }

        /// <summary>
        /// Set the UnityActions to invoke for all the UIButtons this popup has references to.
        /// <para/> Each UnityAction will be assigned to the UIButton with the same index in the Buttons list.
        /// <para/> The UnityAction will be invoked when the UIButton's either on click or submit behaviour is invoked.
        /// </summary>
        /// <param name="popup"> Target popup </param>
        /// <param name="actions"> UnityActions to invoke </param>
        /// <returns> The popup instance </returns>
        public static T SetEvents<T>(this T popup, params UnityAction[] actions) where T : UIPopup
        {
            int actionsCount = actions.Length;
            if (actionsCount == 0) return popup;
            bool hasGraphicRaycaster = popup.GetComponentInChildren<GraphicRaycaster>();
            for (int i = 0; i < popup.Buttons.Count; i++)
            {
                UIButton button = popup.Buttons[i];                         //get the button
                if (button == null) continue;                               //if the button is null, continue
                if (!hasGraphicRaycaster)                                   //if the popup doesn't have a graphic raycaster, check if the button has one
                    if (!button.GetComponent<GraphicRaycaster>())           //if the button doesn't have a graphic raycaster
                        button.gameObject.AddComponent<GraphicRaycaster>(); //add a graphic raycaster to the button
                if (actionsCount <= i) continue;                            //if the event count is less than the index, continue
                UnityAction action = actions[i];                            //get the action
                if (action == null) continue;                               //if the action is null, continue
                button.onClickBehaviour.Event.AddListener(action.Invoke);   //add the action to the button on click behaviour
                button.onSubmitBehaviour.Event.AddListener(action.Invoke);  //add the action to the button on submit behaviour
            }

            return popup;
        }

        #region Popup Queue

        /// <summary> Add a popup to the popup queue and show it (if the queue is not already showing a popup). </summary>
        /// <param name="popup"> Target popup </param>
        /// <param name="queueName"> Name of the popup queue to add the popup to </param>
        /// <returns> The popup instance </returns>
        public static T ShowFromQueue<T>(this T popup, string queueName = UIPopup.k_DefaultQueueName) where T : UIPopup
        {
            if (string.IsNullOrEmpty(queueName))
            {
                Debug.LogError($"Cannot show popup {popup.name} from queue because the queue name is null or empty");
                return popup;
            }
            UIPopup.AddPopupToQueue(popup);
            return popup;
        }

        #endregion
    }
}
