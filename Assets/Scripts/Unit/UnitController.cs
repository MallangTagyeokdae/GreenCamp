using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public List<Unit> unitList = new List<Unit>(); // unitList 초기화

    // public GameObject unitPref; -> unity에서 직접 끌어 쓸 때 사용

    // 본인 팀의 unitPrefab들을 unit 직업을 key값으로 받는 Dictionary로 저장
    private Dictionary<string, GameObject> unitPrefabs = new Dictionary<string, GameObject>(); 

    // private int _unitCount; -> GameStatus에서 관리하기로 함
    private string TeamID; // 본인 TeamID

    public void Start()
    {
        // _unitCount = 0;
        TeamID = GameStatus.instance.teamID; // 본인 TeamID를 GameStatus에서 instance로 받아옴.
        Debug.Log(TeamID); 

        // 폴더 경로에 있는 모든 GameObject를 Load해 unitPrefabsList에 임시로 저장
        List<GameObject> unitPrefabsList = new List<GameObject>(Resources.LoadAll<GameObject>($"Prefabs/Units/{TeamID}TeamUnits"));

        // foreach문으로 List 안에 있는 unit 객체들을 다시 Dictionary로 저장
        foreach (GameObject unit in unitPrefabsList)
        {
            if (!unitPrefabs.ContainsKey(unit.name))
            {
                unitPrefabs.Add(unit.name, unit);
            }
        }

        // foreach (var kvp in unitPrefabs)
        // {
        //     Debug.Log($"Key: {kvp.Key}, Value: {kvp.Value.name}");
        // }
    }
    public void createUnit(Vector3 unitLocation, string unitType) // 유닛 생성 함수
    {
        GameObject unitInstance = Instantiate(unitPrefabs[unitType], unitLocation, Quaternion.identity); // unit instance 생성

        // switch문으로 unit을 List에 저장 -> Dictionary로 바꿀 예정
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
