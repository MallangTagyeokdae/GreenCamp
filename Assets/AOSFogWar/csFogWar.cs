/*
 * Created :    Winter 2022
 * Author :     SeungGeon Kim (keithrek@hanmail.net)
 * Project :    FogWar
 * Filename :   csHomebrewFogWar.cs (non-static monobehaviour module)
 * 
 * All Content (C) 2022 Unlimited Fischl Works, all rights reserved.
 */



using System;                       // Convert
using System.IO;                    // Directory
using System.Linq;                  // Enumerable
using System.Collections.Generic;   // List
using UnityEngine;                  // Monobehaviour
using UnityEditor;                  // Handles



namespace FischlWorks_FogWar
{



    /// The non-static high-level monobehaviour interface of the AOS Fog of War module.

    /// This class holds serialized data for various configuration properties,\n
    /// and is resposible for scanning / saving / loading the LevelData object.\n
    /// The class handles the update frequency of the fog, plus some shader businesses.\n
    /// Various public interfaces related to FogRevealer's FOV are also available.
    public class csFogWar : MonoBehaviour
    {
        /// A class for storing the base level data.
        /// 
        /// This class is later serialized into Json format.\n
        /// Empty spaces are stored as 0, while the obstacles are stored as 1.\n
        /// If a level is loaded instead of being scanned, 
        /// the level dimension properties of csFogWar will be replaced by the level data.
        [System.Serializable]
        public class LevelData
        {
            public void AddColumn(LevelColumn levelColumn)
            {
                levelRow.Add(levelColumn);
            }

            // Indexer definition
            public LevelColumn this[int index] {
                get {
                    if (index >= 0 && index < levelRow.Count)
                    {
                        return levelRow[index];
                    }
                    else
                    {
                        Debug.LogErrorFormat("index given in x axis is out of range");

                        return null;
                    }
                }
                set {
                    if (index >= 0 && index < levelRow.Count)
                    {
                        levelRow[index] = value;
                    }
                    else
                    {
                        Debug.LogErrorFormat("index given in x axis is out of range");

                        return;
                    }
                }
            }

            // Adding private getter / setters are not allowed for serialization
            public int levelDimensionX = 0;
            public int levelDimensionY = 0;

            public float unitScale = 0;

            public float scanSpacingPerUnit = 0;

            [SerializeField]
            private List<LevelColumn> levelRow = new List<LevelColumn>();
        }



        [System.Serializable]
        public class LevelColumn
        {
            public LevelColumn(IEnumerable<ETileState> stateTiles)
            {
                levelColumn = new List<ETileState>(stateTiles);
            }

            // If I create a separate Tile class, it will impact the size of the save file (but enums will be saved as int)
            public enum ETileState
            {
                Empty,
                Obstacle
            }

            // Indexer definition
            public ETileState this[int index] {
                get {
                    if (index >= 0 && index < levelColumn.Count)
                    {
                        return levelColumn[index];
                    }
                    else
                    {
                        Debug.LogErrorFormat("index given in y axis is out of range");

                        return ETileState.Empty;
                    }
                }
                set {
                    if (index >= 0 && index < levelColumn.Count)
                    {
                        levelColumn[index] = value;
                    }
                    else
                    {
                        Debug.LogErrorFormat("index given in y axis is out of range");

                        return;
                    }
                }
            }

            [SerializeField]
            private List<ETileState> levelColumn = new List<ETileState>();
        }



        [System.Serializable]
        public class FogRevealer
        {
            public FogRevealer(Transform revealerTransform, int sightRange, bool updateOnlyOnMove)
            {
                this.revealerTransform = revealerTransform;
                this.sightRange = sightRange;
                this.updateOnlyOnMove = updateOnlyOnMove;
            }

            public Vector2Int GetCurrentLevelCoordinates(csFogWar fogWar)
            {
                currentLevelCoordinates = new Vector2Int(
                    fogWar.GetUnitX(revealerTransform.position.x),
                    fogWar.GetUnitY(revealerTransform.position.z));

                return currentLevelCoordinates;
            }

            // To be assigned manually by the user
            [SerializeField]
            private Transform revealerTransform = null;
            // These are called expression-bodied properties btw, being stricter here because these are not pure data containers
            public Transform _RevealerTransform => revealerTransform;

            [SerializeField]
            private int sightRange = 0;
            public int _SightRange => sightRange;

            [SerializeField]
            private bool updateOnlyOnMove = true;
            public bool _UpdateOnlyOnMove => updateOnlyOnMove;

            private Vector2Int currentLevelCoordinates = new Vector2Int();
            public Vector2Int _CurrentLevelCoordinates {
                get {
                    lastSeenAt = currentLevelCoordinates;

                    return currentLevelCoordinates;
                }
            }

            [Header("Debug")]
            [SerializeField]
            private Vector2Int lastSeenAt = new Vector2Int(Int32.MaxValue, Int32.MaxValue);
            public Vector2Int _LastSeenAt => lastSeenAt;
        }



        [BigHeader("Basic Properties")]
        [SerializeField]
        private List<FogRevealer> fogRevealers = null;
        public List<FogRevealer> _FogRevealers => fogRevealers;
        [SerializeField]
        private Transform levelMidPoint = null;
        public Transform _LevelMidPoint => levelMidPoint;
        [SerializeField]
        [Range(1, 30)]
        private float FogRefreshRate = 10;

        [BigHeader("Fog Properties")]
        [SerializeField]
        [Range(0, 100)]
        private float fogPlaneHeight = 1;
        [SerializeField]
        private Material fogPlaneMaterial = null;
        [SerializeField]
        private Color fogColor = new Color32(5, 15, 25, 255);
        [SerializeField]
        [Range(0, 1)]
        private float fogPlaneAlpha = 1;
        [SerializeField]
        [Range(1, 5)]
        private float fogLerpSpeed = 2.5f;
        public bool keepRevealedTiles = false;
        [ShowIf("keepRevealedTiles")]
        [Range(0, 1)]
        public float revealedTileOpacity = 0.5f;
        [Header("Debug")]
        [SerializeField]
        private Texture2D fogPlaneTextureLerpTarget = null;
        [SerializeField]
        public Texture2D fogPlaneTextureLerpBuffer = null;

        [BigHeader("Level Data")]
        [SerializeField]
        private TextAsset LevelDataToLoad = null;
        [SerializeField]
        private bool saveDataOnScan = true;
        [ShowIf("saveDataOnScan")]
        [SerializeField]
        private string levelNameToSave = "Default";

        [BigHeader("Scan Properties")]
        [SerializeField]
        [Range(1, 128)]
        [Tooltip("If you need more than 128 units, consider using raycasting-based fog modules instead.")]
        private int levelDimensionX = 11;
        [SerializeField]
        [Range(1, 128)]
        [Tooltip("If you need more than 128 units, consider using raycasting-based fog modules instead.")]
        private int levelDimensionY = 11;
        [SerializeField]
        private float unitScale = 1;
        public float _UnitScale => unitScale;
        [SerializeField]
        private float scanSpacingPerUnit = 0.25f;
        [SerializeField]
        private float rayStartHeight = 5;
        [SerializeField]
        private float rayMaxDistance = 10;
        [SerializeField]
        private LayerMask obstacleLayers = new LayerMask();
        [SerializeField]
        private bool ignoreTriggers = true;

        [BigHeader("Debug Options")]
        [SerializeField]
        private bool drawGizmos = false;
        [SerializeField]
        private bool LogOutOfRange = false;

        // External shadowcaster module
        public Shadowcaster shadowcaster { get; private set; } = new Shadowcaster();

        public LevelData levelData { get; private set; } = new LevelData();

        // The primitive plane which will act as a mesh for rendering the fog with
        private GameObject fogPlane = null;

        private float FogRefreshRateTimer = 0;

        private const string levelScanDataPath = "/LevelData";



        // --- --- ---



        private void Start()
        {
            CheckProperties();

            InitializeVariables();

            if (LevelDataToLoad == null)
            {
                ScanLevel();

                if (saveDataOnScan == true)
                {
                    // Preprocessor definitions are used because the save function code will be stripped out on build
#if UNITY_EDITOR
                    SaveScanAsLevelData();
#endif
                }
            }
            else
            {
                LoadLevelData();
            }

            InitializeFog();

            // This part passes the needed references to the shadowcaster
            shadowcaster.Initialize(this);

            // This is needed because we do not update the fog when there's no unit-scale movement of each fogRevealer
            ForceUpdateFog();
        }



        private void Update()
        {
            UpdateFog();
        }



        // --- --- ---


        private void CheckProperties()
        {
            foreach (FogRevealer fogRevealer in fogRevealers)
            {
                if (fogRevealer._RevealerTransform == null)
                {
                    Debug.LogErrorFormat("Please assign a Transform component to each Fog Revealer!");
                }
            }

            if (unitScale <= 0)
            {
                Debug.LogErrorFormat("Unit Scale must be bigger than 0!");
            }

            if (scanSpacingPerUnit <= 0)
            {
                Debug.LogErrorFormat("Scan Spacing Per Unit must be bigger than 0!");
            }

            if (levelMidPoint == null)
            {
                Debug.LogErrorFormat("Please assign the Level Mid Point property!");
            }

            if (fogPlaneMaterial == null)
            {
                Debug.LogErrorFormat("Please assign the \"FogPlane\" material to the Fog Plane Material property!");
            }
        }



        private void InitializeVariables()
        {
            // This is for faster development iteration purposes
            if (obstacleLayers.value == 0)
            {
                obstacleLayers = LayerMask.GetMask("Default");
            }

            // This is also for faster development iteration purposes
            if (levelNameToSave == String.Empty)
            {
                levelNameToSave = "Default";
            }
        }



        private void InitializeFog()
        {
            fogPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);

            fogPlane.name = "[RUNTIME] Fog_Plane";
            fogPlane.layer = 8;

            fogPlane.transform.position = new Vector3(0,fogPlaneHeight,0);

            fogPlane.transform.localScale = new Vector3(
                (levelDimensionX * unitScale) / 10.0f,
                1,
                (levelDimensionY * unitScale) / 10.0f);

            fogPlaneTextureLerpTarget = new Texture2D(levelDimensionX, levelDimensionY);
            fogPlaneTextureLerpBuffer = new Texture2D(levelDimensionX, levelDimensionY);

            fogPlaneTextureLerpBuffer.wrapMode = TextureWrapMode.Clamp;

            fogPlaneTextureLerpBuffer.filterMode = FilterMode.Bilinear;

            fogPlane.GetComponent<MeshRenderer>().material = new Material(fogPlaneMaterial);

            fogPlane.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", fogPlaneTextureLerpBuffer);

            fogPlane.GetComponent<MeshCollider>().enabled = true;
        }



        private void ForceUpdateFog()
        {
            UpdateFogField();

            Graphics.CopyTexture(fogPlaneTextureLerpTarget, fogPlaneTextureLerpBuffer);
        }



        private void UpdateFog()
        {
            fogPlane.transform.position = new Vector3(
                levelMidPoint.position.x,
                levelMidPoint.position.y + fogPlaneHeight,
                levelMidPoint.position.z);

            FogRefreshRateTimer += Time.deltaTime;

            if (FogRefreshRateTimer < 1 / FogRefreshRate)
            {
                UpdateFogPlaneTextureBuffer();

                return;
            }
            else
            {
                // This is to cancel out minor excess values
                FogRefreshRateTimer -= 1 / FogRefreshRate;
            }

            foreach (FogRevealer fogRevealer in fogRevealers)
            {
                if (fogRevealer._UpdateOnlyOnMove == false)
                {
                    break;
                }

                Vector2Int currentLevelCoordinates = fogRevealer.GetCurrentLevelCoordinates(this);

                if (currentLevelCoordinates != fogRevealer._LastSeenAt)
                {
                    break;
                }

                if (fogRevealer == fogRevealers.Last())
                {
                    return;
                }
            }

            UpdateFogField();

            UpdateFogPlaneTextureBuffer();
        }



        private void UpdateFogField()
        {
            shadowcaster.ResetTileVisibility();

            foreach (FogRevealer fogRevealer in fogRevealers)
            {
                fogRevealer.GetCurrentLevelCoordinates(this);

                shadowcaster.ProcessLevelData(
                    fogRevealer._CurrentLevelCoordinates,
                    Mathf.RoundToInt(fogRevealer._SightRange / unitScale));
            }

            UpdateFogPlaneTextureTarget();
        }



        // Doing shader business on the script, if we pull this out as a shader pass, same operations must be repeated
        private void UpdateFogPlaneTextureBuffer()
        {
            Color[] bufferPixels = fogPlaneTextureLerpBuffer.GetPixels();
            Color[] targetPixels = fogPlaneTextureLerpTarget.GetPixels();

            if (bufferPixels.Length != targetPixels.Length)
            {
                Debug.LogErrorFormat("Fog plane texture buffer and target have different pixel counts");
                return;
            }

            for (int i = 0; i < bufferPixels.Length; i++)
            {
                bufferPixels[i] = Color.Lerp(bufferPixels[i], targetPixels[i], fogLerpSpeed * Time.deltaTime);
            }
            
            fogPlaneTextureLerpBuffer.SetPixels(bufferPixels);

            fogPlaneTextureLerpBuffer.Apply();
        }



        private void UpdateFogPlaneTextureTarget()
        {
            fogPlane.GetComponent<MeshRenderer>().material.SetColor("_Color", fogColor);

            fogPlaneTextureLerpTarget.SetPixels(shadowcaster.fogField.GetColors(fogPlaneAlpha, this));

            fogPlaneTextureLerpTarget.Apply();
        }



        private void ScanLevel()
        {
            Debug.LogFormat("There is no level data file assigned, scanning level...");

            // These operations have no real computational meaning, but it will bring consistency to the data
            levelData.levelDimensionX = levelDimensionX;
            levelData.levelDimensionY = levelDimensionY;
            levelData.unitScale = unitScale;
            levelData.scanSpacingPerUnit = scanSpacingPerUnit;

            for (int xIterator = 0; xIterator < levelDimensionX; xIterator++)
            {
                // Adding a new list for column (y axis) for each unit in row (x axis)
                levelData.AddColumn(new LevelColumn(Enumerable.Repeat(LevelColumn.ETileState.Empty, levelDimensionY)));

                for (int yIterator = 0; yIterator < levelDimensionY; yIterator++)
                {
                    bool isObstacleHit = Physics.BoxCast(
                        new Vector3(
                            GetWorldX(xIterator),
                            levelMidPoint.position.y + rayStartHeight,
                            GetWorldY(yIterator)),
                        new Vector3(
                            (unitScale - scanSpacingPerUnit) / 2.0f,
                            unitScale / 2.0f,
                            (unitScale - scanSpacingPerUnit) / 2.0f),
                        Vector3.down,
                        Quaternion.identity,
                        rayMaxDistance,
                        obstacleLayers,
                        (QueryTriggerInteraction)(2 - Convert.ToInt32(ignoreTriggers)));

                    if (isObstacleHit == true)
                    {
                        levelData[xIterator][yIterator] = LevelColumn.ETileState.Obstacle;
                    }
                }
            }

            Debug.LogFormat("Successfully scanned level with a scale of {0} x {1}", levelDimensionX, levelDimensionY);
        }



        // We intend to use Application.dataPath only for accessing project files directory (only in unity editor)
#if UNITY_EDITOR
        private void SaveScanAsLevelData()
        {
            string fullPath = Application.dataPath + levelScanDataPath + "/" + levelNameToSave + ".json";

            if (Directory.Exists(Application.dataPath + levelScanDataPath) == false)
            {
                Directory.CreateDirectory(Application.dataPath + levelScanDataPath);

                Debug.LogFormat("level scan data folder at \"{0}\" is missing, creating...", levelScanDataPath);
            }

            if (File.Exists(fullPath) == true)
            {
                Debug.LogFormat("level scan data already exists, overwriting...");
            }

            string levelJson = JsonUtility.ToJson(levelData);

            File.WriteAllText(fullPath, levelJson);

            Debug.LogFormat("Successfully saved level scan data at \"{0}\"", fullPath);
        }
#endif



        private void LoadLevelData()
        {
            Debug.LogFormat("Level scan data with a name of \"{0}\" is assigned, loading...", LevelDataToLoad.name);

            // Exception check is indirectly performed through branching on the upper part of the code
            string levelJson = LevelDataToLoad.ToString();

            levelData = JsonUtility.FromJson<LevelData>(levelJson);

            levelDimensionX = levelData.levelDimensionX;
            levelDimensionY = levelData.levelDimensionY;
            unitScale = levelData.unitScale;
            scanSpacingPerUnit = levelData.scanSpacingPerUnit;

            Debug.LogFormat("Successfully loaded level scan data with the name of \"{0}\"", LevelDataToLoad.name);
        }



        /// Adds a new FogRevealer instance to the list and returns its index
        public int AddFogRevealer(FogRevealer fogRevealer)
        {
            if(!fogRevealers.Contains(fogRevealer))
            {
                fogRevealers.Add(fogRevealer);
            }
            return fogRevealers.Count - 1;
        }



        /// Removes a FogRevealer instance from the list with index
        public void RemoveFogRevealer(GameObject target)
        {
            for(int i=fogRevealers.Count - 1; i >= 0; i--)
            {
                if(fogRevealers[i]._RevealerTransform != null && fogRevealers[i]._RevealerTransform.gameObject == target)
                {
                    Debug.Log("제거 : " + target.name);
                    fogRevealers.RemoveAt(i);
                    return;
                }
            }
        }



        /// Replaces the FogRevealer list with the given one
        public void ReplaceFogRevealerList(List<FogRevealer> fogRevealers)
        {
            this.fogRevealers = fogRevealers;
        }



        /// Checks if the given level coordinates are within level dimension range.
        public bool CheckLevelGridRange(Vector2Int levelCoordinates)
        {
            bool result =
                levelCoordinates.x >= 0 &&
                levelCoordinates.x < levelData.levelDimensionX &&
                levelCoordinates.y >= 0 &&
                levelCoordinates.y < levelData.levelDimensionY;

            if (result == false && LogOutOfRange == true)
            {
                Debug.LogFormat("Level coordinates \"{0}\" is out of grid range", levelCoordinates);
            }

            return result;
        }



        /// Checks if the given world coordinates are within level dimension range.
        public bool CheckWorldGridRange(Vector3 worldCoordinates)
        {
            Vector2Int levelCoordinates = WorldToLevel(worldCoordinates);

            return CheckLevelGridRange(levelCoordinates);
        }



        /// Checks if the given pair of world coordinates and additionalRadius is visible by FogRevealers.
        public bool CheckVisibility(Vector3 worldCoordinates, int additionalRadius)
        {
            Vector2Int levelCoordinates = WorldToLevel(worldCoordinates);

            if (additionalRadius == 0)
            {
                return shadowcaster.fogField[levelCoordinates.x][levelCoordinates.y] == 
                    Shadowcaster.LevelColumn.ETileVisibility.Revealed;
            }

            int scanResult = 0;

            for (int xIterator = -1; xIterator < additionalRadius + 1; xIterator++)
            {
                for (int yIterator = -1; yIterator < additionalRadius + 1; yIterator++)
                {
                    if (CheckLevelGridRange(new Vector2Int(
                        levelCoordinates.x + xIterator, 
                        levelCoordinates.y + yIterator)) == false)
                    {
                        scanResult = 0;

                        break;
                    }

                    scanResult += Convert.ToInt32(
                        shadowcaster.fogField[levelCoordinates.x + xIterator][levelCoordinates.y + yIterator] == 
                        Shadowcaster.LevelColumn.ETileVisibility.Revealed);
                }
            }

            if (scanResult > 0)
            {
                return true;
            }

            return false;
        }



        /// Converts unit (divided by unitScale, then rounded) world coordinates to level coordinates.
        public Vector2Int WorldToLevel(Vector3 worldCoordinates)
        {
            Vector2Int unitWorldCoordinates = GetUnitVector(worldCoordinates);

            return new Vector2Int(
                unitWorldCoordinates.x + (levelDimensionX / 2),
                unitWorldCoordinates.y + (levelDimensionY / 2));
        }



        /// Converts level coordinates into world coordinates.
        public Vector3 GetWorldVector(Vector2Int worldCoordinates)
        {
            return new Vector3(
                GetWorldX(worldCoordinates.x + (levelDimensionX / 2)), 
                0, 
                GetWorldY(worldCoordinates.y + (levelDimensionY / 2)));
        }



        /// Converts "pure" world coordinates into unit world coordinates.
        public Vector2Int GetUnitVector(Vector3 worldCoordinates)
        {
            return new Vector2Int(GetUnitX(worldCoordinates.x), GetUnitY(worldCoordinates.z));
        }



        /// Converts level coordinate to corresponding unit world coordinates.
        public float GetWorldX(int xValue)
        {
            if (levelData.levelDimensionX % 2 == 0)
            {
                return (levelMidPoint.position.x - ((levelDimensionX / 2.0f) - xValue) * unitScale);
            }

            return (levelMidPoint.position.x - ((levelDimensionX / 2.0f) - (xValue + 0.5f)) * unitScale);
        }



        /// Converts world coordinate to unit world coordinates.
        public int GetUnitX(float xValue)
        {
            return Mathf.RoundToInt((xValue - levelMidPoint.position.x) / unitScale);
        }



        /// Converts level coordinate to corresponding unit world coordinates.
        public float GetWorldY(int yValue)
        {
            if (levelData.levelDimensionY % 2 == 0)
            {
                return (levelMidPoint.position.z - ((levelDimensionY / 2.0f) - yValue) * unitScale);
            }

            return (levelMidPoint.position.z - ((levelDimensionY / 2.0f) - (yValue + 0.5f)) * unitScale);
        }



        /// Converts world coordinate to unit world coordinates.
        public int GetUnitY(float yValue)
        {
            return Mathf.RoundToInt((yValue - levelMidPoint.position.z) / unitScale);
        }



#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Application.isPlaying == false)
            {
                return;
            }

            if (drawGizmos == false)
            {
                return;
            }

            Handles.color = Color.yellow;

            for (int xIterator = 0; xIterator < levelDimensionX; xIterator++)
            {
                for (int yIterator = 0; yIterator < levelDimensionY; yIterator++)
                {
                    if (levelData[xIterator][yIterator] == LevelColumn.ETileState.Obstacle)
                    {
                        if (shadowcaster.fogField[xIterator][yIterator] == Shadowcaster.LevelColumn.ETileVisibility.Revealed)
                        {
                            Handles.color = Color.green;
                        }
                        else
                        {
                            Handles.color = Color.red;
                        }

                        Handles.DrawWireCube(
                            new Vector3(
                                GetWorldX(xIterator),
                                levelMidPoint.position.y,
                                GetWorldY(yIterator)),
                            new Vector3(
                                unitScale - scanSpacingPerUnit,
                                unitScale,
                                unitScale - scanSpacingPerUnit));
                    }
                    else
                    {
                        Gizmos.color = Color.yellow;

                        Gizmos.DrawSphere(
                            new Vector3(
                                GetWorldX(xIterator),
                                levelMidPoint.position.y,
                                GetWorldY(yIterator)),
                            unitScale / 5.0f);
                    }

                    if (shadowcaster.fogField[xIterator][yIterator] == Shadowcaster.LevelColumn.ETileVisibility.Revealed)
                    {
                        Gizmos.color = Color.green;

                        Gizmos.DrawSphere(
                            new Vector3(
                                GetWorldX(xIterator),
                                levelMidPoint.position.y,
                                GetWorldY(yIterator)),
                            unitScale / 3.0f);
                    }
                }
            }
        }
#endif
    }



    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public string _BaseCondition {
            get { return mBaseCondition; }
        }

        private string mBaseCondition = String.Empty;

        public ShowIfAttribute(string baseCondition)
        {
            mBaseCondition = baseCondition;
        }
    }



    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class BigHeaderAttribute : PropertyAttribute
    {
        public string _Text {
            get { return mText; }
        }

        private string mText = String.Empty;

        public BigHeaderAttribute(string text)
        {
            mText = text;
        }
    }



}