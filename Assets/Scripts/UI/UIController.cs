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
    public List<UIContainer> UILists;
    public BuildingController buildingController;
    private TMP_Text _level;
    
    public void ShowBottomNavigaionBar() {
        
    }

    // UI 변경하는 함수
    public UIContainer SetUI(int UIindex, GameObject clickedObject)
    {
        UIContainer selectedUI = UILists[UIindex];
        UIContainer currentUI = GameManager.instance.currentUI;
        Debug.Log("UI 변경 확인");

        // ---------------------- 준현 --------------------
        if(currentUI != selectedUI)
        { // 현재 UI랑 바뀌어질 UI가 다르면 UI를 바꾸고 바뀐 UI를 리턴
            if(UIindex == 1) // 선택된 UI가 병영이면
            {
                int key = clickedObject.GetComponent<BuildingID>().GetKey();
                Debug.Log(key);
                Building selectedBarrack = buildingController.buildingList[key];
                _level = selectedUI.transform.Find("LeftSide/LeftSide/LevelArea/Level").GetComponent<TMP_Text>();
                _level.text = selectedBarrack.buildingLevel.ToString();
            }
            currentUI.Hide();
            selectedUI.Show();
            return selectedUI;
            Debug.Log("UI 변경됨");
        } else {
            int key = clickedObject.GetComponent<BuildingID>().GetKey();
            Building selectedBarrack = buildingController.buildingList[key];
                _level = selectedUI.transform.Find("LeftSide/LeftSide/LevelArea/Level").GetComponent<TMP_Text>();
                _level.text = selectedBarrack.buildingLevel.ToString();
        }
        // ----------------------------------------------
        // 현재 UI랑 바뀔 Ui가 같으면 현재 UI를 리턴
        return currentUI;
    }

    // ---------------- 준현 ---------------------

    public void UpdateLevel(UIContainer currentUI, GameObject clickedObject)
    {
        int level = buildingController.buildingList[clickedObject.GetComponent<BuildingID>().GetKey()].buildingLevel;
        SetLevel(currentUI, level);
    }
    
    public void SetLevel(UIContainer currentUI, int currentLevel)
    {
        _level = currentUI.transform.Find("LeftSide/LeftSide/LevelArea/Level").GetComponent<TMP_Text>();
        _level.text = currentLevel.ToString();
    }


    // ----------------------------------------
}
