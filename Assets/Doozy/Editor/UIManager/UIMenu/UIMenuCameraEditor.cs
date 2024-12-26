// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animators.Internal;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using Doozy.Runtime.UIManager.UIMenu;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
namespace Doozy.Editor.UIManager.UIMenu
{
    [CustomEditor(typeof(UIMenuCamera), true)]
    public class UIMenuCameraEditor : UnityEditor.Editor
    {
        public UIMenuCamera castedTarget => (UIMenuCamera)target;

        private VisualElement root { get; set; }
        private FluidComponentHeader componentHeader { get; set; }

        private FloatField customMultiShotDurationFloatField { get; set; }
        private FloatField uiContainerShowDelayFloatField { get; set; }
        private FloatField uiSelectableStateDurationFloatField { get; set; }
        private FloatField uiToggleAnimationDurationFloatField { get; set; }

        private FluidButton recordAnimatorButton { get; set; }
        private FluidButton recordCustomMultiShotButton { get; set; }
        private FluidButton recordProgressorButton { get; set; }
        private FluidButton recordReactorControllerButton { get; set; }
        private FluidButton recordUIContainerButton { get; set; }
        private FluidButton recordUISelectableButton { get; set; }
        private FluidButton recordUIToggleButton { get; set; }
        private FluidButton resetPathButton { get; set; }
        private FluidButton takeSnapshotButton { get; set; }
        
        private FluidField customMultiShotDurationFluidField { get; set; }
        private FluidField multiShotFluidField { get; set; }
        private FluidField snapshotCameraFluidField { get; set; }
        private FluidField snapshotsFolderNameFluidField { get; set; }
        private FluidField snapshotTargetFluidField { get; set; }
        private FluidField targetAnimatorFluidField { get; set; }
        private FluidField targetPathFluidField { get; set; }
        private FluidField targetProgressorFluidField { get; set; }
        private FluidField targetReactorControllerFluidField { get; set; }
        private FluidField targetUIContainerFluidField { get; set; }
        private FluidField targetUISelectableFluidField { get; set; }
        private FluidField targetUIToggleFluidField { get; set; }
        private FluidField uiContainerShowDelayFluidField { get; set; }
        private FluidField uiSelectableStateDurationFluidField { get; set; }
        private FluidField uiToggleAnimationDurationFluidField { get; set; }

        private FluidToggleSwitch autoDeleteFilesFromTargetPathSwitch { get; set; }
        private FluidToggleSwitch generateSpriteSheetSwitch { get; set; }

        private IntegerField multiShotFPSIntegerField { get; set; }

        private ObjectField snapshotCameraObjectField { get; set; }
        private ObjectField snapshotTargetObjectField { get; set; }
        private ObjectField targetAnimatorObjectField { get; set; }
        private ObjectField targetProgressorObjectField { get; set; }
        private ObjectField targetReactorControllerObjectField { get; set; }
        private ObjectField targetUIContainerObjectField { get; set; }
        private ObjectField targetUISelectableObjectField { get; set; }
        private ObjectField targetUIToggleObjectField { get; set; }

        private TextField snapshotsFolderNameTextField { get; set; }
        private TextField targetPathTextField { get; set; }

        private SerializedProperty propertyAutoDeleteFilesFromTargetPath { get; set; }
        private SerializedProperty propertyCustomMultiShotDuration { get; set; }
        private SerializedProperty propertyGenerateSpriteSheet { get; set; }
        private SerializedProperty propertyMultiShotFPS { get; set; }
        private SerializedProperty propertySnapshotCamera { get; set; }
        private SerializedProperty propertySnapshotsFolderName { get; set; }
        private SerializedProperty propertySnapshotTarget { get; set; }
        private SerializedProperty propertyTargetAnimator { get; set; }
        private SerializedProperty propertyTargetPath { get; set; }
        private SerializedProperty propertyTargetProgressor { get; set; }
        private SerializedProperty propertyTargetReactorController { get; set; }
        private SerializedProperty propertyTargetUIContainer { get; set; }
        private SerializedProperty propertyTargetUISelectable { get; set; }
        private SerializedProperty propertyTargetUIToggle { get; set; }
        private SerializedProperty propertyUIContainerShowDelay { get; set; }
        private SerializedProperty propertyUISelectableStateDuration { get; set; }
        private SerializedProperty propertyUIToggleAnimationDuration { get; set; }

        private void OnEnable()
        {
            castedTarget.FindTarget();
        }

        private void OnDestroy()
        {
            componentHeader?.Recycle();

            autoDeleteFilesFromTargetPathSwitch?.Recycle();
            customMultiShotDurationFluidField?.Recycle();
            multiShotFluidField?.Recycle();
            recordAnimatorButton?.Recycle();
            recordCustomMultiShotButton?.Recycle();
            recordProgressorButton?.Recycle();
            recordReactorControllerButton?.Recycle();
            recordUIContainerButton?.Recycle();
            recordUISelectableButton?.Recycle();
            recordUIToggleButton?.Recycle();
            resetPathButton?.Recycle();
            snapshotCameraFluidField?.Recycle();
            snapshotTargetFluidField?.Recycle();
            targetAnimatorFluidField?.Recycle();
            targetProgressorFluidField?.Recycle();
            targetReactorControllerFluidField?.Recycle();
            targetUISelectableFluidField?.Recycle();
            targetUIToggleFluidField?.Recycle();
            uiContainerShowDelayFluidField?.Recycle();
            uiSelectableStateDurationFluidField?.Recycle();
            uiToggleAnimationDurationFluidField?.Recycle();
        }

        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        private void FindProperties()
        {
            propertyAutoDeleteFilesFromTargetPath = serializedObject.FindProperty(nameof(UIMenuCamera.AutoDeleteFilesFromTargetPath));
            propertyCustomMultiShotDuration = serializedObject.FindProperty(nameof(UIMenuCamera.CustomMultiShotDuration));
            propertyGenerateSpriteSheet = serializedObject.FindProperty(nameof(UIMenuCamera.GenerateSpriteSheet));
            propertyMultiShotFPS = serializedObject.FindProperty(nameof(UIMenuCamera.MultiShotFPS));
            propertySnapshotCamera = serializedObject.FindProperty(nameof(UIMenuCamera.SnapshotCamera));
            propertySnapshotsFolderName = serializedObject.FindProperty(nameof(UIMenuCamera.SnapshotsFolderName));
            propertySnapshotTarget = serializedObject.FindProperty(nameof(UIMenuCamera.SnapshotTarget));
            propertyTargetAnimator = serializedObject.FindProperty(nameof(UIMenuCamera.TargetAnimator));
            propertyTargetPath = serializedObject.FindProperty(nameof(UIMenuCamera.TargetPath));
            propertyTargetProgressor = serializedObject.FindProperty(nameof(UIMenuCamera.TargetProgressor));
            propertyTargetReactorController = serializedObject.FindProperty(nameof(UIMenuCamera.TargetReactorController));
            propertyTargetUIContainer = serializedObject.FindProperty(nameof(UIMenuCamera.TargetUIContainer));
            propertyTargetUISelectable = serializedObject.FindProperty(nameof(UIMenuCamera.TargetUISelectable));
            propertyTargetUIToggle = serializedObject.FindProperty(nameof(UIMenuCamera.TargetUIToggle));
            propertyUIContainerShowDelay = serializedObject.FindProperty(nameof(UIMenuCamera.UIContainerShowDelay));
            propertyUISelectableStateDuration = serializedObject.FindProperty(nameof(UIMenuCamera.UISelectableStateDuration));
            propertyUIToggleAnimationDuration = serializedObject.FindProperty(nameof(UIMenuCamera.UIToggleAnimationDuration));
        }

        private void InitializeEditor()
        {
            FindProperties();

            root = DesignUtils.GetEditorRoot();

            componentHeader =
                FluidComponentHeader.Get()
                    .SetElementSize(ElementSize.Large)
                    .SetComponentNameText(ObjectNames.NicifyVariableName(nameof(UIMenuCamera)))
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Camera)
                    .AddManualButton()
                    .AddYouTubeButton();

            autoDeleteFilesFromTargetPathSwitch = FluidToggleSwitch.Get("Auto Delete files from Target Path").BindToProperty(propertyAutoDeleteFilesFromTargetPath);
            multiShotFPSIntegerField = DesignUtils.NewIntegerField(propertyMultiShotFPS).SetStyleFlexGrow(1);
            snapshotCameraObjectField = DesignUtils.NewObjectField(propertySnapshotCamera, typeof(Camera)).SetStyleFlexGrow(1);
            snapshotsFolderNameTextField = DesignUtils.NewTextField(propertySnapshotsFolderName).SetStyleFlexGrow(1);
            snapshotTargetObjectField = DesignUtils.NewObjectField(propertySnapshotTarget, typeof(RectTransform)).SetStyleFlexGrow(1);
            targetAnimatorObjectField = DesignUtils.NewObjectField(propertyTargetAnimator, typeof(ReactorAnimator)).SetStyleFlexGrow(1);
            targetPathTextField = DesignUtils.NewTextField(propertyTargetPath).SetStyleFlexGrow(1);
            targetProgressorObjectField = DesignUtils.NewObjectField(propertyTargetProgressor, typeof(Progressor)).SetStyleFlexGrow(1);
            targetReactorControllerObjectField = DesignUtils.NewObjectField(propertyTargetReactorController, typeof(ReactorController)).SetStyleFlexGrow(1);
            targetUIContainerObjectField = DesignUtils.NewObjectField(propertyTargetUIContainer, typeof(UIContainer)).SetStyleFlexGrow(1);
            targetUISelectableObjectField = DesignUtils.NewObjectField(propertyTargetUISelectable, typeof(UISelectable)).SetStyleFlexGrow(1);
            targetUIToggleObjectField = DesignUtils.NewObjectField(propertyTargetUIToggle, typeof(UIToggle)).SetStyleFlexGrow(1);
            customMultiShotDurationFloatField = DesignUtils.NewFloatField(propertyCustomMultiShotDuration).SetStyleFlexGrow(1);
            uiContainerShowDelayFloatField = DesignUtils.NewFloatField(propertyUIContainerShowDelay).SetStyleFlexGrow(1);
            uiSelectableStateDurationFloatField = DesignUtils.NewFloatField(propertyUISelectableStateDuration).SetStyleFlexGrow(1);
            uiToggleAnimationDurationFloatField = DesignUtils.NewFloatField(propertyUIToggleAnimationDuration).SetStyleFlexGrow(1);

            snapshotTargetFluidField = FluidField.Get("Snapshot Target").AddFieldContent(snapshotTargetObjectField);
            snapshotCameraFluidField = FluidField.Get("Snapshot Camera").AddFieldContent(snapshotCameraObjectField);

            FluidButton SnapshotButton() =>
                FluidButton.Get("Take MultiShot")
                    .SetIcon(EditorSpriteSheets.EditorUI.Components.EditorTextureGroup)
                    .SetButtonStyle(ButtonStyle.Contained);

            takeSnapshotButton = SnapshotButton()
                .SetLabelText("Take Snapshot")
                .SetElementSize(ElementSize.Large)
                .SetIcon(EditorSpriteSheets.EditorUI.Icons.Texture)
                .SetOnClick(() => castedTarget.TakeSnapshot());

            const float durationFieldWidth = 80f;

            //Custom MultiShot
            recordCustomMultiShotButton = SnapshotButton().SetOnClick(() => castedTarget.CustomMultiShot());
            customMultiShotDurationFluidField = FluidField.Get("Custom MultiShot Duration").SetIcon(EditorSpriteSheets.EditorUI.Icons.Cooldown)
                .AddFieldContent(customMultiShotDurationFloatField);
            
            //UIContainer
            recordUIContainerButton = SnapshotButton().SetOnClick(() => castedTarget.UIContainerMultiShot());
            targetUIContainerFluidField = FluidField.Get("UIContainer").SetIcon(EditorSpriteSheets.UIManager.Icons.UIContainer)
                .AddFieldContent(targetUIContainerObjectField);
            recordUIContainerButton.SetEnabled(propertyTargetUIContainer.objectReferenceValue);
            targetUIContainerObjectField.RegisterValueChangedCallback(evt =>
            {
                if (evt == null) return;
                recordUIContainerButton.SetEnabled(evt.newValue);
            });
            uiContainerShowDelayFluidField =
                FluidField.Get("Show Delay")
                    .AddFieldContent(uiContainerShowDelayFloatField)
                    .SetStyleWidth(durationFieldWidth, durationFieldWidth, durationFieldWidth);

            //UIToggle
            recordUIToggleButton = SnapshotButton().SetOnClick(() => castedTarget.UIToggleMultiShot());
            targetUIToggleFluidField = FluidField.Get("UIToggle").SetIcon(EditorSpriteSheets.UIManager.Icons.UIToggle)
                .AddFieldContent(targetUIToggleObjectField);
            recordUIToggleButton.SetEnabled(propertyTargetUIToggle.objectReferenceValue);
            targetUIToggleObjectField.RegisterValueChangedCallback(evt =>
            {
                if (evt == null) return;
                recordUIToggleButton.SetEnabled(evt.newValue);
            });
            uiToggleAnimationDurationFluidField =
                FluidField.Get("Animation Duration")
                    .AddFieldContent(uiToggleAnimationDurationFloatField)
                    .SetStyleWidth(durationFieldWidth, durationFieldWidth, durationFieldWidth);
            
            //UISelectable
            recordUISelectableButton = SnapshotButton().SetOnClick(() => castedTarget.UISelectableMultiShot());
            targetUISelectableFluidField = FluidField.Get("UISelectable").SetIcon(EditorSpriteSheets.UIManager.Icons.UISelectable)
                .AddFieldContent(targetUISelectableObjectField);
            recordUISelectableButton.SetEnabled(propertyTargetUISelectable.objectReferenceValue);
            targetUISelectableObjectField.RegisterValueChangedCallback(evt =>
            {
                if (evt == null) return;
                recordUISelectableButton.SetEnabled(evt.newValue);
            });
            uiSelectableStateDurationFluidField =
                FluidField.Get("State Duration")
                    .AddFieldContent(uiSelectableStateDurationFloatField)
                    .SetStyleWidth(durationFieldWidth, durationFieldWidth, durationFieldWidth);

            //ReactorController
            recordReactorControllerButton = SnapshotButton().SetOnClick(() => castedTarget.ReactorControllerMultiShot());
            targetReactorControllerFluidField = FluidField.Get("Reactor Controller").SetIcon(EditorSpriteSheets.Reactor.Icons.ReactorController)
                .AddFieldContent(targetReactorControllerObjectField);
            recordReactorControllerButton.SetEnabled(propertyTargetReactorController.objectReferenceValue);
            targetReactorControllerObjectField.RegisterValueChangedCallback(evt =>
            {
                if (evt == null) return;
                recordReactorControllerButton.SetEnabled(evt.newValue);
            });

            //ReactorAnimator
            recordAnimatorButton = SnapshotButton().SetOnClick(() => castedTarget.AnimatorMultiShot());
            targetAnimatorFluidField = FluidField.Get("Animator").SetIcon(EditorSpriteSheets.Reactor.Icons.ReactorIcon)
                .AddFieldContent(targetAnimatorObjectField);
            recordAnimatorButton.SetEnabled(propertyTargetAnimator.objectReferenceValue);
            targetAnimatorObjectField.RegisterValueChangedCallback(evt =>
            {
                if (evt == null) return;
                recordAnimatorButton.SetEnabled(evt.newValue);
            });

            //Progressor
            recordProgressorButton = SnapshotButton().SetOnClick(() => castedTarget.ProgressorMultiShot());
            targetProgressorFluidField = FluidField.Get("Progressor").SetIcon(EditorSpriteSheets.Reactor.Icons.Progressor)
                .AddFieldContent(targetProgressorObjectField);
            recordProgressorButton.SetEnabled(propertyTargetProgressor.objectReferenceValue);
            targetProgressorObjectField.RegisterValueChangedCallback(evt =>
            {
                if (evt == null) return;
                recordProgressorButton.SetEnabled(evt.newValue);
            });

            multiShotFluidField = FluidField.Get("MultiShot FPS")
                .SetElementSize(ElementSize.Tiny)
                .AddFieldContent(multiShotFPSIntegerField);

            generateSpriteSheetSwitch =
                FluidToggleSwitch.Get("Generate Sprite Sheet")
                    .BindToProperty(propertyGenerateSpriteSheet);


            //Snapshots Folder Name
            snapshotsFolderNameFluidField =
                FluidField.Get("Snapshots Folder Name")
                    .AddFieldContent(snapshotsFolderNameTextField);

            resetPathButton =
                FluidButton.Get()
                    .SetIcon(EditorSpriteSheets.EditorUI.Icons.Reset)
                    .SetTooltip("Reset Target Path")
                    .SetElementSize(ElementSize.Small)
                    .SetOnClick(() => targetPathTextField.value = castedTarget.defaultTargetPath);

            //Target Path
            targetPathFluidField =
                FluidField.Get("Target Path")
                    .AddFieldContent(targetPathTextField)
                    .AddInfoElement(DesignUtils.row.SetStyleAlignItems(Align.FlexEnd).AddChild(resetPathButton));


            //Drag and Drop

            targetPathTextField.RegisterCallback<AttachToPanelEvent>(_ =>
            {
                targetPathTextField.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
                targetPathTextField.RegisterCallback<DragPerformEvent>(OnDragPerformEvent);
            });

            targetPathTextField.RegisterCallback<DetachFromPanelEvent>(_ =>
            {
                targetPathTextField.UnregisterCallback<DragUpdatedEvent>(OnDragUpdate);
                targetPathTextField.UnregisterCallback<DragPerformEvent>(OnDragPerformEvent);
            });

            void OnDragUpdate(DragUpdatedEvent evt)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            }

            void OnDragPerformEvent(DragPerformEvent evt)
            {
                string folderPath = $"{AssetDatabase.GetAssetPath(DragAndDrop.objectReferences[0])}/";
                targetPathTextField.value = folderPath;
            }
        }

        private void Compose()
        {
            root
                .AddChild(componentHeader)
                .AddChild(DesignUtils.spaceBlock)
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(snapshotTargetFluidField)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(snapshotCameraFluidField)
                )
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(DesignUtils.flexibleSpace)
                        .AddChild(takeSnapshotButton)
                        .AddChild(DesignUtils.flexibleSpace)
                )
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(generateSpriteSheetSwitch)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(multiShotFluidField)
                )
                .AddChild(DesignUtils.spaceBlock)
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleAlignItems(Align.Center)
                        .AddChild(customMultiShotDurationFluidField)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(recordCustomMultiShotButton)
                )
                .AddChild(DesignUtils.spaceBlock)
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleAlignItems(Align.Center)
                        .AddChild(targetUIContainerFluidField)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(uiContainerShowDelayFluidField)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(recordUIContainerButton)
                )
                .AddChild(DesignUtils.spaceBlock)
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleAlignItems(Align.Center)
                        .AddChild(targetUIToggleFluidField)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(uiToggleAnimationDurationFluidField)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(recordUIToggleButton)
                )
                .AddChild(DesignUtils.spaceBlock)
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleAlignItems(Align.Center)
                        .AddChild(targetUISelectableFluidField)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(uiSelectableStateDurationFluidField)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(recordUISelectableButton)
                )
                .AddChild(DesignUtils.spaceBlock)
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleAlignItems(Align.Center)
                        .AddChild(targetReactorControllerFluidField)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(recordReactorControllerButton)
                )
                .AddChild(DesignUtils.spaceBlock)
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleAlignItems(Align.Center)
                        .AddChild(targetAnimatorFluidField)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(recordAnimatorButton)
                )
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleAlignItems(Align.Center)
                        .AddChild(targetProgressorFluidField)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(recordProgressorButton)
                )
                .AddChild(DesignUtils.spaceBlock2X)
                .AddChild(targetPathFluidField)
                .AddChild(autoDeleteFilesFromTargetPathSwitch)
                .AddChild(DesignUtils.endOfLineBlock);
        }
    }
}
