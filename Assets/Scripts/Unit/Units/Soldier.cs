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
                unitMaxHealth: 100,
                unitCurrentHealth: 100,
                unitCost: 0,
                unitPower: 30,
                unitPowerRange: 1,
                unitMoveSpeed: 2,
                populationCost: 1)
    { }
    public void Init(string teamID, int unitID, Vector3 unitLocation)
    {
        this.teamID = teamID;
        this.unitID = unitID;
        this.unitType = "Soldier";
        this.unitLocation = unitLocation;
        this.unitMaxHealth = 100;
        this.unitCurrentHealth = 100;
        this.unitCost = 0;
        this.unitPower = 30;
        this.unitPowerRange = 1;
        this.unitMoveSpeed = 2;
        this.populationCost = 1;
    }
}

