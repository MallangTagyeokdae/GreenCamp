using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building
{

    public string teamID { get; set; }
    public int buildingID { get; set; }
    public string buildingType { get; set; }
    public Vector3 buildingLocation { get; set; }
    public int buildingHealth { get; set; }
    public int buildingCost { get; set; }
    public int buildingLevel { get; set; }
    
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

}
