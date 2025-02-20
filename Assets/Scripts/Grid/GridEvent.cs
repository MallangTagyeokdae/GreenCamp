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
    public List<Collider> detectedColliders = new List<Collider>();

    void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }
    void Start()
    {
        UnSetRender();
        _clickEventHandler = GetComponent<ClickEventHandler>();
        _clickEventHandler.mouseHoverEvent.AddListener((Vector3 pos) => GameManager.instance.gridHandler.HoveredGrid(pos));
        _clickEventHandler.leftClickDownEvent.AddListener((Vector3 pos) => {
            GameManager.instance.CreateBuilding();
            GameManager.instance.SetClickedObject(GameManager.instance.ground);
        });
        _clickEventHandler.rightClickDownEvent.AddListener((Vector3 pos) => {
            GameManager.instance.SetState("InGame");
            GameManager.instance.SetClickedObject(GameManager.instance.ground);
            GameManager.instance.SetBuildingListUI();
            GameManager.instance.grid.SetActive(false);
        });
    }
    private void OnDisable()
    {
        UnSetRender();
        detectedColliders.Clear();
    }
    public void OnBuiltIn()
    {
        SetBuilted();
        ChangeMesh();
    }

    public void UnSetRender()
    {
        gameObject.GetComponent<Renderer>().enabled = false;
    }
    public void SetRander()
    {
        gameObject.GetComponent<Renderer>().enabled = true;
    }
    public void SetDefault()
    {
        gameObject.tag = "Clickable";
        gridState = State.Default;
    }
    public void SetHovered()
    {
        gridState = State.Hovered;
    }
    public void SetSelected()
    {
        gridState = State.Selected;
    }
    public void SetHasObject()
    {
        gridState = State.HasObject;
    }
    public void SetBuilted()
    {
        gameObject.tag = "Untagged";
        gridState = State.Builted;
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

            SetHasObject();
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
                SetDefault();
                ChangeMesh();
            }
        }
    }

    public void CheckHasObject()
    {
        if(detectedColliders.Count == 0 && !GetIsBuilted())
        {
            SetDefault();
        }
    }
}