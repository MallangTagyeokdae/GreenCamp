using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBuilding : Building
{
    public ResourceBuilding(string teamID, int buildingID, Vector3 buildingLocation)
     : base(
        teamID,
        buildingID,
        buildingType : "ResourceBuilding",
        buildingLocation,
        buildingMaxHealth : 500,
        buildingCost : 0,
        buildingLevel : 1
        )
    {
    }

    public void Init(string teamID, int buildingID, Vector3 buildingLocation, Slider buildingHealthBar, Slider buildingProgressBar)
    {
        this.teamID = teamID;
        this.buildingID = buildingID;
        this.buildingType = "ResourceBuilding";
        this.buildingLocation = buildingLocation;
        this.buildingMaxHealth = 250;
        this.buildingCurrentHealth = 0;
        this.buildingLevel = 1;
        this.buildingCost = 0;
        this.buildingHealthBar = buildingHealthBar;
        this.buildingProgressBar = buildingProgressBar;
    }

    public override void InitTime()
    {
        time = 0f;
        loadingTime = 20f;
        gameObject.GetComponent<MeshFilter>().mesh = progressMesh1;
    }

    public override void UpdateCreateBuildingTime(float update)
    {
        float incrementPerSec = buildingMaxHealth / loadingTime;
        time = update;
        this.buildingCurrentHealth += incrementPerSec * Time.deltaTime;
        this.buildingProgress = time/loadingTime*100;

        this.buildingHealthBar.value = (float)(buildingCurrentHealth * 1.0 / buildingMaxHealth);
        this.buildingProgressBar.value = (float)this.buildingProgress / 100;
        UpdateMesh();
    }
    public override void UpdateMesh()
    {
        if (time > loadingTime)
        {
            this.gameObject.GetComponent<MeshFilter>().mesh = completeMesh;
        }
    }
    public override void InitOrderTime(float totalTime){}
    public override void UpdateOrderTime(float update){}
}
