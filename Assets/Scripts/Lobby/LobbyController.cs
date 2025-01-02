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

public class LobbyController : MonoBehaviour
{
    public RoomInfo currentRoom;
    public UnityEvent loggedIn;
    public ScrollRect scrollRect;
    public GameObject prefab;
    public Transform parent;
    private List<string> _roomList;
    // Start is called before the first frame update
    void Start()
    {
        _roomList = new List<string>();
    }

    public void LogIn()
    {
        loggedIn.Invoke();
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
                room.transform.Find("EnterBtn").GetComponent<UIButton>().pressedState.stateEvent.Event.AddListener(() => PhotonManager.instance.JoinRoom(roomInfo));
                room.name = roomInfo.Name;
                Debug.Log(room.transform.Find("Title").gameObject);//.GetComponent<TextMeshPro>().text = roomInfo.Name;
            }
        }

        foreach (string roomName in _roomList)
        {
            if (!rooms.Contains(roomName))
            {
                GameObject go = parent.Find($"{roomName}").gameObject;
                if (go != null)
                {
                    Destroy(go); // 해당 룸 네임을 가진 방을 파괴
                }
            }
        }
        _roomList = rooms;
    }

}
