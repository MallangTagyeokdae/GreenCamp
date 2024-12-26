// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Colors;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Doozy.Editor.EditorUI.Utils
{
    public static class DesignUtils
    {
        private static Font s_defaultFont;
        public static Font unityDefaultFont => s_defaultFont ? s_defaultFont : s_defaultFont = new VisualElement().GetStyleUnityFont();

        public const int k_FieldLabelFontSize = 9;
        public const int k_NormalLabelFontSize = 11;
        public const int k_Spacing = 4;
        public const int k_Spacing2X = k_Spacing * 2;
        public const int k_Spacing3X = k_Spacing * 3;
        public const int k_Spacing4X = k_Spacing * 4;
        public const int k_EndOfLineSpacing = k_Spacing * 6;
        public const int k_ToolbarHeight = 32;
        public const int k_RootPadding = 6;

        public static VisualElement endOfLineBlock => GetSpaceBlock(0, k_EndOfLineSpacing, "End of Line");
        public static VisualElement spaceBlock => GetSpaceBlock(k_Spacing, k_Spacing);
        public static VisualElement spaceBlock2X => GetSpaceBlock(k_Spacing2X, k_Spacing2X);
        public static VisualElement spaceBlock3X => GetSpaceBlock(k_Spacing3X, k_Spacing3X);
        public static VisualElement spaceBlock4X => GetSpaceBlock(k_Spacing4X, k_Spacing4X);

        public const int k_FieldBorderRadius = 6;

        public const int k_FieldNameTiny = 10;
        public const int k_FieldNameSmall = 10;
        public const int k_FieldNameNormal = 11;
        public const int k_FieldNameLarge = 11;

        public static Color placeholderColor => EditorColors.Default.Placeholder;

        public static Color tabButtonColorOff => EditorColors.Default.Background;
        public static Color fieldBackgroundColor => EditorColors.Default.FieldBackground;
        public static Color fieldIconColor => EditorColors.Default.FieldIcon;
        public static Color fieldNameTextColor => EditorColors.Default.TextSubtitle;
        public static Font fieldNameTextFont => EditorFonts.Ubuntu.Light;

        public static Color dividerColor => EditorColors.Default.UnityThemeInversed.WithAlpha(0.1f);

        public static Color disabledTextColor => EditorColors.Default.Placeholder;

        public static Color callbacksColor => EditorColors.Default.Action;
        public static EditorSelectableColorInfo callbackSelectableColor => EditorSelectableColors.Default.Action;

        private static VisualElement divider =>
            new VisualElement()
                .SetName("Divider")
                .SetStyleFlexGrow(1)
                .SetStyleFlexShrink(0)
                .SetStyleAlignSelf(Align.Stretch)
                .SetStyleMargins(k_Spacing)
                .SetStyleBackgroundColor(dividerColor);

        public static VisualElement dividerHorizontal =>
            divider.SetStyleHeight(1, 1, 1);

        public static VisualElement dividerVertical =>
            divider.SetStyleWidth(1, 1, 1);

        /// <summary> Get a new VisualElement as a column </summary>
        public static VisualElement column =>
            new VisualElement().SetName("Column").SetStyleFlexDirection(FlexDirection.Column).SetStyleFlexGrow(1);

        /// <summary> Get a new VisualElement as a row </summary>
        public static VisualElement row =>
            new VisualElement().SetName("Row").SetStyleFlexDirection(FlexDirection.Row).SetStyleFlexGrow(1);

        /// <summary> Get a new VisualElement as empty flexible space </summary>
        public static VisualElement flexibleSpace =>
            new VisualElement()
                .SetName("Flexible Space")
                .SetStyleFlexGrow(1);

        public static VisualElement fieldContainer =>
            new VisualElement()
                .SetStylePadding(k_Spacing)
                .SetStyleBackgroundColor(fieldBackgroundColor)
                .SetStyleBorderRadius(k_FieldBorderRadius);

        public static Label fieldLabel =>
            new Label()
                .ResetLayout()
                .SetStyleUnityFont(fieldNameTextFont)
                .SetStyleFontSize(k_FieldLabelFontSize)
                .SetStyleColor(fieldNameTextColor);

        public static VisualElement GetEditorRoot() =>
            new VisualElement()
                .SetStylePaddingRight(k_RootPadding);

        public static VisualElement editorRoot
            => GetEditorRoot();

        public static FluidComponentHeader editorComponentHeader =>
            FluidComponentHeader.Get()
                .SetElementSize(ElementSize.Large);

        public static VisualElement editorToolbarContainer =>
            new VisualElement()
                .SetName("Toolbar Container")
                .SetStyleFlexDirection(FlexDirection.Row)
                .SetStyleMarginTop(-4)
                .SetStyleMarginLeft(47)
                .SetStyleMarginRight(4)
                .SetStyleFlexGrow(1);

        public static VisualElement editorContentContainer =>
            new VisualElement()
                .SetName("Content Container")
                .SetStyleFlexGrow(1)
                .SetStyleMarginLeft(43);

        public static VisualElement FullScreenVisualElement() =>
            new VisualElement()
                .SetStylePosition(Position.Absolute)
                .SetStyleLeft(0)
                .SetStyleTop(0)
                .SetStyleRight(0)
                .SetStyleBottom(0)
                .SetPickingMode(PickingMode.Ignore)
                .SetStyleJustifyContent(Justify.Center)
                .SetStyleAlignItems(Align.Center);

        public static VisualElement GetToolbarContainer() =>
            row
                .SetStyleHeight(k_ToolbarHeight)
                .SetStylePaddingLeft(k_Spacing)
                .SetStylePaddingRight(k_Spacing)
                .SetStyleAlignItems(Align.Center)
                .SetStyleJustifyContent(Justify.FlexEnd)
                .SetStyleBackgroundColor(EditorColors.Default.BoxBackground);

        public static VisualElement GetSpaceBlock(int size, string name = "") =>
            GetSpaceBlock(size, size, name);

        public static VisualElement GetSpaceBlock(int width, int height, string name = "") =>
            new VisualElement()
                .SetName($"{name} Space Block ({width}x{height})")
                .SetStyleWidth(width)
                .SetStyleHeight(height)
                .SetStyleAlignSelf(Align.Center)
                .SetStyleFlexShrink(0);

        public static VisualElement UnityEventField(string labelText, SerializedProperty property) =>
            new VisualElement()
                .SetName($"UnityEvent: {labelText}")
                .AddChild(UnityEventLabel(labelText))
                .AddChild(NewPropertyField(property));

        public static Label NewFieldNameLabel(string text) =>
            fieldLabel
                .SetName($"Label: {text}")
                .SetText(text);

        public static Label UnityEventLabel(string labelText) =>
            NewLabel(labelText, 10)
                .SetStyleBackgroundColor(EditorColors.Default.BoxBackground)
                .SetStyleBorderRadius(4, 4, 0, 0)
                .SetStylePadding(8, 4, 8, 6)
                .SetStyleMarginBottom(-2);

        public static Label NewLabel(string text = "", int labelFontSize = k_NormalLabelFontSize) =>
            new Label(text)
                .SetName($"Label: {text}")
                .ResetLayout()
                .SetStyleTextAlign(TextAnchor.MiddleLeft)
                .SetStyleFontSize(labelFontSize);

        public static EnumField NewEnumField(SerializedProperty property, bool invisibleField = false) =>
            NewEnumField(property.propertyPath, invisibleField);

        public static EnumField NewEnumField(string bindingPath, bool invisibleField = false) =>
            new EnumField().SetBindingPath(bindingPath)
                .SetName($"Enum: {bindingPath}")
                .ResetLayout()
                .SetStyleFlexShrink(1)
                .SetStyleAlignSelf(Align.Center)
                .SetStyleDisplay(invisibleField ? DisplayStyle.None : DisplayStyle.Flex);

        public static PropertyField NewPropertyField(SerializedProperty property, bool invisibleField = false) =>
            NewPropertyField(property.propertyPath, invisibleField);

        public static PropertyField NewPropertyField(string bindingPath, bool invisibleField = false) =>
            new PropertyField().SetBindingPath(bindingPath)
                .SetName($"Property: {bindingPath}")
                .ResetLayout()
                .SetStyleFlexGrow(1)
                .SetStyleDisplay(invisibleField ? DisplayStyle.None : DisplayStyle.Flex);

        public static ObjectField NewObjectField(SerializedProperty property, Type objectType, bool allowSceneObjects = true, bool invisibleField = false) =>
            NewObjectField(property.propertyPath, objectType, allowSceneObjects, invisibleField);

        public static ObjectField NewObjectField(string bindingPath, Type objectType, bool allowSceneObjects = true, bool invisibleField = false) =>
            new ObjectField
                {
                    bindingPath = bindingPath,
                    objectType = objectType,
                    allowSceneObjects = allowSceneObjects
                }
                .SetName($"Object: {bindingPath}")
                .ResetLayout()
                .SetStyleFlexShrink(1)
                .SetStyleAlignSelf(Align.Center)
                .SetStyleDisplay(invisibleField ? DisplayStyle.None : DisplayStyle.Flex);

        public static IntegerField NewIntegerField(SerializedProperty property, bool invisibleField = false) =>
            NewIntegerField(property.propertyPath, invisibleField);

        public static IntegerField NewIntegerField(string bindingPath, bool invisibleField = false) =>
            new IntegerField().SetBindingPath(bindingPath)
                .SetName($"Int: {bindingPath}")
                .ResetLayout()
                .SetStyleFlexShrink(1)
                .SetStyleAlignSelf(Align.Center)
                .SetStyleDisplay(invisibleField ? DisplayStyle.None : DisplayStyle.Flex);

        public static FloatField NewFloatField(SerializedProperty property, bool invisibleField = false) =>
            NewFloatField(property.propertyPath, invisibleField);

        public static FloatField NewFloatField(string bindingPath, bool invisibleField = false) =>
            new FloatField().SetBindingPath(bindingPath)
                .SetName($"Int: {bindingPath}")
                .ResetLayout()
                .SetStyleFlexShrink(1)
                .SetStyleAlignSelf(Align.Center)
                .SetStyleDisplay(invisibleField ? DisplayStyle.None : DisplayStyle.Flex);

        public static Vector2Field NewVector2Field(SerializedProperty property, bool invisibleField = false) =>
            NewVector2Field(property.propertyPath, invisibleField);

        public static Vector2Field NewVector2Field(string bindingPath, bool invisibleField = false) =>
            new Vector2Field().SetBindingPath(bindingPath)
                .SetName($"Float: {bindingPath}")
                .ResetLayout()
                .SetStyleFlexShrink(1)
                .SetStyleAlignSelf(Align.Center)
                .SetStyleDisplay(invisibleField ? DisplayStyle.None : DisplayStyle.Flex);

        public static Vector3Field NewVector3Field(SerializedProperty property, bool invisibleField = false) =>
            NewVector3Field(property.propertyPath, invisibleField);

        public static Vector3Field NewVector3Field(string bindingPath, bool invisibleField = false) =>
            new Vector3Field().SetBindingPath(bindingPath)
                .SetName($"Float: {bindingPath}")
                .ResetLayout()
                .SetStyleFlexShrink(1)
                .SetStyleAlignSelf(Align.Center)
                .SetStyleDisplay(invisibleField ? DisplayStyle.None : DisplayStyle.Flex);

        public static TextField NewTextField(SerializedProperty property, bool isDelayed = false, bool invisibleField = false) =>
            NewTextField(property.propertyPath, isDelayed, invisibleField);

        public static TextField NewTextField(string bindingPath, bool isDelayed = false, bool invisibleField = false)
        {
            TextField field =
                new TextField().SetBindingPath(bindingPath)
                    .SetName($"Text: {bindingPath}")
                    .ResetLayout()
                    .SetStyleFlexShrink(1)
                    .SetStyleAlignSelf(Align.Center)
                    .SetStyleDisplay(invisibleField ? DisplayStyle.None : DisplayStyle.Flex);

            field.isDelayed = isDelayed;

            return field;
        }

        public static Slider NewSlider(SerializedProperty property, float start, float end) =>
            NewSlider(property.propertyPath, start, end);

        public static Slider NewSlider(string bindingPath, float start, float end) =>
            new Slider(start, end)
                .SetBindingPath(bindingPath)
                .ResetLayout()
                .SetStyleFlexGrow(1);

        public static Toggle NewToggle(SerializedProperty property, bool invisibleField = false) =>
            NewToggle(property.propertyPath, invisibleField);

        public static Toggle NewToggle(string bindingPath, bool invisibleField = false) =>
            new Toggle().SetBindingPath(bindingPath)
                .SetName($"Toggle: {bindingPath}")
                .ResetLayout()
                .SetStyleAlignSelf(Align.Center)
                .SetStyleDisplay(invisibleField ? DisplayStyle.None : DisplayStyle.Flex);

        public static FluidToggleButtonTab NameTab() =>
            FluidToggleButtonTab.Get()
                .SetTabPosition(TabPosition.TabOnBottom)
                .SetElementSize(ElementSize.Small)
                .SetIcon(EditorSpriteSheets.EditorUI.Icons.Label)
                .SetLabelText("Name")
                .SetContainerColorOff(tabButtonColorOff);

        /// <summary> Fluid button used under headers for in-editor actions </summary>
        public static FluidButton SystemButton(IEnumerable<Texture2D> textures) =>
            FluidButton
                .Get()
                .SetIcon(textures)
                .SetStyleAlignSelf(Align.FlexStart)
                .SetElementSize(ElementSize.Tiny);

        /// <summary>
        /// Get a switch that connects to a given property and applies SetEnabled on a target container, depending on its value
        /// </summary>
        /// <param name="property"> Target bool property </param>
        /// <param name="content"> Content that gets SetEnabled true or false, depending on the switch value changes </param>
        /// <param name="sColor"> Selectable color </param>
        /// <param name="labelPrefix"> Prefix added to the label '{labelPrefix} Enabled' </param>
        public static FluidToggleSwitch GetEnableDisableSwitch(SerializedProperty property, VisualElement content, EditorSelectableColorInfo sColor, string labelPrefix = "")
        {
            FluidToggleSwitch fluidSwitch =
                FluidToggleSwitch.Get()
                    .SetToggleAccentColor(sColor)
                    .BindToProperty(property.propertyPath);

            fluidSwitch.SetOnValueChanged(evt => Update(evt.newValue));

            Update(property.boolValue);

            void Update(bool enabled)
            {
                fluidSwitch.SetLabelText($"{labelPrefix}{(labelPrefix.IsNullOrEmpty() ? "" : " ")}{(enabled ? "Enabled" : "Disabled")}");
                content.SetEnabled(enabled);
            }

            return fluidSwitch;
        }

        /// <summary>
        /// Sorts the component order in the Inspector
        /// <para/> Fluid button used under headers for in-editor actions
        /// </summary>
        public static FluidButton SystemButton_SortComponents(GameObject targetGameObject, params string[] customSortedComponentNames) =>
            SystemButton(EditorSpriteSheets.EditorUI.Icons.SortAz)
                .SetTooltip("Sort the components, on this gameObject, in a custom alphabetical order")
                .SetOnClick(() => EditorUtils.SortComponents(targetGameObject, customSortedComponentNames));

        /// <summary>
        /// Rename the target gameObject to the given new name (has Undo)
        /// <para/> Fluid button used under headers for in-editor actions
        /// </summary>
        public static FluidButton SystemButton_RenameComponent(GameObject targetGameObject, Func<string> newName) =>
            SystemButton(EditorSpriteSheets.EditorUI.Icons.Edit)
                .SetTooltip($"Rename GameObject")
                .SetOnClick(() =>
                {
                    Undo.RecordObject(targetGameObject, "Rename");
                    targetGameObject.name = newName.Invoke();
                });

        /// <summary>
        /// Rename the target gameObject to the given new name (has Undo)
        /// <para/> Fluid button used under headers for in-editor actions
        /// </summary>
        public static FluidButton SystemButton_RenameAsset(Object targetAsset, Func<string> newName) =>
            SystemButton(EditorSpriteSheets.EditorUI.Icons.Edit)
                .SetTooltip($"Rename Asset")
                .SetOnClick(() =>
                {
                    if (targetAsset == null) return;
                    if (newName == null) return;
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(targetAsset), newName.Invoke());
                    EditorUtility.SetDirty(targetAsset);
                    AssetDatabase.SaveAssets();
                });

        /// <summary> Get a FluidToggleButtonTab set up to be used under a ComponentHeader bar </summary>
        /// <param name="textures"> Tab Button icon textures </param>
        /// <param name="selectableAccentColor"> Accent color for the tab button </param>
        public static FluidToggleButtonTab GetTabButtonForComponentSection(IEnumerable<Texture2D> textures = null, EditorSelectableColorInfo selectableAccentColor = null)
        {
            FluidToggleButtonTab tabButton =
                FluidToggleButtonTab.Get()
                    .SetContainerColorOff(tabButtonColorOff)
                    .SetTabPosition(TabPosition.TabOnBottom)
                    .SetElementSize(ElementSize.Normal);

            if (textures != null)
                tabButton.SetIcon(textures);
            if (selectableAccentColor != null)
                tabButton.SetToggleAccentColor(selectableAccentColor);

            tabButton.iconReaction?.SetDuration(0.6f);

            return tabButton;
        }

        /// <summary> Get a FluidToggleButtonTab, an EnabledIndicator and a container with them assembled, set up to be used under a ComponentHeader bar </summary>
        /// <param name="textures"> Tab Button icon textures </param>
        /// <param name="selectableAccentColor"> Accent color for the tab button </param>
        /// <param name="accentColor"> Accent color for the indicator </param>
        public static (FluidToggleButtonTab, EnabledIndicator, VisualElement) GetTabButtonForComponentSectionWithEnabledIndicator(IEnumerable<Texture2D> textures, EditorSelectableColorInfo selectableAccentColor, Color accentColor)
        {
            FluidToggleButtonTab tab = GetTabButtonForComponentSection(textures, selectableAccentColor);
            EnabledIndicator indicator = EnabledIndicator.Get().SetEnabledColor(accentColor);
            VisualElement container = column.SetStyleFlexGrow(0).AddChild(indicator).AddChild(tab);
            return (tab, indicator, container);
        }

        public static FluidButton GetNewTinyButton
        (
            string text,
            IEnumerable<Texture2D> textures,
            EditorSelectableColorInfo selectableColor = null,
            string tooltip = ""
        ) =>
            FluidButton.Get()
                .SetLabelText(text)
                .SetIcon(textures)
                .SetAccentColor(selectableColor ?? EditorSelectableColors.Default.ButtonIcon)
                .SetTooltip(tooltip)
                .SetButtonStyle(ButtonStyle.Contained)
                .SetElementSize(ElementSize.Tiny);

        /// <summary> Get a Fluid ListView for the given array property </summary>
        /// <param name="arrayProperty"> Target arrayProperty (not checked; will result in error if a property of an array is not passed) </param>
        /// <param name="listTitle"> Title displayed on top of the list </param>
        /// <param name="listDescription"> Descriptions displayed below the title </param>
        public static FluidListView NewPropertyListView(SerializedProperty arrayProperty, string listTitle, string listDescription)
        {
            var flv = new FluidListView();
            var itemsSource = new List<SerializedProperty>();

            flv.SetListTitle(listTitle);
            flv.SetListDescription(listDescription);
            flv.listView.selectionType = SelectionType.None;
            flv.listView.itemsSource = itemsSource;
            flv.listView.makeItem = () => new PropertyFluidListViewItem(flv);

            flv.listView.bindItem = (element, i) =>
            {
                var item = (PropertyFluidListViewItem)element;
                item.Update(i, itemsSource[i]);
                item.OnRemoveButtonClick += itemProperty =>
                {
                    int propertyIndex = 0;
                    for (int j = 0; j < arrayProperty.arraySize; j++)
                    {
                        if (itemProperty.propertyPath != arrayProperty.GetArrayElementAtIndex(j).propertyPath)
                            continue;
                        propertyIndex = j;
                        break;
                    }
                    arrayProperty.DeleteArrayElementAtIndex(propertyIndex);
                    arrayProperty.serializedObject.ApplyModifiedProperties();

                    UpdateItemsSource();
                };
            };
            #if UNITY_2021_2_OR_NEWER
            flv.listView.fixedItemHeight = 30;
            flv.SetPreferredListHeight((int)flv.listView.fixedItemHeight * 6);
            #else
            flv.listView.itemHeight = 30;
            flv.SetPreferredListHeight(flv.listView.itemHeight * 6);
            #endif
            flv.SetDynamicListHeight(false);
            flv.HideFooterWhenEmpty(true);
            flv.UseSmallEmptyListPlaceholder(true);
            flv.emptyListPlaceholder.SetIcon(EditorSpriteSheets.EditorUI.Placeholders.EmptyListViewSmall);

            //ADD ITEM BUTTON (plus button)
            flv.AddNewItemButtonCallback += () =>
            {
                arrayProperty.InsertArrayElementAtIndex(0);
                arrayProperty.serializedObject.ApplyModifiedProperties();
                UpdateItemsSource();
            };

            UpdateItemsSource();

            int arraySize = arrayProperty.arraySize;
            flv.schedule.Execute(() =>
            {
                if (arrayProperty.arraySize == arraySize) return;
                arraySize = arrayProperty.arraySize;
                UpdateItemsSource();

            }).Every(100);

            void UpdateItemsSource()
            {
                itemsSource.Clear();
                for (int i = 0; i < arrayProperty.arraySize; i++)
                    itemsSource.Add(arrayProperty.GetArrayElementAtIndex(i));

                flv?.Update();
            }

            flv.schedule.Execute(flv.Update);
            return flv;
        }
        

        /// <summary> Get a Fluid ListView for the given array property </summary>
        /// <param name="arrayProperty"> Target arrayProperty (not checked; will result in error if a property of an array is not passed) </param>
        /// <param name="listTitle"> Title displayed on top of the list </param>
        /// <param name="listDescription"> Descriptions displayed below the title </param>
        /// <param name="objectType"> Type of object this ListView handles </param>
        /// <param name="allowDragAndDrop"> Drag and Drop functionality is added automatically if TRUE. Set FALSE if you want to no Drag and Drop or intend to implement a custom one </param>
        public static FluidListView NewObjectListView(SerializedProperty arrayProperty, string listTitle, string listDescription, Type objectType, bool allowDragAndDrop = true)
        {
            var flv = new FluidListView();
            var itemsSource = new List<SerializedProperty>();

            flv.SetListTitle(listTitle);
            flv.SetListDescription(listDescription);
            flv.listView.selectionType = SelectionType.None;
            flv.listView.itemsSource = itemsSource;
            flv.listView.makeItem = () => new ObjectFluidListViewItem(flv, objectType);

            flv.listView.bindItem = (element, i) =>
            {
                var item = (ObjectFluidListViewItem)element;
                item.Update(i, itemsSource[i]);
                item.OnRemoveButtonClick += itemProperty =>
                {
                    int propertyIndex = 0;
                    for (int j = 0; j < arrayProperty.arraySize; j++)
                    {
                        if (itemProperty.propertyPath != arrayProperty.GetArrayElementAtIndex(j).propertyPath)
                            continue;
                        propertyIndex = j;
                        break;
                    }
                    arrayProperty.DeleteArrayElementAtIndex(propertyIndex);
                    arrayProperty.serializedObject.ApplyModifiedProperties();

                    UpdateItemsSource();
                };
            };
            #if UNITY_2021_2_OR_NEWER
            flv.listView.fixedItemHeight = 30;
            flv.SetPreferredListHeight((int)flv.listView.fixedItemHeight * 6);
            #else
            flv.listView.itemHeight = 30;
            flv.SetPreferredListHeight(flv.listView.itemHeight * 6);
            #endif
            flv.SetDynamicListHeight(false);
            flv.HideFooterWhenEmpty(true);
            flv.UseSmallEmptyListPlaceholder(true);
            flv.emptyListPlaceholder.SetIcon(EditorSpriteSheets.EditorUI.Placeholders.EmptyListViewSmall);

            //ADD ITEM BUTTON (plus button)
            flv.AddNewItemButtonCallback += () =>
            {
                arrayProperty.InsertArrayElementAtIndex(0);
                arrayProperty.GetArrayElementAtIndex(0).objectReferenceValue = null;
                arrayProperty.serializedObject.ApplyModifiedProperties();
                UpdateItemsSource();
            };

            UpdateItemsSource();

            int arraySize = arrayProperty.arraySize;
            flv.schedule.Execute(() =>
            {
                if (arrayProperty.arraySize == arraySize) return;
                arraySize = arrayProperty.arraySize;
                UpdateItemsSource();

            }).Every(100);

            void UpdateItemsSource()
            {
                itemsSource.Clear();
                for (int i = 0; i < arrayProperty.arraySize; i++)
                    itemsSource.Add(arrayProperty.GetArrayElementAtIndex(i));

                flv?.Update();
            }

            if (!allowDragAndDrop)
            {
                flv.schedule.Execute(flv.Update);
                return flv;
            }

            //Drag and Drop
            {
                flv.RegisterCallback<AttachToPanelEvent>(_ =>
                {
                    flv.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
                    flv.RegisterCallback<DragPerformEvent>(OnDragPerformEvent);
                });

                flv.RegisterCallback<DetachFromPanelEvent>(_ =>
                {
                    flv.UnregisterCallback<DragUpdatedEvent>(OnDragUpdate);
                    flv.UnregisterCallback<DragPerformEvent>(OnDragPerformEvent);
                });

                void OnDragUpdate(DragUpdatedEvent evt)
                {
                    bool isValid = DragAndDrop.objectReferences.Any(item => item.GetType() == objectType || item.GetType().IsSubclassOf(objectType));
                    if (!isValid)
                    {
                        foreach (Object item in DragAndDrop.objectReferences)
                        {
                            Type itemType = item.GetType();
                            if (itemType == objectType)
                            {
                                isValid = true;
                                break;
                            }

                            if (item is GameObject go && go.GetComponent(objectType) != null)
                            {
                                isValid = true;
                                break;
                            }
                        }
                    }
                    if (!isValid) return;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                }

                void OnDragPerformEvent(DragPerformEvent evt)
                {
                    var references =
                        DragAndDrop.objectReferences
                            .Where(item => item != null)
                            .OrderBy(item => item.name)
                            .ToList();

                    foreach (Object item in references)
                    {
                        //check if we're dragging the correct object
                        if (item.GetType() == objectType || item.GetType().IsSubclassOf(objectType))
                        {
                            bool canAddItem = true;
                            for (int i = 0; i < arrayProperty.arraySize; i++)
                            {
                                if (arrayProperty.GetArrayElementAtIndex(i).objectReferenceValue != item)
                                    continue;
                                canAddItem = false;
                                break;
                            }
                            if (!canAddItem) continue;
                            arrayProperty.InsertArrayElementAtIndex(arrayProperty.arraySize);
                            arrayProperty.GetArrayElementAtIndex(arrayProperty.arraySize - 1).objectReferenceValue = item;
                            continue;
                        }

                        //if the dragged object is a GameObject, check to see if the correct object is attached to it
                        if (item is GameObject go)
                        {
                            Component component = go.GetComponent(objectType);
                            if (component == null) continue;
                            bool canAddComponent = true;
                            for (int i = 0; i < arrayProperty.arraySize; i++)
                            {
                                if (arrayProperty.GetArrayElementAtIndex(i).objectReferenceValue != component)
                                    continue;
                                canAddComponent = false;
                                break;
                            }
                            if (!canAddComponent) continue;
                            arrayProperty.InsertArrayElementAtIndex(arrayProperty.arraySize);
                            arrayProperty.GetArrayElementAtIndex(arrayProperty.arraySize - 1).objectReferenceValue = go.GetComponent(objectType);
                        }
                    }
                    arrayProperty.serializedObject.ApplyModifiedProperties();
                }
            }

            flv.schedule.Execute(flv.Update);
            return flv;
        }

        public static FluidToggleSwitch GetDebugSwitch(SerializedProperty property) =>
            FluidToggleSwitch.Get()
                .SetLabelText("Debug Mode")
                .SetTooltip("Enable relevant debug messages to be printed to the console")
                .SetToggleAccentColor(EditorSelectableColors.EditorUI.Red)
                .BindToProperty(property)
                .SetStyleAlignSelf(Align.FlexEnd);

        public static class Buttons
        {
            public static FluidButton RefreshDatabase
            (
                string labelText,
                string tooltipText,
                EditorSelectableColorInfo selectableColor,
                UnityAction onClickCallback
            ) =>
                FluidButton.Get()
                    .SetLabelText(labelText)
                    .SetTooltip(tooltipText)
                    .SetAccentColor(selectableColor)
                    .SetOnClick(onClickCallback)
                    .SetButtonStyle(ButtonStyle.Contained)
                    .SetElementSize(ElementSize.Small)
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Refresh);
        }
    }
}
