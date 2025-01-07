using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : Unit
{
    public Healer(string teamID, int unitID, Vector3 unitLocation)
                : base(
                teamID,
                unitID,
                unitType: "Healer",
                unitLocation,
                unitMaxHealth: 200,
                unitCurrentHealth: 200,
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
        this.unitType = "Healer";
        this.unitLocation = unitLocation;
        this.unitMaxHealth = 200;
        this.unitCurrentHealth = 200;
        this.unitCost = 0;
        this.unitPower = 0;
        this.unitPowerRange = 0;
        this.unitMoveSpeed = 1;
        this.populationCost = 0;
    }
}
