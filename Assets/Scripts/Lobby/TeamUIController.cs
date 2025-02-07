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
    public UIToggle BlueToggle;
    public GameObject BlueToggleImg;
    public GameObject BluePlayer;
    public UIToggle RedToggle;
    public GameObject RedToggleImg;
    public GameObject RedPlayer;

    public void OnTeamSelect(Player player, bool IsMasterClient)
    {
        string playerTeam = PhotonManager.instance.GetTeam(player);
        // DeselectTeam(player, IsMasterClient);
        Debug.Log("현재 팀: " + playerTeam);
        if (playerTeam == "Blue" || playerTeam == "")
        {
            if (IsMasterClient == true)
            {
                if (player == PhotonNetwork.LocalPlayer)
                {
                    ActiveBlue(player);
                }
                else
                {
                    ActiveRed(player);
                }
            }

        }
        else
        {

            if (IsMasterClient == true)
            {
                if (player == PhotonNetwork.LocalPlayer)
                {
                    ActiveRed(player);
                }
                else
                {
                    ActiveBlue(player);
                }
            }
        }
    }

    private void ActiveBlue(Player player)
    {

        BluePlayer.transform.parent.gameObject.SetActive(true);
        BlueToggle.Select();
        BlueToggle.SetIsOn(true, true);
        BluePlayer.GetComponent<TMP_Text>().text = player.NickName;

        if (player != PhotonNetwork.LocalPlayer)
        {
            if (RedToggleImg.activeSelf)
            {
                RedToggleImg.SetActive(false);
            }
            BlueToggleImg.SetActive(true);
            Debug.Log("로컬 사용자가 아님 -> 블루 이미지 켜기");
        }
    }
    private void ActiveRed(Player player)
    {

        RedPlayer.transform.parent.gameObject.SetActive(true);
        RedToggle.Select();
        RedToggle.SetIsOn(true, true);
        RedPlayer.GetComponent<TMP_Text>().text = player.NickName;
        if (player != PhotonNetwork.LocalPlayer)
        {
            if (BlueToggleImg.activeSelf)
            {
                BlueToggleImg.SetActive(false);
            }
            RedToggleImg.SetActive(true);
            Debug.Log("로컬 사용자가 아님 -> 블루 이미지 켜기");
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



// public void DeselectTeam(Player player, bool IsMasterClient)
// {
//     //팀 선택 이전에 팀이 있었으면 처리할 것들
//     string playerPreviousTeam = PhotonManager.instance.GetPreviousTeam(player); //이전 팀으로 해야하는데,,?
//     Debug.Log("이전 팀: " + playerPreviousTeam);

//     if (playerPreviousTeam == "Blue")
//     {
//         if (player != PhotonNetwork.LocalPlayer)
//         {
//             // BlueToggle.interactable = true;
//             BlueToggleImg.SetActive(false);
//         }
//         // BluePlayer.GetComponent<TMP_Text>().text = player.NickName;
//         BluePlayer.transform.parent.gameObject.SetActive(false);
//     }

//     else if (playerPreviousTeam == "Red")
//     {
//         if (player != PhotonNetwork.LocalPlayer)
//         {
//             // RedToggle.interactable = true;
//             RedToggleImg.SetActive(false);
//         }
//         // RedPlayer.GetComponent<TMP_Text>().text = player.NickName;
//         RedPlayer.transform.parent.gameObject.SetActive(false);
//     }

//     else
//     {
//         Debug.Log("Deselct 함수: 아무것도 선택 안 함");

//         if (IsMasterClient)
//         {
//             if (player != PhotonNetwork.LocalPlayer)
//             {
//                 RedToggleImg.SetActive(true);
//                 Debug.Log("Deselect 함수: 로컬 플레이어가 아님 -> 레드 이미지 켜기");
//             }
//             RedPlayer.transform.parent.gameObject.SetActive(true);
//             RedToggle.Select();
//             RedToggle.SetIsOn(true, true);
//             RedPlayer.GetComponent<TMP_Text>().text = player.NickName;
//         }
//     }
// }