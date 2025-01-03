using System;
using System.Collections.Generic;
using Doozy.Runtime.UIManager.Containers;
using ExitGames.Client.Photon.StructWrapping;
using TMPro;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public string unitInfo;

    public List<UIContainer> UILists;
    public BuildingController buildingController;
    private TMP_Text _level;

    private TMP_Text _health;
    public void DisplayUnitInfo(int unitID)
    {
        Debug.Log("asdf");
        TMP_Text unitType = UILists[6].transform.Find("LeftSide/UnitInfoField/UnitType").GetComponent<TMP_Text>();
        TMP_Text unitPower = UILists[6].transform.Find("LeftSide/UnitInfoField/UnitPowerField/UnitPower").GetComponent<TMP_Text>();
        TMP_Text unitPowerRange = UILists[6].transform.Find("LeftSide/UnitInfoField/UnitPowerRangeField/UnitPowerRange").GetComponent<TMP_Text>();
        TMP_Text unitMoveSpeed = UILists[6].transform.Find("LeftSide/UnitInfoField/UnitMoveSpeedField/UnitMoveSpeed").GetComponent<TMP_Text>();

        unitType.text = GameManager.instance.selectedUnit.unitType;
        unitPower.text = $"{GameManager.instance.selectedUnit.unitPower}";
        unitPowerRange.text = $"{GameManager.instance.selectedUnit.unitPowerRange}";
        unitMoveSpeed.text = $"{GameManager.instance.selectedUnit.unitMoveSpeed}";
    }
    // UI 변경하는 함수
    public UIContainer SetUI(int UIindex, GameObject clickedObject)
    {
        UIContainer selectedUI = UILists[UIindex];
        UIContainer currentUI = GameManager.instance.currentUI;
        Building clickedBuidling = clickedObject.GetComponent<Building>();
        Debug.Log("UI 변경 확인");

        // ---------------------- 준현 --------------------
        if (UIindex >= 1 && UIindex <= 5)
        {
            // 레벨 설정
            _level = selectedUI.transform.Find("LeftSide/LeftSide/LevelArea/Level").GetComponent<TMP_Text>();
            _level.text = clickedBuidling.buildingLevel.ToString();

            // 체력 설정
            _health = selectedUI.transform.Find("LeftSide/LeftSide/HealthArea/Health").GetComponent<TMP_Text>();
            _health.text = clickedBuidling.buildingHealth.ToString();
        }

        if (UIindex == 1)
        {

        }
        else if (UIindex == 2)
        {

        }
        else if (UIindex == 6)
        {
            // DisplayUnitInfo(GameManager.instance.selectedObject.GetComponent<Unit>().unitID);
        }
        if (currentUI != selectedUI)
        {
            currentUI.Hide();
            selectedUI.Show();
            return selectedUI;
            Debug.Log("UI 변경됨");
        }
        // ----------------------------------------------
        // 현재 UI랑 바뀔 Ui가 같으면 현재 UI를 리턴
        return currentUI;
    }

    // ---------------- 준현 ---------------------

    public void UpdateLevel(UIContainer currentUI, GameObject clickedObject)
    {
        int level = clickedObject.GetComponent<Building>().buildingLevel;
        SetLevel(currentUI, level);
    }

    public void SetLevel(UIContainer currentUI, int currentLevel)
    {
        _level = currentUI.transform.Find("LeftSide/LeftSide/LevelArea/Level").GetComponent<TMP_Text>();
        _level.text = currentLevel.ToString();
    }

    public void SetHealth(UIContainer currentUI, int currentHealth)
    {

    }

    // ----------------------------------------
}
