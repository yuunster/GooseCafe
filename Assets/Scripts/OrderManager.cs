using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

struct Order
{
    public GameObject GameObject;
    public VisualElement VisualElement;
    public GameObject Customer;
}

public class OrderManager : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private GameObject customerSpawner;
    [SerializeField] private GameObject customerWaitingArea;
    [SerializeField] private GameObject orderCounter;
    [SerializeField] private GameObject[] possibleOrders;
    [SerializeField] private Texture2D[] possibleImages;
    [SerializeField] private int generateCustomerSpeed = 5;
    [SerializeField] private int orderDuration = 30;
    [SerializeField] private int maxCustomers = 20;
    [SerializeField] private int patienceDuration = 10;
    private List<Order> currentOrders;
    private List<GameObject> waitingCustomers;
    private VisualElement root;
    private GroupBox groupBox;
    private int score = 0;
    private Dictionary<string, int> scoreMap = new Dictionary<string, int>
    {
        { "MilkTea", 100 },
        { "MilkTeaBoba", 300 }
    };
    private UIManager uiManager;

    private void Awake()
    {
        uiManager = FindObjectOfType<UIManager>();
    }

    private void Start()
    {
        root = uiDocument.rootVisualElement;
        groupBox = root.Q<GroupBox>("Orders");
        currentOrders = new List<Order>();
        waitingCustomers = new List<GameObject>();

        GenerateCustomer();
        StartCoroutine(WaitToGenerateCustomer());
    }

    private void AddOrder(int orderIndex, GameObject customer)
    {
        Order newOrder = new Order();
        newOrder.GameObject = possibleOrders[orderIndex];
        newOrder.Customer = customer;

        // Container
        VisualElement newOrderContainer = new VisualElement();
        newOrderContainer.style.flexDirection = FlexDirection.Column;

        // Image
        Image imageElement = new Image
        {
            image = possibleImages[orderIndex],
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
        progressBar.Q("", "unity-progress-bar__progress").style.backgroundColor = Color.red;
        newOrderContainer.Add(imageElement);
        newOrderContainer.Add(progressBar);
        newOrder.VisualElement = newOrderContainer;

        groupBox.Add(newOrderContainer);
        currentOrders.Add(newOrder);

        StartCoroutine(WaitToIncompleteOrder(newOrder, progressBar));
    }

    private int GenerateOrder()
    {
        return Random.Range(0, possibleOrders.Length);
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
        StartCoroutine(RemoveCustomer(order.Customer));
    }

    private void RemoveOrder(int index)
    {
        currentOrders.RemoveAt(index);
        groupBox.RemoveAt(index);
    }

    private IEnumerator RemoveCustomer(GameObject customer)
    {
        Customer customerScript = customer.GetComponent<Customer>();

        customerScript.navAgent.SetDestination(customerSpawner.transform.position);
        customerScript.leaving = true;

        yield return new WaitForSeconds(5);

        if (customerScript.label != null) uiManager.RemoveCustomerLabel(customerScript.label);
        Destroy(customer);
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
                StartCoroutine(RemoveCustomer(order.Customer));
                UpdateScore(scoreMap[input.tag]);   // Update score based on the items score value defined at the beginning of this class

                return;
            }
        }
    }

    private void GenerateCustomer()
    {
        if (waitingCustomers.Count >= maxCustomers) return;

        int randomIndex = Random.Range(0, GameAssets.i.customerPrefabs.Length);
        GameObject customer = Instantiate(GameAssets.i.customerPrefabs[randomIndex], customerSpawner.transform.position, Quaternion.identity);
        Customer customerScript = customer.GetComponent<Customer>();

        int orderIndex = GenerateOrder();
        customerScript.orderIndex = orderIndex;

        // Image
        Image image = new Image { image = possibleImages[orderIndex] };
        image.style.width = 100;
        image.style.height = 100;
        image.style.flexGrow = 0;
        image.style.position = Position.Absolute;
        image.style.display = DisplayStyle.None;    // Hide image until correctly positioned by UIManager

        customerScript.image = image;
        uiManager.AddCustomerOrderImage(customer, image);


        if (waitingCustomers.Count == 0)    // If first customer, navigate to orderCounter. Else, navigate to the last person in line.
        {
            customer.GetComponent<Customer>().navAgent.SetDestination(orderCounter.transform.position);
            customerScript.patienceTimer = StartCoroutine(StartPatienceTimer(customerScript));
        }
        else
        {
            customer.GetComponent<Customer>().leader = waitingCustomers[waitingCustomers.Count - 1].transform;   // Make the new customer follow the last customer in line
        }

        waitingCustomers.Add(customer);
    }

    private IEnumerator WaitToGenerateCustomer()
    {
        yield return new WaitForSeconds(generateCustomerSpeed);
        GenerateCustomer();
        StartCoroutine(WaitToGenerateCustomer());
    }

    public void TakeOrder()
    {
        if (waitingCustomers.Count == 0) return;

        Customer firstCustomerScript = waitingCustomers[0].GetComponent<Customer>();
        if (waitingCustomers.Count > 1)
        {
            Customer secondCustomerScript = waitingCustomers[1].GetComponent<Customer>();
            secondCustomerScript.navAgent.SetDestination(orderCounter.transform.position);
            secondCustomerScript.leader = null;
            secondCustomerScript.patienceTimer = StartCoroutine(StartPatienceTimer(secondCustomerScript));
        }

        uiManager.RemoveCustomerImage(firstCustomerScript.image);
        AddOrder(firstCustomerScript.orderIndex, firstCustomerScript.gameObject);
        firstCustomerScript.navAgent.SetDestination(customerWaitingArea.transform.position);

        waitingCustomers.Remove(firstCustomerScript.gameObject);

        firstCustomerScript.waiting = true;
        StopCoroutine(firstCustomerScript.patienceTimer);
    }

    private IEnumerator StartPatienceTimer(Customer customer)
    {
        yield return new WaitForSeconds(patienceDuration);
        CustomerPatienceTimeOut(customer);
    }

    private void CustomerPatienceTimeOut(Customer customer)
    {
        UpdateScore(-100);
        uiManager.RemoveCustomerImage(customer.image);
        customer.label = uiManager.AddCustomerLabel(customer.gameObject, ">:(");

        if (waitingCustomers.Count > 1)
        {
            Customer secondCustomerScript = waitingCustomers[1].GetComponent<Customer>();
            secondCustomerScript.navAgent.SetDestination(orderCounter.transform.position);
            secondCustomerScript.leader = null;
            secondCustomerScript.patienceTimer = StartCoroutine(StartPatienceTimer(secondCustomerScript));
        }

        waitingCustomers.Remove(customer.gameObject);
        StartCoroutine(RemoveCustomer(customer.gameObject));
    }
}
