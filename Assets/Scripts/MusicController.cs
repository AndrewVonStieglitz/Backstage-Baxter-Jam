using System.Collections;
using System.Collections.Generic;
using Cables;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField] private List<AudioClip> musicList = new List<AudioClip>();

    private void OnEnable()
    {
        GameEvents.onCableConnect += PlayMusic;
        GameEvents.onCableDisconnect += StopMusic;
    }

    private void OnDisable()
    {
        GameEvents.onCableConnect -= PlayMusic;
        GameEvents.onCableDisconnect -= StopMusic;
    }

    private void PlayMusic(CableController cable, SpeakerController speaker)
    {
        if (cable.AmpID <= musicList.Count && cable.AmpID >= 0)
        {
            speaker.PlayMusic(musicList[cable.AmpID - 1], cable.AmpID);
        }
        else
        {
            Debug.Log("ID is invalid. Song may not be added to MusicController's musicList");
        }
    }

    private void StopMusic(CableController cable, SpeakerController speaker)
    {
        speaker.StopMusic();
    }
}
