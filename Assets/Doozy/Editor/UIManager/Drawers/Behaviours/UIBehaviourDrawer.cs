// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Mody.Drawers;
using Doozy.Editor.Reactor.Internal;
using Doozy.Runtime.Mody;
using Doozy.Runtime.Reactor.Extensions;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Drawers
{
    [CustomPropertyDrawer(typeof(UIBehaviour), true)]
    public class UIBehaviourDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var behaviourNameProperty = property.FindPropertyRelative("BehaviourName");
            var receiverProperty = property.FindPropertyRelative("Receiver");
            var cooldownProperty = property.FindPropertyRelative("Cooldown");
            
            //ToDo add cooldown to the editor

            SerializedProperty eventNameProperty = property.FindPropertyRelative(nameof(ModyEvent.EventName));
            SerializedProperty runnersProperty = property.FindPropertyRelative(nameof(ModyEvent.Runners));
            SerializedProperty eventProperty = property.FindPropertyRelative(nameof(ModyEvent.Event));

            var behaviourName = (UIBehaviour.Name)behaviourNameProperty.enumValueIndex;

            Image behaviourIcon =
                new Image()
                    .SetStyleSize(26)
                    .SetStyleBackgroundImageTintColor(DesignUtils.fieldIconColor);

            var iconReaction = behaviourIcon
                .GetTexture2DReaction()
                .SetEditorHeartbeat()
                .SetTextures(GetBehaviourTextures(behaviourName));

            var drawer = DesignUtils.row;
            var foldout = new FluidFoldout()
                .SetStyleFlexGrow(1)
                .SetElementSize(ElementSize.Normal)
                .SetLabelText(ObjectNames.NicifyVariableName(behaviourName.ToString()));

            foldout.animatedContainer.SetClearOnHide(true);

            foldout.animatedContainer.SetOnShowCallback(() =>
            {
                foldout
                    .AddContent(DesignUtils.UnityEventField(eventNameProperty.stringValue, eventProperty))
                    .AddContent(DesignUtils.spaceBlock2X)
                    .AddContent(ModyEventDrawer.ActionRunnersListView(runnersProperty))
                    .Bind(property.serializedObject);
            });

            drawer.RegisterCallback<PointerEnterEvent>(evt =>
            {
                iconReaction?.Play();
            });

            drawer.RegisterCallback<DetachFromPanelEvent>(evt =>
            {
                iconReaction?.Recycle();
                foldout.Dispose();
            });

            drawer
                .AddChild(behaviourIcon)
                .AddChild(foldout);

            return drawer;
        }

        public static IEnumerable<Texture2D> GetBehaviourTextures(UIBehaviour.Name behaviour)
        {
            switch (behaviour)
            {
                case UIBehaviour.Name.PointerEnter: return EditorSpriteSheets.EditorUI.Icons.PointerEnter;
                case UIBehaviour.Name.PointerExit: return EditorSpriteSheets.EditorUI.Icons.PointerExit;
                case UIBehaviour.Name.PointerDown: return EditorSpriteSheets.EditorUI.Icons.PointerDown;
                case UIBehaviour.Name.PointerUp: return EditorSpriteSheets.EditorUI.Icons.PointerUp;
                case UIBehaviour.Name.PointerClick: return EditorSpriteSheets.EditorUI.Icons.ButtonClick;
                case UIBehaviour.Name.PointerDoubleClick: return EditorSpriteSheets.EditorUI.Icons.ButtonDoubleClick;
                case UIBehaviour.Name.PointerLongClick: return EditorSpriteSheets.EditorUI.Icons.ButtonLongClick;
                case UIBehaviour.Name.PointerLeftClick: return EditorSpriteSheets.EditorUI.Icons.ButtonLeftClick;
                case UIBehaviour.Name.PointerMiddleClick: return EditorSpriteSheets.EditorUI.Icons.ButtonMiddleClick;
                case UIBehaviour.Name.PointerRightClick: return EditorSpriteSheets.EditorUI.Icons.ButtonRightClick;
                case UIBehaviour.Name.Selected: return EditorSpriteSheets.EditorUI.Icons.Selected;
                case UIBehaviour.Name.Deselected: return EditorSpriteSheets.EditorUI.Icons.Deselected;
                case UIBehaviour.Name.Submit: return EditorSpriteSheets.EditorUI.Icons.Border;
                default: return EditorSpriteSheets.EditorUI.Icons.QuestionMark;
            }
        }
    }
}
