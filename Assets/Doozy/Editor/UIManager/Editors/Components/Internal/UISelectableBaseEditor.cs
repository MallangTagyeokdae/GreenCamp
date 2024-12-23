// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Components;
using Doozy.Editor.Reactor.Ticker;
using Doozy.Editor.UIManager.Components;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager;
using Doozy.Runtime.UIManager.Animators;
using Doozy.Runtime.UIManager.Components;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global

namespace Doozy.Editor.UIManager.Editors.Components.Internal
{
    public abstract class UISelectableBaseEditor : UnityEditor.Editor
    {
        protected const string k_ShowNavigationKey = "SelectableEditor.ShowNavigation";
        protected static bool showNavigation { get; set; }

        public virtual Color accentColor => EditorColors.UIManager.UIComponent;
        public virtual EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.UIComponent;

        public UISelectable castedSelectable => (UISelectable)target;
        public List<UISelectable> castedSelectables => targets.Cast<UISelectable>().ToList();

        protected List<BaseUISelectableAnimator> selectableAnimators { get; set; }

        protected VisualElement root { get; set; }
        protected ComponentReactionControls reactionControls { get; set; }
        protected FluidComponentHeader componentHeader { get; set; }
        protected VisualElement toolbarContainer { get; private set; }
        protected VisualElement contentContainer { get; private set; }

        protected FluidToggleGroup tabsGroup { get; set; }
        protected FluidTab settingsTab { get; set; }
        protected FluidTab statesTab { get; set; }
        protected FluidTab behavioursTab { get; set; }
        protected FluidTab navigationTab { get; set; }

        protected FluidAnimatedContainer settingsAnimatedContainer { get; set; }
        protected FluidAnimatedContainer statesAnimatedContainer { get; set; }
        protected FluidAnimatedContainer behavioursAnimatedContainer { get; set; }
        protected FluidAnimatedContainer navigationAnimatedContainer { get; set; }

        protected SerializedProperty propertyInteractable { get; set; }
        protected SerializedProperty propertyNavigationMode { get; set; }
        protected SerializedProperty propertyNavigation { get; set; }
        protected SerializedProperty propertyNavigationSelectOnDown { get; set; }
        protected SerializedProperty propertyNavigationSelectOnLeft { get; set; }
        protected SerializedProperty propertyNavigationSelectOnRight { get; set; }
        protected SerializedProperty propertyNavigationSelectOnUp { get; set; }

        protected SerializedProperty propertyCurrentUISelectionState { get; set; }
        protected SerializedProperty propertyCurrentStateName { get; set; }
        protected SerializedProperty propertyNormalState { get; set; }
        protected SerializedProperty propertyHighlightedState { get; set; }
        protected SerializedProperty propertyPressedState { get; set; }
        protected SerializedProperty propertySelectedState { get; set; }
        protected SerializedProperty propertyDisabledState { get; set; }
        protected SerializedProperty propertyDeselectAfterPress { get; set; }
        private SerializedProperty propertyBehaviours { get; set; }

        protected bool resetToStartValue { get; set; }

        protected virtual void OnEnable()
        {
            showNavigation = EditorPrefs.GetBool(k_ShowNavigationKey);
            if (showNavigation) RegisterToSceneView();

            if (Application.isPlaying) return;
            resetToStartValue = false;
            SearchForAnimators();
            ResetAnimatorInitializedState();
        }

        protected virtual void OnDisable()
        {
            UnregisterFromSceneView();
        }

        protected virtual void OnDestroy()
        {
            if (resetToStartValue) ResetToStartValues();
            
            reactionControls?.Dispose();

            componentHeader?.Recycle();

            tabsGroup?.Recycle();
            settingsTab?.Dispose();
            statesTab?.Dispose();
            navigationTab?.Dispose();

            statesAnimatedContainer?.Dispose();
            navigationAnimatedContainer?.Dispose();
            settingsAnimatedContainer?.Dispose();
        }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        protected virtual void ResetAnimatorInitializedState()
        {
            foreach (UISelectable s in castedSelectables)
                foreach (var animator in selectableAnimators.RemoveNulls())
                    if (animator.controller == s)
                        animator.animatorInitialized = false;
        }

        protected virtual void ResetToStartValues()
        {
            foreach (UISelectable s in castedSelectables)
                foreach (var a in selectableAnimators.RemoveNulls())
                    if (a.controller == s)
                    {
                        a.StopAllReactions();
                        a.ResetToStartValues();
                    }
        }

        protected virtual void SetState(UISelectionState state)
        {
            /*if (state == UISelectionState.Selected) {
                state = UISelectionState.Normal;
            }*/

            foreach (UISelectable s in castedSelectables)
            {
                if (Application.isPlaying)
                {
                    s.SetState(state);
                    continue;
                }

                HeartbeatCheck();
                foreach (var a in selectableAnimators.RemoveNulls())
                    if (a.controller == s)
                        a.Play(state);
            }
        }

        protected abstract void SearchForAnimators();

        protected virtual void HeartbeatCheck()
        {
            if (Application.isPlaying) return;

            foreach (UISelectable s in castedSelectables)
                foreach (var a in selectableAnimators.RemoveNulls())
                    if (a.controller == s && a.animatorInitialized == false)
                    {
                        resetToStartValue = true;
                        a.InitializeAnimator();
                        foreach (EditorHeartbeat eh in a.SetHeartbeat<EditorHeartbeat>().Cast<EditorHeartbeat>())
                            eh.StartSceneViewRefresh(a);
                    }
        }

        protected virtual void InitializeReactionControls()
        {
            reactionControls =
                new ComponentReactionControls(castedSelectable)
                    .SetBackgroundColor(color: EditorColors.Default.BoxBackground)
                    .AddComponentControls
                    (
                        resetCallback: () =>
                        {
                            HeartbeatCheck();
                            ResetToStartValues();
                        },
                        normalCallback: () => SetState(UISelectionState.Normal),
                        highlightedCallback: () => SetState(UISelectionState.Highlighted),
                        pressedCallback: () => SetState(UISelectionState.Pressed),
                        selectedCallback: () => SetState(UISelectionState.Selected),
                        disabledCallback: () => SetState(UISelectionState.Disabled)
                    )
                    .SetButtonSetAccentColor(selectableAccentColor);
        }

        protected virtual void FindProperties()
        {
            propertyInteractable = serializedObject.FindProperty("m_Interactable");

            propertyCurrentUISelectionState = serializedObject.FindProperty("CurrentUISelectionState");
            propertyCurrentStateName = serializedObject.FindProperty("CurrentStateName");
            propertyNormalState = serializedObject.FindProperty("NormalState");
            propertyHighlightedState = serializedObject.FindProperty("HighlightedState");
            propertyPressedState = serializedObject.FindProperty("PressedState");
            propertySelectedState = serializedObject.FindProperty("SelectedState");
            propertyDisabledState = serializedObject.FindProperty("DisabledState");

            propertyNavigation = serializedObject.FindProperty("m_Navigation");
            propertyNavigationMode = propertyNavigation.FindPropertyRelative("m_Mode");
            propertyNavigationSelectOnUp = propertyNavigation.FindPropertyRelative("m_SelectOnUp");
            propertyNavigationSelectOnDown = propertyNavigation.FindPropertyRelative("m_SelectOnDown");
            propertyNavigationSelectOnLeft = propertyNavigation.FindPropertyRelative("m_SelectOnLeft");
            propertyNavigationSelectOnRight = propertyNavigation.FindPropertyRelative("m_SelectOnRight");

            propertyDeselectAfterPress = serializedObject.FindProperty("DeselectAfterPress");

            propertyBehaviours = serializedObject.FindProperty("Behaviours");
        }

        protected virtual void InitializeEditor()
        {
            FindProperties();
            root = DesignUtils.GetEditorRoot();
            componentHeader = DesignUtils.editorComponentHeader.SetAccentColor(accentColor);
            toolbarContainer = DesignUtils.editorToolbarContainer;
            tabsGroup = FluidToggleGroup.Get().SetControlMode(FluidToggleGroup.ControlMode.OneToggleOn);
            contentContainer = DesignUtils.editorContentContainer;

            InitializeReactionControls();
            InitializeSettings();
            InitializeStates();
            BehavioursCallbacks();
            InitializeNavigation();

            root.schedule.Execute(() => settingsTab.ButtonSetIsOn(true, false));

            //refresh tabs enabled indicator
            root.schedule.Execute(() =>
            {
                void UpdateIndicator(FluidTab tab, bool toggleOn, bool animateChange)
                {
                    if (tab == null) return;
                    if (tab.indicator.isOn != toggleOn)
                        tab.indicator.Toggle(toggleOn, animateChange);
                }

                bool HasStates() =>
                    castedSelectable.normalState.stateEvent.hasCallbacks ||
                    castedSelectable.highlightedState.stateEvent.hasCallbacks ||
                    castedSelectable.pressedState.stateEvent.hasCallbacks ||
                    castedSelectable.selectedState.stateEvent.hasCallbacks ||
                    castedSelectable.disabledState.stateEvent.hasCallbacks;

                bool HasBehaviours()
                {
                    UIBehaviours behaviours = castedSelectable.behaviours;
                    return
                        behaviours?.behaviours != null &&
                        behaviours.behaviours.Count > 0 &&
                        behaviours.behaviours.Any(b => b.hasCallbacks);
                }

                //initial indicators state update (no animation)
                UpdateIndicator(statesTab, HasStates(), false);
                UpdateIndicator(behavioursTab, HasBehaviours(), false);
                UpdateIndicator(navigationTab, showNavigation, false);

                //subsequent indicators state update (animated)
                root.schedule.Execute(() =>
                {
                    UpdateIndicator(statesTab, HasStates(), true);
                    UpdateIndicator(behavioursTab, HasBehaviours(), true);
                    UpdateIndicator(navigationTab, showNavigation, true);

                }).Every(200);
            });
        }

        protected FluidTab GetTab(string labelText) =>
            new FluidTab()
                .SetLabelText(labelText)
                .IndicatorSetEnabledColor(accentColor)
                .ButtonSetAccentColor(selectableAccentColor)
                .AddToToggleGroup(tabsGroup);

        protected virtual void InitializeSettings()
        {
            settingsAnimatedContainer = new FluidAnimatedContainer("Settings", true).Hide(false);
            settingsTab =
                GetTab("Settings")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Settings)
                    .ButtonSetOnValueChanged(evt => settingsAnimatedContainer.Toggle(evt.newValue, evt.animateChange));

            settingsAnimatedContainer.SetOnShowCallback(() =>
            {
                FluidToggleCheckbox interactableCheckbox = FluidToggleCheckbox.Get()
                    .SetLabelText("Interactable")
                    .SetTooltip("Can the Selectable be interacted with?")
                    .BindToProperty(propertyInteractable);

                FluidToggleCheckbox deselectAfterPressCheckbox = FluidToggleCheckbox.Get()
                    .SetLabelText("Deselect after Press")
                    .BindToProperty(propertyDeselectAfterPress);

                settingsAnimatedContainer
                    .AddContent
                    (
                        DesignUtils.row
                            .AddChild(interactableCheckbox)
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(deselectAfterPressCheckbox)
                    )
                    .Bind(serializedObject);
            });

        }

        protected virtual void InitializeStates()
        {
            statesAnimatedContainer = new FluidAnimatedContainer("States", true).Hide(false);
            statesTab =
                GetTab("States")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.SelectableStates)
                    .IndicatorSetEnabledColor(DesignUtils.callbacksColor)
                    .ButtonSetAccentColor(DesignUtils.callbackSelectableColor)
                    .ButtonSetOnValueChanged(evt => statesAnimatedContainer.Toggle(evt.newValue, evt.animateChange));

            statesAnimatedContainer.SetOnShowCallback(() =>
            {
                statesAnimatedContainer
                    .AddContent(DesignUtils.NewPropertyField(propertyNormalState))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(DesignUtils.NewPropertyField(propertyHighlightedState))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(DesignUtils.NewPropertyField(propertyPressedState))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(DesignUtils.NewPropertyField(propertySelectedState))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(DesignUtils.NewPropertyField(propertyDisabledState))
                    .Bind(serializedObject);
            });
        }

        protected virtual void BehavioursCallbacks()
        {
            behavioursAnimatedContainer = new FluidAnimatedContainer("Behaviours", true).Hide(false);
            behavioursTab =
                GetTab("Behaviours")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.UIBehaviour)
                    .IndicatorSetEnabledColor(DesignUtils.callbacksColor)
                    .ButtonSetAccentColor(DesignUtils.callbackSelectableColor)
                    .ButtonSetOnValueChanged(evt => behavioursAnimatedContainer.Toggle(evt.newValue, evt.animateChange));

            behavioursAnimatedContainer.SetOnShowCallback(() =>
            {
                behavioursAnimatedContainer
                    .AddContent(DesignUtils.NewPropertyField(propertyBehaviours))
                    .Bind(serializedObject);
            });
        }

        protected virtual void InitializeNavigation()
        {
            navigationAnimatedContainer = new FluidAnimatedContainer("Navigation", true).Hide(false);
            navigationTab =
                GetTab("Navigation")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Navigation)
                    .ButtonSetOnValueChanged(evt => navigationAnimatedContainer.Toggle(evt.newValue, evt.animateChange));

            navigationAnimatedContainer.SetOnShowCallback(() =>
            {
                FluidField NavigationSelectField(string text, IEnumerable<Texture2D> textures, SerializedProperty property) =>
                    FluidField.Get()
                        .SetLabelText(text)
                        .SetIcon(textures)
                        .SetElementSize(ElementSize.Small)
                        .AddFieldContent
                        (
                            DesignUtils.NewObjectField(property, typeof(Selectable))
                                .SetStyleFlexGrow(1)
                        );

                VisualElement explicitNavigationContainer =
                    new VisualElement()
                        .SetName("Explicit Navigation")
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(NavigationSelectField("Select On Up", EditorSpriteSheets.EditorUI.Arrows.ArrowUp, propertyNavigationSelectOnUp))
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(NavigationSelectField("Select On Down", EditorSpriteSheets.EditorUI.Arrows.ArrowDown, propertyNavigationSelectOnDown))
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(NavigationSelectField("Select On Left", EditorSpriteSheets.EditorUI.Arrows.ArrowLeft, propertyNavigationSelectOnLeft))
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(NavigationSelectField("Select On Right", EditorSpriteSheets.EditorUI.Arrows.ArrowRight, propertyNavigationSelectOnRight));

                EnumField navigationModeEnumField = DesignUtils.NewEnumField(propertyNavigationMode).SetStyleFlexGrow(1).SetStyleHeight(26);
                navigationModeEnumField.RegisterValueChangedCallback(evt =>
                {
                    if (evt?.newValue == null) return;
                    explicitNavigationContainer?.SetStyleDisplay((Navigation.Mode)evt.newValue == Navigation.Mode.Explicit ? DisplayStyle.Flex : DisplayStyle.None);
                });

                explicitNavigationContainer?.SetStyleDisplay((Navigation.Mode)propertyNavigationMode.enumValueIndex == Navigation.Mode.Explicit ? DisplayStyle.Flex : DisplayStyle.None);

                FluidToggleButtonTab visualizeNavigationButton =
                    FluidToggleButtonTab.Get()
                        .SetLabelText("Navigation")
                        .SetToggleAccentColor(selectableAccentColor)
                        .SetElementSize(ElementSize.Small)
                        .SetTabPosition(TabPosition.FloatingTab)
                        .SetStyleFlexShrink(0)
                        .SetIsOn(showNavigation, false);

                visualizeNavigationButton.SetOnClick(() =>
                {
                    showNavigation = !showNavigation;
                    UpdateVisualizeNavigationButton(visualizeNavigationButton);
                });

                UpdateVisualizeNavigationButton(visualizeNavigationButton);

                navigationAnimatedContainer
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent
                    (
                        FluidField.Get("Navigation Mode")
                            .SetIcon(EditorSpriteSheets.EditorUI.Icons.Navigation)
                            .AddFieldContent
                            (
                                DesignUtils.row
                                    .SetStyleJustifyContent(Justify.FlexEnd)
                                    .AddChild(navigationModeEnumField)
                                    .AddChild(DesignUtils.spaceBlock)
                                    .AddChild(visualizeNavigationButton)
                            )
                    )
                    .AddContent(explicitNavigationContainer)
                    .AddContent(DesignUtils.endOfLineBlock)
                    .Bind(serializedObject);
            });
        }

        protected virtual VisualElement Toolbar()
        {
            return
                toolbarContainer
                    .AddChild(settingsTab)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(statesTab)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(behavioursTab)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(navigationTab)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(DesignUtils.flexibleSpace)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild
                    (
                        DesignUtils.SystemButton_SortComponents
                        (
                            ((UISelectable)target).gameObject,
                            nameof(RectTransform),
                            nameof(UISelectable)
                        )
                    );
        }

        protected virtual VisualElement Content()
        {
            return
                contentContainer
                    .AddChild(settingsAnimatedContainer)
                    .AddChild(statesAnimatedContainer)
                    .AddChild(behavioursAnimatedContainer)
                    .AddChild(navigationAnimatedContainer);
        }

        protected virtual void Compose()
        {
            root
                .AddChild(reactionControls)
                .AddChild(componentHeader)
                .AddChild(Toolbar())
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(Content())
                .AddChild(DesignUtils.endOfLineBlock);
        }

        private void UpdateVisualizeNavigationButton(FluidToggleButtonTab navigationButton)
        {
            if (showNavigation)
            {
                RegisterToSceneView();
                navigationButton
                    .SetLabelText("Hide Navigation")
                    .SetTooltip("Hide selectable navigation flow in Scene View")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Hide);
            }
            else
            {
                UnregisterFromSceneView();
                navigationButton
                    .SetLabelText("Show Navigation")
                    .SetTooltip("Show selectable navigation flow in Scene View")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Show);
            }
            EditorPrefs.SetBool(k_ShowNavigationKey, showNavigation);
            SceneView.RepaintAll();
        }

        #region Visualize Navigation

        private static void DrawNavigation(SceneView sceneView)
        {
            if (!showNavigation)
                return;

            foreach (Selectable s in Selectable.allSelectablesArray.Where(s => s != null))
            {
                if (StageUtility.IsGameObjectRenderedByCamera(s.gameObject, Camera.current))
                    DrawNavigationForSelectable(s);
            }
        }

        private static void DrawNavigationForSelectable(Selectable sel)
        {
            if (sel == null)
                return;

            Transform transform = sel.transform;
            bool active = Selection.transforms.Any(e => e == transform);

            Handles.color = new Color(1.0f, 0.6f, 0.2f, active ? 1.0f : 0.4f);
            DrawNavigationArrow(-Vector2.right, sel, sel.FindSelectableOnLeft());
            DrawNavigationArrow(Vector2.up, sel, sel.FindSelectableOnUp());

            Handles.color = new Color(1.0f, 0.9f, 0.1f, active ? 1.0f : 0.4f);
            DrawNavigationArrow(Vector2.right, sel, sel.FindSelectableOnRight());
            DrawNavigationArrow(-Vector2.up, sel, sel.FindSelectableOnDown());
        }

        private const float K_ARROW_THICKNESS = 2.5f;
        private const float K_ARROW_HEAD_SIZE = 1.2f;

        private static void DrawNavigationArrow(Vector2 direction, Selectable fromObj, Selectable toObj)
        {
            if (fromObj == null || toObj == null)
                return;
            Transform fromTransform = fromObj.transform;
            Transform toTransform = toObj.transform;

            var sideDir = new Vector2(direction.y, -direction.x);
            Vector3 fromPoint = fromTransform.TransformPoint(GetPointOnRectEdge(fromTransform as RectTransform, direction));
            Vector3 toPoint = toTransform.TransformPoint(GetPointOnRectEdge(toTransform as RectTransform, -direction));
            float fromSize = HandleUtility.GetHandleSize(fromPoint) * 0.05f;
            float toSize = HandleUtility.GetHandleSize(toPoint) * 0.05f;
            fromPoint += fromTransform.TransformDirection(sideDir) * fromSize;
            toPoint += toTransform.TransformDirection(sideDir) * toSize;
            float length = Vector3.Distance(fromPoint, toPoint);
            Vector3 fromTangent = fromTransform.rotation * direction * length * 0.3f;
            Quaternion rotation = toTransform.rotation;
            Vector3 toTangent = rotation * -direction * length * 0.3f;

            Handles.DrawBezier(fromPoint, toPoint, fromPoint + fromTangent, toPoint + toTangent, Handles.color, null, K_ARROW_THICKNESS);
            Handles.DrawAAPolyLine(K_ARROW_THICKNESS, toPoint, toPoint + rotation * (-direction - sideDir) * toSize * K_ARROW_HEAD_SIZE);
            Handles.DrawAAPolyLine(K_ARROW_THICKNESS, toPoint, toPoint + rotation * (-direction + sideDir) * toSize * K_ARROW_HEAD_SIZE);
        }

        private static Vector3 GetPointOnRectEdge(RectTransform rect, Vector2 dir)
        {
            if (rect == null)
                return Vector3.zero;
            if (dir != Vector2.zero)
                dir /= Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y));
            Rect rectRect = rect.rect;
            dir = rectRect.center + Vector2.Scale(rectRect.size, dir * 0.5f);
            return dir;
        }

        #endregion

        #region SceneView

        private bool registeredToSceneView { get; set; }
        private void RegisterToSceneView()
        {
            if (registeredToSceneView) return;
            SceneView.duringSceneGui += DrawNavigation;
            registeredToSceneView = true;
        }
        private void UnregisterFromSceneView()
        {
            if (!registeredToSceneView) return;
            SceneView.duringSceneGui -= DrawNavigation;
            registeredToSceneView = false;
        }

        #endregion
    }
}
