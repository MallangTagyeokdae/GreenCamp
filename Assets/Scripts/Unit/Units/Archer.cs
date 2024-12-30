using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Unit
{
    public Archer(string teamID, int unitID, Vector3 unitLocation)
                : base (
                teamID,
                unitID,
                unitType: 0,
                unitLocation, 
                unitHealth: 0, 
                unitCost: 0, 
                unitPower: 0, 
                unitPowerRange: 0, 
                unitMoveSpeed: 0, 
                populationCost: 0){}
}
