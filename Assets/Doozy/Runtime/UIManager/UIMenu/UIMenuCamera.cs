// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Doozy.Runtime.Common;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animators.Internal;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Color = UnityEngine.Color;

#if INPUT_SYSTEM_PACKAGE
using UnityEngine.InputSystem;
#endif

// ReSharper disable UnusedMethodReturnValue.Local
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeMadeStatic.Local

namespace Doozy.Runtime.UIManager.UIMenu
{
    /// <summary>
    /// Specialized component used to take snapshots, multi-snapshots and create a static or animated preview for UIMenu items.
    /// It can generate a png file (as a snapshot), multiple png files (as a multi shot) and a sprite sheet (as a multi shot by grouping all shots into one sliced texture)
    /// <para/> Works on only in Play Mode in the Unity Editor 
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("UIMenu/Editor Only/UIMenu Camera")]
    public class UIMenuCamera : MonoBehaviour
    {
        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/UIMenu/Editor Only/UIMenu Camera", false, 7)]
        private static void CreateComponent(UnityEditor.MenuCommand menuCommand)
        {
            GameObjectUtils.AddToScene<UIMenuCamera>("UIMenu Camera", false, true);
        }
        #endif

        /// <summary>
        /// Constant used to lower the given FPS frequency by 0.1% to avoid the "dot crawl" effect (a display artifact)
        /// <footer>https://en.wikipedia.org/wiki/Frame_rate</footer>
        /// </summary>
        private const float k_FPSFrequencyModifier = 1.001f;
        private const int k_MinFPS = 1;
        private static float GetTickInterval(int fps) => 1f / (Mathf.Max(k_MinFPS, fps) * k_FPSFrequencyModifier);

        public enum State
        {
            Idle,
            Running,
            Processing
        }

        public RectTransform SnapshotTarget;
        public Camera SnapshotCamera;

        public string SnapshotsFolderName = "_Snapshots";
        public string TargetPath = string.Empty;
        public bool AutoDeleteFilesFromTargetPath;

        public int MultiShotFPS = 24;

        public bool GenerateSpriteSheet = true;

        public float CustomMultiShotDuration = 2f;
        
        public UIContainer TargetUIContainer;
        public float UIContainerShowDelay = 1f;

        public UIToggle TargetUIToggle;
        public float UIToggleAnimationDuration = 1f;
        
        public UISelectable TargetUISelectable;
        public float UISelectableStateDuration = 1.5f;

        public ReactorController TargetReactorController;

        public ReactorAnimator TargetAnimator;

        public Progressor TargetProgressor;

        [SerializeField] private State CurrentState;
        public State currentState
        {
            get => CurrentState;
            set
            {
                CurrentState = value;
                OnStateChanged?.Invoke(value);
            }
        }

        // ReSharper disable once UnassignedField.Global
        public UnityAction<State> OnStateChanged;
        public bool AutoGenerateSnapshots = true;
        public bool AutoOverrideSettings = true;

        /// <summary>
        /// Invoked after a snapshot session
        /// <para/> fileName, targetPath, snapshotData
        /// </summary>
        public UnityAction<string, string, List<SnapshotData>> OnSnapshot;

        public string defaultTargetPath =>
            PathUtils.CleanPath(Path.Combine("Assets", SnapshotsFolderName));

        private bool initialized { get; set; }
        private RenderTexture camRenderTexture { get; set; }
        private Rect target { get; set; }
        private Canvas rootCanvas { get; set; }
        private CanvasScaler canvasScaler { get; set; }

        private bool m_MultiShotStarted;
        private int m_Width, m_Height;
        private double m_ElapsedTime, m_LastTickTime;
        private float tickInterval { get; set; }
        private float timeSinceStartup => Time.realtimeSinceStartup;

        private List<SnapshotData> m_SnapshotData;
        public List<SnapshotData> snapshotData => m_SnapshotData ?? (m_SnapshotData = new List<SnapshotData>());

        #if INPUT_SYSTEM_PACKAGE
        private InputAction m_Space;
        #endif

        private static bool canRun => Application.isPlaying;

        private void Reset()
        {
            ResetTargetPath();
        }

        public void FindTarget()
        {
            SnapshotTarget = SnapshotTarget ? SnapshotTarget : GetComponent<RectTransform>();

            if (SnapshotCamera != null) return;


            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("UIMenuCamera: No Canvas found in the hierarchy");
                return;
            }

            if (!canvas.isRootCanvas) canvas = canvas.rootCanvas;

            if (canvas == null)
            {
                Debug.LogError("UIMenuCamera: No ROOT Canvas found in the hierarchy");
                return;
            }

            Camera targetCamera = canvas.worldCamera;

            if (targetCamera == null)
            {
                targetCamera = Camera.main;

                if (targetCamera == null)
                {
                    Debug.LogError("UIMenuCamera: No Camera found in the hierarchy");
                    return;
                }
            }

            SnapshotCamera = targetCamera;
        }

        private void Initialize()
        {
            if (initialized) return;
            target = SnapshotTarget.rect;
            rootCanvas = SnapshotTarget.GetComponentInParent<Canvas>().rootCanvas;
            canvasScaler = SnapshotTarget.GetComponentInParent<CanvasScaler>();
            initialized = true;
            currentState = State.Idle;
        }

        private void Awake()
        {
            if (!canRun) return;
            FindTarget();
            Initialize();

            #if INPUT_SYSTEM_PACKAGE
            {
                m_Space = new InputAction("Space", InputActionType.Button, "<Keyboard>/space");
                m_Space.performed += _ => StartStopMultiShot();
            }
            #endif
        }

        private void OnEnable()
        {
            if (!canRun) return;

            #if INPUT_SYSTEM_PACKAGE
            {
                m_Space.Enable();
            }
            #endif
        }

        private void OnDisable()
        {
            if (!canRun) return;
            #if INPUT_SYSTEM_PACKAGE
            {
                m_Space.Dispose();
            }
            #endif
        }

        private void Update()
        {
            if (!Application.isPlaying) return;
            if (!m_MultiShotStarted) return;
            m_ElapsedTime += timeSinceStartup - m_LastTickTime;
            m_LastTickTime = timeSinceStartup;
            if (m_ElapsedTime < tickInterval) return;
            m_ElapsedTime = 0;

            TakeSnapshot(false);
        }

        private void StartStopMultiShot()
        {
            if (m_MultiShotStarted)
            {
                StopMultiShot();
                return;
            }
            StartMultiShot();
        }

        public void StartMultiShot()
        {
            if (m_MultiShotStarted) return;
            if (AutoOverrideSettings) OverrideSettings();
            ResetTime();
            snapshotData.Clear();
            m_MultiShotStarted = true;
            currentState = State.Running;
        }

        public void StopMultiShot()
        {
            if (!m_MultiShotStarted) return;
            if (AutoOverrideSettings) RestoreSettings();
            m_MultiShotStarted = false;
            OnSnapshot?.Invoke(SnapshotTarget.name, TargetPath, snapshotData);
            if (AutoGenerateSnapshots)
                GenerateSnapshots(SnapshotTarget.name, TargetPath, snapshotData);
        }

        public void CancelMultiShot()
        {
            if (!m_MultiShotStarted) return;
            m_MultiShotStarted = false;
            snapshotData.Clear();
            currentState = State.Idle;
        }

        public void TakeSnapshot(bool singleShot = true)
        {
            if (!CanRun()) return;
            if (singleShot)
            {
                CancelMultiShot();
                snapshotData.Clear();
                currentState = State.Idle;
                currentState = State.Running;
            }
            Run(singleShot);
        }

        #region Custom MultiShot

        public void CustomMultiShot()
        {
            StopAllCoroutines();
            StartCoroutine(MultiShotForCustomDuration());
        }
        
        private IEnumerator MultiShotForCustomDuration()
        {
            yield return new WaitForSecondsRealtime(tickInterval);
            StartMultiShot();
            yield return new WaitForSecondsRealtime(tickInterval);
            yield return new WaitForSeconds(CustomMultiShotDuration);
            yield return new WaitForSecondsRealtime(tickInterval);
            StopMultiShot();
        }
        
        #endregion
        
        #region UIContainer MultiShot

        public void UIContainerMultiShot()
        {
            if (TargetUIContainer == null) return;
            StopAllCoroutines();
            StartCoroutine(MultiShotUIContainer());
        }

        private IEnumerator MultiShotUIContainer()
        {
            TargetUIContainer.InstantShow();
            StartMultiShot();
            yield return new WaitForSecondsRealtime(tickInterval);
            TargetUIContainer.Hide();
            yield return new WaitForSecondsRealtime(TargetUIContainer.totalDurationForHide);
            yield return new WaitForSecondsRealtime(UIContainerShowDelay);
            yield return new WaitForSecondsRealtime(tickInterval);
            TargetUIContainer.Show();
            yield return new WaitForSecondsRealtime(TargetUIContainer.totalDurationForShow);
            yield return new WaitForSecondsRealtime(tickInterval);
            StopMultiShot();
        }

        #endregion

        #region UIToggle MultiShot
        
        public void UIToggleMultiShot()
        {
            if (TargetUIToggle == null) return;
            StopAllCoroutines();
            StartCoroutine(MultiShotUIToggle());
        }
        
        private IEnumerator MultiShotUIToggle()
        {
            TargetUIToggle.SetIsOn(false, false);
            yield return new WaitForSecondsRealtime(0.5f);
            StartMultiShot();
            yield return new WaitForSecondsRealtime(tickInterval);
            TargetUIToggle.isOn = true;
            yield return new WaitForSecondsRealtime(UIToggleAnimationDuration);
            yield return new WaitForSecondsRealtime(tickInterval);
            TargetUIToggle.isOn = false;
            yield return new WaitForSecondsRealtime(UIToggleAnimationDuration);
            yield return new WaitForSecondsRealtime(tickInterval);
            StopMultiShot();
        }

        #endregion

        #region UISelectable MultiShot

        public void UISelectableMultiShot()
        {
            if (TargetUISelectable == null) return;
            StopAllCoroutines();
            StartCoroutine(MultiShotUISelectable());
        }

        private IEnumerator MultiShotUISelectable()
        {
            if (TargetUISelectable.isToggle)
            {
                TargetUISelectable.isOn = true;
                yield return new WaitForSecondsRealtime(UISelectableStateDuration);
            }
            yield return new WaitForSecondsRealtime(0.5f);
            StartMultiShot();
            yield return new WaitForSecondsRealtime(0.1f);
            TargetUISelectable.SetState(UISelectionState.Highlighted);
            yield return new WaitForSecondsRealtime(UISelectableStateDuration * 0.25f);
            TargetUISelectable.SetState(UISelectionState.Pressed);
            if (TargetUISelectable.isToggle)
            {
                TargetUISelectable.isOn = false;
            }
            yield return new WaitForSecondsRealtime(UISelectableStateDuration * 0.25f);
            TargetUISelectable.SetState(UISelectionState.Normal);
            yield return new WaitForSecondsRealtime(UISelectableStateDuration * 0.5f);
            if (TargetUISelectable.isToggle)
            {
                TargetUISelectable.SetState(UISelectionState.Pressed);
                TargetUISelectable.isOn = true;
                yield return new WaitForSecondsRealtime(UISelectableStateDuration * 0.5f);
            }
            TargetUISelectable.SetState(UISelectionState.Disabled);
            yield return new WaitForSecondsRealtime(UISelectableStateDuration * 0.25f);
            TargetUISelectable.SetState(UISelectionState.Normal);
            yield return new WaitForSecondsRealtime(0.1f);
            StopMultiShot();
        }

        #endregion

        #region ReactorController MultiShot

        public void ReactorControllerMultiShot()
        {
            if (TargetReactorController == null) return;
            StopAllCoroutines();
            StartCoroutine(MultiShotReactorController());
        }

        private IEnumerator MultiShotReactorController()
        {
            // TargetReactorController.SetProgressAtZero();
            StartMultiShot();
            yield return new WaitForSecondsRealtime(tickInterval * 3f);
            TargetReactorController.Play();
            yield return null;
            yield return new WaitForSecondsRealtime(TargetReactorController.GetTotalDuration());
            yield return new WaitForSecondsRealtime(tickInterval * 3f);
            StopMultiShot();
        }

        #endregion

        #region Animator MultiShot

        public void AnimatorMultiShot()
        {
            if (TargetAnimator == null) return;
            StopAllCoroutines();
            StartCoroutine(MultiShotAnimator());
        }

        private IEnumerator MultiShotAnimator()
        {
            // TargetAnimator.SetProgressAtZero();
            StartMultiShot();
            yield return new WaitForSecondsRealtime(tickInterval * 3f);
            TargetAnimator.Play();
            yield return null;
            yield return new WaitForSecondsRealtime(TargetAnimator.GetTotalDuration());
            yield return new WaitForSecondsRealtime(tickInterval * 3f);
            StopMultiShot();
        }

        #endregion

        #region Progressor MultiShot

        public void ProgressorMultiShot()
        {
            if (TargetProgressor == null) return;
            StopAllCoroutines();
            StartCoroutine(MultiShotTargetProgressor());
        }

        private IEnumerator MultiShotTargetProgressor()
        {
            StartMultiShot();
            yield return new WaitForSecondsRealtime(0.1f);
            TargetProgressor.Play();
            yield return null;
            yield return new WaitForSecondsRealtime(TargetProgressor.GetTotalDuration());
            yield return new WaitForSecondsRealtime(0.1f);
            StopMultiShot();
        }

        #endregion

        private void ResetTime()
        {
            tickInterval = GetTickInterval(MultiShotFPS);
            m_ElapsedTime = 0;
            m_LastTickTime = timeSinceStartup;
        }

        public void ResetTargetPath()
        {
            TargetPath = defaultTargetPath;
        }

        private bool CanRun()
        {
            #if !UNITY_EDITOR
            {
                return false;
            }
            #endif

            if (Application.isPlaying) return true;
            #if UNITY_EDITOR
            {
                UnityEditor.EditorUtility.DisplayDialog("UI Menu Camera", "Works only in Play Mode", "Ok");
            }
            #endif
            return false;
        }

        #region Snapshot Camera Settings

        private CameraClearFlags snapshotCameraClearFlags { get; set; }
        private Color snapshotCameraBackgroundColor { get; set; }

        private UIMenuCamera OverrideSnapshotCameraSettings()
        {
            snapshotCameraClearFlags = SnapshotCamera.clearFlags;
            snapshotCameraBackgroundColor = SnapshotCamera.backgroundColor;
            SnapshotCamera.clearFlags = CameraClearFlags.SolidColor;
            SnapshotCamera.backgroundColor = Color.clear;
            return this;
        }

        private UIMenuCamera RestoreSnapshotCameraSettings()
        {
            SnapshotCamera.clearFlags = snapshotCameraClearFlags;
            SnapshotCamera.backgroundColor = snapshotCameraBackgroundColor;
            return this;
        }

        #endregion

        #region Root Canvas Settings

        private RenderMode rootCanvasRenderMode { get; set; }
        private Camera rootCanvasWorldCamera { get; set; }

        private UIMenuCamera OverrideRootCanvasSettings()
        {
            rootCanvasRenderMode = rootCanvas.renderMode;
            rootCanvasWorldCamera = rootCanvas.worldCamera;
            rootCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            rootCanvas.worldCamera = SnapshotCamera;
            return this;
        }

        private UIMenuCamera RestoreRootCanvasSettings()
        {
            rootCanvas.renderMode = rootCanvasRenderMode;
            rootCanvas.worldCamera = rootCanvasWorldCamera;
            return this;
        }

        #endregion

        #region Canvas Scaler Settings

        private CanvasScaler.ScaleMode canvasScalerUiScaleMode { get; set; }
        private float canvasScalerScaleFactor { get; set; }

        private UIMenuCamera OverrideCanvasScalerSettings()
        {
            if (canvasScaler == null)
                return this;
            
            canvasScalerUiScaleMode = canvasScaler.uiScaleMode;
            canvasScalerScaleFactor = canvasScaler.scaleFactor;
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            canvasScaler.scaleFactor = 1f;
            return this;
        }

        private UIMenuCamera RestoreCanvasScalerSettings()
        {
            if (canvasScaler == null)
                return this;
            
            canvasScaler.uiScaleMode = canvasScalerUiScaleMode;
            canvasScaler.scaleFactor = canvasScalerScaleFactor;
            return this;
        }

        #endregion

        public void OverrideSettings()
        {
            OverrideSnapshotCameraSettings();
            OverrideRootCanvasSettings();
            OverrideCanvasScalerSettings();
        }

        public void RestoreSettings()
        {
            RestoreSnapshotCameraSettings();
            RestoreRootCanvasSettings();
            RestoreCanvasScalerSettings();
        }

        private void Run(bool singleShot = true)
        {
            target = SnapshotTarget.rect;

            if (singleShot) OverrideSettings();

            //size and offsets
            m_Width = Convert.ToInt32(target.width);                                               //calculate the width of the object to capture
            m_Height = Convert.ToInt32(target.height);                                             //calculate the height of the object to capture
            var screenshotTexture = new Texture2D(m_Width, m_Height, TextureFormat.ARGB32, false); //create our snapshot texture
            var scrRenderTexture = new RenderTexture(m_Width, m_Height, 24);                       //create our render texture
            camRenderTexture = SnapshotCamera.targetTexture;                                       //save the initial render texture reference
            SnapshotCamera.targetTexture = scrRenderTexture;                                       //reference our render texture
            SnapshotCamera.Render();                                                               //render camera to our texture (snapshot)
            SnapshotCamera.targetTexture = camRenderTexture;                                       //restore the initial render texture reference
            RenderTexture.active = scrRenderTexture;                                               //set the active render texture as our texture
            screenshotTexture.ReadPixels(new Rect(0, 0, m_Width, m_Height), 0, 0);                 //read data
            screenshotTexture.Apply();                                                             //apply

            if (singleShot) RestoreSettings();

            //create fileName
            string fileName = SnapshotTarget.name;
            fileName = singleShot ? fileName : $"{fileName}/{fileName}_{snapshotData.Count:000}";

            //add snapshot data the list (fileName, path for the new file, data bytes)
            string snapshotRelativePath = Path.Combine(TargetPath, $"{fileName}.png");
            string snapshotAbsolutePath = PathUtils.ToAbsolutePath(snapshotRelativePath);
            snapshotData.Add
            (
                new SnapshotData
                (
                    SnapshotTarget.name,            //fileName
                    snapshotAbsolutePath,           //path
                    screenshotTexture.EncodeToPNG() //bytes
                )
            );

            RenderTexture.active = null; //clear the active RenderTexture
            Destroy(scrRenderTexture);   //destroy our temp render texture from memory
            Destroy(screenshotTexture);  //destroy the temp screenshot texture from memory
            if (singleShot)              //if this is a single shot -> generate the snapshot immediately
            {
                OnSnapshot?.Invoke(SnapshotTarget.name, TargetPath, snapshotData);
                if (AutoGenerateSnapshots)
                    GenerateSnapshots(SnapshotTarget.name, TargetPath, snapshotData);
            }
        }

        public void GenerateSnapshots(string fileName, string targetPath, List<SnapshotData> snapshot)
        {
            #if UNITY_EDITOR
            {
                int numberOfSnapshots = snapshot.Count;
                if (numberOfSnapshots == 0)
                {
                    currentState = State.Idle;
                    return;
                }

                bool multiShot = numberOfSnapshots > 1;

                currentState = State.Processing;

                PathUtils.CreatePath(targetPath);

                if (multiShot & AutoDeleteFilesFromTargetPath & !GenerateSpriteSheet)
                {
                    string folderPath = PathUtils.GetDirectoryName(snapshot[0].path);
                    if (Directory.Exists(folderPath))
                        foreach (string path in Directory.GetFiles(folderPath))
                            File.Delete(path);
                }

                if (multiShot & GenerateSpriteSheet)
                {
                    Rect rect = SnapshotTarget.rect;
                    int width = (int)rect.width;
                    int height = (int)rect.height;

                    int columns = (int)Mathf.Sqrt(numberOfSnapshots);
                    int rows = Mathf.CeilToInt(numberOfSnapshots / (float)columns);

                    int spriteSheetWidth = width * columns;
                    int spriteSheetHeight = height * rows;

                    //Calculate Max Texture Size for the sprite sheet
                    const int maxSizeLimit = 8192; //Hard Limit
                    int maxWidthOrHeight = Mathf.Max(spriteSheetWidth, spriteSheetHeight);
                    if (maxWidthOrHeight > maxSizeLimit)
                    {
                        Debug.Log($"Cannot create sprite sheet as it would be over the max size limit ({maxSizeLimit}). Either make fewer snapshots (decrease duration) or make the RectTransform of the Snapshot Target smaller in size");
                        snapshot.Clear();
                        currentState = State.Idle;
                        return;
                    }

                    int maxTextureSize = 128;
                    while (maxWidthOrHeight > maxTextureSize)
                        maxTextureSize *= 2;

                    int x = 0;
                    int y = height * (rows - 1);

                    // ReSharper disable once IdentifierTypo
                    var spriteMetaDatas = new List<UnityEditor.SpriteMetaData>();
                    var spriteSheet = new Texture2D(spriteSheetWidth, spriteSheetHeight);
                    for (int i = 0; i < numberOfSnapshots; i++)
                    {
                        spriteMetaDatas.Add(new UnityEditor.SpriteMetaData
                        {
                            rect = new Rect(x, y, width, height),
                            name = $"~{fileName.RemoveWhitespaces().RemoveAllSpecialCharacters()}_{i:000}"
                        });

                        var texture = new Texture2D(width, height);
                        texture.LoadImage(snapshot[i].bytes);
                        Color[] colors = texture.GetPixels(0, 0, width, height);
                        spriteSheet.SetPixels(x, y, width, height, colors);

                        x += width;
                        if (x > (columns - 1) * width)
                        {
                            y -= height;
                            x = 0;
                        }
                    }

                    //add transparent pixels when possible
                    if (columns * rows > numberOfSnapshots)
                    {
                        for (int i = 0; i < columns * rows - numberOfSnapshots; i++)
                        {
                            var colors = new Color[width * height];
                            for (int j = 0; j < colors.Length; j++) colors[j] = Color.clear;
                            spriteSheet.SetPixels(x, y, width, height, colors);
                            x += width;
                        }
                    }

                    spriteSheet.Apply();

                    string spriteSheetPath = PathUtils.CleanPath(Path.Combine(targetPath, $"{fileName}.png"));
                    File.WriteAllBytes(PathUtils.ToAbsolutePath(spriteSheetPath), spriteSheet.EncodeToPNG());
                    UnityEditor.AssetDatabase.Refresh();

                    //Apply sprite sheet settings
                    var importer = UnityEditor.AssetImporter.GetAtPath(spriteSheetPath) as UnityEditor.TextureImporter;

                    System.Diagnostics.Debug.Assert(importer != null, nameof(importer) + " != null");
                    importer.textureType = UnityEditor.TextureImporterType.Sprite;
                    importer.spriteImportMode = UnityEditor.SpriteImportMode.Multiple;
                    importer.spritesheet = spriteMetaDatas.ToArray();
                    importer.maxTextureSize = maxTextureSize;

                    var settings = new UnityEditor.TextureImporterSettings();
                    importer.ReadTextureSettings(settings);
                    settings.spriteMeshType = SpriteMeshType.FullRect;
                    settings.spriteExtrude = 0;
                    settings.spriteGenerateFallbackPhysicsShape = false;
                    settings.readable = true;
                    importer.SetTextureSettings(settings);

                    UnityEditor.AssetDatabase.ImportAsset(spriteSheetPath, UnityEditor.ImportAssetOptions.ForceUpdate);

                    Debugger.Log($"Created a sprite sheet at {spriteSheetPath}. (size: {spriteSheetWidth}x{spriteSheetHeight}) (columns: {columns}) (rows: {rows}) ({numberOfSnapshots} sprites @ w:{width} h:{height})");
                }
                else
                {
                    if (multiShot & !GenerateSpriteSheet)
                        PathUtils.CreatePath(Path.Combine(targetPath, fileName));

                    if (!multiShot && File.Exists(snapshot[0].path))
                        UnityEditor.AssetDatabase.DeleteAsset(PathUtils.ToRelativePath(snapshot[0].path));

                    foreach (SnapshotData data in snapshot)
                        data.CreateSnapshot();

                    UnityEditor.AssetDatabase.Refresh();
                    UnityEditor.AssetDatabase.StartAssetEditing();
                    {
                        foreach (SnapshotData data in snapshot)
                            SetTextureSettingsToGUI(data.path);
                    }
                    UnityEditor.AssetDatabase.StopAssetEditing();
                    UnityEditor.AssetDatabase.SaveAssets();
                    UnityEditor.AssetDatabase.Refresh();
                    Debugger.Log($"Created {numberOfSnapshots} snapshot{(multiShot ? "s" : "")} at {snapshot[0].path.RemoveLast(snapshot[0].fileName.Length + 4 + (multiShot ? 4 : 0))}");
                }

                snapshot.Clear();
                currentState = State.Idle;
            }
            #endif
        }

        /// <summary>
        /// Set the texture at the target path to have the following settings:
        /// <para/> textureType = TextureImporterType.GUI
        /// <para/> mipmapEnabled = true
        /// <para/> filterMode = FilterMode.Trilinear
        /// <para/> textureCompression = TextureImporterCompression.Uncompressed
        /// </summary>
        /// <param name="filePath"> Texture file path 'Assets/MyFolderName/MyTextureName.png' </param>
        public static void SetTextureSettingsToGUI(string filePath)
        {
            #if UNITY_EDITOR
            {
                if (filePath.StartsWith(Application.dataPath))
                    filePath = "Assets" + filePath.Substring(Application.dataPath.Length);

                var textureImporter = UnityEditor.AssetImporter.GetAtPath(filePath) as UnityEditor.TextureImporter;
                if (textureImporter == null) return;
                Debug.Assert(textureImporter != null, nameof(textureImporter) + " != null");
                bool requiresImport = false;
                if (textureImporter.textureType != UnityEditor.TextureImporterType.GUI)
                {
                    textureImporter.textureType = UnityEditor.TextureImporterType.GUI;
                    requiresImport = true;
                }

                if (textureImporter.mipmapEnabled != true)
                {
                    textureImporter.mipmapEnabled = true;
                    requiresImport = true;
                }

                if (textureImporter.filterMode != FilterMode.Trilinear)
                {
                    textureImporter.filterMode = FilterMode.Trilinear;
                    requiresImport = true;
                }

                if (textureImporter.textureCompression != UnityEditor.TextureImporterCompression.Uncompressed)
                {
                    textureImporter.textureCompression = UnityEditor.TextureImporterCompression.Uncompressed;
                    requiresImport = true;
                }

                if (!requiresImport) return;
                UnityEditor.AssetDatabase.ImportAsset(filePath, UnityEditor.ImportAssetOptions.ForceUpdate);
            }
            #endif
        }

        public readonly struct SnapshotData
        {
            public string fileName { get; }
            public string path { get; }
            public byte[] bytes { get; }

            public SnapshotData(string fileName, string path, byte[] bytes)
            {
                this.bytes = bytes;
                this.fileName = fileName;
                this.path = PathUtils.CleanPath(path);
            }

            public void CreateSnapshot() =>
                File.WriteAllBytes(path, bytes);
        }
    }
}
