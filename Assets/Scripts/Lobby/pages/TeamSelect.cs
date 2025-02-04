using System.Collections;
using System.Collections.Generic;
using Doozy.Runtime.UIManager.Components;
using Photon.Pun;
using UnityEngine;

public class TeamSelect : LobbyState
{
    private GameObject _uiGameObject = GameObject.Find("UI");
    private GameObject _teamSelectPage = GameObject.Find("SelectTeamUI");
    private GameObject _gameStartBtn;
    public bool conitinueNext = true;
    public void InitPage()
    {
        _uiGameObject.SetActive(false);
        _gameStartBtn = _teamSelectPage.transform.Find("GameStartBtn").gameObject;
        if (PhotonNetwork.IsMasterClient)
        {
            // Debug.Log("이새끼가실행돼야되는데");
            _gameStartBtn.GetComponent<UIButton>().interactable = true;
        }
        else
        {
            // Debug.Log("왜이새끼가실행되지");
            _gameStartBtn.GetComponent<UIButton>().interactable = false;
        }
        //Debug.Log(_teamSelectPage.name);
        //_teamSelectPage.SetActive(true);
    }
    public void OutPage(string next)
    {
        _uiGameObject.SetActive(true);
        // _teamSelectPage.SetActive(false);
    }
    public bool Continue(){
        return true;
    }
}
