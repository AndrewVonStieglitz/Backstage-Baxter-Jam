using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public static class UIEvents
{
    static PluggablesSO pluggables;
    static RecipeUI recipeUI;
    private static void UpdateHappinessMeter()
    {

    }

    public static void AddRecipe(RecipeData Recipe)
    {
        recipeUI.Input_1.sprite = Recipe.instrument.sprite;
        recipeUI.Input_2.sprite = Recipe.amp.sprite;
        recipeUI.Input_3.sprite = Recipe.speaker.sprite;
    }

    private static void RemoveRecipe()
    {

    }

    private static void DisplayRecipe()
    {

    }

    private static void ClearRecipe()
    {

    }
}
