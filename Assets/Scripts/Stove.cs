using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class Stove : MonoBehaviour
{
    [SerializeField] public GameObject heldItem;
    private Coroutine cookCoroutine;

    // Returns true if successfully inputted. Returns false otherwise.
    public bool Input(GameObject input)
    {
        if (heldItem != null) return false;

        heldItem = input;
        heldItem.transform.SetParent(this.transform);
        heldItem.transform.position = this.transform.position + transform.up * 0.7f;
        heldItem.transform.rotation = Quaternion.identity;
        heldItem.GetComponent<Collider>().enabled = false;
        heldItem.GetComponent<Rigidbody>().isKinematic = true;
        if (!heldItem.GetComponent<Pot>().isEmpty) StartCooking();  // If pot is not empty, start cooking

        return true;
    }

    public void ReplacePot(GameObject newPot)
    {
        Destroy(heldItem);
        heldItem = null;
        Input(newPot);
    }

    public void StopCooking()
    {
        if (cookCoroutine != null)
        {
            StopCoroutine(cookCoroutine);
            cookCoroutine = null;
        }
    }

    public void RemovePot()
    {
        heldItem = null;
        StopCooking();
    }

    public void StartCooking()
    {
        StopCooking();
        cookCoroutine = StartCoroutine(CookTimer());
    }    

    private IEnumerator CookTimer()
    {
        Pot potScript = heldItem.GetComponent<Pot>();
        yield return new WaitForSeconds(potScript.cookTime);
        GameObject cookedPot = potScript.Cook();
        ReplacePot(cookedPot);
    }
}
