using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class MapCameraController : MonoBehaviour
{
    [Header("Velocidades")]
    public float dragSpeed = 10f;
    public float zoomSpeed = 10f;

    [Header("Posiciones Limite")]
    public float minX = -10f;
    public float maxX = 10f;
    public float minZ = -15f;
    public float maxZ = 25f;
    public float minY = 10f;
    public float maxY = 32f;

    private Vector3 dragOrigin;
    private bool isDragging = false;

    private List<InteractableTiles> levelTiles;

    private void Awake()
    {
        levelTiles = new List<InteractableTiles>();
        levelTiles.AddRange(FindObjectsByType<InteractableTiles>(FindObjectsSortMode.None));

        foreach (InteractableTiles tile in levelTiles)
        {
            if(tile.levelName == PlayerPrefs.GetString("LastLevelCleared"))
            {
                transform.position = new Vector3(tile.transform.position.x, transform.position.y, tile.transform.position.z - 15.0f);
            }
        }
    }

    void Update()
    {
        HandleDrag();
        HandleZoom();
        ClampPosition();
    }

    void HandleDrag()
    {
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 currentMousePos = Input.mousePosition;
            Vector3 diff = Camera.main.ScreenToViewportPoint(dragOrigin - currentMousePos);

            Vector3 move = new Vector3(diff.x * dragSpeed, 0, diff.y * dragSpeed);
            transform.Translate(move, Space.World);

            dragOrigin = currentMousePos;
        }
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > 0.01f && !(scroll > 0 && transform.position.y == minY))
        {
            Vector3 zoomDirection = new Vector3(0, -1f, 1f).normalized;
            transform.position += zoomDirection * scroll * zoomSpeed;
        }
    }

    void ClampPosition()
    {
        Vector3 clamped = transform.position;
        clamped.x = Mathf.Clamp(clamped.x, minX, maxX);
        clamped.y = Mathf.Clamp(clamped.y, minY, maxY);
        clamped.z = Mathf.Clamp(clamped.z, minZ, maxZ);
        transform.position = clamped;
    }
}