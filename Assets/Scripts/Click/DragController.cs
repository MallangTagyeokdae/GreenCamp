using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragController : MonoBehaviour
{
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
        // 사각형의 4개 점을 구합니다
        Vector3 topLeft = new Vector3(Mathf.Min(start.x, end.x), Mathf.Max(start.y, end.y), Mathf.Max(start.z, end.z));  // 왼쪽 상단
        Vector3 topRight = new Vector3(Mathf.Max(start.x, end.x), Mathf.Max(start.y, end.y), Mathf.Max(start.z, end.z)); // 오른쪽 상단
        Vector3 bottomLeft = new Vector3(Mathf.Min(start.x, end.x), Mathf.Min(start.y, end.y), Mathf.Min(start.z, end.z)); // 왼쪽 하단
        Vector3 bottomRight = new Vector3(Mathf.Max(start.x, end.x), Mathf.Min(start.y, end.y), Mathf.Min(start.z, end.z)); // 오른쪽 하단

        // 사각형의 4개 변을 그립니다 (각각의 4개의 점을 연결)
        Debug.DrawLine(topLeft, topRight, Color.red);   // 왼쪽 상단 -> 오른쪽 상단
        Debug.DrawLine(topRight, bottomRight, Color.red); // 오른쪽 상단 -> 오른쪽 하단
        Debug.DrawLine(bottomRight, bottomLeft, Color.red); // 오른쪽 하단 -> 왼쪽 하단
        Debug.DrawLine(bottomLeft, topLeft, Color.red); // 왼쪽 하단 -> 왼쪽 상단
    }
}
