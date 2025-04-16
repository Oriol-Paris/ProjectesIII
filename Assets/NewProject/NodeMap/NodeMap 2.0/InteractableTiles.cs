using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTiles : MonoBehaviour
{
    public string levelName;
    public bool isInteractible = false;

    public List<GameObject> spawnedEnemies;

    private void Start()
    {
        GetComponentInChildren<Light>().enabled = false;
        isInteractible = false;

        if(PlayerPrefs.GetString("LastLevelCleared") == "" || PlayerPrefs.GetString("LastLevelCleared") == null)
        {
            PlayerPrefs.SetString("LastLevelCleared", "Tutorial");
        }

        switch (levelName)
        {
            case "Level1":
                if(PlayerPrefs.GetString("LastLevelCleared") == "Tutorial")
                {
                    GetComponentInChildren<Light>().enabled = true;
                    isInteractible = true;
                }
                break;
            case "Level2":
            case "Level3":
                if (PlayerPrefs.GetString("LastLevelCleared") == "Level1")
                {
                    GetComponentInChildren<Light>().enabled = true;
                    isInteractible = true;
                }
                break;
            case "Level4":
                if (PlayerPrefs.GetString("LastLevelCleared") == "Level2")
                {
                    GetComponentInChildren<Light>().enabled = true;
                    isInteractible = true;
                }
                break;
            case "ShopScene":
                if (PlayerPrefs.GetString("LastLevelCleared") == "Level3")
                {
                    GetComponentInChildren<Light>().enabled = true;
                    isInteractible = true;
                }
                break;
            case "Level5":
                if (PlayerPrefs.GetString("LastLevelCleared") == "ShopScene")
                {
                    GetComponentInChildren<Light>().enabled = true;
                    isInteractible = true;
                }
                break;
            case "Level6":
                if (PlayerPrefs.GetString("LastLevelCleared") == "Level4" || 
                    PlayerPrefs.GetString("LastLevelCleared") == "Level5")
                {
                    GetComponentInChildren<Light>().enabled = true;
                    isInteractible = true;
                }
                break;
            default:
                break;
        }

        /*if(levelName != "ShopScene")
        {
            if (PlayerPrefs.GetFloat("DifficultyMultiplier") == 1)
            {
                spawnedEnemies[0].SetActive(true);
                spawnedEnemies[1].SetActive(false);
                spawnedEnemies[2].SetActive(false);
            }
            else if (PlayerPrefs.GetFloat("DifficultyMultiplier") == 1.5f)
            {
                spawnedEnemies[0].SetActive(true);
                spawnedEnemies[1].SetActive(true);
                spawnedEnemies[2].SetActive(false);
            }
            else
            {
                spawnedEnemies[0].SetActive(true);
                spawnedEnemies[1].SetActive(true);
                spawnedEnemies[2].SetActive(true);
            }
        }*/
    }

    private void OnMouseDown()
    {
        if(isInteractible)
        {
            FindAnyObjectByType<MapInteractionManager>().AnimateTileGroup(levelName);
        }
    }
}