using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBuilding : Building
{
    private void Awake()
    {
        this.maxHealth = 750;
        this.currentHealth = 0f;
        this.armor = 10;
    }
    public float increasePersent { get; set; }
    public ResourceBuilding(string teamID, int buildingID, Vector3 buildingLocation)
     : base(
        teamID,
        buildingID,
        type : "ResourceBuilding",
        buildingLocation,
        maxHealth : 500,
        cost : 0,
        level : 1
        )
    {
    }

    public void Init(string teamID, int buildingID, Vector3 buildingLocation, Slider buildingHealthBar, Slider buildingProgressBar, List<Collider> colliders)
    {
        this.teamID = teamID;
        this.ID = buildingID;
        this.type = "ResourceBuilding";
        this.location = buildingLocation;
        this.maxHealth = 250;
        this.currentHealth = .1f;
        this.progress = 0;
        this.level = 1;
        this.cost = 70;
        this.levelUpCost = 75;
        this.increaseLevelCost = 15;
        this.healthBar = buildingHealthBar;
        this.progressBar = buildingProgressBar;
        this.loadingTime = 25f;
        this.underGrid = colliders;
        this.population = 1;
        this.increasePersent = .5f;
        this.fow = 40;
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
