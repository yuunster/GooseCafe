using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("KitchenTestScene");
    }

    public void ShowRules()
    {
        // need to implement new UI panel for rules here
    }

    public void QuitGame()
    {
        Debug.Log("game quit");
        Application.Quit();
    }
}

