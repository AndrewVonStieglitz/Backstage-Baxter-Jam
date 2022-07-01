using System.Collections;
using UnityEngine;

public class BPMMovement : MonoBehaviour
{
    [SerializeField] float bpm; //possible placeholder - could make a static member somewhere else to get this value?

    [Range(0, 0.2f)]
    [SerializeField] float yScale; //from 0-1.0
    [Range(0, 0.2f)]
    [SerializeField] float xScale;
    float beat;
    [Range(10, 200)]
    //[SerializeField] int speed;
    [SerializeField] float test;
    static bool isStarted;
    Vector3 originalScale;
    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale;

    }

    // Update is called once per frame
    void Update()
    {

        beat = bpm / 60; //bps
        //test = -Mathf.Cos(360 * Time.deltaTime);
        //StartCoroutine("Bopping");
        float x = (xScale / 2) * Mathf.Cos((Mathf.PI * 2 * Time.time) * beat) + (originalScale.x + (xScale / 2));
        
        float y = -(yScale/2) * Mathf.Cos((Mathf.PI * 2 * Time.time) * beat) + ( originalScale.y + (yScale/2));
        
        transform.localScale = new Vector3(x, y, 1);
    }

    IEnumerator Bopping()
    {

        float x = (xScale / 2) * Mathf.Cos((Mathf.PI * 2 * Time.time) * beat) + (originalScale.x + (xScale / 2));

        float y = -(yScale / 2) * Mathf.Cos((Mathf.PI * 2 * Time.time) * beat) + (originalScale.y + (yScale / 2));

        transform.localScale = new Vector3(x, y, 1);
        yield return new WaitForSeconds(beat);
    }
}
