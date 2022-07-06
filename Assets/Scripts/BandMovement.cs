using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class BandMovement : MonoBehaviour
{
    [SerializeField] float bpm; //possible placeholder - could make a static member somewhere else to get this value?
    [Range(0, 0.2f)]
    [SerializeField] float yScale; //from 0-1.0
    [Range(0, 0.2f)]
    [SerializeField] float xScale;
    [SerializeField] bool isDrums;
    float beat;
    [Range(10, 200)]
    //[SerializeField] int speed;
    [SerializeField] float test;
    static bool isStarted;
    Vector3 originalScale;
    [SerializeField] public bool isConnected;

    [SerializeField] GameObject backlight;
    SpriteRenderer circle;
    public Light2D[] lights;
    Light2D mainLight;
    Light2D stageLight;
    [SerializeField] GameManager gameManager;

    Color inactiveColour;
    Color originalColour;
    // Start is called before the first frame update
    void Start()
    {
        
        originalScale = transform.localScale;
        bpm = gameManager.currentSong.bpm;
        lights = backlight.GetComponentsInChildren<Light2D>();
        mainLight = lights[0];
        stageLight = lights[2];
        isStarted = false;
        //Debug.Log(lights[2]);
        if (isDrums)
            isConnected = true;
        circle = backlight.GetComponent<SpriteRenderer>();
        inactiveColour = new Color(0.313f, 0.262f, 0.353f);
        originalColour = circle.color;
        isStarted = gameManager.isStarted;
    }

    // Update is called once per frame
    void Update()
    {
        isStarted = gameManager.isStarted;
        beat = bpm / 60; //bps
        //test = -Mathf.Cos(360 * Time.deltaTime);
        //StartCoroutine("Bopping");
        if (isStarted && isConnected)
        {
            float x = (xScale / 2) * Mathf.Cos((Mathf.PI * 2 * Time.time) * beat) + (originalScale.x + (xScale / 2));

            float y = -(yScale / 2) * Mathf.Cos((Mathf.PI * 2 * Time.time) * beat) + (originalScale.y + (yScale / 2));

            transform.localScale = new Vector3(x, y, 1);
        }
        if (!isConnected)
        {
            circle.color = inactiveColour;
            mainLight.enabled = false;
            stageLight.enabled = false;
        }
        else
        {
            if (isStarted)
            {
                circle.color = originalColour;
                mainLight.enabled = true;
                stageLight.enabled = true;
            }
        }
    }

    void Bopping()
    {

        float x = (xScale / 2) * Mathf.Cos((Mathf.PI * 2 * Time.time) * beat) + (originalScale.x + (xScale / 2));

        float y = -(yScale / 2) * Mathf.Cos((Mathf.PI * 2 * Time.time) * beat) + (originalScale.y + (yScale / 2));

        transform.localScale = new Vector3(x, y, 1);
        
    }


}
