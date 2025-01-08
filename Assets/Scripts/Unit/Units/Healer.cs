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
                unitMaxHealth: 85,
                unitCurrentHealth: 85,
                unitCost: 0,
                unitPower: 0,
                unitPowerRange: 2,
                unitMoveSpeed: 1,
                populationCost: 1)
    { }
    public void Init(string teamID, int unitID, Vector3 unitLocation)
    {
        this.teamID = teamID;
        this.unitID = unitID;
        this.unitType = "Healer";
        this.unitLocation = unitLocation;
        this.unitMaxHealth = 85;
        this.unitCurrentHealth = 85;
        this.unitCost = 0;
        this.unitPower = 0;
        this.unitPowerRange = 2;
        this.unitMoveSpeed = 1;
        this.populationCost = 1;
    }
}
