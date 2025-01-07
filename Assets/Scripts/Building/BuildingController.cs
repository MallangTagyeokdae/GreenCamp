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
    public Dictionary<int, Building> buildingDictionary = new Dictionary<int, Building>();
    private Dictionary<string, GameObject> _buildingPrefabs = new Dictionary<string, GameObject>();
    private string _teamID;
    private int _buildingID;
    public GameObject buildingObject;
    public void Start()
    {
        _teamID = GameStatus.instance.teamID;
        _buildingID = 0;

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

    public Building CreateBuilding(Vector3 buildingLocation, string buildingType)
    // 건물 생성
    {
        // 객체 생성
        buildingObject = PhotonNetwork.Instantiate($"Prefabs/Buildings/{_teamID}TeamBuildings/{buildingType}", buildingLocation, Quaternion.Euler(new Vector3(-90, 90, 0)));
        buildingObject.name = buildingType + _buildingID.ToString(); // 새로 생성될 오브젝트에 고유한 이름을 붙여줌
        GameObject gameObject = buildingObject; // SetClickedObject에 넣을 임의 변수 만듦 -> call by value로 되기 떄문에 buildingObject가 바뀌어도 값이 안바뀜
        // 좌클릭 했을 때 callback 함수 넣어줌
        buildingObject.GetComponent<ClickEventHandler>().leftClickDownEvent.AddListener((Vector3 pos) => GameManager.instance.SetClickedObject(gameObject));
        Building newBuilding;
        switch (buildingType)
        {
            case "Command":
                // UI 바뀌는 callback 함수 넣어줌
                buildingObject.GetComponent<ClickEventHandler>().leftClickDownEvent.AddListener((Vector3 pos) => GameManager.instance.SetBuildingInfo(2));

                // Command 객체를 넣어준다. -> 오브젝트를 통해서 건물의 정보를 알 수 있게 하기위해
                Command _newCommand = buildingObject.AddComponent<Command>();
                // Command 정보 초기화
                _newCommand.Init(_teamID, _buildingID, buildingLocation);

                // Dictionary에 추가
                buildingDictionary.Add(_buildingID, _newCommand);
                newBuilding = _newCommand;
                newBuilding.progressMesh1 = _newCommand.progressMesh1;
                newBuilding.completeMesh = _newCommand.completeMesh;
                break;

            case "Barrack":
                // UI 바뀌는 callback 함수 넣어줌
                buildingObject.GetComponent<ClickEventHandler>().leftClickDownEvent.AddListener((Vector3 pos) => GameManager.instance.SetBuildingInfo(3));

                // Barrack 객체를 불러온다. -> 오브젝트를 통해서 건물의 정보를 알 수 있게 하기위해
                Barrack _newBarrack = buildingObject.GetComponent<Barrack>();
                // 배럭 정보 초기화
                _newBarrack.Init(_teamID, _buildingID, buildingLocation);

                // Dictionary에 추가
                buildingDictionary.Add(_buildingID, _newBarrack);
                newBuilding = _newBarrack;
                newBuilding.progressMesh1 = _newBarrack.progressMesh1;
                newBuilding.completeMesh = _newBarrack.completeMesh;
                break;
            case "PopulationBuilding":
                buildingObject.GetComponent<ClickEventHandler>().leftClickDownEvent.AddListener((Vector3 pos) => GameManager.instance.SetBuildingInfo(4));

                PopulationBuilding _newPop = buildingObject.GetComponent<PopulationBuilding>();
                _newPop.Init(_teamID, _buildingID, buildingLocation);

                buildingDictionary.Add(_buildingID, _newPop);
                newBuilding = _newPop;
                newBuilding.progressMesh1 = _newPop.progressMesh1;
                newBuilding.completeMesh = _newPop.completeMesh;
                break;
            case "ResourceBuilding":
                buildingObject.GetComponent<ClickEventHandler>().leftClickDownEvent.AddListener((Vector3 pos) => GameManager.instance.SetBuildingInfo(5));

                ResourceBuilding _newResource = buildingObject.GetComponent<ResourceBuilding>();
                _newResource.Init(_teamID, _buildingID, buildingLocation);

                buildingDictionary.Add(_buildingID, _newResource);
                newBuilding = _newResource;
                newBuilding.progressMesh1 = _newResource.progressMesh1;
                newBuilding.completeMesh = _newResource.completeMesh;
                break;
            case "Defender":
                buildingObject.GetComponent<ClickEventHandler>().leftClickDownEvent.AddListener((Vector3 pos) => GameManager.instance.SetBuildingInfo(6));

                Defender _newDefender = buildingObject.GetComponent<Defender>();
                _newDefender.Init(_teamID, _buildingID, buildingLocation);
                
                buildingDictionary.Add(_buildingID, _newDefender);
                newBuilding = _newDefender;
                newBuilding.progressMesh1 = _newDefender.progressMesh1;
                newBuilding.completeMesh = _newDefender.completeMesh;
                break;
            default: //일단 초기화를 위해서 더미데이터를 넣음음
                buildingObject.GetComponent<ClickEventHandler>().leftClickDownEvent.AddListener((Vector3 pos) => GameManager.instance.SetBuildingInfo(6));

                Defender defautBuilding = buildingObject.AddComponent<Defender>();
                defautBuilding.Init(_teamID, _buildingID, buildingLocation);
                buildingDictionary.Add(_buildingID, defautBuilding);
                newBuilding = defautBuilding;
                break;
        }

        _buildingID++;
        return newBuilding;
    }

    public void UpdateBuilding(Vector3 buildingLocation, string buildingType){}
    

    // 디버깅용
    public void PrintList(Dictionary<int, Building> buildings)
    {
        for (int i = 0; i < _buildingID; i++)
        {
            Debug.Log(buildings[i].buildingType);
        }
    }

    public void BuildingAttacked(int buildingID, int damage)
    {
        Building selectedBuilding = buildingDictionary[buildingID];

    }

    public void DestroyBuilding(int buildingID)
    {

    }

    public void UpgradeBuilding()
    {
        Building building = GameManager.instance.clickedObject[0].GetComponent<Building>();
        building.buildingLevel++;
    }
}
