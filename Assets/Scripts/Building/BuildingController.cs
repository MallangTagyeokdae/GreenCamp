using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    public  List<Building> buildingList;
    private int _buildingCount;
    public void Initialize()
    {
        _buildingCount = 0;
    }

    public void createBuilding(Building building)
    // 건물 생성
    {
        buildingList.Add(building);
    }

    public void buildingAttacked(int buildingID, int damage)
    {
        Building selectedBuilding = buildingList[buildingID];

    }

    public void destroyBuilding(int buildingID)
    {

    }
    
    public void upgradeBuilding(int buildingID)
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
