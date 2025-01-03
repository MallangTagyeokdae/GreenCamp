using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public int buildingHealth { get; set; }
    public int buildingCost { get; set; }
    public int buildingLevel { get; set; }

    //------------조영리------------------
    public float time { get; set; }
    public BuildingState buildingState = BuildingState.InProgress;
    //------------------------------------------

    public Building(string teamID, int buildingID, string buildingType, Vector3 buildingLocation,
    int buildingHealth, int buildingCost, int buildingLevel)
    {
        this.teamID = teamID;
        this.buildingID = buildingID;
        this.buildingType = buildingType;
        this.buildingLocation = buildingLocation;
        this.buildingHealth = buildingHealth;
        this.buildingCost = buildingCost;
        this.buildingLevel = buildingLevel;
    }

    //------------조영리------------------
    public void InitTime(){
        this.time = 0f;
    }

    public void UpdateTime(float update){
        this.time += update;
    }
    //------------------------------------------

}
