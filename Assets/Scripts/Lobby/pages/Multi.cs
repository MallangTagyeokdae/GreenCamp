using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Multi :MonoBehaviour, LobbyState{
    private GameObject popUp;
    private TMP_Text roomName;
    private Transform parent;

    public void InitPage(){ //하드코딩했음,,,
        GameObject go = GameObject.Find("MultiModePage");
        parent = go.transform;
        popUp = parent.Find("CreateRoomArea").gameObject;
        roomName = popUp.transform.Find("RoomNameField").Find("Text Area").Find("Text").GetComponent<TMP_Text>();
        popUp.SetActive(false);
        roomName.text = "";
    }
    public void OutPage(string next){
        if(next == "TeamSelect"){
            PhotonManager.instance.CreateRoom(roomName.text);
        }
    }
}
