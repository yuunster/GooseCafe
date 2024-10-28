using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Combiner : MonoBehaviour
{
    [SerializeField] private GameObject[] required;
    [SerializeField] private GameObject output;

    private BoxCollider coll;
    private Vector3 outputSpawnLocation;

    private List<GameObject> inputted;
    public GameObject outputtedItem;

    private void Start()
    {
        inputted = new List<GameObject>();
        coll = GetComponent<BoxCollider>();
        outputtedItem = null;
        outputSpawnLocation = transform.position +
            new Vector3(0, coll.size.y / 2, 0);   // account for height of combiner collider
            //new Vector3(0, output.GetComponent<CapsuleCollider>().height / 2, 0);   // account for height of outputted item
    }

    // Returns true if successfully inputted. Returns false otherwise.
    public bool Input(GameObject input)
    {
        foreach (GameObject r in required)
        {
            if (r.CompareTag(input.tag)) // Check if input matches a required item
            {
                foreach(GameObject i in inputted)   // Check if input has already been inputted
                {
                    if (i.CompareTag(input.tag)) return false;
                }

                inputted.Add(input);    // Add a reference to the prefab in inputted
                Combine();
                return true;
            }
        }

        return false;
    }

    public void Combine()
    {
        if (inputted.Count != required.Length) return;
        
        foreach (GameObject r in required)
        {
            if (!inputted.Any(i => i.CompareTag(r.tag))) return;    // If a required ingredient is not in inputted, return
        }

        foreach (GameObject i in inputted)
        {
            Destroy(i);
        }
        inputted.Clear();
        outputtedItem = Instantiate(output, outputSpawnLocation, Quaternion.identity);
    }
}
