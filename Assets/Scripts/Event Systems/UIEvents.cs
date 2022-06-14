using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public static class UIEvents
{
    public static Action<recipe> onDisplayRecipe;
    public static void DisplayRecipe(recipe recipe)
    {
        if (onDisplayRecipe != null)
        {
            onDisplayRecipe(recipe);
        }
    }

    public static Action<recipe> onCloseRecipe;
    public static void CloseRecipe(recipe recipe)
    {
        if (onCloseRecipe != null)
        {
            onCloseRecipe(recipe);
        }
    }

    public static Action onClearRecipes;
    public static void ClearRecipes()
    {
        if (onClearRecipes != null)
        {
            onClearRecipes();
        }
    }
}
