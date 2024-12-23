// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIManager.Components;
using Doozy.Editor.UIManager.Editors.Components;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager;
using Doozy.Runtime.UIManager.Animators;
using Doozy.Runtime.UIManager.Components;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Animators.Internal
{
    public abstract class BaseUISelectableAnimatorEditor : BaseTargetComponentAnimatorEditor
    {
        protected ComponentReactionControls reactionControls { get; set; }

        protected BaseUISelectableAnimator castedAnimator => (BaseUISelectableAnimator)target;
        protected List<BaseUISelectableAnimator> castedAnimators => targets.Cast<BaseUISelectableAnimator>().ToList();

        protected FluidTab normalTab { get; set; }
        protected FluidTab highlightedTab { get; set; }
        protected FluidTab pressedTab { get; set; }
        protected FluidTab selectedTab { get; set; }
        protected FluidTab disabledTab { get; set; }

        protected FluidAnimatedContainer normalAnimatedContainer { get; set; }
        protected FluidAnimatedContainer highlightedAnimatedContainer { get; set; }
        protected FluidAnimatedContainer pressedAnimatedContainer { get; set; }
        protected FluidAnimatedContainer selectedAnimatedContainer { get; set; }
        protected FluidAnimatedContainer disabledAnimatedContainer { get; set; }

        protected SerializedProperty propertyToggleCommand { get; set; }
        protected SerializedProperty propertyNormalAnimation { get; set; }
        protected SerializedProperty propertyHighlightedAnimation { get; set; }
        protected SerializedProperty propertyPressedAnimation { get; set; }
        protected SerializedProperty propertySelectedAnimation { get; set; }
        protected SerializedProperty propertyDisabledAnimation { get; set; }

        protected bool resetToStartValue { get; set; }

        protected override void OnEnable()
        {
            if (Application.isPlaying) return;
            resetToStartValue = false;
            ResetAnimatorInitializedState();
        }

        protected override void OnDisable()
        {
            if (Application.isPlaying) return;
            if (resetToStartValue) ResetToStartValues();
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();

            normalTab?.Recycle();
            highlightedTab?.Recycle();
            pressedTab?.Recycle();
            selectedTab?.Recycle();
            disabledTab?.Recycle();

            normalAnimatedContainer?.Dispose();
            highlightedAnimatedContainer?.Dispose();
            pressedAnimatedContainer?.Dispose();
            selectedAnimatedContainer?.Dispose();
            disabledAnimatedContainer?.Dispose();
        }

        protected abstract void ResetAnimatorInitializedState();
        protected abstract void ResetToStartValues();
        protected abstract void PlayNormal();
        protected abstract void PlayHighlighted();
        protected abstract void PlayPressed();
        protected abstract void PlaySelected();
        protected abstract void PlayDisabled();
        protected abstract void HeartbeatCheck();
        
        protected virtual void InitializeReactionControls()
        {
            reactionControls =
                new ComponentReactionControls(castedAnimator.controller)
                    .AddComponentControls
                    (
                        resetCallback: () =>
                        {
                            resetToStartValue = true;
                            HeartbeatCheck();
                            ResetToStartValues();
                        },
                        normalCallback: () =>
                        {
                            if (Application.isPlaying)
                            {
                                castedAnimators.ForEach(a => a.Play(UISelectionState.Normal));
                                return;
                            }
                            resetToStartValue = true;
                            HeartbeatCheck();
                            PlayNormal();
                        },
                        highlightedCallback: () =>
                        {
                            if (Application.isPlaying)
                            {
                                castedAnimators.ForEach(a => a.Play(UISelectionState.Highlighted));
                                return;
                            }
                            resetToStartValue = true;
                            HeartbeatCheck();
                            PlayHighlighted();
                        },
                        pressedCallback: () =>
                        {
                            if (Application.isPlaying)
                            {
                                castedAnimators.ForEach(a => a.Play(UISelectionState.Pressed));
                                return;
                            }
                            resetToStartValue = true;
                            HeartbeatCheck();
                            PlayPressed();
                        },
                        selectedCallback: () =>
                        {
                            if (Application.isPlaying)
                            {
                                castedAnimators.ForEach(a => a.Play(UISelectionState.Selected));
                                return;
                            }
                            resetToStartValue = true;
                            HeartbeatCheck();
                            PlaySelected();
                        },
                        disabledCallback: () =>
                        {
                            if (Application.isPlaying)
                            {
                                castedAnimators.ForEach(a => a.Play(UISelectionState.Disabled));
                                return;
                            }
                            resetToStartValue = true;
                            HeartbeatCheck();
                            PlayDisabled();
                        }
                    );
        }

        protected override void FindProperties()
        {
            base.FindProperties();
            propertyToggleCommand = serializedObject.FindProperty("ToggleCommand");
            propertyNormalAnimation = serializedObject.FindProperty("NormalAnimation");
            propertyHighlightedAnimation = serializedObject.FindProperty("HighlightedAnimation");
            propertyPressedAnimation = serializedObject.FindProperty("PressedAnimation");
            propertySelectedAnimation = serializedObject.FindProperty("SelectedAnimation");
            propertyDisabledAnimation = serializedObject.FindProperty("DisabledAnimation");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();
            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(UISelectable)))
                .SetIcon(EditorSpriteSheets.UIManager.Icons.UISelectableAnimator);
            InitializeReactionControls();
            InitializeNormal();
            InitializeHighlighted();
            InitializePressed();
            InitializeSelected();
            InitializeDisabled();

            //refresh tabs enabled indicator
            root.schedule.Execute(() =>
            {
                void UpdateIndicator(FluidTab tab, bool toggleOn, bool animateChange)
                {
                    if (tab == null) return;
                    if (tab.indicator.isOn != toggleOn)
                        tab.indicator.Toggle(toggleOn, animateChange);
                }

                //initial indicators state update (no animation)
                UpdateIndicator(normalTab, castedAnimator.IsStateEnabled(UISelectionState.Normal), false);
                UpdateIndicator(highlightedTab, castedAnimator.IsStateEnabled(UISelectionState.Highlighted), false);
                UpdateIndicator(pressedTab, castedAnimator.IsStateEnabled(UISelectionState.Pressed), false);
                UpdateIndicator(selectedTab, castedAnimator.IsStateEnabled(UISelectionState.Selected), false);
                UpdateIndicator(disabledTab, castedAnimator.IsStateEnabled(UISelectionState.Disabled), false);

                //subsequent indicators state update (animated)
                root.schedule.Execute(() =>
                {
                    UpdateIndicator(normalTab, castedAnimator.IsStateEnabled(UISelectionState.Normal), true);
                    UpdateIndicator(highlightedTab, castedAnimator.IsStateEnabled(UISelectionState.Highlighted), true);
                    UpdateIndicator(pressedTab, castedAnimator.IsStateEnabled(UISelectionState.Pressed), true);
                    UpdateIndicator(selectedTab, castedAnimator.IsStateEnabled(UISelectionState.Selected), true);
                    UpdateIndicator(disabledTab, castedAnimator.IsStateEnabled(UISelectionState.Disabled), true);
                    
                }).Every(200);;
            });
        }

        private FluidTab GetFluidTab(string labelText, UnityAction<bool> callback) =>
            FluidTab.Get()
                .SetLabelText(labelText)
                .SetElementSize(ElementSize.Small)
                .IndicatorSetEnabledColor(accentColor)
                .ButtonSetAccentColor(selectableAccentColor)
                .ButtonSetOnValueChanged(evt => callback?.Invoke(evt.newValue))
                .AddToToggleGroup(tabsGroup);

        protected virtual void InitializeNormal()
        {
            normalAnimatedContainer = new FluidAnimatedContainer("Normal", true).Hide(false);
            normalTab = GetFluidTab("Normal", value => normalAnimatedContainer.Toggle(value));
            normalAnimatedContainer.AddOnShowCallback(() =>
            {
                normalAnimatedContainer
                    .AddContent(DesignUtils.NewPropertyField(propertyNormalAnimation))
                    .Bind(serializedObject);
            });
        }

        protected virtual void InitializeHighlighted()
        {
            highlightedAnimatedContainer = new FluidAnimatedContainer("Highlighted", true).Hide(false);
            highlightedTab = GetFluidTab("Highlighted", value => highlightedAnimatedContainer.Toggle(value));
            highlightedAnimatedContainer.AddOnShowCallback(() =>
            {
                highlightedAnimatedContainer
                    .AddContent(DesignUtils.NewPropertyField(propertyHighlightedAnimation))
                    .Bind(serializedObject);
            });
        }

        protected virtual void InitializePressed()
        {
            pressedAnimatedContainer = new FluidAnimatedContainer("Pressed", true).Hide(false);
            pressedTab = GetFluidTab("Pressed", value => pressedAnimatedContainer.Toggle(value));
            pressedAnimatedContainer.AddOnShowCallback(() =>
            {
                pressedAnimatedContainer
                    .AddContent(DesignUtils.NewPropertyField(propertyPressedAnimation))
                    .Bind(serializedObject);
            });
        }

        protected virtual void InitializeSelected()
        {
            selectedAnimatedContainer = new FluidAnimatedContainer("Selected", true).Hide(false);
            selectedTab = GetFluidTab("Selected", value => selectedAnimatedContainer.Toggle(value));
            selectedAnimatedContainer.AddOnShowCallback(() =>
            {
                selectedAnimatedContainer
                    .AddContent(DesignUtils.NewPropertyField(propertySelectedAnimation))
                    .Bind(serializedObject);
            });
        }

        protected virtual void InitializeDisabled()
        {
            disabledAnimatedContainer = new FluidAnimatedContainer("Disabled", true).Hide(false);
            disabledTab = GetFluidTab("Disabled", value => disabledAnimatedContainer.Toggle(value));
            disabledAnimatedContainer.AddOnShowCallback(() =>
            {
                disabledAnimatedContainer
                    .AddContent(DesignUtils.NewPropertyField(propertyDisabledAnimation))
                    .Bind(serializedObject);
            });
        }

        protected override VisualElement Toolbar()
        {
            return base.Toolbar()
                .AddChild(normalTab)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(highlightedTab)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(pressedTab)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(selectedTab)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(disabledTab)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(DesignUtils.flexibleSpace);
        }


        protected override VisualElement Content()
        {
            return base.Content()
                .AddChild(normalAnimatedContainer)
                .AddChild(highlightedAnimatedContainer)
                .AddChild(pressedAnimatedContainer)
                .AddChild(selectedAnimatedContainer)
                .AddChild(disabledAnimatedContainer);
        }


        protected override void Compose()
        {
            root
                .AddChild(reactionControls)
                .AddChild(componentHeader)
                .AddChild(Toolbar())
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(Content())
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(GetController(propertyController, propertyToggleCommand))
                .AddChild(DesignUtils.endOfLineBlock);
        }

        protected internal static FluidField GetController(SerializedProperty controllerProperty, SerializedProperty toggleCommandProperty)
        {
            ObjectField controllerObjectField =
                DesignUtils.NewObjectField(controllerProperty, typeof(UISelectable))
                    .SetTooltip($"{ObjectNames.NicifyVariableName(nameof(UISelectable))} controller")
                    .SetStyleFlexGrow(1);

            EnumField toggleCommandEnumField =
                DesignUtils.NewEnumField(toggleCommandProperty)
                    .SetStyleWidth(50, 50, 50)
                    .SetStyleAlignSelf(Align.Center)
                    .SetStyleMarginRight(DesignUtils.k_Spacing);

            void ShowToggleCommand(bool show) =>
                toggleCommandEnumField.SetStyleDisplay(show ? DisplayStyle.Flex : DisplayStyle.None);

            ShowToggleCommand(controllerProperty.objectReferenceValue != null && ((UISelectable)controllerProperty.objectReferenceValue).isToggle);
            controllerObjectField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue == null)
                {
                    ShowToggleCommand(false);
                    return;
                }

                ShowToggleCommand(((UISelectable)evt.newValue).isToggle);
            });

            return FluidField.Get()
                .SetLabelText($"Controller")
                .SetIcon(UISelectableEditor.selectableIconTextures)
                .SetStyleMinWidth(200)
                .AddFieldContent
                (
                    DesignUtils.row
                        .SetStyleFlexGrow(0)
                        .AddChild(toggleCommandEnumField)
                        .AddChild(controllerObjectField)
                );
        }
    }
}
