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
                unitCost: 50,
                unitPower: 15,
                unitPowerRange: 3,
                unitMoveSpeed: 1,
                populationCost: 1)
    { }
    public void Init(string teamID, int unitID, Vector3 unitLocation)
    {
        this.teamID = teamID;
        this.unitID = unitID;
        this.unitType = "Tanker";
        this.unitLocation = unitLocation;
        this.maxHealth = 150;
        this.currentHealth = 150;
        this.unitCost = 50;
        this.unitPower = 15;
        this.unitPowerRange = 3;
        this.unitMoveSpeed = 1;
        this.populationCost = 1;
        this.population = 2;
    }
}
