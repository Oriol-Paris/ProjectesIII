using System.Collections.Generic;
using UnityEngine;

public class BulletDirectionVisualizer : MonoBehaviour
{
    [SerializeField] private OG_MovementByMouse movementScript;
    private bool isShowingDirections = false;

    void Start()
    {
        if (movementScript == null)
        {
            movementScript = FindObjectOfType<OG_MovementByMouse>();
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ShowBulletDirections();
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            HideBulletDirections();
        }
    }

    private void ShowBulletDirections()
    {
        if (isShowingDirections) return;

        isShowingDirections = true;
        List<Bullet> pausedBullets = movementScript.GetPausedBullets();
        foreach (var bullet in pausedBullets)
        {
            if (bullet != null)
            {
                // Create a line renderer to show the direction
                LineRenderer lineRenderer = bullet.gameObject.AddComponent<LineRenderer>();
                lineRenderer.startWidth = 0.1f;
                lineRenderer.endWidth = 0.1f;
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, bullet.transform.position);
                lineRenderer.SetPosition(1, bullet.transform.position + bullet.GetOriginalDirection().normalized * 2f);
                lineRenderer.material = new Material(Shader.Find("Sprites/Default")) { color = Color.red };

                // Set the sorting layer and order in layer
                lineRenderer.sortingLayerName = "Default"; // Set the sorting layer name if needed
                lineRenderer.sortingOrder = 5; // Set the order in layer to 5
            }
        }
    }

    private void HideBulletDirections()
    {
        if (!isShowingDirections) return;

        isShowingDirections = false;
        List<Bullet> pausedBullets = movementScript.GetPausedBullets();
        foreach (var bullet in pausedBullets)
        {
            if (bullet != null)
            {
                // Remove the line renderer
                LineRenderer lineRenderer = bullet.GetComponent<LineRenderer>();
                if (lineRenderer != null)
                {
                    Destroy(lineRenderer);
                }
            }
        }
    }
}