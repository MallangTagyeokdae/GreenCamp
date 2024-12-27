using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float edgeSize = 10f; // 가장자리 감지 범위q
    public float moveSpeed = 110f; // 카메라 이동 속도
    public Vector2 xBounds = new Vector2(-250, 250); // x축으로 이동하는 범위
    public Vector2 zBounds = new Vector2(-250, 250); // z축으로 이동하는 범위

    // 화면 모서리에 마우스의 위치 2차원 x,y 좌표를 게임상의 3차원 x,z에 적용시킴
    void Update()
    {
        Vector3 pos = transform.position; // 물체 위치를 지정할 변수

        // 마우스 위치 가져오기
        Vector3 mousePosition = Input.mousePosition;

        // 화면 가장자리 체크
        // 마우스가 화면 왼쪽
        if (mousePosition.x < edgeSize && pos.x > xBounds.x)
            pos.x -= moveSpeed * Time.deltaTime;
        
        // 마우스가 화면 오른쪽
        else if (mousePosition.x > Screen.width - edgeSize && pos.x < xBounds.y)
            pos.x +=  moveSpeed * Time.deltaTime;

        // 마우스가 화면 아래쪽
        if (mousePosition.y < edgeSize && pos.z > zBounds.x)
            pos.z -= moveSpeed * Time.deltaTime;
        
        // 마우스가 화면 위쪽
        else if (mousePosition.y > Screen.height - edgeSize && pos.z < zBounds.y)
            pos.z += moveSpeed * Time.deltaTime;

        // 카메라 위치 업데이트
        transform.position = pos;
    }
}
