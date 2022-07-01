// DEPRICATED
using System.Collections.Generic;
using Cables;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField] private List<AudioClip> musicList = new List<AudioClip>();
    private float timer;

    private bool timerActive;

    //IDictionary<recipe, SpeakerController>

    private void OnEnable()
    {
        StartTimer();
        GameEvents.onGameStart += StartTimer;
        //GameEvents.onCableConnect += PlayMusic; // DEPRICATED
        GameEvents.onStartSong += ResetTimer;
        //GameEvents.onCableDisconnect += StopMusic;
    }

    private void OnDisable()
    {
        GameEvents.onGameStart -= StartTimer;
        //GameEvents.onCableConnect -= PlayMusic; // DEPRICATED
        GameEvents.onStartSong -= ResetTimer;
        //GameEvents.onCableDisconnect -= StopMusic;
    }

    private void Update()
    {
        if (timerActive)
            timer += Time.deltaTime;
    }

    // DEPRICATED
    //private void PlayMusic(CableController cable, SpeakerController speaker)
    //{
    //    var ampID = cable.amp.AmpID;
        
    //    if (ampID <= musicList.Count && ampID >= 0)
    //    {
    //        speaker.PlayMusic(musicList[ampID], ampID, timer);
    //    }
    //    else
    //    {
    //        Debug.Log("ID is invalid. Song may not be added to MusicController's musicList");
    //    }
    //}

    private void PlayMusic(CableController cable, PlugCable speaker)
    {
        // If we are still going to use this class, which I am unsure about, 
        // here we would find the instrument associated with the PlugCable, and begin
        // playing it's track. 
        // The game manager and instruments are more likely to handle this instead. 
    }

    private void StopMusic(CableController cable, SpeakerController speaker)
    {
        speaker.StopMusic();
    }

    public void ResetTimer()
    {
        timer = 0;
        StartTimer();
    }

    [ContextMenu("Start Timer")]
    private void StartTimer()
    {
        timerActive = true;
    }

    // TODO: Should be called when then game ends or is paused
    private void StopTimer()
    {
        timerActive = false;
    }
}
