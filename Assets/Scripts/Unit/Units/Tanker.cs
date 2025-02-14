using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tanker : Unit
{
    void Start()
    {
        this.maxHealth = 150;
        this.currentHealth = 150;
        this.unitPower = 15;
    }
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
        this.unitCost = 50;
        this.unitPowerRange = 3;
        this.unitMoveSpeed = 4;
        this.populationCost = 1;
        this.population = 2;
        this.fow = 30;
        this.armor = 20;
    }
}
