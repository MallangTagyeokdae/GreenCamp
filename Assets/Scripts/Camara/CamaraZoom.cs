using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class CamaraZoom : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public Transform cameraTransform;
    public float zoomSpeed = 10f;
    public float minFOV = 10f;
    public float maxFOV = 70f;
    public float minRotationX = 10f;
    public float maxRotationX = 45f;
    public float minZoomTOOX = 25f;
    public float minZoomTOOY = 30f;
    public float maxZoomTOOX = 50f;
    public float maxZoomTOOY = 12.5f;


    // Update is called once per frame
    void Update()
    {
        // 마우스 스크롤 받기
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        Console.Write(scrollInput);

        if(scrollInput != 0)
        {
            float currentFOV = virtualCamera.m_Lens.FieldOfView;

            currentFOV -= scrollInput * zoomSpeed;
            currentFOV = Mathf.Clamp(currentFOV, minFOV, maxFOV);

            // FOV 설정
            virtualCamera.m_Lens.FieldOfView = currentFOV;

            // FOV 비율에 따라 Rotation x 계산
            float t = Mathf.InverseLerp(minFOV, maxFOV, currentFOV); // 확대되는 정도를 0~1 사이의 값으로 바꾼다.
            float newRotationX = Mathf.Lerp(minRotationX, maxRotationX, t);

            // 카메라 Rotation x 업데이트
            Vector3 newRotation = cameraTransform.eulerAngles;
            newRotation.x = newRotationX;
            cameraTransform.eulerAngles = newRotation;

            ZoomCamara(t);
        }
    }

    private void ZoomCamara(float t)
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
