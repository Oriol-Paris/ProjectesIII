using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerPathFollower : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float yOffset = 1.2f;

    private List<AnimateMapTile> pathTiles;
    private int currentIndex;
    
    private AnimateMapTile currentTile;
    private bool passedCurrentTile = false;

    void Start()
    {

        List<InteractableTiles>  levelTiles = new List<InteractableTiles>();
        levelTiles.AddRange(FindObjectsByType<InteractableTiles>(FindObjectsSortMode.None));
        
        foreach (InteractableTiles tile in levelTiles)
        {
            if (tile.levelName == PlayerPrefs.GetString("LastLevelCleared"))
            {
                if (tile.levelName == "Level6")
                {
                    transform.position = levelTiles[0].transform.position + new Vector3(0, yOffset, 0);
                    transform.SetParent(levelTiles[0].transform);
                    currentTile = levelTiles[0].GetComponent<AnimateMapTile>();
                }
                else
                {
                    transform.position = tile.transform.position + new Vector3(0, yOffset, 0);
                    transform.SetParent(tile.transform);
                    currentTile = tile.GetComponent<AnimateMapTile>();
                }
            }
        }
    }

    public void SetTilePath(List<AnimateMapTile> tiles, string sceneToLoad)
    {
        pathTiles = tiles;
        if (pathTiles == null || pathTiles.Count == 0) return;

        currentIndex = 0;

        if (pathTiles.Count > 1)
            StartCoroutine(FollowPath(sceneToLoad));
    }

    private IEnumerator FollowPath(string sceneToLoad = null)
    {
        yield return new WaitForSeconds(0.3f);

        for (currentIndex = 1; currentIndex < pathTiles.Count; currentIndex++)
        {
            AnimateMapTile nextTile = pathTiles[currentIndex];

            if(pathTiles[currentIndex] == currentTile)
            {
                passedCurrentTile = true;
            }

            if (passedCurrentTile && currentIndex != pathTiles.Count)
            {
                transform.SetParent(null);

                Vector3 start = transform.position;
                Vector3 target = nextTile.transform.position + new Vector3(0, yOffset, 0);
                float distance = Vector3.Distance(start, target);
                float elapsed = 0f;

                while (elapsed * moveSpeed < distance)
                {
                    transform.SetParent(nextTile.transform);
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01((elapsed * moveSpeed) / distance);
                    transform.position = Vector3.Lerp(start, target, t);
                    yield return null;
                }

                transform.position = target;
                transform.SetParent(nextTile.transform);
            }
            else
            {
                yield return new WaitForSeconds(0.15f);
            }
        }

        if (sceneToLoad != null)
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}