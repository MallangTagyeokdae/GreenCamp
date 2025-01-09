using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UIElements;

public class UnitController : MonoBehaviour
{
    public Dictionary<int, Unit> unitDictionary = new Dictionary<int, Unit>(); // unitList 초기화

    // public GameObject unitPref; -> unity에서 직접 끌어 쓸 때 사용

    // 본인 팀의 unitPrefab들을 unit 직업을 key값으로 받는 Dictionary로 저장
    private Dictionary<string, GameObject> unitPrefabs = new Dictionary<string, GameObject>();

    // private int _unitCount; -> GameStatus에서 관리하기로 함
    private string _teamID; // 본인 TeamID
    private int _unitID;
    private Unit _createdUnit;
    public GameObject unitObject;
    private void Start()
    {
        _teamID = GameStatus.instance.teamID; // 본인 TeamID를 GameStatus에서 instance로 받아옴.
        _unitID = 0; // unit 고유 id 초기화

        // 폴더 경로에 있는 모든 GameObject를 Load해 unitPrefabsList에 임시로 저장
        List<GameObject> unitPrefabsList = new List<GameObject>(Resources.LoadAll<GameObject>($"Prefabs/Units/{_teamID}TeamUnits"));

        // // foreach문으로 List 안에 있는 unit 객체들을 다시 Dictionary로 저장
        foreach (GameObject unit in unitPrefabsList)
        {
            if (!unitPrefabs.ContainsKey(unit.name))
            {
                unitPrefabs.Add(unit.name, unit);
            }
        }
    }

    public Unit CreateUnit(Vector3 unitLocation, string unitType) // 유닛 생성 함수
    {
        unitObject = PhotonNetwork.Instantiate($"Prefabs/Units/{_teamID}TeamUnits/{unitType}", unitLocation, Quaternion.Euler(new Vector3(0, 180, 0)));
        unitObject.name = unitType + _unitID.ToString();
        GameObject gameObject = unitObject;
        

        // switch문으로 unit을 Dictionary에 저장
        switch (unitType)
        {
            case "Archer":
                Archer newArcher = unitObject.AddComponent<Archer>();
                newArcher.Init(_teamID, _unitID, unitLocation);
                unitDictionary.Add(_unitID, newArcher);
                _createdUnit = newArcher;
                break;
            case "Soldier":
                Soldier newSoldier = unitObject.AddComponent<Soldier>();
                newSoldier.Init(_teamID, _unitID, unitLocation);
                unitDictionary.Add(_unitID, newSoldier);
                _createdUnit = newSoldier;
                break;
            case "Tanker":
                Tanker newTanker = unitObject.AddComponent<Tanker>();
                newTanker.Init(_teamID, _unitID, unitLocation);
                unitDictionary.Add(_unitID, newTanker);
                _createdUnit = newTanker;
                break;
            case "Healer":
                Healer newHealer = unitObject.AddComponent<Healer>();
                newHealer.Init(_teamID, _unitID, unitLocation);
                unitDictionary.Add(_unitID, newHealer);
                _createdUnit = newHealer;
                break;
            default:
                Debug.Log("unitType을 찾을 수 없습니다");
                _unitID--;
                break;
        }
        unitObject.GetComponent<ClickEventHandler>().leftClickDownEvent.AddListener((Vector3 pos) =>
        {
            GameManager.instance.SetClickedObject(gameObject);
            GameManager.instance.SetUnitInfo(7);
            Unit unit = gameObject.GetComponent<Unit>();
            GameManager.instance.SetHealthBar(unit);
        }
        );
        unitObject.GetComponent<ClickEventHandler>().draggedEvent.AddListener( ()=> GameManager.instance.AddClickedObject(gameObject));
        Unit unit = gameObject.GetComponent<Unit>();
        unit.SetAttCollision((GameObject go) => Debug.Log($"coll check: {go.name}"));
        _unitID++;
        return _createdUnit;

    }
    public void SetActiveHealthBar(List<GameObject> clickedObject, bool activeStatus = false)
    {
        foreach (GameObject go in clickedObject)
        {
            SetActiveHealthBar(go, activeStatus);
        }
    }
    public void SetActiveHealthBar(GameObject go, bool activeStatus = false)
    {
        if (go.TryGetComponent(out Unit unit))
            unit.healthBar.gameObject.SetActive(activeStatus);
    }
    public void unitAttacked(int unitID, int damage)
    {
        Unit selectedUnit = unitDictionary[unitID];
    }
    public void destroyUnit(int unitID)
    {

    }
    public IEnumerator MoveUnit(GameObject unitObject, Vector3 newLocation, int order)
    {
        Unit unit = unitObject.GetComponent<Unit>();
        unit.SetOrder(order); //유닛에 대한 사용자의 명령이 Move (0: Idle, 1: Move, 2: Offensive, 3: Attack)

        while (Vector3.Distance(unitObject.transform.position, newLocation) > 0.01f)
        {
            unit.transform.position = Vector3.MoveTowards(
                unit.transform.position,
                newLocation,
                unit.unitMoveSpeed * Time.deltaTime
            );

            Vector3 moveDirection = (newLocation - unit.transform.position).normalized;

            if (moveDirection != Vector3.zero)
            {
                unit.transform.rotation = Quaternion.LookRotation(moveDirection);
            }

            unit.unitAnimator.SetBool("isWalking", true);

            yield return null;
        }
        unit.transform.position = newLocation;
        unit.SetOrder(0);
        unit.unitAnimator.SetBool("isWalking", false);
    }

    /*public IEnumerator Attack(GameObject ally, GameObject enemy){

    }*/
}
