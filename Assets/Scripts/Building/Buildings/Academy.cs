using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Academy : Building
{
    private void Awake()
    {
        this.maxHealth = 150;
        this.currentHealth = 0;
    }
    
    public Academy(string teamID, int buildingID, Vector3 buildingLocation)
     : base(
        teamID,
        buildingID,
        type : "Academy",
        buildingLocation,
        maxHealth : 150,
        cost : 0,
        level : 1
        )
    {
    }

    public void Init(string teamID, int buildingID, Vector3 buildingLocation, Slider buildingHealthBar, Slider buildingProgressBar, List<Collider> colliders)
    {
        this.teamID = teamID;
        this.ID = buildingID;
        this.type = "Academy";
        this.location = buildingLocation;
        this.progress = 0;
        this.level = 1;
        this.cost = 40;
        this.levelUpCost = 100;
        this.increaseLevelCost = 30;
        this.healthBar = buildingHealthBar;
        this.progressBar = buildingProgressBar;
        this.loadingTime = 15f;
        this.underGrid = colliders;
        this.population = 2;
        this.fow = 40;
        this.armor = 10;
    }

    [PunRPC]
    public override void DestroyEntity()
    {
        Destroy(gameObject);
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
