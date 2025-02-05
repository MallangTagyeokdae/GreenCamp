using System.Collections;
using System.Collections.Generic;
using Doozy.Runtime.UIManager.Components;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class TeamSelect : LobbyState
{
    private GameObject _uiGameObject = GameObject.Find("UI");
    private GameObject _teamSelectPage = GameObject.Find("SelectTeamUI");
    private GameObject _gameStartBtn;
    //private TMP_Text _RedTeam;
    //private TMP_Text _BlueTeam;
    public bool continueNext = true;
    public void InitPage()
    {
        _uiGameObject.SetActive(false);
        _gameStartBtn = _teamSelectPage.transform.Find("GameStartBtn").gameObject;
        //_RedTeam = GameObject.Find("BluePlayer").GetComponent<TMP_Text>();
        //_BlueTeam = GameObject.Find("RedPlayer").GetComponent<TMP_Text>();;
        if (PhotonNetwork.IsMasterClient)
        {
            _gameStartBtn.GetComponent<UIButton>().interactable = true;
        }
        else
        {
            _gameStartBtn.GetComponent<UIButton>().interactable = false;
        }
    }

    public void OutPage(string next)
    {
        _uiGameObject.SetActive(true);
    }

    public bool Continue(){
        return true;
    }

    /*public void SetRedTeam(string name){
        _RedTeam.text = name;
    }

    public void SetBlueTeam(string name){
        _BlueTeam.text = name;
    }*/
}