using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using Doozy.Runtime.UIManager.Listeners;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager;
using Photon.Pun;

public class LobbyController : MonoBehaviour
{
    public RoomInfo currentRoom;
    public UnityEvent loggedIn;
    public ScrollRect scrollRect;
    public GameObject prefab;
    public Transform parent;
    private List<string> _roomList;
    // Start is called before the first frame update
    private LobbyState state;
    void Start()
    {
        _roomList = new List<string>();
        state = new Home();
    }

    public void LogIn()
    {
        loggedIn.Invoke();
        //outGameState.SetLobbyState("Login");
    }

    public void SetState(string stateName){
        this.state.OutPage(stateName);
        switch (stateName){
            case "Home": state = new Home(); break;
            case "Single": state = new Single(); break;
            case "Multi": state = new Multi(); break;
            case "Setting": state = new Setting(); break;
            case "TeamSelect": state = new TeamSelect(); break;
        }
        this.state.InitPage();
    }

    public void CreateRoom(){
    }

    public void updateRoomList(List<RoomInfo> roomList)
    {
        List<string> rooms = new List<string>();
        foreach (RoomInfo roomInfo in roomList)
        {
            rooms.Add(roomInfo.Name);
            if (!_roomList.Contains(roomInfo.Name))
            {
                GameObject room = Instantiate(prefab, parent);
                room.transform.Find("EnterBtn").GetComponent<UIButton>().pressedState.stateEvent.Event.AddListener(() =>
                {
                    PhotonManager.instance.JoinRoom(roomInfo);
                });
                room.transform.Find("EnterBtn").GetComponent<UIButton>().pressedState.stateEvent.Event.AddListener(() => { Debug.Log("test pressed button!"); });

                room.name = roomInfo.Name;
                string roomname = room.name.Substring(room.name.IndexOf("~") + 1);
                if(roomname == null){
                    room.transform.Find("Title").gameObject.GetComponent<TMP_Text>().text = "";
                }
                else{
                    room.transform.Find("Title").gameObject.GetComponent<TMP_Text>().text = roomname;
                }
            }
        } //rooms에 현재 남은 room의 list가 name으로 입력되어 있음

        foreach (string roomName in _roomList)
        {
            if (!rooms.Contains(roomName)) //_roomList에 존재하는데 rooms에 존재하지 않는 다면 더 이상 존재하지 않는 방임
            {
                GameObject go = parent.Find(roomName).gameObject;
                if (go != null)
                {
                    Destroy(go); // 해당 룸 네임을 가진 방을 파괴
                }
            }
        }
        _roomList = rooms;
    }

}
