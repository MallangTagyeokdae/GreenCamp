// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Global;
using Doozy.Runtime.Mody;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIManager.Events;
using Doozy.Runtime.UIManager.Input;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable UnusedMember.Global

namespace Doozy.Runtime.UIManager.Containers
{
    /// <summary>
    /// Basic container with show and hide capabilities.
    /// All other containers use this as their base.
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    // [RequireComponent(typeof(GraphicRaycaster))]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("UI/Containers/UIContainer")]
    [SelectionBase]
    public class UIContainer : MonoBehaviour, ICanvasElement, IUseMultiplayerInfo
    {
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/UI/Containers/UIContainer", false, 8)]
        private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        {
            GameObjectUtils.AddToScene<UIContainer>("UIContainer", false, true);
        }
        #endif

        /// <summary> Stream category name </summary>
        public const string k_StreamCategory = nameof(UIContainer);
        /// <summary> Default animation duration </summary>
        public const float k_DefaultAnimationDuration = 0.3f;

        #region MultiplayerInfo

        [SerializeField] private MultiplayerInfo MultiplayerInfo;
        /// <summary> Reference to the MultiPlayerInfo component </summary>
        public MultiplayerInfo multiplayerInfo => MultiplayerInfo;

        /// <summary> Check if a MultiplayerInfo has been referenced </summary>
        public bool hasMultiplayerInfo => multiplayerInfo != null;

        /// <summary> Player index for this component </summary>
        public int playerIndex => multiplayerMode & hasMultiplayerInfo ? multiplayerInfo.playerIndex : inputSettings.defaultPlayerIndex;

        /// <summary> Set the a reference to a MultiplayerInfo </summary>
        /// <param name="reference"> MultiplayerInfo reference </param>
        public void SetMultiplayerInfo(MultiplayerInfo reference) =>
            MultiplayerInfo = reference;

        #endregion

        /// <summary> Reference to the UIManager Input Settings </summary>
        public static UIManagerInputSettings inputSettings => UIManagerInputSettings.instance;

        /// <summary> Check if Multiplayer Mode is enabled </summary>
        public static bool multiplayerMode => inputSettings.multiplayerMode;

        private Canvas m_Canvas;
        /// <summary> Reference to the Canvas component attached to this GameObject </summary>
        public Canvas canvas => m_Canvas ? m_Canvas : m_Canvas = GetComponent<Canvas>();

        private CanvasGroup m_CanvasGroup;
        /// <summary> Reference to the CanvasGroup component attached to this GameObject  </summary>
        public CanvasGroup canvasGroup => m_CanvasGroup ? m_CanvasGroup : m_CanvasGroup = GetComponent<CanvasGroup>();

        private GraphicRaycaster m_GraphicRaycaster;
        /// <summary> Reference to the GraphicRaycaster component attached to this GameObject  </summary>
        public GraphicRaycaster graphicRaycaster => m_GraphicRaycaster ? m_GraphicRaycaster : m_GraphicRaycaster = GetComponent<GraphicRaycaster>();

        private RectTransform m_RectTransform;
        /// <summary> Reference to the RectTransform component attached to this GameObject  </summary>
        public RectTransform rectTransform => m_RectTransform ? m_RectTransform : m_RectTransform = GetComponent<RectTransform>();

        /// <summary> Flag used to determine if a Canvas component is attached to this GameObject </summary>
        public bool hasCanvas { get; private set; }

        /// <summary> Flag used to determine if a CanvasGroup component is attached to this GameObject </summary>
        public bool hasGraphicRaycaster { get; private set; }

        /// <summary> Flag used to determine if a CanvasGroup component is attached to this GameObject </summary>
        public bool hasCanvasGroup { get; private set; }

        /// <summary> Behaviour on Start </summary>
        public ContainerBehaviour OnStartBehaviour = ContainerBehaviour.Disabled;

        #region Visibility

        private int m_LastFrameVisibilityStateChanged;

        /// <summary> Current visibility state </summary>
        private VisibilityState m_VisibilityState = VisibilityState.Visible;

        /// <summary> Current visibility state </summary>
        public VisibilityState visibilityState
        {
            get => m_VisibilityState;
            private set => SetVisibility(value, true);
        }

        /// <summary> Visibility state is Visible </summary>
        public bool isVisible => visibilityState == VisibilityState.Visible;

        /// <summary> Visibility state is Hidden </summary>
        public bool isHidden => visibilityState == VisibilityState.Hidden;

        /// <summary> Visibility state is IsShowing - Show animation is running </summary>
        public bool isShowing => visibilityState == VisibilityState.IsShowing;

        /// <summary> Visibility state is IsHiding - Hide animation is running </summary>
        public bool isHiding => visibilityState == VisibilityState.IsHiding;

        /// <summary> Visibility state is either IsShowing or IsHiding - either Show or Hide animation is running </summary>
        public bool inTransition => isShowing || isHiding;

        /// <summary> Show animation started - executed when visibility state changed to IsShowing </summary>
        public ModyEvent OnShowCallback;

        /// <summary> Returns TRUE if the OnShowCallback event is not null and has at least one listener </summary>
        public bool hasOnShowCallbacks => OnShowCallback != null && OnShowCallback.hasCallbacks;

        /// <summary> Visible - Show animation finished - executed when visibility state changed to Visible </summary>
        public ModyEvent OnVisibleCallback;

        /// <summary> Returns TRUE if the OnVisibleCallback event is not null and has at least one listener </summary>
        public bool hasOnVisibleCallbacks => OnVisibleCallback != null && OnVisibleCallback.hasCallbacks;

        /// <summary> Hide animation started - executed when visibility state changed to IsHiding </summary>
        public ModyEvent OnHideCallback;

        /// <summary> Returns TRUE if the OnHideCallback event is not null and has at least one listener </summary>
        public bool hasOnHideCallbacks => OnHideCallback != null && OnHideCallback.hasCallbacks;

        /// <summary> Hidden - Hide animation finished - callback invoked when visibility state changed to Hidden </summary>
        public ModyEvent OnHiddenCallback;

        /// <summary> Returns TRUE if the OnHiddenCallback event is not null and has at least one listener </summary>
        public bool hasOnHiddenCallbacks => OnHiddenCallback != null && OnHiddenCallback.hasCallbacks;

        /// <summary> Visibility changed - callback invoked when visibility state changed </summary>
        public VisibilityStateEvent OnVisibilityChangedCallback;

        /// <summary> Returns TRUE if the OnVisibilityChangedCallback event is not null and has at least one listener </summary>
        public bool hasOnVisibilityChangedCallbacks => OnVisibilityChangedCallback != null && OnVisibilityChangedCallback.GetPersistentEventCount() > 0;

        /// <summary> Returns TRUE if any of the available callbacks has at least one listener </summary>
        public bool hasCallbacks => hasOnShowCallbacks | hasOnVisibleCallbacks | hasOnHideCallbacks | hasOnHiddenCallbacks | hasOnVisibilityChangedCallbacks;

        [SerializeField] private List<Progressor> ShowProgressors;
        /// <summary> Progressors triggered on Show. Plays forward. </summary>
        public List<Progressor> showProgressors => ShowProgressors ?? (ShowProgressors = new List<Progressor>());

        [SerializeField] private List<Progressor> HideProgressors;
        /// <summary> Progressors triggered on Hide. Plays forward. </summary>
        public List<Progressor> hideProgressors => HideProgressors ?? (HideProgressors = new List<Progressor>());

        [SerializeField] private List<Progressor> ShowHideProgressors;
        /// <summary> Progressors triggered on both Show and Hide. Plays forward on Show and in reverse on Hide. </summary>
        public List<Progressor> showHideProgressors => ShowHideProgressors ?? (ShowHideProgressors = new List<Progressor>());

        /// <summary> Action invoked every time before the container needs to change its state </summary>
        public UnityAction<ShowHideExecute> showHideExecute { get; set; }

        /// <summary> Flag to keep track if the first show/hide command has been issued </summary>
        public bool executedFirstCommand { get; protected set; }

        /// <summary> Keeps track of the previously executed show/hide command </summary>
        public ShowHideExecute previouslyExecutedCommand { get; protected set; }

        #endregion

        /// <summary> AnchoredPosition3D to snap to on Awake </summary>
        public Vector3 CustomStartPosition;

        /// <summary> If TRUE, the rectTransform.anchoredPosition3D will 'snap' to the CustomStartPosition on Awake </summary>
        public bool UseCustomStartPosition;

        /// <summary> If TRUE, after this container gets shown, it will get automatically hidden after the AutoHideAfterShowDelay time interval has passed </summary>
        public bool AutoHideAfterShow;

        /// <summary> If AutoHideAfterShow is TRUE, this is the time interval after which this container will get automatically hidden </summary>
        public float AutoHideAfterShowDelay;

        /// <summary> If TRUE, when this container gets hidden, the GameObject this container component is attached to, will be disabled </summary>
        public bool DisableGameObjectWhenHidden;

        /// <summary> If TRUE, when this container gets hidden, the Canvas component found on the same GameObject this container component is attached to, will be disabled </summary>
        public bool DisableCanvasWhenHidden = true;

        /// <summary> If TRUE, when this container gets hidden, the GraphicRaycaster component found on the same GameObject this container component is attached to, will be disabled </summary>
        public bool DisableGraphicRaycasterWhenHidden = true;

        /// <summary> If TRUE, when this container is shown, any GameObject that is selected by the EventSystem.current will get deselected </summary>
        public bool ClearSelectedOnShow;

        /// <summary> If TRUE, when this container is hidden, any GameObject that is selected by the EventSystem.current will get deselected </summary>
        public bool ClearSelectedOnHide;

        /// <summary> If TRUE, after this container has been shown, the referenced selectable GameObject will get automatically selected by EventSystem.current </summary>
        public bool AutoSelectAfterShow;

        /// <summary> Reference to the GameObject that should be selected after this container has been shown. Works only if AutoSelectAfterShow is TRUE </summary>
        public GameObject AutoSelectTarget;

        /// <summary> Check if there are any referenced Show reactions </summary>
        public bool hasShowReactions => showReactions != null && showReactions.Count > 0;

        /// <summary> Check if there are any referenced Hide reactions </summary>
        public bool hasHideReactions => hideReactions != null && hideReactions.Count > 0;

        /// <summary> Check if any Show animation is active (running) </summary>
        public bool anyShowAnimationIsActive => showReactions.Any(show => show.isActive);

        /// <summary> Check if any Hide animation is active (running) </summary>
        public bool anyHideAnimationIsActive => hideReactions.Any(hide => hide.isActive);

        /// <summary> Check if any Show or Hide animation is active (running) </summary>
        public bool anyAnimationIsActive => anyShowAnimationIsActive | anyHideAnimationIsActive;

        /// <summary> Check if there are any referenced Show progressors </summary>
        public bool hasShowProgressors => showProgressors.Count > 0;

        /// <summary> Check if there are any referenced Hide progressors </summary>
        public bool hasHideProgressors => hideProgressors.Count > 0;

        /// <summary> Check if there are any referenced ShowHide progressors </summary>
        public bool hasShowHideProgressors => showHideProgressors.Count > 0;

        /// <summary> Check if there are any referenced progressors </summary>
        public bool hasProgressors => hasShowProgressors || hasHideProgressors || hasShowHideProgressors;

        /// <summary> Check if any referenced Show progressor is active (running) </summary>
        public bool anyShowProgressorIsActive => showProgressors.Where(p => p != null).Any(p => p.reaction.isActive);

        /// <summary> Check if any referenced Hide progressor is active (running) </summary>
        public bool anyHideProgressorIsActive => hideProgressors.Where(p => p != null).Any(p => p.reaction.isActive);

        /// <summary> Check if any referenced ShowHide progressor is active (running) </summary>
        public bool anyShowHideProgressorIsActive => showHideProgressors.Where(p => p != null).Any(p => p.reaction.isActive);

        /// <summary> Check if any referenced progressor is active (running) </summary>
        public bool anyProgressorIsActive => anyShowProgressorIsActive | anyHideProgressorIsActive | anyShowHideProgressorIsActive;

        private HashSet<Reaction> m_ShowReactions;
        /// <summary>
        /// Collection of reactions triggered by Show.
        /// <para/> This collection is dynamically generated at runtime.
        /// It is populated by all the animators controlled by this UIContainer.
        /// The animators automatically add/remove their reactions to/from this collection. 
        /// </summary>
        internal HashSet<Reaction> showReactions => m_ShowReactions ??= new HashSet<Reaction>();
        /// <summary>
        /// Get the maximum duration for the Show animations Max(startDelay) + Max(duration).
        /// <para> At start this value can be calculated only after 2 frames have passed (the time it takes for the reactions to register) </para>
        /// <para> For reactions that use random intervals for startDelay and/or duration, the maximum interval values are taken into account </para>
        /// </summary>
        public float totalDurationForShow => CalculateTotalShowDuration();

        private HashSet<Reaction> m_HideReactions;
        /// <summary>
        /// Collection of reactions triggered by Hide.
        /// <para/> This collection is dynamically generated at runtime.
        /// It is populated by all the animators controlled by this UIContainer.
        /// The animators automatically add/remove their reactions to/from this collection. 
        /// </summary>
        internal HashSet<Reaction> hideReactions => m_HideReactions ??= new HashSet<Reaction>();
        /// <summary>
        /// Get the maximum duration for the Hide animations Max(startDelay) + Max(duration).
        /// <para> At start this value can be calculated only after 2 frames have passed (the time it takes for the reactions to register) </para>
        /// <para> For reactions that use random intervals for startDelay and/or duration, the maximum interval values are taken into account </para>
        /// </summary>
        public float totalDurationForHide => CalculateTotalHideDuration();

        private Coroutine m_AutoHideCoroutine;
        private Coroutine m_CoroutineIsShowing;
        private Coroutine m_CoroutineIsHiding;
        private Coroutine m_DisableGameObjectWithDelayCoroutine;

        #if ENABLE_INPUT_SYSTEM
        /// <summary>
        /// Reference to a MultiplayerEventSystem component that will be used to handle the input events if multiplayer mode is enabled.
        /// It only works with the new Input System.
        /// For this to work, the MultiplayerEventSystem component should me placed on the same GameObject as the MultiplayerInfo component.
        /// </summary>
        protected UnityEngine.InputSystem.UI.MultiplayerEventSystem MultiplayerEventSystem;
        #endif


        public UIContainer()
        {
            UseCustomStartPosition = true;

            OnShowCallback = new ModyEvent(nameof(OnShowCallback));
            OnVisibleCallback = new ModyEvent(nameof(OnVisibleCallback));
            OnHideCallback = new ModyEvent(nameof(OnHideCallback));
            OnHiddenCallback = new ModyEvent(nameof(OnHiddenCallback));
            OnVisibilityChangedCallback = new VisibilityStateEvent();
        }

        #if UNITY_EDITOR

        protected virtual void OnValidate()
        {
            if (!UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this) && !Application.isPlaying)
                CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }

        #endif // if UNITY_EDITOR

        #region ICanvasElement

        public virtual void Rebuild(CanvasUpdate executing) {}
        public virtual void LayoutComplete() {}
        public virtual void GraphicUpdateComplete() {}
        public bool IsDestroyed() => this == null;

        #endregion

        protected virtual void Awake()
        {
            if (!Application.isPlaying) return;
            BackButton.Initialize();

            // m_Canvas = GetComponent<Canvas>();
            // m_GraphicRaycaster = GetComponent<GraphicRaycaster>();
            hasCanvas = GetComponent<Canvas>() != null;
            hasGraphicRaycaster = GetComponent<GraphicRaycaster>() != null;
            hasCanvasGroup = GetComponent<CanvasGroup>() != null;

            showReactions.Remove(null);
            hideReactions.Remove(null);

            executedFirstCommand = false;

            if (UseCustomStartPosition)
            {
                SetCustomStartPosition(CustomStartPosition);
            }
        }

        protected virtual void OnEnable()
        {
            if (!Application.isPlaying) return;
            hasCanvas = GetComponent<Canvas>() != null;
            hasGraphicRaycaster = GetComponent<GraphicRaycaster>() != null;
            hasCanvasGroup = GetComponent<CanvasGroup>() != null;

            showReactions.Remove(null);
            hideReactions.Remove(null);
        }

        protected virtual void Start()
        {
            if (!Application.isPlaying) return;
            RunBehaviour(OnStartBehaviour);
        }

        protected virtual void OnDisable()
        {
            if (!Application.isPlaying) return;
            StopIsShowingCoroutine();
            StopIsHidingCoroutine();

            showReactions.Remove(null);
            foreach (Reaction reaction in showReactions)
                reaction.Stop();

            hideReactions.Remove(null);
            foreach (Reaction reaction in hideReactions)
                reaction.Stop();

            StopAllCoroutines();
        }

        protected virtual void OnDestroy() {}

        /// <summary>
        /// Set the custom start position for the UIContainer.
        /// </summary>
        /// <param name="startPosition"> The new start position </param>
        /// <param name="jumpToPosition"> If true, the UIContainer will be immediately moved to the new position </param>
        public virtual void SetCustomStartPosition(Vector3 startPosition, bool jumpToPosition = true)
        {
            CustomStartPosition = startPosition;

            showReactions.Remove(null);
            foreach (Reaction reaction in showReactions)
                if (reaction is UIMoveReaction moveReaction)
                    moveReaction.startPosition = startPosition;

            hideReactions.Remove(null);
            foreach (Reaction reaction in hideReactions)
                if (reaction is UIMoveReaction moveReaction)
                    moveReaction.startPosition = startPosition;

            if (jumpToPosition)
                rectTransform.anchoredPosition3D = startPosition;
        }

        /// <summary>
        /// Set the given GameObject as the selected object in the EventSystem or MultiplayerEventSystem, depending on the current input system and if multiplayer mode is enabled.
        /// </summary>
        /// <param name="selectable"> The GameObject to select </param>
        protected virtual void SetSelected(GameObject selectable)
        {
            #if ENABLE_INPUT_SYSTEM // if the new input system is enabled
            if
            (
                multiplayerMode && // multiplayer mode is enabled
                (
                    MultiplayerEventSystem != null || // multiplayer event system is already set
                    (
                        hasMultiplayerInfo &&                                                  // this UIContainer has multiplayer info
                        multiplayerInfo.gameObject.TryGetComponent(out MultiplayerEventSystem) // multiplayer event system is found
                    )
                )
            )
            {
                MultiplayerEventSystem.SetSelectedGameObject(selectable); // set selected game object in the multiplayer event system
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(selectable); // set selected game object in the default event system
            }

            #elif ENABLE_LEGACY_INPUT_MANAGER // if the old input system is enabled
            EventSystem.current.SetSelectedGameObject(gameObject); // set selected game object in the default event system
            #endif
        }

        private void ExecutedCommand(ShowHideExecute command)
        {
            showHideExecute?.Invoke(command);
            executedFirstCommand = true;
            previouslyExecutedCommand = command;

            if (!hasProgressors) return;

            showProgressors.RemoveNulls();
            hideProgressors.RemoveNulls();
            showHideProgressors.RemoveNulls();

            // ReSharper disable Unity.NoNullPropagation
            switch (command)
            {
                case ShowHideExecute.Show:
                    hideProgressors.ForEach(p => p.Stop());
                    showProgressors.ForEach(p =>
                    {
                        p.SetProgressAtZero();
                        p.Play(PlayDirection.Forward);
                    });
                    showHideProgressors.ForEach(p =>
                    {
                        p.SetProgressAtZero();
                        p.Play(PlayDirection.Forward);
                    });
                    break;
                case ShowHideExecute.Hide:
                    showProgressors.ForEach(p => p.Stop());
                    hideProgressors.ForEach(p =>
                    {
                        p.SetProgressAtZero();
                        p.Play(PlayDirection.Forward);
                    });
                    showHideProgressors.ForEach(p =>
                    {
                        p.SetProgressAtOne();
                        p.Play(PlayDirection.Reverse);
                    });
                    break;
                case ShowHideExecute.InstantShow:
                    hideProgressors.ForEach(p => p.Stop());
                    showProgressors.ForEach(p => p.SetProgressAtOne());
                    showHideProgressors.ForEach(p => p.SetProgressAtOne());
                    break;
                case ShowHideExecute.InstantHide:
                    showProgressors.ForEach(p => p.Stop());
                    hideProgressors.ForEach(p => p.SetProgressAtOne());
                    showHideProgressors.ForEach(p => p.SetProgressAtZero());
                    break;
                case ShowHideExecute.ReverseShow:
                    hideProgressors.ForEach(p => p.Stop());
                    showProgressors.ForEach(p =>
                    {
                        if (p.reaction.isActive)
                        {
                            p.Reverse();
                        }
                        else
                        {
                            p.Play(PlayDirection.Reverse);
                        }
                    });
                    showHideProgressors.ForEach(p =>
                    {
                        if (p.reaction.isActive && p.reaction.direction == PlayDirection.Forward)
                        {
                            p.Reverse();
                        }
                        else
                        {
                            p.Play(PlayDirection.Reverse);
                        }
                    });
                    break;
                case ShowHideExecute.ReverseHide:
                    showProgressors.ForEach(p => p.Stop());
                    hideProgressors.ForEach(p =>
                    {
                        if (p.reaction.isActive)
                        {
                            p.Reverse();
                        }
                        else
                        {
                            p.Play(PlayDirection.Reverse);
                        }
                    });
                    showHideProgressors.ForEach(p =>
                    {
                        if (p.reaction.isActive && p.reaction.direction == PlayDirection.Reverse)
                        {
                            p.Reverse();
                        }
                        else
                        {
                            p.Play(PlayDirection.Forward);
                        }
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(command), command, null);
            }
            // ReSharper restore Unity.NoNullPropagation
        }

        #region Instant Show/Hide/Toggle

        /// <summary>
        /// Show in the current frame without animations.
        /// <para/> Triggers visibility states IsShowing and then Visible.
        /// </summary>
        public virtual void InstantShow() =>
            InstantShow(true);

        /// <summary>
        /// Show in the current frame without animations.
        /// <para/> Triggers visibility states IsShowing and then Visible.
        /// </summary>
        /// <param name="triggerCallbacks"> Should callbacks be triggered or not </param>
        public virtual void InstantShow(bool triggerCallbacks)
        {
            if (isVisible) return;

            StopIsShowingCoroutine();
            StopIsHidingCoroutine();

            if (hasCanvas) canvas.enabled = true; //enable the canvas
            if (hasGraphicRaycaster)
            {
                graphicRaycaster.enabled = true; //enable the graphic raycaster
                if (hasCanvasGroup)
                {
                    canvasGroup.blocksRaycasts = graphicRaycaster.enabled; //blocks raycasts if the graphic raycaster is enabled
                }
            }
            gameObject.SetActive(true); //set the active state to true (in case it has been disabled when hidden)

            ExecutedCommand(ShowHideExecute.InstantShow);

            if (ClearSelectedOnShow)
            {
                SetSelected(null); //clear any selected object
            }

            if (AutoSelectAfterShow && AutoSelectTarget != null) //check that the auto select option is enabled and that a GameObject has been referenced
            {
                SetSelected(AutoSelectTarget); //select the referenced target
            }

            SetVisibility(VisibilityState.IsShowing, triggerCallbacks);
            SetVisibility(VisibilityState.Visible, triggerCallbacks);
        }

        /// <summary>
        /// Hide in the current frame without animations.
        /// <para/> Triggers visibility states IsHiding and then Hidden.
        /// </summary>
        public virtual void InstantHide() =>
            InstantHide(true);

        /// <summary>
        /// Hide in the current frame without animations.
        /// <para/> Triggers visibility states IsHiding and then Hidden.
        /// </summary>
        /// <param name="triggerCallbacks"> Should callbacks be triggered or not </param>
        public virtual void InstantHide(bool triggerCallbacks)
        {
            if (isHidden) return;

            StopIsShowingCoroutine();
            StopIsHidingCoroutine();

            ExecutedCommand(ShowHideExecute.InstantHide);

            if (ClearSelectedOnHide)
            {
                SetSelected(null); //clear any selected object
            }

            SetVisibility(VisibilityState.IsHiding, triggerCallbacks);
            SetVisibility(VisibilityState.Hidden, triggerCallbacks);
        }

        /// <summary>
        /// Toggles the visibility state.
        /// If Visible or IsShowing calls InstantHide.
        /// If Hidden or IsHiding calls InstantShow.
        /// </summary>
        public virtual void InstantToggle() =>
            InstantToggle(true);

        /// <summary>
        /// Toggles the visibility state.
        /// If Visible or IsShowing calls InstantHide.
        /// If Hidden or IsHiding calls InstantShow.
        /// </summary>
        /// <param name="triggerCallbacks"> Should callbacks be triggered or not </param>
        public virtual void InstantToggle(bool triggerCallbacks)
        {
            switch (visibilityState)
            {
                case VisibilityState.Visible:
                case VisibilityState.IsShowing:
                    InstantHide();
                    break;
                case VisibilityState.Hidden:
                case VisibilityState.IsHiding:
                    InstantShow();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region Animated Show/Hide/Toggle

        /// <summary>
        /// Show with animations.
        /// <para/> Triggers visibility states IsShowing when Show starts and then Visible when Show finished.
        /// </summary>
        public virtual void Show() =>
            Show(true);

        /// <summary>
        /// Show with animations.
        /// <para/> Triggers visibility states IsShowing when Show starts and then Visible when Show finished.
        /// </summary>
        /// <param name="triggerCallbacks"> Should callbacks be triggered or not </param>
        public virtual void Show(bool triggerCallbacks)
        {
            if (isShowing || isVisible) return;

            gameObject.SetActive(true); //set the active state to true (in case it has been disabled when hidden)

            if (m_LastFrameVisibilityStateChanged == Time.frameCount)
            {
                StartCoroutine(Coroutiner.DelayExecution(() => Show(triggerCallbacks), 2));
                return;
            }

            if (ClearSelectedOnShow)
            {
                SetSelected(null); //clear any selected object
            }

            if (isHiding)
            {
                StopIsHidingCoroutine();
                ExecutedCommand(ShowHideExecute.ReverseHide);
                m_CoroutineIsShowing = StartCoroutine(IsShowing(triggerCallbacks));
                return;
            }

            canvas.enabled = true; //enable the canvas
            if (hasGraphicRaycaster && DisableGraphicRaycasterWhenHidden)
            {
                graphicRaycaster.enabled = true; //enable the graphic raycaster
                if (hasCanvasGroup)
                {
                    canvasGroup.blocksRaycasts = graphicRaycaster.enabled; //blocks raycasts if the graphic raycaster is enabled
                }
            }

            ExecutedCommand(ShowHideExecute.Show);

            m_CoroutineIsShowing = StartCoroutine(IsShowing(triggerCallbacks));
        }

        private void StopIsShowingCoroutine()
        {
            if (m_CoroutineIsShowing == null) return;
            StopCoroutine(m_CoroutineIsShowing);
            m_CoroutineIsShowing = null;
        }

        /// <summary>
        /// Internal functionality used by the Show process.
        /// <para/> Triggered by Show
        /// </summary>
        /// <param name="triggerCallbacks"> Should callbacks be triggered or not </param>
        private IEnumerator IsShowing(bool triggerCallbacks)
        {
            StopIsHidingCoroutine();
            SetVisibility(VisibilityState.IsShowing, triggerCallbacks);
            yield return new WaitForEndOfFrame();

            while (anyAnimationIsActive)
                yield return null;

            if (hasProgressors)
                while (anyProgressorIsActive)
                    yield return null;

            if (AutoSelectAfterShow && AutoSelectTarget != null) //check that the auto select option is enabled and that a GameObject has been referenced
            {
                SetSelected(AutoSelectTarget); //select the referenced target
            }

            SetVisibility(VisibilityState.Visible, triggerCallbacks);
            m_CoroutineIsShowing = null;
        }

        /// <summary>
        /// Hide with animations.
        /// <para/> Triggers visibility states IsHiding when Hide starts and then Hidden when Hide finished.
        /// </summary>
        public virtual void Hide() =>
            Hide(true);

        /// <summary>
        /// Hide with animations.
        /// <para/> Triggers visibility states IsHiding when Hide starts and then Hidden when Hide finished.
        /// </summary>
        /// <param name="triggerCallbacks"> Should callbacks be triggered or not </param>
        public virtual void Hide(bool triggerCallbacks)
        {
            if (!isActiveAndEnabled) return;
            if (isHiding || isHidden) return;

            if (m_LastFrameVisibilityStateChanged == Time.frameCount)
            {
                StartCoroutine(Coroutiner.DelayExecution(() => Hide(triggerCallbacks), 2));
                return;
            }
            
            if (ClearSelectedOnHide)
            {
                SetSelected(null); //clear any selected object
            }

            if (isShowing)
            {
                StopIsShowingCoroutine();
                ExecutedCommand(ShowHideExecute.ReverseShow);
                m_CoroutineIsHiding = StartCoroutine(IsHiding(triggerCallbacks));
                return;
            }

            ExecutedCommand(ShowHideExecute.Hide);

            m_CoroutineIsHiding = StartCoroutine(IsHiding(triggerCallbacks));
        }

        private void StopIsHidingCoroutine()
        {
            StopDisableGameObject();
            if (m_CoroutineIsHiding == null) return;
            StopCoroutine(m_CoroutineIsHiding);
            m_CoroutineIsHiding = null;
        }

        /// <summary>
        /// Internal functionality used by the Hide process.
        /// <para/> Triggered by Hide
        /// </summary>
        /// <param name="triggerCallbacks"> Should callbacks be triggered or not </param>
        private IEnumerator IsHiding(bool triggerCallbacks)
        {
            StopDisableGameObject();
            StopIsShowingCoroutine();
            SetVisibility(VisibilityState.IsHiding, triggerCallbacks);
            yield return new WaitForEndOfFrame();

            while (anyAnimationIsActive)
                yield return null;

            if (hasProgressors)
                while (anyProgressorIsActive)
                    yield return null;

            SetVisibility(VisibilityState.Hidden, triggerCallbacks);
            m_CoroutineIsHiding = null;
        }

        /// <summary>
        /// Toggle the visibility state.
        /// If Visible or IsShowing calls Hide.
        /// If Hidden or IsHiding calls Show.
        /// </summary>
        public virtual void Toggle() =>
            Toggle(true);

        /// <summary>
        /// Toggle the visibility state.
        /// If Visible or IsShowing calls Hide.
        /// If Hidden or IsHiding calls Show.
        /// </summary>
        /// <param name="triggerCallbacks"> Should callbacks be triggered or not </param>
        public virtual void Toggle(bool triggerCallbacks)
        {
            switch (visibilityState)
            {
                case VisibilityState.Visible:
                case VisibilityState.IsShowing:
                    Hide(triggerCallbacks);
                    break;
                case VisibilityState.Hidden:
                case VisibilityState.IsHiding:
                    Show(triggerCallbacks);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        /// <summary> Set the container visibility </summary>
        /// <param name="state"> New visibility state </param>
        /// <param name="triggerCallbacks"> Should callbacks be triggered or not </param>
        internal void SetVisibility(VisibilityState state, bool triggerCallbacks)
        {
            m_LastFrameVisibilityStateChanged = Time.frameCount;
            m_VisibilityState = state;
            if (triggerCallbacks) OnVisibilityChangedCallback?.Invoke(m_VisibilityState);
            switch (state)
            {
                case VisibilityState.Visible:
                    ExecuteOnVisible(triggerCallbacks);
                    break;
                case VisibilityState.Hidden:
                    ExecuteOnHidden(triggerCallbacks);
                    break;
                case VisibilityState.IsShowing:
                    ExecuteOnShow(triggerCallbacks);
                    break;
                case VisibilityState.IsHiding:
                    ExecuteOnHide(triggerCallbacks);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        /// <summary>
        /// Execute internal operations when the Show animation
        /// is in the process (transition) of becoming visible.
        /// <para/> Triggered at the start of the Show animation.
        /// </summary>
        /// <param name="triggerCallbacks"> Should callbacks be triggered or not </param>
        private void ExecuteOnShow(bool triggerCallbacks)
        {
            if (triggerCallbacks)
            {
                OnShowCallback.Execute();
            }
        }

        /// <summary>
        /// Execute internal operations when the Hide animation
        /// is in the process (transition) of becoming hidden.
        /// <para/> Triggered at the start of the Hide animation.
        /// </summary>
        /// <param name="triggerCallbacks"> Should callbacks be triggered or not </param>
        private void ExecuteOnHide(bool triggerCallbacks)
        {
            if (triggerCallbacks)
            {
                OnHideCallback.Execute();
            }
            StopAutoHide();
        }

        /// <summary>
        /// Execute internal operations when the Show animation
        /// finished and the container Is Visible.
        /// <para/> Triggered at the end of the Show animation.
        /// </summary>
        /// <param name="triggerCallbacks"> Should callbacks be triggered or not </param>
        private void ExecuteOnVisible(bool triggerCallbacks)
        {
            if (triggerCallbacks)
            {
                OnVisibleCallback.Execute();
            }
            StartAutoHide();
        }

        /// <summary>
        /// Execute internal operations when the Hide animation
        /// finished and the container Is Hidden.
        /// <para/> Triggered at the end of the Hide animation.
        /// </summary>
        /// <param name="triggerCallbacks"> Should callbacks be triggered or not </param>
        private void ExecuteOnHidden(bool triggerCallbacks)
        {
            if (triggerCallbacks)
            {
                OnHiddenCallback.Execute();
            }
            if (hasCanvas) canvas.enabled = !DisableCanvasWhenHidden; //disable the canvas, if the option is enabled
            if (hasGraphicRaycaster)
            {
                graphicRaycaster.enabled = !DisableGraphicRaycasterWhenHidden; //disable the graphic raycaster, if the option is enabled
                if (hasCanvasGroup)
                {
                    canvasGroup.blocksRaycasts = graphicRaycaster.enabled; //blocks raycasts should be enabled if the graphic raycaster is enabled
                }
            }

            StartDisableGameObject();
        }

        private void StartDisableGameObject()
        {
            StopDisableGameObject();
            m_DisableGameObjectWithDelayCoroutine = StartCoroutine(DisableGameObjectWithDelay());
        }

        private void StopDisableGameObject()
        {
            if (m_DisableGameObjectWithDelayCoroutine == null)
                return;
            StopCoroutine(m_DisableGameObjectWithDelayCoroutine);
            m_DisableGameObjectWithDelayCoroutine = null;
        }

        private IEnumerator DisableGameObjectWithDelay()
        {
            //we need to wait for 3 frames to make sure all the connected animators have had enough time to initialize (it takes 2 frames for a position animator to get its start position from a layout group (THANKS UNITY!!!) FML)
            yield return null; //wait 1 frame (1 for the money)
            yield return null; //wait 1 frame (2 for the show)
            yield return null; //wait 1 frame (3 to get ready)
            // ...and 4 to f@#king go!
            gameObject.SetActive(!DisableGameObjectWhenHidden); //set the active state to false, if the option is enabled
        }

        private void StartAutoHide()
        {
            StopAutoHide();
            if (!AutoHideAfterShow) return;
            m_AutoHideCoroutine = StartCoroutine(AutoHideEnumerator());
        }

        private void StopAutoHide()
        {
            if (m_AutoHideCoroutine == null) return;
            StopCoroutine(m_AutoHideCoroutine);
            m_AutoHideCoroutine = null;
        }

        private IEnumerator AutoHideEnumerator()
        {
            yield return new WaitForSecondsRealtime(AutoHideAfterShowDelay);
            Hide();
            m_AutoHideCoroutine = null;
        }

        private void RunBehaviour(ContainerBehaviour behaviour)
        {
            switch (behaviour)
            {
                case ContainerBehaviour.Disabled:
                    //ignored
                    return;

                case ContainerBehaviour.InstantHide:
                    m_VisibilityState = VisibilityState.Visible;
                    InstantHide();
                    return;

                case ContainerBehaviour.InstantShow:
                    m_VisibilityState = VisibilityState.Hidden;
                    InstantShow();
                    return;

                case ContainerBehaviour.Hide:
                    m_VisibilityState = VisibilityState.Visible;
                    Hide();
                    return;

                case ContainerBehaviour.Show:
                    InstantHide(false);
                    StartCoroutine(Coroutiner.DelayExecution(Show, 2));
                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(behaviour), behaviour, null);
            }
        }

        private float CalculateTotalShowDuration()
        {
            float duration = CalculateTotalDurationForReactions(showReactions);
            float maxDelay = 0;
            float maxDuration = 0;

            showProgressors.RemoveNulls();
            foreach (FloatReaction r in showProgressors.Select(p => p.reaction))
            {
                maxDelay = Mathf.Max(maxDelay, r.settings.useRandomStartDelay ? r.settings.randomStartDelay.max : r.settings.startDelay);
                maxDuration = Mathf.Max(maxDuration, r.settings.useRandomDuration ? r.settings.randomDuration.max : r.settings.duration);
            }

            showHideProgressors.RemoveNulls();
            foreach (FloatReaction r in showHideProgressors.Select(p => p.reaction))
            {
                maxDelay = Mathf.Max(maxDelay, r.settings.useRandomStartDelay ? r.settings.randomStartDelay.max : r.settings.startDelay);
                maxDuration = Mathf.Max(maxDuration, r.settings.useRandomDuration ? r.settings.randomDuration.max : r.settings.duration);
            }

            return Mathf.Max(duration, maxDelay + maxDuration);
        }

        private float CalculateTotalHideDuration()
        {
            float duration = CalculateTotalDurationForReactions(hideReactions);
            float maxDelay = 0;
            float maxDuration = 0;

            hideProgressors.RemoveNulls();
            foreach (FloatReaction r in hideProgressors.Select(p => p.reaction))
            {
                maxDelay = Mathf.Max(maxDelay, r.settings.useRandomStartDelay ? r.settings.randomStartDelay.max : r.settings.startDelay);
                maxDuration = Mathf.Max(maxDuration, r.settings.useRandomDuration ? r.settings.randomDuration.max : r.settings.duration);
            }

            showHideProgressors.RemoveNulls();
            foreach (FloatReaction r in showHideProgressors.Select(p => p.reaction))
            {
                //don't calculate start delay as this progressor plays in reverse on hide
                // maxDelay = Mathf.Max(maxDelay, r.settings.useRandomStartDelay ? r.settings.randomStartDelay.max : r.settings.startDelay);
                maxDuration = Mathf.Max(maxDuration, r.settings.useRandomDuration ? r.settings.randomDuration.max : r.settings.duration);
            }

            return Mathf.Max(duration, maxDelay + maxDuration);
        }

        private static float CalculateTotalDurationForReactions(IEnumerable<Reaction> reactions, params Reaction[] others)
        {
            if (reactions == null) return 0f;
            float maxDelay = 0;
            float maxDuration = 0;
            foreach (Reaction r in reactions)
            {
                if (r == null) continue;
                maxDelay = Mathf.Max(maxDelay, r.settings.useRandomStartDelay ? r.settings.randomStartDelay.max : r.settings.startDelay);
                maxDuration = Mathf.Max(maxDuration, r.settings.useRandomDuration ? r.settings.randomDuration.max : r.settings.duration);
            }

            if (others == null)
                return maxDelay + maxDuration;

            foreach (Reaction r in others)
            {
                if (r == null) continue;
                maxDelay = Mathf.Max(maxDelay, r.settings.useRandomStartDelay ? r.settings.randomStartDelay.max : r.settings.startDelay);
                maxDuration = Mathf.Max(maxDuration, r.settings.useRandomDuration ? r.settings.randomDuration.max : r.settings.duration);
            }

            return maxDelay + maxDuration;
        }
    }
}
