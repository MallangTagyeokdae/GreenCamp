// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Mody;
using Doozy.Editor.Mody.Components;
using Doozy.Editor.Reactor.Components;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Modules;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Editors.Modules
{
    [CustomEditor(typeof(AnimatorModule), true)]
    public class AnimatorModuleEditor : ModyModuleEditor<AnimatorModule>
    {
        private SerializedProperty propertyAnimators { get; set; }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyAnimators = serializedObject.FindProperty(nameof(AnimatorModule.Animators));
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(AnimatorModule.k_DefaultModuleName)
                .SetSecondaryIcon(EditorSpriteSheets.Reactor.Icons.UIAnimator)
                .AddManualButton()
                .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.UIManager.Modules.AnimatorModule.html")
                .AddYouTubeButton();
        }

        protected override void InitializeSettings()
        {
            base.InitializeSettings();

            settingsAnimatedContainer.SetOnShowCallback(() =>
            {
                var actionsDrawer =
                    new ModyActionsDrawer();

                actionsDrawer.schedule.Execute(() => actionsDrawer.Update());

                VisualElement actionsContainer =
                    new VisualElement().SetName("Actions Container");

                void AddActionToDrawer(ModyActionsDrawerItem item)
                {
                    actionsDrawer.AddItem(item);
                    actionsContainer.AddChild(item.animatedContainer);
                }

                //MODULE ACTIONS
                AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AnimatorModule.PlayForward))));
                AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AnimatorModule.PlayReverse))));
                AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AnimatorModule.Stop))));
                AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AnimatorModule.Finish))));
                AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AnimatorModule.Reverse))));
                AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AnimatorModule.Rewind))));
                AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AnimatorModule.Pause))));
                AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AnimatorModule.Resume))));
                AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AnimatorModule.SetProgressAt))));
                AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AnimatorModule.SetProgressAtZero))));
                AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AnimatorModule.SetProgressAtOne))));
                AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AnimatorModule.PlayToProgress))));
                AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AnimatorModule.PlayFromProgress))));
                AddActionToDrawer(new ModyActionsDrawerItem(serializedObject.FindProperty(nameof(AnimatorModule.UpdateValues))));

                settingsAnimatedContainer
                    .AddContent(AnimatorsListView())
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(actionsDrawer)
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(actionsContainer)
                    .Bind(serializedObject);
            });
        }


        private FluidListView AnimatorsListView()
        {
            var itemsSource = new List<SerializedProperty>();
            SerializedProperty arrayProperty = propertyAnimators;
            var flv = new FluidListView();
            const string animationName = "Animator";
            flv
                // .SetListTitle($"{uiAnimationName}s")
                .SetListDescription($"List of {animationName}s controlled by this module")
                .UseSmallEmptyListPlaceholder(true)
                .HideFooter(true)
                .ShowItemIndex(false);

            flv.emptyListPlaceholder.SetIcon(EditorSpriteSheets.EditorUI.Placeholders.EmptyListViewSmall);
            flv.listView.selectionType = SelectionType.None;
            flv.listView.itemsSource = itemsSource;
            flv.listView.makeItem = () => new AnimatorFluidListViewItem(flv).SetStylePaddingLeft(DesignUtils.k_Spacing);
            flv.listView.bindItem = (element, i) =>
            {
                var item = (AnimatorFluidListViewItem)element;
                item.Update(i, itemsSource[i]);
                item.OnRemoveButtonClick = property =>
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
            flv.listView.fixedItemHeight = 30;
            flv.SetPreferredListHeight((int)flv.listView.fixedItemHeight * 5);
            #else
            flv.listView.itemHeight = 30;
            flv.SetPreferredListHeight(flv.listView.itemHeight * 5);
            #endif

            flv.SetDynamicListHeight(false);

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
                if (arrayProperty == null) return;
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

            return flv;
        }
    }
}
