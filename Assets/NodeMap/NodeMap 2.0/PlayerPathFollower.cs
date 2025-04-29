using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerPathFollower : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float yOffset = 1.2f;

    private List<AnimateMapTile> pathTiles;
    private int currentIndex;

    public void SetTilePath(List<AnimateMapTile> tiles, string sceneToLoad)
    {
        pathTiles = tiles;
        if (pathTiles == null || pathTiles.Count == 0) return;

        currentIndex = 0;

        transform.position = pathTiles[0].transform.position + new Vector3(0, yOffset, 0);
        transform.SetParent(pathTiles[0].transform);

        if (pathTiles.Count > 1)
            StartCoroutine(FollowPath(sceneToLoad));
    }

    private IEnumerator FollowPath(string sceneToLoad = null)
    {
        yield return new WaitForSeconds(0.3f);

        for (currentIndex = 1; currentIndex < pathTiles.Count; currentIndex++)
        {
            AnimateMapTile nextTile = pathTiles[currentIndex];

            transform.SetParent(null);

            Vector3 start = transform.position;
            Vector3 target = nextTile.transform.position + new Vector3(0, yOffset, 0);
            float distance = Vector3.Distance(start, target);
            float elapsed = 0f;

            while (elapsed * moveSpeed < distance)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01((elapsed * moveSpeed) / distance);
                transform.position = Vector3.Lerp(start, target, t);
                yield return null;
            }

            transform.position = target;
            transform.SetParent(nextTile.transform);
        }

        if (sceneToLoad != null)
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}