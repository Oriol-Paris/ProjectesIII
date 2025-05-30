using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapInteractionManager : MonoBehaviour
{
    [Serializable]
    public struct TileGroup
    {
        public string groupName;
        public List<AnimateMapTile> tiles;
    }

    public List<AnimateMapTile> interactableTiles;

    public List<TileGroup> tileGroups;

    private void Start()
    {
        if(PlayerPrefs.GetFloat("DifficultyMultiplier") == 0)
        {
            PlayerPrefs.SetFloat("DifficultyMultiplier", 1);
        }

        if(PlayerPrefs.GetString("LastLevelCleared") != "Tutorial")
        {
            AnimateTileGroup(PlayerPrefs.GetString("LastLevelCleared"), false);
        }

        PlayerPrefs.SetString("EneteredMap", SceneManager.GetActiveScene().name);
    }

    public void AnimateTileGroup(string groupName, bool loadNextScene = true)
    {
        foreach(TileGroup group in tileGroups)
        {
            if (group.groupName == groupName)
            {
                if(loadNextScene)
                {
                    StartCoroutine(TileGroupAnim(group.tiles, groupName));
                }
                else
                {
                    StartCoroutine(TileGroupAnim(group.tiles));
                }
            }
        }
    }

    IEnumerator TileGroupAnim(List<AnimateMapTile> tiles, string name = null)
    {
        if(name != null)
        {
            FindAnyObjectByType<PlayerPathFollower>().SetTilePath(tiles, name);
        }

        foreach (AnimateMapTile tile in tiles)
        {
            tile.wasVisited = true;
            tile.AnimateTile();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Keypad1))
        {
            SceneManager.LoadScene("Level1");
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            SceneManager.LoadScene("Level2");
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            SceneManager.LoadScene("Level3");
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            SceneManager.LoadScene("Level4");
        }
        else if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            SceneManager.LoadScene("Level5");
        }
        else if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            SceneManager.LoadScene("Level6");
        }
        else if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            SceneManager.LoadScene("Level7");
        }
        else if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            SceneManager.LoadScene("Level8");
        }
        else if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            SceneManager.LoadScene("ShopScene");
        }
        else if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            SceneManager.LoadScene("EventScene");
        }
    }
}