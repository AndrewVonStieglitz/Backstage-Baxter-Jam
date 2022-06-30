using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDCassette : MonoBehaviour
{
    private Sprite[,] sprites;
    [SerializeField] private Sprite[] state0Sprites;
    [SerializeField] private Sprite[] state1Sprites;
    [SerializeField] private Sprite[] state2Sprites;
    [SerializeField] private Sprite[] state3Sprites;
    [SerializeField] private Sprite[] state4Sprites;

    private int frame = 0;
    private const int NumOfFrames = 3;
    [SerializeField] private float frameRate = 24f;
    private float frameDuration;
    private int state;
    private const int NumOfStates = 5;
    private SpriteRenderer sr;

    // we can set these in inspector for debug
    public string trackTitle;
    public float trackLength;

    void Start()
    {
        frameDuration = frameRate / 60f;
        sprites = new Sprite[5, 3] {{ state0Sprites[0], state0Sprites[1], state0Sprites[2], },
            { state1Sprites[0], state1Sprites[1], state1Sprites[2], },
            { state2Sprites[0], state2Sprites[1], state2Sprites[2], },
            { state3Sprites[0], state3Sprites[1], state3Sprites[2], },
            { state4Sprites[0], state4Sprites[1], state4Sprites[2], }};
        sr = GetComponent<SpriteRenderer>();
        StartCoroutine(Animate());
        // for debug
        StartCoroutine(StartCountingDown());
    }

    private IEnumerator Animate()
    {
        while (gameObject.activeInHierarchy)
        {
            frame = (frame + 1) % NumOfFrames;
            sr.sprite = sprites[state, frame];
            yield return new WaitForSeconds(frameDuration);
        }
    }

    private IEnumerator StartCountingDown()
    {
        float stateDuration = trackLength / NumOfStates;
        while (state < NumOfStates - 1)
        {
            yield return new WaitForSeconds(stateDuration);
            state++;
        }
    }

    public void NewSong(string newTitle, float newLength)
    {
        state = 0;
        trackTitle = newTitle;
        trackLength = newLength;
        StopCoroutine(StartCountingDown());
        StartCoroutine(StartCountingDown());
    }
}
