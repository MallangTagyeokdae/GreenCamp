// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.Common.Extensions;
using Doozy.Editor.Common.ScriptableObjects;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Reactor.Reflection;
using Doozy.Runtime.Reactor.Reflection.Enums;
using Doozy.Runtime.Reactor.Reflection.Internal;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Doozy.Editor.Reactor.Reflection.Drawers.Internal
{
    public abstract class ReflectedValueDrawer<T> : PropertyDrawer where T : ReflectedValue
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {}

        private static IEnumerable<Texture2D> linkIconTextures => EditorSpriteSheets.EditorUI.Icons.Link;
        
         public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var target = property.GetTargetObjectOfProperty() as T;
            var drawer = new VisualElement();

            var propertyTarget = property.FindPropertyRelative("Target");
            var propertyPropertyName = property.FindPropertyRelative("PropertyName");
            var propertyFieldName = property.FindPropertyRelative("FieldName");
            var propertyTargetValueType = property.FindPropertyRelative("ValueDetails");

            ObjectField targetObjectField = DesignUtils.NewObjectField(propertyTarget, typeof(Object)).SetStyleFlexGrow(1).SetStyleMaxHeight(20);
            TextField propertyNameTextField = DesignUtils.NewTextField(propertyPropertyName).SetStyleFlexGrow(1).SetStyleMarginTop(DesignUtils.k_Spacing);
            TextField fieldNameTextField = DesignUtils.NewTextField(propertyFieldName).SetStyleFlexGrow(1).SetStyleMarginTop(DesignUtils.k_Spacing);
            EnumField invisibleTargetValueTypeEnumField = DesignUtils.NewEnumField(propertyTargetValueType, true);

            FluidButton linkButton =
                FluidButton.Get()
                    .SetLabelText("Link")
                    .SetIcon(linkIconTextures)
                    .SetElementSize(ElementSize.Normal)
                    .SetButtonStyle(ButtonStyle.Contained)
                    .SetStyleMarginLeft(DesignUtils.k_Spacing2X)
                    .SetStyleAlignSelf(Align.Center)
                    .SetOnClick(() =>
                    {
                        var searchWindowContext = new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition));
                        DynamicSearchProvider dsp =
                            ScriptableObject.CreateInstance<DynamicSearchProvider>()
                                .AddItems(target.GetSearchMenuItems());
                        SearchWindow.Open(searchWindowContext, dsp);
                    });

            FluidField targetFluidField =
                FluidField.Get()
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddChild
                            (
                                DesignUtils.column
                                    .SetStyleJustifyContent(Justify.Center)
                                    .AddChild(targetObjectField)
                                    .AddChild(propertyNameTextField)
                                    .AddChild(fieldNameTextField)
                            )
                            .AddChild(linkButton)
                    );

            propertyNameTextField.SetEnabled(false);
            fieldNameTextField.SetEnabled(false);

            invisibleTargetValueTypeEnumField.RegisterValueChangedCallback(evt =>
            {
                if (evt?.newValue == null) return;
                UpdateDrawer((ValueDetails)evt.newValue);
            });

            targetObjectField.RegisterValueChangedCallback(evt =>
            {
                if (evt == null) return;
                linkButton.SetEnabled(evt.newValue != null);

                if (evt.newValue == null)
                {
                    invisibleTargetValueTypeEnumField.value = ValueDetails.None;
                    return;
                }

                if (evt.newValue != evt.previousValue)
                {
                    targetFluidField.schedule.Execute(() =>
                    {
                        if (target != null && !target.IsValid())
                            invisibleTargetValueTypeEnumField.value = ValueDetails.None;
                    });
                }
            });

            UpdateDrawer((ValueDetails)propertyTargetValueType.enumValueIndex);

            return drawer
                    .AddChild(targetFluidField)
                    .AddChild(invisibleTargetValueTypeEnumField)
                ;

            void UpdateDrawer(ValueDetails value)
            {
                linkButton.SetEnabled(propertyTarget.objectReferenceValue != null);
                propertyNameTextField.SetStyleDisplay(value == ValueDetails.IsProperty ? DisplayStyle.Flex : DisplayStyle.None);
                fieldNameTextField.SetStyleDisplay(value == ValueDetails.IsField ? DisplayStyle.Flex : DisplayStyle.None);
            }
        }
    }
}
