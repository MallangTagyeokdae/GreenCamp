using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command : Building
{
    public int attackPower { get; set; }
    public int attackRange { get; set; }

    public Command(string teamID, int buildingID, int buildingType, Vector3 buildingLocation, int buildingHealth, int buildingCost, int attackPower, int attackRange)
     : base(teamID, buildingID, 0, buildingLocation, buildingHealth, buildingCost)
    {
        attackPower = attackPower;
        attackRange = attackRange;
    }
}
