using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.MessageBox;

struct Order
{
    public GameObject GameObject;
    public VisualElement VisualElement;
}

public class OrderManager : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private GameObject[] possibleOrders;
    [SerializeField] private Texture2D[] possibleImages;
    [SerializeField] private int generateOrderSpeed = 5;
    [SerializeField] private int orderDuration = 30;
    private List<Order> currentOrders;
    private VisualElement root;
    private GroupBox groupBox;
    private int score = 0;
    private Dictionary<string, int> scoreMap = new Dictionary<string, int>
    {
        { "MilkTea", 100 },
        { "MilkTeaBoba", 300 }
    };

    private void Start()
    {
        root = uiDocument.rootVisualElement;
        groupBox = root.Q<GroupBox>("Orders");
        currentOrders = new List<Order>();
        GenerateOrder();
        StartCoroutine(WaitToGenerateOrder());
    }

    private void GenerateOrder()
    {
        Order newOrder = new Order();
        int randomIndex = Random.Range(0, possibleOrders.Length);
        newOrder.GameObject = possibleOrders[randomIndex];

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
        progressBar.AddToClassList("progress-fill-red");    // Make progress bar red based on USS.uss style sheet

        newOrderContainer.Add(imageElement);
        newOrderContainer.Add(progressBar);
        newOrder.VisualElement = newOrderContainer;

        groupBox.Add(newOrderContainer);
        currentOrders.Add(newOrder);

        StartCoroutine(WaitToIncompleteOrder(newOrder, progressBar));
    }

    private IEnumerator WaitToGenerateOrder()
    {
        yield return new WaitForSeconds(generateOrderSpeed);
        GenerateOrder();
        StartCoroutine(WaitToGenerateOrder());
    }
    private IEnumerator WaitToIncompleteOrder(Order order, ProgressBar progressBar)
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
        int index = currentOrders.IndexOf(order);
        RemoveOrder(index);
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
        foreach (Order order in currentOrders)
        {
            if (input.CompareTag(order.GameObject.tag))    // If a matching order is found
            {
                int index = currentOrders.FindIndex(x => x.GameObject.CompareTag(input.tag));  // Find index of the matched order
                RemoveOrder(index);
                UpdateScore(scoreMap[input.tag]);   // Update score based on the items score value defined at the beginning of this class

                return;
            }
        }
    }
}
