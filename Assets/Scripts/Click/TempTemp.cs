using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TempTemp : MonoBehaviour
{
    public Material newMaterial;
    public void AddShader(){
        Debug.Log("work check");
        // MeshRenderer 컴포넌트를 가져옵니다.
        MeshRenderer renderer = this.gameObject.GetComponent<MeshRenderer>();

        // 현재 Materials 배열을 가져옵니다.
        Material[] currentMaterials = renderer.materials;

        // 배열의 크기를 하나 늘려서 새 Material을 추가할 수 있도록 합니다.
        Material[] newMaterials = new Material[currentMaterials.Length + 1];

        // 기존의 Materials 배열을 새 배열에 복사합니다.
        for (int i = 0; i < currentMaterials.Length; i++)
        {
            newMaterials[i] = currentMaterials[i];
        }

        // 새 Material을 마지막에 추가합니다.
        newMaterials[newMaterials.Length - 1] = newMaterial;

        // 새로운 배열을 MeshRenderer의 Materials 배열에 할당합니다.
        renderer.materials = newMaterials;
    }
}
