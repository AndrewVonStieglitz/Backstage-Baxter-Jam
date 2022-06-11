using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentMB : MonoBehaviour
{
    [SerializeField] protected InstrumentSO identifierSO;
    [SerializeField] protected AudioClip[] songParts;// the audio associated with each song for this instrument
    [SerializeField] protected Color cableCol;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCol;
    protected bool isDrum; // we have this because drums are loud and produce sound without a speaker
    protected int trackNum { get; set; }

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
        audioSource.clip = songParts[trackNum];
        audioSource.time = startPoint;
        audioSource.Play();
    }

    public void NextSong() // Event system call this
    {
        trackNum++;
        if (isDrum)
            StartPlaying(0f);
    }

    void Start()
    {
        Init();
        trackNum = 0;
        if (isDrum) audioSource.volume = 0.5f; // drums are quieter until mic'd
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        boxCol = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
