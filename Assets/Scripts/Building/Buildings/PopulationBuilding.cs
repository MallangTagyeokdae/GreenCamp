using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PopulationBuilding : Building
{
    public PopulationBuilding(string teamID, int buildingID, Vector3 buildingLocation)
     : base(
        teamID,
        buildingID,
        type : "PopulationBuilding",
        buildingLocation,
        maxHealth : 200,
        cost : 0,
        level : 1
        )
    {
    }

    public void Init(string teamID, int buildingID, Vector3 buildingLocation, Slider buildingHealthBar, Slider buildingProgressBar)
    {
        this.teamID = teamID;
        this.ID = buildingID;
        this.type = "PopulationBuilding";
        this.location = buildingLocation;
        this.maxHealth = 200;
        this.currentHealth = 0;
        this.progress = 0;
        this.level = 1;
        this.cost = 0;
        this.healthBar = buildingHealthBar;
        this.progressBar = buildingProgressBar;
        this.loadingTime = 15f;
    }

    [PunRPC]
    public override void SetProgressMesh1(){
        gameObject.GetComponent<MeshFilter>().mesh = progressMesh1;
    }

    [PunRPC]
    public override void SetProgressMesh2(){
        gameObject.GetComponent<MeshFilter>().mesh = progressMesh2;
    }
    [PunRPC]
    public override void SetCompleteMesh(){
        gameObject.GetComponent<MeshFilter>().mesh = completeMesh;
    }
}
