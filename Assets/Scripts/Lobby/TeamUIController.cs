using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.UIManager.Components;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class TeamUIController : MonoBehaviour
{
    public GameObject BlueToggle;
    public GameObject BlueToggleImg;
    public GameObject BluePlayer;
    public GameObject RedToggle;
    public GameObject RedToggleImg;
    public GameObject RedPlayer;
    public void Init(Player player)
    {
        string playerTeam = PhotonManager.instance.GetTeam(player); //이전 팀으로 해야하는데,,?
        if (playerTeam == "Blue")
        {
            BlueToggle.GetComponent<UIToggle>().interactable = true;
            BlueToggleImg.SetActive(false);
            BluePlayer.transform.parent.gameObject.SetActive(false);
        }

        else if (playerTeam == "Red")
        {
            RedToggle.GetComponent<UIToggle>().interactable = true;
            RedToggleImg.SetActive(false);
            RedPlayer.transform.parent.gameObject.SetActive(false);
        }

        else
        {
            Debug.Log("No team selected");
        }
    }
    public void FirstTeamSelect(bool IsMasterClient)
    {
        if (IsMasterClient == true)
        {
            RedToggleImg.SetActive(true);
            RedToggle.GetComponent<UIToggle>().SetIsOn(true, true);
        }
        else
        {
            BlueToggleImg.SetActive(true);
            BlueToggle.GetComponent<UIToggle>().SetIsOn(true, true);
        }
    }
    public void OnTeamSelect(Player player)
    {
        string playerTeam = PhotonManager.instance.GetTeam(player);
        DeselectTeam(player);
        if (playerTeam == "Blue")
        {
            if (player != PhotonNetwork.LocalPlayer)
            {
                BlueToggle.GetComponent<UIToggle>().interactable = false;
                BlueToggleImg.SetActive(true);
            }
            BluePlayer.transform.parent.gameObject.SetActive(true);
            BluePlayer.GetComponent<TMP_Text>().text = player.NickName;
        }

        else if (playerTeam == "Red")
        {
            if (player != PhotonNetwork.LocalPlayer)
            {
                RedToggle.GetComponent<UIToggle>().interactable = false;
                RedToggleImg.SetActive(true);
            }
            RedPlayer.transform.parent.gameObject.SetActive(true);
            RedPlayer.GetComponent<TMP_Text>().text = player.NickName;
        }

        else
        {
            Debug.Log("No team selected");
        }
    }


    public void DeselectTeam(Player player)
    {
        //팀 선택 이전에 팀이 있었으면 처리할 것들
        string playerTeam = PhotonManager.instance.GetPreviousTeam(player); //이전 팀으로 해야하는데,,?
        if (playerTeam == "Blue")
        {
            if (player != PhotonNetwork.LocalPlayer)
            {
                BlueToggle.GetComponent<UIToggle>().interactable = true;
                BlueToggleImg.SetActive(false);
            }
            BluePlayer.GetComponent<TMP_Text>().text = player.NickName;
            BluePlayer.transform.parent.gameObject.SetActive(false);
        }

        else if (playerTeam == "Red")
        {
            if (player != PhotonNetwork.LocalPlayer)
            {
                RedToggle.GetComponent<UIToggle>().interactable = true;
                RedToggleImg.SetActive(false);
            }
            RedPlayer.GetComponent<TMP_Text>().text = player.NickName;
            RedPlayer.transform.parent.gameObject.SetActive(false);
        }

        else
        {
            Debug.Log("No team selected");
        }
    }

    public void SendTeamSelect()
    {
        Debug.Log("send check");
        byte eventCode = 1;
        object player = PhotonNetwork.LocalPlayer; //팀을 변경한 플레이어 정보를 모든 클라이언트에게 전송
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All }; //ALL, OTHERS, MASTER CLIENT 중에서 선택 (서버에 동기화되는 시점과 타이밍을 맞추기 위해서 ALL사용)
        SendOptions sendOptions = new SendOptions { Reliability = true }; // TCP, UDP 통신 중에서 선택
        PhotonNetwork.RaiseEvent(eventCode, player, options, sendOptions);
    }
}
