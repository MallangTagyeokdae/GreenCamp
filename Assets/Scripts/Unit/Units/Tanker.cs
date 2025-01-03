using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tanker : Unit
{
    public Tanker(string teamID, int unitID, Vector3 unitLocation)
                : base (
                teamID,
                unitID,
                unitType: "Tanker",
                unitLocation, 
                unitHealth: 0, 
                unitCost: 0, 
                unitPower: 0, 
                unitPowerRange: 0, 
                unitMoveSpeed: 0, 
                populationCost: 0){}
}
