using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public void createBuilding(Vector3 buildingPos)
    {
        GameManager.instance.createBuilding(buildingPos);
    }
}
