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
    // public TMP_Text text;
    public List<UIContainer> UILists;
    public BuildingController buildingController;
    private TMP_Text _level;
    private TMP_Text _health;
    
    public void DisplayUnitInfo(int unitID)
    {
        TMP_Text text = UILists[3].transform.Find("UnitInfoText").GetComponent<TMP_Text>();
        text.text = "asdf";
    }
    // UI 변경하는 함수
    public UIContainer SetUI(int UIindex, GameObject clickedObject)
    {
        UIContainer selectedUI = UILists[UIindex];
        UIContainer currentUI = GameManager.instance.currentUI;
        Building clickedBuidling = clickedObject.GetComponent<Building>();
        Debug.Log("UI 변경 확인");

        // ---------------------- 준현 --------------------
        if(UIindex != 0)
        {
            // 레벨 설정
            _level = selectedUI.transform.Find("LeftSide/LeftSide/LevelArea/Level").GetComponent<TMP_Text>();
            _level.text = clickedBuidling.buildingLevel.ToString();

            // 체력 설정
            _health = selectedUI.transform.Find("LeftSide/LeftSide/HealthArea/Health").GetComponent<TMP_Text>();
            _health.text = clickedBuidling.buildingHealth.ToString();
        }

        if(UIindex == 1)
        {
            
        } else if(UIindex == 2)
        {
            
        }
        if(currentUI != selectedUI)
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
