using System.Collections;
using UnityEngine;

public class BPMMovement : MonoBehaviour
{
    public enum Rhythm{
        Common,
        Half
    }

    [SerializeField] Rhythm rhythm;
    [SerializeField] float bpm; //possible placeholder - could make a static member somewhere else to get this value?

    [Range(0, 1f)]
    [SerializeField] float yScale; //from 0-1.0
    [Range(0, 1f)]
    [SerializeField] float xScale;
    float beat;
    [Range(10, 200)]
    //[SerializeField] int speed;
    [SerializeField] float test;
    static bool isStarted;

    // Start is called before the first frame update
    void Start()
    {
        
        switch (rhythm)
        {
            case Rhythm.Common:
                beat = bpm / 60;
                break;
            case Rhythm.Half:
                beat = bpm / 30;
                break;

        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (rhythm)
        {
            case Rhythm.Common:
                beat = bpm / 60;
                break;
            case Rhythm.Half:
                beat = bpm / 120;
                break;
            default:
                beat = bpm / 60;
                break;
        }
        //test = -Mathf.Cos(360 * Time.deltaTime);
        //StartCoroutine("Bopping");
        float x = (xScale / 2) * Mathf.Cos((Mathf.PI * 2 * Time.time) * beat) + (1 + (xScale / 2));
        test = x;
        float y = -(yScale/2) * Mathf.Cos((Mathf.PI * 2 * Time.time) * beat) + ( 1 + (yScale/2));
        
        transform.localScale = new Vector3(x, y, 1);
    }

    IEnumerator Bopping()
    {
        
        float x = (xScale / 2) * Mathf.Cos((Mathf.PI * 2 * Time.time) * bpm / 60) + (1 + (xScale / 2));
        test = x;
        float y = -(yScale / 2) * Mathf.Cos((Mathf.PI * 2 * Time.time) * bpm / 60) + (1 + (yScale / 2));

        transform.localScale = new Vector3(x, y, 1);
        yield return new WaitForSeconds(beat);
    }
}
