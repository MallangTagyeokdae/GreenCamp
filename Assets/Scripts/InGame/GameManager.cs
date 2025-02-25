using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Doozy.Runtime.UIManager.Containers;
using Photon.Pun;
using UnityEngine;
using Unity.VisualScripting;
using FischlWorks_FogWar;
using UnityEngine.SceneManagement;

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
    public GameObject fogWar;
    public GameObject miniMap;
    public Texture2D defaultCursor;
    public Texture2D attackCursor;
    public Texture2D moveCursor;
    public CancellationTokenSource inGameInfoToken = null;
    public Dictionary<GameObject, CancellationTokenSource> tasks = new Dictionary<GameObject, CancellationTokenSource>();
    private Vector3[] _randomRot = { new Vector3(200, 0, 200), new Vector3(-200, 0, 200), new Vector3(200, 0, -200), new Vector3(-200, 0, -200) };
    private Vector3 _commandRot;
    public List<GameObject> groupSet1 = null;
    public List<GameObject> groupSet2 = null;
    public List<GameObject> groupSet3 = null;
    public List<GameObject> groupSet4 = null;
    public Vector3 screenSet1 = Vector3.zero;
    public Vector3 screenSet2 = Vector3.zero;
    public Vector3 screenSet3 = Vector3.zero;
    //-----------------------------
    public int commandLevel = 1;
    private Coroutine masterTimer;

    void Start()
    {
        _ = InitialGame();
    }

    // ================== 상태 관련 함수 ======================
    public async Task InitialGame()
    {
        SetState("Loading"); // 상태변경

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
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

        csFogWar.FogRevealer fogRevealer = new csFogWar.FogRevealer(building.transform, building.fow, true);
        building.fowIndex = fogWar.GetComponent<csFogWar>().AddFogRevealer(fogRevealer);

        building.currentHealth = Mathf.FloorToInt(building.currentHealth); // 소수점 아래자리 버리기
        buildingController.SetBuildingState(building, Building.State.Built, "None");
        _commandRot = building.transform.position;

        building.returnCost = building.levelUpCost; // 작업 취소되면 돌려줄 비용을 레벨업 비용으로 저장
    }
    public void SetState(string newState)
    {
        Enum.TryParse(newState, out GameStates state);
        switch (state)
        {
            case GameStates.Loading:
                GameStatus.instance.gameState = state;
                break;
            case GameStates.InGame:
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                GameStatus.instance.gameState = state;
                PhotonNetwork.AutomaticallySyncScene = false;
                break;
            case GameStates.ConstructionMode:
                GameStatus.instance.gameState = state;
                grid.SetActive(true);
                SetBuildingListUI();
                break;
            case GameStates.SetMoveRot:
                GameStatus.instance.gameState = state;
                Cursor.SetCursor(moveCursor, new Vector2(-5.0f,5.0f), CursorMode.Auto);
                uIController.SetUnitOrderButton(currentUI);
                break;
            case GameStates.SetTargetMode:
                GameStatus.instance.gameState = state;
                Cursor.SetCursor(attackCursor, new Vector2(-5.0f,5.0f), CursorMode.Auto);
                uIController.SetUnitOrderButton(currentUI);
                break;
            case GameStates.EndGame:
                unitController.RemoveCoroutine();
                gameObject.GetComponent<PhotonView>().RPC("ShowGameResult", RpcTarget.All);
                target.SetActive(false);
                GameStatus.instance.gameState = state;
                break;
        }
    }

    public bool CheckState(string checkState)
    {
        if (Enum.TryParse(checkState, out GameStates state))
        {
            return GameStatus.instance.gameState == state ? true : false;
        }
        Debug.Log("CheckState함수에서 State 잘못 입력함");
        return false;
    }
    // ====================================================

    // ================== 클릭 관련 함수 ======================
    //clickedObject에서 제가하는 함수 따로 설정해야할듯
    //스스로가 click에 대한 boolean 변수를 따로 가지고 있는건?
    public void SetClickedObject(GameObject clickedObj)
    {
        if (CheckState("InGame"))
        {
            if (targetObject != null)
            {
                targetObject.GetComponent<Entity>().enemyClickedEffect.SetActive(false);
                targetObject = null;
            }

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if(!clickedObj.GetComponent<Entity>()){
                    return;
                }
                clickedObject.Remove(clickedObj);
                clickedObj.GetComponent<Entity>().clickedEffect.SetActive(false);
                unitController.SetActiveHealthBar(clickedObj);
            }

            else
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

                if (clickedObj.GetComponent<Entity>() != null && clickedObj.GetComponent<Entity>().clickedEffect != null)
                {
                    if (clickedObj.GetComponent<Entity>().teamID == GameStatus.instance.teamID)
                    {
                        clickedObj.GetComponent<Entity>().clickedEffect.SetActive(true);
                    }
                }
                clickedObject.Add(clickedObj);
                unitController.SetActiveHealthBar(clickedObj, true);
            }
        }
    }

    // 리팩토링때 조져야함
    public void AddClickedObject(GameObject clickedObj)
    {

        int startIndex = 0;

        if(!clickedObject[0].GetComponent<Unit>()){
            if(clickedObject[0].TryGetComponent(out Entity entity) && entity.clickedEffect.activeSelf){
                entity.clickedEffect.SetActive(false);
            }
            startIndex = 1;
        }

        if(clickedObject.Count <= 15 + startIndex){
            if (CheckState("InGame"))
            {
                bool isActive = true;
                if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
                    if(!clickedObject.Contains(clickedObj)){
                        clickedObject.Add(clickedObj);
                        if (clickedObj.GetComponent<Entity>() != null && clickedObj.GetComponent<Entity>().clickedEffect != null)
                        {
                            if (clickedObj.GetComponent<Entity>().teamID == GameStatus.instance.teamID)
                            {
                                clickedObj.GetComponent<Entity>().clickedEffect.SetActive(true);
                            }
                        }
                    }
                    else{
                        clickedObject.Remove(clickedObj);
                        if (clickedObj.GetComponent<Entity>() != null && clickedObj.GetComponent<Entity>().clickedEffect != null)
                        {
                            if (clickedObj.GetComponent<Entity>().teamID == GameStatus.instance.teamID)
                            {
                                clickedObj.GetComponent<Entity>().clickedEffect.SetActive(false);
                            }
                        }
                        isActive = false;
                    }
                }
                else{
                   if(!clickedObject.Contains(clickedObj)){
                        clickedObject.Add(clickedObj);
                        if (clickedObj.GetComponent<Entity>() != null && clickedObj.GetComponent<Entity>().clickedEffect != null)
                        {
                            if (clickedObj.GetComponent<Entity>().teamID == GameStatus.instance.teamID)
                            {
                                clickedObj.GetComponent<Entity>().clickedEffect.SetActive(true);
                            }
                        }
                    }
                }
                
                unitController.SetActiveHealthBar(clickedObj, isActive);

                if (clickedObject.Count == startIndex + 1) SetUnitInfo(7, clickedObject[startIndex]);
                else if (clickedObject.Count >= startIndex + 2)
                {
                    uIController.ActiveFalseUI(8);
                    SetGroupUnitUI(8, startIndex);
                }
            }
        }
    }

    public void SetTargetObject(GameObject target, int click)
    {
        if ((CheckState("InGame") && click == 1) || (CheckState("SetTargetMode") && click == 0))
        {
            if (target.GetComponent<Entity>() != null)
            {
                if (targetObject != null)
                {
                    targetObject.GetComponent<Entity>().enemyClickedEffect.SetActive(false);
                }

                if (clickedObject.Count != 1 || clickedObject[0].GetComponent<Unit>())
                {
                    targetObject = target;
                    target.GetComponent<Entity>().enemyClickedEffect.SetActive(true);
                }

                foreach (GameObject go in clickedObject)    //지정한 타겟에게 이동, 해당 타겟이 아니면 move 이외의 조건이 안먹히게 해야함,
                {                                           //지정한 타겟이 att범위에 있으면 move가 아니라 공격으로 명령
                    go.TryGetComponent(out Unit selectedUnit);
                    if (selectedUnit == null || (selectedUnit != null && selectedUnit.state == Unit.State.Die))
                    {
                        continue;
                    }

                    if (selectedUnit.unitBehaviour != null)
                    {
                        StopCoroutine(selectedUnit.unitBehaviour);
                    }

                    selectedUnit.target = targetObject;
                    if (selectedUnit.attackList.Contains(targetObject))
                    {
                        selectedUnit.unitBehaviour = StartCoroutine(unitController.Attack(go, targetObject));
                    }
                    else
                    {
                        selectedUnit.unitBehaviour = StartCoroutine(unitController.Move(go, targetObject, 3));
                    }
                }
            }

            SetState("InGame");
        }
    }

    public void GroundRightClickEvent(Vector3 newLocation)
    {
        if (targetObject != null)
        {
            targetObject.GetComponent<Entity>().enemyClickedEffect.SetActive(false);
            targetObject = null;
        }

        switch (GameStatus.instance.gameState)
        {
            case GameStates.InGame:
                if (clickedObject[0].TryGetComponent(out Building building) && clickedObject.Count == 1)
                {
                    buildingController.SetSponPos(newLocation, building);
                }
                MoveUnit(newLocation, 1);
                break;
            case GameStates.ConstructionMode:
                SetState("InGame");
                grid.SetActive(false);
                break;
            case GameStates.SetMoveRot:
            case GameStates.SetTargetMode:
                SetState("InGame");
                uIController.SetUnitOrderButton(currentUI);
                break;
        }

        GameObject effect = Instantiate(clickEffect, new Vector3(newLocation.x, .2f, newLocation.z), Quaternion.Euler(new Vector3(90, 0, 0)));
        effect.GetComponent<Renderer>().material.renderQueue = 3100;
    }

    public void GroundLeftClickEvent(Vector3 newLocation)
    {
        switch (GameStatus.instance.gameState)
        {
            case GameStates.InGame:
                SetClickedObject(ground);
                SetBuildingListUI();
                break;
            case GameStates.SetMoveRot:
                MoveUnit(newLocation, 1);
                SetState("InGame");
                uIController.SetUnitOrderButton(currentUI);
                break;
            case GameStates.SetTargetMode:
                foreach (GameObject gameObject in clickedObject)
                {
                    Attang(newLocation, 2, gameObject);
                }
                SetState("InGame");
                uIController.SetUnitOrderButton(currentUI);
                break;
        }
    }
    // =====================================================


    //=================== UI 변경 관련 함수들 ===================
    public void SetBuildingListUI() // 건설할 건물 띄워주는 UI, Ground Inspector창에서 직접 넣어줌
    {
        currentUI = uIController.SetBuildingListUI(0, commandLevel);
    }
    public void SetBuildingInfo(int UIindex, Building building)
    {
        if (clickedObject.Count > 0 && clickedObject[0] == building.gameObject && CheckState("InGame"))
        {
            currentUI = uIController.SetBuildingUI(UIindex, building);
        }
        else{
            if(clickedObject.Count == 0){   //임시
                clickedObject.Add(ground);
            }
            SetBuildingListUI();
        }
    }
    public void SetUnitInfo(int UIindex, GameObject unit)
    { // unitDictionary에서 unitID에 해당하는 유닛을 가져옴
        if (GameStatus.instance.gameState == GameStates.InGame && unit.TryGetComponent(out Unit clickedUnit))
            currentUI = uIController.SetUnitUI(UIindex, clickedUnit);
    }
    public void SetGroupUnitUI(int UIindex, int startIndex)
    {
        if (GameStatus.instance.gameState == GameStates.InGame /*&& clickedObject.Count >= 3*/)
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

        if (building.GetComponent<Academy>()) // 다른 건물에서 업그래드이가 완료됐을 때 아카데미를 클릭하고 있으면 UI를 업데이트 해야줘야기 때문에 필터링해줌
        {
            if (clickedObject[0].GetComponent<Academy>())
            {
                SetBuildingInfo(9, building);
            }
        }

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
                case "Academy":
                    SetBuildingInfo(9, building);
                    break;
            }
        }
    }

    public void UpdateEventUI(GameObject eventedObject)
    {
        // 체력이 0이하일때 
        if(eventedObject.GetComponent<Entity>().currentHealth <= 0 && clickedObject.Contains(eventedObject))
        {
            clickedObject.Remove(eventedObject);
        }

        switch (clickedObject.Count())
        {
            case 0:
                // 클릭리스트 크기가 0
                SetClickedObject(ground);
                SetBuildingListUI();
                break;
            case 1:
                if(clickedObject[0].TryGetComponent(out Building building))
                {
                    // 클릭리스트 크기가 1인데 0번째가 빌딩
                    if(uIController.CheckIsUnitUI(currentUI))
                    {
                        SetClickedObject(ground);
                        SetBuildingListUI();
                    }
                    else
                    {
                        ReloadBuildingUI(building);
                    }
                }
                else if(clickedObject[0].TryGetComponent(out Unit unit))
                {
                    // 클릭리스트 크기가 1인데 0번째가 유닛
                    SetUnitInfo(7, unit.gameObject);
                }
                else
                {
                    // 클릭리스트 크기가 1인데 0번째가 그라운드
                    SetClickedObject(ground);
                    SetBuildingListUI();
                }
                break;
            case 2:
                if(clickedObject[0].GetComponent<Unit>())
                {
                    // 클릭리스트 크기가 2인데 0번째가 유닛
                    uIController.ActiveFalseUI(8);
                    SetGroupUnitUI(8, 0);
                }
                else
                {
                    // 클릭리스트 크기가 2인데 0번째가 유닛이 아님
                    SetUnitInfo(7,clickedObject[1]);
                }
                break;
            default:
                if (clickedObject[0].GetComponent<Unit>())
                {
                    // 클릭리스트 크기가 2보다 큰데 0번째가 유닛
                    uIController.ActiveFalseUI(8);
                    SetGroupUnitUI(8, 0);
                }
                else
                {
                    // 클릭리스트 크기가 2보다 큰데 0번째가 유닛이 아님
                    uIController.ActiveFalseUI(8);
                    SetGroupUnitUI(8, 1);
                }
                break;
        }
    }

    public void ResetInfoToken()
    {
        Debug.Log("ResetInfoToken Info 실행");
        inGameInfoToken?.Cancel();
        inGameInfoToken?.Dispose();
        uIController.ResetInGameInfomation();

        inGameInfoToken = new CancellationTokenSource();
    }

    public async Task SetInGameInfo(string info)
    {
        Debug.Log("SetInGame Info 실행");
        ResetInfoToken();
        uIController.SetInGameInfomation(info);

        try
        {
            Debug.Log(info);
            await Task.Delay(3000, inGameInfoToken.Token);
        }
        catch (TaskCanceledException)
        {
            Debug.Log("작업 취소");
        }
        finally
        {
            Debug.Log("설명란 리셋");
            uIController.ResetInGameInfomation();
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

                if (float.IsNaN(buildingPos.x) || float.IsNaN(buildingPos.y) || float.IsNaN(buildingPos.z) ||
                    float.IsInfinity(buildingPos.x) || float.IsInfinity(buildingPos.y) || float.IsInfinity(buildingPos.z))
                {
                    Debug.LogError($"[ERROR] 잘못된 위치 값 감지: {buildingPos}");
                    grid.SetActive(false);
                    SetState("InGame");
                    SetBuildingListUI();
                }
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
        else if (targetOBJ.TryGetComponent(out Command command))
        {
            if (command.state == Building.State.Built && GameStatus.instance.CanCreate(unitType, "Unit") && clickedObject.Count == 1)
            {
                OrderUnitCreation(command, targetOBJ);

                GameStatus.instance.SetResources(unitType, "Unit");

                UpdateResourceUI();
                UpdateUnitPopulationUI();
                ReloadBuildingUI(command);
            }
        }
    }

    private async Task OrderUnitCreation(Building building, GameObject targetOBJ)
    {
        Vector3 destination = Vector3.zero;
        var cts = new CancellationTokenSource(); // 비동기 작업 취소를 위한 토큰 생성

        buildingController.SetBuildingState(building, Building.State.InProgress, unitType);
        ReloadBuildingUI(building);

        Vector3 buildingPos = building.transform.position; // 건물 위치 받음
        buildingPos = new Vector3(buildingPos.x, buildingPos.y, buildingPos.z - 5.5f); // 유닛이 생성되는 기본값

        tasks[targetOBJ] = cts; // 딕셔너리에 건물 오브젝트와 같이 토큰을 저장
        Unit createdUnit = await DelayUnitCreation(building, unitType, buildingPos, cts.Token); // 유닛 생성

        if (createdUnit == null) return;

        // 안개 시야 설정
        csFogWar.FogRevealer fogRevealer = new csFogWar.FogRevealer(createdUnit.transform, createdUnit.fow, true);
        createdUnit.fowIndex = fogWar.GetComponent<csFogWar>().AddFogRevealer(fogRevealer);

        tasks.Remove(targetOBJ); // 유닛 생성이 완료되면 딕셔너리에서 제거해줌

        if (building.TryGetComponent(out Barrack barrack))
        {
            destination = barrack._sponPos; // 유닛이 생성되고 이동할 포지션 받음
        }
        else if (building.TryGetComponent(out Command command))
        {
            destination = command._sponPos;
        }

        // 유닛을 destination으로 이동명령 내리기
        GameObject unitObject = createdUnit.gameObject;

        buildingController.SetBuildingState(building, Building.State.Built, "None");
        createdUnit.unitBehaviour = StartCoroutine(unitController.Move(unitObject, destination, 1));

        ReloadBuildingUI(building);
    }
    // =====================================================

    // =================== 건물 관리 함수들 ===================
    public void SetBuildingType(string buildingType)
    {
        this.buildingType = buildingType;
    }
    public async void LevelUpBuilding()
    {
        if (clickedObject[0].TryGetComponent(out Building building) && clickedObject.Count == 1)
        {
            if (GameStatus.instance.CanLevelUp(building, commandLevel))
            {
                var cts = new CancellationTokenSource(); // 비동기 작업 취소를 위한 토큰 생성

                buildingController.SetBuildingState(building, Building.State.InProgress, "LevelUP");

                GameStatus.instance.currentResourceCount -= building.levelUpCost;

                building.GetComponent<PhotonView>().RPC("ActiveLevelUpEffect", RpcTarget.All, true);
                ReloadBuildingUI(building);

                tasks[building.gameObject] = cts; // 딕셔너리에 건물 오브젝트와 같이 토큰을 저장

                if (await OrderCreate(building, building.level /** 25f*/, cts.Token))

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

        // 안개 시야 설정
        csFogWar.FogRevealer fogRevealer = new csFogWar.FogRevealer(building.transform, building.fow, false);
        building.fowIndex = fogWar.GetComponent<csFogWar>().AddFogRevealer(fogRevealer);

        building.InitTime();

        buildingController.SetBuildingState(building, Building.State.InCreating, "None");

        tasks[building.gameObject] = cts; // 딕셔너리에 건물 오브젝트와 같이 토큰을 저장
        await StartTimer(building.loadingTime, (float time) => UpdateBuildingHealth(building, time), cts.Token);

        tasks.Remove(building.gameObject); // 건물 생성이 완료되면 딕셔너리에서 제거해줌

        buildingController.LastCheckBuildingHealth(building);
        buildingController.SetBuildingState(building, Building.State.Built, "None");

        building.returnCost = building.levelUpCost; // 작업 취소되면 돌려줄 비용을 레벨업 비용으로 저장

        ReloadingGameStatus(building);
        UpdateUnitPopulationUI();
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

    public void Attang(Vector3 newLocation, int order, GameObject orderedObjs)
    {

        if (orderedObjs.TryGetComponent(out Unit selectedUnit))
        {
            if (selectedUnit.state == Unit.State.Die)
            {
                return;
            }

            selectedUnit.destination = newLocation;
            selectedUnit.target = null;
            if (selectedUnit.unitBehaviour != null)
            {
                StopCoroutine(selectedUnit.unitBehaviour);
            }

            if (selectedUnit.aggList.Count == 0)
            {
                selectedUnit.unitBehaviour = StartCoroutine(unitController.Move(orderedObjs, selectedUnit.destination, order));
            }
            else if (selectedUnit.attackList.Count == 0)
            {
                foreach (GameObject enemy in selectedUnit.aggList)
                {
                    selectedUnit.unitBehaviour = StartCoroutine(unitController.Move(orderedObjs, enemy, 3)); // aggro
                    break;
                }
            }
            else
            {
                foreach (GameObject enemy in selectedUnit.attackList)
                {
                    selectedUnit.unitBehaviour = StartCoroutine(unitController.Attack(orderedObjs, enemy));
                    break;
                }

            }
        }
    }


    public void MoveUnit(Vector3 newLocation, int order)
    {
        foreach (GameObject go in clickedObject)
        {
            go.TryGetComponent(out Unit selectedUnit);
            if (selectedUnit == null || (selectedUnit != null && selectedUnit.state == Unit.State.Die))
            {
                continue;
            }

            selectedUnit.target = null;

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
        if (enemyEntity == null || unit.teamID == enemyEntity.teamID || unit.state == Unit.State.Die)
        {
            return;
        }
        else
        {
            if (!unit.attackList.Contains(enemy))
            {
                // Debug.Log(enemy.name + " 가 어택 리스트에 추가됨");
                unit.attackList.Add(enemy);
            }
            if (unit.order == Unit.Order.Move || unit.state == Unit.State.Attack || unit.state == Unit.State.Die || (unit.target != null && unit.target != enemy))
            {
                //Debug.Log($"unit.target != null: {unit.target != null} && unit.target != enemy: {unit.target != enemy}");
                return;
            }

            if (unit.unitBehaviour != null)
            {
                StopCoroutine(unit.unitBehaviour);
            }
            if (enemy != null)
            {
                ally.GetComponent<PhotonView>().RPC("SetTarget", RpcTarget.All, enemy.GetComponent<PhotonView>().ViewID);
            }
            unit.unitBehaviour = StartCoroutine(unitController.Attack(ally, enemy));
        }
    }

    public void Aggregated(GameObject ally, GameObject enemy)
    {
        int order;
        Unit unit = ally.GetComponent<Unit>();
        enemy.TryGetComponent(out Entity enemyEntity);
        if (enemyEntity == null || unit.teamID == enemyEntity.teamID || unit.state == Unit.State.Die)
        {
            return;
        }
        else
        {
            if (!unit.aggList.Contains(enemy))
            {
                unit.aggList.Add(enemy);
            }

            if (unit.state == Unit.State.Idle || (unit.order == Unit.Order.Offensive && unit.state == Unit.State.Move))
            {
                if (unit.unitBehaviour != null)
                {
                    StopCoroutine(unit.unitBehaviour);
                }
                order = (unit.order == Unit.Order.Offensive) ? 2 : 3; // 유닛의 Order가 Offensive면 유지, 아니면 Attack으로 변경
                unit.unitBehaviour = StartCoroutine(unitController.Move(ally, enemy, order));
            }
        }
    }
    public void Heal(GameObject me)
    {
        Unit unit = me.GetComponent<Unit>();

        if (unit.order == Unit.Order.Move || unit.state == Unit.State.Attack || unit.state == Unit.State.Die)
        {
            Debug.Log("오더가 무브이거나 스테이트가 어택이거나 다이");
            return;
        }
        if (unit.unitBehaviour != null)
        {
            StopCoroutine(unit.unitBehaviour);
        }
        unitController.Heal(me);
    }

    public async Task<Unit> DelayUnitCreation(Building building, string unitType, Vector3 buildingPos, CancellationToken token)
    {
        bool progressState = true;
        switch (unitType)
        {
            case "Soldier":
                // progressState = await OrderCreate(building, 10f, token);
                progressState = await OrderCreate(building, 10f, token);
                break;
            case "Archer":
                // progressState = await OrderCreate(building, 15f, token);
                progressState = await OrderCreate(building, 15f, token);
                break;
            case "Tanker":
                // progressState = await OrderCreate(building, 25f, token);
                progressState = await OrderCreate(building, 20f, token);
                break;
            case "Healer":
                // progressState = await OrderCreate(building, 30f, token);
                progressState = await OrderCreate(building, 25f, token);
                break;
            case "Scout":
                // progressState = await OrderCreate(building, 7f, token);
                progressState = await OrderCreate(building, 7f, token);
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
        if (clickedObject[0].GetComponent<Building>() == building && clickedObject.Count == 1)
        {
            uIController.SetProgressBar(currentUI, building.progress / 100, 1);
        }
    }

    public async void UpgradeUnit(string type)
    {
        if (clickedObject[0].TryGetComponent(out Building building) && clickedObject.Count == 1)
        {
            Academy academy = building.GetComponent<Academy>();
            int _upgradeLevel = 0;
            bool _isUpgrade = false;
            switch (type)
            {
                case "Damage":
                    _upgradeLevel = GameStatus.instance.damageLevel;
                    _isUpgrade = GameStatus.instance.isDamageUpgrade;

                    if (GameStatus.instance.CanUpgradeUnit(building, _upgradeLevel, GameStatus.instance.damageUpgradeCost, _isUpgrade))
                    {
                        GameStatus.instance.isDamageUpgrade = !_isUpgrade;
                        CallUpgrade(building, _upgradeLevel, type, GameStatus.instance.damageUpgradeCost);
                    }
                    break;
                case "Armor":
                    _upgradeLevel = GameStatus.instance.armorLevel;
                    _isUpgrade = GameStatus.instance.isArmorUpgrade;

                    if (GameStatus.instance.CanUpgradeUnit(building, _upgradeLevel, GameStatus.instance.armorUpgradeCost, _isUpgrade))
                    {
                        GameStatus.instance.isArmorUpgrade = !_isUpgrade;
                        CallUpgrade(building, _upgradeLevel, type, GameStatus.instance.armorUpgradeCost);
                    }
                    break;
                case "Health":
                    _upgradeLevel = GameStatus.instance.healthLevel;
                    _isUpgrade = GameStatus.instance.isHealthUpgrade;

                    if (GameStatus.instance.CanUpgradeUnit(building, _upgradeLevel, GameStatus.instance.healthUpgradeCost, _isUpgrade))
                    {
                        GameStatus.instance.isHealthUpgrade = !_isUpgrade;
                        CallUpgrade(building, _upgradeLevel, type, GameStatus.instance.healthUpgradeCost);
                    }
                    break;
            }

        }
    }

    public async void CallUpgrade(Building building, int upgradeLevel, string type, int cost)
    {
        GameStatus.instance.currentResourceCount -= cost;
        building.GetComponent<Academy>().returnCost = cost;

        var cts = new CancellationTokenSource(); // 비동기 작업 취소를 위한 토큰 생성

        buildingController.SetBuildingState(building, Building.State.InProgress, type);

        UpdateResourceUI();
        ReloadBuildingUI(building);

        tasks[building.gameObject] = cts; // 딕셔너리에 건물 오브젝트와 같이 토큰을 저장

        // 업그레이드 시간설정 부분
        if (await OrderCreate(building, upgradeLevel * 30f, cts.Token))
        {
            tasks.Remove(building.gameObject); // 레벨업이 완료되면 딕셔너리에서 제거해줌

            // 유닛 업그레이드 종효후 상태 맞춰주기
            // 체력, 공격력, 체력 수치 올리는 로직 여기있음
            switch (type)
            {
                case "Damage":
                    GameStatus.instance.damageIncrease += 3;
                    GameStatus.instance.damageLevel++;
                    unitController.ApplyUnitUpgrade(type, 3);
                    GameStatus.instance.isDamageUpgrade = false;
                    GameStatus.instance.damageUpgradeCost *= 2;
                    break;
                case "Armor":
                    GameStatus.instance.armorIncrease += 3;
                    GameStatus.instance.armorLevel++;
                    unitController.ApplyUnitUpgrade(type, 3);
                    GameStatus.instance.isArmorUpgrade = false;
                    GameStatus.instance.armorUpgradeCost *= 2;
                    break;
                case "Health":
                    GameStatus.instance.healthIncrease += 10;
                    GameStatus.instance.healthLevel++;
                    unitController.ApplyUnitUpgrade(type, 10);
                    GameStatus.instance.isHealthUpgrade = false;
                    GameStatus.instance.healthUpgradeCost *= 2;
                    break;
            }
            buildingController.SetBuildingState(building, Building.State.Built, "None");
        }

        ReloadBuildingUI(building);
    }

    // =====================================================

    // =================== 객체 파괴 함수 ======================

    public void DestroyEntity(GameObject entity)
    {
        if (entity.TryGetComponent(out Building building))
        {
            // InCreating이면 CancelProgress를 실행시킴 -> 건물 파괴, 건설비 리턴
            // InProgress이면 CancelProgress를 실행 -> 진행중인 작업 취소, 돈 리턴, State가 Built로 바뀜
            // Built이면 State를 Destory로 바꾸고 다시 CancelProgress를 실행
            if (tasks.TryGetValue(building.gameObject, out var cts))
            {
                cts.Cancel();
                cts.Dispose();
                tasks.Remove(building.gameObject);
                buildingController.CancelProgress(building);
            }

            switch (building.state)
            {
                case Building.State.Built:
                    buildingController.DestroyBuilding(building);
                    break;
            }

            if (clickedObject[0] == entity && clickedObject.Count() == 1)
            // 건물이 선택되어있고, 크기가 1인경우 (드래그 했을 때 건물이 첫번째 인덱스에 들어간 상황은 유닛 UI가 뜬 상태이기 떄문에)
            {
                SetClickedObject(ground);
                SetBuildingListUI();
            }
        }
        else if (entity.TryGetComponent(out Unit unit))
        {
            if (unit.unitBehaviour != null)
            {
                StopCoroutine(unit.unitBehaviour);
            }
            if (clickedObject.Contains(entity))
            // UI 업데이트해줌
            {
                if (clickedObject.Count == 1)
                {
                    SetClickedObject(ground);
                    SetBuildingListUI();
                }
                else if (clickedObject[0].GetComponent<Unit>())
                {
                    clickedObject.Remove(entity);
                    SetGroupUnitUI(8, 0);
                }
                else
                {
                    clickedObject.Remove(entity);
                    SetGroupUnitUI(8, 1);
                }
            }
            unitController.DestroyUnit(unit);
        }

        UpdateResourceUI();
        UpdateBuildingPopulationUI();
        UpdateUnitPopulationUI();
    }

    // ======================================================

    // =================== 부대지정 함수 ======================== 

    public void SetGroup(List<GameObject> entities, int level) // 부대지정함수, 지정할 요소 리스트, 그룹순서를 받음
    {
        List<GameObject> tempGroup = new List<GameObject>(entities);
        switch(level)
        {
            case 1:
                groupSet1 = tempGroup;
                break;
            case 2:
                groupSet2 = tempGroup;
                break;
            case 3:
                groupSet3 = tempGroup;
                break;
            case 4:
                groupSet4 = tempGroup;
                break;
        }
    }

    public void SetClickedOBJToGroup(int level)
    {
        List<GameObject> tempGroup = new List<GameObject>();
        switch(level)
        {
            case 1:
                tempGroup = groupSet1;
                break;
            case 2:
                tempGroup = groupSet2;
                break;
            case 3:
                tempGroup = groupSet3;
                break;
            case 4:
                tempGroup = groupSet4;
                break;
        }

        for(int i=0; i<tempGroup.Count; i++)
        {
            if(i==0)
            {
                tempGroup[i].GetComponent<ClickEventHandler>().leftClickDownEvent.Invoke(tempGroup[i].transform.position);
            }
            else
            {
                AddClickedObject(tempGroup[i]);
            }
            if(tempGroup[i].TryGetComponent(out Entity entity))
            {
                entity.clickedEffect.SetActive(true);
            }
        }
    }

    public void SetScreen(int level) // 화면지정함수
    {
        Vector3 targetPos = new Vector3(target.transform.position.x, target.transform.position.y, target.transform.position.z);
        switch(level)
        {
            case 1:
                screenSet1 = targetPos;
                break;
            case 2:
                screenSet2 = targetPos;
                break;
            case 3:
                screenSet3 = targetPos;
                break;
        }
    }

    public void SetCamera(int level)
    {
        switch(level)
        {
            case 1:
                target.transform.position = screenSet1;
                break;
            case 2:
                target.transform.position = screenSet2;
                break;
            case 3:
                target.transform.position = screenSet3;
                break;
        }
    }

    // ======================================================

    // =================== 타이머 함수 ======================== 
    private async Task<bool> StartTimer(float time, Action<float> action, CancellationToken token)
    {
        try
        {
            float start = 0f;
            while (time >= start)
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
            if (clickedObject[0] == ground)
            {
                target.transform.position = _commandRot;
            }
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

        switch (GameStatus.instance.gameState)
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

    public void PressedF1()
    {
        switch (GameStatus.instance.gameState)
        {
            case GameStates.InGame:
            case GameStates.SetTargetMode:
            case GameStates.SetMoveRot:
                if(Input.GetKey(KeyCode.LeftShift))
                {
                    SetScreen(1);
                }
                else
                {
                    SetCamera(1);
                }
                break;
        }
    }

    public void PressedF2()
    {
        switch (GameStatus.instance.gameState)
        {
            case GameStates.InGame:
            case GameStates.SetTargetMode:
            case GameStates.SetMoveRot:
                if(Input.GetKey(KeyCode.LeftShift))
                {
                    SetScreen(2);
                }
                else
                {
                    SetCamera(2);
                }
                break;
        }
    }

    public void PressedF3()
    {
        switch (GameStatus.instance.gameState)
        {
            case GameStates.InGame:
            case GameStates.SetTargetMode:
            case GameStates.SetMoveRot:
                if(Input.GetKey(KeyCode.LeftShift))
                {
                    SetScreen(3);
                }
                else
                {
                    SetCamera(3);
                }
                break;
        }
    }

    public void PressedF10()
    {
        // 상태 상관없이 그냥 설정창 띄우기
        uIController.SetSettingPage(true);

    }

    public void Pressed1()
    {
        switch (GameStatus.instance.gameState)
        {
            case GameStates.InGame:
            case GameStates.SetTargetMode:
            case GameStates.SetMoveRot:
                if(Input.GetKey(KeyCode.LeftShift))
                {
                    SetGroup(clickedObject, 1);
                }
                else
                {
                    SetClickedOBJToGroup(1);
                }
                break;
        }
    }

    public void Pressed2()
    {
        switch (GameStatus.instance.gameState)
        {
            case GameStates.InGame:
            case GameStates.SetTargetMode:
            case GameStates.SetMoveRot:
                if(Input.GetKey(KeyCode.LeftShift))
                {
                    SetGroup(clickedObject, 2);
                }
                else
                {
                    SetClickedOBJToGroup(2);
                }
                break;
        }
    }

    public void Pressed3()
    {
        switch (GameStatus.instance.gameState)
        {
            case GameStates.InGame:
            case GameStates.SetTargetMode:
            case GameStates.SetMoveRot:
                if(Input.GetKey(KeyCode.LeftShift))
                {
                    SetGroup(clickedObject, 3);
                }
                else
                {
                    SetClickedOBJToGroup(3);
                }
                break;
        }
    }

    public void Pressed4()
    {
        switch (GameStatus.instance.gameState)
        {
            case GameStates.InGame:
            case GameStates.SetTargetMode:
            case GameStates.SetMoveRot:
                Debug.Log("1눌림");
                if(Input.GetKey(KeyCode.LeftShift))
                {
                    SetGroup(clickedObject, 4);
                }
                else
                {
                    SetClickedOBJToGroup(4);
                }
                break;
        }
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
        switch (GameStatus.instance.gameState)
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
                else if (clickedObj.TryGetComponent(out Academy academy))
                {
                    if (academy.state == Building.State.Built)
                    {
                        UpgradeUnit("Armor");
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
        switch (GameStatus.instance.gameState)
        {
            case GameStates.InGame:
                if (clickedObj == ground && clickedObject.Count == 1)
                {
                    SetState("ConstructionMode");
                    gridHandler.SetBuildingRange(1.25f);
                    SetBuildingType("Barrack");
                }
                else
                {
                    SetClickedObject(ground);
                    SetBuildingListUI();
                }
                break;
        }
    }

    public void PressedC()
    {
        /*
            게임 State확인
            InGame
                clickedObject[0] 확인
                    1.  Barrack이면
                        힐러생성
        */

        GameObject clickedObj = clickedObject[0];
        switch (GameStatus.instance.gameState)
        {
            case GameStates.InGame:

                if (clickedObj.TryGetComponent(out Command command))
                {
                    if (command.state == Building.State.Built) // 권한 확인
                    {
                        // 힐러 생성
                        SetUnitType("Scout");
                        CreateUnit();
                    }
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
        switch (GameStatus.instance.gameState)
        {
            case GameStates.InGame:
                if (clickedObj == ground && commandLevel >= 4 && clickedObject.Count == 1)
                {
                    SetState("ConstructionMode");
                    gridHandler.SetBuildingRange(0.00001f);
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
        switch (GameStatus.instance.gameState)
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
                else if (clickedObj.TryGetComponent(out Academy academy))
                {
                    if (academy.state == Building.State.Built)
                    {
                        UpgradeUnit("Health");
                    }
                }
                else if (clickedObj.TryGetComponent(out Healer healer))
                {
                    if (healer.attackList.Count != 0 && healer.isCool == false)
                    {
                        if (healer.state == Unit.State.Idle)
                        {
                            healer.SetState("Idle");
                        }
                        else
                        {
                            healer.SetOrder(0);
                            healer.ChangeState("Idle");
                        }
                        Heal(healer.gameObject);
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

        switch (GameStatus.instance.gameState)
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
        switch (GameStatus.instance.gameState)
        {
            case GameStates.InGame:
                if (clickedObj == ground && clickedObject.Count == 1)
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
        switch (GameStatus.instance.gameState)
        {
            case GameStates.InGame:
                if (uIController.CheckIsUnitUI(currentUI))
                {
                    foreach (GameObject gameObject in clickedObject)
                    {
                        if (gameObject.TryGetComponent(out Unit unit))
                        {
                            StopCoroutine(unit.unitBehaviour);
                            unit.ChangeState("Idle");
                            unit.SetOrder(0);
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
                else if (clickedObj == ground && clickedObject.Count == 1)
                {
                    SetState("ConstructionMode");
                    gridHandler.SetBuildingRange(0.00001f);
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
        switch (GameStatus.instance.gameState)
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

    public void PressedW()
    {
        /*
            게임 State 확인
            InGame
                clickedObject[0] 확인
                    1.  Academy 이고, state가 Built이면
                        공격력 업그레이드 실행
        */

        GameObject clickedObj = clickedObject[0];
        switch (GameStatus.instance.gameState)
        {
            case GameStates.InGame:
                if (clickedObj.TryGetComponent(out Academy academy))
                {
                    if (academy.state == Building.State.Built)
                    {
                        UpgradeUnit("Damage");
                    }
                }
                break;
        }
    }

    public void PressedU()
    {
        /*
            게임 State 확인
            InGame
                clickedObject[0] 확인
                    1.  Building 이면
                        레벨업 실행
        */
        GameObject clickedObj = clickedObject[0];
        switch (GameStatus.instance.gameState)
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

    public void PressedY()
    {
        /*
           게임 State 확인
           InGame
               clickedObject[0] 확인
                   1.  ground 이면 ConstructionMode로 변경 => 아카데미 건설 세팅해줌
                       GridHandler에서 SetBuildingRange 값을 1.25 로 변경
                       BuildingType을 Academy로 변경
       */

        GameObject clickedObj = clickedObject[0];
        switch (GameStatus.instance.gameState)
        {
            case GameStates.InGame:
                if (clickedObj == ground && clickedObject.Count == 1)
                {
                    SetState("ConstructionMode");
                    gridHandler.SetBuildingRange(1.25f);
                    SetBuildingType("Academy");
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

    //------------------------------------------------------

    public void testleaveroom()
    {
        PhotonManager.instance.LeaveRoom();
    }
}
