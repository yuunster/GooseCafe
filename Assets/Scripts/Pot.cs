using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pot : MonoBehaviour
{
    [SerializeField] private GameObject[] required;
    [SerializeField] private GameObject output;
    [SerializeField] private GameObject trash;

    public float cookTime = 5f;
    public bool isEmpty => inputted.Count == 0;

    private List<GameObject> inputted;

    private void Awake()
    {
        inputted = new List<GameObject>();
    }

    public void Input(GameObject input)
    {
        inputted.Add(input);
    }

    // Returns the GameObject this pot is transforming into
    public GameObject Cook()
    {
        if (CheckValid()) return Instantiate(output);
        else return Instantiate(trash);
    }

    private bool CheckValid()
    {
        if (inputted.Count != required.Length) return false;

        foreach (GameObject r in required)
        {
            if (!inputted.Any(i => i.CompareTag(r.tag))) return false;    // If a required ingredient is not in inputted, return
        }

        return true;
    }
}
