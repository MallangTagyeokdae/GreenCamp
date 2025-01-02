using System.Collections.Generic;
using Doozy.Runtime.UIManager.Containers;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public List<UIContainer> UILists;
    public void ShowBottomNavigaionBar() {
        
    }

    // UI 변경하는 함수
    public UIContainer setUI(UIContainer selectedUI)
    {
        UIContainer currentUI = GameManager.instance.currentUI;
        Debug.Log("UI 변경 확인");

        if(currentUI != selectedUI)
        { // 현재 UI랑 바뀌어질 UI가 다르면 UI를 바꾸고 바뀐 UI를 리턴
            Debug.Log("UI 변경됨");
            currentUI.Hide();
            selectedUI.Show();
            return selectedUI;
        }
        // 현재 UI랑 바뀔 Ui가 같으면 현재 UI를 리턴
        return currentUI;
    }
}
