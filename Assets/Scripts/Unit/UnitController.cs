using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon.StructWrapping;
using FischlWorks_FogWar;
using Photon.Pun;
using TMPro;
using Unity.VisualScripting;
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
    public GameObject enemyUnits; //적 유닛이 hierarchy 창에서 생성될 위치
    public GameObject myUnits; //아군 유닛이 hierarchy 창에서 생성될 위치
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
        GameObject unitObj = unitObject;


        // switch문으로 unit을 Dictionary에 저장
        switch (unitType)
        {
            case "Archer":
                Archer newArcher = unitObject.GetComponent<Archer>();
                newArcher.Init(_teamID, _unitID, unitLocation);
                unitDictionary.Add(_unitID, newArcher);
                _createdUnit = newArcher;
                break;
            case "Soldier":
                Soldier newSoldier = unitObject.GetComponent<Soldier>();
                newSoldier.Init(_teamID, _unitID, unitLocation);
                unitDictionary.Add(_unitID, newSoldier);
                _createdUnit = newSoldier;
                break;
            case "Tanker":
                Tanker newTanker = unitObject.GetComponent<Tanker>();
                newTanker.Init(_teamID, _unitID, unitLocation);
                unitDictionary.Add(_unitID, newTanker);
                _createdUnit = newTanker;
                break;
            case "Healer":
                Healer newHealer = unitObject.GetComponent<Healer>();
                newHealer.Init(_teamID, _unitID, unitLocation);
                unitDictionary.Add(_unitID, newHealer);
                _createdUnit = newHealer;
                break;
            case "Scout":
                Scout newScout = unitObject.GetComponent<Scout>();
                newScout.Init(_teamID, _unitID, unitLocation);
                unitDictionary.Add(_unitID, newScout);
                _createdUnit = newScout;
                break;
            default:
                Debug.Log("unitType을 찾을 수 없습니다");
                _unitID--;
                break;
        }

        Unit unit = unitObj.GetComponent<Unit>();
        GameManager.instance.SetHealthBar(unit);

        unitObj.GetComponent<ClickEventHandler>().leftClickDownEvent.AddListener((Vector3 pos) =>
        {
            GameManager.instance.SetClickedObject(unitObj);
        }
        );
        unitObj.GetComponent<ClickEventHandler>().draggedEvent.AddListener(() => GameManager.instance.AddClickedObject(unitObj));

        // 힐러만 콜백함수로 힐하는 함수 넣음
        if (unit.GetComponent<Healer>())
        {
            unit.SetAttEnter((GameObject ally) =>
            {
                if (ally.TryGetComponent(out Unit allyUnit) && allyUnit.teamID == unit.teamID)
                {
                    unit.GetComponent<Healer>().attackList.Add(ally);
                }
            });
            // unit.SetAttEnter((GameObject ally) => { GameManager.instance.Heal(unit.gameObject); });
        }
        else
        {
            unit.SetAttEnter((GameObject enemy) => { GameManager.instance.AttackUnit(unit.gameObject, enemy); });

        }
        unit.SetAttExit((GameObject enemy) => { unit.attackList.Remove(enemy); });
        unit.SetAggEnter((GameObject enemy) => { GameManager.instance.Aggregated(unit.gameObject, enemy); });
        unit.SetAggExit((GameObject enemy) => { unit.aggList.Remove(enemy); });

        /*if(_teamID == unitObject.GetComponent<Unit>().teamID){
            unitObj.transform.SetParent(myUnits.transform);
        }
        else{
            unitObj.transform.SetParent(enemyUnits.transform);
        }*/
        _unitID++;

        if (_createdUnit.GetComponent<csFogVisibilityAgent>() != null)
        {
            Destroy(_createdUnit.GetComponent<csFogVisibilityAgent>());
        }

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
        {
            unit.healthBar.value = (float)(unit.currentHealth * 1.0 / unit.maxHealth);
            unit.healthBar.gameObject.SetActive(activeStatus);
        }
    }
    public void unitAttacked(int unitID, int damage)
    {
        Unit selectedUnit = unitDictionary[unitID];
    }
    public void DestroyUnit(Unit unit)
    {
        // if (unitDictionary.ContainsKey(unitID)) unitDictionary.Remove(unitID); // 유닛 딕셔너리에서 유닛 삭제

        GameManager.instance.fogWar.GetComponent<csFogWar>().RemoveFogRevealer(unit.gameObject);

        unit.DestroyEntity();
        // GameStatus.instance.currentUnitCount -= unit.populationCost;
    }

    // =================== 유닛 행동 함수 ===================
    public IEnumerator Move(GameObject unitObject, Vector3 newLocation, int order)
    {
        Unit unit = unitObject.GetComponent<Unit>();
        unit.SetOrder(order); //유닛에 대한 사용자의 명령이 Move (0: Idle, 1: Move, 2: Offensive, 3: Attack)
        Vector3 moveDirection = (newLocation - unit.transform.position).normalized;

        if (moveDirection != Vector3.zero)
        {
            unit.transform.rotation = Quaternion.LookRotation(moveDirection);
        }

        unit.ChangeState("Move");

        while (Vector3.Distance(unitObject.transform.position, newLocation) > 0.01f)
        {
            unit.transform.position = Vector3.MoveTowards(
                unit.transform.position,
                newLocation,
                unit.unitMoveSpeed * Time.deltaTime
            );
            yield return null;
        }

        unit.transform.position = newLocation;
        unit.destination = Vector3.zero;
        //Debug.Log("지정된 위치로 이동 끝남");
        unit.SetOrder(0);
        unit.ChangeState("Idle");
    }

    public IEnumerator Move(GameObject unitObject, GameObject enemy, int order)
    {
        Unit unit = unitObject.GetComponent<Unit>();
        unit.SetOrder(order); //유닛에 대한 사용자의 명령이 Move (0: Idle, 1: Move, 2: Offensive, 3: Attack)
        unit.ChangeState("Aggregated");

        while (enemy != null)
        {
            if (enemy == null || enemy.GetComponent<Entity>() == null || enemy.GetComponent<Entity>().currentHealth <= 0 || enemy.tag == "Untagged")
            {
                unit.target = null;
                unit.attackList.Remove(enemy);
                unit.aggList.Remove(enemy);
                break;
            }

            Vector3 moveDirection = (enemy.transform.position - unit.transform.position).normalized;

            if (moveDirection != Vector3.zero)
            {
                unit.transform.rotation = Quaternion.LookRotation(moveDirection);
            }

            unit.transform.position = Vector3.MoveTowards(
                unit.transform.position,
                enemy.transform.position,
                unit.unitMoveSpeed * Time.deltaTime
            );

            yield return null;
        }

        //Debug.Log("적 유닛으로 이동 끝남");
        unit.SetOrder(0);
        unit.ChangeState("Idle");
    }

    // damage 넣는 로직 추가해야함, 적팀일 때 공격도
    public IEnumerator Attack(GameObject ally, GameObject enemy)
    {
        ally.TryGetComponent(out Unit unit);
        unit.ChangeState("Attack");
        unit.target = enemy;

        while (unit.attackList.Contains(enemy)) //적이 죽을 때까지 실행 -> 적이 죽지 않고 공격 범위 밖으로 나가면 triggerexit으로 move로 전환 <-> move와 chase?
        {
            if (enemy == null || enemy.GetComponent<Entity>() == null || enemy.GetComponent<Entity>().currentHealth <= 0 || enemy.tag == "Untagged")
            {
                unit.target = null;
                unit.attackList.Remove(enemy);
                unit.aggList.Remove(enemy);
                break;
            }

            Vector3 rot = (enemy.transform.position - ally.transform.position).normalized;
            if (!unit.isTurretUnit)
            {
                ally.transform.rotation = Quaternion.LookRotation(rot);
            }
            else
            {
                rot.y = 0;
                ally.transform.rotation = Quaternion.LookRotation(rot);
            }
            yield return null;
        }

        /*if (unit.order.Equals(Unit.Order.Offensive))
        {
            Debug.Log($"{unit.name} 얘 State : {unit.order}");
            GameManager.instance.Attang(unit.destination, 2, unit.gameObject);
        }
        else
        {*/
            //Debug.Log($"else에서 {unit.name} 얘 State : {unit.order}");
        unit.SetOrder(0);
        unit.ChangeState("Idle");
        //}
    }
    public void Heal(GameObject me)
    {
        me.TryGetComponent(out Unit unit);
        foreach (GameObject ally in unit.attackList.ToList())
        {
            ally.TryGetComponent(out Entity allyEntity);
            if (ally == null || allyEntity == null)
            {
                unit.attackList.Remove(ally);
            }
            if (allyEntity.currentHealth <= 0 || allyEntity.currentHealth >= allyEntity.maxHealth)
            {
                continue;
            }
            unit.ChangeState("Attack");
            break;
        }
    }

    public void ApplyUnitUpgrade(string type, int degree)
    {
        foreach (KeyValuePair<int, Unit> valuePair in unitDictionary)
        {
            switch (type)
            {
                case "Damage":
                    valuePair.Value.unitPower += degree;
                    break;
                case "Armor":
                    valuePair.Value.armor += degree;
                    break;
                case "Health":
                    valuePair.Value.GetComponent<PhotonView>().RPC("SyncUnitHealth", RpcTarget.All, degree * 1.0f);

                    // float healthPercent = (float)(valuePair.Value.currentHealth / valuePair.Value.maxHealth);
                    // valuePair.Value.maxHealth += degree;
                    // valuePair.Value.currentHealth = Mathf.FloorToInt(valuePair.Value.maxHealth * healthPercent);  
                    break;
            }
        }
    }

    public void RemoveCoroutine()
    {
        foreach(KeyValuePair<int, Unit> valuePair in unitDictionary)
        {
            if(valuePair.Value.unitBehaviour != null)
            {
                StopCoroutine(valuePair.Value.unitBehaviour);
            }
        }
    }
}
