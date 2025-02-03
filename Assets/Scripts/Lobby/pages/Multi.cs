using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class Multi : LobbyState
{
    private GameObject popUp;
    private TMP_InputField roomName;
    private Transform parent;

    public void InitPage()
    { //하드코딩했음,,,
        GameObject go = GameObject.Find("MultiModePage");
        parent = go.transform;
        popUp = parent.Find("CreateRoomArea").gameObject;
        roomName = popUp.transform.Find("RoomNameField").GetComponent<TMP_InputField>();
        roomName.text = "";
        popUp.SetActive(false);
    }
    public void OutPage(string next)
    {
        if (next == "TeamSelect" && PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.JoinedLobby)
        {
            PhotonManager.instance.CreateRoom(roomName.text);
        }
    }
}
