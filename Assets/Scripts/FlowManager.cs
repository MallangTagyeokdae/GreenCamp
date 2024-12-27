using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class FlowManager : MonoBehaviour
{
    bool isLogin;
    void Start() {
        isLogin = false;
    }

    public void Go2AfterLoginScene() {
        isLogin = true;
    }
    public void GameExit(){
        isLogin = false;
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
