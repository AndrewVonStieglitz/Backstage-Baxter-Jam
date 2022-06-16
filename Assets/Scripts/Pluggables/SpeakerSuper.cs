using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlugSpeaker", menuName = "ScriptableObjects/PluggableSpeaker", order = 1)]
public class SpeakerSuper : PluggablesSO
{
    public SpeakerVolume volume;

    public override void OnConnect()
    {
        base.OnConnect();
        // Let the recipe system know that a speaker has been reached and check to see if it is correct. 
    }
}
