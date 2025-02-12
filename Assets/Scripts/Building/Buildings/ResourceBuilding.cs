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
        this.maxHealth = 250;
        this.currentHealth = 0f;
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
        this.cost = 45;
        this.levelUpCost = 100;
        this.healthBar = buildingHealthBar;
        this.progressBar = buildingProgressBar;
        this.loadingTime = 20f;
        this.underGrid = colliders;
        this.population = 1;
        this.increasePersent = 1f;
        this.fow = 25;
    }

    [PunRPC]
    public override void DestroyEntity()
    {
        Destroy(gameObject);
        GameStatus.instance.resourcePerSecond -= increasePersent;
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
