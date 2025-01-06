using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class KeyEvent
{
    public KeyCode keyCode;  // 키 코드
    public UnityEvent onKeyPressed;  // 키가 눌렸을 때 실행될 콜백 함수
}