using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaySoundHandler : MonoBehaviour
{
    private GameObject masterSlider;
    private GameObject bgmSlider;
    private GameObject effectSlider;

    private void Awake() {
        masterSlider = SoundManager.instance.masterSlider;
        bgmSlider = SoundManager.instance.bgmSlider;
        effectSlider = SoundManager.instance.effectSlider;
    }


    public void PlaySound(string soundName){
        Debug.Log("check");
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

    public void SaveSoundSetting(){
        PlayerPrefs.SetFloat("bgmVolume", bgmSlider.GetComponent<Slider>().value);
        PlayerPrefs.SetFloat("effectVolume", effectSlider.GetComponent<Slider>().value);
        PlayerPrefs.SetFloat("masterVolume", masterSlider.GetComponent<Slider>().value);

        Debug.Log($"bgm: {bgmSlider.GetComponent<Slider>().value}");
        Debug.Log($"effect: {effectSlider.GetComponent<Slider>().value}");
        Debug.Log($"master: {masterSlider.GetComponent<Slider>().value}");
    }

    public void LoadSoundSetting(){
        bgmSlider.GetComponent<Slider>().value = PlayerPrefs.HasKey("bgmVolume")? PlayerPrefs.GetFloat("bgmVolume") : 1f;
        effectSlider.GetComponent<Slider>().value = PlayerPrefs.HasKey("effectVolume") ? PlayerPrefs.GetFloat("effectVolume") : 1f;
        masterSlider.GetComponent<Slider>().value = PlayerPrefs.HasKey("masterVolume") ? PlayerPrefs.GetFloat("masterVolume"): 1f;
    }
}
