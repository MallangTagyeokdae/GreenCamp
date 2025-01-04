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
    public UIContainer currentUI; // 현재 보이고 있는 UI를 갖고 있음
    public List<GameObject> clickedObject; // 현재 선택된 게임 Object
    //-----------------------------

    //------------ 영리 ------------ 일단 대충
    public GameObject sponeffect;
    //-----------------------------

    //---------------- 준현 --------------------
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

     public void SetClickedObject(GameObject gameObject)
    {
        clickedObject.Clear();
        clickedObject.Add(gameObject);
    }

        public void ChangeUI(int UIindex)
    {
        currentUI = uIController.SetBuildingUI(UIindex, clickedObject[0]);
    }

    public void CreateBuilding(Vector3 buildingPos)
    {
        DelayBuildingCreation(buildingPos);
        grid.SetActive(false);
    }

    public void SetBuildingType(string buildingType)
    {
        this.buildingType = buildingType;
    }

    public void LevelUpBuilding()
    {
        buildingController.UpgradeBuilding();
        uIController.UpdateLevel(currentUI, clickedObject[0]);
    }

    //------------------------------------
    public void SetUnitType(string unitType)
    {
        this.unitType = unitType;
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
    public void SetUnitInfo(int unitID)
    { // unitDictionary에서 unitID에 해당하는 유닛을 가져옴
        uIController.SetUnitUI
        
        (unitID);
    }
    public void MoveUnit(Vector3 newLocation)
    {   
        Unit selectedUnit = clickedObject[0].GetComponent<Unit>();
        if (clickedObject[0].name.Contains("Barrack")) clickedObject[0].GetComponent<Barrack>().SetSponPos(newLocation);
        else if(selectedUnit.unitBehaviour != null)
        {
            StopCoroutine(selectedUnit.unitBehaviour);
        }
        selectedUnit.unitBehaviour = StartCoroutine(unitController.MoveUnit(clickedObject[0], newLocation));
    }

    //---------------------------------조영리----------------------------------------
    private async Task StartTimer(float time, Action<float> action){
        float start = 0f;
        while(time > start){
            start += Time.deltaTime;
            action.Invoke(start);
            await Task.Yield();
        }
    }

    private async Task DelayBuildingCreation(Vector3 buildingPos)
    {
        //AddComponent로 넣으면 inspector창에서 초기화한 값이 안들어가고 가장 초기의 값이 들어감. inspector 창으로 초기화를 하고 싶으면 script상 초기화 보다는 prefab을 건드리는게 나을듯
        Building building = buildingController.CreateBuilding(buildingPos, buildingType);
        //effect 동작만 되도록 막 넣음
        GameObject effect = Instantiate(sponeffect, building.gameObject.transform);
        effect.transform.localPosition = Vector3.zero;
        effect.transform.localScale = new Vector3(4,4,4);
        effect.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
        //----------------------------
        building.InitTime();
        await StartTimer(building.loadingTime, building.UpdateTime);
        //Destroy-----------------
        Destroy(effect, effect.GetComponent<ParticleSystem>().main.startLifetime.constant);
        //------------------------
        Debug.Log($"check time: {building.time}");
    }
    //-------------------------------------------------------------------------
}
