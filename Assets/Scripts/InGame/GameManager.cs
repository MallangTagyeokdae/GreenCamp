using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Doozy.Runtime.Common;
using Doozy.Runtime.UIManager.Containers;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public enum GameStates
{
    Loading = 0,
    InGame = 1,
    ConstructionMode = 2,
    SetTargetMode = 3,
    EndGame = 4
    
}
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
    public EffectHandler effectHandler;
    public GridHandler gridHandler;
    public GameObject target;
    public GameStates gameState = GameStates.Loading;
    public Dictionary<GameObject, CancellationTokenSource> tasks = new Dictionary<GameObject, CancellationTokenSource>();
    private Vector3[] _randomRot = {new Vector3(200,0,200), new Vector3(-200,0,200), new Vector3(200,0,-200), new Vector3(-200,0,-200)};
    //-----------------------------

    void Start()
    {
         _ = InitialGame();
    }

    // ================== 상태 관련 함수 ======================
    public async Task InitialGame()
    {
        List<Collider> startGrids = new();
        Vector3 startingPoint;

        SetState("Loading"); // 상태변경

        grid.SetActive(true); // Grid를 켜서 본진이 건설될 위치의 Grid 상태를 바꿀준비

        // 일단은 팀 종류에 따라서 위치를 고정했는데 나중에 난수 생성해서 받은 값으로 _randomRot, SetStartingPoint값에 넣어주면 코드 2줄로 줄일 수 있음
        if(GameStatus.instance.teamID == "Red")
        {
            target.transform.position = _randomRot[0];
            startGrids = gridHandler.SetStartingPoint(0);

        } else if(GameStatus.instance.teamID == "Blue")
        {
            target.transform.position = _randomRot[3];
            startGrids = gridHandler.SetStartingPoint(3);
        }

        // 본진이 지어질 위치 Grid의 평균값으로 계산
        startingPoint = gridHandler.CalculateGridScale(startGrids);
        grid.SetActive(false);

        // 본진 생성
        Building building = buildingController.CreateBuilding(startingPoint, buildingType, new Vector3(-90, 90, 0), gridHandler.constructionGrids);
        // 본진 초기값 세팅
        buildingController.initCommand(building);

        // 게임 시작 카운트다운 활성화
        await uIController.CountDown();

        SetState("InGame");
        currentUI.Show();
        target.SetActive(true);
    }
    public void SetState(string newState)
    {
        Enum.TryParse(newState, out GameStates state);
        switch(state)
        {
            case GameStates.Loading:
                gameState = state;
                break;
            case GameStates.InGame:
                gameState = state;
                break;
            case GameStates.ConstructionMode:
                gameState = state;
                break;
            case GameStates.EndGame:
                gameState = state;
                break;
        }
    }

    public bool CheckState(string checkState)
    {
        if(Enum.TryParse(checkState, out GameStates state))
        {
            return gameState == state ? true : false;
        }
        Debug.Log("CheckState함수에서 State 잘못 입력함");
        return false;
    }
    // ====================================================

    // ================== 클릭 관련 함수 ======================
    public void SetClickedObject(GameObject gameObject)
    {
        if(CheckState("InGame"))
        {
            unitController.SetActiveHealthBar(clickedObject);
            clickedObject.Clear();
            clickedObject.Add(gameObject);
            unitController.SetActiveHealthBar(gameObject, true);
        }
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
        if (clickedObject[0].TryGetComponent(out Building building) && clickedObject.Count == 1) buildingController.SetSponPos(newLocation,building);

        MoveUnit(newLocation);
    }
    // =====================================================


    //=================== UI 변경 관련 함수들 ===================
    public void SetBuildingListUI() // 건설할 건물 띄워주는 UI, Ground Inspector창에서 직접 넣어줌
    {
        currentUI = uIController.SetBuildingListUI(0);
    }
    public void SetBuildingInfo(int UIindex, Building building)
    {
        if (clickedObject[0] == building.gameObject && CheckState("InGame"))
        {
            currentUI = uIController.SetBuildingUI(UIindex, building);
        }
    }
    public void SetUnitInfo(int UIindex)
    { // unitDictionary에서 unitID에 해당하는 유닛을 가져옴
        if(gameState == GameStates.InGame && clickedObject[0].TryGetComponent(out Unit unit))
            currentUI = uIController.SetUnitUI(UIindex, unit);
    }
    public void SetHealthBar(Unit unit)
    {
        Debug.Log($"{unit.healthBar==null}");
        unit.healthBar.value = (float)(unit.unitCurrentHealth * 1.0 / unit.unitMaxHealth);
        // unit.healthBar.gameObject.SetActive(true);
    }
    // =====================================================


    // =================== 객체 생성 함수들 ===================
    public void CreateBuilding()
    {
        if(gridHandler.CheckCanBuilt() && CheckState("ConstructionMode")) // 건물이 생성가능한지 확인하는 조건문 나중에 자원, 건물인구수 체크하는것도 추가해야함
        // 건물생성가능여부를 판단하는 기능을 하는 함수를 만들어서 조건문에 넣도록 개선해야함
        {
            Vector3 buildingPos = gridHandler.CalculateGridScale();
            DelayBuildingCreation(buildingPos);
            grid.SetActive(false);
            SetState("InGame");
        }
    }
    public async void CreateUnit() // 해윤
    {
        GameObject targetOBJ = clickedObject[0];
        if (targetOBJ.TryGetComponent(out Barrack barrack))
        {
            if (barrack.state == Building.State.Built)
            {
                var cts = new CancellationTokenSource(); // 비동기 작업 취소를 위한 토큰 생성
                buildingController.SetBuildingState(barrack, Building.State.InProgress, unitType);
                ReloadBuildingUI(barrack);

                Vector3 buildingPos = barrack.transform.position; // 건물 위치 받음
                buildingPos = new Vector3(buildingPos.x, buildingPos.y, buildingPos.z - 5.5f); // 유닛이 생성되는 기본값

                tasks[targetOBJ] = cts; // 딕셔너리에 건물 오브젝트와 같이 토큰을 저장
                Unit createdUnit = await DelayUnitCreation(barrack, unitType, buildingPos, cts.Token); // 유닛 생성
                
                tasks.Remove(targetOBJ); // 유닛 생성이 완료되면 딕셔너리에서 제거해줌

                Vector3 destination = barrack._sponPos; // 유닛이 생성되고 이동할 포지션 받음

                // 유닛을 destination으로 이동명령 내리기
                GameObject unitObject = createdUnit.gameObject;

                buildingController.SetBuildingState(barrack, Building.State.Built, "None");
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
            var cts = new CancellationTokenSource(); // 비동기 작업 취소를 위한 토큰 생성

            buildingController.SetBuildingState(building, Building.State.InProgress, "LevelUP");
            building.GetComponent<PhotonView>().RPC("ActiveLevelUpEffect", RpcTarget.All, true);
            ReloadBuildingUI(building);

            tasks[building.gameObject] = cts; // 딕셔너리에 건물 오브젝트와 같이 토큰을 저장

            await OrderCreate(building, building.level * 10f, cts.Token);

            tasks.Remove(building.gameObject); // 레벨업이 완료되면 딕셔너리에서 제거해줌

            building.GetComponent<PhotonView>().RPC("ActiveLevelUpEffect", RpcTarget.All, false);
            buildingController.UpgradeBuilding(building);
            buildingController.SetBuildingState(building, Building.State.Built, "None");

            ReloadBuildingUI(building);
        }
    }
    private async Task DelayBuildingCreation(Vector3 buildingPos)
    {
        var cts = new CancellationTokenSource(); // 비동기 작업 취소를 위한 토큰 생성
        Debug.Log("함수 진입");

        // 건물 아래 Grid를 Builted로 변경
        gridHandler.SetGridsToBuilted();
        Debug.Log("Grid변경 완료");

        //AddComponent로 넣으면 inspector창에서 초기화한 값이 안들어가고 가장 초기의 값이 들어감. inspector 창으로 초기화를 하고 싶으면 script상 초기화 보다는 prefab을 건드리는게 나을듯
        Building building = buildingController.CreateBuilding(buildingPos, buildingType, new Vector3(-90, 90, 90), gridHandler.constructionGrids);
        Debug.Log("CreateBuilding함수 호출 완료");
        building.InitTime();

        Debug.Log("빌딩 객체 생성 완료");

        tasks[building.gameObject] = cts; // 딕셔너리에 건물 오브젝트와 같이 토큰을 저장
        await StartTimer(building.loadingTime, (float time) => UpdateBuildingHealth(building, time), cts.Token);

        tasks.Remove(building.gameObject); // 건물 생성이 완료되면 딕셔너리에서 제거해줌

        building.currentHealth = Mathf.FloorToInt(building.currentHealth); // 소수점 아래자리 버리기
        buildingController.SetBuildingState(building, Building.State.Built, "None");

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

    public void ReloadBuildingUI(Building building)
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
                Debug.Log(unit.order);
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
        int order = 3; //이거 이제 3번이 맞겠다
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
            /*if (unit.order == Unit.Order.Move || unit.order == Unit.Order.Offensive || unit.state == Unit.State.Attack)
            {
                return;
            }*/
            if (unit.state == Unit.State.Idle)
            {
                if (unit.unitBehaviour != null)
                {
                    StopCoroutine(unit.unitBehaviour);
                }
                unit.unitBehaviour = StartCoroutine(unitController.Move(ally, enemy, order));
            }
        }
    }

    //피격시 idle상태로 변환 후 해당 유닛에게 어택명령?
    public async Task<Unit> DelayUnitCreation(Barrack barrack, string unitType, Vector3 buildingPos, CancellationToken token)
    {
        switch (unitType)
        {
            case "Soldier":
                await OrderCreate(barrack, 2f, token);
                break;
            case "Archer":
                await OrderCreate(barrack, 2f, token);
                break;
            case "Tanker":
                await OrderCreate(barrack, 2f, token);
                break;
            case "Healer":
                await OrderCreate(barrack, 2f, token);
                break;
        }
        return unitController.CreateUnit(buildingPos, unitType);
    }

    private async Task OrderCreate(Building building, float totalTime, CancellationToken token)
    {
        building.InitOrderTime(totalTime);
        await StartTimer(totalTime, (float time) => UpdateBuildingProgress(building, time), token);
        /*PhotonView photonView = building.GetComponent<PhotonView>();
        await StartTimer(totalTime, (float time) =>
        {
            if (photonView.IsMine)      // IsMine Check
            {
                photonView?.RPC("UpdateBuildingProgress", RpcTarget.AllBuffered, building, time);
            }
        });*/
    }
    private void UpdateBuildingProgress(Building building, float time)
    {
        building.UpdateOrderTime(time);
        if (clickedObject[0].GetComponent<Building>() == building)
        {
            uIController.SetProgressBar(currentUI, building.progress / 100, 1);
        }
    }

    // =====================================================


    // =================== 타이머 함수 ======================== 
    private async Task StartTimer(float time, Action<float> action, CancellationToken token)
    {
        float start = 0f;
        while (time > start)
        {
            token.ThrowIfCancellationRequested();
            start += Time.deltaTime;
            action.Invoke(start);
            await Task.Yield();
        }
    }
    // =====================================================

    // =================== 키관련 함수 ======================== 
    public void PressedESC()
    {
        // ESC를 눌렀을 때 현재 선택된 오브젝트에 따라서 관리를 한다.
        GameObject targetObj = clickedObject[0];

        switch(gameState)
        {
            case GameStates.InGame:
                if(targetObj.TryGetComponent(out Building building))
                {
                    if(tasks.TryGetValue(building.gameObject, out var cts))
                    {
                        cts.Cancel();
                        cts.Dispose();
                        tasks.Remove(building.gameObject);
                        buildingController.CancelProgress(building);
                    }
                }
                break;
            case GameStates.ConstructionMode:
                SetState("InGame");
                grid.SetActive(false);
                SetClickedObject(ground);
                SetBuildingListUI();
                break;
        }

    }
    // ===================================================== 

}
