// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.Common.Extensions;
using Doozy.Editor.Common.Utils;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Components;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.UIMenu
{
    [CustomEditor(typeof(UIMenuItem), true)]
    [CanEditMultipleObjects]
    public class UIMenuItemEditor : UnityEditor.Editor
    {
        private static IEnumerable<Texture2D> lockedUnlockedIconTextures => EditorSpriteSheets.EditorUI.Icons.LockedUnlocked;

        public UIMenuItem castedTarget => (UIMenuItem)target;
        public IEnumerable<UIMenuItem> castedTargets => targets.Cast<UIMenuItem>();
        public bool useDefaultIcon => castedTarget != null && (castedTarget.hasIcon || castedTarget.hasAnimatedIcon);

        private VisualElement root { get; set; }
        private FluidComponentHeader componentHeader { get; set; }
        private VisualElement headerToolbar { get; set; }

        public VisualElement prefabTypeContainer { get; private set; }
        public VisualElement prefabTypeNameContainer { get; private set; }
        public VisualElement prefabCategoryContainer { get; private set; }
        public VisualElement prefabNameContainer { get; private set; }
        public VisualElement instantiateModeContainer { get; private set; }
        public VisualElement infoTagContainer { get; private set; }
        public VisualElement iconContainer { get; private set; }
        public VisualElement tagsContainer { get; private set; }

        private EnumField prefabTypeEnumField { get; set; }
        private EnumField instantiateModeEnumField { get; set; }

        private FloatField animationDurationFloatField { get; set; }

        private FluidButton renameAssetButton { get; set; }
        private FluidButton saveButton { get; set; }

        private FluidField prefabFluidField { get; set; }
        private FluidField prefabInfoFluidField { get; set; }
        private FluidField animationDurationFluidField { get; set; }

        private FluidListView iconTextures2DFluidListView { get; set; }
        private FluidListView tagsFluidListView { get; set; }

        private FluidToggleIconButton lockInstantiateModeButton { get; set; }
        private FluidToggleSwitch colorizeSwitch { get; set; }

        private ObjectField prefabObjectField { get; set; }

        private TextField prefabCategoryTextField { get; set; }
        private TextField prefabNameTextField { get; set; }
        private TextField prefabTypeNameTextField { get; set; }
        private TextField infoTagTextField { get; set; }

        private Texture2DAnimationInfo iconTexture2DAnimationInfo { get; set; }

        private SerializedProperty propertyPrefab { get; set; }
        private SerializedProperty propertyPrefabType { get; set; }
        private SerializedProperty propertyPrefabTypeName { get; set; }
        private SerializedProperty propertyPrefabCategory { get; set; }
        private SerializedProperty propertyPrefabName { get; set; }
        private SerializedProperty propertyInstantiateMode { get; set; }
        private SerializedProperty propertyLockInstantiateMode { get; set; }
        private SerializedProperty propertyColorize { get; set; }
        private SerializedProperty propertyAnimationDuration { get; set; }
        private SerializedProperty propertyTags { get; set; }
        private SerializedProperty propertyInfoTag { get; set; }
        private SerializedProperty propertyPreviewSize { get; set; }
        private SerializedProperty propertyIcon { get; set; }
        private SerializedProperty propertySpriteSheet { get; set; }

        private void OnDestroy()
        {
            saveButton?.Recycle();
            renameAssetButton?.Recycle();

            prefabFluidField?.Recycle();
            prefabInfoFluidField?.Recycle();
            animationDurationFluidField?.Recycle();

            lockInstantiateModeButton?.Recycle();
            colorizeSwitch?.Recycle();

            iconTexture2DAnimationInfo?.Dispose();
            iconTextures2DFluidListView?.Dispose();
        }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        private void FindProperties()
        {
            propertyPrefab = serializedObject.FindProperty("Prefab");
            propertyPrefabType = serializedObject.FindProperty("PrefabType");
            propertyPrefabTypeName = serializedObject.FindProperty("PrefabTypeName");
            propertyPrefabCategory = serializedObject.FindProperty("PrefabCategory");
            propertyPrefabName = serializedObject.FindProperty("PrefabName");
            propertyInstantiateMode = serializedObject.FindProperty("InstantiateMode");
            propertyLockInstantiateMode = serializedObject.FindProperty("LockInstantiateMode");
            propertyColorize = serializedObject.FindProperty("Colorize");
            propertyAnimationDuration = serializedObject.FindProperty("AnimationDuration");
            propertyTags = serializedObject.FindProperty("Tags");
            propertyInfoTag = serializedObject.FindProperty("InfoTag");
            propertyPreviewSize = serializedObject.FindProperty("PreviewSize");
            propertyIcon = serializedObject.FindProperty("Icon");
            propertySpriteSheet = serializedObject.FindProperty("SpriteSheet");
        }

        private void InitializeEditor()
        {
            FindProperties();

            root = DesignUtils.GetEditorRoot();

            componentHeader =
                FluidComponentHeader.Get()
                    .SetElementSize(ElementSize.Large)
                    .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(UIMenuItem)))
                    .SetIcon(EditorSpriteSheets.UIManager.UIMenu.UIMenuItem)
                    .AddManualButton("https://doozyentertainment.atlassian.net/wiki/spaces/DUI4/pages/1048346625/UIMenu+Item?atlOrigin=eyJpIjoiMmE1NGJiMGRhMzc4NGNkZWFlMWI1OTFmNjMyMTc0ZmMiLCJwIjoiYyJ9")
                    .AddApiButton("https://api.doozyui.com/api/Doozy.Editor.UIManager.UIMenu.UIMenuItem.html")
                    .AddYouTubeButton();

            saveButton =
                DesignUtils.SystemButton(EditorSpriteSheets.EditorUI.Icons.Save)
                    .SetOnClick(() => { UIMenuItemsDatabase.instance.RefreshDatabase(); });

            renameAssetButton =
                DesignUtils.SystemButton_RenameAsset(castedTarget, () => castedTarget.cleanAssetName);


            prefabObjectField =
                DesignUtils.NewObjectField(propertyPrefab, typeof(GameObject), false)
                    .SetStyleFlexGrow(1);

            prefabFluidField =
                FluidField.Get()
                    .SetLabelText("Prefab")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Prefab)
                    .AddFieldContent(prefabObjectField);

            prefabTypeEnumField =
                DesignUtils.NewEnumField(propertyPrefabType)
                    .ResetLayout()
                    .SetStyleFlexGrow(1);

            prefabTypeNameTextField =
                DesignUtils.NewTextField(propertyPrefabTypeName, true)
                    .ResetLayout()
                    .SetStyleFlexGrow(1);

            prefabCategoryTextField =
                DesignUtils.NewTextField(propertyPrefabCategory, true)
                    .ResetLayout()
                    .SetStyleFlexGrow(1);

            prefabNameTextField =
                DesignUtils.NewTextField(propertyPrefabName, true)
                    .ResetLayout()
                    .SetStyleFlexGrow(1);

            instantiateModeEnumField =
                DesignUtils.NewEnumField(propertyInstantiateMode)
                    .ResetLayout()
                    .SetStyleWidth(60);

            lockInstantiateModeButton =
                FluidToggleIconButton.Get()
                    .SetTooltip("Lock Instantiate Mode to the current setting")
                    .SetIcon(lockedUnlockedIconTextures)
                    .SetElementSize(ElementSize.Small)
                    .BindToProperty(propertyLockInstantiateMode);

            lockInstantiateModeButton.iconReaction.ReverseTexturesOrder();

            colorizeSwitch =
                FluidToggleSwitch.Get()
                    .SetLabelText("Colorize")
                    .SetTooltip("Apply current Unity Theme color to the Preview Image")
                    .BindToProperty(propertyColorize);

            animationDurationFloatField =
                DesignUtils.NewFloatField(propertyAnimationDuration)
                    .SetStyleFlexGrow(1);

            animationDurationFluidField =
                FluidField.Get("Animation Duration")
                    .AddFieldContent(animationDurationFloatField)
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Cooldown)
                    .SetElementSize(ElementSize.Tiny);

            infoTagTextField =
                DesignUtils.NewTextField(propertyInfoTag)
                    .ResetLayout()
                    .SetStyleAlignSelf(Align.FlexStart)
                    .SetStyleWidth(128)
                    .SetMaxLength(16);

            Label GetLabel(string fieldName) =>
                DesignUtils.NewFieldNameLabel(fieldName);


            prefabTypeContainer = DesignUtils.column.SetStyleWidth(128, 128, 128)
                .AddChild(GetLabel("Prefab Type"))
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(prefabTypeEnumField);

            prefabTypeNameContainer = DesignUtils.column
                .AddChild(GetLabel("Prefab Type Name"))
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(prefabTypeNameTextField);

            prefabCategoryContainer = DesignUtils.column
                .AddChild(GetLabel("Prefab Category"))
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(prefabCategoryTextField);

            prefabNameContainer = DesignUtils.column
                .AddChild(GetLabel("Prefab Name"))
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(prefabNameTextField);

            instantiateModeContainer = DesignUtils.column.SetStyleFlexGrow(0).AddChild(GetLabel("Instantiate Mode"))
                .AddChild(DesignUtils.spaceBlock)
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleFlexGrow(0)
                        .AddChild(instantiateModeEnumField)
                        .AddChild(lockInstantiateModeButton)
                );

            infoTagContainer = DesignUtils.column.SetStyleFlexGrow(0)
                .AddChild(GetLabel("Info Tag"))
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(infoTagTextField);


            prefabInfoFluidField =
                FluidField.Get()
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddChild(prefabTypeContainer.SetStyleMarginRight(DesignUtils.k_Spacing))
                            .AddChild(prefabTypeNameContainer)
                    )
                    .AddFieldContent(DesignUtils.spaceBlock)
                    .AddFieldContent
                    (
                        DesignUtils.row
                            .AddChild(prefabCategoryContainer.SetStyleMarginRight(DesignUtils.k_Spacing))
                            .AddChild(prefabNameContainer.SetStyleMarginRight(DesignUtils.k_Spacing))
                            .AddChild(instantiateModeContainer)
                    )
                    .AddFieldContent(DesignUtils.spaceBlock)
                    .AddFieldContent(infoTagContainer)
                ;

            prefabTypeNameTextField.SetEnabled((UIPrefabType)propertyPrefabType.enumValueIndex == UIPrefabType.Custom);
            if (propertyPrefabName.stringValue.IsNullOrEmpty())
            {
                propertyPrefabName.stringValue = ((UIPrefabType)propertyPrefabType.enumValueIndex).ToString();
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }

            prefabTypeEnumField.RegisterValueChangedCallback(evt =>
            {
                if (evt?.newValue == null) return;
                var prefabType = (UIPrefabType)evt.newValue;
                prefabTypeNameTextField.SetEnabled(prefabType == UIPrefabType.Custom);

                if (!propertyPrefabName.stringValue.IsNullOrEmpty() && prefabType == UIPrefabType.Custom)
                    return;
                prefabTypeNameTextField.value = prefabType.ToString();
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            });

            prefabObjectField.RegisterValueChangedCallback(evt =>
            {
                if (evt?.newValue == null) return;
                prefabNameTextField.value = ObjectNames.NicifyVariableName(evt.newValue.name.RemoveWhitespaces().RemoveAllSpecialCharacters());
                castedTarget.Validate();
            });
        }

        private VisualElement GetTagsContainer()
        {
            var itemsSource = new List<SerializedProperty>();
            tagsFluidListView = new FluidListView();

            VisualElement content =
                new VisualElement()
                    .AddChild(tagsFluidListView);

            tagsFluidListView.listView.selectionType = SelectionType.None;
            tagsFluidListView.listView.itemsSource = itemsSource;
            tagsFluidListView.listView.makeItem = () => new PropertyFluidListViewItem(tagsFluidListView);
            tagsFluidListView.listView.bindItem = (element, i) =>
            {
                var item = (PropertyFluidListViewItem)element;
                item.propertyField.TryToHideLabel();
                item.Update(i, itemsSource[i]);
                item.OnRemoveButtonClick += property =>
                {
                    int propertyIndex = 0;
                    for (int j = 0; j < propertyTags.arraySize; j++)
                    {
                        if (property.propertyPath != propertyTags.GetArrayElementAtIndex(j).propertyPath)
                            continue;
                        propertyIndex = j;
                        break;
                    }
                    propertyTags.DeleteArrayElementAtIndex(propertyIndex);
                    propertyTags.serializedObject.ApplyModifiedProperties();

                    UpdateItemsSource();
                };
            };

            #if UNITY_2021_2_OR_NEWER
            tagsFluidListView.listView.fixedItemHeight = 24;
            tagsFluidListView.SetPreferredListHeight((int)tagsFluidListView.listView.fixedItemHeight * 6);
            #else
            tagsFluidListView.listView.itemHeight = 24;
            tagsFluidListView.SetPreferredListHeight(tagsFluidListView.listView.itemHeight * 6);
            #endif

            tagsFluidListView.SetDynamicListHeight(false);
            tagsFluidListView.SetListTitle("Tags");
            tagsFluidListView.SetListDescription("List of tags used to describe this prefab");
            tagsFluidListView.UseSmallEmptyListPlaceholder(true);
            tagsFluidListView.emptyListPlaceholder.SetIcon(EditorSpriteSheets.EditorUI.Placeholders.EmptyListViewSmall);


            //ADD ITEM BUTTON (plus button)
            tagsFluidListView.AddNewItemButtonCallback += () =>
            {
                propertyTags.InsertArrayElementAtIndex(0);
                // propertyTags.GetArrayElementAtIndex(0).objectReferenceValue = null;
                propertyTags.serializedObject.ApplyModifiedProperties();
                UpdateItemsSource();
            };

            var sortAzButton =
                FluidListView.Buttons.sortAzButton
                    .SetOnClick(() =>
                    {
                        Undo.RecordObject(propertyTags.serializedObject.targetObject, "Sort Az");
                        castedTarget.SortTagsAz();
                        propertyTags.serializedObject.UpdateIfRequiredOrScript();
                        propertyTags.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                        UpdateItemsSource();
                    });

            var sortZaButton =
                FluidListView.Buttons.sortZaButton
                    .SetOnClick(() =>
                    {
                        Undo.RecordObject(propertyTags.serializedObject.targetObject, "Sort Za");
                        castedTarget.SortTagsZa();
                        propertyTags.serializedObject.UpdateIfRequiredOrScript();
                        propertyTags.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                        UpdateItemsSource();
                    });

            var clearButton =
                FluidListView.Buttons.clearButton
                    .SetOnClick(() =>
                    {
                        propertyTags.ClearArray();
                        propertyTags.serializedObject.ApplyModifiedProperties();
                    });

            tagsFluidListView.AddToolbarElement(sortAzButton);
            tagsFluidListView.AddToolbarElement(sortZaButton);
            tagsFluidListView.AddToolbarElement(DesignUtils.flexibleSpace);
            tagsFluidListView.AddToolbarElement(clearButton);

            int arraySize = -1;
            tagsFluidListView.schedule.Execute(() =>
            {
                if (propertyTags.arraySize == arraySize) return;
                arraySize = propertyTags.arraySize;
                UpdateItemsSource();

            }).Every(100);

            void UpdateItemsSource()
            {
                itemsSource.Clear();

                for (int i = 0; i < propertyTags.arraySize; i++)
                    itemsSource.Add(propertyTags.GetArrayElementAtIndex(i));

                tagsFluidListView?.Update();
            }

            UpdateItemsSource();
            return content;
        }

        private VisualElement GetIconContainer()
        {
            iconTexture2DAnimationInfo =
                new Texture2DAnimationInfo(propertyIcon, castedTarget.spriteSheet)
                    .SetStyleMarginTop(DesignUtils.k_Spacing)
                    .HideSetTextureButton(true);

            var itemsSource = new List<SerializedProperty>();
            iconTextures2DFluidListView = new FluidListView();

            VisualElement content =
                new VisualElement()
                    .AddChild(iconTextures2DFluidListView)
                    .AddChild(iconTexture2DAnimationInfo);

            content.Bind(propertyIcon.serializedObject);

            iconTexture2DAnimationInfo.SetStyleDisplay(propertyIcon.arraySize > 0 ? DisplayStyle.Flex : DisplayStyle.None);

            content.schedule.Execute(() => { iconTexture2DAnimationInfo.reaction?.SetDuration(propertyAnimationDuration.floatValue); });
            animationDurationFloatField.RegisterValueChangedCallback(evt =>
            {
                if (evt?.newValue == null) return;
                iconTexture2DAnimationInfo?.reaction?.SetDuration(evt.newValue);
            });

            //Drag and Drop
            content.RegisterCallback<AttachToPanelEvent>(_ =>
            {
                content.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
                content.RegisterCallback<DragPerformEvent>(OnDragPerformEvent);
            });

            content.RegisterCallback<DetachFromPanelEvent>(_ =>
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
                    string[] paths = AssetDatabase.FindAssets($"t:{nameof(Texture2D)}", new[] { assetPath });
                    isValid = paths.Select(path => AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(path))).Any(sprite => sprite != null);
                }
                if (!isValid) return;
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            }

            void OnDragPerformEvent(DragPerformEvent evt)
            {
                var references =
                    DragAndDrop.objectReferences
                        .Where(item => item != null && item is Texture2D)
                        .OrderBy(item => item.name)
                        .ToList();

                AssetUtils.RemoveSubAssets(castedTarget);
                if (references.Count == 0) //check if it's a folder
                {
                    string folderPath = AssetDatabase.GetAssetPath(DragAndDrop.objectReferences[0]);
                    string[] guids = AssetDatabase.FindAssets($"t:{nameof(Texture2D)}", new[] { folderPath });
                    references.AddRange(guids.Select(guid => AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guid))).OrderBy(item => item.name));
                    propertySpriteSheet.objectReferenceValue = null;
                    iconTextures2DFluidListView.SetStyleDisplay(DisplayStyle.Flex);
                }
                else //not a folder - look for sprite sheets
                {
                    foreach (Texture2D texture in references.OfType<Texture2D>().ToList().Where(texture => texture.IsSpriteSheet()))
                    {
                        propertySpriteSheet.objectReferenceValue = texture;
                        propertySpriteSheet.serializedObject.ApplyModifiedProperties();
                        castedTarget.SetSpriteSheet(texture);
                        iconTexture2DAnimationInfo.spriteSheet = castedTarget.spriteSheet;
                        iconTexture2DAnimationInfo.Update();
                        iconTextures2DFluidListView.SetStyleDisplay(DisplayStyle.None);
                        return;
                    }
                }

                if (references.Count == 0)
                    return;

                propertyIcon.ClearArray();
                foreach (Object reference in references)
                {
                    propertyIcon.InsertArrayElementAtIndex(propertyIcon.arraySize);
                    propertyIcon.GetArrayElementAtIndex(propertyIcon.arraySize - 1).objectReferenceValue = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GetAssetPath(reference));
                }

                propertyIcon.serializedObject.ApplyModifiedProperties();
                iconTexture2DAnimationInfo.Update();
            }

            iconTextures2DFluidListView.SetStyleDisplay(castedTarget.spriteSheet == null ? DisplayStyle.Flex : DisplayStyle.None);
            iconTextures2DFluidListView.listView.selectionType = SelectionType.None;
            iconTextures2DFluidListView.listView.itemsSource = itemsSource;
            iconTextures2DFluidListView.listView.makeItem = () => new ObjectFluidListViewItem(iconTextures2DFluidListView, typeof(Texture2D));
            iconTextures2DFluidListView.listView.bindItem = (element, i) =>
            {
                var item = (ObjectFluidListViewItem)element;
                item.Update(i, itemsSource[i]);
                item.OnRemoveButtonClick += property =>
                {
                    int propertyIndex = 0;
                    for (int j = 0; j < propertyIcon.arraySize; j++)
                    {
                        if (property.propertyPath != propertyIcon.GetArrayElementAtIndex(j).propertyPath)
                            continue;
                        propertyIndex = j;
                        break;
                    }
                    propertyIcon.DeleteArrayElementAtIndex(propertyIndex);
                    propertyIcon.serializedObject.ApplyModifiedProperties();

                    UpdateItemsSource();
                };
            };

            #if UNITY_2021_2_OR_NEWER
            iconTextures2DFluidListView.listView.fixedItemHeight = 24;
            iconTextures2DFluidListView.SetPreferredListHeight((int)iconTextures2DFluidListView.listView.fixedItemHeight * 6);
            #else
            iconTextures2DFluidListView.listView.itemHeight = 24;
            iconTextures2DFluidListView.SetPreferredListHeight(iconTextures2DFluidListView.listView.itemHeight * 6);
            #endif

            iconTextures2DFluidListView.SetDynamicListHeight(false);
            iconTextures2DFluidListView.UseSmallEmptyListPlaceholder(true);
            iconTextures2DFluidListView.SetListTitle("Prefab Preview");
            iconTextures2DFluidListView.SetListDescription("List of Texture2Ds used to show a preview for this prefab");

            //ADD ITEM BUTTON (plus button)
            iconTextures2DFluidListView.AddNewItemButtonCallback += () =>
            {
                propertyIcon.InsertArrayElementAtIndex(0);
                propertyIcon.GetArrayElementAtIndex(0).objectReferenceValue = null;
                propertyIcon.serializedObject.ApplyModifiedProperties();
                UpdateItemsSource();
            };

            var sortAzButton =
                FluidListView.Buttons.sortAzButton
                    .SetOnClick(() =>
                    {
                        Undo.RecordObject(propertyIcon.serializedObject.targetObject, "Sort Az");
                        castedTarget.SortSpritesAz();
                        propertyIcon.serializedObject.UpdateIfRequiredOrScript();
                        propertyIcon.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                        UpdateItemsSource();
                        UpdateAnimationInfo();
                    });

            var sortZaButton =
                FluidListView.Buttons.sortZaButton
                    .SetOnClick(() =>
                    {
                        Undo.RecordObject(propertyIcon.serializedObject.targetObject, "Sort Za");
                        castedTarget.SortSpritesZa();
                        propertyIcon.serializedObject.UpdateIfRequiredOrScript();
                        propertyIcon.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                        UpdateItemsSource();
                        UpdateAnimationInfo();
                    });

            var clearButton =
                FluidListView.Buttons.clearButton
                    .SetOnClick(() =>
                    {
                        propertyIcon.ClearArray();
                        propertyIcon.serializedObject.ApplyModifiedProperties();
                    });

            iconTextures2DFluidListView.AddToolbarElement(sortAzButton);
            iconTextures2DFluidListView.AddToolbarElement(sortZaButton);
            iconTextures2DFluidListView.AddToolbarElement(DesignUtils.flexibleSpace);
            iconTextures2DFluidListView.AddToolbarElement(clearButton);

            int arraySize = propertyIcon.arraySize;
            iconTextures2DFluidListView.schedule.Execute(() =>
            {
                if (propertyIcon.arraySize == arraySize) return;
                arraySize = propertyIcon.arraySize;
                UpdateItemsSource();
                UpdateAnimationInfo();

            }).Every(100);

            void UpdateAnimationInfo()
            {
                if (propertyIcon.arraySize == 0)
                {
                    iconTexture2DAnimationInfo.SetStyleDisplay(DisplayStyle.None);
                    return;
                }

                bool updateAnimationInfo = false;
                for (int i = 0; i < propertyIcon.arraySize; i++)
                {
                    if (propertyIcon.GetArrayElementAtIndex(i).objectReferenceValue == null)
                        continue;
                    updateAnimationInfo = true;
                    break;
                }

                if (!updateAnimationInfo)
                    return;

                iconTexture2DAnimationInfo.SetStyleDisplay(DisplayStyle.Flex);
                iconTexture2DAnimationInfo.Update();
            }

            void UpdateItemsSource()
            {
                itemsSource.Clear();
                if (propertyIcon == null) return;
                if (propertyIcon.arraySize == 0) return;
                for (int i = 0; i < propertyIcon.arraySize; i++)
                {
                    SerializedProperty arrayElementAtIndex = propertyIcon.GetArrayElementAtIndex(i);
                    if (arrayElementAtIndex == null) continue;
                    itemsSource.Add(arrayElementAtIndex);
                }
                iconTextures2DFluidListView?.Update();
            }

            UpdateItemsSource();
            return content;
        }

        private void Compose()
        {
            headerToolbar = DesignUtils.row
                .SetStyleMargins(50, -4, DesignUtils.k_Spacing2X, DesignUtils.k_Spacing2X)
                .AddChild(DesignUtils.flexibleSpace)
                .AddChild(saveButton)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(renameAssetButton);

            iconContainer = GetIconContainer();
            tagsContainer = GetTagsContainer();

            root
                .AddChild(componentHeader)
                .AddChild(headerToolbar)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(prefabFluidField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(prefabInfoFluidField)
                .AddChild(DesignUtils.endOfLineBlock)
                .AddChild(iconContainer)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(colorizeSwitch.SetStyleMarginRight(DesignUtils.k_Spacing))
                        .AddChild(animationDurationFluidField)
                )
                .AddChild(DesignUtils.endOfLineBlock)
                .AddChild(tagsContainer)
                .AddChild(DesignUtils.endOfLineBlock);
        }

        /// <summary> Used by the UIMenu Generator to use this editor as a settings panel for bulk items creation </summary>
        public void UseAsSettings()
        {
            componentHeader.SetStyleDisplay(DisplayStyle.None);
            headerToolbar.SetStyleDisplay(DisplayStyle.None);
            prefabFluidField.SetStyleDisplay(DisplayStyle.None);
            iconContainer.SetStyleDisplay(DisplayStyle.None);
            colorizeSwitch.SetStyleDisplay(DisplayStyle.None);
            prefabNameContainer.SetStyleDisplay(DisplayStyle.None);
        }
    }
}
