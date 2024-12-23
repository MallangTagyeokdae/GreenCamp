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
using Doozy.Editor.UIManager.Editors.Components;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Audio;
using Doozy.Runtime.UIManager.Components;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
namespace Doozy.Editor.UIManager.Editors.Audio
{
    [CustomEditor(typeof(UISelectableAudio), true)]
    public class UISelectableAudioEditor : BaseTargetComponentAnimatorEditor
    {
        public UISelectableAudio castedTarget => (UISelectableAudio)target;
        public IEnumerable<UISelectableAudio> castedTargets => targets.Cast<UISelectableAudio>();

        protected override Color accentColor => EditorColors.UIManager.AudioComponent;
        protected override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.AudioComponent;
        
        private SerializedProperty propertyAudioSource { get; set; }
        private SerializedProperty propertyNormalAudioClip { get; set; }
        private SerializedProperty propertyHighlightedAudioClip { get; set; }
        private SerializedProperty propertyPressedAudioClip { get; set; }
        private SerializedProperty propertySelectedAudioClip { get; set; }
        private SerializedProperty propertyDisabledAudioClip { get; set; }

        private FluidField audioSourceFluidField { get; set; }
        private FluidField normalAudioClipFluidField { get; set; }
        private FluidField highlightedAudioClipFluidField { get; set; }
        private FluidField pressedAudioClipFluidField { get; set; }
        private FluidField selectedAudioClipFluidField { get; set; }
        private FluidField disabledAudioClipFluidField { get; set; }

        private ObjectField audioSourceObjectField { get; set; }
        private ObjectField normalAudioClipObjectField { get; set; }
        private ObjectField highlightedAudioClipObjectField { get; set; }
        private ObjectField pressedAudioClipObjectField { get; set; }
        private ObjectField selectedAudioClipObjectField { get; set; }
        private ObjectField disabledAudioClipObjectField { get; set; }

        private SerializedProperty propertyToggleCommand { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            audioSourceFluidField?.Recycle();
            normalAudioClipFluidField?.Recycle();
            highlightedAudioClipFluidField?.Recycle();
            pressedAudioClipFluidField?.Recycle();
            selectedAudioClipFluidField?.Recycle();
            disabledAudioClipFluidField?.Recycle();
        }

        protected override void FindProperties()
        {
            base.FindProperties();
            propertyAudioSource = serializedObject.FindProperty("AudioSource");
            propertyNormalAudioClip = serializedObject.FindProperty("NormalAudioClip");
            propertyHighlightedAudioClip = serializedObject.FindProperty("HighlightedAudioClip");
            propertyPressedAudioClip = serializedObject.FindProperty("PressedAudioClip");
            propertySelectedAudioClip = serializedObject.FindProperty("SelectedAudioClip");
            propertyDisabledAudioClip = serializedObject.FindProperty("DisabledAudioClip");

            propertyToggleCommand = serializedObject.FindProperty("ToggleCommand");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(UISelectable)))
                .SetIcon(EditorSpriteSheets.EditorUI.Icons.Sound)
                .SetComponentTypeText("Audio")
                .AddManualButton()
                .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.UIManager.Audio.UISelectableAudio.html")
                .AddYouTubeButton();

            audioSourceObjectField =
                DesignUtils.NewObjectField(propertyAudioSource, typeof(AudioSource))
                    .SetStyleFlexGrow(1)
                    .SetTooltip("Target AudioSource");
            audioSourceFluidField =
                FluidField.Get()
                    .SetLabelText("Audio Source")
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Sound)
                    .AddFieldContent(audioSourceObjectField);

            normalAudioClipObjectField = DesignUtils.NewObjectField(propertyNormalAudioClip, typeof(AudioClip), false).SetStyleFlexGrow(1).SetTooltip("AudioClip played on Normal state");
            normalAudioClipFluidField = FluidField.Get().SetLabelText(" Normal").SetElementSize(ElementSize.Tiny).AddFieldContent(normalAudioClipObjectField);

            highlightedAudioClipObjectField = DesignUtils.NewObjectField(propertyHighlightedAudioClip, typeof(AudioClip), false).SetStyleFlexGrow(1).SetTooltip("AudioClip played on Highlighted state");
            highlightedAudioClipFluidField = FluidField.Get().SetLabelText(" Highlighted").SetElementSize(ElementSize.Tiny).AddFieldContent(highlightedAudioClipObjectField);

            pressedAudioClipObjectField = DesignUtils.NewObjectField(propertyPressedAudioClip, typeof(AudioClip), false).SetStyleFlexGrow(1).SetTooltip("AudioClip played on Pressed state");
            pressedAudioClipFluidField = FluidField.Get().SetLabelText(" Pressed").SetElementSize(ElementSize.Tiny).AddFieldContent(pressedAudioClipObjectField);

            selectedAudioClipObjectField = DesignUtils.NewObjectField(propertySelectedAudioClip, typeof(AudioClip), false).SetStyleFlexGrow(1).SetTooltip("AudioClip played on Selected state");
            selectedAudioClipFluidField = FluidField.Get().SetLabelText(" Selected").SetElementSize(ElementSize.Tiny).AddFieldContent(selectedAudioClipObjectField);

            disabledAudioClipObjectField = DesignUtils.NewObjectField(propertyDisabledAudioClip, typeof(AudioClip), false).SetStyleFlexGrow(1).SetTooltip("AudioClip played on Disabled state");
            disabledAudioClipFluidField = FluidField.Get().SetLabelText(" Disabled").SetElementSize(ElementSize.Tiny).AddFieldContent(disabledAudioClipObjectField);
        }

        protected override void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(BaseUISelectableAnimatorEditor.GetController(propertyController, propertyToggleCommand))
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(audioSourceFluidField)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(normalAudioClipFluidField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(highlightedAudioClipFluidField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(pressedAudioClipFluidField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(selectedAudioClipFluidField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(disabledAudioClipFluidField)
                .AddChild(DesignUtils.endOfLineBlock)
                ;
        }

       
    }
}
