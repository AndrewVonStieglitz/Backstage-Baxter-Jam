using System.Collections.Generic;
using DefaultNamespace.Pluggables;
using UnityEngine;

public class PlugCable : MonoBehaviour
{
    private PluggableMB PMB;
    private InstrumentMB IMB;
    public PluggablesSO pluggable;
    public InstrumentSO instrument;
    private BoxCollider2D boxCol;

    [SerializeField] private ConnectionHead connectionHead;
    public Sprite cableSprite;
    [SerializeField] private PluggableType pluggableType;
    private readonly List<Connection> connections = new List<Connection>();
    protected Connection connectionIn, connectionOut;
    //[SerializeField] private Sprite defaultCableTexture;
    private bool isInstrument;
    [SerializeField] private CableColor itemColor;
    [SerializeField] private Sprite[] cableColorSprites;

    private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioConnectOn;
    [SerializeField] private AudioClip[] audioDisconnectOn;
    [SerializeField] private AudioClip[] audioConnectOff;
    [SerializeField] private AudioClip[] audioDisconnectOff;
    [SerializeField] private AudioClip[] audioElecFailure;

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
        bool hasCable = connectionHead.Connection != null;
        if (GameManager.currentGameState != GameManager.GameState.playing) return;
        //print("Interact called on: " + name + ",\tType: " + pluggableType + ",\thasCable: " + hasCable);// + ",\tIt's colour: " + itemColor + ",\tCable colour: " + cableHead.Cable.cableColor);

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
                    if (connectionOut != null)
                        connectionOut.pluggableEnd.Unplug(false);
                    StartCable();
                }
                break;
            case PluggableType.Speaker:
                if (hasCable)
                    EndCable();
                break;
        }
    }

    public void Unplug(bool useErrorSound)
    {
        connectionIn = null;
        Refresh();
        if (pluggableType != PluggableType.Instrument)
        {
            cableSprite = cableColorSprites[0];
            // apply this to the renderers
            Connection studyConnection = connectionOut;
            while (studyConnection != null)
            {
                //studyCable.transform.GetChild(1).GetComponent<LineRenderer>().material.mainTexture = defaultCableTexture.texture;
                studyConnection.texture = cableColorSprites[0].texture;
                studyConnection = studyConnection.pluggableEnd.connectionOut;
                //lineRenderer.material.mainTexture = cableSprite.texture;
            }
        }
        if (useErrorSound)
            PlayRandomSound(audioElecFailure);
        else
            PlayRandomDisconnectSound();
    }

    public bool Refresh()
    {
        if (connectionIn != null)
        {
            // This bool is false if the recalculation encounters a loop, the most terrible thing. 
            // if it does, pass that on to know not to allow the connection. 
            connectionIn.RecalculatePluggablesList();//bool recalcOK = 
            //if (!recalcOK)
            //    return false;
            cableSprite = connectionIn.pluggableStart.cableSprite;
            if (connectionOut != null)
            {
                connectionOut.texture = connectionIn.pluggableStart.cableSprite.texture;
                //cableOut.transform.GetChild(1).GetComponent<LineRenderer>().material.mainTexture = cableIn.pluggableStart.cableSprite.texture;
            }
            instrument = connectionIn.pluggableStart.instrument;
        }
        if (connectionOut != null)
        {
            connectionOut.RecalculatePluggablesList();//bool recalcOK = 
            //if (!recalcOK) return false;
            if (connectionOut.state == Connection.ConnectionState.Connected)
            {
                connectionOut.pluggableEnd.Refresh();//recalcokOKOK
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
        if (connectionHead == null)
            connectionHead = GameObject.Find("Baxter").transform.GetChild(0).GetComponent<ConnectionHead>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (cableSprite == null)
            cableSprite = cableColorSprites[0];
        isInstrument = instrument != null;
        if (connectionHead == null)
            connectionHead = GameObject.Find("Baxter").GetComponentInChildren<ConnectionHead>();
    }

    private void StartCable()
    {
        if (connectionOut != null)
            connectionOut.pluggableEnd.Unplug(false);
        
        Connection connection = new Connection();
        
        if (connectionOut != null)
        {
            //print(name + " (StartCable()) disconnecting cable to: " + cableOut.pluggableStart.name);
            GameEvents.Disconnect(connectionOut, this);

            // Additional conditions are required to determine whether the target is on or off
            connectionOut.pluggableEnd.PlayRandomSound(audioDisconnectOff);
        }
        connectionOut = connection;
        connections.Add(connection);

        connectionHead.Connection = connection;
        
        PlayRandomSound(audioConnectOn);

        connection.Initialise(this, itemColor);
        Refresh();
        //print("Starting cable on: " + name);
        // TODO: inform game coordinator that a cable is starting from here

        GameEvents.ConnectionStarted(connection);
    }

    private void EndCable()
    {
        Connection connection = connectionHead.Connection;
        PlugCable cableStart = connection.pluggableStart;
        if (cableStart == this) return;
        if (connection.cableColor != itemColor)
        {
            PlayRandomSound(audioElecFailure);
            //print("Wrong colour cannot endcable");
            return;
        }
        //bool refreshOK = Refresh();
        //if (!refreshOK) return;
        if (ContainsLoops(connection))
        {
            // play a truly grusome sound effect
            return;
        }
        connection.pluggablesList.Add(pluggable);
        connection.pluggableEnd = this;
        PlayRandomSound(audioConnectOff);
        if (connectionIn != null)
        {
            print(name + " (EndCable()) disconnecting cable to: " + connectionIn.pluggableStart.name);
            GameEvents.Disconnect(connectionIn, this);

            connectionIn.pluggableStart.PlayRandomSound(audioDisconnectOn);
        }
        connectionIn = connection;
        connection.pluggableEnd = this;
        cableSprite = cableStart.cableSprite;
        Refresh();
        connection.state = Connection.ConnectionState.Connected;
        connectionHead.Connection = null;
        GameEvents.Connect(connection, this);
        print("Connected cable from: " + cableStart.name + ",\t to: " + name);
        // TODO: inform game coordinator that a cable has finished here, if speaker play song. 
    }

    public PlugCable GetPrevPlugCable()
    {
        if (connectionIn != null)
            return connectionIn.pluggableStart;
        return null;
    }

    private bool ContainsLoops(Connection cable)
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
        return connectionIn != null ? connectionIn.instrument : null;
    }

    public void PlayRandomSound(AudioClip[] array) {
        // get a random AudioClip from the given array
        int num = UnityEngine.Random.Range(0, array.Length-1);
        AudioClip ac = array[num];
        //print("Object: " + name + " playing random clip:" + ac.name);
        // play the sound
        audioSource.time = 0f;
        audioSource.clip = ac;
        audioSource.Play();
    }

    public void PlayRandomDisconnectSound() { PlayRandomSound(audioDisconnectOn); }

    public bool IsInstrument() { return isInstrument; } // if someone can make this into actually good code please DM Johnny C
}
