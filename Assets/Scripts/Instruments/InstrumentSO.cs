using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unnamed instrument", menuName = "ScriptableObjects/Instrument", order = 1)]
public class InstrumentSO : ScriptableObject
{
    public string itemName;
    public Sprite sprite;
    public bool isDrum = false;
}
