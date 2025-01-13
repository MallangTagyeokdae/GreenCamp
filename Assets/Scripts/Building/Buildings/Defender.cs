using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Defender : Building
{
    public int attackPower { get; set; }
    public int attackRange { get; set; }
    
    public Defender(string teamID, int buildingID, Vector3 buildingLocation)
     : base(
        teamID,
        buildingID,
        type : "Defender",
        buildingLocation,
        maxHealth : 200,
        cost : 0,
        level : 1
        )
    {
        attackPower = 20;
        attackRange = 10;
    }

    public void Init(string teamID, int buildingID, Vector3 buildingLocation, Slider buildingHealthBar, Slider buildingProgressBar)
    {
        this.teamID = teamID;
        this.ID = buildingID;
        this.type = "Defender";
        this.location = buildingLocation;
        this.maxHealth = 200;
        this.currentHealth = 0;
        this.progress = 0;
        this.level = 1;
        this.cost = 0;
        this.attackPower = 20;
        this.attackRange = 10;
        this.healthBar = buildingHealthBar;
        this.progressBar = buildingProgressBar;
    }

    public override void InitTime()
    {
        time = 0f;
        loadingTime = 20/10f;
        gameObject.GetComponent<MeshFilter>().mesh = progressMesh1;
    }

    public override void UpdateCreateBuildingTime(float update)
    {
        float incrementPerSec = maxHealth / loadingTime;
        time = update;
        this.currentHealth += incrementPerSec * Time.deltaTime;
        this.progress = time/loadingTime*100;

        this.healthBar.value = (float)(currentHealth * 1.0 / maxHealth);
        this.progressBar.value = (float)this.progress / 100;
        UpdateMesh();
    }
    public override void UpdateMesh()
    {
        if (time > loadingTime)
        {
            this.gameObject.GetComponent<MeshFilter>().mesh = completeMesh;
        }
    }
    public override void InitOrderTime(float totalTime)
    {
        this.state = State.InProgress;
        time = 0f;
        loadingTime = totalTime;
    }
    public override void UpdateOrderTime(float update)
    {
        time = update;
        this.progress = time / loadingTime * 100;
        this.progressBar.value = (float)this.progress / 100;
    }
}
