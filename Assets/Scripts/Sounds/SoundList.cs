using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundList", menuName = "SoundList")]
public class SoundList : ScriptableObject
{
    public List<AudioClip> audioList;   //Inspector창에 오디오 리스트를 추가
    private Dictionary<string, AudioClip> _audioClips;    //불러올 오디오를 맵으로 저장(key값은 파일명(string))
    public Dictionary<string, AudioClip> audioClips   //sound manager에서 사용할 오디오 맵 호출시 private 변수의 내용을 get해서 받아옴.
    {
        get
        {
            if (_audioClips == null || audioList.Count != _audioClips.Count)
            {
                _audioClips = Convert2Dictionary();
            }

            return _audioClips;
        }
    }

    private Dictionary<string, AudioClip> Convert2Dictionary()
    {
        Dictionary<string, AudioClip> l_audioMap = new Dictionary<string, AudioClip>();
        foreach (AudioClip clip in audioList)
        {
            if (l_audioMap.ContainsKey(clip.name))
                continue;

            l_audioMap.Add(clip.name, clip);
        }

        return l_audioMap;
    }
}
