using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeLoader : MonoBehaviour
{
    [SerializeField] Album album;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public struct song
{
    public float duration;
    public recipe[] componentRecipes;

    public song(float durationSO, recipe[] componentRecipesSO)
    {
        duration = durationSO;
        componentRecipes = componentRecipesSO;
    }
}

public struct recipe
{
    public PluggablesSO instrument;
    public PluggablesSO amp;
    public PluggablesSO speaker;
    public PluggablesSO[] midAffectors;
    public AudioClip songPart;

    public recipe(PluggablesSO instrumentSO, PluggablesSO ampSO, PluggablesSO speakerSO, PluggablesSO[] midAffectorsSO, AudioClip songPartSO)
    {
        instrument = instrumentSO;
        amp = ampSO;
        speaker = speakerSO;
        midAffectors = midAffectorsSO;
        songPart = songPartSO;
    }
}
