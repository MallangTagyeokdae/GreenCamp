using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager instance{
        get{
            if(_instance == null){
                _instance = FindObjectOfType<GameManager>();
                return _instance;
            }

            return _instance;
        }
    }
    public List<GameObject> gameStates;
    
    //------------ 해윤 ------------
    public UIController uIController;
    public UnitController unitController;
    public Unit selectedUnit;
    //-----------------------------
    
    //------------ 준현 ------------
    public BuildingController buildingController; // 준현
    public Building selectedBuilding; // 준현
    public String buildingType;
    public GameObject grid;
    //-----------------------------
    
    //---------------- 준현 --------------------
    public void createBuilding(Vector3 buildingPos)
    {
        // if(GameStatus.instance.canCreateBuilding(selectedBuilding))
        // { // 건물을 생성할 수 있음
        Debug.Log(buildingPos);
        buildingController.createBuilding(buildingPos, buildingType);
        grid.SetActive(false);
        // }
    }

    public void setBuildingType(string buildingType)
    {
        this.buildingType = buildingType;
    }   
    //------------------------------------
    public void createUnit(Vector3 unitPos) // 해윤
    {
        unitController.createUnit(unitPos, "Archer");
    }
}