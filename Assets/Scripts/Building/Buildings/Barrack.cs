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
}
