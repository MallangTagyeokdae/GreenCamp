using System;
using System.Collections;
using System.Collections.Generic;
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

    public string teamID { get; set; }
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

    public virtual void InitTime(){
    }

    public virtual void UpdateCreateBuildingTime(float update){
    }

    public virtual void UpdateMesh(){
    }
    public virtual void InitOrderTime(float totalTime){} // 건물 생성 외 다른 명령을 내릴 떄 타이머 초기화
    public virtual void UpdateOrderTime(float update){} // 건물 생성 외 다른 명령을 내릴 떄 타이머 업데이트

}
