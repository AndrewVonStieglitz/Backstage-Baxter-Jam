using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdMovement : MonoBehaviour
{
    [SerializeField] float bpm; //possible placeholder - could make a static member somewhere else to get this value?

    [Range(0, 1f)]
    [SerializeField] float yTransform; //boing
    [Range(0, 1f)]
    [SerializeField] float xScale;
    float beat;

    [SerializeField] float test;
   
    static bool isStarted;
    [SerializeField] bool isReversed;

    Vector3 originalPos;
    // Start is called before the first frame update
    void Start()
    {
        originalPos = transform.position;

    }

    // Update is called once per frame
    void Update()
    {

        beat = bpm / 60; //bps
        float scaleX = (xScale / 2) * Mathf.Cos((Mathf.PI * 2 * Time.time) * beat) + (1 + (xScale / 2));
        float transformY = -yTransform * Mathf.Cos((Mathf.PI * 2 * Time.time) * beat);
        if (isReversed)
            transformY = -transformY;
        
        transform.localPosition = new Vector3(originalPos.x, originalPos.y + transformY, originalPos.z);
        transform.localScale = new Vector3(scaleX, 1, 1);
    }
}
