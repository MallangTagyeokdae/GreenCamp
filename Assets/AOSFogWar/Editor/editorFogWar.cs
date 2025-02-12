/*
 * Created :    Winter 2022
 * Author :     SeungGeon Kim (keithrek@hanmail.net)
 * Project :    FogWar
 * Filename :   editorFogWar.cs (custom editor module)
 * 
 * All Content (C) 2022 Unlimited Fischl Works, all rights reserved.
 */



using UnityEngine;  // GUILayout
using UnityEditor;  // Editor



namespace FischlWorks_FogWar
{



    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    // This attribute is intended to be used with a property field to show / hide it following a bool variable
    public class ShowIfAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ShowIfAttribute attributeHandle = (ShowIfAttribute)attribute;

            SerializedProperty baseProperty = property.serializedObject.FindProperty(attributeHandle._BaseCondition);

            if (baseProperty != null)
            {
                GUI.enabled = baseProperty.boolValue;

                EditorGUI.PropertyField(position, property, label, true);
            }
            else
            {
                Debug.LogError("Designated property was not found : " + attributeHandle._BaseCondition);
            }
        }
    }



    [CustomPropertyDrawer(typeof(BigHeaderAttribute))]
    // DecoratorDrawer must be inherited instead of PropertyDrawer in order not to affect any property field beneath
    public class BigHeaderAttributeDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            BigHeaderAttribute attributeHandle = (BigHeaderAttribute)attribute;

            position.yMin += EditorGUIUtility.singleLineHeight * 0.5f;

            // This line of code was fetched from the internal unity header attribute implementation
            position = EditorGUI.IndentedRect(position);

            GUIStyle headerTextStyle = new GUIStyle()
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };

            headerTextStyle.normal.textColor = new Color32(255, 200, 55, 255);

            GUI.Label(position, attributeHandle._Text, headerTextStyle);

            EditorGUI.DrawRect(new Rect(position.xMin, position.yMin, position.width, 1), new Color32(255, 155, 55, 255));
        }

        public override float GetHeight()
        {
            return EditorGUIUtility.singleLineHeight * 2;
        }
    }



}