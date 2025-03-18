using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] private Canvas settings;
    [SerializeField] private Canvas titleScreen;
    [SerializeField] private Canvas creditsScreen;

    [SerializeField] private Button continueButton;
    [SerializeField] private PlayerData playerData;

    private const string LEVEL_START_PATH = "/PlayerDataLevelStart.json";
    private const string PLAYERDATA_START_PATH = "/PlayerData.json";
    private const string NODEMAP_START_PATH = "/NodeMapData.json";

    public void Continue()
    {
        if(File.Exists(Application.persistentDataPath + NODEMAP_START_PATH))
        {
            SceneManager.LoadScene("NodeMap");
        }
        else
        {
            SceneManager.LoadScene("Tutorial");
        }
    }

    public void NewGame()
    {
        string levelStartPath = Application.persistentDataPath + LEVEL_START_PATH;
        string playerDataPath = Application.persistentDataPath + PLAYERDATA_START_PATH;
        string nodeMapPath = Application.persistentDataPath + NODEMAP_START_PATH;

        File.Delete(levelStartPath);
        File.Delete(playerDataPath);
        File.Delete(nodeMapPath);

        playerData.LoadOriginalPlayer();

        SceneManager.LoadScene("Tutorial");
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
        Cursor.visible = true;

        settings.enabled = false;
        creditsScreen.enabled = false;
        titleScreen.enabled = true;
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
