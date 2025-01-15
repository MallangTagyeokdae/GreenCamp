using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OutGameState : MonoBehaviour
{
    public LobbyState lobbyState = LobbyState.Logout;
    public enum LobbyState
    {
        Logout,
        Login,
        JoinGame
    }
    
    public GameObject beforeSelectTeamUI;
    public GameObject selectTeamUI;

    public void SetLobbyState(string newState)
    {
        switch (newState)
        {
            case "Logout":
                lobbyState = LobbyState.Logout;
                beforeSelectTeamUI.SetActive(true);
                selectTeamUI.SetActive(false);
                break;
            case "Login":
                lobbyState = LobbyState.Login;
                beforeSelectTeamUI.SetActive(true);
                selectTeamUI.SetActive(false);
                break;
            case "JoinGame":
                lobbyState = LobbyState.JoinGame;
                beforeSelectTeamUI.SetActive(false);
                selectTeamUI.SetActive(true);
                break;
        }
    }
}
