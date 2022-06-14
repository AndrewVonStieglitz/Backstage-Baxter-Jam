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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
        
    }
}
