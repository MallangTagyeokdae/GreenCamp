using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using UnityEngine.UIElements;
using Unity.AI.Navigation;
using System.Linq;
using Photon.Chat.Demo;
using FischlWorks_FogWar;

public class ClickManager : MonoBehaviour
{
    struct DragBox
    {
        public Vector3 topLeft { get; set; }  // 왼쪽 상단
        public Vector3 topRight { get; set; } // 오른쪽 상단
        public Vector3 bottomLeft { get; set; } // 왼쪽 하단
        public Vector3 bottomRight { get; set; } // 오른쪽 하단

    };
    // ----------------- 준현 ------------------
    public GameObject grid;
    public List<Collider> detectedGrids;
    public List<Collider> constructionGrids;
    public GameObject fow;

    private float _buildingRange;
    private Vector3 _range;
    private Vector3 areaPos;
    private LayerMask _layerMask = 1 << 3;
    // ----------------------------------------

    private float _distance = 300f;
    private Vector3 _dragStartPoint;
    private Vector3 _dragEndPoint;
    public bool drawRay = false;
    private GameObject hoverObj;
    public bool _overlay = true;

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

    private GameObject _lineObject;
    private Vector3 _previousEndPos;
    private DragBox _dragBox;
    #endregion end drag variable
    void Update()
    {
        //Mouse Button Down
        if (Input.GetMouseButtonDown(0)) //좌클릭
        {
            Click(0, (go, position) => { go.GetComponent<ClickEventHandler>().LeftClickDown(position); });  //입력 받은 오브젝트가 가지고 있는 콜백함수를 실행
        }

        if (Input.GetMouseButtonDown(1))  //우클릭
        {
            Click(1, (go, position) => { go.GetComponent<ClickEventHandler>().RightClickDown(position); }); //입력 받은 오브젝트가 가지고 있는 콜백함수를 실행
        }

        //Mouse Button Up
        if (Input.GetMouseButtonUp(0)) //좌클릭
        {
            Click(0, (go, position) => { go.GetComponent<ClickEventHandler>().LeftClickUp(position); });    //입력 받은 오브젝트가 가지고 있는 콜백함수를 실행
        }
        if (Input.GetMouseButtonUp(1)) //우클릭
        {
            Click(1, (go, position) => { go.GetComponent<ClickEventHandler>().RightClickUp(position); });   //입력 받은 오브젝트가 가지고 있는 콜백함수를 실행
        }

        Drag(); //드래그

        //hover
        MouseHover();
    }


    private void Click(int side, Action<GameObject, Vector3> action)    //클릭 시에 ray cast
    {
        //화면 의 클릭은 아무런 동작하지 않도록 하기 위한 변수 및 조건문
        #region 화면 밖에 대한 변수 및 조건문

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        if (Input.mousePosition.x < 0 || Input.mousePosition.x > screenWidth ||
            Input.mousePosition.y < 0 || Input.mousePosition.y > screenHeight)
        {
            return;
        }

        #endregion

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 메인카메라 위치로부터 마우스 위치까지 ray를 생성
        RaycastHit hit;

        if (drawRay) // drawRay가 true인 경우 scene에 ray를 그림
        {
            Debug.DrawRay(ray.origin, ray.direction * _distance, Color.red, 1f);
        }

        // if (Physics.Raycast(ray, out hit, Mathf.Infinity, 7)) 
        // {
        //     Vector2 uv = hit.textureCoord;  // Fog 텍스처 좌표 가져오기
        //     int x = Mathf.FloorToInt(uv.x * fow.GetComponent<csFogWar>().fogPlaneTextureLerpBuffer.width);
        //     int y = Mathf.FloorToInt(uv.y * fow.GetComponent<csFogWar>().fogPlaneTextureLerpBuffer.height);

        //     Color fogColor = fow.GetComponent<csFogWar>().fogPlaneTextureLerpBuffer.GetPixel(x, y);
        //     Debug.Log(hit.point.ToString());
        //     Debug.Log(x + " / " + y);

        //     Debug.Log(fogColor.a);
        //     if (fogColor.a < 1f)
        //     {
        //         Debug.Log("안개 지역 클릭됨! 클릭 무시.");
        //         return;
        //     }
        // }


        if (Physics.Raycast(ray, out hit, _distance) && hit.collider.CompareTag("Clickable"))
        {
            action?.Invoke(hit.collider.gameObject, hit.point); //action에 raycast가 맞은 오브젝트와 맞은 vector3를 반환
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

        if (Physics.Raycast(ray, out hit, _distance))
        {
            // GridHandler에서 Grid바꾸는 함수 실행해야함
            if(hit.collider.CompareTag("Clickable"))
            {
                if (hoverObj != hit.collider.gameObject)
                {
                    if (hoverObj != null)
                    {
                        hoverObj.GetComponent<ClickEventHandler>().DeMouseHover(hit.point);
                    }
                    hoverObj = hit.collider.gameObject;
                }
                hit.collider.GetComponent<ClickEventHandler>().OnMouseHover(hit.point);
            }
        }
    }


    public void Drag()  //미완성
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        if (Input.mousePosition.x < 0 || Input.mousePosition.x > screenWidth ||
            Input.mousePosition.y < 0 || Input.mousePosition.y > screenHeight)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, _distance);

        if (Input.GetMouseButtonDown(0))
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.name == "RealGround" && hit.collider.gameObject.CompareTag("Clickable"))
                {
                    startPos = hit.point;
                    _isDragging = true;
                }
            }

        }

        if (Input.GetMouseButtonUp(0))
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.name == "RealGround" && hit.collider.gameObject.CompareTag("Clickable"))
                {
                    DestroyDragBox();
                    if(!_isDragging){
                        return;
                    }
                    else{
                        _isDragging = false;
                    }
                     
                    endPos = hit.point;
                    Collider[] colliders = startPos != endPos ? SelectObjectInBox() : null;
                    if(colliders != null){
                        foreach(Collider collider in colliders){
                            if(collider.gameObject.CompareTag("Clickable")){
                                collider.gameObject.GetComponent<ClickEventHandler>().Dragged();
                            }                            
                            //Debug.Log($"{collider.gameObject.name}");
                        }
                    }
                    
                }
            }

        }

        if (_isDragging) //end position을 갱신
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.name == "RealGround" && hit.collider.gameObject.CompareTag("Clickable"))
                {
                    _previousEndPos = endPos;
                    endPos = hit.point;
                    if (endPos != _previousEndPos)
                    {
                        UpdateDragBox(startPos, endPos);
                        DestroyDragBox();
                        DrawDebugDragBox();
                        DrawDragBox();
                    }
                }
            }

        }
    }

    private void UpdateDragBox(Vector3 start, Vector3 end)
    {
        _dragBox.topLeft = new Vector3(Mathf.Min(start.x, end.x), Mathf.Max(start.y, end.y), Mathf.Max(start.z, end.z));
        _dragBox.topRight = new Vector3(Mathf.Max(start.x, end.x), Mathf.Max(start.y, end.y), Mathf.Max(start.z, end.z));
        _dragBox.bottomLeft = new Vector3(Mathf.Min(start.x, end.x), Mathf.Min(start.y, end.y), Mathf.Min(start.z, end.z));
        _dragBox.bottomRight = new Vector3(Mathf.Max(start.x, end.x), Mathf.Min(start.y, end.y), Mathf.Min(start.z, end.z));
    }

    private void DrawDebugDragBox()
    {
        Debug.DrawLine(_dragBox.topLeft, _dragBox.topRight, Color.red);   // 왼쪽 상단 -> 오른쪽 상단
        Debug.DrawLine(_dragBox.topRight, _dragBox.bottomRight, Color.red); // 오른쪽 상단 -> 오른쪽 하단
        Debug.DrawLine(_dragBox.bottomRight, _dragBox.bottomLeft, Color.red); // 오른쪽 하단 -> 왼쪽 하단
        Debug.DrawLine(_dragBox.bottomLeft, _dragBox.topLeft, Color.red); // 왼쪽 하단 -> 왼쪽 상단
    }

    private void DrawDragBox()
    {
        _lineObject = new GameObject();
        _lineObject.AddComponent<LineRenderer>();
        LineRenderer lineRenderer = _lineObject.GetComponent<LineRenderer>();
        lineRenderer.sortingOrder = 100;  // 렌더링 순서 설정

        /*lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;*/
        lineRenderer.positionCount = 5;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.SetColor("_Color", Color.green);
        lineRenderer.textureMode = LineTextureMode.Tile;

        lineRenderer.SetPosition(0, _dragBox.topLeft + new Vector3(0, 0.08f, 0));
        lineRenderer.SetPosition(1, _dragBox.bottomLeft + new Vector3(0, 0.08f, 0));
        lineRenderer.SetPosition(2, _dragBox.bottomRight + new Vector3(0, 0.08f, 0));
        lineRenderer.SetPosition(3, _dragBox.topRight + new Vector3(0, 0.08f, 0));
        lineRenderer.SetPosition(4, _dragBox.topLeft + new Vector3(0, 0.08f, 0));
    }

    private void DestroyDragBox()
    {
        Destroy(_lineObject);
    }

    private Collider[] SelectObjectInBox()
    {
        Collider[] colliders = Physics.OverlapBox((startPos + endPos) / 2, new Vector3(
            Mathf.Abs(startPos.x - endPos.x),
            Mathf.Abs(100),
            Mathf.Abs(startPos.z - endPos.z)
        ) / 2, Quaternion.identity);

        return colliders;
    }
}
