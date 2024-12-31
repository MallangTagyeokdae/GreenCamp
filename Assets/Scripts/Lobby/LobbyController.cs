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

public class LobbyController : MonoBehaviour
{
    public RoomInfo currentRoom;
    public UnityEvent loggedIn;
    public ScrollRect scrollRect;
    public GameObject prefab;
    public Transform parent;
    // Start is called before the first frame update

    public void LogIn(){
        loggedIn.Invoke();
    }

    public void updateRoomList(List<RoomInfo> roomList){
        foreach(RoomInfo roomInfo in roomList)
        {   

            GameObject room = Instantiate(prefab, parent);
            room.transform.Find("Title").gameObject.GetComponent<TextMeshPro>().text = roomInfo.Name;
        }
    }

}
