using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIClickHandler : MonoBehaviour, IPointerClickHandler
{
    public GameObject target;
    public void OnPointerClick(PointerEventData eventData)
    {
        target.transform.position = CalculateAxis(eventData.position.x, eventData.position.y);
    }

    Vector3 CalculateAxis(float x, float y){
        float t_x = (x - 17) / 374 * 500 - 250;
        float t_y = (y - 17) / 374 * 500 - 250;

        return new Vector3(t_x, 0 , t_y);
    }
}
