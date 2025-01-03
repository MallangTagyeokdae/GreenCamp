using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Doozy.Runtime.UIManager.Containers;
using Unity.VisualScripting;
using UnityEngine;

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
    public GameObject clickedObject; // 현재 선택된 게임 Object
    //-----------------------------


    //---------------- 준현 --------------------
    void Start()
    {
        currentUI.Show();
    }
    public void CreateBuilding(Vector3 buildingPos)
    {
        // if(GameStatus.instance.canCreateBuilding(selectedBuilding))
        // { // 건물을 생성할 수 있음
        Debug.Log(buildingPos);
        buildingController.CreateBuilding(buildingPos, buildingType);
        grid.SetActive(false);
        // }
    }

    public void SetBuildingType(string buildingType)
    {
        this.buildingType = buildingType;
    }

    public void SetClickedObject(GameObject gameObject)
    {
        clickedObject = gameObject;
    }

    public void ChangeUI(int UIindex)
    {
        currentUI = uIController.SetBuildingUI(UIindex, clickedObject);
    }

    public void LevelUpBuilding()
    {
        buildingController.UpgradeBuilding();
        uIController.UpdateLevel(currentUI, clickedObject);
    }

    //------------------------------------
    public void SetUnitType(string unitType)
    {
        this.unitType = unitType;
    }
    public void CreateUnit() // 해윤
    {
        Vector3 buildingPos = clickedObject.GetComponent<Barrack>().transform.position;
        Vector3 destination = clickedObject.GetComponent<Barrack>()._sponPos;
        buildingPos = new Vector3(buildingPos.x, buildingPos.y, buildingPos.z - 4f);
        Unit createdUnit = unitController.CreateUnit(buildingPos, unitType);
        // 유닛을 destination으로 이동명령 내리기
        createdUnit.Move(destination);
    }
    public void SetUnitInfo(int unitID)
    { // unitDictionary에서 unitID에 해당하는 유닛을 가져옴
        uIController.SetUnitUI
        
        (unitID);
    }
    public void MoveUnit(Vector3 newLocation)
    {
        // 수정해야할 부분 유닛이 선택되었을 때만 이동명령이 내려지도록 수정해야함
        // 지금은 건물 선택되어도 이동명령이 내려짐
        if (clickedObject.name.Contains("Barrack")) clickedObject.GetComponent<Barrack>().SetSponPos(newLocation);
        else if (clickedObject.name != "Ground") unitController.MoveUnit(newLocation);
    }

    private async Task StartTimer(float time, Action<float> update)
    {
        float start = 0f;
        while (start < time)
        {
            start += Time.deltaTime;
            update.Invoke(start);
            await Task.Yield();
        }
    }

    private async Task DelayAction(float time, Action action, Action<float> update)
    {
        await StartTimer(time, update);
        action?.Invoke();
    }
}
