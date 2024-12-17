using UnityEngine;

public class SnakeFollowMouseWithLineRenderer : MonoBehaviour
{
    public Transform head; // Reference to the head of the snake
    public LineRenderer lineRenderer; // LineRenderer component for the snake
    public int segmentCount = 20; // Number of segments in the snake
    public float segmentDistance = 0.5f; // Distance between each segment
    public float maxBendFactor = 1.0f; // Maximum bending effect for the closest segment
    public float minBendFactor = 0.1f; // Minimum bending effect for the furthest segment
    public Transform tail; // Reference to the tail of the snake (fixed in position)

    private Vector3[] segmentPositions; // Array to hold the positions of each segment

    private void Start()
    {
        // Initialize segment positions array based on segment count
        segmentPositions = new Vector3[segmentCount];

        // Set initial positions for each segment along a straight line
        Vector3 startPosition = head.position;
        Vector3 directionToTail = (tail.position - head.position).normalized;

        for (int i = 0; i < segmentCount; i++)
        {
            segmentPositions[i] = startPosition + directionToTail * segmentDistance * i;
        }

        // Set LineRenderer positions and segment count
        lineRenderer.positionCount = segmentCount;
        lineRenderer.SetPositions(segmentPositions);
    }

    private void Update()
    {
        // Get the mouse position in world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Set z to zero for 2D

        // Update the head position to follow the mouse
        head.position = Vector3.Lerp(head.position, mousePosition, Time.deltaTime * 10f);

        // Update segment positions for bending effect
        segmentPositions[0] = head.position; // The first segment follows the head

        for (int i = 1; i < segmentCount; i++)
        {
            // Calculate the target psction based on the previous segment
            Vector3 previousPosition = segmentPositions[i - 1];
            Vector3 directionToPrevious = (previousPosition - segmentPositions[i]).normalized;

            Vector3 targetPosition = previousPosition - directionToPrevious * segmentDistance;

            // Apply bending effect based on distance from the head
            float normalizedIndex = (float)i / segmentCount;
            float bend = Mathf.Lerp(maxBendFactor, minBendFactor, normalizedIndex);

            // Move each segment towards its target position
            segmentPositions[i] = Vector3.Lerp(segmentPositions[i], targetPosition, Time.deltaTime * bend);
        }

        // Ensure the tail segment remains fixed in place
        segmentPositions[segmentCount - 1] = tail.position;

        // Update the LineRenderer with new segment positions
        lineRenderer.SetPositions(segmentPositions);
    }
}
