using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeUI : MonoBehaviour
{
    [SerializeField]
    RecipeData Recipe;

    [SerializeField]
    public Image Input_1;

    [SerializeField]
    public Image Input_2;

    [SerializeField]
    public Image Input_3;

    [SerializeField]
    public Image Input_4;

    [SerializeField]
    public Image Input_5;

    public void UpdateHappinessMeter()
    {

    }

    public void AddRecipe(RecipeData Recipe)
    {
        Input_1.sprite = Recipe.instrument.sprite;
        Input_2.sprite = Recipe.amp.sprite;
        Input_3.sprite = Recipe.speaker.sprite;
    }

    public void RemoveRecipe()
    {

    }

    public void DisplayRecipe()
    {

    }

    public void ClearRecipe()
    {
        Input_1.sprite = null;
        Input_2.sprite = null;
        Input_3.sprite = null;
    }
}
