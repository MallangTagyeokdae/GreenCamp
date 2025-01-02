using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public Dictionary<int, Unit> unitDictionary = new Dictionary<int, Unit>(); // unitList 초기화

    // public GameObject unitPref; -> unity에서 직접 끌어 쓸 때 사용

    // 본인 팀의 unitPrefab들을 unit 직업을 key값으로 받는 Dictionary로 저장
    private Dictionary<string, GameObject> unitPrefabs = new Dictionary<string, GameObject>();

    // private int _unitCount; -> GameStatus에서 관리하기로 함
    private string _teamID; // 본인 TeamID
    private int _unitID;

    public void Start()
    {
        // _unitCount = 0;
        _teamID = GameStatus.instance.teamID; // 본인 TeamID를 GameStatus에서 instance로 받아옴.
        Debug.Log(_teamID);

        // unit 고유 id 초기화
        _unitID = 0;

        // 폴더 경로에 있는 모든 GameObject를 Load해 unitPrefabsList에 임시로 저장
        List<GameObject> unitPrefabsList = new List<GameObject>(Resources.LoadAll<GameObject>($"Prefabs/Units/{_teamID}TeamUnits"));

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
        int getCurrentUnitID = _unitID;
        GameObject unitInstance = Instantiate(unitPrefabs[unitType], unitLocation, Quaternion.identity); // unit instance 생성


        // switch문으로 unit을 Dictionary에 저장
        switch (unitType)
        {
            case "Archer":
                Archer newArcher = new(_teamID, _unitID, unitLocation);
                unitDictionary.Add(_unitID, newArcher);
                break;
            case "Soldier":
                Soldier newSoldier = new(_teamID, _unitID, unitLocation);
                unitDictionary.Add(_unitID, newSoldier);
                break;
            case "Tanker":
                Tanker newTanker = new(_teamID, _unitID, unitLocation);
                unitDictionary.Add(_unitID, newTanker);
                break;
            case "Healer":
                Healer newHealer = new(_teamID, _unitID, unitLocation);
                unitDictionary.Add(_unitID, newHealer);
                break;
            default:
                Debug.Log("unitType을 찾을 수 없습니다");
                _unitID--;
                break;
        }

        foreach (var kvp in unitDictionary.Keys)
        {
            Debug.Log($"Key: {kvp}");
        }
        unitInstance.GetComponent<ClickEventHandler>().leftClickDownEvent.AddListener((Vector3 pos) =>
        {
            GameManager.instance.SetUnitInfo(getCurrentUnitID);
            GameManager.instance.ChangeUI(3);
        });
        _unitID++;
    }
    public void unitAttacked(int unitID, int damage)
    {
        Unit selectedUnit = unitDictionary[unitID];
    }

    public void destroyUnit(int unitID)
    {

    }
    public void moveUnit(int unitID)
    {

    }
}
