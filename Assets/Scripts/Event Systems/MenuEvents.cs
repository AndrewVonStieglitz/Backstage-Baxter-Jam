using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class MenuEvents
{
    public static Action onStartSelected;
    public static void StartSelected()
    {
        if (onStartSelected != null)
        {
            onStartSelected();
        }
    }

    public static Action onGameEnded;
    public static void GameEnded()
    {
        if (onGameEnded != null)
        {
            onGameEnded();
        }
    }

    public static Action onReturnToMainMenu;
    public static void ReturnToMainMenu()
    {
        if (onReturnToMainMenu != null)
        {
            onReturnToMainMenu();
        }
    }

    public static Action onGameQuit;
    public static void GameQuit()
    {
        if (onGameQuit != null)
        {
            onGameQuit();
        }
    }
}
