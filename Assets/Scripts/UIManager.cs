using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

class PotProgressPair
{
	public GameObject pot;
	public ProgressBar progressBar;
}
class CustomerImagePair
{
    public GameObject customer;
    public Image image;
}
class CustomerLabelPair
{
    public GameObject customer;
    public Label label;
}

class CustomerProgressPair
{
    public GameObject customer;
    public ProgressBar progressBar;
}

public class UIManager : MonoBehaviour
{
	private VisualElement root;    // Root of the UI Document
	private List<PotProgressPair> potProgressPairs = new List<PotProgressPair>();
	private List<CustomerImagePair> customerImagePairs = new List<CustomerImagePair>();
    private List<CustomerLabelPair> customerLabelPairs = new List<CustomerLabelPair>();
    private List<CustomerProgressPair> customerProgressPairs = new List<CustomerProgressPair>();

    [SerializeField] private UIDocument uiDocument;
    [SerializeField] float potYOffset = 1.5f;
    [SerializeField] float customerYOffset = 1.5f;

    void Awake()
	{
		root = uiDocument.rootVisualElement;
    }
	
	void Update()
	{
        UpdateProgressBars();
		UpdateCustomerOrderImages();
        UpdateCustomerLabels();
        UpdateCustomerProgressBars();
	}

	public ProgressBar AddPotProgressBar(GameObject pot)
	{
        ProgressBar progressBar = new ProgressBar();
		progressBar.style.height = 50;
		progressBar.style.width = 100;
        progressBar.style.position = Position.Absolute;
		progressBar.style.display = DisplayStyle.None;	// Hide ProgressBar until properly positioned
        progressBar.Q("", "unity-progress-bar__progress").style.backgroundColor = Color.red;

        // Add to UI
        root.Add(progressBar);
        PotProgressPair pair = new PotProgressPair() { pot = pot, progressBar = progressBar };
        potProgressPairs.Add(pair);

        // Position the ProgressBar based on the pot's screen position
        PositionPotProgressBar(progressBar, pot.transform.position);
		StartCoroutine(DelayDisplaying(progressBar));

        return progressBar;
	}

	public void AddCustomerOrderImage(GameObject customer, Image image)
	{
        // Add to UI
        root.Add(image);
        CustomerImagePair pair = new CustomerImagePair() { customer = customer, image = image };
        customerImagePairs.Add(pair);

        // Position the Image based on the customer's screen position
        PositionCustomerImage(image, customer.transform.position);
        StartCoroutine(DelayDisplaying(image));
    }

    public Label AddCustomerLabel(GameObject customer, string text)
    {
        Label label = new Label() { text = text };
        label.style.fontSize = 50;
        label.style.unityFontStyleAndWeight = FontStyle.Bold;
        label.style.color = Color.white;
        label.style.unityTextOutlineColor = Color.red;
        label.style.unityTextOutlineWidth = 10;
        label.style.position = Position.Absolute;
        label.style.display = DisplayStyle.None;

        CustomerLabelPair pair = new CustomerLabelPair() { customer = customer, label = label };
        customerLabelPairs.Add(pair);

        root.Add(label);
        PositionCustomerLabel(label, customer.transform.position);
        StartCoroutine(DelayDisplaying(label));

        return label;
    }

    public ProgressBar AddCustomerProgressBar(GameObject customer)
    {
        ProgressBar progressBar = new ProgressBar();
        progressBar.style.height = 50;
        progressBar.style.width = 100;
        progressBar.style.position = Position.Absolute;
        progressBar.style.display = DisplayStyle.None;	// Hide ProgressBar until properly positioned
        progressBar.Q("", "unity-progress-bar__progress").style.backgroundColor = Color.red;

        // Add to UI
        root.Add(progressBar);
        CustomerProgressPair pair = new CustomerProgressPair() { customer = customer, progressBar = progressBar };
        customerProgressPairs.Add(pair);

        // Position the ProgressBar based on the pot's screen position
        PositionCustomerProgressBar(progressBar, customer.transform.position);
        StartCoroutine(DelayDisplaying(progressBar));

        return progressBar;
    }

    public void RemovePotProgressBar(ProgressBar progressBar)
	{
		foreach (PotProgressPair pair in potProgressPairs)
		{
			if (pair.progressBar == progressBar)
			{
				potProgressPairs.Remove(pair);
			}
			root.Remove(progressBar);
			return;
		}
	}
    public void RemoveCustomerImage(Image image)
    {
        foreach (CustomerImagePair pair in customerImagePairs)
        {
            if (pair.image == image)
            {
                customerImagePairs.Remove(pair);
            }
            root.Remove(image);
            return;
        }
    }
    public void RemoveCustomerLabel(Label label)
    {
        foreach (CustomerLabelPair pair in customerLabelPairs)
        {
            if (pair.label == label)
            {
                customerLabelPairs.Remove(pair);
            }
            root.Remove(label);
            return;
        }
    }
    public void RemoveCustomerProgressBar(ProgressBar progressBar)
    {
        foreach (CustomerProgressPair pair in customerProgressPairs)
        {
            if (pair.progressBar == progressBar)
            {
                customerProgressPairs.Remove(pair);
            }
            root.Remove(progressBar);
            return;
        }
    }

    private void PositionPotProgressBar(ProgressBar progressBar, Vector3 potPosition)
	{
        Vector3 screen = Camera.main.WorldToScreenPoint(potPosition + new Vector3(0, 0.4f, 0)); // Displace by height of pot
		progressBar.style.left = screen.x - (progressBar.contentRect.width / 2);
		progressBar.style.top = Screen.height - screen.y - potYOffset;
    }
    private void PositionCustomerImage(Image image, Vector3 customerPosition)
    {
        Vector3 screen = Camera.main.WorldToScreenPoint(customerPosition + new Vector3(0, 1.4f, 0));    // Displace by height of Customer
        image.style.left = screen.x - (image.contentRect.width / 2);
        image.style.top = Screen.height - screen.y - customerYOffset;
    }

    private void PositionCustomerLabel(Label label, Vector3 customerPosition)
    {
        Vector3 screen = Camera.main.WorldToScreenPoint(customerPosition + new Vector3(0, 1.4f, 0));    // Displace by height of Customer
        label.style.left = screen.x - (label.contentRect.width / 2);
        label.style.top = Screen.height - screen.y - customerYOffset;
    }

    private void PositionCustomerProgressBar(ProgressBar progressBar, Vector3 customerPosition)
    {
        Vector3 screen = Camera.main.WorldToScreenPoint(customerPosition + new Vector3(0, 1.4f, 0));    // Displace by height of Customer
        progressBar.style.left = screen.x - (progressBar.contentRect.width / 2);
        progressBar.style.top = Screen.height - screen.y - customerYOffset + 100;
    }

    private void UpdateProgressBars()
	{
		foreach (var pair in potProgressPairs)
		{
			PositionPotProgressBar(pair.progressBar, pair.pot.transform.position);
		}
	}

	private void UpdateCustomerOrderImages()
	{
		foreach (var pair in customerImagePairs)
		{
			PositionCustomerImage(pair.image, pair.customer.transform.position);
		}
	}

    private void UpdateCustomerLabels()
    {
        foreach (var pair in customerLabelPairs)
        {
            PositionCustomerLabel(pair.label, pair.customer.transform.position);
        }
    }
    private void UpdateCustomerProgressBars()
    {
        foreach (var pair in customerProgressPairs)
        {
            PositionCustomerProgressBar(pair.progressBar, pair.customer.transform.position);
        }
    }

    private IEnumerator DelayDisplaying(VisualElement element)
    {
        yield return null;
        element.style.display = DisplayStyle.Flex;
    }
}
