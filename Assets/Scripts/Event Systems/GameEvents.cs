using UnityEngine;
using System;
using Cables;
using Pluggables;

/*
 * Event List ============================
 * 
 * onGameStart()
 * onGameOver()
 * onTimeUp()
 * onHurt()
 * onCablePickup(CableController cable)
 * onCableDrop(CableController cable)
 * onCableConnectPlug(CableController cable, PlugCable endObj)
 * onCableDisconnectPlug(CableController cable, PlugCable endObj)
 * onCableReset(CableController cable)
 * onCableWind(CableController cable, pole orientation, location)
 * onNextSong()
 * onReadySong(song song)
 * onStartSong()
 * onEndAlbum()
 * onStartAlbum()
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

    public static Action<Connection> onConnectionStarted;
    public static void ConnectionStarted(Connection connection)
    {
        if (onConnectionStarted != null)
        {
            onConnectionStarted(connection);
        }
    }

    public static Action<Connection> onConnectionAbandoned;
    public static void ConnectionAbandoned(Connection connection)
    {
        if (onConnectionAbandoned != null)
        {
            onConnectionAbandoned(connection);
        }
    }

    public static Action<Connection, PlugCable> onConnect;
    public static void Connect(Connection connection, PlugCable endObj)
    {
        if (onConnect != null)
        {
            onConnect(connection, endObj);
        }
    }

    public static Action<Connection, PlugCable> onDisconnect;
    public static void Disconnect(Connection connection, PlugCable endObj)
    {
        if (onDisconnect != null)
        {
            onDisconnect(connection, endObj);
        }
    }

    //When a cable is first pulled from an instrument
    public static Action<CableController, InstrumentSO> onCableIntrumentStart;
    public static void CableIntrumentStart(CableController cable, InstrumentSO instrument)
    {
        if (onCableIntrumentStart != null)
        {
            onCableIntrumentStart(cable, instrument);
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

    public static Action<Vector2, Vector2> onPlayerCableCollision;
    public static void PlayerCableCollision(Vector2 position, Vector2 normal)
    {
        onPlayerCableCollision?.Invoke(position, normal);
    }

    public static Action onNextSong;
    public static void NextSong()
    {
        if (onNextSong != null)
        {
            onNextSong();
        }
    }

    public static Action<song> onReadySong;
    public static void ReadySong(song song)
    {
        if (onReadySong != null)
        {
            onReadySong(song);
        }
    }

    public static Action onStartSong;
    public static void StartSong()
    {
        if (onStartSong != null)
        {
            onStartSong();
        }
    }

    public static Action onEndSong;
    public static void EndSong()
    {
        if (onEndSong != null)
        {
            onEndSong();
        }
    }

    public static Action onEndAlbum;
    public static void EndAlbum()
    {
        if (onEndAlbum != null)
        {
            onEndAlbum();
        }
    }

    public static Action onStartAlbum;
    public static void StartAlbum()
    {
        if (onStartAlbum != null)
        {
            onStartAlbum();
        }
    }

    public static Action<recipe> onRecipeCompleted;
    public static void RecipeCompleted(recipe recipe)
    {
        if (onRecipeCompleted != null)
        {
            onRecipeCompleted(recipe);
        }
    }

    public static Action<recipe> onRecipeBroken;
    public static void RecipeBroken(recipe recipe)
    {
        if (onRecipeBroken != null)
        {
            onRecipeBroken(recipe);
        }
    }
}
