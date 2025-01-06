using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command : Building
{
    public int attackPower { get; set; }
    public int attackRange { get; set; }
    
    public Command(string teamID, int buildingID, Vector3 buildingLocation)
     : base(
        teamID,
        buildingID,
        buildingType : "Command",
        buildingLocation,
        buildingMaxHealth : 500,
        buildingCost : 0,
        buildingLevel : 1
        )
    {
        attackPower = 10;
        attackRange = 10;
    }

    public void Init(string teamID, int buildingID, Vector3 buildingLocation)
    {
        this.teamID = teamID;
        this.buildingID = buildingID;
        this.buildingType = "Command";
        this.buildingLocation = buildingLocation;
        this.buildingMaxHealth = 500;
        this.buildingCurrentHealth = 0;
        this.buildingLevel = 1;
        this.buildingCost = 0;
        this.attackPower = 10;
        this.attackRange = 10;
    }

    public override void InitTime()
    {
        time = 0f;
        loadingTime = 10f;
        gameObject.GetComponent<MeshFilter>().mesh = progressMesh1;
    }

    public override void UpdateTime(float update)
    {
        time = update;
        UpdateMesh();
    }
    public override void UpdateMesh()
    {
        if (time > loadingTime)
        {
            this.gameObject.GetComponent<MeshFilter>().mesh = completeMesh;
        }
    }
}
