using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClickEventHandler : MonoBehaviour
{
    public UnityEvent<Vector3> leftClickDownEvent;
    public UnityEvent<Vector3> leftClickUpEvent;
    public UnityEvent<Vector3> rightClickDownEvent;
    public UnityEvent<Vector3> rightClickUpEvent;
    public UnityEvent<Vector3> mouseHoverEvent;
    public UnityEvent<Vector3> deMouseHoverEvent;
    public UnityEvent draggedEvent;
    public void LeftClickDown(Vector3 pos)
    {
        leftClickDownEvent?.Invoke(pos);
    }

    public void RightClickDown(Vector3 pos)
    {
        rightClickDownEvent?.Invoke(pos);
    }

    public void LeftClickUp(Vector3 pos)
    {
        leftClickUpEvent?.Invoke(pos);
    }

    public void RightClickUp(Vector3 pos)
    {
        rightClickUpEvent?.Invoke(pos);
    }

    public void OnMouseHover(Vector3 pos)
    {
        mouseHoverEvent?.Invoke(pos);
    }

    public void DeMouseHover(Vector3 pos)
    {
        deMouseHoverEvent?.Invoke(pos);
    }

    public void Dragged(){
        draggedEvent?.Invoke();
    }
}
