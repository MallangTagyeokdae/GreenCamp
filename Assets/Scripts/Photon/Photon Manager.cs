using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro.Examples;
using Unity.VisualScripting;
using TMPro;
using ExitGames.Client.Photon;
using Doozy.Runtime.UIManager.Components;
using UnityEngine.SceneManagement;
//using ExitGames.Client.Photon;
//using Photon.Pun.Demo.Cockpit;

public class PhotonManager : MonoBehaviourPunCallbacks, IOnEventCallback // 상속을 MonoBehaviour > MonoBehaviourPunCallbacks로 변경(MonoBehaviour에서 photon 관련 behavior가 추가된 버전)
{
    private static PhotonManager _instance;
    public static PhotonManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PhotonManager>();
                return _instance;
            }
            return _instance;
        }
    }
    private List<RoomInfo> _roomList;// = new List<RoomInfo>();
    private readonly string version = "1.0f";
    public LobbyController lobbyController;
    public TeamUIController teamUIController;
    public UserInfo userInfo;
    public bool loggedin = false;
    public bool session = false;
    private bool _gaming = false;
    void Awake()
    {
        // 이 객체가 씬 전환 시 파괴되지 않도록 설정
        if (_instance == null)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);  // 기존 객체가 있으면 새로 생성된 객체 제거
        }
        //DontDestroyOnLoad(this.gameObject);
        //userInfo.loggedin = false;
        _roomList = new List<RoomInfo>();
        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    // Start is called before the first frame update
    void Start()
    {
        //같은 룸의 유저들에게 자동으로  씬을 로딩
        PhotonNetwork.AutomaticallySyncScene = true;
        //같은 버전의 유저끼리 접속을 허용
        PhotonNetwork.GameVersion = version;
        //포톤 서버와 통신횟수 설정. 초당 30회
        //Debug.Log(PhotonNetwork.SendRate);
    }

    //----------------------------------------------------------------------------------------------------------------------
    void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        lobbyController = FindObjectOfType<LobbyController>();
        teamUIController = FindObjectOfType<TeamUIController>();
    }
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this); // 이벤트 리스너 등록
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this); // 이벤트 리스너 해제
    }

    public void OnEvent(EventData photonEvent)
    {
        Player player;
        switch (photonEvent.Code)
        {
            case 1: // 팀 선택 이벤트
                player = (Player)photonEvent.CustomData;
                teamUIController.OnTeamSelect(player);
                break;
            
            case 2:
                player = (Player)photonEvent.CustomData;
                teamUIController.DeselectTeam(player);
                break;
            case 3: //팀 변경 이벤트
                SetTeam(GetTeam(PhotonNetwork.LocalPlayer) == "Red" ? "Blue" : "Red");
                break;
            case 4:
                _gaming = true;
                if(PhotonNetwork.IsMasterClient){
                    StartGame();
                }
                break;
            default:
                //Debug.Log("Unknown event received: " + photonEvent.Code);
                break;
        }
    }

    public void ConnectGame(string nickName)
    {
        //유저 아이디 할당
        PhotonNetwork.NickName = nickName;
        //서버 접속
        PhotonNetwork.ConnectUsingSettings();
    }

    //포톤 서버 접속 후 호출되는 콜백함수
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master!");
        PhotonNetwork.JoinLobby(); // 로비 입장
        //base.OnConnectedToMaster();
    }

    //로비에 접속 후 호출되는 콜백함수
    /*public override void OnJoinedLobby()
    {
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
    }*/

    //RoomList가 update될 때마다 실행
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList)
            {
                _roomList.Remove(room);
            }
            else if (_roomList.Contains(room))
            {
                _roomList[_roomList.IndexOf(room)] = room;
            }
            else
            {
                _roomList.Add(room);
            }
        }

        if(lobbyController!=null){
            lobbyController.updateRoomList(_roomList);
        }
        //base.OnRoomListUpdate(roomList);
    }

    public void CreateRoom(string roomTitle)
    {
        string roomName = "Room_" + System.Guid.NewGuid().ToString();

        // 룸 생성
        RoomOptions roomOptions = new RoomOptions(); // 최대 플레이어 수와 방 제목을 설정하기 위한 변수
        roomOptions.MaxPlayers = 2; // 최대 플레이어 수 설정
        roomOptions.EmptyRoomTtl = 1000; //n msec 동안 방 파괴 x
        ///string titlecheck = (string)roomInfo.CustomProperties["Title"];
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable{{ "Red", false }, {"Blue", false}};
        


        if (PhotonNetwork.CreateRoom(roomName + "~" + roomTitle, roomOptions))
        {
            Debug.Log("Created room with name: " + roomName + roomTitle);
        }
    }

    //player가 방을 떠날 때 콜백되는 함수
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if(!_gaming){
            teamUIController.DeselectTeam(otherPlayer);
            teamUIController.ActiveStartButton();
        }
        else{
            GameManager.instance.SetState("EndGame");
        }
        
        if (PhotonNetwork.CurrentRoom.PlayerCount == 0)
        {
            Debug.Log("No player left");
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.JoinLobby();
        }
    }

    private void OnApplicationQuit()
    {
        if(!_gaming){
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.JoinLobby();
        }
    }



    public override void OnLeftRoom()
    {
        if(_gaming){
            _gaming = false;
            SceneManager.LoadScene(0);
            PhotonNetwork.AutomaticallySyncScene = true;
        }
        else{
            PhotonNetwork.JoinLobby();
        }
    }

    public List<RoomInfo> GetRoomInfos()
    {
        return _roomList;
    }

    public void JoinRoom(RoomInfo room)
    {
        //SetTeam("");
        PhotonNetwork.JoinRoom(room.Name);
    }

    public override void OnJoinedRoom()
    {
        userInfo.currentRoom = PhotonNetwork.CurrentRoom.Name;
        lobbyController.SetState("TeamSelect");


        foreach (Player player in PhotonNetwork.PlayerList)
        {
            teamUIController.OnTeamSelect(player);
        }
        //Debug.Log($"Red?:{PhotonNetwork.CurrentRoom.CustomProperties["Red"]}");
        if((bool)PhotonNetwork.CurrentRoom.CustomProperties["Red"] == false){
            SetTeam("Red");
        }
        else{
            SetTeam("Blue");
        }
    }

    private bool isRedListenerAdded = false;
    private bool isBlueListenerAdded = false;

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"failed to join room, available rooms: {PhotonNetwork.CountOfRooms}");
        PhotonNetwork.NetworkingClient.State = ClientState.ConnectedToMasterServer;
        PhotonNetwork.JoinLobby();
    }

    public void LeaveRoom()
    {
        userInfo.InitUserInfo();
        //lobbyController.SetState("TeamSelect");
        //방을 나가면서 방의 property에 선택한 팀이 refresh되도록
        //deselect 하라는 통신 이후에 team을 null로 바꿈
        if(!_gaming){
            teamUIController.SendMessage(2);
            if(GetTeam(PhotonNetwork.LocalPlayer) != "Null"){
                PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable{{GetTeam(PhotonNetwork.LocalPlayer), false}});
            }
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "team", "Null" } });
        PhotonNetwork.LeaveRoom();
        //PhotonNetwork.JoinLobby();
    }
    //---------------------------------------------------------------------------------------------------------------------
    public void SetTeam(string teamName)
    {
        // 플레이어의 Custom Properties에 "team" 키로 팀 정보 설정
        //ExitGames.Client.Photon.Hashtable previousPlayerTeam = new ExitGames.Client.Photon.Hashtable { { "previousTeam", GetTeam(PhotonNetwork.LocalPlayer) } };
        /*if(GetTeam(PhotonNetwork.LocalPlayer) == teamName){
            return;
        };*/
        teamUIController.DeselectTeam(PhotonNetwork.LocalPlayer);
        ExitGames.Client.Photon.Hashtable playerTeam = new ExitGames.Client.Photon.Hashtable { { "team", teamName } };

        if(GetTeam(PhotonNetwork.LocalPlayer) != "Null"){
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable{{GetTeam(PhotonNetwork.LocalPlayer), false}});
        }
        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable{{teamName, true}});

        PhotonNetwork.LocalPlayer.SetCustomProperties(playerTeam);
        //PhotonNetwork.LocalPlayer.SetCustomProperties(previousPlayerTeam);
        teamUIController.SendMessage(1);
        //master에게 팀 명단을 갱신하라는 rpc -> master에서 갱신 후 다른 클라이언트들에게 명단 갱신 명령
        Debug.Log($"Team set to: {teamName}");
    }

    // 플레이어의 팀 정보 확인 메서드
    public string GetTeam(Player player)
    {
        if (player.CustomProperties.TryGetValue("team", out object teamName))
        {
            return (string)teamName;
        }
        return "Null";
    }

    public string GetPreviousTeam(Player player)
    {
        if (player.CustomProperties.TryGetValue("previousTeam", out object teamName))
        {
            return (string)teamName;
        }
        return "Null";
    }

    // 모든 플레이어의 팀 정보를 출력
    public void PrintAllTeams()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            string team = GetTeam(player);
            Debug.Log($"Player {player.NickName} is on team {team}");
        }
    }
    //---------------------------------------------------------------------------------------------------------------------

    //방에 있는 유저들의 씬을 게임씬으로 변경
    public void StartGame()
    {
        if (PhotonNetwork.InRoom)
        {
            //_gaming = true;
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel("GameScene");
        }
        /*if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }*/
    }

    
}
