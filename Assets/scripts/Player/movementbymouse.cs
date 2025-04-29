using System.Collections.Generic;
using UnityEngine;

public class MovementByMouse : MonoBehaviour
{
    Vector3 mousePosition;
    Vector3 positionDesired;
    Vector3 playerPosition;
    bool placeSelected;
    bool isMoving;
    bool isDragging;
    float moveTime;
    [SerializeField] private float velocity;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] private int curveResolution = 20;

    // Reference to the PlayerBase script
    [SerializeField] private PlayerBase playerBase;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        placeSelected = false;
        playerPosition = transform.position;
        lineRenderer.enabled = false;  // Start with LineRenderer disabled
        playerBase = GetComponent<PlayerBase>();
    }

    void Update()
    {
        mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10));
        mousePosition.z = 0;

        // Get the circular range from PlayerBase
        float maxDistance = playerBase.GetRange();

        // Handle mouse click to set destination
        if (Input.GetMouseButtonDown(0) && !placeSelected)
        {
            positionDesired = mousePosition;

            // Clamp the positionDesired within the circular range
            float distance = Vector3.Distance(playerPosition, positionDesired);
            if (distance > maxDistance)
            {
                Vector3 direction = (positionDesired - playerPosition).normalized;
                positionDesired = playerPosition + direction * maxDistance;
            }

            moveTime = 0;
            lineRenderer.enabled = true; // Enable the LineRenderer when selecting a path
            isDragging = false; // Reset dragging status

            // If not dragging, move directly to the desired position
            if (distance <= maxDistance)
            {
                placeSelected = true;
                isMoving = true;
                moveTime = 0; // Reset move time for the straight movement
            }
        }

        // Handle dragging to create a bending path
        if (Input.GetMouseButton(0) && !placeSelected)
        {
            // Calculate the distance from the player to the mouse position
            float distance = Vector3.Distance(playerPosition, mousePosition);
            if (distance > maxDistance)
            {
                // Keep the endpoint within the circular range
                Vector3 direction = (mousePosition - playerPosition).normalized;
                positionDesired = playerPosition + direction * maxDistance;
            }
            else
            {
                positionDesired = mousePosition;
            }

            isDragging = true; // Set dragging status to true

            // Generate the curve only when dragging
            List<Vector3> arcPoints = GenerateQuarterArc(playerPosition, positionDesired, curveResolution);
            lineRenderer.positionCount = arcPoints.Count;
            lineRenderer.SetPositions(arcPoints.ToArray());
        }
        else if (!isMoving)
        {
            // When not dragging, show a straight line within the circular range
            UpdateLineRenderer(mousePosition, maxDistance);
        }

        // When the mouse button is released
        if (Input.GetMouseButtonUp(0) && !isMoving)
        {
            if (isDragging)
            {
                placeSelected = true;
                moveTime = 0;
            }
            else
            {
                // Reset move time for straight movement when clicked
                placeSelected = true;
                isMoving = true;
                moveTime = 0;
            }

            lineRenderer.enabled = false;  // Disable line renderer after placement
        }

        // Movement along the arc or straight path
        if (placeSelected)
        {
            if (isDragging)
            {
                // Move along the curved path
                transform.position = FollowPath(moveTime, playerPosition, positionDesired);
            }
            else
            {
                // Move directly to the desired position in a straight line
                transform.position = Vector3.Lerp(playerPosition, positionDesired, moveTime);
            }

            moveTime += Time.deltaTime * velocity;

            if (moveTime >= 1f)
            {
                placeSelected = false;
                isMoving = false;
                playerPosition = transform.position; // Update player position for next movement
            }
        }
    }

    private void UpdateLineRenderer(Vector3 targetPosition, float maxDistance)
    {
        // Clamp the target position within the circular range
        float distance = Vector3.Distance(playerPosition, targetPosition);
        if (distance > maxDistance)
        {
            Vector3 direction = (targetPosition - playerPosition).normalized;
            targetPosition = playerPosition + direction * maxDistance;
        }

        // Show a straight line preview
        List<Vector3> straightLine = new List<Vector3> { playerPosition, targetPosition };
        lineRenderer.positionCount = straightLine.Count;
        lineRenderer.SetPositions(straightLine.ToArray());
    }

    // Generate a quarter arc curve only when dragging
    private List<Vector3> GenerateQuarterArc(Vector3 start, Vector3 end, int resolution)
    {
        List<Vector3> points = new List<Vector3>();
        Vector3 control = (start + end) / 2 + new Vector3(0, 1f, 0); // Add upward curve

        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            points.Add(QuadraticBezier(start, control, end, t));
        }
        return points;
    }

    private Vector3 FollowPath(float t, Vector3 start, Vector3 end)
    {
        Vector3 control = (start + end) / 2 + new Vector3(0, 1f, 0); // Recalculate control point for path following
        return QuadraticBezier(start, control, end, t);
    }

    private Vector3 QuadraticBezier(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1 - t;
        return u * u * p0 + 2 * u * t * p1 + t * t * p2;
    }

    public bool GetIsMoving() { return isMoving; }
    public Vector3 GetPosition() { return playerPosition; }
}
