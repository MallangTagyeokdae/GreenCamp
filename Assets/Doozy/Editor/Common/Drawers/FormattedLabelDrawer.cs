// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Common;
using Doozy.Runtime.UIElements.Extensions;
using TMPro;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.Common.Drawers
{
    [CustomPropertyDrawer(typeof(FormattedLabel), true)]
    public class FormattedLabelDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            ObjectField labelObjectField =
                DesignUtils.NewObjectField(property.FindPropertyRelative("Label"), typeof(TMP_Text));

            TextField formatTextField =
                DesignUtils.NewTextField(property.FindPropertyRelative("Format"))
                    .SetStyleMinWidth(200);

            VisualElement drawer =
                new VisualElement()
                    .SetStyleFlexDirection(FlexDirection.Row)
                    .SetStylePaddingLeft(DesignUtils.k_Spacing)
                    .SetStylePaddingRight(DesignUtils.k_Spacing)
                    .AddChild(labelObjectField)
                    .AddSpace(DesignUtils.k_Spacing, 0)
                    .AddChild(formatTextField);

            return drawer;
        }
    }
}
