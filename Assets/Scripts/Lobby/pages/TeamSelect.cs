using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamSelect : LobbyState
{
    private GameObject _uiGameObject = GameObject.Find("UI");
    private GameObject _teamSelectPage = GameObject.Find("SelectTeamUI");
    public void InitPage()
    {
        _uiGameObject.SetActive(false);
        //Debug.Log(_teamSelectPage.name);
        //_teamSelectPage.SetActive(true);
    }
    public void OutPage(string next)
    {
        _uiGameObject.SetActive(true);
        // _teamSelectPage.SetActive(false);
    }
}
