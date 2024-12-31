using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationBuilding : Building
{
    public PopulationBuilding(string teamID, int buildingID, Vector3 buildingLocation)
     : base(
        teamID,
        buildingID,
        buildingType : "PopulationBuilding",
        buildingLocation,
        buildingHealth : 500,
        buildingCost : 0
        )
    {
    }
}
