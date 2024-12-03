using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject rulesPanel;
    public void PlayGame()
    {
        SceneManager.LoadScene("KitchenTestScene");
    }

    public void ShowSettings()
    {
        SceneManager.LoadScene("settings");
    }

    public void HideSettings()
    {
        SceneManager.LoadScene("MainMenu");  
    }

    public void ShowRules()
    {
        rulesPanel.SetActive(true);
    }

    public void HideRules()
    {
        rulesPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("game quit");
        Application.Quit();
    }
}

