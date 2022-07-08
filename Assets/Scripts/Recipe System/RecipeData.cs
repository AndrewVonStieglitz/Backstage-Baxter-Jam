using System.Collections;
using System.Collections.Generic;
using Pluggables;
using UnityEngine;

[CreateAssetMenu(menuName = "RecipeSystemScriptableObjects/New Recipe")]
public class RecipeData : ScriptableObject
{

    public InstrumentSO instrument;
    public MidAffectorSuper amp;
    public SpeakerSuper speaker;
    public MidAffectorSuper[] midAffectors;
    public AudioClip musicPart;

    public recipe Recipe
    {
        get
        {
            return new recipe(instrument, amp, speaker, midAffectors, musicPart);
        }
    }
}
