using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdMovement : MonoBehaviour
{
    public enum Rhythm
    {
        Common,
        Half,
        SixEight
    }

    [SerializeField] Rhythm rhythm;
    [SerializeField] float bpm;
    float beat;
    [Range(10, 200)]
    [SerializeField] int speed;
    [SerializeField] float test;

    Transform initialTransform;
    static bool isStarted;

    // Start is called before the first frame update
    void Start()
    {
        switch (rhythm)
        {
            case Rhythm.Common:
                beat = 60 / bpm;
                break;
            case Rhythm.Half:
                beat = 30 / bpm;
                break;

        }
        initialTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        float y = Mathf.Cos((360 * Time.time) / beat);
        transform.position = initialTransform.position + new Vector3(0, y);
        
    }
}
