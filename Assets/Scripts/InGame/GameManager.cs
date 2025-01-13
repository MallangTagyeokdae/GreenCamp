using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Doozy.Runtime.UIManager.Containers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                return _instance;
            }

            return _instance;
        }
    }
    public List<GameObject> gameStates;

    //------------ 해윤 ------------
    public UIController uIController;
    public UnitController unitController;
    public string unitType;
    //-----------------------------

    //------------ 준현 ------------
    public BuildingController buildingController; // 준현
    public string buildingType;
    public GameObject grid;
    public GameObject ground;
    public UIContainer currentUI; // 현재 보이고 있는 UI를 갖고 있음
    public List<GameObject> clickedObject; // 현재 선택된 게임 Object
    //-----------------------------

    //------------ 영리 ------------ 일단 대충
    public GameObject sponeffect;
    //-----------------------------

    void Start()
    {
        currentUI.Show();
    }

    // ================== 클릭 관련 함수 ======================
    public void SetClickedObject(GameObject gameObject)
    {
        unitController.SetActiveHealthBar(clickedObject);
        clickedObject.Clear();
        clickedObject.Add(gameObject);
        unitController.SetActiveHealthBar(gameObject, true);
    }

    public void AddClickedObject(GameObject gameObject)
    {
        if (!clickedObject.Contains(gameObject))
        {
            clickedObject.Add(gameObject);
            unitController.SetActiveHealthBar(gameObject, true);
        }
    }
    public void GroundEvent(Vector3 newLocation)
    {
        if (clickedObject[0].name.Contains("Barrack") && clickedObject.Count == 1) SetSponPos(newLocation);

        MoveUnit(newLocation);
    }
    // =====================================================


    //=================== UI 변경 관련 함수들 ===================
    public void SetBuildingListUI(int UIindex)
    {
        currentUI = uIController.SetBuildingListUI(UIindex);
    }
    public void SetBuildingInfo(int UIindex, Building building)
    {
        if (clickedObject[0] == building.gameObject)
        {
            currentUI = uIController.SetBuildingUI(UIindex, building);
        }
    }
    public void SetUnitInfo(int UIindex)
    { // unitDictionary에서 unitID에 해당하는 유닛을 가져옴
        currentUI = uIController.SetUnitUI(UIindex);
    }
    public void SetHealthBar(Unit unit)
    {
        Debug.Log("SetHealthBar 함수 실행 여부 확인");
        unit.healthBar.value = (float)(unit.unitCurrentHealth * 1.0 / unit.unitMaxHealth);
        // unit.healthBar.gameObject.SetActive(true);
    }
    // =====================================================


    // =================== 객체 생성 함수들 ===================
    public void CreateBuilding(Vector3 buildingPos)
    {
        DelayBuildingCreation(buildingPos);
        grid.SetActive(false);
    }
    public async void CreateUnit() // 해윤
    {
        if (clickedObject[0].TryGetComponent(out Barrack barrack))
        {
            if (barrack.state == Building.State.Built)
            {
                buildingController.SetBuildingState(barrack, 2, unitType);
                ReloadBuildingUI(barrack);

                Vector3 buildingPos = barrack.transform.position;
                Vector3 destination = barrack._sponPos;
                buildingPos = new Vector3(buildingPos.x, buildingPos.y, buildingPos.z - 4f);
                Unit createdUnit = await DelayUnitCreation(barrack, unitType, buildingPos);

                // 유닛을 destination으로 이동명령 내리기
                GameObject unitObject = createdUnit.gameObject;

                buildingController.SetBuildingState(barrack, 1, "None");
                createdUnit.unitBehaviour = StartCoroutine(unitController.Move(unitObject, destination, 1));

                ReloadBuildingUI(barrack);
            }
        }
    }
    // =====================================================


    // =================== 건물 관리 함수들 ===================
    public void SetBuildingType(string buildingType)
    {
        this.buildingType = buildingType;
    }
    public async void LevelUpBuilding()
    {
        if (clickedObject[0].TryGetComponent(out Building building))
        {
            buildingController.SetBuildingState(building, 2, "LevelUP");
            ReloadBuildingUI(building);
            await OrderCreate(building, building.level * 10f);
            buildingController.UpgradeBuilding(building);
            buildingController.SetBuildingState(building, 1, "None");
            ReloadBuildingUI(building);
        }
    }
    private async Task DelayBuildingCreation(Vector3 buildingPos)
    {
        //AddComponent로 넣으면 inspector창에서 초기화한 값이 안들어가고 가장 초기의 값이 들어감. inspector 창으로 초기화를 하고 싶으면 script상 초기화 보다는 prefab을 건드리는게 나을듯
        Building building = buildingController.CreateBuilding(buildingPos, buildingType);
        building.InitTime();
        await StartTimer(building.loadingTime, (float time) => UpdateBuildingHealth(building, time));

        //effect 동작만 되도록 막 넣음
        GameObject effect = Instantiate(sponeffect, building.gameObject.transform);
        effect.transform.localPosition = Vector3.zero;
        effect.transform.localScale = new Vector3(4, 4, 4);
        effect.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        //----------------------------

        //Destroy-----------------
        Destroy(effect, effect.GetComponent<ParticleSystem>().main.startLifetime.constant);
        //------------------------
        Debug.Log($"check time: {building.time}");
        building.currentHealth = Mathf.FloorToInt(building.currentHealth); // 소수점 아래자리 버리기
        buildingController.SetBuildingState(building, 1, "None");
        ReloadBuildingUI(building);
    }

    private void UpdateBuildingHealth(Building building, float time)
    { // 건물이 생성될 때 체력을 업데이트 해주는 함수
        building.UpdateCreateBuildingTime(time);
        if (clickedObject[0].GetComponent<Building>() == building)
        {
            uIController.UpdateHealth(currentUI, building);
        }
    }

    private void ReloadBuildingUI(Building building)
    { // 건물이 생성완료 됐을 때 건물을 클릭하고 있으면 건물 UI로 바꿔준다.
        if (clickedObject[0].name == building.name)
        {
            switch (building.type)
            {
                case "Command":
                    SetBuildingInfo(2, building);
                    break;
                case "Barrack":
                    SetBuildingInfo(3, building);
                    break;
                case "PopulationBuilding":
                    SetBuildingInfo(4, building);
                    break;
                case "ResourceBuilding":
                    SetBuildingInfo(5, building);
                    break;
                case "Defender":
                    SetBuildingInfo(6, building);
                    break;
            }
        }
    }

    private void SetSponPos(Vector3 newLocation)
    {
        clickedObject[0].GetComponent<Barrack>().SetSponPos(newLocation);
    }

    // =====================================================


    // =================== 유닛 관리 함수들 =================== 
    public void SetUnitType(string unitType)
    {
        this.unitType = unitType;
    }


    public void MoveUnit(Vector3 newLocation)
    {
        int order = 1;

        if (Input.GetKey(KeyCode.A))
        {
            order = 2;
        }

        foreach (GameObject go in clickedObject)
        {
            go.TryGetComponent(out Unit selectedUnit);

            if (selectedUnit == null)
            {
                continue;
            }

            if (selectedUnit.unitBehaviour != null)
            {
                StopCoroutine(selectedUnit.unitBehaviour);
            }
            selectedUnit.unitBehaviour = StartCoroutine(unitController.Move(go, newLocation, order));
        }
    }

    public void AttackUnit(GameObject ally, GameObject enemy)
    {
        Unit unit = ally.GetComponent<Unit>();
        enemy.TryGetComponent(out Entity enemyEntity);
        if (enemyEntity == null || unit.teamID == enemyEntity.teamID)
        {
            return;
        }
        else
        {
            if (!unit.attackList.Contains(enemy))
            {
                unit.attackList.Add(enemy);
            }
            if (unit.order == Unit.Order.Move || unit.state == Unit.State.Attack)
            {
                return;
            }

            if (unit.unitBehaviour != null)
            {
                StopCoroutine(unit.unitBehaviour);
            }
            unit.unitBehaviour = StartCoroutine(unitController.Attack(ally, enemy));
        }
    }

    public void Aggregated(GameObject ally, GameObject enemy)
    {
        /*
            1. 어그로가 끌렸을 경우 offensive명령으로 해당 유닛을 향해 move
            2. 알아서 공격범위에 들어오면 attackunit이 실행됨.
            3. 근데 만약 어그로가 끌린 대상이 있는데 해당 대상이 아닌 다른 유닛이 공격범위에 들어오면?

        */
        Unit unit = ally.GetComponent<Unit>();
        enemy.TryGetComponent(out Entity enemyEntity);
        if (enemyEntity == null || unit.teamID == enemyEntity.teamID)
        {
            return;
        }
        else
        {
            if (!unit.aggList.Contains(enemy))
            {
                unit.aggList.Add(enemy);
            }
            if (unit.order == Unit.Order.Move || unit.state == Unit.State.Attack)
            {
                return;
            }

            if (unit.unitBehaviour != null)
            {
                StopCoroutine(unit.unitBehaviour);
            }
            unit.unitBehaviour = StartCoroutine(unitController.Move(ally, enemy));
        }
    }

    public async Task<Unit> DelayUnitCreation(Barrack barrack, string unitType, Vector3 buildingPos)
    {
        switch (unitType)
        {
            case "Soldier":
                await OrderCreate(barrack, 2f);
                break;
            case "Archer":
                await OrderCreate(barrack, 2f);
                break;
            case "Tanker":
                await OrderCreate(barrack, 2f);
                break;
            case "Healer":
                await OrderCreate(barrack, 2f);
                break;
        }
        return unitController.CreateUnit(buildingPos, unitType);
    }

    private async Task OrderCreate(Building building, float totalTime)
    {
        building.InitOrderTime(totalTime);
        await StartTimer(totalTime, (float time) => UpdateBuildingProgress(building, time));
    }
    private void UpdateBuildingProgress(Building building, float time)
    {
        building.UpdateOrderTime(time);
        if (clickedObject[0].GetComponent<Barrack>() == building)
        {
            uIController.SetProgressBar(currentUI, building.progress / 100, 1);
        }
    }

    // =====================================================


    // =================== 타이머 함수 =================== 
    private async Task StartTimer(float time, Action<float> action)
    {
        float start = 0f;
        while (time > start)
        {
            start += Time.deltaTime;
            action.Invoke(start);
            await Task.Yield();
        }
    }
    // =====================================================
}
