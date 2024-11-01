using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets _i;
    public static GameAssets i {
        get
        {
            if (_i == null) _i = Instantiate(Resources.Load<GameAssets>("GameAssets"));
            return _i;
        }
    }

    public GameObject[] ingredients;
    public string[] ingredientTags;
    public GameObject pot;

    private void Start()
    {
        ingredientTags = new string[ingredients.Length];
        for (int i = 0; i < ingredients.Length; i++)
        {
            ingredientTags[i] = ingredients[i].tag;
        }
    }
}
