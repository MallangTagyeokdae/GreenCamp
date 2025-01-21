using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    public static SoundManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SoundManager>();
            }

            return _instance;
        }
    }

    public SoundList soundList;

    public AudioClip bgmClip;
    private AudioSource _backGroundMusic;
    private List<AudioSource> _soundChannels;
    private float _bgmVolume = 0f;   //bgm 볼륨값
    private float _soundVolume = 0f; //효과음 볼륨값
    private bool _isMute = false;
    public int channelCount = 5;

    private void Awake()
    {

        _soundChannels = new List<AudioSource>();
        _backGroundMusic = gameObject.AddComponent<AudioSource>();
        _backGroundMusic.loop = true;
        for (int i = 0; i < channelCount; i++)
        {
            _soundChannels.Add(gameObject.AddComponent<AudioSource>());
        }

        PlayBackGroundMusic();
        DontDestroyOnLoad(this.gameObject);
    }

    void Start(){
        _bgmVolume = _backGroundMusic.volume;
        _soundVolume = _backGroundMusic.volume;
    }

    private int GetEmptyChannel()
    {
        int l_channel = -1;

        for (int i = 0; i < channelCount; i++)
        {
            if (_soundChannels[i].isPlaying == false)
            {
                l_channel = i;
                break;
            }
        }

        if (l_channel == -1)
        {
            _soundChannels.Add(gameObject.AddComponent<AudioSource>());
            l_channel = channelCount;
            channelCount++;
        }

        return l_channel;
    }

    public float PlaySound(string soundName)
    {
        if (soundList.audioClips.TryGetValue(soundName, out AudioClip l_audioClip))
        {
            int l_channel = GetEmptyChannel();
            _soundChannels[l_channel].clip = l_audioClip;
            _soundChannels[l_channel].Play();

            return _soundChannels[l_channel].clip.length;
        }

        else
        {
            Debug.Log("there is no such sound: " + soundName);
            return 0f;
        }
    }

    public void MuteAll()
    {
        for (int i = 0; i < channelCount; i++)
        {
            _soundChannels[i].volume = 0f;
        }
        _backGroundMusic.volume = 0f;
        _isMute = true;
    }

    public void ReleaseMute()
    {
        _isMute = false;
        for (int i = 0; i < channelCount; i++)
        {
            _soundChannels[i].volume = _soundVolume;
        }
        _backGroundMusic.volume = _bgmVolume;
    }

    public void SoundVolumeControl(float newVolume)
    {
        if (!_isMute)
        {
            for (int i = 0; i < channelCount; i++)
            {
                _soundChannels[i].volume = newVolume;
            }
        }
        _soundVolume = newVolume;
    }

    # region bgm
    public float PlayBackGroundMusic()
    {
        if (bgmClip != null)
        {
            _backGroundMusic.clip = bgmClip;
            Console.Write("check");
            _backGroundMusic.Play();
            return _backGroundMusic.clip.length;
        }

        else
        {
            Debug.Log("there is no such Music");
            return 0f;
        }
    }

    public void StopBackGroundMusic()
    {
        _backGroundMusic.Stop();
    }

    public void PauseBackGroundMusic(){
        _backGroundMusic.Pause();
    }
    public void UnpauseBackGroundMusic(){
        _backGroundMusic.UnPause();
    }

    public void BackGroundMusicVolumeControl(float newVolume)
    {
        if (!_isMute)
        {
            _backGroundMusic.volume = newVolume;
        }
        _bgmVolume = newVolume;
    }

    #endregion bgm
}

