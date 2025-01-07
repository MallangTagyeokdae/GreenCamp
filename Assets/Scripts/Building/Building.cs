using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Building : MonoBehaviour
{
    public enum BuildingState
    {
        InProgress,  // 건물이 진행 중인 상태
        Built,   // 건물이 완료된 상태
        Destroy
    }

    public string teamID { get; set; }
    public int buildingID { get; set; }
    public string buildingType { get; set; }
    public Vector3 buildingLocation { get; set; }
    public int buildingMaxHealth { get; set; }
    public float buildingCurrentHealth { get; set; }
    public float buildingProgress { get; set; }
    public int buildingCost { get; set; }
    public int buildingLevel { get; set; }
    public Slider buildingHealthBar { get; set; }
    public Slider buildingProgressBar { get; set; }

    //------------조영리------------------
    public float time { get; set; }
    public float loadingTime{get; set;}
    public BuildingState buildingState = BuildingState.InProgress;
    public Mesh progressMesh1;
    public Mesh progressMesh2;
    public Mesh completeMesh;
    //------------------------------------------

    public Building(string teamID, int buildingID, string buildingType, Vector3 buildingLocation,
    int buildingMaxHealth, int buildingCost, int buildingLevel)
    {
        this.teamID = teamID;
        this.buildingID = buildingID;
        this.buildingType = buildingType;
        this.buildingLocation = buildingLocation;
        this.buildingMaxHealth = buildingMaxHealth;
        this.buildingCost = buildingCost;
        this.buildingLevel = buildingLevel;
        this.time = 0f;
        this.loadingTime = 10f;
    }

    //------------조영리------------------
    public virtual void InitTime(){
    }

    public virtual void UpdateTime(float update){
    }

    public virtual void UpdateMesh(){
    }
    //------------------------------------------

}
