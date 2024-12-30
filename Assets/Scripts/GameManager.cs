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
    public UIController uIController;
    public BuildingController _buildingController;
    public UnitController unitController;
    public Building selectedBuilding;
    public Unit selectedUnit;
    public void createBuilding(Vector3 buildingPos)
    {
        // if(GameStatus.instance.canCreateBuilding(selectedBuilding))
        // { // 건물을 생성할 수 있음
        // Debug.Log(buildingPos);
            _buildingController.createBuilding(buildingPos,"Command");
        // }
    }
    public void createUnit(Vector3 unitPos) // 해윤
    {
        unitController.createUnit(unitPos, "Archer");
    }
}
