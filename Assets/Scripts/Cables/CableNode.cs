using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableNode : MonoBehaviour
{
    private PoleController.Orientation nodeOrientation;
    public PoleController.Orientation NodeOrientation { get => nodeOrientation; }

    private CableController cable;
    public CableController CableID { get => cable; }
    // Start is called before the first frame update

    public CableNode(PoleController.Orientation nodeOrientation, CableController cable)
    {
        this.nodeOrientation = nodeOrientation;
        this.cable = cable;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
