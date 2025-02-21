using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PopulationBuilding : Building
{
    private void Awake()
    {
        this.maxHealth = 750;
        this.currentHealth = 0;
        this.armor = 10;
    }
    public int increasePersent { get; set; }
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

    public void Init(string teamID, int buildingID, Vector3 buildingLocation, Slider buildingHealthBar, Slider buildingProgressBar, List<Collider> colliders)
    {
        this.teamID = teamID;
        this.ID = buildingID;
        this.type = "PopulationBuilding";
        this.location = buildingLocation;
        this.progress = 0;
        this.level = 1;
        this.cost = 100;
        this.levelUpCost = 50;
        this.increaseLevelCost = 50;
        this.healthBar = buildingHealthBar;
        this.progressBar = buildingProgressBar;
        this.loadingTime = 15f;
        this.underGrid = colliders;
        this.population = 1;
        this.increasePersent = 5;
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
