using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDHappinessDisplay : MonoBehaviour
{

    //[SerializeField] private Sprite[][] sprites;//this is so sad Unity Editor does not support this
    private Sprite[,] sprites;
    [SerializeField] private Sprite[] state0Sprites;
    [SerializeField] private Sprite[] state1Sprites;
    [SerializeField] private Sprite[] state2Sprites;
    [SerializeField] private Sprite[] state3Sprites;
    [SerializeField] private Sprite[] state4Sprites;
    [SerializeField] private Sprite[] state5Sprites;
    [SerializeField] private Sprite[] state6Sprites;
    [SerializeField] private Sprite[] state7Sprites;
    [SerializeField] private Sprite[] state8Sprites;
    [SerializeField] private Sprite[] state9Sprites;
    [SerializeField] private Sprite[] state10Sprites;
    [SerializeField] private Sprite[] state11Sprites;
    [SerializeField] private Animation[] animations;

    private int frame = 0;
    private const int NumOfFrames = 3; 
    [SerializeField] private float frameRate = 24f;
    private float frameDuration;
    private int state;
    private const int NumOfStates = 6;

    private SpriteRenderer sr;
    private Animator animator;

    void Start()
    {
        frameDuration = frameRate / 60f;
        //sprites = new Sprite[][] { [ state0Sprites[0], state0Sprites[1] ],[state1Sprites[0], state1Sprites[1] ],[state2Sprites[0], state2Sprites[1] ],[state0Sprites[0], state0Sprites[1] ],[state0Sprites[0], state0Sprites[1] ],[state0Sprites[0], state0Sprites[1] ] };
        //sprites = new Sprite[6, 2] { { state0Sprites[0], state0Sprites[1] }, { state1Sprites[0], state1Sprites[1] }, { state2Sprites[0], state2Sprites[1] }, { state3Sprites[0], state3Sprites[1] }, { state4Sprites[0], state4Sprites[1] }, { state5Sprites[0], state5Sprites[1] } };
        sprites = new Sprite[12, 3] { { state0Sprites[0], state0Sprites[1], state0Sprites[2], },
            { state1Sprites[0], state1Sprites[1], state1Sprites[2], },
            { state2Sprites[0], state2Sprites[1], state2Sprites[2], },
            { state3Sprites[0], state3Sprites[1], state3Sprites[2], },
            { state4Sprites[0], state4Sprites[1], state4Sprites[2], },
            { state5Sprites[0], state5Sprites[1], state5Sprites[2], },
            { state6Sprites[0], state6Sprites[1], state6Sprites[2], },
            { state7Sprites[0], state7Sprites[1], state7Sprites[2], },
            { state8Sprites[0], state8Sprites[1], state8Sprites[2], },
            { state9Sprites[0], state9Sprites[1], state9Sprites[2], },
            { state10Sprites[0], state10Sprites[1], state10Sprites[2], },
            { state11Sprites[0], state11Sprites[1], state11Sprites[2], },  };
        sr = GetComponent<SpriteRenderer>();
        //animator = GetComponent<Animator>();
        StartCoroutine(Animate());
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void SetHappiness(float happiness)
    {
        // assuming happiness [0,100]
        state = Mathf.RoundToInt(happiness / 100f);
    }

    //public void SetState(int newState)
    //{
        
    //    state = newState;
    //}
}
