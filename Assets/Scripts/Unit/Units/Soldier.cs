using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Unit
{
    void Start()
    {
        this.maxHealth = 100  + GameStatus.instance.healthIncrease;
        this.currentHealth = 100  + GameStatus.instance.healthIncrease;
        this.unitPower = 30 + GameStatus.instance.damageIncrease;
    }

    public Soldier(string teamID, int unitID, Vector3 unitLocation)
                : base(
                teamID,
                unitID,
                unitType: "Soldier",
                unitLocation,
                unitMaxHealth: 100,
                unitCurrentHealth: 100,
                unitCost: 30,
                unitPower: 30,
                unitPowerRange: 3,
                unitMoveSpeed: 2,
                populationCost: 1)
    { }
    public void Init(string teamID, int unitID, Vector3 unitLocation)
    {
        this.teamID = teamID;
        this.unitID = unitID;
        this.unitType = "Soldier";
        this.unitLocation = unitLocation;
        this.unitCost = 30;
        this.unitPowerRange = 3;
        this.unitMoveSpeed = 5;
        this.populationCost = 1;
        this.population = 1;
        this.fow = 30;
        this.armor = 10 + GameStatus.instance.armorIncrease;
    }
}

