using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Command : Building
{
    void Awake()
    {
        this.maxHealth = 2000;
        this.currentHealth = 2000;
        this.armor = 10;
    }

    public int attackPower { get; set; }
    public int attackRange { get; set; }
    public Vector3 _sponPos { get; set; }

    public List<GameObject> turretArchers;
    public List<GameObject> turretMagician;
    
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
        this.levelUpCost = 50;
        this.increaseLevelCost = 200;
        this.attackPower = 10;
        this.attackRange = 10;
        this._sponPos = new Vector3(buildingLocation.x, buildingLocation.y, buildingLocation.z - 8.0f);
        this.healthBar = buildingHealthBar;
        this.progressBar = buildingProgressBar;
        this.loadingTime = 10f;
        this.underGrid = colliders;
        this.population = 0;
        this.fow = 75;
    }


    public override void InitTime()
    {
        time = 0f;
        loadingTime = .1f;
        gameObject.GetComponent<MeshFilter>().mesh = progressMesh1;
    }
    public void SetSponPos(Vector3 setSponPos)
    {
        this._sponPos = setSponPos;
    }
    
    [PunRPC]
    public void SetMagician()
    {
        for(int i=0; i<turretArchers.Count; i++)
        {
            turretArchers[i].SetActive(false);
            turretMagician[i].SetActive(true);
        }
    }

    [PunRPC]
    public override void DestroyEntity()
    {
        GameStatus.instance.isWin = false;
        gameObject.GetComponent<PhotonView>().RPC("RemoveCommandUnit", RpcTarget.AllBuffered);
        GameManager.instance.SetState("EndGame");
    }

    [PunRPC]
    public void RemoveCommandUnit()
    {
        foreach(GameObject unit in turretArchers)
        {
            if(unit.activeSelf == true)
            {
                unit.SetActive(false);
            }
        }

        foreach(GameObject unit in turretMagician)
        {
            if(unit.activeSelf == true)
            {
                unit.SetActive(false);
            }
        }
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
