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
    private GameObject _blueToggle;
    private GameObject _redToggle;

    public bool continueNext = true;
    public void InitPage()
    {
        _uiGameObject.SetActive(false);
        _gameStartBtn = _teamSelectPage.transform.Find("GameStartBtn").gameObject;
        _blueToggle = _teamSelectPage.transform.Find("UIToggleGroup/BlueTeamToggle").gameObject;
        _redToggle = _teamSelectPage.transform.Find("UIToggleGroup/RedTeamToggle").gameObject;
        foreach (Transform child in _blueToggle.transform)
        {
            child.gameObject.SetActive(false);  // 자식 오브젝트 비활성화
        }
        foreach (Transform child in _redToggle.transform)
        {
            child.gameObject.SetActive(false);  // 자식 오브젝트 비활성화
        }
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

    public bool Continue()
    {
        return true;
    }

    /*public void SetRedTeam(string name){
        _RedTeam.text = name;
    }

    public void SetBlueTeam(string name){
        _BlueTeam.text = name;
    }*/
}