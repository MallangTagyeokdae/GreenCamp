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
        type : "Command",
        buildingLocation,
        maxHealth : 500,
        cost : 0,
        level : 1
        )
    {
        attackPower = 10;
        attackRange = 10;
    }

    public void Init(string teamID, int buildingID, Vector3 buildingLocation)
    {
        this.teamID = teamID;
        this.ID = buildingID;
        this.type = "Command";
        this.location = buildingLocation;
        this.maxHealth = 500;
        this.currentHealth = 0;
        this.level = 1;
        this.cost = 0;
        this.attackPower = 10;
        this.attackRange = 10;
    }

    public override void InitTime()
    {
        time = 0f;
        loadingTime = 10f;
        gameObject.GetComponent<MeshFilter>().mesh = progressMesh1;
    }

    public override void UpdateCreateBuildingTime(float update)
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
