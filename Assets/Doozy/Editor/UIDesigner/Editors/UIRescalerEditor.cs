// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.UIDesigner.Components;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIDesigner.Editors
{
    [CustomEditor(typeof(UIRescaler), true)]
    [CanEditMultipleObjects]
    public class UIRescalerEditor : UnityEditor.Editor
    {
        private Color accentColor => EditorColors.UIDesigner.Color;
        private EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIDesigner.Color;

        private UIRescaler castedTarget => (UIRescaler)target;
        private IEnumerable<UIRescaler> castedTargets => targets.Cast<UIRescaler>();

        private VisualElement root { get; set; }
        private FluidComponentHeader componentHeader { get; set; }
        private VisualElement toolbarContainer { get; set; }

        private FluidButton resetTargetSizeButton { get; set; }

        private FluidField referenceSizeFluidField { get; set; }
        private FluidField targetSizeFluidField { get; set; }

        private FluidToggleSwitch continuousUpdateToggleSwitch { get; set; }

        private Vector2Field referenceSizeVector2Field { get; set; }
        private Vector2Field targetSizeVector2Field { get; set; }

        private SerializedProperty propertyReferenceSize { get; set; }
        private SerializedProperty propertyTargetSize { get; set; }
        private SerializedProperty propertyContinuousUpdate { get; set; }

        private void OnDestroy()
        {
            componentHeader?.Recycle();
            resetTargetSizeButton?.Recycle();
            referenceSizeFluidField?.Recycle();
            targetSizeFluidField?.Recycle();
            continuousUpdateToggleSwitch?.Recycle();
        }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        private void FindProperties()
        {
            propertyReferenceSize = serializedObject.FindProperty("ReferenceSize");
            propertyTargetSize = serializedObject.FindProperty("TargetSize");
            propertyContinuousUpdate = serializedObject.FindProperty("ContinuousUpdate");
        }

        private void InitializeEditor()
        {
            FindProperties();
            root = DesignUtils.GetEditorRoot();
            componentHeader =
                DesignUtils.editorComponentHeader
                    .SetAccentColor(accentColor)
                    .SetIcon(EditorSpriteSheets.UIDesigner.Icons.UIRescaler)
                    .SetComponentNameText("UI Rescaler");

            toolbarContainer = DesignUtils.editorToolbarContainer;

            referenceSizeVector2Field =
                DesignUtils.NewVector2Field(propertyReferenceSize)
                    .SetTooltip
                    (
                        "The reference size of this RectTransform. " +
                        "This is the size that the RectTransform will have when the localScale is set to (1,1,1)"
                    )
                    .SetStyleFlexGrow(1);

            referenceSizeFluidField =
                FluidField.Get("Reference Size")
                    .AddFieldContent(referenceSizeVector2Field);

            targetSizeVector2Field =
                DesignUtils.NewVector2Field(propertyTargetSize)
                    .SetTooltip
                    (
                        "The target size of this RectTransform. " +
                        "This is the value that is used to determine the localScale of the RectTransform."
                    )
                    .SetStyleFlexGrow(1);

            resetTargetSizeButton =
                FluidButton.Get()
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Reset)
                    .SetElementSize(ElementSize.Small)
                    .SetButtonStyle(ButtonStyle.Clear)
                    .SetStyleMarginLeft(DesignUtils.k_Spacing)
                    .SetStyleTop(2)
                    .SetTooltip("Reset Target Size to Reference Size")
                    .SetOnClick(() =>
                    {
                        propertyTargetSize.vector2Value = propertyReferenceSize.vector2Value;
                        serializedObject.ApplyModifiedProperties();
                        serializedObject.Update();
                    });

            targetSizeFluidField =
                FluidField.Get("Target Size")
                    .AddFieldContent(targetSizeVector2Field)
                    .AddInfoElement(resetTargetSizeButton);

            targetSizeFluidField.infoContainer.SetStyleJustifyContent(Justify.FlexEnd);

            continuousUpdateToggleSwitch =
                FluidToggleSwitch.Get()
                    .SetToggleAccentColor(selectableAccentColor)
                    .SetLabelText("Continuous Update")
                    .SetTooltip("If TRUE, the RectTransform localScale will be updated every frame in the LateUpdate method")
                    .BindToProperty(propertyContinuousUpdate);
        }

        private VisualElement Toolbar()
        {
            return
                toolbarContainer
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(DesignUtils.flexibleSpace)
                    .AddChild
                    (
                        DesignUtils.SystemButton_SortComponents
                        (
                            castedTarget.gameObject,
                            nameof(RectTransform),
                            nameof(UIRescaler)
                        )
                    );
        }

        private void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild(Toolbar())
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(targetSizeFluidField)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(referenceSizeFluidField)
                )
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(continuousUpdateToggleSwitch)
                .AddChild(DesignUtils.endOfLineBlock);
        }
    }
}
