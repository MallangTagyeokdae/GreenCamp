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
using Doozy.Runtime.UIManager.Components;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.Editors.Animators.Internal
{
    public abstract class BaseUIToggleAnimatorEditor : BaseTargetComponentAnimatorEditor
    {
        protected ToggleReactionControls reactionControls { get; set; }

        protected BaseUIToggleAnimator castedAnimator => (BaseUIToggleAnimator)target;
        protected List<BaseUIToggleAnimator> castedAnimators => targets.Cast<BaseUIToggleAnimator>().ToList();
        
        protected FluidTab onTab { get; set; }
        protected FluidTab offTab { get; set; }
        
        protected FluidAnimatedContainer onAnimatedContainer { get; set; }
        protected FluidAnimatedContainer offAnimatedContainer { get; set; }
        
        protected SerializedProperty propertyOnAnimation { get; set; }
        protected SerializedProperty propertyOffAnimation { get; set; }
        
        protected bool resetToStartValue { get; set; }

        protected override void OnEnable()
        {
            if(Application.isPlaying) return;
            resetToStartValue = false;
            ResetAnimatorInitializedState();
        }

        protected override void OnDisable()
        {
            if(Application.isPlaying) return;
            if(resetToStartValue) ResetToStartValues();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            reactionControls?.Dispose();
            
            onTab?.Recycle();
            offTab?.Recycle();
            
            onAnimatedContainer?.Dispose();
            offAnimatedContainer?.Dispose();
        }
        
        protected abstract void ResetAnimatorInitializedState();
        protected abstract void ResetToStartValues();
        protected abstract void PlayIsOn();
        protected abstract void PlayIsOff();
        protected abstract void HeartbeatCheck();

        protected virtual void InitializeReactionControls()
        {
            reactionControls =
                new ToggleReactionControls()
                    .AddToggleControls
                    (
                        resetCallback: () =>
                        {
                            resetToStartValue = true;
                            HeartbeatCheck();
                            ResetToStartValues();
                        },
                        onCallback: () =>
                        {
                            if (Application.isPlaying)
                            {
                                castedAnimators.ForEach(a => a.PlayOnAnimation());
                                return;
                            }
                            resetToStartValue = true;
                            HeartbeatCheck();
                            PlayIsOn();
                        },
                        offCallback: () =>
                        {
                            if (Application.isPlaying)
                            {
                                castedAnimators.ForEach(a => a.PlayOffAnimation());
                                return;
                            }
                            resetToStartValue = true;
                            HeartbeatCheck();
                            PlayIsOff();
                        }
                    );
        }

        protected override void FindProperties()
        {
            base.FindProperties();
            
            propertyOnAnimation = serializedObject.FindProperty("OnAnimation");
            propertyOffAnimation = serializedObject.FindProperty("OffAnimation");
        }
        
        protected override void InitializeEditor()
        {
            base.InitializeEditor();
            componentHeader
                .SetComponentNameText(nameof(UIToggle))
                .SetIcon(EditorSpriteSheets.EditorUI.Icons.Animator);
            InitializeReactionControls();
            InitializeOn();
            InitializeOff();
        }
        
        private FluidTab GetFluidTab(string labelText) =>
            FluidTab.Get()
                .SetLabelText(labelText)
                .SetElementSize(ElementSize.Small)
                .IndicatorSetEnabledColor(accentColor)
                .ButtonSetAccentColor(selectableAccentColor)
                .AddToToggleGroup(tabsGroup);

        protected virtual void InitializeOn()
        {
            onAnimatedContainer = new FluidAnimatedContainer("On", true).Hide(false);
            onTab =
                GetFluidTab("On")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.ToggleON)
                    .ButtonSetOnValueChanged(evt => onAnimatedContainer.Toggle(evt.newValue, evt.animateChange));

            onAnimatedContainer.AddOnShowCallback(() =>
            {
                onAnimatedContainer
                    .AddContent(DesignUtils.NewPropertyField(propertyOnAnimation))
                    .Bind(serializedObject);
            });
        }
        
        protected virtual void InitializeOff()
        {
            offAnimatedContainer = new FluidAnimatedContainer("Off", true).Hide(false);
            offTab =
                GetFluidTab("Off")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.ToggleOFF)
                    .ButtonSetOnValueChanged(evt => offAnimatedContainer.Toggle(evt.newValue, evt.animateChange));

            offAnimatedContainer.AddOnShowCallback(() =>
            {
                offAnimatedContainer
                    .AddContent(DesignUtils.NewPropertyField(propertyOffAnimation))
                    .Bind(serializedObject);
            });
        }

        protected override VisualElement Toolbar()
        {
            return base.Toolbar()
                .AddChild(onTab)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(offTab)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(DesignUtils.flexibleSpace);
        }

        protected override VisualElement Content()
        {
            return base.Content()
                .AddChild(onAnimatedContainer)
                .AddChild(offAnimatedContainer);
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
                DesignUtils.NewObjectField(controllerProperty, typeof(UIToggle))
                    .SetStyleFlexGrow(1);

            return
                FluidField.Get()
                    .SetLabelText($"Controller")
                    .SetTooltip($"{nameof(UIToggle)} controller")
                    .SetIcon(EditorSpriteSheets.UIManager.Icons.UIToggle)
                    .SetStyleMinWidth(200)
                    .AddFieldContent(objectField);
        }
    }
}
