using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    public string teamID { get; set; }
    public int unitID { get; set; }
    public string unitType { get; set; }
    public Vector3 unitLocation { get; set; }
    public int unitHealth { get; set; }
    public int unitCost { get; set; }
    public int unitPower { get; set; }
    public int unitPowerRange { get; set; }
    public int unitMoveSpeed { get; set; }
    public int populationCost { get; set; }
    public Unit(string teamID,
                int unitID,
                string unitType,
                Vector3 unitLocation,
                int unitHealth,
                int unitCost,
                int unitPower,
                int unitPowerRange,
                int unitMoveSpeed,
                int populationCost)
    {
        this.teamID = teamID;
        this.unitID = unitID;
        this.unitType = unitType;
        this.unitLocation = unitLocation;
        this.unitHealth = unitHealth;
        this.unitCost = unitCost;
        this.unitPower = unitPower;
        this.unitPowerRange = unitPowerRange;
        this.unitMoveSpeed = unitMoveSpeed;
        this.populationCost = populationCost;
    }
}
