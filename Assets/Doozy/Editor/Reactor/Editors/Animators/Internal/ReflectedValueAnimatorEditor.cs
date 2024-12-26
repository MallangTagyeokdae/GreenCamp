// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Reactor.Animators;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Doozy.Editor.Reactor.Editors.Animators.Internal
{
    [CanEditMultipleObjects]
    public abstract class ReflectedValueAnimatorEditor : BaseReactorAnimatorEditor
    {
        private FluidField valueTargetFluidField { get; set; }
        private PropertyField valueTargetPropertyField { get; set; }
        private SerializedProperty propertyValueTarget { get; set; }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            valueTargetFluidField?.Recycle();
        }

        protected override void FindProperties()
        {
            base.FindProperties();
            propertyValueTarget = serializedObject.FindProperty(nameof(FloatAnimator.ValueTarget));
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            valueTargetPropertyField = 
                DesignUtils.NewPropertyField(propertyValueTarget);
            
            valueTargetFluidField =
                FluidField.Get()
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Atom)
                    .SetLabelText("Value Target")
                    .AddFieldContent(valueTargetPropertyField);
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
                .AddChild(DesignUtils.endOfLineBlock);
        }
    }
}
