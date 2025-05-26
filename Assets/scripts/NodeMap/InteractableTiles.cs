using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTiles : MonoBehaviour
{
    public string levelName;
    public List<string> lastLevelNames;
    public bool isInteractible = false;
    public bool isPressed = false;

    public List<GameObject> spawnedEnemies;

    private void Start()
    {
        GetComponentInChildren<Light>().enabled = false;
        isInteractible = false;

        if(PlayerPrefs.GetString("LastLevelCleared") == "" || 
            PlayerPrefs.GetString("LastLevelCleared") == null ||
            PlayerPrefs.GetString("LastLevelCleared") == "Level7")
        {
            PlayerPrefs.SetString("LastLevelCleared", "Tutorial");
        }

        foreach(string name in lastLevelNames)
        {
            if (PlayerPrefs.GetString("LastLevelCleared") == name)
            {
                GetComponentInChildren<Light>().enabled = true;
                isInteractible = true;
            }
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
        if(isInteractible && !isPressed)
        {
            FindAnyObjectByType<MapInteractionManager>().AnimateTileGroup(levelName);
            isPressed = true;
        }
    }
}