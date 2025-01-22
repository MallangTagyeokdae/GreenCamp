using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Doozy.Runtime.UIManager;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using ExitGames.Client.Photon.StructWrapping;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public string unitInfo;
    public List<UIContainer> UILists; // 유닛, 건물과 관련된 UI들
    public List<UIContainer> Options; // 로딩, 자원, 배경과 관련된 UI들
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
    public UIContainer SetUnitUI(int UIindex, Unit unit)
    {
        UIContainer selectedUI = UILists[UIindex];
        UIContainer currentUI = GameManager.instance.currentUI;

        UIElement uIElement = selectedUI.GetComponent<UIElement>();

        TMP_Text type = uIElement.name;
        TMP_Text range = uIElement.level;
        TMP_Text currentHealth = uIElement.currentHealth;
        TMP_Text maxHealth = uIElement.maxHealth;
        Image image = uIElement.image;
        Slider currentHealthBar = uIElement.progressBar;
        TMP_Text power = selectedUI.transform.Find("LeftSide/LeftBanner/Damage/Damage").GetComponent<TMP_Text>();
        TMP_Text moveSpeed = selectedUI.transform.Find("LeftSide/LeftBanner/MoveSpeed/Speed").GetComponent<TMP_Text>();

        switch(unit.unitType)
        {
            case "Soldier":
                image.sprite = uIElement.uiImages[0];
                break;
            case "Archer":
                image.sprite = uIElement.uiImages[1];
                break;
            case "Tanker":
                image.sprite = uIElement.uiImages[2];
                break;
            case "Healer":
                image.sprite = uIElement.uiImages[3];
                break;
        }

        type.text = unit.unitType;
        power.text = $"{unit.unitPower}";
        range.text = $"{unit.unitPowerRange}";
        moveSpeed.text = $"{unit.unitMoveSpeed}";
        currentHealth.text = $"{unit.unitCurrentHealth}";
        maxHealth.text = $"{unit.unitMaxHealth}";
        currentHealthBar.value = (float)(unit.unitCurrentHealth * 1.0 / unit.unitMaxHealth);

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
                SetBuildingUIImage(selectedUI, clickedBuilding.type);
                break;
            case Building.State.Built:
                 // 건물의 상태(레벨)에 따라서 버튼의 Interactable을 활성화 시켜준다.
                SetButtonByLevel(selectedUI.GetComponent<UIElement>().uiElements, clickedBuilding, (List<UIButton> UIButtons, bool state) => SetInteractable(UIButtons, state), true);
                // 건물의 상태(레벨)에 따라서 활성화된 버튼 위의 잠금을 해제해준다.
                SetButtonByLevel(selectedUI.GetComponent<UIElement>().uiLockElements, clickedBuilding, (List<UIButton> UIButtons, bool state) => SetActive(UIButtons, state), false);
                // UI의 이미지를 업데이트한다.
                SetBuildingUIImage(selectedUI, clickedBuilding.inProgressItem.ToString());
                // UI의 진행바를 업데이트한다.
                SetProgressBar(selectedUI, clickedBuilding.progress/100, 1);
                break;
            case Building.State.InProgress:
            // UI의 모든 버튼의 Interactable을 false로 바꾼다.
                SetInteractable(selectedUI.GetComponent<UIElement>().uiElements, false);
                // UI의 모든 버튼위에 Lock 표시를 한다.
                SetActive(selectedUI.GetComponent<UIElement>().uiLockElements, true);
                // UI의 이미지를 업데이트 해준다.
                SetBuildingUIImage(selectedUI, clickedBuilding.inProgressItem.ToString());
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
        // 이름 설정
        UpdateName(selectedUI, clickedBuilding);

        return CheckUpdateUI(selectedUI, currentUI);
    }
    public void SetBuildingUIImage(UIContainer selectedUI, String progressType) // UI에 이미지 업데이트 하는 함수
    {
        switch(progressType)
        {
            case "Barrack":
            case "LevelUP":
                selectedUI.GetComponent<UIElement>().image.sprite = selectedUI.GetComponent<UIElement>().uiImages[0];
                break;
            case "PopulationBuilding":
            case "Soldier":
                selectedUI.GetComponent<UIElement>().image.sprite = selectedUI.GetComponent<UIElement>().uiImages[1];
                break;
            case "ResourceBuilding":
            case "Archer":
                selectedUI.GetComponent<UIElement>().image.sprite = selectedUI.GetComponent<UIElement>().uiImages[2];
                break;
            case "DefenderBuilding":
            case "Tanker":
                selectedUI.GetComponent<UIElement>().image.sprite = selectedUI.GetComponent<UIElement>().uiImages[3];
                break;
            case "Healer":
                selectedUI.GetComponent<UIElement>().image.sprite = selectedUI.GetComponent<UIElement>().uiImages[4];
                break;
            default:
                selectedUI.GetComponent<UIElement>().image.sprite = null;
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
                uIButton.interactable = state;
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
        SetHealth(currentUI, currentHealth, maxHealth);
    }

    public void SetHealth(UIContainer currentUI, int currentHealth, int maxHealth)
    {
        TMP_Text MaxHealthText = currentUI.GetComponent<UIElement>().maxHealth;
        TMP_Text CurrentHealthText = currentUI.GetComponent<UIElement>().currentHealth;
        CurrentHealthText.text = currentHealth.ToString();
        MaxHealthText.text = maxHealth.ToString();
    }

    public void SetProgressBar(UIContainer currentUI, float currentvalue, float maxValue)
    {
        if(currentUI != UILists[8] && currentUI.TryGetComponent(out UIElement uIElement))
        {
            Slider progresBar = uIElement.progressBar;
            progresBar.value = (float)(currentvalue * 1.0 / maxValue);
        }
    }

    public void UpdateName(UIContainer currentUI, Building building)
    {
        TMP_Text nameText = currentUI.GetComponent<UIElement>().name;
        nameText.text = ParseBuildingName(building);
    }

    public string ParseBuildingName(Building building)
    {
        if(building.name.Contains("Command"))
        {
            return "본진";
        } else if(building.name.Contains("Barrack"))
        {
            return "배럭";
        } else if(building.name.Contains("Population"))
        {
            return "인구수건물";
        } else if(building.name.Contains("Resource"))
        {
            return "자원건물";
        } else if(building.name.Contains("Defender"))
        {
            return "방어건물";
        }
        return "";
    }

    public async Task CountDown()
    {
        TMP_Text countDown = Options[0].transform.Find("CountDown").GetComponent<TMP_Text>();

        for(int i=3; i>0; i--)
        {
            countDown.text = i.ToString();
            if(i == 1)
            {
                Options[1].Show();
                Options[2].Show();
            }
            await Task.Delay(1000);
        }

        Options[0].Hide();
    }

    public UIContainer SetGroupUI(int index, int startIndex, UIContainer currentUI, List<GameObject> clickedObjs)
    {
        UIContainer groupUI = UILists[index];
        int imageIndex = 0;
        for(int i = startIndex; startIndex<clickedObjs.Count; startIndex++)
        {
            if(clickedObjs[i].TryGetComponent(out Unit unit))
            {
                Debug.Log(imageIndex + "/"+ groupUI.GetComponent<UIElement>().groupImages.Count);
                groupUI.GetComponent<UIElement>().groupImages[imageIndex].gameObject.SetActive(true);
                switch(unit.unitType)
                {
                    case "Soldier":
                        groupUI.GetComponent<UIElement>().groupImages[imageIndex].sprite = groupUI.GetComponent<UIElement>().uiImages[0];
                        break;
                    case "Archer":
                        groupUI.GetComponent<UIElement>().groupImages[imageIndex].sprite = groupUI.GetComponent<UIElement>().uiImages[1];
                        break;
                    case "Tnaker":
                        groupUI.GetComponent<UIElement>().groupImages[imageIndex].sprite = groupUI.GetComponent<UIElement>().uiImages[2];
                        break;
                    case "Healer":
                        groupUI.GetComponent<UIElement>().groupImages[imageIndex].sprite = groupUI.GetComponent<UIElement>().uiImages[3];
                        break;
                }
                imageIndex++;
            }
        }
        return CheckUpdateUI(groupUI, currentUI);
    }

    public void ActiveFalseUI(int index)
    {
        UIContainer groupUI = UILists[index];

        foreach(Image image in groupUI.GetComponent<UIElement>().groupImages)
        {
            image.gameObject.SetActive(false);
        }
    }
}
