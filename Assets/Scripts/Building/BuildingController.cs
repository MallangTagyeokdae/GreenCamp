using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    public  List<Building> buildingList = new List<Building>();
    private int _buildingCount;
    private Dictionary<string, GameObject> buildingPrefabs;
    
    public void Start()
    {
        _buildingCount = 0;
         buildingPrefabs = new Dictionary<string, GameObject>()
         {
             {"Command", Resources.Load<GameObject>("Prefabs/Buildings/Command")}
         };
    }

    public void createBuilding(Vector3 buildingLocation, string buildingType)
    // 건물 생성
    {
        GameObject buildingObject = Instantiate(buildingPrefabs["Command"],buildingLocation, Quaternion.Euler(new Vector3(-90, 90, 0)));
        
        switch(buildingType)
        {
            case "Command":
                // Command 객체 생성
                Command newBuilding = new Command("Blue", 0, buildingLocation);
                buildingList.Add(newBuilding);
                break;
            case "Barrack":
                // Barrack 생성 (유닛 생성 건물)
            
                break;
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
