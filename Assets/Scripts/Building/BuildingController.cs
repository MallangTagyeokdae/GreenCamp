using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    public  List<Building> buildingList = new List<Building>();
    private int _buildingCount;
    private Dictionary<int, GameObject> buildingPrefabs;
    public void Start()
    {
        _buildingCount = 0;
        buildingPrefabs = new Dictionary<int, GameObject>()
        {
            {0, Resources.Load<GameObject>("Prefabs/buildings/Command")}
        };
    }

    public void createBuilding(Vector3 buildingLocation)
    // 건물 생성
    {
        if(buildingPrefabs.ContainsKey(0)) // 건물 Prefab에 해당 건물이 있으면
        {
            GameObject buildingObject = Instantiate(buildingPrefabs[_buildingCount],buildingLocation, Quaternion.identity);
            if(0 == 0) // Command 생성
            {
                // Command 객체 생성
                Command newBuilding = new Command("Blue", _buildingCount, 0, buildingLocation, 500, 0, 10, 5 );
                buildingList.Add(newBuilding);
            }
        }
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
