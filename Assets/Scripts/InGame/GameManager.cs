using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Doozy.Runtime.UIManager.Containers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            if (_instance == null)
            {
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
    public string unitType;
    //-----------------------------

    //------------ 준현 ------------
    public BuildingController buildingController; // 준현
    public string buildingType;
    public GameObject grid;
    public GameObject ground;
    public UIContainer currentUI; // 현재 보이고 있는 UI를 갖고 있음
    public List<GameObject> clickedObject; // 현재 선택된 게임 Object
    //-----------------------------

    //------------ 영리 ------------ 일단 대충
    public GameObject sponeffect;
    //-----------------------------

    void Start()
    {
        currentUI.Show();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (clickedObject[0].TryGetComponent<Unit>(out Unit unit))
            {
                StopCoroutine(unit.unitBehaviour);
            }
        }
    }

    // ================== 클릭 관련 함수 ======================
    public void SetClickedObject(GameObject gameObject)
    {
        clickedObject.Clear();
        clickedObject.Add(gameObject);
    }

    public void GroundEvent(Vector3 newLocation)
    {
        if(clickedObject[0].name.Contains("Barrack")) SetSponPos(newLocation);
        else if(clickedObject[0].name.Contains("Archer")
                || clickedObject[0].name.Contains("Healer")
                || clickedObject[0].name.Contains("Soldier")
                || clickedObject[0].name.Contains("Tanker"))
        {
            MoveUnit(newLocation);
            Debug.Log("check");
        }
    }
    // =====================================================


    //=================== UI 변경 관련 함수들 ===================
    public void SetBuildingListUI(int UIindex)
    {
        currentUI = uIController.SetBuildingListUI(UIindex);
    }
    public void SetBuildingInfo(int UIindex)
    {
        currentUI = uIController.SetBuildingUI(UIindex, clickedObject[0]);
    }
    public void SetUnitInfo(int UIindex)
    { // unitDictionary에서 unitID에 해당하는 유닛을 가져옴
       currentUI = uIController.SetUnitUI(UIindex);
    }
    // =====================================================


    // =================== 객체 생성 함수들 ===================
    public void CreateBuilding(Vector3 buildingPos)
    {
        DelayBuildingCreation(buildingPos);
        grid.SetActive(false);
    }
    public void CreateUnit() // 해윤
    {
        Vector3 buildingPos = clickedObject[0].GetComponent<Barrack>().transform.position;
        Vector3 destination = clickedObject[0].GetComponent<Barrack>()._sponPos;
        buildingPos = new Vector3(buildingPos.x, buildingPos.y, buildingPos.z - 4f);
        Unit createdUnit = unitController.CreateUnit(buildingPos, unitType);
        // 유닛을 destination으로 이동명령 내리기
        GameObject unitObject = createdUnit.gameObject;
        //unitController.currentMoveCoroutine.Add(gameObject, StartCoroutine(unitController.MoveUnit(gameObject, destination)));
        createdUnit.unitBehaviour = StartCoroutine(unitController.MoveUnit(unitObject, destination));
    }
    // =====================================================
    

    // =================== 건물 관리 함수들 ===================
    public void SetBuildingType(string buildingType)
    {
        this.buildingType = buildingType;
    }
    public void LevelUpBuilding()
    {
        buildingController.UpgradeBuilding();
        uIController.UpdateLevel(currentUI, clickedObject[0].GetComponent<Building>());
    }
    private async Task DelayBuildingCreation(Vector3 buildingPos)
    {
        //AddComponent로 넣으면 inspector창에서 초기화한 값이 안들어가고 가장 초기의 값이 들어감. inspector 창으로 초기화를 하고 싶으면 script상 초기화 보다는 prefab을 건드리는게 나을듯
        Building building = buildingController.CreateBuilding(buildingPos, buildingType);
        building.InitTime();
        await StartTimer(building.loadingTime, (float time) => UpdateBuildingUI(building, time));

        //effect 동작만 되도록 막 넣음
        GameObject effect = Instantiate(sponeffect, building.gameObject.transform);
        effect.transform.localPosition = Vector3.zero;
        effect.transform.localScale = new Vector3(4,4,4);
        effect.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        //----------------------------

        //Destroy-----------------
        Destroy(effect, effect.GetComponent<ParticleSystem>().main.startLifetime.constant);
        //------------------------
        Debug.Log($"check time: {building.time}");
        building.buildingState = Building.BuildingState.Built;
        CheckingBuiltClear(building);
        ReloadBuildingUI(building);
    }

    private void CheckingBuiltClear(Building building)
    {
        building.buildingCurrentHealth = Mathf.FloorToInt(building.buildingCurrentHealth); // 소수점 아래자리 버리기
        building.buildingHealthBar.gameObject.SetActive(false);
        building.buildingProgressBar.gameObject.SetActive(false);

    }

    private void UpdateBuildingUI(Building building, float time)
    { // 건물이 생성될 때 체력을 업데이트 해주는 함수
        building.UpdateTime(time);
        if(clickedObject[0].GetComponent<Building>() == building)
        {
            uIController.UpdateHealth(currentUI, building);
        }
    }

    private void ReloadBuildingUI(Building building)
    { // 건물이 생성완료 됐을 때 건물을 클릭하고 있으면 건물 UI로 바꿔준다.
        if(clickedObject[0].name == building.name)
        {
            switch(building.buildingType)
            {
                case "Command":
                    SetBuildingInfo(2);
                    break;
                case "Barrack":
                    SetBuildingInfo(3);
                    break;
                case "PopulationBuilding":
                    SetBuildingInfo(4);
                    break;
                case "ResourceBuilding":
                    SetBuildingInfo(5);
                    break;
                case "Defender":
                    SetBuildingInfo(6);
                    break;
            }
        }
    }

    private void SetSponPos(Vector3 newLocation)
    {
        clickedObject[0].GetComponent<Barrack>().SetSponPos(newLocation);
    }

    // =====================================================


    // =================== 유닛 관리 함수들 =================== 
    public void SetUnitType(string unitType)
    {
        this.unitType = unitType;
    }
    
    
    public void MoveUnit(Vector3 newLocation)
    {   
        Unit selectedUnit = clickedObject[0].GetComponent<Unit>();
        if(selectedUnit.unitBehaviour != null)
        {
            StopCoroutine(selectedUnit.unitBehaviour);
        }
        selectedUnit.unitBehaviour = StartCoroutine(unitController.MoveUnit(clickedObject[0], newLocation));
    }
    
    // =====================================================


    // =================== 타이머 함수 =================== 
    private async Task StartTimer(float time, Action<float> action){
        float start = 0f;
        while(time > start){
            start += Time.deltaTime;
            action.Invoke(start);
            await Task.Yield();
        }
    }
    // =====================================================
}
