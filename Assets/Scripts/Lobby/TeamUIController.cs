using System.Collections;
using System.Collections.Generic;
using Doozy.Runtime.UIManager.Components;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class TeamUIController : MonoBehaviour
{
    private string _playerTeam;
    public GameObject BlueBtn;
    public GameObject BluePlayer;
    public GameObject RedBtn;
    public GameObject RedPlayer;

    public void test1(){
        //OnTeamSelect(PhotonNetwork.LocalPlayer);
    }
    public void test2(){
        DeselectTeam(PhotonNetwork.LocalPlayer);
    }

    public void OnTeamSelect(Player player, string team){
        DeselectTeam(player);
        _playerTeam = team;
        Debug.Log($"디자ㅓㅊ마ㅣㅓㄴ어ㅣㅏ미나엎: {_playerTeam}");
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
