using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class Stove : MonoBehaviour
{
    [SerializeField] public GameObject heldItem;
    [SerializeField] private UIManager uiManager;

    private Coroutine cookCoroutine;
    private ProgressBar progressBar;

    // Returns true if successfully inputted. Returns false otherwise.
    public bool Input(GameObject input)
    {
        if (heldItem != null) return false;

        heldItem = input;
        heldItem.transform.SetParent(this.transform);
        heldItem.transform.position = this.transform.position + transform.up * GetComponent<BoxCollider>().size.y / 2 + transform.up * heldItem.GetComponent<BoxCollider>().size.y / 2;
        heldItem.transform.rotation = transform.rotation;
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
            uiManager.RemoveProgressBar(progressBar);
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
        progressBar = uiManager.AddPotProgressBar(heldItem);
        cookCoroutine = StartCoroutine(CookTimer());
    }    

    private IEnumerator CookTimer()
    {
        Pot potScript = heldItem.GetComponent<Pot>();

        float elapsedTime = 0f;

        while (elapsedTime < potScript.cookTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(1 - (elapsedTime / potScript.cookTime)) * 100; // Calculate progress percentage
            progressBar.value = 100 - progress;

            yield return null; // Wait for the next frame
        }

        // Ensure progress bar is empty at the end
        progressBar.value = 0;

        uiManager.RemoveProgressBar(progressBar);

        GameObject cookedPot = potScript.Cook();
        ReplacePot(cookedPot);
    }
}
