using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingController : MonoBehaviour
{
    public Dictionary<int ,Building> buildingList = new Dictionary<int, Building>();
    private int _buildingID;
    private string _teamID;
    private Dictionary<string, GameObject> _buildingPrefabs = new Dictionary<string, GameObject>();
    public void Start()
    {
        _buildingID = 0;

        _teamID = GameStatus.instance.teamID;

        // 폴더 경로에 있는 모든 GameObject를 Load해 unitPrefabsList에 임시로 저장
        List<GameObject> buildingPrefabsList = new List<GameObject>(Resources.LoadAll<GameObject>($"Prefabs/Buildings/{_teamID}TeamBuildings"));

        // foreach문으로 List 안에 있는 building 객체들을 다시 Dictionary로 저장
        foreach (GameObject building in buildingPrefabsList)
        {
            if (!_buildingPrefabs.ContainsKey(building.name))
            {
                _buildingPrefabs.Add(building.name, building);
            }
        }
        
    }

    public void createBuilding(Vector3 buildingLocation, string buildingType)
    // 건물 생성
    {
        GameObject buildingObject = Instantiate(_buildingPrefabs[buildingType],buildingLocation, Quaternion.Euler(new Vector3(-90, 90, 0)));
        switch(buildingType)
        {
            case "Command":
                // Command 객체 생성
                Command _newCommand = new Command(_teamID, 0, buildingLocation);
                buildingList.Add(_buildingID, _newCommand);
                break;
            case "Barrack":
                buildingObject.GetComponent<ClickEventHandler>().leftClickDownEvent.AddListener((Vector3 pos) => GameManager.instance.ChangeUI(1));
                Barrack _newBarrack = new Barrack(_teamID, 0, buildingLocation);
                buildingList.Add(_buildingID, _newBarrack);
                break;
            case "PopulationBuilding":
                PopulationBuilding _newPopulationBuilding = new PopulationBuilding(_teamID, 0, buildingLocation);
                buildingList.Add(_buildingID, _newPopulationBuilding);
                break;
            case "ResourceBuilding":
                ResourceBuilding _newResourceBuilding = new ResourceBuilding(_teamID, 0, buildingLocation);
                buildingList.Add(_buildingID, _newResourceBuilding);
                break;
            case "Defender":
                Defender _newDefender = new Defender(_teamID, 0, buildingLocation);
                buildingList.Add(_buildingID, _newDefender);
                break;
        }
        _buildingID ++;
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
