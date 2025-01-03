using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingController : MonoBehaviour
{
    public Dictionary<int ,Building> buildingList = new Dictionary<int, Building>();
    private int _buildingID;
    private string _teamID;
    private Dictionary<string, GameObject> _buildingPrefabs = new Dictionary<string, GameObject>();
    public GameObject buildingObject;
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

    public void CreateBuilding(Vector3 buildingLocation, string buildingType)
    // 건물 생성
    {
        // 객체 생성
        buildingObject = PhotonNetwork.Instantiate($"Prefabs/Buildings/{_teamID}TeamBuildings/{buildingType}",buildingLocation, Quaternion.Euler(new Vector3(-90, 90, 0)));
        buildingObject.name = buildingType + _buildingID.ToString(); // 새로 생성될 오브젝트에 고유한 이름을 붙여줌
        GameObject gameObject = buildingObject; // SetClickedObject에 넣을 임의 변수 만듦 -> call by value로 되기 떄문에 buildingObject가 바뀌어도 값이 안바뀜
        // 좌클릭 했을 때 callback 함수 넣어줌
        buildingObject.GetComponent<ClickEventHandler>().leftClickDownEvent.AddListener((Vector3 pos) => GameManager.instance.SetClickedObject(gameObject));
        switch(buildingType)
        {
            case "Command":
                // UI 바뀌는 callback 함수 넣어줌
                buildingObject.GetComponent<ClickEventHandler>().leftClickDownEvent.AddListener((Vector3 pos) => GameManager.instance.ChangeUI(1));

                // Command 객체를 넣어준다. -> 오브젝트를 통해서 건물의 정보를 알 수 있게 하기위해
                Command _newCommand = buildingObject.AddComponent<Command>();
                // Command 정보 초기화
                _newCommand.Init(_teamID, _buildingID, buildingLocation);
                
                // Dictionary에 추가
                buildingList.Add(_buildingID, _newCommand);
                break;
            case "Barrack":
                // UI 바뀌는 callback 함수 넣어줌
                buildingObject.GetComponent<ClickEventHandler>().leftClickDownEvent.AddListener((Vector3 pos) => GameManager.instance.ChangeUI(2));

                // Barrack 객체를 넣어준다. -> 오브젝트를 통해서 건물의 정보를 알 수 있게 하기위해
                Barrack _newBarrack = buildingObject.AddComponent<Barrack>();
                // 배럭 정보 초기화
                _newBarrack.Init(_teamID, _buildingID, buildingLocation);
                
                // Dictionary에 추가
                buildingList.Add(_buildingID, _newBarrack);
                break;
            case "PopulationBuilding":
                buildingObject.GetComponent<ClickEventHandler>().leftClickDownEvent.AddListener((Vector3 pos) => GameManager.instance.ChangeUI(3));

                PopulationBuilding _newPop = buildingObject.AddComponent<PopulationBuilding>();
                _newPop.Init(_teamID, _buildingID, buildingLocation);

                buildingList.Add(_buildingID, _newPop);
                break;
            case "ResourceBuilding":
                buildingObject.GetComponent<ClickEventHandler>().leftClickDownEvent.AddListener((Vector3 pos) => GameManager.instance.ChangeUI(4));

                ResourceBuilding _newResource = buildingObject.AddComponent<ResourceBuilding>();
                _newResource.Init(_teamID, _buildingID, buildingLocation);
                buildingList.Add(_buildingID, _newResource);
                break;
            case "Defender":
                buildingObject.GetComponent<ClickEventHandler>().leftClickDownEvent.AddListener((Vector3 pos) => GameManager.instance.ChangeUI(5));

                Defender _newDefender = buildingObject.AddComponent<Defender>();
                _newDefender.Init(_teamID, _buildingID, buildingLocation);
                buildingList.Add(_buildingID, _newDefender);
                break;
        }
        _buildingID ++;
        // printList(buildingList);
    }

    // 디버깅용
    public void PrintList(Dictionary<int, Building> buildings)
    {
        for(int i=0; i<_buildingID; i++)
        {
            Debug.Log(buildings[i].buildingType);
        }
    }

    public void BuildingAttacked(int buildingID, int damage)
    {
        Building selectedBuilding = buildingList[buildingID];

    }

    public void DestroyBuilding(int buildingID)
    {

    }
    
    public void UpgradeBuilding()
    {
        Building building = GameManager.instance.selectedObject.GetComponent<Building>();
        building.buildingLevel++;
    }
}
