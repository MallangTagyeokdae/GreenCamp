// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.EditorUI.Windows.Internal;
using Doozy.Editor.Reactor.Internal;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Colors;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Extensions;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.UIMenu
{
    public class UIMenuWindow : FluidWindow<UIMenuWindow>
    {
        private const string WINDOW_TITLE = "UI Menu";
        public const string k_WindowMenuPath = "Tools/Doozy/UI Menu";

        [MenuItem(k_WindowMenuPath, priority = -1000)]
        public static void Open() => InternalOpenWindow(WINDOW_TITLE);

        [MenuItem("Tools/Doozy/Refresh/UIMenu", priority = -450)]
        public static void RefreshDatabase()
        {
            UIMenuItemsDatabase.instance.RefreshDatabase();
            if (isOpen) instance.UpdateItems();
        }
        
        private static IEnumerable<Texture2D> uiMenuHeaderTextures => EditorSpriteSheets.UIManager.UIMenu.UIMenuHeader;

        private string typeSideMenuWidthKey => EditorPrefsKey($"{nameof(typeSideMenu)}.{nameof(typeSideMenu.customWidth)}");
        private string categorySideMenuWidthKey => EditorPrefsKey($"{nameof(categorySideMenu)}.{nameof(categorySideMenu.customWidth)}");
        private string selectedTypeKey => EditorPrefsKey($"{nameof(selectedType)}");
        private string selectedCategoryKey => EditorPrefsKey($"{nameof(selectedCategory)}");

        private string selectedType { get; set; }
        private string selectedCategory { get; set; }

        private Image uiMenuImage { get; set; }
        private Texture2DReaction uiMenuImageReaction { get; set; }

        private Image itemSizeIcon { get; set; }
        private Texture2DReaction itemSizeIconReaction { get; set; }
        private Label itemSizeLabel { get; set; }
        private VisualElement itemSizeContainer { get; set; }
        private SliderInt itemSizeSlider { get; set; }

        private ScrollView contentScrollView { get; set; }
        private VisualElement itemsContainer { get; set; }

        private FluidToggleButtonTab autoSelectTab { get; set; }
        private FluidToggleButtonTab defaultInstantiateModeTab { get; set; }
        private FluidToggleButtonTab cloneInstantiateModeTab { get; set; }
        private FluidToggleButtonTab linkInstantiateModeTab { get; set; }

        private FluidButton refreshButton { get; set; }
        private FluidButton regenerateButton { get; set; }

        private TemplateContainer templateContainer { get; set; }
        private VisualElement layoutContainer { get; set; }
        private VisualElement sideMenuContainer { get; set; }
        private VisualElement contentContainer { get; set; }

        private FluidSideMenu typeSideMenu { get; set; }
        private FluidResizer typeSideMenuResizer { get; set; }
        private FluidSideMenu categorySideMenu { get; set; }
        private FluidResizer categorySideMenuResizer { get; set; }

        private FluidPlaceholder emptyPlaceholder { get; set; }

        protected override void OnEnable()
        {
            base.OnEnable();
            Undo.undoRedoPerformed -= UndoRedoPerformed;
            Undo.undoRedoPerformed += UndoRedoPerformed;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Undo.undoRedoPerformed -= UndoRedoPerformed;
            AssetDatabase.SaveAssetIfDirty(UIMenuSettings.instance);
        }

        private void UndoRedoPerformed()
        {
            UpdateBottomTabs(true);
        }

        protected override void CreateGUI()
        {
            Initialize();
            Compose();
        }

        private void Initialize()
        {
            root
                .RecycleAndClear()
                .Add(templateContainer = EditorLayouts.UIManager.UIMenuWindow.CloneTree());

            templateContainer
                .SetStyleFlexGrow(1)
                .AddStyle(EditorUI.EditorStyles.UIManager.UIMenuWindow);

            layoutContainer = templateContainer.Q<VisualElement>(nameof(layoutContainer));
            sideMenuContainer = layoutContainer.Q<VisualElement>(nameof(sideMenuContainer));
            contentContainer = layoutContainer.Q<VisualElement>(nameof(contentContainer));

            typeSideMenu =
                new FluidSideMenu()
                    .SetMenuLevel(FluidSideMenu.MenuLevel.Level_0)
                    .IsCollapsable(true)
                    .SetCustomWidth(EditorPrefs.GetInt(typeSideMenuWidthKey, 200))
                    .AddSearch();

            typeSideMenu.searchBox
                .ClearSearchables()
                .ClearConnectedSearchBoxes()
                .AddSearchable(new Finder(this, typeSideMenu.searchBox))
                .SetMinimumNumberOfCharactersToExecuteSearch(3);

            typeSideMenuResizer = new FluidResizer(FluidResizer.Position.Right);
            typeSideMenuResizer.onPointerMoveEvent += evt =>
            {
                if (typeSideMenu.isCollapsed) return;
                typeSideMenu.SetCustomWidth((int)(typeSideMenu.customWidth + evt.deltaPosition.x));
            };
            typeSideMenuResizer.onPointerUp += evt =>
            {
                if (typeSideMenu.isCollapsed) return;
                EditorPrefs.SetInt(typeSideMenuWidthKey, typeSideMenu.customWidth);
            };

            categorySideMenu =
                new FluidSideMenu()
                    .SetMenuLevel(FluidSideMenu.MenuLevel.Level_1)
                    .IsCollapsable(false)
                    .SetCustomWidth(EditorPrefs.GetInt(categorySideMenuWidthKey, 200));

            categorySideMenuResizer = new FluidResizer(FluidResizer.Position.Right);
            categorySideMenuResizer.onPointerMoveEvent += evt => categorySideMenu.SetCustomWidth((int)(categorySideMenu.customWidth + evt.deltaPosition.x));
            categorySideMenuResizer.onPointerUp += evt => EditorPrefs.SetInt(categorySideMenuWidthKey, categorySideMenu.customWidth);

            sideMenuContainer.Add
            (
                DesignUtils.row
                    .AddChild(typeSideMenu)
                    .AddChild(typeSideMenuResizer)
                    .AddChild(categorySideMenu)
                    .AddChild(categorySideMenuResizer)
            );

            contentScrollView = new ScrollView().SetStyleFlexGrow(1);
            contentContainer
                .AddChild(contentScrollView)
                .AddChild
                (
                    DesignUtils.NewFieldNameLabel("Right click on items to maximize")
                        .SetStyleTextAlign(TextAnchor.MiddleRight)
                        .SetStylePaddingRight(20)
                        .SetStyleMarginTop(-11)
                        .SetStyleTop(-8)
                        .SetPickingMode(PickingMode.Ignore)
                        .SetStyleColor(DesignUtils.fieldNameTextColor.WithAlpha(0.5f))
                );

            itemsContainer =
                new VisualElement()
                    .SetName("Items Container")
                    .SetStyleFlexDirection(FlexDirection.Row)
                    .SetStyleFlexWrap(Wrap.Wrap)
                    .SetStyleFlexGrow(1)
                    .SetStyleFlexShrink(0)
                    .SetStylePadding(DesignUtils.k_Spacing3X);

            contentScrollView.contentContainer.AddChild(itemsContainer);

            InitializeUIMenuHeader();
            InitializeImageResizeSlider();
            InitializeBottomToolbar();
            UpdateItems();
        }

        private void InitializeUIMenuHeader()
        {
            uiMenuImage =
                new Image()
                    .SetStyleBackgroundImageTintColor(EditorColors.Default.WindowHeaderTitle)
                    .SetStyleFlexShrink(0)
                    .SetStyleMarginLeft(DesignUtils.k_Spacing3X);

            uiMenuImageReaction =
                uiMenuImage
                    .GetTexture2DReaction(uiMenuHeaderTextures)
                    .SetEditorHeartbeat();

            uiMenuImageReaction.Play();

            Texture2D uiMenuHeaderFirstTexture = uiMenuHeaderTextures.First();
            float headerWidth = uiMenuHeaderFirstTexture.width;
            float headerHeight = uiMenuHeaderFirstTexture.height;
            uiMenuImage.SetStyleSize(headerWidth * 0.6f, headerHeight * 0.6f);

            uiMenuImage.RegisterCallback<PointerEnterEvent>(evt => uiMenuImageReaction?.Play());
            uiMenuImage.AddManipulator(new Clickable(() => uiMenuImageReaction?.Play()));
        }

        private void InitializeImageResizeSlider()
        {
            itemSizeIcon =
                new Image()
                    .SetName("Item Size Icon")
                    .ResetLayout()
                    .SetStyleBackgroundImageTintColor(EditorColors.Default.Icon)
                    .SetStyleSize(32);

            itemSizeIconReaction =
                itemSizeIcon
                    .GetTexture2DReaction(EditorSpriteSheets.EditorUI.Icons.Zoom)
                    .SetEditorHeartbeat()
                    .SetDuration(0.6f);

            itemSizeLabel =
                DesignUtils.fieldLabel
                    .SetName("Item Size Label")
                    .SetStyleWidth(28)
                    .SetStyleMarginLeft(6)
                    .SetStyleTop(2);

            void UpdateLabel(int value)
            {
                itemSizeLabel.text = $"{value}px";
            }

            itemSizeSlider =
                new SliderInt(UIMenuSettings.k_MinItemSize, UIMenuSettings.k_MaxItemSize)
                    .SetName("Item Size Slider")
                    .ResetLayout()
                    .SetStyleWidth(88);

            itemSizeSlider.value = UIMenuSettings.instance.itemSize;
            UpdateLabel(itemSizeSlider.value);

            itemSizeSlider.RegisterValueChangedCallback(evt =>
            {
                if (evt?.newValue == null) return;

                const float snapInterval = 4;
                float value = evt.newValue;
                value = Mathf.Round(value / snapInterval) * snapInterval;
                itemSizeSlider.SetValueWithoutNotify((int)value);

                UIMenuSettings.instance.itemSize = (int)value;
                EditorUtility.SetDirty(UIMenuSettings.instance);

                foreach (UIMenuItemButton itemButton in itemButtons)
                    itemButton.Resize(itemSizeSlider.value);

                UpdateLabel(itemSizeSlider.value);
            });

            itemSizeContainer =
                DesignUtils.row
                    .SetName("Item Size")
                    .SetTooltip("Resize Items")
                    .SetStyleFlexShrink(0)
                    .SetStyleFlexGrow(0)
                    .SetStyleAlignItems(Align.Center)
                    .AddChild(itemSizeIcon)
                    .AddChild(itemSizeSlider)
                    .AddChild(itemSizeLabel);


            itemSizeContainer.RegisterCallback<PointerEnterEvent>(evt => itemSizeIconReaction?.Play());
        }

        private void InitializeBottomToolbar()
        {
            regenerateButton =
                FluidButton.Get()
                    .SetTooltip("Regenerate the UI Menu")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Reset)
                    .SetElementSize(ElementSize.Tiny)
                    .SetOnClick(() =>
                    {
                        UIMenuItemsDatabase.instance.RefreshDatabase();
                        UpdateItems();
                    });

            refreshButton =
                FluidButton.Get()
                    .SetTooltip("Refresh the UI Menu")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Refresh)
                    .SetElementSize(ElementSize.Tiny)
                    .SetOnClick(() =>
                    {
                        string prefabTypeName = typeSideMenu.buttons.Find(b => b.isOn).buttonLabel.text;
                        string categoryName = categorySideMenu.buttons.Find(b => b.isOn).buttonLabel.text;
                        LoadCategoryItems(prefabTypeName, categoryName);
                    });

            FluidToggleButtonTab GetInstantiateTab(string labelText) =>
                FluidToggleButtonTab.Get()
                    .SetLabelText(labelText)
                    .SetContainerColorOff(DesignUtils.tabButtonColorOff)
                    .SetToggleAccentColor(EditorSelectableColors.UIManager.UIComponent);

            autoSelectTab =
                GetInstantiateTab("Auto Select")
                    .SetTooltip("Auto select newly created objects")
                    .SetOnClick(() =>
                    {
                        Undo.RecordObject(UIMenuSettings.instance, "Update Auto Select");
                        UIMenuSettings.instance.SelectNewlyCreatedObjects = !UIMenuSettings.instance.SelectNewlyCreatedObjects;
                        EditorUtility.SetDirty(UIMenuSettings.instance);
                        AssetDatabase.SaveAssetIfDirty(UIMenuSettings.instance);
                        UpdateBottomTabs(true);
                    });

            cloneInstantiateModeTab =
                GetInstantiateTab(PrefabInstantiateModeSetting.Clone.ToString())
                    .SetTooltip("Clone Instantiate Mode\n\nIgnore the default menu item behaviour when instantiating its target object and create a clone")
                    .SetTabPosition(TabPosition.TabOnLeft)
                    .SetOnClick(() =>
                    {
                        Undo.RecordObject(UIMenuSettings.instance, "Update InstantiateMode");
                        UIMenuSettings.instance.InstantiateMode = PrefabInstantiateModeSetting.Clone;
                        EditorUtility.SetDirty(UIMenuSettings.instance);
                        AssetDatabase.SaveAssetIfDirty(UIMenuSettings.instance);
                        UpdateBottomTabs(true);
                    });

            defaultInstantiateModeTab =
                GetInstantiateTab(PrefabInstantiateModeSetting.Default.ToString())
                    .SetTooltip("Default Instantiate Mode\n\nUse the default menu item behaviour when instantiating its target object")
                    .SetTabPosition(TabPosition.TabInCenter)
                    .SetOnClick(() =>
                    {
                        Undo.RecordObject(UIMenuSettings.instance, "Update InstantiateMode");
                        UIMenuSettings.instance.InstantiateMode = PrefabInstantiateModeSetting.Default;
                        EditorUtility.SetDirty(UIMenuSettings.instance);
                        AssetDatabase.SaveAssetIfDirty(UIMenuSettings.instance);
                        UpdateBottomTabs(true);
                    });


            linkInstantiateModeTab =
                GetInstantiateTab(PrefabInstantiateModeSetting.Link.ToString())
                    .SetTooltip("Link Instantiate Mode\n\nIgnore the default menu item behaviour when instantiating its target object and create a prefab link")
                    .SetTabPosition(TabPosition.TabOnRight)
                    .SetOnClick(() =>
                    {
                        Undo.RecordObject(UIMenuSettings.instance, "Update InstantiateMode");
                        UIMenuSettings.instance.InstantiateMode = PrefabInstantiateModeSetting.Link;
                        EditorUtility.SetDirty(UIMenuSettings.instance);
                        AssetDatabase.SaveAssetIfDirty(UIMenuSettings.instance);
                        UpdateBottomTabs(true);
                    });


            UpdateBottomTabs(false);
        }

        private static EditorSelectableColorInfo menuSelectableColor => EditorSelectableColors.Default.UnityThemeInversed;
        private List<UIMenuItemButton> itemButtons { get; set; } = new List<UIMenuItemButton>();

        private void ClearItems()
        {
            itemButtons.Clear();
            itemsContainer?.RecycleAndClear();
        }

        public void UpdateItems()
        {
            ClearItems();
            sideMenuContainer.SetStyleDisplay(DisplayStyle.Flex);

            //Check - Database is empty?
            if (UIMenuItemsDatabase.instance.isEmpty)
            {
                sideMenuContainer.SetStyleDisplay(DisplayStyle.None);
                emptyPlaceholder ??= FluidPlaceholder.Get("The menu is empty...", FluidPlaceholder.defaultTextures);
                contentContainer.Insert(0, emptyPlaceholder.Play());
                return;
            }
            emptyPlaceholder?.Recycle();

            typeSideMenu.buttons.ForEach(b => b?.Recycle());
            typeSideMenu.buttons.Clear();

            foreach (string prefabTypeName in UIMenuItemsDatabase.GetPrefabTypes())
            {
                FluidToggleButtonTab typeButton = typeSideMenu.AddButton(prefabTypeName, menuSelectableColor);

                string cleanPrefabTypeName = prefabTypeName.RemoveWhitespaces().RemoveAllSpecialCharacters();
                bool knownType = false;
                foreach (EditorSpriteSheets.UIManager.UIMenu.SpriteSheetName sheetName in Enum.GetValues(typeof(EditorSpriteSheets.UIManager.UIMenu.SpriteSheetName)))
                {
                    if (!sheetName.ToString().Equals(cleanPrefabTypeName))
                        continue;
                    typeButton.SetIcon(EditorSpriteSheets.UIManager.UIMenu.GetTextures(sheetName));
                    typeButton.iconReaction.SetDuration(0.8f);
                    knownType = true;
                }
                if (!knownType)
                {
                    typeButton.SetIcon(EditorSpriteSheets.EditorUI.Icons.Prefab);
                }

                typeButton.OnValueChanged += itemType =>
                {
                    if (!itemType.newValue) return;
                    categorySideMenu.buttons.ForEach(b => b?.Recycle());
                    categorySideMenu.buttons.Clear();

                    foreach (string categoryName in UIMenuItemsDatabase.GetCategories(prefabTypeName))
                    {
                        FluidToggleButtonTab categoryButton = categorySideMenu.AddButton(categoryName, menuSelectableColor);
                        categoryButton.OnValueChanged += itemCategory =>
                        {
                            if (!itemCategory.newValue) return;
                            if (typeSideMenu.searchBox.isSearching)
                            {
                                typeSideMenu.searchBox.ClearSearch();
                                root.schedule.Execute(() =>
                                {
                                    categoryButton.SetIsOn(true);
                                });
                                return;
                            }
                            LoadCategoryItems(prefabTypeName, categoryName);
                        };
                    }

                    categorySideMenu.buttons.First()?.SetIsOn(true);
                };
            }

            typeSideMenu.buttons.First()?.SetIsOn(true);
        }

        public void LoadCategoryItems(string prefabTypeName, string categoryName)
        {
            selectedType = prefabTypeName;
            selectedCategory = categoryName;
            EditorPrefs.SetString(selectedTypeKey, selectedType);
            EditorPrefs.SetString(selectedCategoryKey, selectedCategory);
            LoadItems(UIMenuItemsDatabase.GetCategoryMenuItems(prefabTypeName, categoryName));
        }

        public void LoadItems(IEnumerable<UIMenuItem> items)
        {
            ClearItems();
            foreach (UIMenuItem item in items)
            {
                var itemButton = new UIMenuItemButton();
                itemButton.SetUIMenuItem(item);
                itemButton.Resize(UIMenuSettings.instance.itemSize);
                itemButtons.Add(itemButton);
                itemsContainer.AddChild(itemButton);
            }

            // foreach (UIMenuItemButton itemButton in items.Select(item => new UIMenuItemButton().SetUIMenuItem(item)))
            // {
            //     itemButton.Resize(UIMenuSettings.instance.itemSize);
            //     itemButtons.Add(itemButton);
            //     itemsContainer.AddChild(itemButton);
            // }
        }

        private void UpdateBottomTabs(bool animateChange)
        {
            root.schedule.Execute(() =>
            {
                UIMenuSettings settings = UIMenuSettings.instance;
                autoSelectTab?.SetIsOn(settings.SelectNewlyCreatedObjects, animateChange);
                cloneInstantiateModeTab?.SetIsOn(settings.InstantiateMode == PrefabInstantiateModeSetting.Clone, animateChange);
                defaultInstantiateModeTab?.SetIsOn(settings.InstantiateMode == PrefabInstantiateModeSetting.Default, animateChange);
                linkInstantiateModeTab?.SetIsOn(settings.InstantiateMode == PrefabInstantiateModeSetting.Link, animateChange);
            });
        }

        private void Compose()
        {
            root
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleFlexShrink(0)
                        .SetStyleFlexGrow(0)
                        .SetStyleAlignItems(Align.Center)
                        .SetStyleBackgroundColor(EditorColors.Default.BoxBackground)
                        .SetStylePadding(DesignUtils.k_Spacing2X)
                        .AddChild(regenerateButton)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(refreshButton)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(autoSelectTab)
                        .AddChild(DesignUtils.flexibleSpace)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(cloneInstantiateModeTab)
                        .AddSpace(2, 0)
                        .AddChild(defaultInstantiateModeTab)
                        .AddSpace(2, 0)
                        .AddChild(linkInstantiateModeTab)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(DesignUtils.flexibleSpace)
                        .AddChild(itemSizeContainer)
                )
                ;
        }

        private class Finder : ISearchable
        {
            private UIMenuWindow target { get; }
            private FluidSearchBox searchBox { get; }
            private List<UIMenuItem> searchItems { get; set; } = new List<UIMenuItem>();

            public Finder(UIMenuWindow target, FluidSearchBox searchBox)
            {
                this.target = target;
                this.searchBox = searchBox;
            }

            public bool isSearching => searchBox.isSearching;
            public string searchPattern => searchBox.searchPattern;
            public bool hasSearchResults { get; private set; }
            public VisualElement searchResults => new VisualElement();

            public void ClearSearch()
            {
                hasSearchResults = false;
                Search(string.Empty);
                target.contentScrollView.contentViewport.SetStyleJustifyContent(Justify.FlexStart);
                target.LoadItems(UIMenuItemsDatabase.GetCategoryMenuItems(target.selectedType, target.selectedCategory));
            }

            public void Search(string pattern)
            {
                searchBox.searchTextField.value = pattern;
                UpdateSearchResults();
                hasSearchResults = searchItems.Count > 0;
            }

            public void UpdateSearchResults()
            {
                searchItems.Clear();
                target.contentScrollView.contentViewport.SetStyleJustifyContent(Justify.Center);

                //SEARCH PATTERN length
                int searchPatternLength = searchBox.searchPattern.Length;
                //MIN SEARCH PATTERN LENGTH CONSTRAINT
                int minimumSearchLength = searchBox.minimumNumberOfCharactersToExecuteTheSearch;

                //UPDATE SEARCH VISUAL - tell the user how many characters are needed to start the search
                if (isSearching && searchPatternLength < minimumSearchLength)
                {
                    //CALCULATE how many characters does the user need to add to the search box to the search to start
                    int numberOfCharactersNeeded = minimumSearchLength - searchPatternLength;
                    //SHOW SEARCH VISUAL <<< with the info text for the user
                    target.ClearItems();
                    target.itemsContainer.Add(searchBox.EmptySearchPlaceholderElement($"Add {numberOfCharactersNeeded} more character{(numberOfCharactersNeeded != 1 ? "s" : "")}..."));
                    //STOP
                    return;
                }

                searchItems = UIMenuItemsDatabase.GetItems();
                for (int i = searchItems.Count - 1; i >= 0; i--)
                {
                    if (!Regex.IsMatch(searchItems[i].cleanPrefabName, searchBox.searchPattern.RemoveWhitespaces(), RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace))
                        searchItems.RemoveAt(i);
                }

                hasSearchResults = searchItems.Count > 0;

                //CHECK FLAG for SEARCH RESULTS
                if (!hasSearchResults)
                {
                    //NO SEARCH RESULTS <<< show search visual
                    target.ClearItems();
                    target.itemsContainer.Add(searchBox.EmptySearchPlaceholderElement());
                    //STOP
                    return;
                }

                target.contentScrollView.contentViewport.SetStyleJustifyContent(Justify.FlexStart);
                target.LoadItems
                (
                    isSearching
                        ? searchItems
                        : UIMenuItemsDatabase.GetCategoryMenuItems(target.selectedType, target.selectedCategory)
                );
            }
        }
    }
}
