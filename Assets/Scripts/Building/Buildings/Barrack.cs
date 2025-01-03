using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrack : Building
{
    public Barrack(string teamID, int buildingID, Vector3 buildingLocation)
     : base(
        teamID,
        buildingID,
        buildingType : "Barrack",
        buildingLocation,
        buildingHealth : 500,
        buildingCost : 50,
        buildingLevel : 1
        )
    {}

    public void Init(string teamID, int buildingID, Vector3 buildingLocation)
    {
        this.teamID = teamID;
        this.buildingID = buildingID;
        this.buildingType = "Barrack";
        this.buildingLocation = buildingLocation;
        this.buildingHealth = 500;
        this.buildingLevel = 1;
        this.buildingCost = 50;
    }
}
