using System.Collections;
using System.Collections.Generic;
using Doozy.Runtime.UIManager.Containers;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class LogIn : LobbyState
{
    [SerializeField]
    private readonly GameObject page;
    public TMP_InputField nickName;
    private GameObject loginPage;
    private GameObject homePage;
    public bool conitinueNext = false;
    public void InitPage(){
        loginPage = GameObject.Find("LoginPage");
        homePage = GameObject.Find("AfterLoginPage");
        nickName = GameObject.Find("UserInfo").GetComponent<TMP_InputField>();
        Debug.Log($"check {nickName.name}");
    }
    public void OutPage(string next){
        PhotonManager.instance.ConnectGame(nickName.text);
        loginPage.GetComponent<UIContainer>().Hide();
        homePage.GetComponent<UIContainer>().Show();
    }

    private bool LogInCheck(){
        if(nickName.text != ""){
            return true;
        }
        else {
            return false;
        }
    }

    public bool Continue(){
        return LogInCheck();
    }
}
