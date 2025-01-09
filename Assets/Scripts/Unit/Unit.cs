using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Unit : Entity
{
    public string teamID { get; set; }
    public int unitID { get; set; }
    public string unitType { get; set; }
    public Vector3 unitLocation { get; set; }
    public int unitMaxHealth { get; set; }
    public int unitCurrentHealth { get; set; }
    public int unitCost { get; set; }
    public int unitPower { get; set; }
    public int unitPowerRange { get; set; }
    // private Collider u
    public int unitMoveSpeed { get; set; }
    public int populationCost { get; set; }
    public Slider healthBar;

    [HideInInspector]
    public Animator unitAnimator;
    private void Awake()
    {
        unitAnimator = GetComponent<Animator>();
        healthBar = gameObject.transform.Find("HealthBar/UnitCurrentHealth").GetComponent<Slider>();
        healthBar.gameObject.SetActive(false);
        Debug.Log(unitAnimator.name);
    }
    public Coroutine unitBehaviour;

    public Unit(string teamID,
                int unitID,
                string unitType,
                Vector3 unitLocation,
                int unitMaxHealth,
                int unitCurrentHealth,
                int unitCost,
                int unitPower,
                int unitPowerRange,
                int unitMoveSpeed,
                int populationCost)
    {
        this.teamID = teamID;
        this.unitID = unitID;
        this.unitType = unitType;
        transform.position = unitLocation;
        this.unitMaxHealth = unitMaxHealth;
        this.unitCurrentHealth = unitCurrentHealth;
        this.unitCost = unitCost;
        this.unitPower = unitPower;
        this.unitPowerRange = unitPowerRange;
        this.unitMoveSpeed = unitMoveSpeed;
        this.populationCost = populationCost;
    }
}
