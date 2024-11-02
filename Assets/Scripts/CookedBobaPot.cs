using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookedBobaPot : MonoBehaviour
{
    [SerializeField] private int uses = 1;
    [SerializeField] private GameObject emptyPot;

    // Returns a new pot gameobject if used
    public GameObject UseBoba()
    {
        uses--;

        if (uses <= 0)
        {
            GameObject newPot = Instantiate(emptyPot, transform.position, transform.rotation);
            Destroy(this.gameObject);
            return newPot;
        }

        return null;
    }
}
