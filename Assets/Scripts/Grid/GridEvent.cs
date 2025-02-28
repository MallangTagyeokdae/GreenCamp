using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class GridEvent : MonoBehaviour
{
    public enum State
    {
        Default = 0,
        Hovered = 1,
        Selected = 2,
        HasObject = 3,
        Builted = 4
    }

    public Material defaultMaterial;
    public Material hoveredMaterial;
    public Material selectedMaterial;
    public Material blockMaterial;
    public Material builtedMaterial;
    public State gridState = State.Default;
    private ClickEventHandler _clickEventHandler;
    private MeshRenderer _meshRenderer;
    private MeshCollider _meshCollider;
    public List<Collider> detectedColliders = new List<Collider>();

    void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshCollider = GetComponent<MeshCollider>();
    }

    // 처음에 시작하면 MeshCollider, MeshRenderer, clickEventHandler다 비활성화 시킴
    // OverlapSphere에 들어오면 위에 세개를 다 켬
    // OverlapSphere에 나가면 다시 다 끔
    void Start()
    {
        _clickEventHandler = GetComponent<ClickEventHandler>();
        _clickEventHandler.mouseHoverEvent.AddListener((Vector3 pos) => GameManager.instance.gridHandler.HoveredGrid(pos));
        _clickEventHandler.leftClickDownEvent.AddListener((Vector3 pos) => {
            GameManager.instance.CreateBuilding();
            GameManager.instance.SetClickedObject(GameManager.instance.ground);
            GameManager.instance.gridHandler.ChangeGridNormal();
        });
        _clickEventHandler.rightClickDownEvent.AddListener((Vector3 pos) => {
            GameManager.instance.SetState("InGame");
            GameManager.instance.SetClickedObject(GameManager.instance.ground);
            GameManager.instance.SetBuildingListUI();
            GameManager.instance.gridHandler.ChangeGridNormal();
            
        });
        WhenHoveredOut();
    }
    private void OnDisable()
    {
        ChangeStateRenderer(false);
        detectedColliders.Clear();
    }
    public void OnBuiltIn()
    {
        SetGridState(State.Builted);
        ChangeMesh();
    }

    public void WhenHoveredOut()
    {
        ChangeStateRenderer(false);
        // ChangeStateClickEventHandler(false);
        // ChangeStateMeshCollider(false);
    }

    public void WhenHoveredIn()
    {
        ChangeStateRenderer(true);
        // ChangeStateClickEventHandler(true);
        // ChangeStateMeshCollider(true);
    }  

    public void ChangeStateClickEventHandler(bool state)
    {
        _clickEventHandler.enabled = state;
    }

    public void ChangeStateRenderer(bool state)
    {
        _meshRenderer.enabled = state;
    }
    public void ChangeStateMeshCollider(bool state)
    {
        _meshCollider.enabled = state;
    }

    public void SetGridState(State state)
    {
        switch(state)
        {
            case State.Default:
                gameObject.tag = "Clickable";
                gameObject.tag = "Untagged";
                gridState = State.Default;
                break;
            case State.Hovered:
                gameObject.tag = "Clickable";
                gridState = State.Hovered;
                break;
            case State.Selected:
                gridState = State.Selected;
                break;  
            case State.HasObject:
                gameObject.tag = "Untagged";
                gridState = State.HasObject;
                break;
            case State.Builted:
                gameObject.tag = "Untagged";
                gridState = State.Builted;
                break;
        }
    }

    public bool GetIsHovered()
    {
        return gridState == State.Hovered ? true : false;
    }
    public bool GetIsSelected()
    {
        return gridState == State.Selected ? true : false; 
    }
    public bool GetIsHasObject()
    {
        return gridState == State.HasObject ? true : false;
    }
    public bool GetIsBuilted()
    {
        return gridState == State.Builted ? true : false;
    }
    public void ChangeMesh()
    {
        switch(gridState)
        {
            case State.Default:
                _meshRenderer.material = defaultMaterial;
                break;
            case State.Hovered:
                _meshRenderer.material = hoveredMaterial;
                break;
            case State.Selected:
                _meshRenderer.material = selectedMaterial;
                break;
            case State.HasObject:
                _meshRenderer.material = builtedMaterial;
                break;
            case State.Builted:
                _meshRenderer.material = builtedMaterial;
                break;
        }
    }
    public void ChangeMeshToBlocked()
    {
        _meshRenderer.material = blockMaterial;
    }

    private void OnTriggerEnter(Collider obj)
    {
        if((obj.CompareTag("Clickable") && !GetIsBuilted()) || (obj.gameObject.layer == 7 && !GetIsBuilted()))
        {
            if(!detectedColliders.Contains(obj))
                detectedColliders.Add(obj);

            SetGridState(State.HasObject);
            ChangeMesh();
        }
    }

    private void OnTriggerExit(Collider obj)
    {
        if(obj.CompareTag("Clickable") && !GetIsBuilted())
        {
            detectedColliders.Remove(obj);

            if(detectedColliders.Count == 0)
            {
                SetGridState(State.Default);
                ChangeMesh();
            }
        }
    }

    public void CheckHasObject()
    {
        if(detectedColliders.Count == 0 && !GetIsBuilted())
        {
            SetGridState(State.Default);
        }
    }
}