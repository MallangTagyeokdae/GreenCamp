using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundHandler : MonoBehaviour
{
    public void PlaySound(string soundName){
        SoundManager.instance.PlaySound(soundName);
    }
}
