using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
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
        //좌클릭
        LeftClick();

        //우클릭
        RightClick();

        //hover
        MouseHover();
    }

    private void LeftClick()
    {
        ClickDown(0);
        ClickUp(0);
    }

    private void RightClick()
    {
        ClickDown(1);
        ClickUp(1);
    }


    private void ClickDown(int side)    //클릭 시에 ray cast
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

        if (Input.GetMouseButtonDown(side))
        {
            if (drawRay)
            {
                Debug.DrawRay(ray.origin, ray.direction * _distance, Color.red, 1f);
            }

            if (Physics.Raycast(ray, out hit, _distance) && hit.collider.tag == "Clickable")
            {
                switch (side)
                {
                    case 0:
                        hit.collider.GetComponent<ClickEventHandler>().LeftClickDown(hit.point);
                        break;
                    case 1:
                        hit.collider.GetComponent<ClickEventHandler>().RightClickDown(hit.point);
                        break;
                }
            }
        }
    }

    private void ClickUp(int side)    //클릭 시에 ray cast
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

        if (Input.GetMouseButtonUp(side))
        {
            if (drawRay)
            {
                Debug.DrawRay(ray.origin, ray.direction * _distance, Color.red, 1f);
            }

            if (Physics.Raycast(ray, out hit, _distance) && hit.collider.tag == "Clickable")
            {
                switch (side)
                {
                    case 0:
                        hit.collider.GetComponent<ClickEventHandler>().LeftClickUp(hit.point);
                        break;
                    case 1:
                        hit.collider.GetComponent<ClickEventHandler>().RightClickUp(hit.point);
                        break;
                }
            }
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
