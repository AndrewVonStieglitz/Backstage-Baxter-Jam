using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeUI : MonoBehaviour
{
    [SerializeField]
    PluggablesSO PluggablesRecpie_1;

    [SerializeField]
    Image Recipe_1;

    [SerializeField]
    Image Recipe_2;

    [SerializeField]
    Image Recipe_3;

    [SerializeField]
    Image Recipe_4;

    [SerializeField]
    Image Recipe_5;

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

    public void AddRecipe()
    {
        Recipe_1.sprite = PluggablesRecpie_1.sprite;
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
