using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridEvent : MonoBehaviour
{
    public Material defaultMaterial;
    public Material hoveredMaterial;
    private bool _isHovered = false;
    private bool _isBuilted = false;
    public Material builtedMaterial;
    private ClickEventHandler _clickEventHandler;
    private MeshRenderer _meshRenderer;

    void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _clickEventHandler = GetComponent<ClickEventHandler>();
        _clickEventHandler.mouseHoverEvent.AddListener((Vector3 pos) => OnMouseHovered());
        _clickEventHandler.deMouseHoverEvent.AddListener((Vector3 pos) => OnMouseHoveredOut());
        _clickEventHandler.leftClickDownEvent.AddListener((Vector3 pos) => {
            GameManager.instance.CreateBuilding(gameObject.transform.position);
            GameManager.instance.SetClickedObject(GameManager.instance.ground);
            OnBuiltIn();
        });
    }
    public void OnBuiltIn()
    {
        _isBuilted = true;
        ChangeMesh();
    }
    public void OnMouseHovered()
    {
        SetisHovered();
        ChangeMesh();
    }

    public void OnMouseHoveredOut()
    {
        UnSetHovered();
        ChangeMesh();
    }

    public void SetisHovered()
    {
        if(!_isHovered)
        {
            _isHovered = true;
        }
    }

    public void UnSetHovered()
    {
        if(_isHovered)
        {
            _isHovered = false;
        }
    }
    public void ChangeMesh()
    {
        if(!_isBuilted)
        {
            _meshRenderer.material = _isHovered ? hoveredMaterial : defaultMaterial;
        } else {
            _meshRenderer.material = builtedMaterial;
            gameObject.tag = "Untagged";
        }
    }
}