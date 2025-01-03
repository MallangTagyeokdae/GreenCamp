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
        buildingCost : 0,
        buildingLevel : 1
        )
    {
    }

    public void Init(string teamID, int buildingID, Vector3 buildingLocation)
    {
        this.teamID = teamID;
        this.buildingID = buildingID;
        this.buildingType = "ResourceBuilding";
        this.buildingLocation = buildingLocation;
        this.buildingHealth = 500;
        this.buildingLevel = 1;
        this.buildingCost = 0;
    }
}
