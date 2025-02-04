using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public interface LobbyState
{
    public void InitPage();
    public void OutPage(string next);
    public bool Continue();
}