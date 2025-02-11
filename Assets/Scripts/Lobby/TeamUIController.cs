using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.UIManager.Components;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.StructWrapping;
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

    public void OnTeamSelect(Player player)
    {
        //DeselectTeam(player);
        Debug.Log($"이름: {player.NickName}, 팀: {PhotonManager.instance.GetTeam(player)}");
        string playerTeam = PhotonManager.instance.GetTeam(player);
        if(playerTeam == "Red"){
            ActiveRed(player);
        }
        else if(playerTeam == "Blue"){
            ActiveBlue(player);
        }
        else{
            Debug.Log("Error: 팀이 정의되지 않았습니다.");
        }
    }

    private void ActiveBlue(Player player)
    {
        //이름 띄우기
        BluePlayer.transform.parent.gameObject.SetActive(true);
        BluePlayer.GetComponent<TMP_Text>().text = player.NickName;
        
        if(player != PhotonNetwork.LocalPlayer){
            BlueToggle.GetComponent<UIToggle>().interactable = false;
            BlueToggleImg.SetActive(true);
        }
        else{
            BlueToggle.Select();
            BlueToggle.SetIsOn(true, false);
        }
    }
    private void ActiveRed(Player player)
    {
        //이름 띄우기
        RedPlayer.transform.parent.gameObject.SetActive(true);
        RedPlayer.GetComponent<TMP_Text>().text = player.NickName;
        
        if(player != PhotonNetwork.LocalPlayer){
            RedToggle.GetComponent<UIToggle>().interactable = false;
            RedToggleImg.SetActive(true);
        }
        else{
            RedToggle.Select();
            RedToggle.SetIsOn(true, false);
        }
    }

    public void SendTeamSelect()
    {
        byte eventCode = 1;
        object player = PhotonNetwork.LocalPlayer; //팀을 변경한 플레이어 정보를 모든 클라이언트에게 전송
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All }; //ALL, OTHERS, MASTER CLIENT 중에서 선택 (서버에 동기화되는 시점과 타이밍을 맞추기 위해서 ALL사용)
        SendOptions sendOptions = new SendOptions { Reliability = true }; // TCP, UDP 통신 중에서 선택
        PhotonNetwork.RaiseEvent(eventCode, player, options, sendOptions);
    }

    public void ChangeTeam(){
        byte eventCode = 2;
        object log = "Change Team";
        RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        SendOptions  sendOptions = new SendOptions {Reliability = true};
        PhotonNetwork.RaiseEvent(eventCode, log, options, sendOptions);
    }

    public void DeselectTeam(Player player){
        //Deselect시에 roomproperty false로 변경
        Debug.Log($"이름: {player.NickName}");
        if(PhotonManager.instance.GetTeam(player) == "Red"){
            RedPlayer.transform.parent.gameObject.SetActive(false);
            if(player != PhotonNetwork.LocalPlayer){
                RedToggle.GetComponent<UIToggle>().interactable = true;
                RedToggleImg.SetActive(false);
            }
        }
        else if(PhotonManager.instance.GetTeam(player) == "Blue"){
            BluePlayer.transform.parent.gameObject.SetActive(false);
            if(player != PhotonNetwork.LocalPlayer){
                BlueToggle.GetComponent<UIToggle>().interactable = true;
                BlueToggleImg.SetActive(false);
            }
        }
        else{
            Debug.Log("No team");
        }
    }
}