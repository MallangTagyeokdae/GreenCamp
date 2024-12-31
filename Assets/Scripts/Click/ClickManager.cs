using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using UnityEngine.UIElements;

public class ClickManager : MonoBehaviour
{
    private float _distance = 100f;
    private Vector3 _dragStartPoint;
    private Vector3 _dragEndPoint;
    public bool drawRay = false;

    #region drag variable
    private bool _isDragging;
    private Vector3 _startPos;
    private Vector3 _endPos;
    public Vector3 startPos
    {
        get
        {
            return _startPos;
        }
        set
        {
            _startPos = value;
        }
    }
    public Vector3 endPos
    {
        get
        {
            return _endPos;
        }
        set
        {
            _endPos = value;
        }
    }
    #endregion end drag variable
    void Update()
    {
        //Mouse Button Down
        if (Input.GetMouseButtonDown(0)) //좌클릭
        {
            Click(0, (go, position) => { go.GetComponent<ClickEventHandler>().LeftClickDown(position); });
        }

        if (Input.GetMouseButtonDown(1))  //우클릭
        {
            Click(1, (go, position) => { go.GetComponent<ClickEventHandler>().RightClickDown(position); });
        }

        //Mouse Button Up
        if (Input.GetMouseButtonUp(0)) //좌클릭
        {
            Click(0, (go, position) => { go.GetComponent<ClickEventHandler>().LeftClickUp(position); });
        }
        if (Input.GetMouseButtonUp(1)) //우클릭릭
        {
            Click(1, (go, position) => { go.GetComponent<ClickEventHandler>().RightClickUp(position); });
        }

        Drag();
        //hover
        MouseHover();
    }


    private void Click(int side, Action<GameObject, Vector3> action)    //클릭 시에 ray cast
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
        if (Physics.Raycast(ray, out hit, _distance) && hit.collider.CompareTag("Barrack"))
        {
            Debug.Log("배럭 선택됨");
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

    public void Drag()
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
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, 100f) && hit.collider.gameObject.CompareTag("Ground"))
            {
                startPos = hit.point;
                _isDragging = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (Physics.Raycast(ray, out hit, 100f) && hit.collider.gameObject.CompareTag("Ground"))
            {
                endPos = hit.point;
                SelectObjectInBox();
                _isDragging = false;
            }
        }

        if (_isDragging)
        {
            if (Physics.Raycast(ray, out hit, 100f) && hit.collider.gameObject.CompareTag("Ground"))
            {
                endPos = hit.point;
                DrawDebugDragBox(startPos, endPos);
            }
        }
    }

    private void DrawDebugDragBox(Vector3 start, Vector3 end)
    {
        Vector3 topLeft = new Vector3(Mathf.Min(start.x, end.x), Mathf.Max(start.y, end.y), Mathf.Max(start.z, end.z));  // 왼쪽 상단
        Vector3 topRight = new Vector3(Mathf.Max(start.x, end.x), Mathf.Max(start.y, end.y), Mathf.Max(start.z, end.z)); // 오른쪽 상단
        Vector3 bottomLeft = new Vector3(Mathf.Min(start.x, end.x), Mathf.Min(start.y, end.y), Mathf.Min(start.z, end.z)); // 왼쪽 하단
        Vector3 bottomRight = new Vector3(Mathf.Max(start.x, end.x), Mathf.Min(start.y, end.y), Mathf.Min(start.z, end.z)); // 오른쪽 하단

        Debug.DrawLine(topLeft, topRight, Color.red);   // 왼쪽 상단 -> 오른쪽 상단
        Debug.DrawLine(topRight, bottomRight, Color.red); // 오른쪽 상단 -> 오른쪽 하단
        Debug.DrawLine(bottomRight, bottomLeft, Color.red); // 오른쪽 하단 -> 왼쪽 하단
        Debug.DrawLine(bottomLeft, topLeft, Color.red); // 왼쪽 하단 -> 왼쪽 상단
    }

    private Collider[] SelectObjectInBox()
    {
        Collider[] colliders = Physics.OverlapBox((startPos + endPos) / 2, new Vector3(
            Mathf.Abs(startPos.x - endPos.x),
            Mathf.Abs(startPos.y - endPos.y),
            Mathf.Abs(startPos.z - endPos.z)
        ) / 2, Quaternion.identity);

        Debug.Log(colliders);
        return colliders;
    }
}
