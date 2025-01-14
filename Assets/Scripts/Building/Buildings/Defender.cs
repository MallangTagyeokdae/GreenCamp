using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
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
        this.loadingTime = 20f;
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
