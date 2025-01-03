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
                StopCoroutine(unitController.currentMoveCoroutine[clickedObject[0]]);
                unitController.currentMoveCoroutine.Remove(clickedObject[0]);
            }
        }
    }
    public void CreateBuilding(Vector3 buildingPos)
    {
        // if(GameStatus.instance.canCreateBuilding(selectedBuilding))
        // { // 건물을 생성할 수 있음
        Debug.Log(buildingPos);
        //buildingController.CreateBuilding(buildingPos, buildingType);
        DelayAction(10f, buildingPos);
        grid.SetActive(false);
        // }
    }

    public void SetBuildingType(string buildingType)
    {
        this.buildingType = buildingType;
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
        GameObject gameObject = createdUnit.gameObject;
        unitController.currentMoveCoroutine.Add(gameObject, StartCoroutine(unitController.MoveUnit(gameObject, destination)));
    }
    public void SetUnitInfo(int unitID)
    { // unitDictionary에서 unitID에 해당하는 유닛을 가져옴
        uIController.SetUnitUI(unitID);
    }
    public void MoveUnit(Vector3 newLocation)
    {
        if (clickedObject[0].name.Contains("Barrack")) clickedObject[0].GetComponent<Barrack>().SetSponPos(newLocation);
        else if (unitController.currentMoveCoroutine.ContainsKey(clickedObject[0]))
        {
            StopCoroutine(unitController.currentMoveCoroutine[clickedObject[0]]);
            unitController.currentMoveCoroutine.Remove(clickedObject[0]);
        }
        unitController.currentMoveCoroutine.Add(clickedObject[0], StartCoroutine(unitController.MoveUnit(clickedObject[0], newLocation)));


    private async Task DelayAction(float time, Vector3 buildingPos)
    {
        Building building = buildingController.CreateBuilding(buildingPos, buildingType);
        Type actualType = building.GetType();
        (actualType)building.InitTime();
        await StartTimer(time, building.UpdateTime);
        Debug.Log($"check time: {building.time}");
    }
    }
}
