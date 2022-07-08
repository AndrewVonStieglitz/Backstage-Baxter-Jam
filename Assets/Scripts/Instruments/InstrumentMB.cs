using System.Collections;
using System.Collections.Generic;
using Pluggables;
using UnityEngine;

public class InstrumentMB : MonoBehaviour
{
    [SerializeField] public InstrumentSO identifierSO;
    [SerializeField] protected AudioClip songParts;// the audio associated with each song for this instrument
    [SerializeField] protected Color cableCol;
    public CableColor cableColor;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCol;
    protected bool isDrum; // we have this because drums are loud and produce sound without a speaker

    public void Init()
    {
        if (identifierSO != null)
        {
            spriteRenderer.sprite = identifierSO.sprite;
            isDrum = identifierSO.isDrum;
        }
        else
            print("ERROR: " + name + " instrument scriptable object is missing");
    }

    public void StartPlaying(float startPoint)// Event system call this
    {
        print("IMB: " + name + " StartPlaying called");
        audioSource.clip = songParts;
        audioSource.time = startPoint;
        audioSource.Play();
    }

    private void StopPlaying()
    {
        audioSource.Stop();
    }

    public void StartSong() // Event system call this
    {
        if (isDrum)
            StartPlaying(0f);
    }

    void Start()
    {
        Init();
        if (isDrum) audioSource.volume = 0.5f; // drums are quieter until mic'd
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        boxCol = GetComponent<BoxCollider2D>();
    }

    private void ReadyInstrument(song song) //Sets caudio clip to play when song is readied
    {
        foreach (recipe recipe in song.componentRecipes)
        {
            if (recipe.instrument == identifierSO)
            {
                this.songParts = recipe.songPart;
                return;
            }
        }
    }

    private void OnRecipeCompleted(recipe recipe) //Uses GameManager timer to get time. If recipe's instrument = this, start playing
    {
        //print("IBM: " + name + ", called recipe complete via event system");
        if (recipe.instrument == identifierSO)
        {
            StartPlaying(GameManager.timeElapsed);
            print(name + " start playing song with timer: " + GameManager.timer);
        }
    }

    private void OnRecipeBroken(recipe recipe) //Likewise, stops playing of recipe's isntrument is not this
    {
        if (recipe.instrument == this)
        {
            StopPlaying();
        }
    }


    protected float GetVolume(SpeakerSuper speaker) // Currently unused
    {
        switch (speaker.volume)
        {
            case SpeakerVolume.Loud:
                {
                    return 0.8f;
                }
            case SpeakerVolume.Mid:
                {
                    return 0.5f;
                }
            default:
                {
                    return 0.3f;
                }
        }
    }

    private void OnEnable()
    {
        GameEvents.onReadySong += ReadyInstrument;
        GameEvents.onRecipeCompleted += OnRecipeCompleted;
        GameEvents.onRecipeBroken += OnRecipeBroken;
        GameEvents.onTimeUp += StopPlaying;
    }

    private void OnDisable()
    {
        GameEvents.onReadySong -= ReadyInstrument;
        GameEvents.onRecipeCompleted -= OnRecipeCompleted;
        GameEvents.onRecipeBroken -= OnRecipeBroken;
        GameEvents.onTimeUp -= StopPlaying;
    }

    public InstrumentSO GetIdentifierSO() { return identifierSO; }
}
