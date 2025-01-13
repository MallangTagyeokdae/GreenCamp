using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectHandler : MonoBehaviour
{
    public List<GameObject> effectLists;

    public GameObject CreateEffect(int index, Transform transform, Vector3 rot, int scale)
    {
        GameObject newEffect = Instantiate(effectLists[index],transform);
        newEffect.transform.localPosition = Vector3.zero;
        newEffect.transform.localScale = new Vector3(scale, scale, scale);
        newEffect.transform.localRotation = Quaternion.Euler(rot);
        return newEffect;
    }

    public void DestoryEffectGetTime(GameObject effect, float time)
    {
        Destroy(effect, time);
    }
    
    public void DestoryEffectImmed(GameObject effect)
    {
        Destroy(effect);
    }
}
