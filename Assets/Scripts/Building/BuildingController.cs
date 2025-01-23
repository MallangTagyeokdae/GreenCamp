using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class BuildingController : MonoBehaviour
{
    public Dictionary<int, Building> buildingDictionary = new Dictionary<int, Building>();
    private string _teamID;
    private int _buildingID;
    public GameObject buildingObject;
    public GameObject enemyBuildings;   //적 빌딩이 hierarchy 창에서 생성될 위치
    public GameObject myBuildings;  //아군 빌딩이 hierarchy 창에서 생성될 위치
    
    public void Awake()
    {
        _buildingID = 0;
    }

    public Building CreateBuilding(Vector3 buildingLocation, string buildingType, Vector3 rot, List<Collider> grids)
    // 건물 생성
    {
        _teamID = GameStatus.instance.teamID;
        // 객체 생성
        buildingObject = PhotonNetwork.Instantiate($"Prefabs/Buildings/{_teamID}TeamBuildings/{buildingType}", buildingLocation, Quaternion.Euler(rot));
        buildingObject.name = buildingType + _buildingID.ToString(); // 새로 생성될 오브젝트에 고유한 이름을 붙여줌
        GameObject gameObject = buildingObject; // SetClickedObject에 넣을 임의 변수 만듦 -> call by value로 되기 떄문에 buildingObject가 바뀌어도 값이 안바뀜
        // 좌클릭 했을 때 callback 함수 넣어줌
        buildingObject.GetComponent<ClickEventHandler>().leftClickDownEvent.AddListener((Vector3 pos) => GameManager.instance.SetClickedObject(gameObject));
        
        Slider healthBar = buildingObject.transform.Find("UI/HealthUI/CurrentHealth").GetComponent<Slider>();
        Slider progressBar = buildingObject.transform.Find("UI/ProgressUI/CurrentProgress").GetComponent<Slider>();
        healthBar.gameObject.SetActive(true);
        progressBar.gameObject.SetActive(true);
        Building newBuilding;
        switch (buildingType)
        {
            case "Command":

                // Command 객체를 넣어준다. -> 오브젝트를 통해서 건물의 정보를 알 수 있게 하기위해
                Command _newCommand = buildingObject.GetComponent<Command>();
                // Command 정보 초기화
                _newCommand.Init(_teamID, _buildingID, buildingLocation, healthBar, progressBar, grids);

                // Dictionary에 추가
                buildingDictionary.Add(_buildingID, _newCommand);
                newBuilding = _newCommand;
                newBuilding.progressMesh1 = _newCommand.progressMesh1;
                newBuilding.completeMesh = _newCommand.completeMesh;

                // UI 바뀌는 callback 함수 넣어줌
                buildingObject.GetComponent<ClickEventHandler>().leftClickDownEvent.AddListener((Vector3 pos) => GameManager.instance.SetBuildingInfo(2, newBuilding));
                break;

            case "Barrack":
                // Barrack 객체를 불러온다. -> 오브젝트를 통해서 건물의 정보를 알 수 있게 하기위해
                Barrack _newBarrack = buildingObject.GetComponent<Barrack>();
                // 배럭 정보 초기화
                _newBarrack.Init(_teamID, _buildingID, buildingLocation, healthBar, progressBar,grids);

                // Dictionary에 추가
                buildingDictionary.Add(_buildingID, _newBarrack);
                newBuilding = _newBarrack;
                newBuilding.progressMesh1 = _newBarrack.progressMesh1;
                newBuilding.completeMesh = _newBarrack.completeMesh;

                // UI 바뀌는 callback 함수 넣어줌
                buildingObject.GetComponent<ClickEventHandler>().leftClickDownEvent.AddListener((Vector3 pos) => GameManager.instance.SetBuildingInfo(3, newBuilding));
                // ESC 눌렀을 때 행동 취소하는 함수 넣어줌
                break;
            case "PopulationBuilding":
                PopulationBuilding _newPop = buildingObject.GetComponent<PopulationBuilding>();
                _newPop.Init(_teamID, _buildingID, buildingLocation, healthBar, progressBar,grids);

                buildingDictionary.Add(_buildingID, _newPop);
                newBuilding = _newPop;
                newBuilding.progressMesh1 = _newPop.progressMesh1;
                newBuilding.completeMesh = _newPop.completeMesh;
                buildingObject.GetComponent<ClickEventHandler>().leftClickDownEvent.AddListener((Vector3 pos) => GameManager.instance.SetBuildingInfo(4, newBuilding));
                break;
            case "ResourceBuilding":
                ResourceBuilding _newResource = buildingObject.GetComponent<ResourceBuilding>();
                _newResource.Init(_teamID, _buildingID, buildingLocation, healthBar, progressBar,grids);

                buildingDictionary.Add(_buildingID, _newResource);
                newBuilding = _newResource;
                newBuilding.progressMesh1 = _newResource.progressMesh1;
                newBuilding.completeMesh = _newResource.completeMesh;

                buildingObject.GetComponent<ClickEventHandler>().leftClickDownEvent.AddListener((Vector3 pos) => GameManager.instance.SetBuildingInfo(5, newBuilding));
                break;
            case "Defender":
                Defender _newDefender = buildingObject.GetComponent<Defender>();
                _newDefender.Init(_teamID, _buildingID, buildingLocation, healthBar, progressBar,grids);
                
                buildingDictionary.Add(_buildingID, _newDefender);
                newBuilding = _newDefender;
                newBuilding.progressMesh1 = _newDefender.progressMesh1;
                newBuilding.completeMesh = _newDefender.completeMesh;

                buildingObject.GetComponent<ClickEventHandler>().leftClickDownEvent.AddListener((Vector3 pos) => GameManager.instance.SetBuildingInfo(6, newBuilding));
                break;
            default: //일단 초기화를 위해서 더미데이터를 넣음음
                Defender defautBuilding = buildingObject.AddComponent<Defender>();
                defautBuilding.Init(_teamID, _buildingID, buildingLocation, healthBar, progressBar,grids);
                buildingDictionary.Add(_buildingID, defautBuilding);
                newBuilding = defautBuilding;

                buildingObject.GetComponent<ClickEventHandler>().leftClickDownEvent.AddListener((Vector3 pos) => GameManager.instance.SetBuildingInfo(6, newBuilding));
                break;
        }

        int[] data = GameStatus.instance.CheckObjName(buildingType);
        newBuilding.returnCost = data[0];
        newBuilding.returnPopulation = data[1];
        _buildingID++;
        return newBuilding;
    }
    // 디버깅용
    public void PrintList(Dictionary<int, Building> buildings)
    {
        for (int i = 0; i < _buildingID; i++)
        {
            Debug.Log(buildings[i].type);
        }
    }

    public void BuildingAttacked(int buildingID, int damage)
    {
        Building selectedBuilding = buildingDictionary[buildingID];

    }

    public async Task DestroyBuilding(Building building)
    {
        building.gameObject.tag = "Untagged";
        SetBuildingState(building,Building.State.Destroy,"None");
        building.SetProgressMesh1();
        building.ActiveDestroyEffect();
        await StartTimer(5f);
        GameManager.instance.gridHandler.SetAfterDestroy(building.underGrid);
        Destroy(building.gameObject);
    }

    public void UpgradeBuilding(Building building)
    {
        float healthPercent = building.currentHealth / building.maxHealth;
        building.maxHealth += building.level * 50;
        Debug.Log(healthPercent);
        building.currentHealth = Mathf.FloorToInt(building.currentHealth * healthPercent);
        building.level++;

        switch(building)
        {
            case Command:
                GameStatus.instance.maxBuildingCount += 3;
                building.GetComponent<Command>().attackPower += 5;
                break;
            case ResourceBuilding:
                GameStatus.instance.resourcePerSecond += .5f;
                building.GetComponent<ResourceBuilding>().increasePersent += .5f;
                break;
            case PopulationBuilding:
                GameStatus.instance.maxUnitCount += 5;
                building.GetComponent<PopulationBuilding>().increasePersent += 5;
                break;
            case Defender:
                building.GetComponent<Defender>().attackPower += 5;
                break;
        }
    }

    public void SetBuildingState(Building building, Building.State state, string progressType) // 빌딩 상태 업데이트하는 함수
    {
        switch(state)
        {
            case Building.State.InCreating:
                int[] data = GameStatus.instance.CheckObjName(building.type);
                building.returnCost = data[0];
                building.returnPopulation = data[1];
                break;
            case Building.State.Destroy:
            case Building.State.Built:
                building.healthBar.gameObject.SetActive(false);
                building.progressBar.gameObject.SetActive(false);
                building.progress = 0;
                building.time = 0;
                break;
            case Building.State.InProgress:
                building.progressBar.gameObject.SetActive(true);
                building.progress = 0;
                break;

        }
        building.state = state;
        Enum.TryParse(progressType, out Building.InProgressItem item);
        switch(item)
        {
            case Building.InProgressItem.LevelUP:
                building.returnCost = building.levelUpCost;
                break;
            case Building.InProgressItem.Soldier:
            case Building.InProgressItem.Archer:
            case Building.InProgressItem.Tanker:
            case Building.InProgressItem.Healer:
                int[] data = GameStatus.instance.CheckObjName(progressType);
                building.returnCost = data[0];
                building.returnPopulation = data[1];
                break;
        }
        building.inProgressItem = item;
    }

    public void SetSponPos(Vector3 newLocation, Barrack barrack)
    {
        barrack.SetSponPos(newLocation);
    }
    
    public void CancelProgress(Building building)
    {
        if(building.state.Equals(Building.State.InCreating))
        {
            DestroyBuilding(building);
            GameManager.instance.SetBuildingListUI();
            GameManager.instance.SetClickedObject(GameManager.instance.ground);

            GameStatus.instance.currentBuildingCount -= building.population;
        } else if(building.state.Equals(Building.State.InProgress))
        {
            switch(building.inProgressItem)
            {
                case Building.InProgressItem.LevelUP:
                    building.GetComponent<PhotonView>().RPC("ActiveLevelUpEffect", RpcTarget.All, false);
                    break;
                case Building.InProgressItem.Soldier:
                case Building.InProgressItem.Archer:
                case Building.InProgressItem.Tanker:
                case Building.InProgressItem.Healer:
                    GameStatus.instance.currentUnitCount -= building.returnPopulation;
                    break;
                default:
                    break;
            }
            SetBuildingState(building, Building.State.Built, "None");
            GameManager.instance.ReloadBuildingUI(building);
        }
        GameStatus.instance.currentResourceCount += Mathf.FloorToInt(building.returnCost * 0.7f); // 취소하면 비용의 70프로만 돌려줌 소숫점아래 버림
    }

    private async Task StartTimer(float time)
    {
        float start = 0f;
        while (time > start)
        {
            start += Time.deltaTime;
            await Task.Yield();
        }
    }
}
