using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableController : MonoBehaviour
{
    [SerializeField] private int cableID;
    public int CableID { get => cableID; }

    private int ampID;
    public int AmpID { get => ampID; }


    // Start is called before the first frame update
    public CableController(int cableID, int ampID)
    {
        this.cableID = cableID;
        this.ampID = ampID;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
