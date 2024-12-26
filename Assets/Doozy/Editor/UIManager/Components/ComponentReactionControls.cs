// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.Reactor.Components;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager;
using Doozy.Runtime.UIManager.Components;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Components
{
    public class ComponentReactionControls : ReactionControls
    {
        public FluidTab normalTab { get; private set; }
        public FluidTab highlightedTab { get; private set; }
        public FluidTab pressedTab { get; private set; }
        public FluidTab selectedTab { get; private set; }
        public FluidTab disabledTab { get; private set; }

        public UISelectable targetSelectable { get; set; }

        public override void Dispose()
        {
            base.Dispose();

            normalTab?.Dispose();
            highlightedTab?.Dispose();
            pressedTab?.Dispose();
            selectedTab?.Dispose();
            disabledTab?.Dispose();
        }

        public ComponentReactionControls(UISelectable targetSelectable)
        {
            this.targetSelectable = targetSelectable;
        }

        public ComponentReactionControls SetButtonSetAccentColor(EditorSelectableColorInfo selectableColor)
        {
            normalTab.ButtonSetAccentColor(selectableColor);
            highlightedTab.ButtonSetAccentColor(selectableColor);
            pressedTab.ButtonSetAccentColor(selectableColor);
            selectedTab.ButtonSetAccentColor(selectableColor);
            disabledTab.ButtonSetAccentColor(selectableColor);
            return this;
        }
        
        public ComponentReactionControls AddComponentControls
        (
            UnityAction resetCallback,
            UnityAction normalCallback,
            UnityAction highlightedCallback,
            UnityAction pressedCallback,
            UnityAction selectedCallback,
            UnityAction disabledCallback
        )
        {
            VisualElement resetButtonContainer =
                DesignUtils.row
                    .SetName("Editor Only Container")
                    .SetStyleDisplay(EditorApplication.isPlayingOrWillChangePlaymode ? DisplayStyle.None : DisplayStyle.Flex)
                    .SetStyleFlexGrow(0)
                    .SetStyleAlignItems(Align.Center)
                    .AddChild(GetResetButton(resetCallback))
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(DesignUtils.dividerVertical)
                    .AddChild(DesignUtils.spaceBlock2X);

            FluidTab GetTab(string labelText, UnityAction callback)
            {
                var tab =
                    new FluidTab()
                        .SetLabelText(labelText)
                        .SetName(labelText)
                        .SetTabPosition(TabPosition.FloatingTab)
                        .SetElementSize(ElementSize.Small)
                        .ButtonSetAccentColor(EditorSelectableColors.Reactor.Red);

                tab.indicator
                    .SetIcon(null)
                    .SetEnabledColor(EditorColors.Reactor.Red)
                    .SetDisabledColor(Color.clear)
                    .Toggle(false, false)
                    .Update();

                tab.ButtonSetOnClick(() =>
                {
                    callback?.Invoke();
                    if (Application.isPlaying) return;
                    tab.schedule.Execute(() =>
                    {
                        tab.ButtonSetIsOn(false);
                    }).ExecuteLater(200);
                });

                return tab;
            }

            normalTab = GetTab("Normal", normalCallback);
            highlightedTab = GetTab("Highlighted", highlightedCallback);
            pressedTab = GetTab("Pressed", pressedCallback);
            selectedTab = GetTab("Selected", selectedCallback);
            disabledTab = GetTab("Disabled", disabledCallback);

            if (Application.isPlaying)
            {
                schedule.Execute(() =>
                {
                    if (!Application.isPlaying) return;
                    if (targetSelectable == null) return;
                    UISelectionState state = targetSelectable.currentUISelectionState;
                    normalTab.ButtonSetIsOn(state == UISelectionState.Normal);
                    highlightedTab.ButtonSetIsOn(state == UISelectionState.Highlighted);
                    pressedTab.ButtonSetIsOn(state == UISelectionState.Pressed);
                    selectedTab.ButtonSetIsOn(state == UISelectionState.Selected);
                    disabledTab.ButtonSetIsOn(state == UISelectionState.Disabled);

                }).Every(50);
            }

            return this
                .AddItem(resetButtonContainer)
                .AddItem(normalTab)
                .AddSpaceBlock()
                .AddItem(highlightedTab)
                .AddSpaceBlock()
                .AddItem(pressedTab)
                .AddSpaceBlock()
                .AddItem(selectedTab)
                .AddSpaceBlock()
                .AddItem(disabledTab)
                .AddSpaceBlock2X()
                .AddChild(DesignUtils.dividerVertical)
                .AddChild(DesignUtils.spaceBlock)
                .AddFlexibleSpace()
                .AddItem(icon);
        }
    }
}
