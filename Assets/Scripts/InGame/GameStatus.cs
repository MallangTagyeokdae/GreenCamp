using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.Properties;
using Unity.VisualScripting;
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
    public float currentResourceCount { get; set; }
    public float resourcePerSecond { get; set; }
    public int maxUnitCount { get; set; }
    public int currentUnitCount { get; set; }
    public int maxBuildingCount { get; set; }
    public int currentBuildingCount { get; set; }
    public bool isWin { get; set; }
    public bool isDamageUpgrade { get; set; }
    public bool isArmorUpgrade { get; set; }
    public bool isHealthUpgrade { get; set; }
    public int damageIncrease { get; set; }
    public int armorIncrease { get; set; }
    public int healthIncrease { get; set; }
    public int damageLevel { get; set; }
    public int armorLevel { get; set; }
    public int healthLevel { get; set; }
    public int damageUpgradeCost { get; set; }
    public int armorUpgradeCost { get; set; }
    public int healthUpgradeCost { get; set; }

    public void InitGameStatus()
    {
        teamID = PhotonManager.instance.GetTeam(PhotonNetwork.LocalPlayer);
        maxResourceCount = 10000;
        currentResourceCount = 10000;
        resourcePerSecond = 1;
        maxUnitCount = 10;
        currentUnitCount = 0;
        maxBuildingCount = 1;
        currentBuildingCount = 0;
        isWin = true;
        isDamageUpgrade = false;
        isArmorUpgrade = false;
        isHealthUpgrade = false;
        this.damageLevel = 1;
        this.armorLevel = 1;
        this.healthLevel = 1;
        armorIncrease = 0;
        damageIncrease = 0;
        healthIncrease = 0;
        damageUpgradeCost = 30;
        armorUpgradeCost = 30;
        healthUpgradeCost = 30;
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

    [PunRPC]
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

    public bool CanCreate(string objectName, string objectType) // 생성할 객체의 이름, 객체의 종류를 받아서 생성가능 여부를 리턴
    {
        int[] data = CheckObjName(objectName);
        if (currentResourceCount < data[0])
        {
            Debug.Log("자원부족");
            return false;
        }
        switch (objectType)
        {
            case "Unit":
                if (maxUnitCount < currentUnitCount + data[1])
                {
                    Debug.Log("인구수 부족");
                    return false;
                }
                break;
            case "Building":
                if (maxBuildingCount < currentBuildingCount + data[1])
                {
                    Debug.Log("건물 인구수 부족");
                    return false;
                }
                break;
        }
        return true;
    }

    public bool CanLevelUp(Building building, int commandLevel)
    {
        if (building.level > 5)
        {
            Debug.Log("최대레벨");
            return false;
        }
        else if (currentResourceCount < building.levelUpCost)
        {
            Debug.Log("자원 부족");
            return false;
        }
        else if (building.GetComponent<Command>())
        {
            return true;
        }
        else if (!building.GetComponent<Command>() && (building.level <= commandLevel))
        {
            return true;
        }
        Debug.Log("건물레벨 <= 커멘드 레벨이 되어야함");
        return false;
    }

    public bool CanUpgradeUnit(Building building, int level, int cost, bool isProgressing)
    {   
        if(!isProgressing && (building.level >= level))
        {
            if(currentResourceCount >= cost)
            {
                return true;
            }
            Debug.Log("자원 부족");
            return false;
        }
        Debug.Log("업그레이드 레벨 <= 건물 레벨이 되어야함");
        return false;
    }

    public void SetResources(string objectName, string objectType)
    {
        int[] data = CheckObjName(objectName);

        currentResourceCount -= data[0];

        switch (objectType)
        {
            case "Unit":
                currentUnitCount += data[1];
                break;
            case "Building":
                currentBuildingCount += data[1];
                break;
        }
    }

    public int[] CheckObjName(string objectName) // 생성할 객체의 이름을 받아 종류에 따른 비용, 인구수를 담은 배열을 리턴함
    {
        int[] data = new int[2]; // 0: cost / 1: population
        switch (objectName)
        {
            case "Soldier":
                data[0] = 30;
                data[1] = 1;
                break;
            case "Archer":
                data[0] = 40;
                data[1] = 1;
                break;
            case "Tankder":
                data[0] = 50;
                data[1] = 2;
                break;
            case "Healer":
                data[0] = 65;
                data[1] = 2;
                break;
            case "Scout":
                data[0] = 20;
                data[1] = 1;
                break;
            case "Barrack":
                data[0] = 50;
                data[1] = 1;
                break;
            case "PopulationBuilding":
                data[0] = 25;
                data[1] = 1;
                break;
            case "ResourceBuilding":
                data[0] = 45;
                data[1] = 1;
                break;
            case "Defender":
                data[0] = 25;
                data[1] = 1;
                break;
            case "Academy":
                data[0] = 40;
                data[1] = 2;
                break;
        }
        return data;
    }
    // ------------------------------------------------

}
