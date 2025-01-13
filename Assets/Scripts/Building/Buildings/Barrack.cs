using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Barrack : Building
{
    public Vector3 _sponPos { get; set; }
    public Barrack(string teamID, int buildingID, Vector3 buildingLocation)
     : base(
        teamID,
        buildingID,
        type: "Barrack",
        buildingLocation,
        maxHealth: 500,
        cost: 50,
        level: 1
        )
    { }

    public void Init(string teamID, int buildingID, Vector3 buildingLocation, Slider buildingHealthBar, Slider buildingProgressBar)
    {
        this.teamID = teamID;
        this.ID = buildingID;
        this.type = "Barrack";
        this.location = buildingLocation;
        this.maxHealth = 500;
        this.currentHealth = 0;
        this.progress = 0;
        this.level = 1;
        this.cost = 50;
        this._sponPos = new Vector3(buildingLocation.x, buildingLocation.y, buildingLocation.z - 4f);
        this.healthBar = buildingHealthBar;
        this.progressBar = buildingProgressBar;
    }

    public void SetSponPos(Vector3 setSponPos)
    {
        this._sponPos = setSponPos;
    }


    public override void InitTime()
    {
        time = 0f;
        loadingTime = 30/10f;
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
        if (time > loadingTime/2 && time < loadingTime) {
            this.gameObject.GetComponent<MeshFilter>().mesh = progressMesh2;
        } else if (time > loadingTime)
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
