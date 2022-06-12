using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlugCable : MonoBehaviour
{
    private PluggableMB PMB;
    private InstrumentMB IMB;
    private PluggablesSO pluggable;
    private InstrumentSO instrument;
    private BoxCollider2D boxCol;

    [SerializeField] private GameObject cablePrefab;
    [SerializeField] private Cables.CableHead cableHead;
    public Sprite cableSprite;
    [SerializeField] private PluggableType pluggableType;
    private readonly List<Cables.CableController> cables = new List<Cables.CableController>();
    protected Cables.CableController cableIn, cableOut;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("CableHead")) return;
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
    }

    private void StartCable()
    {
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
        print("Starting cable on: " + name);
        // TODO: inform game coordinator that a cable is starting from here
    }

    private void EndCable()
    {
        Cables.CableController cable = cableHead.cable;
        PlugCable cableStart = cable.pluggableStart;
        if (cableStart == this) return;
        cable.nodes.Last().MoveNode(transform.position);
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
        print("Connected cable from: " + cableStart.name + ",\t to: " + name);
        // TODO: inform game coordinator that a cable has finished here, if speaker play song. 
    }
}
