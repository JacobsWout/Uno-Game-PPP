using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    [SerializeField] string webURL = "https://www.octomangames.com";
    [SerializeField] GameObject howToPlayPanel;

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void HowToPlay()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(true);
        }
    }

    public void ShowWebsite()
    {
        Application.OpenURL(webURL);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
