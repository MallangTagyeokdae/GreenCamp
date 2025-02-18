using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UserInfo", menuName = "UserInfo")]
public class UserInfo : ScriptableObject
{
    public string user;
    public string currentRoom;
    
    public void InitUserInfo(){
        currentRoom = null;
    }
}
