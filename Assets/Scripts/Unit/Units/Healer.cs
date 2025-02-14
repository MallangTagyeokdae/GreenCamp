using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : Unit
{
    void Start()
    {
        this.maxHealth = 85 + GameStatus.instance.healthIncrease;
        this.currentHealth = 85 + GameStatus.instance.healthIncrease;
        this.unitPower = 0;
    }
    public GameObject HealingEffect;
    public Healer(string teamID, int unitID, Vector3 unitLocation)
                : base(
                teamID,
                unitID,
                unitType: "Healer",
                unitLocation,
                unitMaxHealth: 85,
                unitCurrentHealth: 85,
                unitCost: 65,
                unitPower: 0,
                unitPowerRange: 4,
                unitMoveSpeed: 1,
                populationCost: 1)
    { }
    public void Init(string teamID, int unitID, Vector3 unitLocation)
    {
        this.teamID = teamID;
        this.unitID = unitID;
        this.unitType = "Healer";
        this.unitLocation = unitLocation;
        this.unitCost = 65;
        this.unitPowerRange = 4;
        this.unitMoveSpeed = 1;
        this.populationCost = 1;
        this.population = 2;
        this.fow = 30;
        this.armor = 5 + GameStatus.instance.armorIncrease;
    }
}
