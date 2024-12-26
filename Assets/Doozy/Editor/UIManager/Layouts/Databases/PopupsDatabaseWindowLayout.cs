// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.Common.Layouts;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Interfaces;
using Doozy.Editor.UIElements;
using Doozy.Runtime.Common;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Layouts.Databases
{
    public sealed class PopupsDatabaseWindowLayout : FluidWindowLayout, IDashboardDatabaseWindowLayout
    {
        public int order => 0;
        
        public override string layoutName => "Popups";
        public override List<Texture2D> animatedIconTextures => EditorSpriteSheets.UIManager.Icons.UIPopupDatabase;
        public override Color accentColor => EditorColors.UIManager.UIComponent;
        public override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.UIComponent;
        
        private UnityEngine.Object targetObject => null;
        private UnityAction onUpdateCallback => null;
        private static List<UIPopupLink> database => UIPopupDatabase.instance.database;
        private string groupTypeName => "Popups";

        private static bool databaseIsEmpty => database.Count == 0;

        private FluidListView fluidListView { get; set; }

        private bool initialized { get; set; }

        private FluidButton refreshDatabaseButton { get; set; }

        public PopupsDatabaseWindowLayout()
        {
            AddHeader("Popups Database", "UIPopup Links", EditorSpriteSheets.UIManager.Icons.UIPopupDatabase);
            sideMenu
                .SetMenuLevel(FluidSideMenu.MenuLevel.Level_2)
                .IsCollapsable(false)
                .SetAccentColor(selectableAccentColor);
            Initialize();
        }

        private void Initialize()
        {
            if (!initialized)
            {
                //SIDE MENU - ToolbarContainer - Refresh Database button
                refreshDatabaseButton = DesignUtils.Buttons.RefreshDatabase
                (
                    "Refresh Database",
                    $"Search for all the '{nameof(UIPopupLink)}' assets in the project and update the database.",
                    selectableAccentColor,
                    () =>
                    {
                        UIPopupDatabase.instance.RefreshDatabase();
                        schedule.Execute(UpdateDatabase);
                    });

                sideMenu.toolbarContainer
                    .SetStyleDisplay(DisplayStyle.Flex)
                    .AddChild(refreshDatabaseButton);

                fluidListView = new FluidListView();
                fluidListView.listView.selectionType = SelectionType.None;
                fluidListView.listView.makeItem = () => PrefabLinkDatabaseItemRow.Get();
                fluidListView.listView.bindItem = (element, i) =>
                    ((PrefabLinkDatabaseItemRow)element)
                    .SetTarget(database[i])
                    .SetDeleteHandler(ItemDeleteHandler);

                #if UNITY_2021_2_OR_NEWER
                fluidListView.listView.fixedItemHeight = 30;
                #else
                fluidListView.listView.itemHeight = 30;
                #endif

                fluidListView
                    .SetItemsSource(database)
                    .SetDynamicListHeight(true)
                    .HideAddNewItemButton(); //HIDE ADD NEW ITEM BUTTON (plus button)


                Undo.undoRedoPerformed -= UndoRedoPerformed;
                Undo.undoRedoPerformed += UndoRedoPerformed;
                initialized = true;
            }

            UpdateDatabase();
        }

        private void ItemDeleteHandler(PrefabLink targetItem)
        {
            if (!EditorUtility.DisplayDialog
                (
                    "Confirmation",
                    $"Are you sure you want to remove the '{targetItem.prefabName}' from the database and delete the '{targetItem.name}' asset file?",
                    "Yes",
                    "Cancel"
                )
               ) return;

            Undo.RecordObject(targetObject, "Remove Item");
            UIPopupDatabase.instance.Delete((UIPopupLink)targetItem);
            EditorUtility.SetDirty(targetObject);
            AssetDatabase.SaveAssetIfDirty(targetObject);
            onUpdateCallback?.Invoke();
            UpdateDatabase();
        }

        private void UndoRedoPerformed()
        {
            UpdateDatabase();
        }

        private void UpdateDatabase()
        {
            UIPopupDatabase.instance.Sort();
            onUpdateCallback?.Invoke();

            if (ShowEmptyDatabase())
                return;

            content
                .RecycleAndClear()
                .AddChild(fluidListView);
            fluidListView.Update();
        }

        private bool ShowEmptyDatabase()
        {
            if (!databaseIsEmpty)
                return false; //database is NOT empty

            content
                .RecycleAndClear()
                .AddChild
                (
                    new VisualElement()
                        .SetName("Empty Database - Placeholder Container")
                        .SetStyleFlexGrow(1)
                        .SetStyleJustifyContent(Justify.Center)
                        .AddChild(FluidPlaceholder.Get("Empty Database", EditorSpriteSheets.EditorUI.Placeholders.EmptyDatabase).Play())
                );

            return true; // database is empty
        }
    }
}
