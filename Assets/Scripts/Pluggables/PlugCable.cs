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
    [SerializeField] private Sprite defaultCableTexture;
    private bool isInstrument;

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
            cableSprite = defaultCableTexture;
            // apply this to the renderers
            Cables.CableController studyCable = cableOut;
            while (studyCable != null)
            {
                studyCable.transform.GetChild(1).GetComponent<LineRenderer>()
                    .material.mainTexture = defaultCableTexture.texture;
                studyCable = studyCable.pluggableEnd.cableOut;
                //lineRenderer.material.mainTexture = cableSprite.texture;
            }
        }
    }

    public void Refresh()
    {
        if (cableIn != null)
        {
            cableIn.RecalculatePluggablesList();
            cableSprite = cableIn.pluggableStart.cableSprite;
            if (cableOut != null)
            {
                cableOut.transform.GetChild(1).GetComponent<LineRenderer>()
                    .material.mainTexture = cableIn.pluggableStart.cableSprite.texture;
            }
            instrument = cableIn.pluggableStart.instrument;
        }
        if (cableOut != null)
        {
            cableOut.RecalculatePluggablesList();
            if (cableOut.state == Cables.CableController.CableState.Completed)
                cableOut.pluggableEnd.Refresh();
        }
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
    }

    private void Start()
    {
        if (cableSprite == null)
            cableSprite = defaultCableTexture;
        isInstrument = instrument != null;
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
        }
        cableOut = cable;
        cables.Add(cable);

        cableHead.NewCable(cable);

        cable.Initialise(this);
        Refresh();
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

    public InstrumentSO GetPathsInstrument() {
        if (instrument != null)
            return instrument;
        return cableIn != null ? cableIn.instrument : null;
    }

    public bool IsInstrument() { return isInstrument; } // if someone can make this into actually good code please DM Johnny C
}
