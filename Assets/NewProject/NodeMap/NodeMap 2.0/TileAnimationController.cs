using UnityEngine;

public class TileAnimationController : MonoBehaviour
{
    AnimateMapTile[] tiles;

    private void Start()
    {
        tiles = FindObjectsByType<AnimateMapTile>(FindObjectsSortMode.None);
    }

    void Update()
    {
        // Por ejemplo, para testearlo, animamos todos
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (var tile in tiles)
                tile.AnimateTile();
        }
    }
}
