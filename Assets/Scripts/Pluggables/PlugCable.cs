using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlugCable : MonoBehaviour
{
    private PluggableMB PMB;
    private InstrumentMB IMB;
    public PluggablesSO pluggable;
    public InstrumentSO instrument;
    private BoxCollider2D boxCol;

    [SerializeField] private GameObject cablePrefab;
    [SerializeField] private Cables.CableHead cableHead;
    public Sprite cableSprite;
    [SerializeField] private PluggableType pluggableType;
    private readonly List<Cables.CableController> cables = new List<Cables.CableController>();
    protected Cables.CableController cableIn, cableOut;

    private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioConnectOn;
    [SerializeField] private AudioClip[] audioDisconnectOn;
    [SerializeField] private AudioClip[] audioConnectOff;
    [SerializeField] private AudioClip[] audioDisconnectOff;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // comment out to replace into Interact()
        //if (!collision.CompareTag("CableHead")) return;
        //bool hasCable = cableHead.cable != null;

        //switch (pluggableType)
        //{
        //    case PluggableType.Instrument:
        //        if (!hasCable)
        //            StartCable();
        //        break;
        //    case PluggableType.Mid:
        //        if (hasCable)
        //            EndCable();
        //        else
        //            StartCable();
        //        break;
        //    case PluggableType.Speaker:
        //        if (hasCable)
        //            EndCable();
        //        break;
        //}
    }

    public void Interact()
    {
        //moved functionality from on trigger enter 2D
        bool hasCable = cableHead.cable != null;

        switch (pluggableType)
        {
            case PluggableType.Instrument:
                if (!hasCable)
                    StartCable();
                break;
            case PluggableType.Mid:
                if (hasCable)
                    EndCable();
                else
                    StartCable();
                break;
            case PluggableType.Speaker:
                if (hasCable)
                    EndCable();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        
        if (pluggableType == PluggableType.Instrument)
        {
            IMB = GetComponent<InstrumentMB>();
            instrument = IMB.GetIdentifierSO();
        }
        else
        {
            PMB = GetComponent<PluggableMB>();
            pluggable = PMB.GetIdentifierSO();
        }
        
        boxCol = GetComponent<BoxCollider2D>();

        audioSource = GetComponent<AudioSource>();
    }

    private void StartCable()
    {
        GameObject cableObject = Instantiate(cablePrefab, transform);
        Cables.CableController cable = cableObject.GetComponent<Cables.CableController>();
        if (cableOut)
        {
            print(name + " (StartCable()) disconnecting cable to: " + cableOut.pluggableStart.name);
            GameEvents.CableDisconnectPlug(cableOut, this);

            // Additional conditions are required to determine whether the target is on or off
            playRandomSound(audioConnectOff);
        }
        cableOut = cable;
        cables.Add(cable);

        cableHead.NewCable(cable);

        cable.Initialise(this);
        print("Starting cable on: " + name);
        // TODO: inform game coordinator that a cable is starting from here
    }

    private void EndCable()
    {
        Cables.CableController cable = cableHead.cable;
        PlugCable cableStart = cable.pluggableStart;
        if (cableStart == this) return;
        cable.nodes.Last().MoveNode(transform.position);
        cable.pluggablesList.Add(pluggable);
        cable.Complete();
        cable.pluggableEnd = this;
        if (cableIn)
        {
            print(name + " (EndCable()) disconnecting cable to: " + cableIn.pluggableStart.name);
            GameEvents.CableDisconnectPlug(cableIn, this);

            playRandomSound(audioConnectOn);
        }
        cableIn = cable;
        GameEvents.CableConnectPlug(cable, this);
        cable.pluggableEnd = this;
        cableSprite = cableStart.cableSprite;
        print("Connected cable from: " + cableStart.name + ",\t to: " + name);
        // TODO: inform game coordinator that a cable has finished here, if speaker play song. 
    }

    public InstrumentSO GetPathsInstrument() {
        if (instrument != null)
            return instrument;
        return cableIn != null ? cableIn.instrument : null;
    }

    private void playRandomSound(AudioClip[] array) {
        // get a random AudioClip from the given array
        int num = UnityEngine.Random.Range(0, array.Length-1);
        AudioClip ac = array[num];

        // play the sound
        audioSource.clip = ac;
        audioSource.Play();
    }
}
