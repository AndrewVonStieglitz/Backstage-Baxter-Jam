using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeHUD : MonoBehaviour
{
    [SerializeField] private Transform recipeContainer;

    public song currentSong;

    private InstrumentMB[] instruments;

    private void Start()
    {
        instruments = FindObjectsOfType<InstrumentMB>();
    }

    private void OnReadySong(song newSong)
    {
        currentSong = newSong;
    }

    private void OnStartSong()
    {
        // Recipe and Ingredient displays are already set up in the scene so we don't have to constantly delete
        // and reinstantiate objects.
        // This loops through each recipe UI and each ingredient UI, then updates them with the new info
        // If the recipe or ingredient display won't be used, it disables it so the UI automatically rearranges.

        int longestRecipe = 0;

        for (int recipeIndex = 0; recipeIndex < currentSong.componentRecipes.Length; recipeIndex++)
        {
            recipe recipe = currentSong.componentRecipes[recipeIndex];
            int length = 2 + (recipe.amp != null ? 1 : 0) + recipe.midAffectors.Length;
            longestRecipe = Mathf.Max(longestRecipe, length);
        }

        for (int recipeIndex = 0; recipeIndex < currentSong.componentRecipes.Length; recipeIndex++)
        {
            Transform recipeUI = recipeContainer.GetChild(recipeIndex);
            recipe recipe = currentSong.componentRecipes[recipeIndex];

            // Terrible way to get the instrument color, but it works for now
            InstrumentMB instrument = Array.Find(instruments, x => x.identifierSO.itemName == recipe.instrument.itemName);
            int colorIndex = 0;
            if (instrument != null)
            {
                colorIndex = (int)instrument.color;
            }

            recipeUI.GetChild(0).GetComponentInChildren<Image>().sprite = recipe.instrument.sprite;
            int ingredientIndex = 1;

            if (recipe.amp != null)
            {
                recipeUI.GetChild(1).GetComponentInChildren<Image>().sprite = recipe.amp.coloredSprites[colorIndex];
                ingredientIndex++;
            }

            int ingredientOffsetAmp = ingredientIndex;

            // Loops through all midAffectors (if any)
            for (; ingredientIndex < currentSong.componentRecipes[recipeIndex].midAffectors.Length + ingredientOffsetAmp; ingredientIndex++)
            {
                recipeUI.GetChild(ingredientIndex).GetComponentInChildren<Image>().sprite = recipe.midAffectors[ingredientIndex - ingredientOffsetAmp].coloredSprites[colorIndex];
            }
            recipeUI.GetChild(ingredientIndex).GetComponentInChildren<Image>().sprite = recipe.speaker.coloredSprites[colorIndex];

            
            for (int i = 0; i < recipeUI.childCount; i++)
            {
                // Hides/Shows ingredientUI elements depending on if they are used
                recipeUI.GetChild(i).GetComponent<CanvasGroup>().alpha = i <= ingredientIndex ? 1 : 0;
                // Disables all ingredientUI that are past the longest recipe to fix UI
                recipeUI.GetChild(i).gameObject.SetActive(i < longestRecipe);
            }
        }

        for (int i = 0; i < recipeContainer.childCount; i++)
        {
            recipeContainer.GetChild(i).gameObject.SetActive(i < currentSong.componentRecipes.Length);
        }
    }

    private void OnEnable()
    {
        GameEvents.onReadySong += OnReadySong;
        GameEvents.onStartSong += OnStartSong;
    }

    private void OnDisable()
    {
        GameEvents.onReadySong -= OnReadySong;
        GameEvents.onStartSong -= OnStartSong;
    }
}
