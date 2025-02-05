using System.Collections;
using System.Collections.Generic;
using Doozy.Runtime.UIManager.Components;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class TeamUIController : MonoBehaviour
{
    private string _playerTeam;
    public GameObject BlueBtn;
    public GameObject BluePlayer;
    public GameObject RedBtn;
    public GameObject RedPlayer;

    public void OnTeamSelect(Player player){
        DeselectTeam(player);
        _playerTeam = PhotonManager.instance.GetTeam(player);
        if(_playerTeam == "Blue"){
            if(player != PhotonNetwork.LocalPlayer){
                BlueBtn.GetComponent<UIButton>().interactable = false;
            }
            BluePlayer.transform.parent.gameObject.SetActive(true);
            BluePlayer.GetComponent<TMP_Text>().text = player.NickName;
        }

        else if(_playerTeam == "Red"){
            if(player != PhotonNetwork.LocalPlayer){
                RedBtn.GetComponent<UIButton>().interactable = false;
            }
            RedPlayer.transform.parent.gameObject.SetActive(true);
            RedPlayer.GetComponent<TMP_Text>().text = player.NickName;
        }

        else{
            Debug.Log("No team selected");
        }
    }

    public void SendTeamSelect(){
        byte eventCode = 1;
        object player = PhotonNetwork.LocalPlayer; //팀을 변경한 플레이어 정보를 모든 클라이언트에게 전송
        RaiseEventOptions options = new RaiseEventOptions {Receivers = ReceiverGroup.All}; //ALL, OTHERS, MASTER CLIENT 중에서 선택 (서버에 동기화되는 시점과 타이밍을 맞추기 위해서 ALL사용)
        SendOptions sendOptions = new SendOptions{Reliability = true}; // TCP, UDP 통신 중에서 선택
        PhotonNetwork.RaiseEvent(eventCode, player, options, sendOptions);
    }

    public void DeselectTeam(Player player){
        //팀 선택 이전에 팀이 있었으면 처리할 것들
        if(_playerTeam == "Blue"){
            if(player != PhotonNetwork.LocalPlayer){
                BlueBtn.GetComponent<UIButton>().interactable = true;
            }
            BluePlayer.GetComponent<TMP_Text>().text = player.NickName;
            BluePlayer.transform.parent.gameObject.SetActive(false);
        }

        else if(_playerTeam == "Red"){
            if(player != PhotonNetwork.LocalPlayer){
                RedBtn.GetComponent<UIButton>().interactable = true;
            }
            RedPlayer.GetComponent<TMP_Text>().text = player.NickName;
            RedPlayer.transform.parent.gameObject.SetActive(false);
        }

        else{
            Debug.Log("No team selected");
        }
    }
}
