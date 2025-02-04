using System.Collections;
using System.Collections.Generic;
using Doozy.Runtime.UIManager.Components;
using Doozy._Examples.Common;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class Multi : LobbyState
{
    private GameObject _popUp;
    private TMP_InputField _roomName;
    private UIButton _createRoomBtn;
    private Transform parent;
    public bool conitinueNext = true;

    public void InitPage()
    { //하드코딩했음,,,
        GameObject go = GameObject.Find("MultiModePage");
        parent = go.transform;
        _popUp = parent.Find("CreateRoomArea").gameObject;
        _roomName = _popUp.transform.Find("RoomNameField").GetComponent<TMP_InputField>();
        _roomName.text = "";
        _createRoomBtn = _popUp.transform.Find("CreateRoomBtn").GetComponent<UIButton>();
        _createRoomBtn.onClickEvent.AddListener(CreateRoom);

        _popUp.SetActive(false);
    }
    void CreateRoom()
    {
        if (PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.JoinedLobby && _createRoomBtn.isSelected)

            PhotonManager.instance.CreateRoom(_roomName.text);
    }
    
    public void OutPage(string next)
    {
    }

    public bool Continue(){
        return true;
    }
}
