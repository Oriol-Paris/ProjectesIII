using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    [Header("Canvas Settings")]
    [SerializeField] private Canvas settings;
    [SerializeField] private Canvas language;
    [SerializeField] private Canvas titleScreen;
    [SerializeField] private Canvas creditsScreen;

    [Header("Button Settings")]
    [SerializeField] private Button continueButton;
    [SerializeField] private PlayerData playerData;

    [Header("AudioSource Settings")]
    [SerializeField] private AudioSource enterSfxSource;
    [SerializeField] private AudioSource exitSfxSource;

    private const string LEVEL_START_PATH = "/PlayerDataLevelStart.json";
    private const string PLAYERDATA_START_PATH = "/PlayerData.json";
    private const string NODEMAP_START_PATH = "/NodeMapData.json";

    public void Continue()
    {
        StartCoroutine(PlaySFX(false));
    }

    public void NewGame()
    {
        StartCoroutine(PlaySFX(true));
    }

    public void BootSettings()
    {
        enterSfxSource.Play();
        creditsScreen.enabled = false;
        settings.enabled = true;
       
        titleScreen.enabled = false;
        language.enabled = false;
    }
    public void BootCredits()
    {
        enterSfxSource.Play();
        creditsScreen.enabled = true;
        settings.enabled = false;
        titleScreen.enabled = false;
        language.enabled = false;
    }
    public void BootLanguage()
    {
        enterSfxSource.Play();
        creditsScreen.enabled = false;
        language.enabled = true;
        settings.enabled = false;
        titleScreen.enabled = false;
    }

    public void ExitCredits()
    {
        if (creditsScreen.enabled)
        {
            exitSfxSource.Play();
            settings.enabled = false;
            creditsScreen.enabled = false;
            language.enabled = false;
            titleScreen.enabled = true;
        }
    } 
    public void ExitSettings()
    {
        if (settings.enabled)
        {
            exitSfxSource.Play();
            settings.enabled = false;
            creditsScreen.enabled = false;
            language.enabled = false;
            titleScreen.enabled = true;
        }
    }
    public void ExitLanguage()
    {
        if (language.enabled)
        {
            exitSfxSource.Play();
            settings.enabled = false;
            creditsScreen.enabled = false;
            language.enabled = false;
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
        exitSfxSource.Play();
        Application.Quit();
    }

    private IEnumerator PlaySFX(bool newGame)
    {
        enterSfxSource.Play();
        yield return new WaitForSeconds(0.1f);
        
        if(newGame)
        {
            string levelStartPath = Application.persistentDataPath + LEVEL_START_PATH;
            string playerDataPath = Application.persistentDataPath + PLAYERDATA_START_PATH;
            string nodeMapPath = Application.persistentDataPath + NODEMAP_START_PATH;

            File.Delete(levelStartPath);
            File.Delete(playerDataPath);
            File.Delete(nodeMapPath);

            playerData.CopyDataFrom(playerData.originalPlayer);
            playerData.Save();
            PlayerPrefs.SetString("EneteredMap", "StarterNodeMap");
            PlayerPrefs.SetFloat("DifficultyMultiplier", 1);
            PlayerPrefs.SetString("LastLevelCleared", "");

            SceneManager.LoadScene("Tutorial");
        }
        else
        {
            if(PlayerPrefs.GetString("EneteredMap") != "")
            {
                SceneManager.LoadScene(PlayerPrefs.GetString("EneteredMap"));
            }
            else
            {
                SceneManager.LoadScene("StarterNodeMap");
            }
        }
    }
}
