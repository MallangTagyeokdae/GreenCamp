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
    public UIContainer CheckUpdateUI(UIContainer selectedUI, UIContainer currentUI)
    { // 현재 선택된 UI랑 GM에 있는 UI랑 같은지 확인해서 화면에 업데이트 하고, 선택된 UI를 리턴해줌
        if (selectedUI != currentUI)
        {
            currentUI.Hide();
            selectedUI.Show();
            return selectedUI;
        }
        return currentUI;
    }
    public UIContainer SetBuildingListUI(int UIindex)
    {
        UIContainer selectedUI = UILists[UIindex];
        UIContainer currentUI = GameManager.instance.currentUI;
        return CheckUpdateUI(selectedUI, currentUI);
    }
    public UIContainer SetUnitUI(int UIindex)
    {
        UIContainer selectedUI = UILists[UIindex];
        UIContainer currentUI = GameManager.instance.currentUI;
        Unit selectedUnit = GameManager.instance.clickedObject[0].GetComponent<Unit>();

        TMP_Text unitType = selectedUI.transform.Find("LeftSide/UnitInfoField/UnitType").GetComponent<TMP_Text>();
        TMP_Text unitPower = selectedUI.transform.Find("LeftSide/UnitInfoField/UnitPowerField/UnitPower").GetComponent<TMP_Text>();
        TMP_Text unitPowerRange = selectedUI.transform.Find("LeftSide/UnitInfoField/UnitPowerRangeField/UnitPowerRange").GetComponent<TMP_Text>();
        TMP_Text unitMoveSpeed = selectedUI.transform.Find("LeftSide/UnitInfoField/UnitMoveSpeedField/UnitMoveSpeed").GetComponent<TMP_Text>();
        Slider unitCurrentHealth = selectedUI.transform.Find("LeftSide/UnitInfoField/UnitHealthField/UnitCurrentHealth").GetComponent<Slider>();

        unitType.text = selectedUnit.unitType;
        unitPower.text = $"{selectedUnit.unitPower}";
        unitPowerRange.text = $"{selectedUnit.unitPowerRange}";
        unitMoveSpeed.text = $"{selectedUnit.unitMoveSpeed}";
        unitCurrentHealth.value = (float)(selectedUnit.unitCurrentHealth * 1.0 / selectedUnit.unitMaxHealth);

        return CheckUpdateUI(selectedUI, currentUI);
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
        }
        else
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

        return CheckUpdateUI(selectedUI, currentUI);
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
        int currentHealth = (int)Math.Round(clickedObject.GetComponent<Building>().buildingCurrentHealth);
        int maxHealth = clickedObject.GetComponent<Building>().buildingMaxHealth;
        SetHealth(currentUI, currentHealth, maxHealth);
    }

    public void SetHealth(UIContainer currentUI, int currentHealth, int maxHealth)
    {
        TMP_Text healthText = currentUI.transform.Find("LeftSide/LeftSide/HealthArea/Health").GetComponent<TMP_Text>();
        Slider healthBar = currentUI.transform.Find("LeftSide/BuildingCurrentHealth").GetComponent<Slider>();
        healthText.text = currentHealth.ToString();
        healthBar.value = (float)(currentHealth * 1.0 / maxHealth);
    }

    public void SetUnSet(UIElements uIElements)
    {
        foreach(GameObject gameObject in uIElements.initialActive)
        {
            gameObject.SetActive(false);
        }
        foreach(GameObject gameObject1 in uIElements.initialUnActive)
        {
            gameObject.SetActive(true);
        }
    }

    // ----------------------------------------
}
