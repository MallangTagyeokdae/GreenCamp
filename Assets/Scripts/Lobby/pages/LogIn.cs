using System.Collections;
using System.Collections.Generic;
using Doozy.Runtime.UIManager.Containers;
using Photon.Pun;
using TMPro;
using UnityEditor.TestTools.CodeCoverage;
using UnityEngine;

public class LogIn : LobbyState
{
    [SerializeField]
    private readonly GameObject page;
    public TMP_InputField nickName;
    private GameObject _loginPage;
    private GameObject _afterLoginPage;
    

    public LogIn(GameObject loginPage, GameObject afterLoginPage, GameObject nickName){
        _loginPage = loginPage;
        _afterLoginPage = afterLoginPage;
        this.nickName = nickName.GetComponent<TMP_InputField>();
    }
    public void InitPage(){
        if(PhotonManager.instance.loggedin){
            _loginPage.SetActive(false);
            _afterLoginPage.GetComponent<UIContainer>().Show();
        }
        else{
            _loginPage.GetComponent<UIContainer>().Show();
        }
    }
    public void OutPage(string next){
        if(!PhotonManager.instance.session){
            PhotonManager.instance.session = true;
            PhotonManager.instance.ConnectGame(nickName.text);
        }
        _loginPage.GetComponent<UIContainer>().Hide();
        _afterLoginPage.GetComponent<UIContainer>().Show();
    }

    private bool LogInCheck(){
        if(nickName.text != ""){
            PhotonManager.instance.loggedin = true;
            return true;
        }
        else {
            return false;
        }
    }

    public bool Continue(){
        LogInCheck();
        return PhotonManager.instance.loggedin;
    }
}
