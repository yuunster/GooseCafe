using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class OrderManager : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private GameObject[] possibleOrders;
    [SerializeField] private Texture2D[] possibleImages;
    [SerializeField] private int generateOrderSpeed = 5;
    [SerializeField] private int orderDuration = 30;
    private List<GameObject> currentOrders;
    private VisualElement root;
    private GroupBox groupBox;
    private int score = 0;

    private void Start()
    {
        root = uiDocument.rootVisualElement;
        groupBox = root.Q<GroupBox>("Orders");
        currentOrders = new List<GameObject>();
        GenerateOrder();
        StartCoroutine(WaitToGenerateOrder());
    }

    private void GenerateOrder()
    {
        int randomIndex = Random.Range(0, possibleOrders.Length);
        currentOrders.Add(possibleOrders[randomIndex]);

        // Container
        VisualElement newOrderContainer = new VisualElement();
        newOrderContainer.style.flexDirection = FlexDirection.Column;

        // Image
        Image imageElement = new Image
        {
            image = possibleImages[randomIndex],
            scaleMode = ScaleMode.ScaleToFit
        };
        imageElement.style.width = 100;
        imageElement.style.height = 100;
        imageElement.style.flexGrow = 0;

        // Progress Bar
        ProgressBar progressBar = new ProgressBar();
        progressBar.style.height = 50;
        progressBar.value = 100;    // Start the progress bar at full
        progressBar.title = "";

        newOrderContainer.Add(imageElement);
        newOrderContainer.Add(progressBar);
        groupBox.Add(newOrderContainer);

        StartCoroutine(WaitToIncompleteOrder(newOrderContainer, progressBar));
    }

    private IEnumerator WaitToGenerateOrder()
    {
        yield return new WaitForSeconds(generateOrderSpeed);
        GenerateOrder();
        StartCoroutine(WaitToGenerateOrder());
    }
    private IEnumerator WaitToIncompleteOrder(VisualElement orderContainer, ProgressBar progressBar)
    {
        float elapsedTime = 0f;

        while (elapsedTime < orderDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(1 - (elapsedTime / orderDuration)) * 100; // Calculate progress percentage
            progressBar.value = progress;

            yield return null; // Wait for the next frame
        }

        // Ensure progress bar is empty at the end
        progressBar.value = 0;
        int index = groupBox.IndexOf(orderContainer);
        if (index != -1) RemoveOrder(index);
    }

    private void RemoveOrder(int index)
    {
        currentOrders.RemoveAt(index);
        groupBox.RemoveAt(index);
    }

    private void UpdateScore(int scoreAdjustment)
    {
        this.score += scoreAdjustment;
        Label scoreLabel = root.Q<Label>("Score");
        scoreLabel.text = "Score: " + this.score;
    }

    public void CheckAndFinishOrder(GameObject input)
    {
        foreach (GameObject order in currentOrders)
        {
            if (input.CompareTag(order.tag))    // If a matching order is found
            {
                int index = currentOrders.FindIndex(x => x.CompareTag(order.tag));  // Find index of the matched order
                RemoveOrder(index);
                UpdateScore(100);

                return;
            }
        }
    }
}
