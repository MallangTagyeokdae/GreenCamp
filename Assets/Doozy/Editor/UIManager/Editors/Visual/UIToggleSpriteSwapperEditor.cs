// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.UIManager.Editors.Animators.Internal;
using Doozy.Runtime.Reactor.Targets;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Visual;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIManager.Editors.Visual
{
    [CustomEditor(typeof(UIToggleSpriteSwapper), true)]
    [CanEditMultipleObjects]
    public class UIToggleSpriteSwapperEditor : BaseTargetComponentAnimatorEditor
    {
        public UIToggleSpriteSwapper castedTarget => (UIToggleSpriteSwapper)target;
        public IEnumerable<UIToggleSpriteSwapper> castedTargets => targets.Cast<UIToggleSpriteSwapper>();
        
        protected override Color accentColor => EditorColors.UIManager.AudioComponent;
        protected override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.AudioComponent;
        
        private SerializedProperty propertySpriteTarget { get; set; }
        private SerializedProperty propertyOnSprite { get; set; }
        private SerializedProperty propertyOffSprite { get; set; }
        
        private FluidField spriteTargetFluidField { get; set; }
        private FluidField onSpriteFluidField { get; set; }
        private FluidField offSpriteFluidField { get; set; }
        
        private ObjectField spriteTargetObjectField { get; set; }
        private ObjectField onSpriteObjectField { get; set; }
        private ObjectField offSpriteObjectField { get; set; }
        
        private IVisualElementScheduledItem targetFinder { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            spriteTargetFluidField?.Recycle();
            onSpriteFluidField?.Recycle();
            offSpriteFluidField?.Recycle();
        }
        
        protected override void FindProperties()
        {
            base.FindProperties();
            
            propertySpriteTarget = serializedObject.FindProperty("SpriteTarget");
            propertyOnSprite = serializedObject.FindProperty("OnSprite");
            propertyOffSprite = serializedObject.FindProperty("OffSprite");
        }
        
        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(UIToggle)))
                .SetIcon(EditorSpriteSheets.UIManager.Icons.SpriteSwapper)
                .SetComponentTypeText("Sprite Swapper")
                .AddManualButton()
                .AddApiButton()
                .AddYouTubeButton();

            spriteTargetObjectField =
                DesignUtils.NewObjectField(propertySpriteTarget, typeof(ReactorSpriteTarget), false)
                    .SetStyleFlexGrow(1)
                    .SetTooltip("Sprite target");
            spriteTargetFluidField =
                FluidField.Get()
                    .SetLabelText("Sprite Target")
                    .SetIcon(EditorSpriteSheets.Reactor.Icons.SpriteTarget)
                    .AddFieldContent(spriteTargetObjectField);

            onSpriteObjectField = DesignUtils.NewObjectField(propertyOnSprite, typeof(Sprite), false).SetStyleFlexGrow(1).SetTooltip("Sprite to set when the UIToggle isOn state transitions to TRUE");
            onSpriteFluidField = FluidField.Get().SetLabelText(" On").SetElementSize(ElementSize.Tiny).AddFieldContent(onSpriteObjectField);
            offSpriteObjectField = DesignUtils.NewObjectField(propertyOffSprite, typeof(Sprite), false).SetStyleFlexGrow(1).SetTooltip("Sprite to set when the UIToggle isOn state transitions to FALSE");
            offSpriteFluidField = FluidField.Get().SetLabelText(" Off").SetElementSize(ElementSize.Tiny).AddFieldContent(offSpriteObjectField);
            
            targetFinder = root.schedule.Execute(() =>
            {
                if (castedTarget == null)
                    return;

                if (castedTarget.spriteTarget != null)
                {
                    targetFinder.Pause();
                    return;
                }

                castedTarget.FindTarget();

            }).Every(1000);
        }

        protected override void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(BaseUIContainerAnimatorEditor.GetController(propertyController))
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(spriteTargetFluidField)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(onSpriteFluidField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(offSpriteFluidField)
                .AddChild(DesignUtils.endOfLineBlock)
                ;
        }
    }
}
