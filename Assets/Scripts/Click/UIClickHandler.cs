using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIClickHandler : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public GameObject target;
    public GameObject clickManger;
    public Camera minicam;
    private bool _dragging = false;
    private bool _uiPointed = false;



    public void OnPointerDown(PointerEventData eventData)
    {
        _uiPointed = true;
        clickManger.SetActive(false);
        Vector3 origin = minicam.transform.position;
        Vector3 dest = CalculateAxis(eventData.position.x, eventData.position.y);
        Vector3 direction = (dest - origin).normalized;
        float distance = 400f;

        RaycastHit[] hits = Physics.RaycastAll(origin, direction, distance);

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            //해당 위치에 그라운드를 클릭하는 판정으로 만들면 될듯? 해당 그라운드의 event를 invoke하는 방식으로
            foreach(RaycastHit hit in hits){
                if(hit.collider.name == "RealGround"){
                    hit.collider.gameObject.GetComponent<ClickEventHandler>().rightClickDownEvent.Invoke(hit.point);
                    break;
                }
            }
        }
        else if(eventData.button == PointerEventData.InputButton.Left){
            if(GameStatus.instance.gameState == GameStates.SetTargetMode){
                foreach(RaycastHit hit in hits){
                    if(hit.collider.name == "RealGround"){
                        hit.collider.gameObject.GetComponent<ClickEventHandler>().leftClickDownEvent.Invoke(hit.point);
                        break;
                    }
                }
            }
            else{
                target.transform.position = CalculateAxis(eventData.position.x, eventData.position.y);
            } 
        }
    }

    public void OnPointerUpEvent(){
        if(_uiPointed){
            clickManger.SetActive(true);
            _dragging = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && GameStatus.instance.gameState != GameStates.SetTargetMode)
        {
            _dragging = true;
            target.transform.position = CalculateAxis(eventData.position.x, eventData.position.y);
        }
    }

    Vector3 CalculateAxis(float x, float y){
        if(x < 17){
            x = 17;
        }
        else if(x > 391){
            x = 391;
        }
        if(y < 17){
            y = 17;
        }
        else if(y > 391){
            y = 391;
        }

        float t_x = (x - 17) / 374 * 500 - 250;
        float t_y = (y - 17) / 374 * 500 - 250;

        return new Vector3(t_x, 0 , t_y);
    }

    
}
