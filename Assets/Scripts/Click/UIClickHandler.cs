using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIClickHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public GameObject target;
    public GameObject clickManger;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            target.transform.position = CalculateAxis(eventData.position.x, eventData.position.y);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            //해당 위치에 그라운드를 클릭하는 판정으로 만들면 될듯? 해당 그라운드의 event를 invoke하는 방식으로
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        clickManger.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        clickManger.SetActive(true);
    }

    Vector3 CalculateAxis(float x, float y){
        float t_x = (x - 17) / 374 * 500 - 250;
        float t_y = (y - 17) / 374 * 500 - 250;

        return new Vector3(t_x, 0 , t_y);
    }
}
