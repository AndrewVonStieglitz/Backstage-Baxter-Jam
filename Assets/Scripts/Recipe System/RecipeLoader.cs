using System.Collections;
using System.Collections.Generic;
using Pluggables;
using UnityEngine;

public class RecipeLoader : MonoBehaviour
{
    [SerializeField] public Album album;
    int currentSong = 0;
    
    void PlaySong()
    {
        if (currentSong < album.songDataList.Count)
        {
            GameEvents.ReadySong(album.songDataList[currentSong].Song);
            currentSong++;
        }
        else
        {
            GameEvents.EndAlbum();
        }
    }

    void StartAlbum()
    {
        currentSong = 0;
        PlaySong();
    }

    private void OnEnable()
    {
        GameEvents.onNextSong += PlaySong;
        GameEvents.onStartAlbum += StartAlbum; 

    }

    private void OnDisable()
    {
        GameEvents.onNextSong -= PlaySong;
        GameEvents.onStartAlbum -= StartAlbum;
    }
}

public struct song
{
    public string songName;
    public float duration;
    public float bpm;
    public recipe[] componentRecipes;
    public AudioClip drumTrack; 

    public song(string songNameSO, float durationSO, recipe[] componentRecipesSO, AudioClip drumTrack, float songBPM)
    {
        songName = songNameSO;
        duration = durationSO;
        componentRecipes = componentRecipesSO;
        this.drumTrack = drumTrack;
        bpm = songBPM;
    }
}

public struct recipe
{
    public InstrumentSO instrument;
    public MidAffectorSuper amp;
    public SpeakerSuper speaker;
    public MidAffectorSuper[] midAffectors;
    public AudioClip songPart;

    public recipe(InstrumentSO instrumentSO, MidAffectorSuper ampSO, SpeakerSuper speakerSO, MidAffectorSuper[] midAffectorsSO, AudioClip songPartSO)
    {
        instrument = instrumentSO;
        amp = ampSO;
        speaker = speakerSO;
        midAffectors = midAffectorsSO;
        songPart = songPartSO;
    }
}
