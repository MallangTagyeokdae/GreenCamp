using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro.Examples;

public class PhotonManager : MonoBehaviourPunCallbacks
{   
    private static PhotonManager _instance;
    public static PhotonManager instance{
        get{
            if(_instance == null){
                _instance = FindObjectOfType<PhotonManager>();
                return _instance;
            }
            return _instance;
        }
    }
    private List<RoomInfo> _roomList;// = new List<RoomInfo>();
    private readonly string version = "1.0f";
    private string userId = "Charlie";
    void Awake()
    {
        // 이 객체가 씬 전환 시 파괴되지 않도록 설정
        DontDestroyOnLoad(this.gameObject);
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
        //Debug.Log(userId);
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
    public override void OnJoinedLobby()
    {
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        _roomList = roomList;
        //base.OnRoomListUpdate(roomList);
    }

    public List<RoomInfo> GetRoomInfos(){
        return _roomList; 
    }

    public void JoinRoom(RoomInfo room){
        PhotonNetwork.JoinRoom(room.Name);
    }

    public void StartGame(){
        PhotonNetwork.LoadLevel("GameScene");
    }

}
