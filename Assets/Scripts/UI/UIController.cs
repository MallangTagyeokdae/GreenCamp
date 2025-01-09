using System;
using System.Collections.Generic;
using Doozy.Runtime.UIManager.Components;
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
    public UIElement uIElement;
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

        if (clickedBuidling.state == Building.State.InCreating) // 건물이 생성중일 때
        {
            selectedUI = UILists[1];
        } else if (clickedBuidling.state == Building.State.InProgress) // 건물에서 작업중인 상황일 때 UI 표시
        {
            SetInteractable(selectedUI.GetComponent<UIElement>().uiElements);
            SetActive(selectedUI.GetComponent<UIElement>().uiLockElements, true);
        } else if(clickedBuidling.state == Building.State.Built)
        {
            SetInteractable(selectedUI.GetComponent<UIElement>().uiElements, true);
            SetButtonByLevel(selectedUI.GetComponent<UIElement>().uiLockElements, clickedBuidling);
        }
        // 레벨 설정
        UpdateLevel(selectedUI, clickedBuidling);
        // 체력 설정
        // UpdateHealth(selectedUI, clickedBuidling);

        return CheckUpdateUI(selectedUI, currentUI);
    }
    
    private void SetButtonByLevel(List<UIButton> uIButtons, Building building)
    {
        if(building.TryGetComponent(out Barrack barrack))
        {
            int level = barrack.level;
            switch(level)
            {
                case 1:
                    SetActive(uIButtons.GetRange(1,1));
                    SetActive(uIButtons.GetRange(2,3),true);
                    break;
                case 2:
                    SetActive(uIButtons.GetRange(1,2));
                    SetActive(uIButtons.GetRange(3,2), true);
                    break;
                case 4:
                    SetActive(uIButtons.GetRange(1,3));
                    SetActive(uIButtons.GetRange(4,1), true);
                    break;
                case 3:
                    SetActive(uIButtons.GetRange(1,4));
                    break;
            }
        }
        uIButtons[0].gameObject.SetActive(false);
    }
    
    private void SetInteractable(List<UIButton> uIButtons, bool state = false)
    {
        foreach(UIButton uIButton in uIButtons)
        {
            uIButton.GetComponent<UIButton>().interactable = state;
        }
    }

    private void SetActive(List<UIButton> uIButtons, bool state = false)
    {
        foreach(UIButton uIButton in uIButtons)
        {
            uIButton.gameObject.SetActive(state);
        }
    }

    private void CheckProgressedElement(List<UIButton> uiElements) // 현재 건물에서 진행중인 작업이 무엇인지 확인하는 함수
    {
        foreach(UIButton uIButton in uiElements)
        {
            if(uIButton.gameObject.tag == "InProgress")
            {

            }
        }
    }

    public void UpdateLevel(UIContainer currentUI, Building clickedObject)
    {
        int level = clickedObject.GetComponent<Building>().level;
        SetLevel(currentUI, level);
    }

    public void SetLevel(UIContainer currentUI, int currentLevel)
    {
        _level = currentUI.transform.Find("LeftSide/LeftSide/LevelArea/Level").GetComponent<TMP_Text>();
        _level.text = currentLevel.ToString();
    }

    public void UpdateHealth(UIContainer currentUI, Building clickedObject)
    {
        int currentHealth = (int)Math.Round(clickedObject.GetComponent<Building>().currentHealth);
        int maxHealth = clickedObject.GetComponent<Building>().maxHealth;
        SetHealth(currentUI, currentHealth, maxHealth);
    }

    public void SetHealth(UIContainer currentUI, int currentHealth, int maxHealth)
    {
        TMP_Text healthText = currentUI.transform.Find("LeftSide/LeftSide/HealthArea/Health").GetComponent<TMP_Text>();
        Slider healthBar = currentUI.transform.Find("LeftSide/BuildingCurrentHealth").GetComponent<Slider>();
        healthText.text = currentHealth.ToString();
        healthBar.value = (float)(currentHealth * 1.0 / maxHealth);
    }
}
