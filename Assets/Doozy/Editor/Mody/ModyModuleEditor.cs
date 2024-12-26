// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Mody.Components;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Mody;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Mody
{
    public class ModyModuleEditor<T> : EditorUIEditor<T> where T : ModyModule
    {
        protected override Color accentColor => EditorColors.Mody.Module;
        protected override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Mody.Module;

        protected FluidTab settingsTab { get; set; }
        protected FluidTab nameTab { get; set; }

        protected FluidAnimatedContainer settingsAnimatedContainer { get; set; }
        protected FluidAnimatedContainer moduleNameAnimatedContainer { get; set; }

        protected SerializedProperty propertyModuleName { get; private set; }

        private ModyModuleStateIndicator m_StateIndicator;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            nameTab?.Dispose();
            settingsTab?.Dispose();
            settingsAnimatedContainer?.Dispose();
            moduleNameAnimatedContainer?.Dispose();
            m_StateIndicator?.Dispose();
        }

        protected override void FindProperties()
        {
            base.FindProperties();

            propertyModuleName = serializedObject.FindProperty("ModuleName");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetIcon(EditorSpriteSheets.Mody.Icons.ModyModule);

            m_StateIndicator = new ModyModuleStateIndicator().SetStyleMarginLeft(DesignUtils.k_Spacing2X);
            castedTarget.UpdateState();
            m_StateIndicator.UpdateState(castedTarget.state);

            EnumField invisibleStateEnum = DesignUtils.NewEnumField("ModuleCurrentState", true);
            root.Add(invisibleStateEnum);
            invisibleStateEnum?.RegisterValueChangedCallback(evt =>
            {
                if (evt?.newValue == null) return;
                m_StateIndicator?.UpdateState((ModuleState)evt.newValue);
            });

            if (EditorApplication.isPlayingOrWillChangePlaymode)
                componentHeader.AddElement(m_StateIndicator);

            InitializeSettings();
            InitializeModuleName();

            root.schedule.Execute(() => settingsTab.ButtonSetIsOn(true, false));
        }

        protected FluidTab GetTab(string labelText) =>
            new FluidTab()
                .SetElementSize(ElementSize.Small)
                .SetLabelText(labelText)
                .IndicatorSetEnabledColor(accentColor)
                .ButtonSetAccentColor(selectableAccentColor)
                .AddToToggleGroup(tabsGroup);

        protected virtual void InitializeSettings()
        {
            settingsAnimatedContainer = new FluidAnimatedContainer("Settings", true).Hide(false);
            settingsTab =
                GetTab("Settings")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Settings)
                    .ButtonSetOnValueChanged(evt => settingsAnimatedContainer.Toggle(evt.newValue, evt.animateChange));

            settingsAnimatedContainer.SetOnShowCallback(() =>
            {
                var actionsDrawer =
                    new ModyActionsDrawer();

                actionsDrawer.schedule.Execute(() => actionsDrawer.Update());

                VisualElement actionsContainer =
                    new VisualElement().SetName("Actions Container");

                #pragma warning disable CS8321
                void AddActionToDrawer(ModyActionsDrawerItem item)
                {
                    actionsDrawer.AddItem(item);
                    actionsContainer.AddChild(item.animatedContainer);
                }
                #pragma warning restore CS8321

                settingsAnimatedContainer
                    .AddContent(actionsDrawer)
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent(actionsContainer)
                    .Bind(serializedObject);
            });
        }

        protected virtual void InitializeModuleName()
        {
            moduleNameAnimatedContainer = new FluidAnimatedContainer("Module Name", true).Hide(false);
            nameTab =
                GetTab("Module Name")
                    .SetElementSize(ElementSize.Small)
                    .ButtonSetOnValueChanged(evt => moduleNameAnimatedContainer.Toggle(evt.newValue, evt.animateChange));

            void UpdateComponentTypeText(string nameOfTheAnimator)
            {
                nameOfTheAnimator = nameOfTheAnimator.IsNullOrEmpty() ? string.Empty : $"Module - {nameOfTheAnimator}";
                componentHeader.SetComponentTypeText(nameOfTheAnimator);
            }

            UpdateComponentTypeText(propertyModuleName.stringValue);
            
            moduleNameAnimatedContainer.SetOnShowCallback(() =>
            {
                TextField moduleNameTextField =
                    DesignUtils.NewTextField(propertyModuleName).SetStyleFlexGrow(1);
                moduleNameTextField.RegisterValueChangedCallback(evt => UpdateComponentTypeText(evt.newValue));

                UpdateComponentTypeText(propertyModuleName.stringValue);

                moduleNameAnimatedContainer
                    .AddContent
                    (
                        FluidField.Get()
                            .SetLabelText("Module Name")
                            .SetTooltip("Name of the Module")
                            .AddFieldContent(moduleNameTextField)
                            .SetIcon(EditorSpriteSheets.EditorUI.Icons.Label)
                    )
                    .AddChild(DesignUtils.endOfLineBlock)
                    .Bind(serializedObject);
            });
        }

        protected override VisualElement Toolbar()
        {
            return
                base.Toolbar()
                    .AddChild(settingsTab)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(DesignUtils.flexibleSpace)
                    .AddChild(DesignUtils.spaceBlock2X)
                    .AddChild(nameTab);
        }

        protected override VisualElement Content()
        {
            return
                base.Content()
                    .AddChild(settingsAnimatedContainer)
                    .AddChild(moduleNameAnimatedContainer);
        }
    }
}
