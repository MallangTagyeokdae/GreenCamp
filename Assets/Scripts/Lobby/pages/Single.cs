using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Single : LobbyState{
    [SerializeField]
    private readonly GameObject page;
    public bool conitinueNext = true;
    public void InitPage(){

    }
    public void OutPage(string next){}
    public bool Continue(){
        return true;
    }
}
