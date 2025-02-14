using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Command : Building
{
    void Awake()
    {
        this.maxHealth = 500;
        this.currentHealth = 500;
    }

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

    public void Init(string teamID, int buildingID, Vector3 buildingLocation, Slider buildingHealthBar, Slider buildingProgressBar, List<Collider> colliders)
    {
        this.teamID = teamID;
        this.ID = buildingID;
        this.type = "Command";
        this.location = buildingLocation;
        this.level = 1;
        this.cost = 0;
        this.levelUpCost = 100;
        this.attackPower = 10;
        this.attackRange = 10;
        this.healthBar = buildingHealthBar;
        this.progressBar = buildingProgressBar;
        this.loadingTime = 10f;
        this.underGrid = colliders;
        this.population = 0;
        this.fow = 75;
        this.armor = 20;
    }


    public override void InitTime()
    {
        time = 0f;
        loadingTime = .1f;
        gameObject.GetComponent<MeshFilter>().mesh = progressMesh1;
    }

    [PunRPC]
    public override void DestroyEntity()
    {
        GameStatus.instance.isWin = false;
        GameManager.instance.SetState("EndGame");
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
