using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

/*public interface LobbyState
{
    public void initPage(){}*/
    /*public enum LobbyState
    {
        Null = 0,
        Home = 1,
        Single = 2,
        Multi = 3,
        Setting = 4,
        TeamSelect = 5
    }

    private LobbyState lobbyState = LobbyState.Null;
    
    public GameObject beforeSelectTeamUI;
    public GameObject selectTeamUI;

    public List<GameObject> pages;

    public void SetLobbyState(string newState)
    {
        switch (newState)
        {
            case "Home":
                lobbyState = LobbyState.Home;
                break;
            case "Single":
                lobbyState = LobbyState.Single;
                break;
            case "Multi":
                lobbyState = LobbyState.Multi;
                //초기 화면으로 재구성
                beforeSelectTeamUI.SetActive(true);
                selectTeamUI.SetActive(false);
                break;
            case "Setting":
                lobbyState = LobbyState.Setting;
                beforeSelectTeamUI.SetActive(false);
                selectTeamUI.SetActive(true);
                break;
            case "TeamSelect":
                lobbyState = LobbyState.TeamSelect;
                break;
        }
    }*/
/*}

public class Home : MonoBehaviour, LobbyState{
    [SerializeField]
    private readonly GameObject page;
    public void initPage(){

    }
}

public class Single : MonoBehaviour, LobbyState{
    [SerializeField]
    private readonly GameObject page;
    public void initPage(){

    }
}

public class Multi : MonoBehaviour, LobbyState{
    [SerializeField]
    private GameObject popUp;
    private TMP_Text tMP_Text;
    public void initPage(){
        popUp.SetActive(false);
        tMP_Text.text = "";
    }
}

public class Setting : MonoBehaviour, LobbyState{
    [SerializeField]
    private readonly GameObject page;
    public void initPage(){

    }
}

public class TeamSelect : MonoBehaviour, LobbyState{
    [SerializeField]
    private readonly GameObject page;
    public void initPage(){

    }
}*/