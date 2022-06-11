using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeData : ScriptableObject
{
    public recipe Recipe
    {
        get
        {
            return new recipe(instrument, amp, speaker, midAffectors, musicPart);
        }
    }

    public PluggablesSO instrument;
    public PluggablesSO amp;
    public PluggablesSO speaker;
    public PluggablesSO[] midAffectors;
    public AudioClip musicPart;
}
