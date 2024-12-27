using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CamaraMovement : MonoBehaviour
{
    public float edgeSize = 10f; // 가장자리 감지 범위q
    public float moveSpeed = 15f; // 카메라 이동 속도
    public Vector2 xBounds = new Vector2(-250, 250); // x축으로 이동하는 범위
    public Vector2 zBounds = new Vector2(-250, 250); // z축으로 이동하는 범위

    // 화면 모서리에 마우스의 위치 2차원 x,y 좌표를 게임상의 3차원 x,z에 적용시킴
    void Update()
    {
        Vector3 pos = transform.position;

        // 마우스 위치 가져오기
        Vector3 mousePosition = Input.mousePosition;

        // 화면 가장자리 체크
        if (mousePosition.x < edgeSize && pos.x > xBounds.x) // 마우스가 화면 왼쪽
        {
            pos.x -= moveSpeed * Time.deltaTime;
        }
        else if (mousePosition.x > Screen.width - edgeSize && pos.x < xBounds.y) // 마우스가 화면 오른쪽
        {
            pos.x +=  moveSpeed * Time.deltaTime;
        }

        if (mousePosition.y < edgeSize && pos.z > zBounds.x) // 마우스가 화면 아래쪽
        {
            pos.z -= moveSpeed * Time.deltaTime;
        }
        else if (mousePosition.y > Screen.height - edgeSize && pos.z < zBounds.y) // 마우스가 화면 위쪽
        {
            pos.z += moveSpeed * Time.deltaTime;
        }

        // 카메라 위치 업데이트
        transform.position = pos;
    }
}
