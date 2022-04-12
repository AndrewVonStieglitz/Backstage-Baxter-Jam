using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
 * onCableDisconnect(CableController cable, SpeakerController speaker)
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

    public static Action<CableController, SpeakerController> onCableDisconnect;
    public static void CableDisconnect(CableController cable, SpeakerController speaker)
    {
        if (onCableDisconnect != null)
        {
            onCableDisconnect(cable, speaker);
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

    public static Action<CableController, PoleController.Orientation, Vector3> onCableWind;
    public static void CableWind(CableController cable, PoleController.Orientation orientation, Vector3 location)
    {
        if (onCableWind != null)
        {
            onCableWind(cable, orientation, location);
        }
    }

}
