using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class CamaraZoom : MonoBehaviour
{
    // CinemachineVirtualCamera 참조
    public CinemachineVirtualCamera virtualCamera;
    public Transform cameraTransform;
    // ---------------------------------
    // 줌 속도 및 줌 최대 최소 수치
    public float zoomSpeed = 10f;

    public float minFOV = 10f;
    public float maxFOV = 70f;
    // ---------------------------------
    // 줌됐을 때 물체를 쿼터뷰에서 사이드 뷰로 전환하는데 이때 Min, Max 값
    public float minRotationX = 10f;
    public float maxRotationX = 45f;
    public float minZoomTOOX = 25f;
    public float minZoomTOOY = 30f;
    public float maxZoomTOOX = 50f;
    public float maxZoomTOOY = 12.5f;
    // ---------------------------------


    // Update is called once per frame
    void Update()
    {
        // 마우스 스크롤 신호 받기
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if(scrollInput != 0) // 스크롤이 된다면 실행
        {
            // 카메라에서 현재 확대 수치를 받음
            float currentFOV = virtualCamera.m_Lens.FieldOfView;

            // 확대 수치 계산
            currentFOV -= scrollInput * zoomSpeed;
            currentFOV = Mathf.Clamp(currentFOV, minFOV, maxFOV);

            // VM 카메라의 FOV 설정
            virtualCamera.m_Lens.FieldOfView = currentFOV;

            // FOV 비율에 따라 Rotation x 계산
            float t = Mathf.InverseLerp(minFOV, maxFOV, currentFOV); // 확대되는 정도를 0~1 사이의 값으로 바꾼다.
            
            UpdateRotation(t);
            // 확대 정도에 따라서 VM카메라의 TrackedObjectOffset을 조정한다.
            UpdateTOOoffset(t);
        }
    }

    private void UpdateRotation(float t)
    {
        float newRotationX = Mathf.Lerp(minRotationX, maxRotationX, t);

        // 카메라 Rotation x 업데이트
        Vector3 newRotation = cameraTransform.eulerAngles;
        newRotation.x = newRotationX;
        cameraTransform.eulerAngles = newRotation;
    }

    private void UpdateTOOoffset(float t)
    {
        var framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        Vector3 TOOoffset = framingTransposer.m_TrackedObjectOffset;
        
        float newTOOX = Mathf.Lerp(maxZoomTOOX, minZoomTOOX, t);
        float newTOOY = Mathf.Lerp(maxZoomTOOY, minZoomTOOY, t);

        TOOoffset.x = newTOOX;
        TOOoffset.y = newTOOY;
        framingTransposer.m_TrackedObjectOffset = TOOoffset;
    }

}
