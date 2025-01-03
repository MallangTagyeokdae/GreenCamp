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
    public GameObject selectedObject;
    public Unit selectedUnit;
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
        Debug.Log("선택된건물 업데이트 됨");
    }

    public void ChangeUI(int UIindex)
    {
        currentUI = uIController.SetUI(UIindex, clickedObject);
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
    public void CreateUnit(Vector3 unitPos) // 해윤
    {
        unitController.createUnit(unitPos, unitType);
    }
    public void SetUnitInfo(int unitID)
    {
        // Debug.Log(unitController.unitDictionary.ContainsKey(unitID));
        selectedUnit = unitController.unitDictionary[unitID];  // unitDictionary에서 unitID에 해당하는 유닛을 가져옴
        uIController.DisplayUnitInfo(unitID);  // UI에 유닛 정보를 표시
        // Debug.Log("유닛생성 성공");
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
