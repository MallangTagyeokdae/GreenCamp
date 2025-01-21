using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaySoundHandler : MonoBehaviour
{
    public GameObject masterSlider;
    public GameObject bgmSlider;
    public GameObject effectSlider;


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

    public void MasterVolumeControl(){
        float masterVolume = masterSlider.GetComponent<Slider>().value;
        SoundManager.instance.ApplyMasterVolume(masterVolume);
    }

}
