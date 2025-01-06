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
    // Ground 눌렀을 때 건물 리스트 UI 나타내는 함수
    public UIContainer SetBuildingListUI(int UIindex)
    {
        UIContainer selectedUI = UILists[UIindex];
        UIContainer currentUI = GameManager.instance.currentUI;
        if(selectedUI != currentUI)
        {
            currentUI.Hide();
            selectedUI.Show();
            return selectedUI;
        }
        return currentUI;
    }
    public void SetUnitUI(int unitID)
    {
        TMP_Text unitType = UILists[7].transform.Find("LeftSide/UnitInfoField/UnitType").GetComponent<TMP_Text>();
        TMP_Text unitPower = UILists[7].transform.Find("LeftSide/UnitInfoField/UnitPowerField/UnitPower").GetComponent<TMP_Text>();
        TMP_Text unitPowerRange = UILists[7].transform.Find("LeftSide/UnitInfoField/UnitPowerRangeField/UnitPowerRange").GetComponent<TMP_Text>();
        TMP_Text unitMoveSpeed = UILists[7].transform.Find("LeftSide/UnitInfoField/UnitMoveSpeedField/UnitMoveSpeed").GetComponent<TMP_Text>();

        unitType.text = GameManager.instance.clickedObject[0].GetComponent<Unit>().unitType;
        unitPower.text = $"{GameManager.instance.clickedObject[0].GetComponent<Unit>().unitPower}";
        unitPowerRange.text = $"{GameManager.instance.clickedObject[0].GetComponent<Unit>().unitPowerRange}";
        unitMoveSpeed.text = $"{GameManager.instance.clickedObject[0].GetComponent<Unit>().unitMoveSpeed}";
    }
    // Building UI 변경하는 함수
    public UIContainer SetBuildingUI(int UIindex, GameObject clickedObject)
    {
        UIContainer selectedUI = UILists[UIindex];
        UIContainer currentUI = GameManager.instance.currentUI;
        Building clickedBuidling = clickedObject.GetComponent<Building>();

        // ---------------------- 준현 --------------------
        if (clickedBuidling.buildingState == 0) // 건물이 생성중일 때
        {
            selectedUI = UILists[1];
            UpdateHealth(selectedUI, clickedBuidling);
        } else
        {
            if (UIindex >= 1 && UIindex <= 5)
            {
                // 레벨 설정
                UpdateLevel(selectedUI, clickedBuidling);

                // 체력 설정
                _health = selectedUI.transform.Find("LeftSide/LeftSide/HealthArea/Health").GetComponent<TMP_Text>();
                _health.text = clickedBuidling.buildingCurrentHealth.ToString();
            }

            if (UIindex == 1)
            {

            }
            else if (UIindex == 2)
            {

            }
        }

        if (currentUI != selectedUI)
        {
            currentUI.Hide();
            selectedUI.Show();
            return selectedUI;
        }
        // ----------------------------------------------
        // 현재 UI랑 바뀔 Ui가 같으면 현재 UI를 리턴
        return currentUI;
    }

    // ---------------- 준현 ---------------------

    public void UpdateLevel(UIContainer currentUI, Building clickedObject)
    {
        int level = clickedObject.GetComponent<Building>().buildingLevel;
        SetLevel(currentUI, level);
    }

    public void SetLevel(UIContainer currentUI, int currentLevel)
    {
        _level = currentUI.transform.Find("LeftSide/LeftSide/LevelArea/Level").GetComponent<TMP_Text>();
        _level.text = currentLevel.ToString();
    }

    public void UpdateHealth(UIContainer currentUI, Building clickedObject)
    {
        int currentHealth = clickedObject.GetComponent<Building>().buildingCurrentHealth;
        SetHealth(currentUI, currentHealth);
    }

    public void SetHealth(UIContainer currentUI, int currentHealth)
    {
        TMP_Text healthText = currentUI.transform.Find("LeftSide/LeftSide/HealthArea/Health").GetComponent<TMP_Text>();
        healthText.text = currentHealth.ToString();
    }

    // ----------------------------------------
}
