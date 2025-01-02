using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender : Building
{
    public int attackPower { get; set; }
    public int attackRange { get; set; }
    
    public Defender(string teamID, int buildingID, Vector3 buildingLocation)
     : base(
        teamID,
        buildingID,
        buildingType : "Defender",
        buildingLocation,
        buildingHealth : 500,
        buildingCost : 0,
        buildingLevel : 1
        )
    {
        attackPower = 20;
        attackRange = 10;
    }
}
