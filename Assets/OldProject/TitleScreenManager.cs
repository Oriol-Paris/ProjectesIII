using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] private Canvas settings;
    [SerializeField] private Canvas titleScreen;
    public void StartGame()
    {
        SceneManager.LoadScene("Tutorial");
    }
    public void BootSettings()
    {
        settings.enabled = true;
        titleScreen.enabled = false;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        settings.enabled = false;
        titleScreen.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            if(settings.enabled)
            {
                settings.enabled = false;
                titleScreen.enabled = true;
            }
        }
    }
    public void CloseGame()
    {
        Application.Quit();
    }
}
