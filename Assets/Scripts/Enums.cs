using UnityEngine;

public enum SpeakerVolume
{
    Loud, 
    Mid,
    Quiet
}

public enum MidAffectorType
{
    Amp,
    Reverb,
    Delay
}

public enum PluggableType
{
    Instrument, // only wire out
    Mid,        // wire out and in (amp & mid-affector)
    Speaker     // only wire in
}

public enum CableColor
{
    Grey,
    Red,
    Green, 
    Blue, 
    Purple
}