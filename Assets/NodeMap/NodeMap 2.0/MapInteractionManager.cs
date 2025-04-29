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

   [SerializeField] private GameObject player;
    public List<AnimateMapTile> interactableTiles;

    public List<TileGroup> tileGroups;

    private void Start()
    {
        if(PlayerPrefs.GetFloat("DifficultyMultiplier") == 0)
        {
            PlayerPrefs.SetFloat("DifficultyMultiplier", 1);
        }

        switch (PlayerPrefs.GetString("LastLevelCleared"))
        {
            case "Level1":
                AnimateTileGroup("Level1", false);
                break;
            case "Level2":
                AnimateTileGroup("Level2", false);
                break;
            case "Level3":
                AnimateTileGroup("Level3", false);
                break;
            case "ShopScene":
                AnimateTileGroup("ShopScene", false);
                break;
            case "Level4":
                AnimateTileGroup("Level4", false);
                break;
            case "EventScene":
                AnimateTileGroup("EventScene", false);
                break;
        }
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
            player.transform.position = new Vector3(tile.transform.position.x,tile.transform.position.y+3,tile.transform.position.z-2.15f);
            tile.AnimateTile();
            yield return new WaitForSeconds(0.1f);
        }
    }
}