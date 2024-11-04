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

    public GameObject[] ingredientPrefabs;
    public string[] ingredientTags;
    public GameObject pot;
    public GameObject[] customerPrefabs;

    private void Start()
    {
        ingredientTags = new string[ingredientPrefabs.Length];
        for (int i = 0; i < ingredientPrefabs.Length; i++)
        {
            ingredientTags[i] = ingredientPrefabs[i].tag;
        }
    }
}
