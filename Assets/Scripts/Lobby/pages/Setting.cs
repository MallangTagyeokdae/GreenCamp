using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Setting : LobbyState{
    [SerializeField]
    private readonly GameObject page;
    private float _masterVol;
    private float _bgmVol;
    private float _effectVol;

    public void InitPage(){
    }
    public void OutPage(string next){

    }
}
