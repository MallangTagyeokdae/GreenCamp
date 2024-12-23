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
using Doozy.Runtime.UIManager.Containers;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace Doozy.Editor.UIManager.Editors.Audio
{
    [CustomEditor(typeof(UIContainerAudio), true)]
    [CanEditMultipleObjects]
    public class UIContainerAudioEditor : BaseTargetComponentAnimatorEditor
    {
        public UIContainerAudio castedTarget => (UIContainerAudio)target;
        public IEnumerable<UIContainerAudio> castedTargets => targets.Cast<UIContainerAudio>();

        protected override Color accentColor => EditorColors.UIManager.AudioComponent;
        protected override EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.AudioComponent;

        private SerializedProperty propertyAudioSource { get; set; }
        private SerializedProperty propertyShowAudioClip { get; set; }
        private SerializedProperty propertyHideAudioClip { get; set; }

        private FluidField audioSourceFluidField { get; set; }
        private FluidField showAudioClipFluidField { get; set; }
        private FluidField hideAudioClipFluidField { get; set; }

        private ObjectField audioSourceObjectField { get; set; }
        private ObjectField showAudioClipObjectField { get; set; }
        private ObjectField hideAudioClipObjectField { get; set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            audioSourceFluidField?.Recycle();
            showAudioClipFluidField?.Recycle();
            hideAudioClipFluidField?.Recycle();
        }
      
        protected override void FindProperties()
        {
            base.FindProperties();

            propertyAudioSource = serializedObject.FindProperty("AudioSource");
            propertyShowAudioClip = serializedObject.FindProperty("ShowAudioClip");
            propertyHideAudioClip = serializedObject.FindProperty("HideAudioClip");
        }

        protected override void InitializeEditor()
        {
            base.InitializeEditor();

            componentHeader
                .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(UIContainer)))
                .SetIcon(EditorSpriteSheets.EditorUI.Icons.Sound)
                .SetComponentTypeText("Audio")
                .AddManualButton()
                .AddApiButton("https://api.doozyui.com/api/Doozy.Runtime.UIManager.Audio.UIContainerAudio.html")
                .AddYouTubeButton();


            audioSourceObjectField = DesignUtils.NewObjectField(propertyAudioSource, typeof(AudioSource)).SetStyleFlexGrow(1).SetTooltip("Target AudioSource");
            audioSourceFluidField = FluidField.Get().SetLabelText("Audio Source").SetIcon(EditorSpriteSheets.EditorUI.Icons.Sound).AddFieldContent(audioSourceObjectField);

            showAudioClipObjectField = DesignUtils.NewObjectField(propertyShowAudioClip, typeof(AudioClip), false).SetStyleFlexGrow(1).SetTooltip("AudioClip played on Show");
            showAudioClipFluidField = FluidField.Get().SetLabelText(" Show").SetElementSize(ElementSize.Tiny).AddFieldContent(showAudioClipObjectField);

            hideAudioClipObjectField = DesignUtils.NewObjectField(propertyHideAudioClip, typeof(AudioClip), false).SetStyleFlexGrow(1).SetTooltip("AudioClip played on Hide");
            hideAudioClipFluidField = FluidField.Get().SetLabelText(" Hide").SetElementSize(ElementSize.Tiny).AddFieldContent(hideAudioClipObjectField);
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
                .AddChild(showAudioClipFluidField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(hideAudioClipFluidField)
                .AddChild(DesignUtils.endOfLineBlock)
                ;
        }

     
    }
}
