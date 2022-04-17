using System.Collections;
using System.Collections.Generic;
using Cables;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField] private List<AudioClip> musicList = new List<AudioClip>();
    private float timer;

    private void OnEnable()
    {
        GameEvents.onCableConnect += PlayMusic;
        //GameEvents.onCableDisconnect += StopMusic;
    }

    private void OnDisable()
    {
        GameEvents.onCableConnect -= PlayMusic;
        //GameEvents.onCableDisconnect -= StopMusic;
    }

    private void Update()
    {
        if (GameManager.CurrentGameState == GameManager.GameState.playing )
        {
            timer += Time.deltaTime;
        }

    }

    private void PlayMusic(CableController cable, SpeakerController speaker)
    {
        var ampID = cable.amp.AmpID;
        
        if (ampID <= musicList.Count && ampID >= 0)
        {
            speaker.PlayMusic(musicList[ampID], ampID);
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
