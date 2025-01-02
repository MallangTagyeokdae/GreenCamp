using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command : Building
{
    public int attackPower { get; set; }
    public int attackRange { get; set; }
    
    public Command(string teamID, int buildingID, Vector3 buildingLocation)
     : base(
        teamID,
        buildingID,
        buildingType : "Command",
        buildingLocation,
        buildingHealth : 500,
        buildingCost : 0,
        buildingLevel : 1
        )
    {
        attackPower = 10;
        attackRange = 10;
    }
}
