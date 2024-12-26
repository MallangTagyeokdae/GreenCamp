// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Ticker;
using Doozy.Editor.UIManager.Editors.Animators.Internal;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager;
using Doozy.Runtime.UIManager.Animators;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace Doozy.Editor.UIManager.Editors.Animators
{
    [CustomEditor(typeof(UISelectableVector3Animator), true)]
    [CanEditMultipleObjects]
    public class UISelectableVector3AnimatorEditor : BaseUISelectableAnimatorEditor
    {
        public UISelectableVector3Animator castedTarget => (UISelectableVector3Animator)target;
        public List<UISelectableVector3Animator> castedTargets => targets.Cast<UISelectableVector3Animator>().ToList();

        private FluidField valueTargetFluidField { get; set; }
        private SerializedProperty propertyValueTarget { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            valueTargetFluidField?.Recycle();
        }

        protected override void ResetAnimatorInitializedState()
        {
            foreach (var a in castedTargets)
                a.animatorInitialized = false;
        }

        protected override void ResetToStartValues()
        {
            foreach (var a in castedTargets)
            {
                a.StopAllReactions();
                a.ResetToStartValues();
            }
        }
        
        protected override void PlayNormal()
        {
            foreach (var a in castedTargets)
                a.GetAnimation(UISelectionState.Normal).Play();
        }

        protected override void PlayHighlighted()
        {
            foreach (var a in castedTargets)
                a.GetAnimation(UISelectionState.Highlighted).Play();
        }

        protected override void PlayPressed()
        {
            foreach (var a in castedTargets)
                a.GetAnimation(UISelectionState.Pressed).Play();
        }

        protected override void PlaySelected()
        {
            foreach (var a in castedTargets)
                a.GetAnimation(UISelectionState.Selected).Play();
        }

        protected override void PlayDisabled()
        {
            foreach (var a in castedTargets)
                a.GetAnimation(UISelectionState.Disabled).Play();
        }

        protected override void HeartbeatCheck()
        {
            if (Application.isPlaying) return;
            foreach (var a in castedTargets)
                if (a.animatorInitialized == false)
                {
                    a.InitializeAnimator();
                    foreach (EditorHeartbeat eh in a.SetHeartbeat<EditorHeartbeat>().Cast<EditorHeartbeat>())
                        eh.StartSceneViewRefresh(a);
                }
        }
        
        protected override void FindProperties()
        {
            base.FindProperties();
            propertyValueTarget = serializedObject.FindProperty(nameof(UISelectableVector3Animator.ValueTarget));
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentTypeText("Vector3 Animator")
                .SetIcon(EditorSpriteSheets.Reactor.Icons.Vector3Animator)
                .AddManualButton()
                .AddApiButton()
                .AddYouTubeButton();

            valueTargetFluidField =
                FluidField.Get<PropertyField>(propertyValueTarget)
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Atom)
                    .SetLabelText("Value Target");

            normalAnimatedContainer.AddOnShowCallback(() => normalAnimatedContainer.Bind(serializedObject));
            highlightedAnimatedContainer.AddOnShowCallback(() => highlightedAnimatedContainer.Bind(serializedObject));
            pressedAnimatedContainer.AddOnShowCallback(() => pressedAnimatedContainer.Bind(serializedObject));
            selectedAnimatedContainer.AddOnShowCallback(() => selectedAnimatedContainer.Bind(serializedObject));
            disabledAnimatedContainer.AddOnShowCallback(() => disabledAnimatedContainer.Bind(serializedObject));
        }

        protected override void Compose()
        {
            root
                .AddChild(reactionControls)
                .AddChild(componentHeader)
                .AddChild(Toolbar())
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(Content())
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(valueTargetFluidField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(GetController(propertyController, propertyToggleCommand))
                .AddChild(DesignUtils.endOfLineBlock);
        }
    }
}
