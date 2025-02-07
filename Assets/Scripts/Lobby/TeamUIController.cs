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
    public void OnTeamSelect(Player player, bool IsMasterClient)
    {
        string playerTeam = PhotonManager.instance.GetTeam(player);
        DeselectTeam(player, IsMasterClient);
        if (playerTeam == "Blue" && BlueToggleImg.activeSelf == false)
        {
            if (player != PhotonNetwork.LocalPlayer)
            {
                BlueToggleImg.SetActive(true);
                Debug.Log("OnSelect 함수: 로컬 플레이어가 아님 -> 블루 이미지 켜기");
            }
            BluePlayer.transform.parent.gameObject.SetActive(true);
            BluePlayer.GetComponent<TMP_Text>().text = player.NickName;
        }

        else if (playerTeam == "Red" && RedToggleImg.activeSelf == false)
        {
            if (player != PhotonNetwork.LocalPlayer)
            {
                RedToggleImg.SetActive(true);
                Debug.Log("OnSelect 함수: 로컬 플레이어가 아님 -> 레드 이미지 켜기");
            }
            RedPlayer.transform.parent.gameObject.SetActive(true);
            RedPlayer.GetComponent<TMP_Text>().text = player.NickName;
        }

        else if (playerTeam == "null")
        {
            BlueToggleImg.SetActive(false);
            BluePlayer.transform.parent.gameObject.SetActive(false);
            RedToggleImg.SetActive(false);
            RedPlayer.transform.parent.gameObject.SetActive(false);
        }
    }


    public void DeselectTeam(Player player, bool IsMasterClient)
    {
        //팀 선택 이전에 팀이 있었으면 처리할 것들
        string playerPreviousTeam = PhotonManager.instance.GetPreviousTeam(player); //이전 팀으로 해야하는데,,?

        if (playerPreviousTeam == "Blue")
        {
            if (player != PhotonNetwork.LocalPlayer)
            {
                // BlueToggle.GetComponent<UIToggle>().interactable = true;
                BlueToggleImg.SetActive(false);
            }
            // BluePlayer.GetComponent<TMP_Text>().text = player.NickName;
            BluePlayer.transform.parent.gameObject.SetActive(false);
        }

        else if (playerPreviousTeam == "Red")
        {
            if (player != PhotonNetwork.LocalPlayer)
            {
                // RedToggle.GetComponent<UIToggle>().interactable = true;
                RedToggleImg.SetActive(false);
            }
            // RedPlayer.GetComponent<TMP_Text>().text = player.NickName;
            RedPlayer.transform.parent.gameObject.SetActive(false);
        }

        else
        {
            Debug.Log("Deselct 함수: 아무것도 선택 안 함");

            if (IsMasterClient)
            {
                if (player != PhotonNetwork.LocalPlayer)
                {
                    RedToggleImg.SetActive(true);
                    Debug.Log("Deselect 함수: 로컬 플레이어가 아님 -> 레드 이미지 켜기");
                }
                RedPlayer.transform.parent.gameObject.SetActive(true);
                RedToggle.GetComponent<UIToggle>().Select();
                RedToggle.GetComponent<UIToggle>().SetIsOn(true, true);
                RedPlayer.GetComponent<TMP_Text>().text = player.NickName;
            }
            else
            {
            }
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
