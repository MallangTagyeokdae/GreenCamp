using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tab2NextSelectable : MonoBehaviour
{
    public List<GameObject> Objects = new List<GameObject>();
    public int selectedIndex = 0;
    public void Selected(string objectName) {
        int idx = Objects.FindIndex(x => x.name == objectName);
        selectedIndex = idx;
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Tab) && (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))){
            selectedIndex--;
            if(selectedIndex < 0) {
                selectedIndex = 1;
            }
            SelectInputField();
        }
        else if(Input.GetKeyDown(KeyCode.Tab)) {
            selectedIndex++;
            if(selectedIndex > Objects.Count - 1) selectedIndex = 0;
            SelectInputField();
        }
        void SelectInputField() {               
            Objects[selectedIndex].GetComponent<Selectable>().Select();
        }
    }                
}
