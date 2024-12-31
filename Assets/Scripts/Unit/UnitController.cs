using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public List<Unit> unitList = new List<Unit>();
    // public GameObject unitPref;
    private Dictionary<string, GameObject> unitPrefabs = new Dictionary<string, GameObject>();
    // private int _unitCount;
    private string TeamID;

    public void Start()
    {
        // _unitCount = 0;
        TeamID = GameStatus.instance.teamID;
        Debug.Log(TeamID);
        List<GameObject> unitPrefabsList = new List<GameObject>(Resources.LoadAll<GameObject>($"Prefabs/Units/{TeamID}TeamUnits"));
        foreach (GameObject unit in unitPrefabsList)
        {
            if (!unitPrefabs.ContainsKey(unit.name))
            {
                unitPrefabs.Add(unit.name, unit);
            }
        }
        foreach (var kvp in unitPrefabs)
        {
            Debug.Log($"Key: {kvp.Key}, Value: {kvp.Value.name}");
        }
    }
    public void createUnit(Vector3 unitLocation, string unitType)
    {
        GameObject unitInstance = Instantiate(unitPrefabs[unitType], unitLocation, Quaternion.identity);

        switch (unitType)
        {
            case "Archer":
                Archer newArcher = new(TeamID, 0, unitLocation);
                unitList.Add(newArcher);
                break;
            case "Soldier":
                Soldier newSoldier = new(TeamID, 0, unitLocation);
                unitList.Add(newSoldier);
                break;
            case "Tanker":
                Tanker newTanker = new(TeamID, 0, unitLocation);
                unitList.Add(newTanker);
                break;
            case "Healer":
                Healer newHealer = new(TeamID, 0, unitLocation);
                unitList.Add(newHealer);
                break;
            default:
                Debug.Log("unitType을 찾을 수 없습니다");
                break;
        }
    }
    public void unitAttacked(int unitID, int damage)
    {
        Unit selectedUnit = unitList[unitID];
    }

    public void destroyUnit(int unitID)
    {

    }
    public void moveUnit(int unitID)
    {

    }
}
