// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ObjectNames = Doozy.Runtime.Common.Utils.ObjectNames;

namespace Doozy.Editor.Common.Addons
{
    #if DOOZY_42
    [CreateAssetMenu(menuName = "Doozy/42/Addon Info", fileName = "AddonInfo", order = 1000)]
    #endif
    [Serializable]
    public class AddonInfo : ScriptableObject
    {
        public string AddonId;
        public string AddonCategory;
        public string AddonName;
        public string AddonDescription;
        public string UnityAssetStoreURL;
        public string DoozyURL;
        public string ManualURL;
        public string YouTubePresentationURL;
        public string YouTubeTutorialURL;
        public Texture2D AddonImage;
    }

    [CustomEditor(typeof(AddonInfo))]
    public class AddonInfoEditor : UnityEditor.Editor
    {
        private AddonInfo castedTarget => (AddonInfo)target;
        private IEnumerable<AddonInfo> castedTargets => targets.Cast<AddonInfo>();

        private Color accentColor => EditorColors.EditorUI.LightGreen;
        private EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.EditorUI.LightGreen;

        private VisualElement root { get; set; }
        private FluidComponentHeader componentHeader { get; set; }

        private TextField addonIdTextField { get; set; }
        private TextField addonCategoryTextField { get; set; }
        private TextField addonNameTextField { get; set; }
        private TextField addonDescriptionTextField { get; set; }
        private TextField doozyURLTextField { get; set; }
        private TextField manualURLTextField { get; set; }
        private TextField unityAssetStoreURLTextField { get; set; }
        private TextField youTubePresentationURLTextField { get; set; }
        private TextField youTubeTutorialURLTextField { get; set; }
        private ObjectField addonImageObjectField { get; set; }
        private Image previewAddonImage { get; set; }

        private FluidField addonIdFluidField { get; set; }
        private FluidField addonCategoryFluidField { get; set; }
        private FluidField addonNameFluidField { get; set; }
        private FluidField addonDescriptionFluidField { get; set; }
        private FluidField doozyURLFluidField { get; set; }
        private FluidField manualURLFluidField { get; set; }
        private FluidField unityAssetStoreURLFluidField { get; set; }
        private FluidField youTubePresentationURLFluidField { get; set; }
        private FluidField youTubeTutorialURLFluidField { get; set; }
        private FluidField addonImageFluidField { get; set; }

        private FluidButton validateButton { get; set; }

        private SerializedProperty propertyAddonId { get; set; }
        private SerializedProperty propertyAddonCategory { get; set; }
        private SerializedProperty propertyAddonName { get; set; }
        private SerializedProperty propertyAddonDescription { get; set; }
        private SerializedProperty propertyDoozyURL { get; set; }
        private SerializedProperty propertyManualURL { get; set; }
        private SerializedProperty propertyUnityAssetStoreURL { get; set; }
        private SerializedProperty propertyYouTubePresentationURL { get; set; }
        private SerializedProperty propertyYouTubeTutorialURL { get; set; }
        private SerializedProperty propertyAddonImage { get; set; }

        private void OnDestroy()
        {
            componentHeader?.Recycle();
            addonIdFluidField?.Recycle();
            addonCategoryFluidField?.Recycle();
            addonNameFluidField?.Recycle();
            addonDescriptionFluidField?.Recycle();
            doozyURLFluidField?.Recycle();
            manualURLFluidField?.Recycle();
            unityAssetStoreURLFluidField?.Recycle();
            youTubePresentationURLFluidField?.Recycle();
            youTubeTutorialURLFluidField?.Recycle();
            addonImageFluidField?.Recycle();
        }

        public override VisualElement CreateInspectorGUI()
        {
            FindProperties();
            InitializeEditor();
            Compose();
            return root;
        }

        private void FindProperties()
        {
            propertyAddonId = serializedObject.FindProperty(nameof(AddonInfo.AddonId));
            propertyAddonCategory = serializedObject.FindProperty(nameof(AddonInfo.AddonCategory));
            propertyAddonName = serializedObject.FindProperty(nameof(AddonInfo.AddonName));
            propertyAddonDescription = serializedObject.FindProperty(nameof(AddonInfo.AddonDescription));
            propertyDoozyURL = serializedObject.FindProperty(nameof(AddonInfo.DoozyURL));
            propertyManualURL = serializedObject.FindProperty(nameof(AddonInfo.ManualURL));
            propertyUnityAssetStoreURL = serializedObject.FindProperty(nameof(AddonInfo.UnityAssetStoreURL));
            propertyYouTubePresentationURL = serializedObject.FindProperty(nameof(AddonInfo.YouTubePresentationURL));
            propertyYouTubeTutorialURL = serializedObject.FindProperty(nameof(AddonInfo.YouTubeTutorialURL));
            propertyAddonImage = serializedObject.FindProperty(nameof(AddonInfo.AddonImage));
        }

        private void InitializeEditor()
        {
            root = DesignUtils.editorRoot;
            componentHeader = DesignUtils.editorComponentHeader
                .SetComponentTypeText(ObjectNames.NicifyVariableName(nameof(AddonInfo)))
                .SetIcon(EditorSpriteSheets.EditorUI.Icons.Store)
                .SetAccentColor(accentColor);

            addonIdTextField = DesignUtils.NewTextField(propertyAddonId).SetStyleFlexGrow(1);
            addonCategoryTextField = DesignUtils.NewTextField(propertyAddonCategory).SetStyleFlexGrow(1);
            addonNameTextField = DesignUtils.NewTextField(propertyAddonName).SetStyleFlexGrow(1);
            addonDescriptionTextField = DesignUtils.NewTextField(propertyAddonDescription).SetStyleFlexGrow(1).SetMultiline(true);
            doozyURLTextField = DesignUtils.NewTextField(propertyDoozyURL).SetStyleFlexGrow(1);
            manualURLTextField = DesignUtils.NewTextField(propertyManualURL).SetStyleFlexGrow(1);
            unityAssetStoreURLTextField = DesignUtils.NewTextField(propertyUnityAssetStoreURL).SetStyleFlexGrow(1);
            youTubePresentationURLTextField = DesignUtils.NewTextField(propertyYouTubePresentationURL).SetStyleFlexGrow(1);
            youTubeTutorialURLTextField = DesignUtils.NewTextField(propertyYouTubeTutorialURL).SetStyleFlexGrow(1);
            addonImageObjectField = DesignUtils.NewObjectField(propertyAddonImage, typeof(Texture2D)).SetStyleFlexGrow(1);

            addonIdFluidField = FluidField.Get("Addon Id").AddFieldContent(addonIdTextField);
            addonCategoryFluidField = FluidField.Get("Addon Category").AddFieldContent(addonCategoryTextField);
            addonNameFluidField = FluidField.Get("Addon Name").AddFieldContent(addonNameTextField);
            addonDescriptionFluidField = FluidField.Get("Addon Description").AddFieldContent(addonDescriptionTextField);
            doozyURLFluidField = FluidField.Get("Doozy URL").AddFieldContent(doozyURLTextField);
            manualURLFluidField = FluidField.Get("Manual URL").AddFieldContent(manualURLTextField);
            unityAssetStoreURLFluidField = FluidField.Get("Unity Asset Store URL").AddFieldContent(unityAssetStoreURLTextField);
            youTubePresentationURLFluidField = FluidField.Get("YouTube Presentation URL").AddFieldContent(youTubePresentationURLTextField);
            youTubeTutorialURLFluidField = FluidField.Get("YouTube Tutorial URL").AddFieldContent(youTubeTutorialURLTextField);
            addonImageFluidField = FluidField.Get("Addon Image").AddFieldContent(addonImageObjectField);

            validateButton =
                FluidButton.Get("Validate")
                    .SetAccentColor(selectableAccentColor)
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Atom)
                    .SetButtonStyle(ButtonStyle.Contained)
                    .SetElementSize(ElementSize.Large)
                    .SetOnClick(Validate);

            previewAddonImage =
                new Image()
                    .SetStyleSize(256)
                    .SetStylePadding(DesignUtils.k_Spacing2X)
                    .SetStyleBackgroundScaleMode(ScaleMode.ScaleToFit)
                    .SetStyleBackgroundImage((Texture2D)propertyAddonImage.objectReferenceValue);

            addonImageObjectField.RegisterValueChangedCallback(evt =>
            {
                if (evt == null) return;
                previewAddonImage.SetStyleBackgroundImage((Texture2D)evt.newValue);
            });
        }

        private void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(addonIdFluidField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(addonCategoryFluidField)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(addonNameFluidField)
                )
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(addonDescriptionFluidField)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(doozyURLFluidField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(manualURLFluidField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(unityAssetStoreURLFluidField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(youTubePresentationURLFluidField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(youTubeTutorialURLFluidField)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild(addonImageFluidField)
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(validateButton)
                        .AddFlexibleSpace()
                        .AddChild(previewAddonImage)
                )
                ;
            ;
        }

        private void Validate()
        {
            string newName = $"{nameof(AddonInfo)} - {addonCategoryTextField.value} - {addonNameTextField.value}";
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(castedTarget), newName);
            EditorUtility.SetDirty(castedTarget);
            AssetDatabase.SaveAssetIfDirty(castedTarget);
        }
    }
}
