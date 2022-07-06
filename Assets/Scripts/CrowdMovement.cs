using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdMovement : MonoBehaviour
{
    [SerializeField] float bpm; //possible placeholder - could make a static member somewhere else to get this value?

    [Range(0, 0.2f)]
    [SerializeField] float yTransform; //boing
    [Range(0, 0.2f)]
    [SerializeField] float xScale;
    float beat;

    [SerializeField] float test;
   
    bool isStarted;
    [SerializeField] bool isBopping;
    [SerializeField] bool isVibing;
    [SerializeField] bool isReversed;

    [SerializeField] GameManager gameManager;

    Vector3 originalPos;
    // Start is called before the first frame update
    void Start()
    {
        originalPos = transform.localPosition;
        //bpm = recipeLoader.GetSong().bpm;
        bpm = gameManager.currentSong.bpm;
        isStarted = false;
    }

    // Update is called once per frame
    void Update()
    {
       
        
        beat = bpm / 60; //bps
        float scaleX = (xScale / 2) * Mathf.Cos((Mathf.PI * 2 * (float)AudioSettings.dspTime) * beat) + (1 + (xScale / 2));
        float transformY = -yTransform * Mathf.Cos((Mathf.PI * 2 * (float)AudioSettings.dspTime) * beat);
        if (isReversed)
            transformY = -transformY;

        if (isBopping)
            transform.localPosition = new Vector3(originalPos.x, originalPos.y + transformY, originalPos.z);
        
        if(isVibing)
            transform.localScale = new Vector3(scaleX, 1, 1);
    }
}
