using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] private Canvas settings;
    [SerializeField] private Canvas titleScreen;
    [SerializeField] private Canvas creditsScreen;
    public void StartGame()
    {
        SceneManager.LoadScene("Node Map");
    }
    public void BootSettings()
    {
        creditsScreen.enabled = false;
        settings.enabled = true;
        titleScreen.enabled = false;
    }
    public void BootCredits()
    {
        creditsScreen.enabled = true;
        settings.enabled = false;
        titleScreen.enabled = false;
    }

    public void ExitCredits()
    {
        if (creditsScreen.enabled)
        {
            settings.enabled = false;
            creditsScreen.enabled = false;
            titleScreen.enabled = true;
        }
    } 
    public void ExitSettings()
    {
        if (settings.enabled)
        {
            settings.enabled = false;
            creditsScreen.enabled = false;
            titleScreen.enabled = true;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        settings.enabled = false;
        creditsScreen.enabled = false;
        titleScreen.enabled = true;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
    public void CloseGame()
    {
        Application.Quit();
    }
}
