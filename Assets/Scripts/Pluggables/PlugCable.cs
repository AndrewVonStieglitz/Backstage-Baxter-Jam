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
    //[SerializeField] private Sprite defaultCableTexture;
    private bool isInstrument;
    [SerializeField] private CableColor itemColor;
    [SerializeField] private Sprite[] cableColorSprites;

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
        bool hasCable = cableHead.Cable != null;
        print("Interact called on: " + name + ",\tType: " + pluggableType + ",\thasCable: " + hasCable);// + ",\tIt's colour: " + itemColor + ",\tCable colour: " + cableHead.Cable.cableColor);

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
                {
                    if (cableOut != null)
                        cableOut.pluggableEnd.Unplug();
                    StartCable();
                }
                break;
            case PluggableType.Speaker:
                if (hasCable)
                    EndCable();
                break;
        }
    }

    public void Unplug()
    {
        cableIn = null;
        Refresh();
        if (pluggableType != PluggableType.Instrument)
        {
            cableSprite = cableColorSprites[0];
            // apply this to the renderers
            Cables.CableController studyCable = cableOut;
            while (studyCable != null)
            {
                //studyCable.transform.GetChild(1).GetComponent<LineRenderer>().material.mainTexture = defaultCableTexture.texture;
                studyCable.SetTexture(cableColorSprites[0].texture);
                studyCable = studyCable.pluggableEnd.cableOut;
                //lineRenderer.material.mainTexture = cableSprite.texture;
            }
        }
    }

    public bool Refresh()
    {
        if (cableIn != null)
        {
            // This bool is false if the recalculation encounters a loop, the most terrible thing. 
            // if it does, pass that on to know not to allow the connection. 
            cableIn.RecalculatePluggablesList();//bool recalcOK = 
            //if (!recalcOK)
            //    return false;
            cableSprite = cableIn.pluggableStart.cableSprite;
            if (cableOut != null)
            {
                cableOut.SetTexture(cableIn.pluggableStart.cableSprite.texture);
                //cableOut.transform.GetChild(1).GetComponent<LineRenderer>().material.mainTexture = cableIn.pluggableStart.cableSprite.texture;
            }
            instrument = cableIn.pluggableStart.instrument;
        }
        if (cableOut != null)
        {
            cableOut.RecalculatePluggablesList();//bool recalcOK = 
            //if (!recalcOK) return false;
            if (cableOut.state == Cables.CableController.CableState.Completed)
            {
                cableOut.pluggableEnd.Refresh();//recalcokOKOK
            }
        }
        return true;
    }

    private void Awake()
    {
        
        if (pluggableType == PluggableType.Instrument)
        {
            IMB = GetComponent<InstrumentMB>();
            instrument = IMB.GetIdentifierSO();
            itemColor = IMB.cableColor;
            cableSprite = cableColorSprites[(int)itemColor];
        }
        else
        {
            PMB = GetComponent<PluggableMB>();
            PMB.itemColor = itemColor;
            pluggable = PMB.GetIdentifierSO();
            PMB.Init();
        }
        
        boxCol = GetComponent<BoxCollider2D>();
        if (cableHead == null)
            cableHead = GameObject.Find("Baxter").transform.GetChild(0).GetComponent<Cables.CableHead>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (cableSprite == null)
            cableSprite = cableColorSprites[0];
        isInstrument = instrument != null;
        if (cableHead == null)
            cableHead = GameObject.Find("Baxter").GetComponentInChildren<Cables.CableHead>();
    }

    private void StartCable()
    {
        if (cableOut != null)
            cableOut.pluggableEnd.Unplug();
        GameObject cableObject = Instantiate(cablePrefab, transform);
        Cables.CableController cable = cableObject.GetComponent<Cables.CableController>();
        if (cableOut)
        {
            print(name + " (StartCable()) disconnecting cable to: " + cableOut.pluggableStart.name);
            GameEvents.CableDisconnectPlug(cableOut, this);

            // Additional conditions are required to determine whether the target is on or off
            cableOut.pluggableEnd.PlayRandomSound(audioDisconnectOff);
        }
        cableOut = cable;
        cables.Add(cable);

        cableHead.NewCable(cable);
        PlayRandomSound(audioConnectOn);

        cable.Initialise(this, itemColor);
        Refresh();
        print("Starting cable on: " + name);
        // TODO: inform game coordinator that a cable is starting from here
    }

    private void EndCable()
    {
        Cables.CableController cable = cableHead.Cable;
        PlugCable cableStart = cable.pluggableStart;
        if (cableStart == this) return;
        if (cable.cableColor != itemColor)
        {
            // TODO Play a wrong SFX
            print("Wrong colour cannot endcable");
            return;
        }
        //bool refreshOK = Refresh();
        //if (!refreshOK) return;
        if (ContainsLoops(cable))
        {
            // play a truly grusome sound effect
            return;
        }
        cable.nodes.Last().MoveNode(transform.position);
        cable.pluggablesList.Add(pluggable);
        cable.Complete();
        cable.pluggableEnd = this;
        PlayRandomSound(audioConnectOff);
        if (cableIn)
        {
            print(name + " (EndCable()) disconnecting cable to: " + cableIn.pluggableStart.name);
            GameEvents.CableDisconnectPlug(cableIn, this);

            cableIn.pluggableStart.PlayRandomSound(audioDisconnectOn);
        }
        cableIn = cable;
        GameEvents.CableConnectPlug(cable, this);
        cable.pluggableEnd = this;
        cableSprite = cableStart.cableSprite;
        Refresh();
        print("Connected cable from: " + cableStart.name + ",\t to: " + name);
        // TODO: inform game coordinator that a cable has finished here, if speaker play song. 
    }

    public PlugCable GetPrevPlugCable()
    {
        if (cableIn != null)
            return cableIn.pluggableStart;
        return null;
    }

    private bool ContainsLoops(Cables.CableController cable)
    {
        //print("Checking for loops on: " + name);
        List<PlugCable> seenPlugCables = new List<PlugCable>();
        seenPlugCables.Add(this);
        PlugCable studyCable = cable.pluggableStart;
        while (studyCable != null)
        {
            if (seenPlugCables.Contains(studyCable))
                return true;
            seenPlugCables.Add(studyCable);
            studyCable = studyCable.GetPrevPlugCable();
        }
        //print("Found no loops");
        return false;
    }

    public InstrumentSO GetPathsInstrument() {
        if (instrument != null)
            return instrument;
        return cableIn != null ? cableIn.instrument : null;
    }

    public void PlayRandomSound(AudioClip[] array) {
        // get a random AudioClip from the given array
        int num = UnityEngine.Random.Range(0, array.Length-1);
        AudioClip ac = array[num];
        print("Object: " + name + " playing random clip:" + ac.name);
        // play the sound
        audioSource.clip = ac;
        audioSource.Play();
    }

    public void PlayRandomDisconnectSound() { PlayRandomSound(audioDisconnectOn); }

    public bool IsInstrument() { return isInstrument; } // if someone can make this into actually good code please DM Johnny C
}
