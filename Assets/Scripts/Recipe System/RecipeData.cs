using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RecipeSystemScriptableObjects/New Recipe")]
public class RecipeData : ScriptableObject
{
    public recipe Recipe
    {
        get
        {
            return new recipe(instrument, amp, speaker, midAffectors, musicPart);
        }
    }

    public InstrumentSO instrument;
    public MidAffectorSuper amp;
    public SpeakerSuper speaker;
    public MidAffectorSuper[] midAffectors;
    public AudioClip musicPart;
}
