using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Timer : MonoBehaviour
{
    public float timeLimit = 120f;
    private float timeRemaining;
    
    public TMP_Text timerText; // UI text to show the remaining time
    public GameObject gameOverPanel; // panel for when game is over
    public Button restartButton;
    public Button quitButton;

    private bool timerIsActive = true; // to check if timer is done

    // Start is called before the first frame update
    void Start()
    {
        // initialize remaining time
        timeRemaining = timeLimit;

        // hide panels initially (should be set like that in unity)
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Add listeners to buttons
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartLevel);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    // Update is called once per frame
    void Update()
    {
        // if the timer is active, then count down
        if (timerIsActive)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                timeRemaining = 0;
                Time.timeScale = 0; // pauses game
                ShowGameOver(); // shows game over panel
            }

            if (timerText != null)
            {
                timerText.text = "Time Left: " + Mathf.Round(timeRemaining).ToString();
            }
        }
        
    }

    // show the game over panel
    void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        timerIsActive = true;   // Ensure timer is active when restarting
        timeRemaining = timeLimit; // Reset the timer to the original time limit
    }

    public void QuitGame()
    {
        // quit this application
        Application.Quit();
    }
}
