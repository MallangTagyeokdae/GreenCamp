using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro.Examples;
using Unity.VisualScripting;
//using ExitGames.Client.Photon;
//using Photon.Pun.Demo.Cockpit;

public class PhotonManager : MonoBehaviourPunCallbacks // 상속을 MonoBehaviour > MonoBehaviourPunCallbacks로 변경(MonoBehaviour에서 photon 관련 behavior가 추가된 버전)
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
    private string userId = "Charlie";
    public LobbyController lobbyController;
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
        //유저 아이디 할당
        PhotonNetwork.NickName = userId;
        //포톤 서버와 통신횟수 설정. 초당 30회
        Debug.Log(PhotonNetwork.SendRate);
        //서버 접속
        PhotonNetwork.ConnectUsingSettings();
    }

    //포톤 서버 접속 후 호출되는 콜백함수
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master!");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
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
        foreach(RoomInfo room in roomList){
            if(room.RemovedFromList){
                _roomList.Remove(room);
            }
            else if(_roomList.Contains(room)){
                continue;
            }
            else{
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
        Debug.Log($"join room: {PhotonNetwork.CurrentRoom.Name} Successed");
        Debug.Log("joined room title: " + PhotonNetwork.CurrentRoom.CustomProperties["Title"]);
        userInfo.currentRoom = PhotonNetwork.CurrentRoom.Name;
        if(!PhotonNetwork.IsMasterClient){
            lobbyController.SetState("TeamSelect");
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
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.JoinLobby();
    }
    //---------------------------------------------------------------------------------------------------------------------
    public void SetTeam(string teamName)
    {
        // 플레이어의 Custom Properties에 "team" 키로 팀 정보 설정
        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable { { "team", teamName } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);

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
        if(PhotonNetwork.InRoom){
            PhotonNetwork.LoadLevel("GameScene");
        }
        /*if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }*/
    }

}
