using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using UnityEngine.UIElements;

public class ClickManager : MonoBehaviour
{

    private float _distance = 100f;
    private bool _isDragging = false;
    private Vector3 _dragStartPoint;
    private Vector3 _dragEndPoint;
    public bool drawRay = false;
    void Update()
    {
        //Mouse Button Down
        if (Input.GetMouseButtonDown(0)) //좌클릭
        {
            Click(0, (go, position) => {go.GetComponent<ClickEventHandler>().LeftClickDown(position);});
        }

        if (Input.GetMouseButtonDown(1))  //우클릭
        {
            Click(1, (go, position) => {go.GetComponent<ClickEventHandler>().RightClickDown(position);});
        }

        //Mouse Button Up
        if (Input.GetMouseButtonUp(0)) //좌클릭
        {
            Click(0, (go, position) => {go.GetComponent<ClickEventHandler>().LeftClickUp(position);});
        }
        if (Input.GetMouseButtonUp(1)) //우클릭릭
        {
            Click(1, (go, position) => {go.GetComponent<ClickEventHandler>().RightClickUp(position);});
        }

        //hover
        MouseHover();
    }


    private void Click(int side, Action<GameObject , Vector3> action)    //클릭 시에 ray cast
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        if (Input.mousePosition.x < 0 || Input.mousePosition.x > screenWidth ||
            Input.mousePosition.y < 0 || Input.mousePosition.y > screenHeight)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (drawRay)
        {
            Debug.DrawRay(ray.origin, ray.direction * _distance, Color.red, 1f);
        }

        if (Physics.Raycast(ray, out hit, _distance) && hit.collider.CompareTag("Clickable"))
        {
            action?.Invoke(hit.collider.gameObject, hit.point);
        }
    }


    private void MouseHover()   //항상 ray cast
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        if (Input.mousePosition.x < 0 || Input.mousePosition.x > screenWidth ||
            Input.mousePosition.y < 0 || Input.mousePosition.y > screenHeight)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (drawRay)
        {
            Debug.DrawRay(ray.origin, ray.direction * _distance, Color.green);
        }

        if (Physics.Raycast(ray, out hit, _distance) && hit.collider.CompareTag("Clickable"))
        {
            hit.collider.GetComponent<ClickEventHandler>().OnMouseHover(hit.point);
        }
    }
}
