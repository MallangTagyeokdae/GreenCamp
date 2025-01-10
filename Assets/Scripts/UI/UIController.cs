using System;
using System.Collections.Generic;
using Doozy.Runtime.UIManager;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using ExitGames.Client.Photon.StructWrapping;
using TMPro;
using Unity.Profiling;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
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
    public UIContainer SetBuildingUI(int UIindex, Building clickedBuilding)
    {
        UIContainer selectedUI = UILists[UIindex];
        UIContainer currentUI = GameManager.instance.currentUI;

        switch(clickedBuilding.state)
        {
            case Building.State.InCreating:
                selectedUI = UILists[1];
                break;
            case Building.State.Built:
                 // 건물의 상태(레벨)에 따라서 버튼의 Interactable을 활성화 시켜준다.
                SetButtonByLevel(selectedUI.GetComponent<UIElement>().uiElements, clickedBuilding, (List<UIButton> UIButtons, bool state) => SetInteractable(UIButtons, state), true);
                // 건물의 상태(레벨)에 따라서 활성화된 버튼 위의 잠금을 해제해준다.
                SetButtonByLevel(selectedUI.GetComponent<UIElement>().uiLockElements, clickedBuilding, (List<UIButton> UIButtons, bool state) => SetActive(UIButtons, state), false);
                // UI의 이미지를 업데이트한다.
                SetBuildingUIImage(selectedUI, clickedBuilding.inProgressItem);
                // UI의 진행바를 업데이트한다.
                SetProgressBar(selectedUI, clickedBuilding.progress/100, 1);
                break;
            case Building.State.InProgress:
            // UI의 모든 버튼의 Interactable을 false로 바꾼다.
                SetInteractable(selectedUI.GetComponent<UIElement>().uiElements, false);
                // UI의 모든 버튼위에 Lock 표시를 한다.
                SetActive(selectedUI.GetComponent<UIElement>().uiLockElements, true);
                // UI의 이미지를 업데이트 해준다.
                SetBuildingUIImage(selectedUI, clickedBuilding.inProgressItem);
                // UI의 진행바를 업데이트한다.
                SetProgressBar(selectedUI, clickedBuilding.progress/100, 1);
                break;
            case Building.State.Destroy:
                break;
        }
        // 레벨 설정
        UpdateLevel(selectedUI, clickedBuilding);
        // 체력 설정
        UpdateHealth(selectedUI, clickedBuilding);

        return CheckUpdateUI(selectedUI, currentUI);
    }
    public void SetBuildingUIImage(UIContainer selectedUI, Enum progressType) // UI에 이미지 업데이트 하는 함수
    {
        switch(progressType)
        {
            case Building.InProgressItem.LevelUP:
                selectedUI.GetComponent<UIElement>().uiImages[0].sprite = selectedUI.GetComponent<UIElement>().uiImages[1].sprite;
                break;
            case Building.InProgressItem.Soldier:
                selectedUI.GetComponent<UIElement>().uiImages[0].sprite = selectedUI.GetComponent<UIElement>().uiImages[2].sprite;
                break;
            case Building.InProgressItem.Archer:
                selectedUI.GetComponent<UIElement>().uiImages[0].sprite = selectedUI.GetComponent<UIElement>().uiImages[3].sprite;
                break;
            case Building.InProgressItem.Tanker:
                selectedUI.GetComponent<UIElement>().uiImages[0].sprite = selectedUI.GetComponent<UIElement>().uiImages[4].sprite;
                break;
            case Building.InProgressItem.Healer:
                selectedUI.GetComponent<UIElement>().uiImages[0].sprite = selectedUI.GetComponent<UIElement>().uiImages[5].sprite;
                break;
            default:
                selectedUI.GetComponent<UIElement>().uiImages[0].sprite = null;
                break;
        }
    }
    
    private void SetButtonByLevel(List<UIButton> uIButtons, Building building, Action<List<UIButton>,bool> action, bool state)
    {
        if(building.TryGetComponent(out Barrack barrack))
        {
            int level = barrack.level;
            switch(level)
            {
                case 1:
                    action(uIButtons.GetRange(1,1), state);
                    action(uIButtons.GetRange(2,3), !state);
                    break;
                case 2:
                    action(uIButtons.GetRange(1,2), state);
                    action(uIButtons.GetRange(3,2), !state);
                    break;
                case 3:
                    action(uIButtons.GetRange(1,3), state);
                    action(uIButtons.GetRange(4,1), !state);
                    break;
                case 4:
                    action(uIButtons.GetRange(1,4), state);
                    break;
            }
        }
        // 레벨에 상관없이 레벨업 버튼은 Interactable -> true, Lock 버튼 Active -> false로 해야함
        if(state)
        {
            uIButtons[0].interactable = state;
        } else if(!state)
        {
            uIButtons[0].gameObject.SetActive(state);
        }
    }
    
    private void SetInteractable(List<UIButton> uIButtons, bool state)
    {
        foreach(UIButton uIButton in uIButtons)
        {   
            if(uIButton.interactable != state)
            {
                if(EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject == uIButton.gameObject)
                {
                    if(state)
                        EventSystem.current.SetSelectedGameObject(null);
                    else
                    {
                        uIButton.OnDeselect(null);
                        uIButton.SetState(UISelectionState.Normal);
                        
                    }
                }
                Debug.Log(uIButton.name);
                uIButton.interactable = state;
            }
        }
    }

    private void SetActive(List<UIButton> uIButtons, bool state)
    {
        foreach(UIButton uIButton in uIButtons)
        {
            if(uIButton.gameObject.activeSelf != state)
                uIButton.gameObject.SetActive(state);
        }
    }

    public void UpdateLevel(UIContainer currentUI, Building clickedObject)
    {
        int level = clickedObject.GetComponent<Building>().level;
        SetLevel(currentUI, level);
    }

    public void SetLevel(UIContainer currentUI, int currentLevel)
    {
        _level = currentUI.GetComponent<UIElement>().level;
        _level.text = currentLevel.ToString();
    }

    public void UpdateHealth(UIContainer currentUI, Building clickedObject) // 체력 숫자를 표시하는 함수
    {
        int currentHealth = (int)Math.Round(clickedObject.GetComponent<Building>().currentHealth);
        int maxHealth = clickedObject.GetComponent<Building>().maxHealth;
        if(clickedObject.state == (Building.State).0) // 건설중인경우에는 체력바도 업데이트 해줘야함
        {
            SetProgressBar(currentUI, currentHealth, maxHealth);
        }
        SetHealth(currentUI, currentHealth);
    }

    public void SetHealth(UIContainer currentUI, int currentHealth)
    {
        TMP_Text healthText = currentUI.GetComponent<UIElement>().health;
        healthText.text = currentHealth.ToString();
    }

    public void SetProgressBar(UIContainer currentUI, float currentvalue, float maxValue)
    {
        Slider progresBar = currentUI.GetComponent<UIElement>().progressBar;
        progresBar.value = (float)(currentvalue * 1.0 / maxValue);
    }

}
