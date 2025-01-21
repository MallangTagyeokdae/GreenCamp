using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundHandler : MonoBehaviour
{
    public void PlaySound(string soundName){
        SoundManager.instance.PlaySound(soundName);
    }

    public void MuteAll()
    {
        SoundManager.instance.MuteAll();
    }

    public void ReleaseMute()
    {
        SoundManager.instance.ReleaseMute();
    }

}
