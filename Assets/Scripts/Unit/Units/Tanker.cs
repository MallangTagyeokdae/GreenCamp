using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tanker : Unit
{
    void Start()
    {
        this.maxHealth = 200  + GameStatus.instance.healthIncrease;
        this.currentHealth = 200  + GameStatus.instance.healthIncrease;
        this.unitPower = 15 + GameStatus.instance.damageIncrease;
        this.armor = 15 + GameStatus.instance.armorIncrease;
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
        this.unitCost = 85;
        this.unitPowerRange = 3;
        this.unitMoveSpeed = 5;
        this.populationCost = 1;
        this.population = 2;
        this.fow = 30;
    }
}
