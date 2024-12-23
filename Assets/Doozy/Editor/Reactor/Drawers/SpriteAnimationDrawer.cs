// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.Common.Extensions;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Components;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Doozy.Editor.Reactor.Drawers
{
    [CustomPropertyDrawer(typeof(SpriteAnimation), true)]
    public class SpriteAnimationDrawer : PropertyDrawer
    {
        private static Color accentColor => EditorColors.Reactor.Red;
        private static EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Reactor.Red;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var drawer = new VisualElement();
            if (property == null) return drawer;
            drawer.RegisterCallback<DetachFromPanelEvent>(evt => drawer.RecycleAndClear());
            var target = property.GetTargetObjectOfProperty() as SpriteAnimation;

            #region SerializedProperties

            //Animation
            SerializedProperty propertySprites = property.FindPropertyRelative("Sprites");
            SerializedProperty propertyAnimation = property.FindPropertyRelative("Animation");
            SerializedProperty propertyAnimationEnabled = propertyAnimation.FindPropertyRelative("Enabled");
            //CALLBACKS            
            SerializedProperty propertyOnPlayCallback = property.FindPropertyRelative("OnPlayCallback");
            SerializedProperty propertyOnStopCallback = property.FindPropertyRelative("OnStopCallback");
            SerializedProperty propertyOnFinishCallback = property.FindPropertyRelative("OnFinishCallback");

            #endregion

            #region ComponentHeader

            FluidComponentHeader componentHeader =
                FluidComponentHeader.Get()
                    .SetAccentColor(accentColor)
                    .SetElementSize(ElementSize.Tiny)
                    .SetComponentNameText("Sprite Animation")
                    .AddManualButton("www.bit.ly/DoozyKnowledgeBase4")
                    .AddApiButton()
                    .AddYouTubeButton();

            #endregion

            #region Containers

            VisualElement contentContainer = new VisualElement().SetName("Content Container").SetStyleFlexGrow(1);
            FluidAnimatedContainer settingsAnimatedContainer = new FluidAnimatedContainer("Animation", true).Hide(false);
            FluidAnimatedContainer spritesAnimatedContainer = new FluidAnimatedContainer("Sprites", true).Hide(false);
            FluidAnimatedContainer callbacksAnimatedContainer = new FluidAnimatedContainer("Callbacks", true).Hide(false);

            //settings container content
            settingsAnimatedContainer.SetOnShowCallback(() =>
            {
                settingsAnimatedContainer
                    .AddContent(GetAnimationContent(propertyAnimation, propertyAnimationEnabled))
                    .Bind(property.serializedObject);
            });

            //sprites container content
            spritesAnimatedContainer.SetOnShowCallback(() =>
            {
                spritesAnimatedContainer
                    .AddContent(GetSpritesContent(propertySprites, target))
                    .Bind(property.serializedObject);
            });

            //callbacks container content
            callbacksAnimatedContainer.SetOnShowCallback(() =>
            {
                callbacksAnimatedContainer
                    .AddContent
                    (
                        FluidField.Get()
                            .AddFieldContent(DesignUtils.NewPropertyField(propertyOnPlayCallback.propertyPath))
                            .AddFieldContent(DesignUtils.spaceBlock)
                            .AddFieldContent(DesignUtils.NewPropertyField(propertyOnStopCallback.propertyPath))
                            .AddFieldContent(DesignUtils.spaceBlock)
                            .AddFieldContent(DesignUtils.NewPropertyField(propertyOnFinishCallback.propertyPath))
                    )
                    .AddContent(DesignUtils.endOfLineBlock)
                    .Bind(property.serializedObject);
            });

            #endregion

            #region Toolbar

            VisualElement toolbarContainer =
                new VisualElement()
                    .SetName("Toolbar Container")
                    .SetStyleFlexDirection(FlexDirection.Row)
                    .SetStyleMarginTop(-1)
                    .SetStyleMarginLeft(4)
                    .SetStyleMarginRight(4)
                    .SetStyleFlexGrow(1);

            FluidTab settingsTab =
                FluidTab.Get()
                    .SetLabelText("Settings")
                    .SetElementSize(ElementSize.Small)
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Settings)
                    .ButtonSetAccentColor(selectableAccentColor)
                    .IndicatorSetEnabledColor(accentColor)
                    .ButtonSetOnValueChanged(evt => settingsAnimatedContainer.Toggle(evt.newValue));

            FluidTab spritesTab =
                FluidTab.Get()
                    .SetLabelText("Sprites")
                    .SetElementSize(ElementSize.Small)
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Sprite)
                    .ButtonSetAccentColor(selectableAccentColor)
                    .IndicatorSetEnabledColor(accentColor)
                    .ButtonSetOnValueChanged(evt => spritesAnimatedContainer.Toggle(evt.newValue));

            FluidTab callbacksTab =
                FluidTab.Get()
                    .SetLabelText("Callbacks")
                    .SetElementSize(ElementSize.Small)
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.UnityEvent)
                    .ButtonSetAccentColor(DesignUtils.callbackSelectableColor)
                    .IndicatorSetEnabledColor(DesignUtils.callbacksColor)
                    .ButtonSetOnValueChanged(evt => callbacksAnimatedContainer.Toggle(evt.newValue));

            //create tabs group
            FluidToggleGroup tabsGroup = FluidToggleGroup.Get().SetControlMode(FluidToggleGroup.ControlMode.OneToggleOn);
            settingsTab.button.AddToToggleGroup(tabsGroup);
            callbacksTab.button.AddToToggleGroup(tabsGroup);
            spritesTab.button.AddToToggleGroup(tabsGroup);



            //update tab indicators
            drawer.schedule.Execute(() =>
            {
                void UpdateIndicator(FluidTab fluidTab, bool toggleOn, bool animateChange)
                {
                    if (fluidTab.indicator.isOn == toggleOn) return;
                    fluidTab.indicator.Toggle(toggleOn, animateChange);
                }

                bool HasCallbacks() =>
                    target != null &&
                    target.OnPlayCallback?.GetPersistentEventCount() > 0 |  //HasOnPlayCallback
                    target.OnStopCallback?.GetPersistentEventCount() > 0 |  //HasOnPlayCallback
                    target.OnFinishCallback?.GetPersistentEventCount() > 0; //HasOnFinishCallback

                //initial indicators state update (no animation)
                UpdateIndicator(settingsTab, propertyAnimationEnabled.boolValue, false);
                UpdateIndicator(spritesTab, propertySprites.arraySize > 0, false);
                UpdateIndicator(callbacksTab, HasCallbacks(), false);

                drawer.schedule.Execute(() =>
                {
                    //subsequent indicators state update (animated)
                    UpdateIndicator(settingsTab, propertyAnimationEnabled.boolValue, true);
                    UpdateIndicator(spritesTab, propertySprites.arraySize > 0, true);
                    UpdateIndicator(callbacksTab, HasCallbacks(), true);

                }).Every(200);
            });

            toolbarContainer
                .AddChild(settingsTab)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(spritesTab)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(callbacksTab)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(DesignUtils.flexibleSpace);

            #endregion

            #region Compose

            drawer
                .AddChild(componentHeader)
                .AddChild(toolbarContainer)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild
                (
                    contentContainer
                        .AddChild(settingsAnimatedContainer)
                        .AddChild(spritesAnimatedContainer)
                        .AddChild(callbacksAnimatedContainer)
                );

            #endregion

            return drawer;
        }

        private static VisualElement GetSpritesContent(SerializedProperty arrayProperty, SpriteAnimation animation)
        {
            SpriteAnimationInfo animationInfo =
                new SpriteAnimationInfo(arrayProperty)
                    .SetStyleMarginTop(DesignUtils.k_Spacing);

            animationInfo.spriteSetter = sprite =>
            {
                if (sprite == null) return;
                if (animation == null) return;
                if (animation.spriteTarget == null) return;
                Component objectToUndo = animation.spriteTarget.GetComponent(animation.spriteTarget.targetType);
                Undo.RecordObject(objectToUndo, "Set Sprite");
                animation.spriteTarget.SetSprite(sprite);
                EditorUtility.SetDirty(objectToUndo);
            };

            var itemsSource = new List<SerializedProperty>();
            var fluidListView = new FluidListView();

            VisualElement content =
                new VisualElement()
                    .AddChild(fluidListView)
                    .AddChild(animationInfo)
                    .AddChild(DesignUtils.endOfLineBlock);
            content.Bind(arrayProperty.serializedObject);

            animationInfo.SetStyleDisplay(arrayProperty.arraySize > 0 ? DisplayStyle.Flex : DisplayStyle.None);

            content.RegisterCallback<AttachToPanelEvent>(evt =>
            {
                content.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
                content.RegisterCallback<DragPerformEvent>(OnDragPerformEvent);
            });

            content.RegisterCallback<DetachFromPanelEvent>(evy =>
            {
                content.UnregisterCallback<DragUpdatedEvent>(OnDragUpdate);
                content.UnregisterCallback<DragPerformEvent>(OnDragPerformEvent);
            });

            void OnDragUpdate(DragUpdatedEvent evt)
            {
                bool isValid = DragAndDrop.objectReferences.Any(item => item is Texture);
                if (!isValid) //check if it's a folder 
                {
                    string assetPath = AssetDatabase.GetAssetPath(DragAndDrop.objectReferences[0]);
                    string[] paths = AssetDatabase.FindAssets($"t:{nameof(Texture)}", new[] { assetPath });
                    isValid = paths.Select(path => AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(path))).Any(sprite => sprite != null);
                }
                if (!isValid) return;
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            }

            void OnDragPerformEvent(DragPerformEvent evt)
            {
                var references = DragAndDrop.objectReferences.Where(item => item != null && item is Texture).OrderBy(item => item.name).ToList();
                if (references.Count == 0) //check if it's a folder
                {
                    string folderPath = AssetDatabase.GetAssetPath(DragAndDrop.objectReferences[0]);
                    string[] guids = AssetDatabase.FindAssets($"t:{nameof(Texture)}", new[] { folderPath });
                    references.AddRange(guids.Select(guid => AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guid))).OrderBy(item => item.name));
                }
                else //not a folder - look for sprite sheets
                {
                    var temp = references.ToList();
                    references.Clear();
                    foreach (Texture texture in temp.OfType<Texture>().ToList())
                    {
                        string assetPath = AssetDatabase.GetAssetPath(texture); //get sheet asset path

                        if (texture.IsSpriteSheet())
                        {
                            references.AddRange(AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath).OfType<Sprite>().OrderBy(item => item.name));
                        }
                        else
                        {
                            references.Add(AssetDatabase.LoadAssetAtPath<Sprite>(assetPath));
                        }
                    }
                }

                if (references.Count == 0)
                    return;

                Sprite firstSprite = null;
                arrayProperty.ClearArray();
                foreach (Sprite sprite in references.OfType<Sprite>())
                {
                    firstSprite ??= sprite;
                    arrayProperty.InsertArrayElementAtIndex(arrayProperty.arraySize);
                    arrayProperty.GetArrayElementAtIndex(arrayProperty.arraySize - 1).objectReferenceValue = sprite;
                }
                
                //Update sprite via the Set Sprite button setter
                animationInfo.spriteSetter.Invoke(firstSprite);
                
                arrayProperty.serializedObject.ApplyModifiedProperties();
                animation.UpdateAnimationSprites();
                animationInfo.Update();
            }

            fluidListView.listView.selectionType = SelectionType.None;
            fluidListView.listView.itemsSource = itemsSource;
            fluidListView.listView.makeItem = () => new ObjectFluidListViewItem(fluidListView, typeof(Sprite));
            fluidListView.listView.bindItem = (element, i) =>
            {
                var item = (ObjectFluidListViewItem)element;
                item.Update(i, itemsSource[i]);
                item.OnRemoveButtonClick += property =>
                {
                    int propertyIndex = 0;
                    for (int j = 0; j < arrayProperty.arraySize; j++)
                    {
                        if (property.propertyPath != arrayProperty.GetArrayElementAtIndex(j).propertyPath)
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
            fluidListView.listView.fixedItemHeight = 24;
            fluidListView.SetPreferredListHeight((int)fluidListView.listView.fixedItemHeight * 6);
            #else
            fluidListView.listView.itemHeight = 24;
            fluidListView.SetPreferredListHeight(fluidListView.listView.itemHeight * 6);
            #endif

            fluidListView.SetDynamicListHeight(false);

            //ADD ITEM BUTTON (plus button)
            fluidListView.AddNewItemButtonCallback += () =>
            {
                arrayProperty.InsertArrayElementAtIndex(0);
                arrayProperty.GetArrayElementAtIndex(0).objectReferenceValue = null;
                arrayProperty.serializedObject.ApplyModifiedProperties();
                UpdateItemsSource();
            };

            var sortAzButton =
                FluidListView.Buttons.sortAzButton
                    .SetOnClick(() =>
                    {
                        Undo.RecordObject(arrayProperty.serializedObject.targetObject, "Sort Az");
                        animation.SortSpritesAz();
                        arrayProperty.serializedObject.UpdateIfRequiredOrScript();
                        arrayProperty.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                        UpdateItemsSource();
                        UpdateAnimationInfo();
                    });

            var sortZaButton =
                FluidListView.Buttons.sortZaButton
                    .SetOnClick(() =>
                    {
                        Undo.RecordObject(arrayProperty.serializedObject.targetObject, "Sort Za");
                        animation.SortSpritesZa();
                        arrayProperty.serializedObject.UpdateIfRequiredOrScript();
                        arrayProperty.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                        UpdateItemsSource();
                        UpdateAnimationInfo();
                    });

            var clearButton =
                FluidListView.Buttons.clearButton
                    .SetOnClick(() =>
                    {
                        arrayProperty.ClearArray();
                        arrayProperty.serializedObject.ApplyModifiedProperties();
                    });

            fluidListView.AddToolbarElement(sortAzButton);
            fluidListView.AddToolbarElement(sortZaButton);
            fluidListView.AddToolbarElement(DesignUtils.flexibleSpace);
            fluidListView.AddToolbarElement(clearButton);

            int arraySize = arrayProperty.arraySize;
            fluidListView.schedule.Execute(() =>
            {
                if (arrayProperty.arraySize == arraySize) return;
                arraySize = arrayProperty.arraySize;
                UpdateItemsSource();
                UpdateAnimationInfo();

            }).Every(100);

            void UpdateAnimationInfo()
            {
                if (arrayProperty.arraySize == 0)
                {
                    animationInfo.SetStyleDisplay(DisplayStyle.None);
                    return;
                }

                bool updateAnimationInfo = false;
                for (int i = 0; i < arrayProperty.arraySize; i++)
                {
                    if (arrayProperty.GetArrayElementAtIndex(i).objectReferenceValue == null)
                        continue;
                    updateAnimationInfo = true;
                    break;
                }

                if (!updateAnimationInfo)
                    return;

                animationInfo.SetStyleDisplay(DisplayStyle.Flex);
                animationInfo.Update();
            }

            void UpdateItemsSource()
            {
                itemsSource.Clear();

                for (int i = 0; i < arrayProperty.arraySize; i++)
                    itemsSource.Add(arrayProperty.GetArrayElementAtIndex(i));

                fluidListView?.Update();
            }

            UpdateItemsSource();

            return content;
        }

        private const int HEIGHT = 42;
        
        private static VisualElement GetAnimationContent(SerializedProperty propertyAnimation, SerializedProperty propertyAnimationEnabled)
        {
            SerializedProperty propertyFromReferenceValue = propertyAnimation.FindPropertyRelative("FromReferenceValue");
            SerializedProperty propertyToReferenceValue = propertyAnimation.FindPropertyRelative("ToReferenceValue");

            SerializedProperty propertyFromFrameOffset = propertyAnimation.FindPropertyRelative("FromFrameOffset");
            SerializedProperty propertyToFrameOffset = propertyAnimation.FindPropertyRelative("ToFrameOffset");

            SerializedProperty propertyFromCustomValue = propertyAnimation.FindPropertyRelative("FromCustomValue");
            SerializedProperty propertyToCustomValue = propertyAnimation.FindPropertyRelative("ToCustomValue");

            SerializedProperty propertyFromCustomProgress = propertyAnimation.FindPropertyRelative("FromCustomProgress");
            SerializedProperty propertyToCustomProgress = propertyAnimation.FindPropertyRelative("ToCustomProgress");

            SerializedProperty propertySettings = propertyAnimation.FindPropertyRelative("Settings");

            var content = new VisualElement();
            content.SetEnabled(propertyAnimationEnabled.boolValue);
            FluidToggleSwitch enableSwitch = DesignUtils.GetEnableDisableSwitch(propertyAnimationEnabled, content, selectableAccentColor, "Animation");
            
            FluidField fromReferenceValueFluidField = FluidField.Get<EnumField>(propertyFromReferenceValue, "From Frame").SetStyleHeight(HEIGHT, HEIGHT, HEIGHT);
            FluidField toReferenceValueFluidField = FluidField.Get<EnumField>(propertyToReferenceValue, "To Frame").SetStyleHeight(HEIGHT, HEIGHT, HEIGHT);

            FluidField fromFrameOffsetFluidField = FluidField.Get<FloatField>(propertyFromFrameOffset, "From Offset").SetStyleHeight(HEIGHT, HEIGHT, HEIGHT);
            FluidField toFrameOffsetFluidField = FluidField.Get<FloatField>(propertyToFrameOffset, "To Offset").SetStyleHeight(HEIGHT, HEIGHT, HEIGHT);

            FluidField fromCustomValueFluidField = FluidField.Get<FloatField>(propertyFromCustomValue, "From Custom Frame").SetStyleHeight(HEIGHT, HEIGHT, HEIGHT);
            FluidField toCustomValueFluidField = FluidField.Get<FloatField>(propertyToCustomValue, "To Custom Frame").SetStyleHeight(HEIGHT, HEIGHT, HEIGHT);

            #region Custom Progress

            Label fromCustomProgressLabel = GetOffsetLabel(() => $"{(propertyFromCustomProgress.floatValue * 100).Round(0)}%");
            Label toCustomProgressLabel = GetOffsetLabel(() => $"{(propertyToCustomProgress.floatValue * 100).Round(0)}%");

            Slider fromCustomProgressSlider = DesignUtils.NewSlider(propertyFromCustomProgress, 0f, 1f);
            Slider toCustomProgressSlider = DesignUtils.NewSlider(propertyToCustomProgress, 0f, 1f);

            fromCustomProgressSlider.RegisterValueChangedCallback(evt =>
                fromCustomProgressLabel.SetText($"{(evt.newValue * 100).Round(0)}%"));

            toCustomProgressSlider.RegisterValueChangedCallback(evt =>
                toCustomProgressLabel.SetText($"{(evt.newValue * 100).Round(0)}%"));

            FluidField fromCustomProgressFluidField =
                GetOffsetField
                (
                    "Custom Progress", fromCustomProgressLabel, fromCustomProgressSlider,
                    () =>
                    {
                        propertyFromCustomProgress.floatValue = 0f;
                        propertyFromCustomProgress.serializedObject.ApplyModifiedProperties();
                    });

            FluidField toCustomProgressFluidField =
                GetOffsetField
                (
                    "Custom Progress", toCustomProgressLabel, toCustomProgressSlider,
                    () =>
                    {
                        propertyToCustomProgress.floatValue = 1f;
                        propertyToCustomProgress.serializedObject.ApplyModifiedProperties();
                    });

            #endregion

            PropertyField settingsPropertyField = DesignUtils.NewPropertyField(propertySettings).SetName("Animation Settings");

            VisualElement foldoutContent =
                new VisualElement()
                    .AddChild
                    (
                        DesignUtils.row
                            .AddChild(enableSwitch)
                            .AddChild(DesignUtils.flexibleSpace)
                    )
                    .AddChild
                    (
                        content
                            .AddChild
                            (
                                DesignUtils.row
                                    .SetName("From To Settings")
                                    .AddChild
                                    (
                                        DesignUtils.column
                                            .SetName("From Settings")
                                            .AddChild(fromReferenceValueFluidField)
                                            .AddChild(DesignUtils.spaceBlock)
                                            .AddChild(fromFrameOffsetFluidField)
                                            .AddChild(fromCustomValueFluidField)
                                            .AddChild(fromCustomProgressFluidField)
                                    )
                                    .AddChild(DesignUtils.spaceBlock)
                                    .AddChild
                                    (
                                        DesignUtils.column
                                            .SetName("To Settings")
                                            .AddChild(toReferenceValueFluidField)
                                            .AddChild(DesignUtils.spaceBlock)
                                            .AddChild(toFrameOffsetFluidField)
                                            .AddChild(toCustomValueFluidField)
                                            .AddChild(toCustomProgressFluidField)
                                    )
                            )
                            .AddChild(DesignUtils.spaceBlock)
                            .AddChild(settingsPropertyField)
                            .AddChild(DesignUtils.endOfLineBlock)
                    );

            void Update()
            {
                var fromReferenceValue = (FrameReferenceValue)propertyFromReferenceValue.enumValueIndex;
                bool showFromOffset =
                    fromReferenceValue == FrameReferenceValue.FirstFrame ||
                    fromReferenceValue == FrameReferenceValue.LastFrame ||
                    fromReferenceValue == FrameReferenceValue.CurrentFrame;

                fromFrameOffsetFluidField.SetStyleDisplay(showFromOffset ? DisplayStyle.Flex : DisplayStyle.None);
                fromCustomValueFluidField.SetStyleDisplay(fromReferenceValue == FrameReferenceValue.CustomFrame ? DisplayStyle.Flex : DisplayStyle.None);
                fromCustomProgressFluidField.SetStyleDisplay(fromReferenceValue == FrameReferenceValue.CustomProgress ? DisplayStyle.Flex : DisplayStyle.None);

                var toReferenceValue = (FrameReferenceValue)propertyToReferenceValue.enumValueIndex;
                bool showToOffset =
                    toReferenceValue == FrameReferenceValue.FirstFrame ||
                    toReferenceValue == FrameReferenceValue.LastFrame ||
                    toReferenceValue == FrameReferenceValue.CurrentFrame;

                toFrameOffsetFluidField.SetStyleDisplay(showToOffset ? DisplayStyle.Flex : DisplayStyle.None);
                toCustomValueFluidField.SetStyleDisplay(toReferenceValue == FrameReferenceValue.CustomFrame ? DisplayStyle.Flex : DisplayStyle.None);
                toCustomProgressFluidField.SetStyleDisplay(toReferenceValue == FrameReferenceValue.CustomProgress ? DisplayStyle.Flex : DisplayStyle.None);
            }

            //FromReferenceValue
            EnumField invisibleFieldRotateFromReferenceValueEnum = new EnumField { bindingPath = propertyFromReferenceValue.propertyPath }.SetStyleDisplay(DisplayStyle.None);
            foldoutContent.AddChild(invisibleFieldRotateFromReferenceValueEnum);
            invisibleFieldRotateFromReferenceValueEnum.RegisterValueChangedCallback(changeEvent => Update());

            //ToReferenceValue
            EnumField invisibleFieldRotateToReferenceValueEnum = new EnumField { bindingPath = propertyToReferenceValue.propertyPath }.SetStyleDisplay(DisplayStyle.None);
            foldoutContent.AddChild(invisibleFieldRotateToReferenceValueEnum);
            invisibleFieldRotateToReferenceValueEnum.RegisterValueChangedCallback(changeEvent => Update());

            foldoutContent.Bind(propertyAnimation.serializedObject);

            Update();
            return foldoutContent;
        }

        private static Label GetOffsetLabel(Func<string> value) =>
            DesignUtils.fieldLabel
                .ResetLayout()
                .SetText(value.Invoke())
                .SetStyleAlignSelf(Align.Center)
                .SetStyleTextAlign(TextAnchor.MiddleRight)
                .SetStyleWidth(24);

        private static FluidField GetOffsetField(string labelText, VisualElement label, VisualElement slider, UnityAction onClickCallback) =>
            FluidField.Get()
                .SetStyleHeight(HEIGHT, HEIGHT, HEIGHT)
                .SetLabelText(labelText)
                .AddFieldContent
                (
                    DesignUtils.row
                        .SetStyleJustifyContent(Justify.Center)
                        .AddChild(label)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(slider)
                        .AddChild
                        (
                            FluidButton.Get(EditorSpriteSheets.EditorUI.Icons.Reset)
                                .SetElementSize(ElementSize.Tiny)
                                .SetTooltip("Reset")
                                .SetOnClick(onClickCallback)
                        )
                );
    }
}
