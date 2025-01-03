using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Unit
{
    public Soldier(string teamID, int unitID, Vector3 unitLocation)
                : base(
                teamID,
                unitID,
                unitType: "Soldier",
                unitLocation,
                unitHealth: 0,
                unitCost: 0,
                unitPower: 0,
                unitPowerRange: 0,
                unitMoveSpeed: 0,
                populationCost: 0)
    { }
    public void Init(string teamID, int unitID, Vector3 unitLocation)
    {
        this.teamID = teamID;
        this.unitID = unitID;
        this.unitType = unitType;
        this.unitLocation = unitLocation;
        this.unitHealth = 0;
        this.unitCost = 0;
        this.unitPower = 0;
        this.unitPowerRange = 0;
        this.unitMoveSpeed = 0;
        this.populationCost = 0;
    }
}

