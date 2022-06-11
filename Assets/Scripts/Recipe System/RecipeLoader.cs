using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeLoader : MonoBehaviour
{
    [SerializeField] Album album;
    int currentSong = 0;
    // Start is called before the first frame update

    void PlaySong()
    {
        if (currentSong < album.songDataList.Count)
        {
            GameEvents.StartSong(album.songDataList[currentSong].Song);
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
    public float duration;
    public recipe[] componentRecipes;

    public song(float durationSO, recipe[] componentRecipesSO)
    {
        duration = durationSO;
        componentRecipes = componentRecipesSO;
    }
}

public struct recipe
{
    public PluggablesSO instrument;
    public PluggablesSO amp;
    public PluggablesSO speaker;
    public PluggablesSO[] midAffectors;
    public AudioClip songPart;

    public recipe(PluggablesSO instrumentSO, PluggablesSO ampSO, PluggablesSO speakerSO, PluggablesSO[] midAffectorsSO, AudioClip songPartSO)
    {
        instrument = instrumentSO;
        amp = ampSO;
        speaker = speakerSO;
        midAffectors = midAffectorsSO;
        songPart = songPartSO;
    }
}
