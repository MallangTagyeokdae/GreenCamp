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
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Animators;
using Doozy.Runtime.UIManager.Containers;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Animators.Internal
{
    public abstract class BaseUIContainerAnimatorEditor : BaseTargetComponentAnimatorEditor
    {
        protected ContainerReactionControls reactionControls { get; set; }

        protected BaseUIContainerAnimator castedAnimator => (BaseUIContainerAnimator)target;
        protected List<BaseUIContainerAnimator> castedAnimators => targets.Cast<BaseUIContainerAnimator>().ToList();

        protected FluidTab showTab { get; set; }
        protected FluidTab hideTab { get; set; }

        protected FluidAnimatedContainer showAnimatedContainer { get; set; }
        protected FluidAnimatedContainer hideAnimatedContainer { get; set; }

        protected SerializedProperty propertyShowAnimation { get; set; }
        protected SerializedProperty propertyHideAnimation { get; set; }

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

            reactionControls?.Dispose();

            showTab?.Recycle();
            hideTab?.Recycle();

            showAnimatedContainer?.Dispose();
            hideAnimatedContainer?.Dispose();
        }

        protected abstract void ResetAnimatorInitializedState();
        protected abstract void ResetToStartValues();
        protected abstract void PlayShow();
        protected abstract void PlayHide();
        protected abstract void PlayReverseShow();
        protected abstract void PlayReverseHide();
        protected abstract void HeartbeatCheck();

        protected virtual void InitializeReactionControls()
        {
            reactionControls =
                new ContainerReactionControls()
                    .AddContainerControls
                    (
                        resetCallback: () =>
                        {
                            resetToStartValue = true;
                            HeartbeatCheck();
                            ResetToStartValues();
                        },
                        showCallback: () =>
                        {
                            if (Application.isPlaying)
                            {
                                castedAnimators.ForEach(a => a.Show());
                                return;
                            }
                            resetToStartValue = true;
                            HeartbeatCheck();
                            PlayShow();
                        },
                        hideCallback: () =>
                        {
                            if (Application.isPlaying)
                            {
                                castedAnimators.ForEach(a => a.Hide());
                                return;
                            }
                            resetToStartValue = true;
                            HeartbeatCheck();
                            PlayHide();
                        },
                        reverseShowCallback: () =>
                        {
                            if (Application.isPlaying) return;
                            resetToStartValue = true;
                            HeartbeatCheck();
                            PlayReverseShow();
                        },
                        reverseHideCallback: () =>
                        {
                            if (Application.isPlaying) return;
                            resetToStartValue = true;
                            HeartbeatCheck();
                            PlayReverseHide();
                        });
        }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyShowAnimation = serializedObject.FindProperty("ShowAnimation");
            propertyHideAnimation = serializedObject.FindProperty("HideAnimation");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();
            componentHeader
                .SetComponentNameText(nameof(UIContainer))
                .SetIcon(EditorSpriteSheets.EditorUI.Icons.Animator);
            InitializeReactionControls();
            InitializeShow();
            InitializeHide();
        }

        private FluidTab GetFluidTab(string labelText) =>
            FluidTab.Get()
                .SetLabelText(labelText)
                .SetElementSize(ElementSize.Small)
                .IndicatorSetEnabledColor(accentColor)
                .ButtonSetAccentColor(selectableAccentColor)
                .AddToToggleGroup(tabsGroup);

        protected virtual void InitializeShow()
        {
            showAnimatedContainer = new FluidAnimatedContainer("Show", true).Hide(false);
            showTab =
                GetFluidTab("Show")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Show)
                    .ButtonSetOnValueChanged(evt => showAnimatedContainer.Toggle(evt.newValue, evt.animateChange));

            showAnimatedContainer.AddOnShowCallback(() =>
            {
                showAnimatedContainer
                    .AddContent(DesignUtils.NewPropertyField(propertyShowAnimation))
                    .Bind(serializedObject);
            });
        }

        protected virtual void InitializeHide()
        {
            hideAnimatedContainer = new FluidAnimatedContainer("Hide", true).Hide(false);
            hideTab =
                GetFluidTab("Hide")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Hide)
                    .ButtonSetOnValueChanged(evt => hideAnimatedContainer.Toggle(evt.newValue, evt.animateChange));

            hideAnimatedContainer.AddOnShowCallback(() =>
            {
                hideAnimatedContainer
                    .AddContent(DesignUtils.NewPropertyField(propertyHideAnimation))
                    .Bind(serializedObject);
            });
        }

        protected override VisualElement Toolbar()
        {
            return base.Toolbar()
                .AddChild(showTab)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(hideTab)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(DesignUtils.flexibleSpace);
        }

        protected override VisualElement Content()
        {
            return base.Content()
                .AddChild(showAnimatedContainer)
                .AddChild(hideAnimatedContainer);
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
                .AddChild(GetController(propertyController))
                .AddChild(DesignUtils.endOfLineBlock);
        }

        protected internal static FluidField GetController(SerializedProperty controllerProperty)
        {
            ObjectField objectField = 
                DesignUtils.NewObjectField(controllerProperty, typeof(UIContainer))
                    .SetStyleFlexGrow(1);
            
            return
                FluidField.Get()
                .SetLabelText($"Controller")
                .SetTooltip($"{nameof(UIContainer)} controller")
                .SetIcon(EditorSpriteSheets.UIManager.Icons.UIContainer)
                .SetStyleMinWidth(200)
                .AddFieldContent(objectField);
        }
    }
}
