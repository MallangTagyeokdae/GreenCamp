// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Runtime.Common;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Common.Layouts
{
    public class CategoryNameItemCategoryRow : VisualElement
    {
        public TemplateContainer templateContainer { get; }
        public VisualElement layoutContainer { get; }
        public VisualElement leftContainer { get; }
        public VisualElement middleContainer { get; }
        public Label nameLabel { get; }
        public TextField nameTextField { get; }
        public VisualElement rightContainer { get; }

        public static Font font => EditorFonts.Ubuntu.Regular;
        private static Color textColor => EditorColors.Default.TextDescription;
        
        public FluidToggleButtonTab buttonRename { get; }
        public FluidButton buttonSave { get; }
        public FluidButton buttonCancel { get; }
        public FluidButton buttonRemove { get; }
        
        public string target { get; private set; }
        
        public UnityAction<string> removeHandler { get; set; }
        public Func<string, string, bool> saveHandler { get; set; }
        
        public CategoryNameItemCategoryRow()
        {
            this.SetStyleFlexShrink(0);
            
            Add(templateContainer = EditorLayouts.Common.CategoryNameItemCategoryRow.CloneTree());

            templateContainer
                .SetStyleFlexGrow(1)
                .AddStyle(EditorStyles.Common.CategoryNameItemCategoryRow);

            layoutContainer = templateContainer.Q<VisualElement>(nameof(layoutContainer));
            leftContainer = layoutContainer.Q<VisualElement>(nameof(leftContainer));
            middleContainer = layoutContainer.Q<VisualElement>(nameof(middleContainer));
            nameLabel = middleContainer.Q<Label>(nameof(nameLabel));
            nameTextField = middleContainer.Q<TextField>(nameof(nameTextField));
            rightContainer = layoutContainer.Q<VisualElement>(nameof(rightContainer));

            nameLabel.SetStyleColor(textColor);
            nameLabel.SetStyleUnityFont(font);

            buttonRename = NewButtonRename();
            buttonSave = NewButtonSave().SetStyleDisplay(DisplayStyle.None);
            buttonCancel = NewButtonCancel().SetStyleDisplay(DisplayStyle.None);
            buttonRemove = NewButtonRemove();
            
            buttonRename.SetOnValueChanged(change =>
            {
                if (change.newValue)
                {
                    nameLabel.SetStyleDisplay(DisplayStyle.None);
                    buttonRemove.SetStyleDisplay(DisplayStyle.None);
                    nameTextField.SetStyleDisplay(DisplayStyle.Flex);
                    buttonSave.SetStyleDisplay(DisplayStyle.Flex);
                    buttonCancel.SetStyleDisplay(DisplayStyle.Flex);
                    return;
                }

                nameLabel.SetStyleDisplay(DisplayStyle.Flex);
                buttonRemove.SetStyleDisplay(DisplayStyle.Flex);
                nameTextField.SetStyleDisplay(DisplayStyle.None);
                buttonSave.SetStyleDisplay(DisplayStyle.None);
                buttonCancel.SetStyleDisplay(DisplayStyle.None);
            });
            
            buttonSave.SetOnClick(() =>
            {
                if (saveHandler == null) throw new NullReferenceException(nameof(saveHandler));
                bool result = saveHandler.Invoke(target, nameTextField.value);
                if (!result) return;
                buttonRename.SetIsOn(false);
            });
            
            buttonCancel.SetOnClick(() =>
            {
                buttonRename.SetIsOn(false);
            });
            
            buttonRemove.SetOnClick(() =>
            {
                if (removeHandler == null) throw new NullReferenceException(nameof(removeHandler));
                removeHandler.Invoke(target);
            });
            
            leftContainer
                .AddChild(buttonRename);
            
            rightContainer
                .AddChild(buttonSave)
                .AddChild(buttonCancel)
                .AddChild(buttonRemove);
        }

        public void Reset()
        {
            target = string.Empty;
            nameLabel.text = string.Empty;
            nameTextField.value = string.Empty;
            
            buttonRename.isOn = false;

            removeHandler = null;
            saveHandler = null;

            nameLabel.SetStyleDisplay(DisplayStyle.Flex);
            buttonRemove.SetStyleDisplay(DisplayStyle.Flex);
            nameTextField.SetStyleDisplay(DisplayStyle.None);
            buttonSave.SetStyleDisplay(DisplayStyle.None);
            buttonCancel.SetStyleDisplay(DisplayStyle.None);
        }
        
        public CategoryNameItemCategoryRow SetTarget(string category)
        {
            Reset();
            target = category;
            nameLabel.text = category;
            nameTextField.value = category;
            SetEnabled(!category.Equals(CategoryNameItem.k_DefaultName));
            return this;
        }
        
        public CategoryNameItemCategoryRow SetRemoveHandler(UnityAction<string> removeCallback)
        {
            removeHandler = removeCallback;
            return this;
        }
        
        public CategoryNameItemCategoryRow SetSaveHandler(Func<string, string, bool> saveCallback)
        {
            saveHandler = saveCallback;
            return this;
        }
        
        public CategoryNameItemCategoryRow ShowRemoveCategoryButton()
        {
            buttonRemove.SetStyleDisplay(DisplayStyle.Flex);
            return this;
        }
        
        public CategoryNameItemCategoryRow HideRemoveCategoryButton()
        {
            buttonRemove.SetStyleDisplay(DisplayStyle.None);
            return this;
        }
        
        private static FluidToggleButtonTab NewButtonRename() =>
            FluidToggleButtonTab.Get()
                .SetElementSize(ElementSize.Small)
                .SetIcon(EditorSpriteSheets.EditorUI.Icons.Edit)
                .SetToggleAccentColor(EditorSelectableColors.Default.Action)
                .SetTooltip("Rename Category");
        
        private static FluidButton NewButtonSave() =>
            FluidButton.Get()
                .SetElementSize(ElementSize.Small)
                .SetIcon(EditorSpriteSheets.EditorUI.Icons.Save)
                .SetAccentColor(EditorSelectableColors.Default.Add)
                .SetTooltip("Save");

        private static FluidButton NewButtonCancel() =>
            FluidButton.Get()
                .SetElementSize(ElementSize.Small)
                .SetIcon(EditorSpriteSheets.EditorUI.Icons.Close)
                .SetAccentColor(EditorSelectableColors.Default.Remove)
                .SetTooltip("Cancel");
        
        private static FluidButton NewButtonRemove() =>
            FluidButton.Get()
                .SetElementSize(ElementSize.Small)
                .SetIcon(EditorSpriteSheets.EditorUI.Icons.Minus)
                .SetAccentColor(EditorSelectableColors.Default.Remove)
                .SetTooltip("Remove Category");
    }
}
