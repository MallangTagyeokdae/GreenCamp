using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingID : MonoBehaviour
{
    public int key;

    public void SetKey(int newKey)
    {
        key = newKey;
    }

    public int GetKey()
    {
        return key;
    }
}
