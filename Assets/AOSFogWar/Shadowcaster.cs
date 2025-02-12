/*
 * Created :    Winter 2022
 * Author :     SeungGeon Kim (keithrek@hanmail.net)
 * Project :    FogWar
 * Filename :   Shadowcaster.cs (non-static module)
 * 
 * All Content (C) 2022 Unlimited Fischl Works, all rights reserved.
 */

/*
 * This part of the code utilizes the algorithm known as "Shadowcasting", the FOV algorithm of retro Roguelikes.
 * http://www.adammil.net/blog/v125_Roguelike_Vision_Algorithms.html
 * The subject is wonderfully explained in depth inside the above link.
 * https://www.albertford.com/shadowcasting
 * By the way, this script is based on the above version of the implementation. Be sure to check it out!
 */



using System.Linq;                  // Enumerable
using System.Collections.Generic;   // List
using UnityEngine;                  // Vector2



namespace FischlWorks_FogWar
{



    /// A non-static submodule that performs shadowcasting algorithm over a LevelData object.

    /// An instance of this module is instantiated for each construction of the csFogWar object.\n
    /// While the LevelData class defined within csFogWar class holds the information about obstacles,\n
    /// the FogField class defined within Shadowcaster class holds the information about player sight.
    public class Shadowcaster
    {
        /// A class that holds visibility data updated based on the FOV.
        /// 
        /// This class is only instantiated and managed by a single Shadowcaster object,\n
        /// and the object only lasts per session, unlike the serialized LevelData of csFogWar.\n
        /// This class also has the GetColors() method, which returns the actual texture data in a 1D Color array format. 
        public class FogField
        {
            public void AddColumn(LevelColumn levelColumn)
            {
                levelRow.Add(levelColumn);
            }

            public void Reset(csFogWar fogWar)
            {
                foreach (LevelColumn levelColumn in levelRow)
                {
                    levelColumn.Reset(fogWar);
                }
            }

            public Color[] GetColors(float fogPlaneAlpha, csFogWar fogWar)
            {
                if (colors == null)
                {
                    colors = new Color[levelRow.Count * levelRow[0].Count()];
                }

                for (int xIterator = 0; xIterator < levelRow[0].Count(); xIterator++)
                {
                    for (int yIterator = 0; yIterator < levelRow.Count; yIterator++)
                    {
                        int visibility = (int)levelRow[yIterator][levelRow[0].Count() - 1 - xIterator];

                        float tileOpacity = 1 - visibility;

                        if (fogWar.keepRevealedTiles == true && visibility == (int)LevelColumn.ETileVisibility.PreviouslyRevealed)
                        {
                            tileOpacity = fogWar.revealedTileOpacity;
                        }

                        // The reason that the darker side is the revealed ones is to let users customize fog's color
                        colors[levelRow.Count() * (xIterator + 1) - (yIterator + 1)] =
                        new Color(1, 1, 1, (tileOpacity) * fogPlaneAlpha);
                    }
                }

                return colors;
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

            private List<LevelColumn> levelRow = new List<LevelColumn>();

            // To be initialized with the dependant initialization function call
            private Color[] colors = null;
        }



        public class LevelColumn
        {
            public LevelColumn(IEnumerable<ETileVisibility> visibilityTiles)
            {
                levelColumn = new List<ETileVisibility>(visibilityTiles);
            }

            // This is separated from the LevelColumn class of csHomebrewOfWar due to the size of the level data file
            public enum ETileVisibility
            {
                Hidden,
                Revealed,
                PreviouslyRevealed
            }

            public void Reset(csFogWar fogWar)
            {
                for (int i = 0; i < levelColumn.Count; i++)
                {
                    if (fogWar.keepRevealedTiles == false)
                    {
                        levelColumn[i] = ETileVisibility.Hidden;

                        continue;
                    }

                    if (levelColumn[i] == ETileVisibility.Revealed)
                    {
                        levelColumn[i] = ETileVisibility.PreviouslyRevealed;
                    }
                }
            }

            public int Count()
            {
                return levelColumn.Count;
            }

            // Indexer definition
            public ETileVisibility this[int index] {
                get {
                    if (index >= 0 && index < levelColumn.Count)
                    {
                        return levelColumn[index];
                    }
                    else
                    {
                        Debug.LogErrorFormat("index given in y axis is out of range");

                        return ETileVisibility.Hidden;
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

            private List<ETileVisibility> levelColumn = new List<ETileVisibility>();
        }



        /// An iterator that transforms coordinates based on a single quadrant to all the others.
        /// 
        /// For an explanation in depth, refer to the link below.\n
        /// https://www.albertford.com/shadowcasting
        private class QuadrantIterator
        {
            public QuadrantIterator(csFogWar fogWar)
            {
                this.fogWar = fogWar;
            }

            public enum ECardinal
            {
                East,
                North,
                West,
                South
            }

            public Vector2Int QuadrantToLevel(Vector2Int quadrantVector)
            {
                Vector2Int quadrantPoint = new Vector2Int();

                switch (cardinal)
                {
                    default:
                    case ECardinal.East:
                        quadrantPoint = new Vector2Int(originPoint.x + quadrantVector.x, originPoint.y + quadrantVector.y);
                        break;
                    case ECardinal.North:
                        quadrantPoint = new Vector2Int(originPoint.x - quadrantVector.y, originPoint.y + quadrantVector.x);
                        break;
                    case ECardinal.West:
                        quadrantPoint =  new Vector2Int(originPoint.x - quadrantVector.x, originPoint.y - quadrantVector.y);
                        break;
                    case ECardinal.South:
                        quadrantPoint = new Vector2Int(originPoint.x + quadrantVector.y, originPoint.y - quadrantVector.x);
                        break;
                }

                return fogWar.WorldToLevel(fogWar.GetWorldVector(quadrantPoint));
            }

            public ECardinal cardinal { get; set; } = ECardinal.East;

            public Vector2Int originPoint { get; set; } = new Vector2Int();

            // To be initialized within the constructor, needed for reference
            private csFogWar fogWar = null;
        }



        private class ColumnIterator
        {
            public ColumnIterator(int depth, int maxDepth, float startSlope, float endSlope)
            {
                this.depth = depth;
                this.maxDepth = maxDepth;
                this.startSlope = startSlope;
                this.endSlope = endSlope;
            }

            public List<Vector2Int> GetTiles()
            {
                List<Vector2Int> quadrantPoints = new List<Vector2Int>();

                int minRow = Mathf.RoundToInt(depth * startSlope);
                int maxRow = Mathf.RoundToInt(depth * endSlope);

                for (int i = minRow; i < maxRow + 1; i++)
                {
                    quadrantPoints.Add(new Vector2Int(depth, i));
                }

                if (endSlope == 1)
                {
                    quadrantPoints.RemoveAt(quadrantPoints.Count - 1);
                }

                return quadrantPoints;
            }

            public bool IsProceedable()
            {
                return (depth < maxDepth);
            }

            public void ProceedIfPossible()
            {
                if (depth < maxDepth)
                {
                    depth += 1;
                }
            }

            // In my implementaion, we consider the depth as the x axis of the eastern quadrant
            public int depth { get; private set; } = 0;
            public int maxDepth { get; private set; } = 0;

            // The variable startSlope is the 'lower' one, and the endSlope is the 'higher' one
            public float startSlope { get; set; } = 0;
            public float endSlope { get; set; } = 0;
        }



        // Parent manager module, passed with initialization
        private csFogWar fogWar = null;

        public FogField fogField { get; private set; } = new FogField();

        // We declare this here to prevent creating and destroying the same object over and over
        private QuadrantIterator quadrantIterator = null;



        /// Initializes the shadowcaster module with properties from the dependant csFogWar object.
        public void Initialize(csFogWar fogWar)
        {
            this.fogWar = fogWar;

            for (int xIterator = 0; xIterator < fogWar.levelData.levelDimensionX; xIterator++)
            {
                // Adding a new list for column (y axis) for each unit in row (x axis)
                fogField.AddColumn(new LevelColumn(
                    Enumerable.Repeat(LevelColumn.ETileVisibility.Hidden, fogWar.levelData.levelDimensionY)));
            }

            quadrantIterator = new QuadrantIterator(fogWar);
        }



        /// Resets all tile's visibility info.
        public void ResetTileVisibility()
        {
            fogField.Reset(fogWar);
        }



        /// Processes the level data with shadowcasting algorithm, and updates the FogField object accordingly
        public void ProcessLevelData(Vector2Int revealerPoint, int sightRange)
        {
            // Reveal the first tile where the revealer is at
            RevealTile(fogWar.WorldToLevel(fogWar.GetWorldVector(revealerPoint)));

            // Give the quadrant iterator the revealer's position
            quadrantIterator.originPoint = revealerPoint;

            // We deal with 90 degrees each, anti-clockwise, starting from east to west
            foreach (int cardinal in System.Enum.GetValues(typeof(QuadrantIterator.ECardinal)))
            {
                quadrantIterator.cardinal = (QuadrantIterator.ECardinal)cardinal;

                // Here goes the BFS algorithm, we queue a new pass during each pass if needed, then start the new one
                Queue<ColumnIterator> columnIterators = new Queue<ColumnIterator>();

                // The first pass of the given quadrant, start from slope -1 to slope 1
                columnIterators.Enqueue(new ColumnIterator(1, sightRange, -1, 1));

                while (columnIterators.Count > 0)
                {
                    ColumnIterator columnIterator = columnIterators.Dequeue();

                    // Note that the given points may have negative y values instead of starting from zero
                    List<Vector2Int> quadrantPoints = columnIterator.GetTiles();

                    // This is to detect points where the obstacle tile and the empty tile are adjacent
                    Vector2Int lastQuadrantPoint = new Vector2Int();

                    // This is to skip the first pass where the lastQuadrantPoint variable is not assigned yet
                    bool firstStepFlag = true;

                    foreach (Vector2Int quadrantPoint in quadrantPoints)
                    {
                        if (IsTileObstacle(quadrantPoint) == true || IsTileVisible(columnIterator, quadrantPoint))
                        {
                            RevealTileIteratively(quadrantPoint, sightRange);
                        }

                        if (firstStepFlag == false)
                        {
                            if (IsTileObstacle(lastQuadrantPoint) == true && IsTileEmpty(quadrantPoint) == true)
                            {
                                columnIterator.startSlope = GetQuadrantSlope(quadrantPoint);
                            }

                            if (IsTileEmpty(lastQuadrantPoint) == true && IsTileObstacle(quadrantPoint) == true)
                            {
                                if (columnIterator.IsProceedable() == false)
                                {
                                    continue;
                                }

                                ColumnIterator nextColumnIterator = new ColumnIterator(
                                    columnIterator.depth,
                                    sightRange,
                                    columnIterator.startSlope,
                                    GetQuadrantSlope(quadrantPoint));

                                nextColumnIterator.ProceedIfPossible();

                                columnIterators.Enqueue(nextColumnIterator);
                            }
                        }

                        lastQuadrantPoint = quadrantPoint;

                        firstStepFlag = false;
                    }

                    if (IsTileEmpty(lastQuadrantPoint) == true && columnIterator.IsProceedable() == true)
                    {
                        columnIterator.ProceedIfPossible();

                        columnIterators.Enqueue(columnIterator);
                    }
                }
            }
        }



        private void RevealTileIteratively(Vector2Int quadrantPoint, int sightRange)
        {
            Vector2Int levelCoordinates = quadrantIterator.QuadrantToLevel(quadrantPoint);

            if (fogWar.CheckLevelGridRange(levelCoordinates) == false)
            {
                return;
            }

            if (quadrantPoint.magnitude > sightRange)
            {
                return;
            }

            fogField[levelCoordinates.x][levelCoordinates.y] = LevelColumn.ETileVisibility.Revealed;
        }



        private void RevealTile(Vector2Int levelCoordinates)
        {
            if (fogWar.CheckLevelGridRange(levelCoordinates) == false)
            {
                return;
            }

            fogField[levelCoordinates.x][levelCoordinates.y] = LevelColumn.ETileVisibility.Revealed;
        }



        private bool IsTileEmpty(Vector2Int quadrantPoint)
        {
            Vector2Int levelCoordinates = quadrantIterator.QuadrantToLevel(quadrantPoint);

            if (fogWar.CheckLevelGridRange(levelCoordinates) == false)
            {
                return true;
            }

            return (fogWar.levelData[levelCoordinates.x][levelCoordinates.y] == csFogWar.LevelColumn.ETileState.Empty);
        }



        private bool IsTileObstacle(Vector2Int quadrantPoint)
        {
            Vector2Int levelCoordinates = quadrantIterator.QuadrantToLevel(quadrantPoint);

            if (fogWar.CheckLevelGridRange(levelCoordinates) == false)
            {
                return false;
            }

            return (fogWar.levelData[levelCoordinates.x][levelCoordinates.y] == csFogWar.LevelColumn.ETileState.Obstacle);
        }



        private bool IsTileVisible(ColumnIterator columnIterator, Vector2Int quadrantPoint)
        {
            return (quadrantPoint.y >= columnIterator.depth * columnIterator.startSlope) &&
                (quadrantPoint.y <= columnIterator.depth * columnIterator.endSlope);
        }



        private float GetQuadrantSlope(Vector2Int quadrantPoint)
        {
            // The reason that this is not simply y / x is that the wall is diamond-shaped, refer to the links at the top
            return (((quadrantPoint.y * 2) - 1) / ((float)quadrantPoint.x * 2));
        }
    }



}