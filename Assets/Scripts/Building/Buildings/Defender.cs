using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender : Building
{
    public int attackPower { get; set; }
    public int attackRange { get; set; }
    
    public Defender(string teamID, int buildingID, Vector3 buildingLocation)
     : base(
        teamID,
        buildingID,
        buildingType : "Defender",
        buildingLocation,
        buildingMaxHealth : 200,
        buildingCost : 0,
        buildingLevel : 1
        )
    {
        attackPower = 20;
        attackRange = 10;
    }

    public void Init(string teamID, int buildingID, Vector3 buildingLocation)
    {
        this.teamID = teamID;
        this.buildingID = buildingID;
        this.buildingType = "Defender";
        this.buildingLocation = buildingLocation;
        this.buildingMaxHealth = 200;
        this.buildingCurrentHealth = 0;
        this.buildingLevel = 1;
        this.buildingCost = 0;
        this.attackPower = 20;
        this.attackRange = 10;
    }

    public override void InitTime()
    {
        time = 0f;
        loadingTime = 20f;
        gameObject.GetComponent<MeshFilter>().mesh = progressMesh1;
    }

    public override void UpdateTime(float update)
    {
        time = update;
        this.buildingCurrentHealth = (int)Math.Round(time/loadingTime*buildingMaxHealth);
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
