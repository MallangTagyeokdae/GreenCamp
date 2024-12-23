// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Mody.Drawers;
using Doozy.Runtime.Mody;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Drawers
{
    [CustomPropertyDrawer(typeof(UISelectableState), true)]
    public class UISelectableStateDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty stateTypeProperty = property.FindPropertyRelative("StateType");
            SerializedProperty stateEventProperty = property.FindPropertyRelative("StateEvent");
            var state = (UISelectionState)stateTypeProperty.enumValueIndex;
            stateEventProperty.FindPropertyRelative("EventName").stringValue = $"{state} State";
            return new VisualElement()
                .SetName($"UISelectableState: {state}")
                .SetStyleFlexGrow(1)
                .AddChild
                (
                    FluidField.Get()
                        .AddFieldContent(DesignUtils.NewPropertyField(stateEventProperty))
                );
            ;
        }
    }
}
