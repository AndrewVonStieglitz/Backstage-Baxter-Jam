using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeHUD : MonoBehaviour
{
    [SerializeField] private Transform recipeContainer;

    private song currentSong;

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

        for (int recipeIndex = 0; recipeIndex < currentSong.componentRecipes.Length; recipeIndex++)
        {
            Transform recipeUI = recipeContainer.GetChild(recipeIndex);
            recipe recipe = currentSong.componentRecipes[recipeIndex];

            // Terrible way to get the instrument color, but it works for now
            InstrumentMB instrument = Array.Find(instruments, x => x.identifierSO.itemName == recipe.instrument.itemName);
            int colorIndex = (int)instrument.cableColor;

            recipeUI.GetChild(0).GetComponentInChildren<Image>().sprite = recipe.instrument.sprite;
            recipeUI.GetChild(1).GetComponentInChildren<Image>().sprite = recipe.amp.coloredSprites[colorIndex];

            // Loops through all midAffectors (if any)
            int ingredientIndex = 2;
            for (; ingredientIndex < currentSong.componentRecipes[recipeIndex].midAffectors.Length + 2; ingredientIndex++)
            {
                recipeUI.GetChild(ingredientIndex).GetComponentInChildren<Image>().sprite = recipe.midAffectors[ingredientIndex - 2].coloredSprites[colorIndex];
            }
            recipeUI.GetChild(ingredientIndex).GetComponentInChildren<Image>().sprite = recipe.speaker.coloredSprites[colorIndex];

            // Enables/Disables ingredientUI elements depending on if they are used
            for (int i = 0; i < recipeUI.childCount; i++)
            {
                recipeUI.GetChild(i).gameObject.SetActive(i <= ingredientIndex);
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
