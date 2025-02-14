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
        this.maxHealth = 25;
        this.currentHealth = 0;
    }
    public int increasePersent { get; set; }
    public int damageUpgradeCost { get; set; }
    public int armorUpgradeCost { get; set; }
    public int damageLevel { get; set; }
    public int armorLevel { get; set; }
    
    public Academy(string teamID, int buildingID, Vector3 buildingLocation)
     : base(
        teamID,
        buildingID,
        type : "Academy",
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
        this.type = "Academy";
        this.location = buildingLocation;
        this.progress = 0;
        this.level = 1;
        this.cost = 25;
        this.levelUpCost = 100;
        this.healthBar = buildingHealthBar;
        this.progressBar = buildingProgressBar;
        this.loadingTime = 15f;
        this.underGrid = colliders;
        this.population = 1;
        this.increasePersent = 10;
        this.fow = 40;
        this.damageLevel = 1;
        this.armorLevel = 1;
        this.damageUpgradeCost = 30;
        this.armorUpgradeCost = 30;
    }

    [PunRPC]
    public override void DestroyEntity()
    {
        Destroy(gameObject);
        GameStatus.instance.maxUnitCount -= increasePersent;
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
