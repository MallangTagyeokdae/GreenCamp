using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceBuilding : Building
{
    public ResourceBuilding(string teamID, int buildingID, Vector3 buildingLocation)
     : base(
        teamID,
        buildingID,
        buildingType : "ResourceBuilding",
        buildingLocation,
        buildingHealth : 500,
        buildingCost : 0
        )
    {
    }
}
