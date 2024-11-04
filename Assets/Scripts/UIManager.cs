using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

class PotProgressPair
{
	public GameObject pot;
	public ProgressBar progressBar;
}
class CustomerOrderImagePair
{
    public GameObject customer;
    public Image orderImage;
}

public class UIManager : MonoBehaviour
{
	private VisualElement root;    // Root of the UI Document
	private List<PotProgressPair> potProgressPairs = new List<PotProgressPair>();
	private List<CustomerOrderImagePair> customerOrderImagePairs = new List<CustomerOrderImagePair>();

	[SerializeField] float potYOffset = 1.5f;
    [SerializeField] float customerYOffset = 1.5f;

    void Start()
	{
		root = GetComponent<UIDocument>().rootVisualElement;
    }
	
	void Update()
	{
        UpdateProgressBars();
		UpdateCustomerOrderImages();
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
        PositionProgressBar(progressBar, pot.transform.position);
		StartCoroutine(DelayDisplaying(progressBar));

        return progressBar;
	}

	public void AddCustomerOrderImage(GameObject customer, Image image)
	{
        // Add to UI
        root.Add(image);
        CustomerOrderImagePair pair = new CustomerOrderImagePair() { customer = customer, orderImage = image };
        customerOrderImagePairs.Add(pair);

        // Position the Image based on the customer's screen position
        PositionCustomerImage(image, customer.transform.position);
        StartCoroutine(DelayDisplaying(image));
    }

	public void RemoveProgressBar(ProgressBar progressBar)
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
        foreach (CustomerOrderImagePair pair in customerOrderImagePairs)
        {
            if (pair.orderImage == image)
            {
                customerOrderImagePairs.Remove(pair);
            }
            root.Remove(image);
            return;
        }
    }

    private void PositionProgressBar(ProgressBar progressBar, Vector3 potPosition)
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

    public void UpdateProgressBars()
	{
		foreach (var pair in potProgressPairs)
		{
			PositionProgressBar(pair.progressBar, pair.pot.transform.position);
		}
	}

	public void UpdateCustomerOrderImages()
	{
		foreach (var pair in customerOrderImagePairs)
		{
			PositionCustomerImage(pair.orderImage, pair.customer.transform.position);
		}
	}

    private IEnumerator DelayDisplaying(VisualElement element)
    {
        yield return null;
        element.style.display = DisplayStyle.Flex;
    }
}
