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

        if(PlayerPrefs.GetString("LastLevelCleared") == "" || 
            PlayerPrefs.GetString("LastLevelCleared") == null ||
            PlayerPrefs.GetString("LastLevelCleared") == "Level5")
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
            case "EventScene":
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
            case "Level4":
                if (PlayerPrefs.GetString("LastLevelCleared") == "ShopScene")
                {
                    GetComponentInChildren<Light>().enabled = true;
                    isInteractible = true;
                }
                break;
            case "Level5":
                if (PlayerPrefs.GetString("LastLevelCleared") == "Level4" || 
                    PlayerPrefs.GetString("LastLevelCleared") == "EventScene")
                {
                    GetComponentInChildren<Light>().enabled = true;
                    isInteractible = true;
                }
                break;
            default:
                break;
        }

        if (spawnedEnemies.Count >= 3)
        {
            float difficulty = PlayerPrefs.GetFloat("DifficultyMultiplier", 1f);

            for (int i = 0; i < spawnedEnemies.Count; i++)
            {
                spawnedEnemies[i].SetActive(false);
            }

            if(isInteractible)
            {
                if (difficulty == 1)
                {
                    spawnedEnemies[0].SetActive(true);
                }
                else if (difficulty == 1.5f)
                {
                    spawnedEnemies[0].SetActive(true);
                    spawnedEnemies[1].SetActive(true);
                }
                else
                {
                    for (int i = 0; i < spawnedEnemies.Count; i++)
                    {
                        spawnedEnemies[i].SetActive(true);
                    }
                }
            }
        }
    }

    private void OnMouseDown()
    {
        if(isInteractible)
        {
            FindAnyObjectByType<MapInteractionManager>().AnimateTileGroup(levelName);
        }
    }
}