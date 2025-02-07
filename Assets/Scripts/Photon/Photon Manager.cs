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
    void Awake()
    {
        // 이 객체가 씬 전환 시 파괴되지 않도록 설정
        DontDestroyOnLoad(this.gameObject);
        _roomList = new List<RoomInfo>();

    }

    // Start is called before the first frame update
    void Start()
    {
        //같은 룸의 유저들에게 자동으로  씬을 로딩
        PhotonNetwork.AutomaticallySyncScene = true;
        //같은 버전의 유저끼리 접속을 허용
        PhotonNetwork.GameVersion = version;
        //포톤 서버와 통신횟수 설정. 초당 30회
        Debug.Log(PhotonNetwork.SendRate);
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

        lobbyController.updateRoomList(_roomList);
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


        if (PhotonNetwork.CreateRoom(roomName + "~" + roomTitle, roomOptions))
        {
            Debug.Log("Created room with name: " + roomName + roomTitle);
        }
    }

    //player가 방을 떠날 때 콜백되는 함수
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        lobbyController.SetState("TeamSelect");
        SetTeam("Red");

        if (PhotonNetwork.CurrentRoom.PlayerCount == 0)
        {
            Debug.Log("No player left");
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.JoinLobby();
        }
    }

    private void OnApplicationQuit()
    {
        Debug.Log($"check leave 1 {PhotonNetwork.CountOfRooms}");
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.JoinLobby();
        Debug.Log($"check leave 2 {PhotonNetwork.CountOfRooms}");
    }



    /*public override void OnLeftRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 0)
        {
            Debug.Log("No player left");
            PhotonNetwork.LeaveRoom();
        }
    }*/

    public List<RoomInfo> GetRoomInfos()
    {
        return _roomList;
    }

    public void JoinRoom(RoomInfo room)
    {
        PhotonNetwork.JoinRoom(room.Name);
    }

    public override void OnJoinedRoom()
    {
        userInfo.currentRoom = PhotonNetwork.CurrentRoom.Name;
        AddCallbackFunc();
        lobbyController.SetState("TeamSelect");
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                if (GetTeam(PhotonNetwork.MasterClient) == "Red")
                {
                    SetTeam("Blue");
                }
                else
                {
                    SetTeam("Red");
                }
            }
            teamUIController.OnTeamSelect(player, PhotonNetwork.IsMasterClient);
        }
    }
    private bool isRedListenerAdded = false;
    private bool isBlueListenerAdded = false;

    public void AddCallbackFunc()
    {
        RemoveCallbackFunc();
        if (!isRedListenerAdded)
        {
            teamUIController.RedToggle.onClickEvent.AddListener(() => SetTeam("Red"));
            isRedListenerAdded = true;
        }

        if (!isBlueListenerAdded)
        {
            teamUIController.BlueToggle.onClickEvent.AddListener(() => SetTeam("Blue"));
            isBlueListenerAdded = true;
        }
    }

    public void RemoveCallbackFunc()
    {
        if (isRedListenerAdded)
        {
            teamUIController.RedToggle.onClickEvent.RemoveListener(() => SetTeam("Red"));
            isRedListenerAdded = false;
        }

        if (isBlueListenerAdded)
        {
            teamUIController.BlueToggle.onClickEvent.RemoveListener(() => SetTeam("Blue"));
            isBlueListenerAdded = false;
        }
    }



    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"failed to join room, available rooms: {PhotonNetwork.CountOfRooms}");
        PhotonNetwork.NetworkingClient.State = ClientState.ConnectedToMasterServer;
        PhotonNetwork.JoinLobby();
    }

    public void LeaveRoom()
    {
        userInfo.InitUserInfo();
        lobbyController.SetState("TeamSelect");
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.JoinLobby();
    }
    //---------------------------------------------------------------------------------------------------------------------
    public void SetTeam(string teamName)
    {
        // 플레이어의 Custom Properties에 "team" 키로 팀 정보 설정
        // ExitGames.Client.Photon.Hashtable previousPlayerTeam = new ExitGames.Client.Photon.Hashtable { { "previousTeam", GetTeam(PhotonNetwork.LocalPlayer) } };
        ExitGames.Client.Photon.Hashtable playerTeam = new ExitGames.Client.Photon.Hashtable { { "team", teamName } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerTeam);
        // PhotonNetwork.LocalPlayer.SetCustomProperties(previousPlayerTeam);
        teamUIController.SendTeamSelect();
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
            PhotonNetwork.LoadLevel("GameScene");
        }
        /*if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }*/
    }

    //----------------------------------------------------------------------------------------------------------------------
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
        switch (photonEvent.Code)
        {
            case 1: // 팀 선택 이벤트
                Player player = (Player)photonEvent.CustomData;
                teamUIController.OnTeamSelect(player, PhotonNetwork.IsMasterClient);
                break;

            default:
                //Debug.Log("Unknown event received: " + photonEvent.Code);
                break;
        }
    }
}
