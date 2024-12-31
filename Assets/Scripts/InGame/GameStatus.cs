using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public class GameStatus : MonoBehaviour
{
    private void Awake()
    {
        InitGameStatus();
    }
    private static GameStatus _instance;
    public static GameStatus instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameStatus>();
            }

            return _instance;
        }
    }
    public string teamID { get; set; }
    public int maxResourceCount { get; set; }
    public int currentResourceCount { get; set; }
    public int resourcePerSecond { get; set; }
    public int maxUnitCount { get; set; }
    public int currentUnitCount { get; set; }
    public int maxBuildingCount { get; set; }
    public int currentBuildingCount { get; set; }

    public void InitGameStatus()
    {
        teamID = "Blue";
        maxResourceCount = 1000;
        currentResourceCount = 0;
        resourcePerSecond = 5;
        maxUnitCount = 7;
        currentUnitCount = 0;
        maxBuildingCount = 7;
        currentBuildingCount = 0;
    }

    public void IncreaseMaxResourceCount(int count)
    {
        maxResourceCount += count;
    }

    public void IncreaseResourcePerSecond(int count)
    {
        resourcePerSecond += count;
    }

    public void IncreaseMaxUnitCount(int count)
    {
        maxUnitCount += count;
    }

    public void IncreaseMaxBuildingCount(int count)
    {
        maxBuildingCount += count;
    }

    public void UpdateResource()
    {
        currentResourceCount += resourcePerSecond;
    }

    public void UpdateUnitCount()
    {
        currentUnitCount++;
    }

    public void UpdateBuildingCount()
    {
        currentBuildingCount++;
    }

    // ------------------------------------------------
    // Building Create 체크 관련 함수들

    public bool canCreateBuilding(Building building)
    {
        if (this.currentResourceCount < building.buildingCost)
        {
            Debug.Log("자원부족");
            return false;
        }
        if (this.maxBuildingCount < this.currentBuildingCount + 1)
        {
            Debug.Log("건물 인구수 부족");
            return false;
        }
        return true;
    }

    // ------------------------------------------------

}
