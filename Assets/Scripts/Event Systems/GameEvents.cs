using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cables;

/*
 * Event List ============================
 * 
 * onGameStart()
 * onGameOver()
 * onTimeUp()
 * onHurt()
 * onCablePickup(CableController cable)
 * onCableDrop(CableController cable)
 * onCableSpawn(AmpController amp, CableController cable)
 * onCableConnect(CableController cable, SpeakerController speaker)
 * onCableConnectPlug(CableController cable, PlugCable endObj)
 * onCableDisconnect(CableController cable, SpeakerController speaker)
 * onCableDisconnectPlug(CableController cable, PlugCable endObj)
 * onCableReset(CableController cable)
 * onCableWind(CableController cable, pole orientation, location)
 */

public static class GameEvents
{
    public static Action onGameStart;
    public static void GameStart()
    {
        if (onGameStart != null)
        {
            onGameStart();
        }
    }

    public static Action onGameOver;
    public static void GameOver()
    {
        if (onGameOver != null)
        {
            onGameOver();
        }
    }

    public static Action onTimeUp;
    public static void TimeUp()
    {
        if (onTimeUp != null)
        {
            onTimeUp();
        }
    }

    public static Action onHurt;
    public static void Hurt()
    {
        if (onHurt != null)
        {
            onHurt();
        }
    }

    public static Action<CableController> onCablePickup;
    public static void CablePickup(CableController cable)
    {
        if (onCablePickup != null)
        {
            onCablePickup(cable);
        }
    }

    public static Action<CableController> onCableDrop;
    public static void CableDrop(CableController cable)
    {
        if (onCableDrop != null)
        {
            onCableDrop(cable);
        }
    }

    public static Action<AmpController, CableController> onCableSpawn;
    public static void CableSpawn(AmpController amp, CableController cable)
    {
        if (onCableSpawn !=null)
        {
            onCableSpawn(amp, cable);
        }
    }

    public static Action<CableController, SpeakerController> onCableConnect;
    public static void CableConnect(CableController cable, SpeakerController speaker)
    {
        if (onCableConnect != null)
        {
            onCableConnect(cable, speaker);
        }
    }

    // pluggables compatible
    public static Action<CableController, PlugCable> onCableConnectPlug;
    public static void CableConnectPlug(CableController cable, PlugCable endObj)
    {
        if (onCableConnectPlug != null)
        {
            onCableConnectPlug(cable, endObj);
        }
    }

    public static Action<CableController, SpeakerController> onCableDisconnect;
    public static void CableDisconnect(CableController cable, SpeakerController speaker)
    {
        if (onCableDisconnect != null)
        {
            onCableDisconnect(cable, speaker);
        }
    }

    // pluggables compatible
    public static Action<CableController, PlugCable> onCableDisconnectPlug;
    public static void CableDisconnectPlug(CableController cable, PlugCable endObj)
    {
        if (onCableDisconnectPlug != null)
        {
            onCableDisconnectPlug(cable, endObj);
        }
    }

    public static Action<CableController> onCableReset;
    public static void CableReset(CableController cable)
    {
        if (onCableReset != null)
        {
            onCableReset(cable);
        }
    }

    public static Action<CableController, Vector3> onCableWind;
    public static void CableWind(CableController cable, Vector3 location)
    {
        if (onCableWind != null)
        {
            onCableWind(cable, location);
        }
    }

}
