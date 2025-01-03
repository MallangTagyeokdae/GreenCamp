using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : Unit
{
    public Healer(string teamID, int unitID, Vector3 unitLocation)
                : base (
                teamID,
                unitID,
                unitType: "Healer",
                unitLocation, 
                unitHealth: 0, 
                unitCost: 0, 
                unitPower: 0, 
                unitPowerRange: 0, 
                unitMoveSpeed: 0, 
                populationCost: 0){}
}
