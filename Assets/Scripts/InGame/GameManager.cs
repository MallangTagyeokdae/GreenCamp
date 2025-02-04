using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Doozy.Runtime.UIManager.Containers;
using Photon.Pun;
using UnityEngine;

public enum GameStates
{
    Loading = 0,
    InGame = 1,
    ConstructionMode = 2,
    SetTargetMode = 3,
    SetMoveRot = 4,
    EndGame = 5

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
    public GameObject clickEffect;
    public UIContainer currentUI; // 현재 보이고 있는 UI를 갖고 있음
    public List<GameObject> clickedObject; // 현재 선택된 게임 Object
    public GameObject targetObject;
    public EffectHandler effectHandler;
    public GridHandler gridHandler;
    public GameObject target;
    public GameStates gameState = GameStates.Loading;
    public Dictionary<GameObject, CancellationTokenSource> tasks = new Dictionary<GameObject, CancellationTokenSource>();
    private Vector3[] _randomRot = { new Vector3(200, 0, 200), new Vector3(-200, 0, 200), new Vector3(200, 0, -200), new Vector3(-200, 0, -200) };
    //-----------------------------
    private int _commandLevel = 1;
    private Coroutine masterTimer;

    void Start()
    {
        _ = InitialGame();
    }

    // ================== 상태 관련 함수 ======================
    public async Task InitialGame()
    {

        SetState("Loading"); // 상태변경

        grid.SetActive(true); // Grid를 켜서 본진이 건설될 위치의 Grid 상태를 바꿀준비

        if (PhotonNetwork.IsMasterClient)
        {
            int[] randomNums = MakeRandom(4);
            gameObject.GetComponent<PhotonView>().RPC("CreateCommand", RpcTarget.Others, randomNums[1]);
            CreateCommand(randomNums[0]);
        }

        // 게임 시작 카운트다운 활성화
        await uIController.CountDown();
        SetState("InGame");
        currentUI.Show();
        UpdateUnitPopulationUI();
        UpdateBuildingPopulationUI();
        target.SetActive(true);
        if (PhotonNetwork.IsMasterClient)
        {
            masterTimer = StartCoroutine(MasterTimer());
        }
    }

    [PunRPC]
    private void CreateCommand(int index)
    {
        Vector3 startingPoint;
        List<Collider> startGrids = new();
        target.transform.position = _randomRot[index];
        startGrids = gridHandler.SetStartingPoint(index);

        // 본진이 지어질 위치 Grid의 평균값으로 계산
        startingPoint = gridHandler.CalculateGridScale(startGrids);
        grid.SetActive(false);

        // 본진 생성
        Building building = buildingController.CreateBuilding(startingPoint, buildingType, new Vector3(-90, 90, 0), gridHandler.constructionGrids);
        // 본진 초기값 세팅
        Debug.Log(building.name);
        building.currentHealth = Mathf.FloorToInt(building.currentHealth); // 소수점 아래자리 버리기
        buildingController.SetBuildingState(building, Building.State.Built, "None");

        building.returnCost = building.levelUpCost; // 작업 취소되면 돌려줄 비용을 레벨업 비용으로 저장
    }
    public void SetState(string newState)
    {
        Enum.TryParse(newState, out GameStates state);
        switch (state)
        {
            case GameStates.Loading:
                gameState = state;
                break;
            case GameStates.InGame:
                gameState = state;
                break;
            case GameStates.ConstructionMode:
                gameState = state;
                grid.SetActive(true);
                SetBuildingListUI();
                break;
            case GameStates.SetMoveRot:
                gameState = state;
                break;
            case GameStates.SetTargetMode:
                gameState = state;
                break;
            case GameStates.EndGame:
                gameObject.GetComponent<PhotonView>().RPC("ShowGameResult",RpcTarget.All);
                target.SetActive(false);
                gameState = state;
                break;
        }
    }

    public bool CheckState(string checkState)
    {
        if (Enum.TryParse(checkState, out GameStates state))
        {
            return gameState == state ? true : false;
        }
        Debug.Log("CheckState함수에서 State 잘못 입력함");
        return false;
    }
    // ====================================================

    // ================== 클릭 관련 함수 ======================
    //clickedObject에서 제가하는 함수 따로 설정해야할듯
    //스스로가 click에 대한 boolean 변수를 따로 가지고 있는건?
    public void SetClickedObject(GameObject gameObject)
    {
        if (CheckState("InGame"))
        {
            unitController.SetActiveHealthBar(clickedObject);
            foreach (GameObject obj in clickedObject)
            {
                if (obj.GetComponent<Entity>() != null && obj.GetComponent<Entity>().clickedEffect != null)
                {
                    if (obj.GetComponent<Entity>().teamID == GameStatus.instance.teamID)
                    {
                        obj.GetComponent<Entity>().clickedEffect.SetActive(false);
                    }
                }

            }
            clickedObject.Clear();

            if (gameObject.GetComponent<Entity>() != null && gameObject.GetComponent<Entity>().clickedEffect != null)
            {
                if (gameObject.GetComponent<Entity>().teamID == GameStatus.instance.teamID)
                {
                    gameObject.GetComponent<Entity>().clickedEffect.SetActive(true);
                }
            }
            clickedObject.Add(gameObject);
            unitController.SetActiveHealthBar(gameObject, true);
        }
    }

    // 리팩토링때 조져야함
    public void AddClickedObject(GameObject gameObject)
    {
        if (gameObject.GetComponent<Entity>() != null && gameObject.GetComponent<Entity>().clickedEffect != null)
        {
            if (gameObject.GetComponent<Entity>().teamID == GameStatus.instance.teamID)
            {
                gameObject.GetComponent<Entity>().clickedEffect.SetActive(true);
            }
        }

        if (clickedObject[0].GetComponent<Unit>() && clickedObject.Count <= 15)
        {
            if (!clickedObject.Contains(gameObject) && CheckState("InGame"))
            {
                clickedObject.Add(gameObject);
                unitController.SetActiveHealthBar(gameObject, true);
                if (clickedObject.Count == 1) SetUnitInfo(7, clickedObject[0]);
                else if (clickedObject.Count >= 3)
                {
                    uIController.ActiveFalseUI(8);
                    SetGroupUnitUI(8, 0);
                }
            }
        }
        else if (!clickedObject[0].GetComponent<Unit>() && clickedObject.Count <= 16)
        {
            if (!clickedObject.Contains(gameObject) && CheckState("InGame"))
            {
                clickedObject.Add(gameObject);

                unitController.SetActiveHealthBar(gameObject, true);
                if (clickedObject.Count == 2) SetUnitInfo(7, clickedObject[1]);
                else if (clickedObject.Count >= 3)
                {
                    uIController.ActiveFalseUI(8);
                    SetGroupUnitUI(8, 1);
                }
            }
        }
    }
    public void SetTargetObject(GameObject gameObject)
    {
        if (CheckState("InGame"))
        {
            if (gameObject.GetComponent<Entity>() == null)
            {
                if (targetObject != null) targetObject.GetComponent<Entity>().enemyClickedEffect.SetActive(false);
            }
            if (gameObject.GetComponent<Entity>() != null && gameObject.GetComponent<Entity>().teamID != GameStatus.instance.teamID)
            {
                if (targetObject != null) targetObject.GetComponent<Entity>().enemyClickedEffect.SetActive(false);
                targetObject = gameObject;
                gameObject.GetComponent<Entity>().enemyClickedEffect.SetActive(true);
            }
        }
    }
    public void GroundRightClickEvent(Vector3 newLocation)
    {
        switch (gameState)
        {
            case GameStates.InGame:
                if (clickedObject[0].TryGetComponent(out Barrack barrack) && clickedObject.Count == 1)
                    buildingController.SetSponPos(newLocation, barrack);
                MoveUnit(newLocation, 1);
                break;
            case GameStates.ConstructionMode:
            case GameStates.SetMoveRot:
            case GameStates.SetTargetMode:
                SetState("InGame");
                break;
        }

        GameObject effect = Instantiate(clickEffect, new Vector3(newLocation.x, .2f, newLocation.z), Quaternion.Euler(new Vector3(90, 0, 0)));
    }

    public void GroundLeftClickEvent(Vector3 newLocation)
    {
        switch (gameState)
        {
            case GameStates.InGame:
                SetClickedObject(ground);
                SetTargetObject(ground);
                SetBuildingListUI();
                break;
            case GameStates.SetMoveRot:
                MoveUnit(newLocation, 1);
                SetState("InGame");
                break;
            case GameStates.SetTargetMode:
                MoveUnit(newLocation, 2);
                SetState("InGame");
                break;
        }
    }
    // =====================================================


    //=================== UI 변경 관련 함수들 ===================
    public void SetBuildingListUI() // 건설할 건물 띄워주는 UI, Ground Inspector창에서 직접 넣어줌
    {
        currentUI = uIController.SetBuildingListUI(0, _commandLevel);
    }
    public void SetBuildingInfo(int UIindex, Building building)
    {
        if (clickedObject[0] == building.gameObject && CheckState("InGame"))
        {
            currentUI = uIController.SetBuildingUI(UIindex, building);
        }
    }
    public void SetUnitInfo(int UIindex, GameObject unit)
    { // unitDictionary에서 unitID에 해당하는 유닛을 가져옴
        if (gameState == GameStates.InGame && unit.TryGetComponent(out Unit clickedUnit))
            currentUI = uIController.SetUnitUI(UIindex, clickedUnit);
    }
    public void SetGroupUnitUI(int UIindex, int startIndex)
    {
        if (gameState == GameStates.InGame && clickedObject.Count >= 3)
        {
            currentUI = uIController.SetGroupUI(UIindex, startIndex, currentUI, clickedObject);
        }
    }
    public void SetHealthBar(Unit unit)
    {
        unit.healthBar.value = (float)(unit.currentHealth * 1.0 / unit.maxHealth);
        // unit.healthBar.gameObject.SetActive(true);
    }

    [PunRPC]
    public void UpdateResourceUI()
    {
        uIController.infoText[0].text = Mathf.FloorToInt(GameStatus.instance.currentResourceCount).ToString();
    }

    [PunRPC]
    public void ShowGameResult()
    {
        uIController.SetEndResult();
    }
    public void UpdateUnitPopulationUI()
    {
        uIController.infoText[1].text = GameStatus.instance.currentUnitCount.ToString();
        uIController.infoText[2].text = GameStatus.instance.maxUnitCount.ToString();
    }
    public void UpdateBuildingPopulationUI()
    {
        uIController.infoText[3].text = GameStatus.instance.currentBuildingCount.ToString();
        uIController.infoText[4].text = GameStatus.instance.maxBuildingCount.ToString();
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

    public void UpdateEventUI(GameObject eventedObject)
    {
        if(eventedObject.TryGetComponent(out Building building))
        {
            ReloadBuildingUI(building);
        } else if(eventedObject.TryGetComponent(out Unit unit))
        {
            SetUnitInfo(7, unit.gameObject);

        }
    }
    // =====================================================


    // =================== 객체 생성 함수들 ===================
    public void CreateBuilding()
    {
        if (gridHandler.CheckCanBuilt() && CheckState("ConstructionMode")) // 건물이 생성가능한지 확인하는 조건문 나중에 자원, 건물인구수 체크하는것도 추가해야함
        // 건물생성가능여부를 판단하는 기능을 하는 함수를 만들어서 조건문에 넣도록 개선해야함
        {
            if (GameStatus.instance.CanCreate(buildingType, "Building"))
            {
                Vector3 buildingPos = gridHandler.CalculateGridScale();
                DelayBuildingCreation(buildingPos);
                GameStatus.instance.SetResources(buildingType, "Building");

                UpdateResourceUI();
                UpdateBuildingPopulationUI();
            }
            grid.SetActive(false);
            SetState("InGame");
            SetBuildingListUI();
        }
    }
    public async void CreateUnit()
    {
        GameObject targetOBJ = clickedObject[0];
        if (targetOBJ.TryGetComponent(out Barrack barrack))
        {
            if (barrack.state == Building.State.Built && GameStatus.instance.CanCreate(unitType, "Unit"))
            {
                OrderUnitCreation(barrack, targetOBJ);

                GameStatus.instance.SetResources(unitType, "Unit");

                UpdateResourceUI();
                UpdateUnitPopulationUI();
                ReloadBuildingUI(barrack);
            }
        }
    }

    private async Task OrderUnitCreation(Barrack barrack, GameObject targetOBJ)
    {
        var cts = new CancellationTokenSource(); // 비동기 작업 취소를 위한 토큰 생성

        buildingController.SetBuildingState(barrack, Building.State.InProgress, unitType);
        ReloadBuildingUI(barrack);

        Vector3 buildingPos = barrack.transform.position; // 건물 위치 받음
        buildingPos = new Vector3(buildingPos.x, buildingPos.y, buildingPos.z - 5.5f); // 유닛이 생성되는 기본값

        tasks[targetOBJ] = cts; // 딕셔너리에 건물 오브젝트와 같이 토큰을 저장
        Unit createdUnit = await DelayUnitCreation(barrack, unitType, buildingPos, cts.Token); // 유닛 생성

        if (createdUnit == null) return;

        tasks.Remove(targetOBJ); // 유닛 생성이 완료되면 딕셔너리에서 제거해줌

        Vector3 destination = barrack._sponPos; // 유닛이 생성되고 이동할 포지션 받음

        // 유닛을 destination으로 이동명령 내리기
        GameObject unitObject = createdUnit.gameObject;

        buildingController.SetBuildingState(barrack, Building.State.Built, "None");
        createdUnit.unitBehaviour = StartCoroutine(unitController.Move(unitObject, destination, 1));

        ReloadBuildingUI(barrack);
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
            if (GameStatus.instance.CanLevelUp(building, _commandLevel))
            {
                var cts = new CancellationTokenSource(); // 비동기 작업 취소를 위한 토큰 생성

                buildingController.SetBuildingState(building, Building.State.InProgress, "LevelUP");

                GameStatus.instance.currentResourceCount -= building.levelUpCost;

                building.GetComponent<PhotonView>().RPC("ActiveLevelUpEffect", RpcTarget.All, true);
                ReloadBuildingUI(building);

                tasks[building.gameObject] = cts; // 딕셔너리에 건물 오브젝트와 같이 토큰을 저장

                if (await OrderCreate(building, building.level * 10f, cts.Token))
                {
                    tasks.Remove(building.gameObject); // 레벨업이 완료되면 딕셔너리에서 제거해줌

                    building.GetComponent<PhotonView>().RPC("ActiveLevelUpEffect", RpcTarget.All, false);
                    buildingController.UpgradeBuilding(building);
                    buildingController.SetBuildingState(building, Building.State.Built, "None");

                    UpdateUnitPopulationUI();
                    UpdateBuildingPopulationUI();
                }

                ReloadBuildingUI(building);
            }
        }
    }
    private async Task DelayBuildingCreation(Vector3 buildingPos)
    {
        var cts = new CancellationTokenSource(); // 비동기 작업 취소를 위한 토큰 생성


        // 건물 아래 Grid를 Builted로 변경
        gridHandler.SetGridsToBuilted();

        //AddComponent로 넣으면 inspector창에서 초기화한 값이 안들어가고 가장 초기의 값이 들어감. inspector 창으로 초기화를 하고 싶으면 script상 초기화 보다는 prefab을 건드리는게 나을듯
        Building building = buildingController.CreateBuilding(buildingPos, buildingType, new Vector3(-90, 90, 90), gridHandler.constructionGrids);
        building.InitTime();

        buildingController.SetBuildingState(building, Building.State.InCreating, "None");

        tasks[building.gameObject] = cts; // 딕셔너리에 건물 오브젝트와 같이 토큰을 저장
        await StartTimer(building.loadingTime, (float time) => UpdateBuildingHealth(building, time), cts.Token);

        tasks.Remove(building.gameObject); // 건물 생성이 완료되면 딕셔너리에서 제거해줌

        building.currentHealth = Mathf.FloorToInt(building.currentHealth); // 소수점 아래자리 버리기
        buildingController.SetBuildingState(building, Building.State.Built, "None");

        building.returnCost = building.levelUpCost; // 작업 취소되면 돌려줄 비용을 레벨업 비용으로 저장

        ReloadingGameStatus(building);
        UpdateUnitPopulationUI();
        ReloadBuildingUI(building);
    }

    private void UpdateBuildingHealth(Building building, float time)
    { // 건물이 생성될 때 체력을 업데이트 해주는 함수
        building.GetComponent<PhotonView>().RPC("UpdateCreateBuildingTime", RpcTarget.All, time);
        if (clickedObject[0].GetComponent<Building>() == building)
        {
            uIController.UpdateHealth(currentUI, building);
        }
    }


    public void ReloadingGameStatus(Building building)
    {
        switch (building)
        {
            case ResourceBuilding:
                GameStatus.instance.resourcePerSecond += building.GetComponent<ResourceBuilding>().increasePersent;
                break;
            case PopulationBuilding:
                GameStatus.instance.maxUnitCount += building.GetComponent<PopulationBuilding>().increasePersent;
                break;
        }
    }

    // =====================================================


    // =================== 유닛 관리 함수들 =================== 
    public void SetUnitType(string unitType)
    {
        this.unitType = unitType;
    }


    public void MoveUnit(Vector3 newLocation, int order)
    {
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
            if (unit.state == Unit.State.Idle || unit.order == Unit.Order.Offensive)
            {
                if (unit.unitBehaviour != null)
                {
                    StopCoroutine(unit.unitBehaviour);
                }
                unit.unitBehaviour = StartCoroutine(unitController.Move(ally, enemy, order));
            }
        }
    }

    public async Task<Unit> DelayUnitCreation(Barrack barrack, string unitType, Vector3 buildingPos, CancellationToken token)
    {
        bool progressState = true;
        switch (unitType)
        {
            case "Soldier":
                progressState = await OrderCreate(barrack, 2f, token);
                break;
            case "Archer":
                progressState = await OrderCreate(barrack, 2f, token);
                break;
            case "Tanker":
                progressState = await OrderCreate(barrack, 2f, token);
                break;
            case "Healer":
                progressState = await OrderCreate(barrack, 2f, token);
                break;
        }
        return progressState == true ? unitController.CreateUnit(buildingPos, unitType) : null;
    }

    private async Task<bool> OrderCreate(Building building, float totalTime, CancellationToken token)
    {
        building.InitOrderTime(totalTime);
        return await StartTimer(totalTime, (float time) => UpdateBuildingProgress(building, time), token);
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

    // =================== 객체 파괴 함수 ======================

    public void DestroyEntity(GameObject entity)
    {
        if(entity.TryGetComponent(out Building building))
        {
            // InCreating이면 CancelProgress를 실행시킴 -> 건물 파괴, 건설비 리턴
            // InProgress이면 CancelProgress를 실행 -> 진행중인 작업 취소, 돈 리턴
            //               State를 Destroy로 바꾸고 다시 CancelProgress를 실행
            // Built이면 State를 Destory로 바꾸고 다시 CancelProgress를 실행
            if(building.state == Building.State.InProgress)
            {
                buildingController.CancelProgress(building);
                buildingController.SetBuildingState(building, Building.State.Destroy, "None");

            }
        } else if(entity.TryGetComponent(out Unit unit))
        {
            unit.DestroyEntity();
            // 유닛 컨트롤러에서 전체 인구수 -1 하는 로직 추가
        }

        UpdateResourceUI();
        UpdateBuildingPopulationUI();
        UpdateUnitPopulationUI();
    }

    // ======================================================

    // =================== 타이머 함수 ======================== 
    private async Task<bool> StartTimer(float time, Action<float> action, CancellationToken token)
    {
        try
        {
            float start = 0f;
            while (time > start)
            {
                token.ThrowIfCancellationRequested();
                start += Time.deltaTime;
                action.Invoke(start);
                await Task.Yield();
            }
            return true;
        }
        catch (OperationCanceledException)
        {
            Debug.Log("작업취소");
            return false;
        }
    }

    private IEnumerator MasterTimer()
    {
        float time = 0f;
        while (true)
        {
            time += Time.deltaTime;
            if (time > 1f)
            {
                GameStatus.instance.GetComponent<PhotonView>().RPC("UpdateResource", RpcTarget.All);
                gameObject.GetComponent<PhotonView>().RPC("UpdateResourceUI", RpcTarget.All);
                time = 0f;
            }
            yield return null;
        }
    }
    // =====================================================

    // =================== 키관련 함수 ======================== 
    public void PressedSpace()
    {
        if (clickedObject.Count == 1)
        {
            if (clickedObject[0] == ground) return;
            else target.transform.position = clickedObject[0].transform.position;
        }
        else
        {
            target.transform.position = clickedObject[1].transform.position;
        }
    }
    public void PressedESC()
    {
        // ESC를 눌렀을 때 현재 선택된 오브젝트에 따라서 관리를 한다.
        GameObject targetObj = clickedObject[0];

        switch (gameState)
        {
            case GameStates.InGame:
                if (targetObj.TryGetComponent(out Building building))
                {
                    if (tasks.TryGetValue(building.gameObject, out var cts))
                    {
                        cts.Cancel();
                        cts.Dispose();
                        tasks.Remove(building.gameObject);
                        buildingController.CancelProgress(building);
                        UpdateResourceUI();
                        UpdateBuildingPopulationUI();
                        UpdateUnitPopulationUI();
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

    public void PressedF10()
    {
        // 상태 상관없이 그냥 설정창 띄우기
        uIController.SetSettingPage(true);

    }

    public void PressedA()
    {
        /*
            게임 State 확인
            clicked[0] 종류 확인
            유닛 -> GameState를 SetTargetMode로 바꿈
            배럭 -> 아처 생성
        */

        GameObject clickedObj = clickedObject[0];
        switch (gameState)
        {
            case GameStates.InGame:
                if (uIController.CheckIsUnitUI(currentUI))
                {
                    SetState("SetTargetMode");
                }
                else if (clickedObj.TryGetComponent(out Barrack barrack))
                {
                    if (CheckCreateUnitPermission(barrack, 2) && barrack.state == Building.State.Built) // 권한 확인
                    {
                        // 아처 생성
                        SetUnitType("Archer");
                        CreateUnit();
                    }
                }
                break;
        }
    }

    public void PressedB()
    {
        /*
           게임 State 확인
           InGame
               clickedObject[0] 확인
                   1.  ground 이면 ConstructionMode로 변경 => 배럭 건설 세팅해줌
                       GridHandler에서 SetBuildingRange 값을 1.25 로 변경
                       BuildingType을 Barrack으로 변경
       */

        GameObject clickedObj = clickedObject[0];
        switch (gameState)
        {
            case GameStates.InGame:
                if (clickedObj == ground)
                {
                    SetState("ConstructionMode");
                    gridHandler.SetBuildingRange(1.25f);
                    SetBuildingType("Barrack");
                }
                break;
        }
    }

    public void PressedD()
    {
        /*
            게임 State 확인
            InGame
                clickedObject[0] 확인
                    1.  ground 이면 ConstructionMode로 변경 => 디펜더 건설 세팅해줌
                        GridHandler에서 SetBuildingRange 값을 0.0001로 변경
                        BuildingType을 Defender로 변경
        */

        GameObject clickedObj = clickedObject[0];
        switch (gameState)
        {
            case GameStates.InGame:
                if (clickedObj == ground)
                {
                    SetState("ConstructionMode");
                    gridHandler.SetBuildingRange(0.001f);
                    SetBuildingType("Defender");
                }
                break;
        }
    }



    public void PressedH()
    {
        /*
            게임 State확인
            InGame
                clickedObject[0] 확인
                    1.  Barrack이면
                        힐러생성
        */

        GameObject clickedObj = clickedObject[0];
        switch (gameState)
        {
            case GameStates.InGame:

                if (clickedObj.TryGetComponent(out Barrack barrack))
                {
                    if (CheckCreateUnitPermission(barrack, 4) && barrack.state == Building.State.Built) // 권한 확인
                    {
                        // 힐러 생성
                        SetUnitType("Healer");
                        CreateUnit();
                    }
                }
                break;
        }
    }

    public void PressedL()
    {
        /*
            게임 State 확인
            InGame
                clickedObject[0] 확인
                    1.  Building 이면
                        레벨업 실행
        */
        GameObject clickedObj = clickedObject[0];
        switch (gameState)
        {
            case GameStates.InGame:
                if (clickedObj.TryGetComponent(out Building building))
                {
                    if (building.state == Building.State.Built)
                    {
                        LevelUpBuilding();
                    }
                }
                break;
        }
    }

    public void PressedM()
    {
        /*
            게임 State 확인
            InGame
                currentUI확인
                    UI가 7 or 8일떄 (유닛이 선택된 상태)
                        SetMoveRot 모드로 변경
        */

        switch (gameState)
        {
            case GameStates.InGame:
                if (uIController.CheckIsUnitUI(currentUI))
                {
                    SetState("SetMoveRot");
                }
                break;
        }
    }

    public void PressedR()
    {
        /*
            게임 State 확인
            InGame
                1. clickedObject[0] = ground 이면 ConstructionMode로 변경 => 터렛 건설 세팅해줌
                    GridHandler에서 SetBuildingRange 값을 1.25 로 변경
                    BuildingType을 ResourceBuilding으로 변경
        */

        GameObject clickedObj = clickedObject[0];
        switch (gameState)
        {
            case GameStates.InGame:
                if (clickedObj == ground)
                {
                    SetState("ConstructionMode");
                    gridHandler.SetBuildingRange(1.25f);
                    SetBuildingType("ResourceBuilding");
                }
                break;
        }
    }

    public void PressedS()
    {
        /*
            게임 State가 InGame인지 확인
                currentUI확인
                    UI가 7 or 8일떄 (유닛이 선택된 상태)
                        clicked Object에 정지명령
                    다른경우
                    clickedObject[0] 확인
                        1. 배럭 -> 솔져 생성
                        2. ground -> ConstructionMode로 변경 => 자원건물 건설 세팅해줌
                            GridHandler에서 SetBuildingRange 값을 0.0001로 변경
                            BuildingType을 PopulationBuilding로 변경
        */

        GameObject clickedObj = clickedObject[0];
        switch (gameState)
        {
            case GameStates.InGame:
                if (uIController.CheckIsUnitUI(currentUI))
                {
                    foreach (GameObject gameObject in clickedObject)
                    {
                        if (gameObject.TryGetComponent(out Unit unit))
                        {
                            unit.ChangeState("Idle");
                            StopCoroutine(unit.unitBehaviour);
                        }
                    }

                }
                else if (clickedObj.TryGetComponent(out Barrack barrack))
                {
                    if (CheckCreateUnitPermission(barrack, 1) && barrack.state == Building.State.Built) // 권한 확인
                    {
                        // 솔저 생성
                        SetUnitType("Soldier");
                        CreateUnit();
                    }
                }
                else if (clickedObj == ground)
                {
                    SetState("ConstructionMode");
                    gridHandler.SetBuildingRange(0.0001f);
                    SetBuildingType("PopulationBuilding");
                }
                break;
        }
    }

    public void PressedT()
    {
        /*
            게임 State갸 InGame인지 확인
                InGame -> clickedObject[0] 확인
                    1. 배럭이면 -> 탱커 생성
        */

        GameObject clickedObj = clickedObject[0];
        switch (gameState)
        {
            case GameStates.InGame:

                if (clickedObj.TryGetComponent(out Barrack barrack))
                {
                    if (CheckCreateUnitPermission(barrack, 3) && barrack.state == Building.State.Built) // 권한 확인
                    {
                        // 탱커 생성
                        SetUnitType("Tanker");
                        CreateUnit();
                    }
                }
                break;
        }
    }

    private bool CheckCreateUnitPermission(Barrack barrack, int level)
    {
        // 건물이 유닛을 생성할 레벨이 되는지 확인하는 함수
        return barrack.level >= level ? true : false;
    }

    // ===================================================== 

    private int[] MakeRandom(int range)
    {
        int[] randomNum;
        System.Random random = new System.Random();
        return Enumerable.Range(0, range) // 0부터 3까지 숫자 생성
                         .OrderBy(x => random.Next()) // 랜덤으로 섞기
                         .Take(2) // 앞에서 2개 선택
                         .ToArray(); // 배열로 변환
    }
}
