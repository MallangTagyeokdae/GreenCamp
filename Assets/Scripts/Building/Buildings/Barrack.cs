using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
        this.loadingTime = 30f;
    }

    public void SetSponPos(Vector3 setSponPos)
    {
        this._sponPos = setSponPos;
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
