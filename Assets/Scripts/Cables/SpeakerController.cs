using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakerController : MonoBehaviour
{
    [SerializeField] private int speakerID;
    public int SpeakerID { get => speakerID; }

    private AudioSource speakerAudio;

    private void Awake()
    {
        speakerAudio = GetComponent<AudioSource>();

        //Adds component if does not currently exist
        if (!speakerAudio)
        {
            speakerAudio = gameObject.AddComponent<AudioSource>();
            speakerAudio.loop = true;
            speakerAudio.playOnAwake = false;
            speakerAudio.Stop();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayMusic(AudioClip audioclip)
    {
        if (speakerAudio)
        {
            speakerAudio.clip = audioclip;
            speakerAudio.Play();
        }
        else
        {
            Debug.Log("Speaker is missing audio source");
        }
    }

    public void StopMusic()
    {
        if (speakerAudio)
        {
            speakerAudio.clip = null;
            speakerAudio.Stop();
        }
        else
        {
            Debug.Log("Speaker is missing audio source");
        }
    }
}
