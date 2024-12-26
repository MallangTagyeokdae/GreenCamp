// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Common.Drawers;
using Doozy.Editor.EditorUI;
using Doozy.Editor.UIManager.ScriptableObjects;
using Doozy.Editor.UIManager.Windows;
using Doozy.Runtime.UIManager;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Drawers
{
    [CustomPropertyDrawer(typeof(UIStepperId), true)]
    public class UIStepperIdDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property) =>
            CategoryNameIdUtils.CreateDrawer
            (
                property,
                () => UIStepperIdDatabase.instance.database.GetCategories(),
                targetCategory => UIStepperIdDatabase.instance.database.GetNames(targetCategory),
                EditorSpriteSheets.EditorUI.Icons.GenericDatabase,
                SteppersDatabaseWindow.Open,
                "Open Steppers Database Window",
                UIStepperIdDatabase.instance,
                EditorSelectableColors.UIManager.UIComponent
            );
    }
}
