using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
    public List<KeyEvent> keyEvents;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(KeyEvent keyEvent in keyEvents){
            if(Input.GetKeyDown(keyEvent.keyCode)){
                keyEvent.onKeyPressed?.Invoke();
            }
        }
        
    }
}
