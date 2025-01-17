using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public abstract class Building : Entity
{
    public enum State
    {
        InCreating = 0,  // 건물이 건설 중인 상태
        Built = 1,   // 건물이 완료된 상태
        InProgress = 2,  // 건물이 진행 중인 상태
        Destroy = 3
    }

    public enum InProgressItem
    {
        None = 0,
        LevelUP = 1,
        Soldier = 2,
        Archer = 3,
        Tanker = 4,
        Healer = 5
    }

    //public string teamID { get; set; }
    public int ID { get; set; }
    public string type { get; set; }
    public Vector3 location { get; set; }
    public int maxHealth { get; set; }
    public float currentHealth { get; set; }
    public float progress { get; set; }
    public int cost { get; set; }
    public int level { get; set; }
    public Slider healthBar { get; set; }
    public Slider progressBar { get; set; }
    public float time { get; set; }
    public float loadingTime{get; set;}
    public State state = State.InCreating;
    public InProgressItem inProgressItem = InProgressItem.None;
    public Mesh progressMesh1;
    public Mesh progressMesh2;
    public Mesh completeMesh;
    public GameObject completeEffect;
    public GameObject levelUpEffect;

    public Building(string teamID, int ID, string type, Vector3 location,
    int maxHealth, int cost, int level)
    {
        this.teamID = teamID;
        this.ID = ID;
        this.type = type;
        this.location = location;
        this.maxHealth = maxHealth;
        this.cost = cost;
        this.level = level;
        this.time = 0f;
        this.loadingTime = 10f;
    }

    public virtual void InitTime()
    {
        time = 0f;
        loadingTime = 30/10f;
        //gameObject.GetComponent<MeshFilter>().mesh = progressMesh1;
        this.gameObject.GetComponent<PhotonView>().RPC("SetProgressMesh1", RpcTarget.AllBuffered);
    }

    public virtual void UpdateCreateBuildingTime(float update)//
    {
        float incrementPerSec = maxHealth / loadingTime;
        time = update;
        this.currentHealth += incrementPerSec * Time.deltaTime;
        this.progress = time/loadingTime*100;

        this.healthBar.value = (float)(currentHealth * 1.0 / maxHealth);
        this.progressBar.value = (float)this.progress / 100;
        UpdateMesh();
    }

    public virtual void UpdateMesh() //
    {
        if (time > loadingTime/2 && time < loadingTime) {
            //this.gameObject.GetComponent<MeshFilter>().mesh = progressMesh2;
            this.gameObject.GetComponent<PhotonView>().RPC("SetProgressMesh2", RpcTarget.AllBuffered);
        } else if (time > loadingTime)
        {
            //this.gameObject.GetComponent<MeshFilter>().mesh = completeMesh;
            this.gameObject.GetComponent<PhotonView>().RPC("SetCompleteMesh", RpcTarget.AllBuffered);
            gameObject.GetComponent<PhotonView>().RPC("GenerateCompleteEffect", RpcTarget.All);
        }
    }

    public virtual void InitOrderTime(float totalTime)//
    {
        this.state = State.InProgress;
        time = 0f;
        loadingTime = totalTime;
    }

    public virtual void UpdateOrderTime(float update)//
    {
        time = update;
        this.progress = time / loadingTime * 100;
        this.progressBar.value = (float)this.progress / 100;
    }

    [PunRPC]
    public virtual void SetProgressMesh1(){
        gameObject.GetComponent<MeshFilter>().mesh = progressMesh1;
    }

    [PunRPC]
    public virtual void SetProgressMesh2(){
        gameObject.GetComponent<MeshFilter>().mesh = progressMesh2;
    }
    [PunRPC]
    public virtual void SetCompleteMesh(){
        gameObject.GetComponent<MeshFilter>().mesh = completeMesh;
    }

    [PunRPC]
    public virtual void GenerateCompleteEffect(){
        completeEffect.SetActive(true);
    }

    [PunRPC]
    public virtual void ActiveLevelUpEffect(bool active = true){
        levelUpEffect.SetActive(active);
    }
}
