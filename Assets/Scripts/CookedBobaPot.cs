using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookedBobaPot : MonoBehaviour
{
    [SerializeField] private int uses = 1;
    [SerializeField] private GameObject emptyPot;

    public void UseBoba()
    {
        uses--;

        if (uses <= 0)
        {
            Instantiate(emptyPot, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
