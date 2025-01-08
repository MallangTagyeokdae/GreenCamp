using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tanker : Unit
{
    public Tanker(string teamID, int unitID, Vector3 unitLocation)
                : base(
                teamID,
                unitID,
                unitType: "Tanker",
                unitLocation,
                unitMaxHealth: 150,
                unitCurrentHealth: 150,
                unitCost: 0,
                unitPower: 15,
                unitPowerRange: 0,
                unitMoveSpeed: 0,
                populationCost: 0)
    { }
    public void Init(string teamID, int unitID, Vector3 unitLocation)
    {
        this.teamID = teamID;
        this.unitID = unitID;
        this.unitType = "Tanker";
        this.unitLocation = unitLocation;
        this.unitMaxHealth = 150;
        this.unitCurrentHealth = 150;
        this.unitCost = 0;
        this.unitPower = 15;
        this.unitPowerRange = 0;
        this.unitMoveSpeed = 3;
        this.populationCost = 0;
    }
}
