// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Common.Drawers;
using Doozy.Editor.EditorUI;
using Doozy.Editor.Reactor.ScriptableObjects;
using Doozy.Editor.Reactor.Windows;
using Doozy.Runtime.Reactor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.Reactor.Drawers
{
    [CustomPropertyDrawer(typeof(ProgressorId), true)]
    public class ProgressorIdDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property) =>
            CategoryNameIdUtils.CreateDrawer
            (
                property,
                () => ProgressorIdDatabase.instance.database.GetCategories(),
                targetCategory => ProgressorIdDatabase.instance.database.GetNames(targetCategory),
                EditorSpriteSheets.EditorUI.Icons.GenericDatabase,
                ProgressorsDatabaseWindow.Open,
                "Open Progressors Database Window",
                ProgressorIdDatabase.instance,
                EditorSelectableColors.Reactor.Red
            );
    }
}
