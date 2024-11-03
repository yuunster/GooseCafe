using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;

class PotProgressPair
{
	public GameObject pot;
	public ProgressBar progressBar;
}

public class UIManager : MonoBehaviour
{
	private VisualElement root;    // Root of the UI Document
	private List<PotProgressPair> pairs = new List<PotProgressPair>();

	[SerializeField] int yOffset = 20;
	[SerializeField] int xOffset = 0;

	void Start()
	{
		root = GetComponent<UIDocument>().rootVisualElement;
    }
	
	void Update()
	{
        UpdateProgressBars();
	}

	public ProgressBar AddPotProgressBar(GameObject pot)
	{
        ProgressBar progressBar = new ProgressBar();
		progressBar.style.height = 50;
		progressBar.style.width = 100;
        progressBar.AddToClassList("progress-fill-red");    // Make progress bar red based on USS.uss style sheet

        // Add to UI
        root.Add(progressBar);
		PotProgressPair pair = new PotProgressPair() { pot = pot, progressBar = progressBar };
		pairs.Add(pair);

		// Position the ProgressBar based on the pot's screen position
		PositionProgressBar(progressBar, pot.transform.position);
		return progressBar;
	}

	public void RemoveProgressBar(ProgressBar progressBar)
	{
		foreach (PotProgressPair pair in pairs)
		{
			if (pair.progressBar == progressBar)
			{
				pairs.Remove(pair);
			}
			root.Remove(progressBar);
			return;
		}
	}

	private void PositionProgressBar(ProgressBar progressBar, Vector3 potPosition)
	{
        Vector3 screen = Camera.main.WorldToScreenPoint(potPosition);
		progressBar.style.left = screen.x - (progressBar.layout.width / 2);
		progressBar.style.top = Screen.height - screen.y;

		var cameraRay = Camera.main.ScreenPointToRay(screen);
		Debug.DrawRay(cameraRay.origin, cameraRay.direction * 100, Color.magenta);

        print("left: " + progressBar.style.left + "|top: " + progressBar.style.top + "Screen: " + Screen.width + "x" + Screen.height);
    }

    // Call this method to update all progress bars
    public void UpdateProgressBars()
	{
		foreach (var pair in pairs)
		{
			PositionProgressBar(pair.progressBar, pair.pot.transform.position);
		}
	}
}
