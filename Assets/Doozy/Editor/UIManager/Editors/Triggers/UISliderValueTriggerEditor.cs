// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIManager.Editors.Triggers.Internal;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Triggers;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Editors.Triggers
{
    [CustomEditor(typeof(UISliderValueTrigger), true)]
    [CanEditMultipleObjects]
    public class UISliderValueTriggerEditor  : BaseValueTriggerEditor<UISlider>
    {
        protected override VisualElement GetTargetObjectField()
        {
            ObjectField targetObjectField = 
                DesignUtils.NewObjectField(propertyTarget, typeof(UISlider))
                    .SetTooltip("Target UI Slider")
                    .SetStyleFlexGrow(1);

            return FluidField.Get("Target")
                .AddFieldContent(targetObjectField);
        }
        
        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetSecondaryIcon(EditorSpriteSheets.UIManager.Icons.UISlider)
                .SetComponentNameText("UI Slider")
                .AddManualButton()
                .AddApiButton()
                .AddYouTubeButton();
        }
    }
}
