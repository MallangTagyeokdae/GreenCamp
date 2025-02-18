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
using Doozy.Runtime.UIManager.Containers;

public class LobbyController : MonoBehaviour
{
    public RoomInfo currentRoom;
    public ScrollRect scrollRect;
    public GameObject prefab;
    public Transform parent;
    private List<string> _roomList;
    private Dictionary<string, GameObject> _roomObjects;
    // Start is called before the first frame update
    public List<GameObject> pages;
    public GameObject nickName;
    private LobbyState state;
    void Start()
    {
        _roomList = new List<string>();
        _roomObjects = new Dictionary<string, GameObject>();
        //SetState("LogIn");
    }

    public void SetState(string stateName)
    {
        bool continueNext = true;
        if (this.state != null)
        {
            continueNext = this.state.Continue();
            if(continueNext){
                this.state.OutPage(stateName);
            }
        }

        if (continueNext)
        {
            switch (stateName)
            {
                case "Home": state = new Home(); break;
                case "Single": state = new Single(); break;
                case "Multi": state = new Multi(); break;
                case "Setting": state = new Setting(); break;
                case "TeamSelect": state = new TeamSelect(); break;
                case "LogIn": state = new LogIn(pages[0], pages[1], nickName); break;
            }
            this.state.InitPage();
        }
    }

    public void CreateRoom()
    {
    }

    public void updateRoomList(List<RoomInfo> roomList)
    {
        List<string> rooms = new List<string>();
        foreach (RoomInfo roomInfo in roomList)
        {
            rooms.Add(roomInfo.Name);

            GameObject room;

            if (!_roomList.Contains(roomInfo.Name))
            {
                room = Instantiate(prefab, parent);
                _roomObjects.Add(roomInfo.Name, room); //새로운 방이면 dictionary에 GameObject와 방 이름을 dictionary로 저장

                room.transform.Find("EnterBtn").GetComponent<UIButton>().pressedState.stateEvent.Event.AddListener(() =>
                {
                    PhotonManager.instance.JoinRoom(roomInfo);
                });
                room.transform.Find("EnterBtn").GetComponent<UIButton>().pressedState.stateEvent.Event.AddListener(() => { Debug.Log("test pressed button!"); });

                room.name = roomInfo.Name;
                string roomname = room.name.Substring(room.name.IndexOf("~") + 1);
                if (roomname == null)
                {
                    room.transform.Find("Title").gameObject.GetComponent<TMP_Text>().text = "";
                }
                else
                {
                    room.transform.Find("Title").gameObject.GetComponent<TMP_Text>().text = roomname;
                }
            }
            else
            {
                room = _roomObjects[roomInfo.Name]; //기존에 존재하는 방일 경우 room에 해당 방의 GameObject를 대입
            }

            room.transform.Find("UserCount").gameObject.GetComponent<TMP_Text>().text = roomInfo.PlayerCount + " / " + roomInfo.MaxPlayers; //방의 인원 수를 변경.


        } //rooms에 현재 남은 room의 list가 name으로 입력되어 있음

        foreach (string roomName in _roomList)
        {
            if (!rooms.Contains(roomName)) //_roomList에 존재하는데 rooms에 존재하지 않는 다면 더 이상 존재하지 않는 방임
            {
                GameObject go = parent.Find(roomName).gameObject;
                if (go != null)
                {
                    _roomObjects.Remove(roomName);
                    Destroy(go); // 해당 룸 네임을 가진 방을 파괴
                }
            }
        }
        _roomList = rooms;
    }

}
