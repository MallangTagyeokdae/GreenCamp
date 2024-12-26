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
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Audio;
using Doozy.Runtime.UIManager.Components;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace Doozy.Editor.UIManager.Editors.Audio
{
    [CustomEditor(typeof(UIToggleAudio), true)]
    [CanEditMultipleObjects]
    public class UIToggleAudioEditor : BaseTargetComponentAnimatorEditor
    {
        public UIToggleAudio castedTarget => (UIToggleAudio)target;
        public IEnumerable<UIToggleAudio> castedTargets => targets.Cast<UIToggleAudio>();
        
        protected override Color accentColor => EditorColors.UIManager.AudioComponent;
        protected override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.AudioComponent;
        
        private SerializedProperty propertyAudioSource { get; set; }
        private SerializedProperty propertyOnAudioClip { get; set; }
        private SerializedProperty propertyOffAudioClip { get; set; }
        
        private FluidField audioSourceFluidField { get; set; }
        private FluidField onAudioClipFluidField { get; set; }
        private FluidField offAudioClipFluidField { get; set; }
        
        private ObjectField audioSourceObjectField { get; set; }
        private ObjectField onAudioClipObjectField { get; set; }
        private ObjectField offAudioClipObjectField { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            audioSourceFluidField?.Recycle();
            onAudioClipFluidField?.Recycle();
            offAudioClipFluidField?.Recycle();
        }
        
        protected override void FindProperties()
        {
            base.FindProperties();
            
            propertyAudioSource = serializedObject.FindProperty("AudioSource");
            propertyOnAudioClip = serializedObject.FindProperty("OnAudioClip");
            propertyOffAudioClip = serializedObject.FindProperty("OffAudioClip");
        }
        
        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(UIToggle)))
                .SetIcon(EditorSpriteSheets.EditorUI.Icons.Sound)
                .SetComponentTypeText("Audio")
                .AddManualButton()
                .AddApiButton()
                .AddYouTubeButton();


            audioSourceObjectField = DesignUtils.NewObjectField(propertyAudioSource, typeof(AudioSource)).SetStyleFlexGrow(1).SetTooltip("Target AudioSource");
            audioSourceFluidField = FluidField.Get().SetLabelText("Audio Source").SetIcon(EditorSpriteSheets.EditorUI.Icons.Sound).AddFieldContent(audioSourceObjectField);

            onAudioClipObjectField = DesignUtils.NewObjectField(propertyOnAudioClip, typeof(AudioClip)).SetStyleFlexGrow(1).SetTooltip("Audio Clip played when the toggle transitions to the On state");
            onAudioClipFluidField = FluidField.Get().SetLabelText(" On").SetElementSize(ElementSize.Tiny).AddFieldContent(onAudioClipObjectField);
            
            offAudioClipObjectField = DesignUtils.NewObjectField(propertyOffAudioClip, typeof(AudioClip)).SetStyleFlexGrow(1).SetTooltip("Audio Clip played when the toggle transitions to the Off state");
            offAudioClipFluidField = FluidField.Get().SetLabelText(" Off").SetElementSize(ElementSize.Tiny).AddFieldContent(offAudioClipObjectField);
        }
        
        protected override void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(BaseUIContainerAnimatorEditor.GetController(propertyController))
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(audioSourceFluidField)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(onAudioClipFluidField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(offAudioClipFluidField)
                .AddChild(DesignUtils.endOfLineBlock)
                ;
        }
    }
}
