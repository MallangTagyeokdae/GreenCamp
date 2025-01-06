using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Barrack : Building
{
    public Vector3 _sponPos { get; set; }
    public Barrack(string teamID, int buildingID, Vector3 buildingLocation)
     : base(
        teamID,
        buildingID,
        buildingType: "Barrack",
        buildingLocation,
        buildingMaxHealth: 500,
        buildingCost: 50,
        buildingLevel: 1
        )
    { }

    public void Init(string teamID, int buildingID, Vector3 buildingLocation)
    {
        this.teamID = teamID;
        this.buildingID = buildingID;
        this.buildingType = "Barrack";
        this.buildingLocation = buildingLocation;
        this.buildingMaxHealth = 500;
        this.buildingCurrentHealth = 0;
        this.buildingLevel = 1;
        this.buildingCost = 50;
        this._sponPos = new Vector3(buildingLocation.x, buildingLocation.y, buildingLocation.z - 4f);
    }

    public void SetSponPos(Vector3 setSponPos)
    {
        this._sponPos = setSponPos;
    }


    public override void InitTime()
    {
        time = 0f;
        loadingTime = 30f;
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
        if (time > loadingTime/2 && time < loadingTime) {
            this.gameObject.GetComponent<MeshFilter>().mesh = progressMesh2;
        } else if (time > loadingTime)
        {
            this.gameObject.GetComponent<MeshFilter>().mesh = completeMesh;
        }
    }
}
