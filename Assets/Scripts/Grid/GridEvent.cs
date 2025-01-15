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
        Builted = 3
    }

    public Material defaultMaterial;
    public Material hoveredMaterial;
    public Material selectedMaterial;
    public Material builtedMaterial;
    public State gridState = State.Default;
    private ClickEventHandler _clickEventHandler;
    private MeshRenderer _meshRenderer;

    void Start()
    {
        UnSetRender();
        _meshRenderer = GetComponent<MeshRenderer>();
        _clickEventHandler = GetComponent<ClickEventHandler>();
        _clickEventHandler.mouseHoverEvent.AddListener((Vector3 pos) => GameManager.instance.gridHandler.HoveredGrid(pos));
        _clickEventHandler.leftClickDownEvent.AddListener((Vector3 pos) => {
            GameManager.instance.CreateBuilding(gameObject.transform.position);
            GameManager.instance.SetClickedObject(GameManager.instance.ground);
        });
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
    public void SetBuilted()
    {
        gridState = State.Builted;
    }
    public void SetHovered()
    {
        gridState = State.Hovered;
    }
    public void SetDefault()
    {
        gridState = State.Default;
    }
    public void SetSelected()
    {
        gridState = State.Selected;
    }

    public bool GetIsHovered()
    {
        return gridState == State.Hovered ? true : false;
    }
    public bool GetIsBuilted()
    {
        return gridState == State.Builted ? true : false;
    }
    public bool GetIsSelected()
    {
        return gridState == State.Selected ? true : false; 
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
            case State.Builted:
                _meshRenderer.material = builtedMaterial;
                break;
        }
    }

    private void OnTriggerEnter(Collider obj)
    {
        if(obj.CompareTag("Clickable"))
        {
            SetBuilted();
            ChangeMesh();
        }
    }

    private void OnTriggerExit(Collider obj)
    {
        if(obj.CompareTag("Clickable"))
        {
            SetDefault();
            ChangeMesh();
        }
    }
}