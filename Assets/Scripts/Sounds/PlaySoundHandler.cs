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

    public void MasterVolumeControl(){
        float masterVolume = masterSlider.GetComponent<Slider>().value;
        SoundManager.instance.ApplyMasterVolume(masterVolume);
    }

    public void MuteAll()
    {
        SoundManager.instance.MuteAll();
    }

    public void ReleaseMute()
    {
        SoundManager.instance.ReleaseMute();
    }

    public void SoundVolumeControl()
    {
        float effectVolume = effectSlider.GetComponent<Slider>().value;
        SoundManager.instance.SoundVolumeControl(effectVolume);
    }

    public void MuteSound()
    {
        SoundManager.instance.MuteSound();
    }

    public void ReleaseSoundMute()
    {
        SoundManager.instance.ReleaseSoundMute();
    }

    public void BackGroundMusicVolumeControl()
    {
        float bgmVolume = bgmSlider.GetComponent<Slider>().value;
        SoundManager.instance.BackGroundMusicVolumeControl(bgmVolume);
    }

    public void MuteBackGroundMusic()
    {
        SoundManager.instance.MuteBackGroundMusic();
    }

    public void ReleaseBackGroundMusicMute()
    {
        SoundManager.instance.ReleaseBackGroundMusicMute();
    }

    

}
