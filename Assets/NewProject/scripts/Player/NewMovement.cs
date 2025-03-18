using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NewMovement : MonoBehaviour
{
    #region VARIABLES

    public Vector3 mousePosition;
    public Vector3 positionDesired;
    public Vector3 playerPosition;
    bool placeSelected;
    Vector3 pointingPosition;
    bool isMoving;
    float maxDistance;
    public float t; // Parameter to control position along the curve
    [SerializeField] public float velocity; // Speed in units per second
    [SerializeField] LineRenderer lineRenderer;
    public Vector3 controlPoint;
    [SerializeField] public int curveResolution = 20;  // Higher number for smoother curve in LineRenderer
    private float playerVelocity;
    private float bulletVelocity;
    // Reference to the PlayerBase script
    [SerializeField] private PlayerBase playerBase;

    private CombatManager combatManager;

    #endregion

    void Start()
    {
        placeSelected = false;
        playerPosition = transform.position;
        playerVelocity = velocity;
        // Get the PlayerBase component
        playerBase = GetComponent<PlayerBase>();
        bulletVelocity = GetComponent<NewMovement>().bulletVelocity;

        combatManager = FindAnyObjectByType<CombatManager>();
    }

    void Update()
    {
        InitUpdate();

        if (!placeSelected)
        {
            // If mouse button is pressed
            if (Input.GetMouseButtonDown(0))
            {
                MousePressed();
            }

            if (Input.GetMouseButton(0))
            {
                MouseDragged();
            }

            if (!isMoving)
            {
                UpdateLineRenderer();
            }
        }

        // When the mouse button is released
        if (Input.GetMouseButtonUp(0) && !isMoving)
        {
            positionDesired = pointingPosition;
            placeSelected = true;
            lineRenderer.enabled = false;
        }

        // Movement along the Bezier curve
        if (placeSelected)
        {
            ApplyMovement();
        }
    }

    private void InitUpdate()
    {
        if (combatManager != null && combatManager.allEnemiesDead)
        {
            return; // Do not allow any mouse interactions if victory condition is met
        }

        // Update mouse position
        mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10));
        mousePosition.z = 0;
        playerPosition = transform.position;
    }

    private void UpdateLineRenderer()
    {
        pointingPosition = mousePosition;
        float distanceToTarget = Vector3.Distance(playerPosition, pointingPosition);
        if (distanceToTarget > playerBase.GetRange())
        {
            Vector3 direction = (pointingPosition - playerPosition).normalized;
            pointingPosition = playerPosition + direction * playerBase.GetRange(); // Clamp to maximum range
        }
        else
        {
            pointingPosition = mousePosition;
        }

        controlPoint = (playerPosition + pointingPosition) + new Vector3(0, -1f, 0); // Set initial control point

        // Update the control point based on the initial position and the new target position
        float curveIntensity = Mathf.Clamp(Vector3.Distance(playerPosition, pointingPosition) / 100f, 0, 2f);
        controlPoint = pointingPosition + new Vector3(0, -curveIntensity, 0); // Adjust height

        // Generate smoother curve points for the LineRenderer
        List<Vector3> smoothCurvePoints = GenerateBezierCurve(playerPosition, controlPoint, pointingPosition, curveResolution);
        lineRenderer.positionCount = smoothCurvePoints.Count;
        lineRenderer.SetPositions(smoothCurvePoints.ToArray());
    }

    #region BEZIER
    private List<Vector3> GenerateBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, int resolution)
    {
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            points.Add(BezierCurve(t, p0, p1, p2));
        }
        return points;
    }

    // Calculate Bezier curve position
    private Vector3 BezierCurve(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0; // (1-t)^2 * P0
        p += 2 * u * t * p1; // 2 * (1-t) * t * P1
        p += tt * p2; // t^2 * P2
        return p;
    }

    #endregion


    #region MOUSE_ACTIONS

    private void MousePressed()
    {
        controlPoint = (playerPosition + pointingPosition) / 2 + new Vector3(0, -1f, 0); // Set initial control point
        t = 0; // Reset t parameter
    }

    private void MouseDragged()
    {
        // Update the end position while dragging, within range limit
        float range = playerBase.GetRange();
        float distanceToTarget = Vector3.Distance(playerPosition, mousePosition);
        if (distanceToTarget > range)
        {
            Vector3 direction = (mousePosition - playerPosition).normalized;
            mousePosition = playerPosition + direction * range; // Clamp to maximum range
        }
    }

    #endregion

    private void ApplyMovement()
    {
        playerBase.SetInAction(true);
        isMoving = true;

        // Calculate the increment for t based on velocity and curve length
        float distanceToTarget = Vector3.Distance(playerPosition, positionDesired);
        float tIncrement = (velocity * Time.deltaTime) / distanceToTarget;  // Fixed increment based on speed

        t = Mathf.Clamp01(t + tIncrement); // Increment t, clamping it between 0 and 1

        if (GetComponent<PlayerBase>().activeStyle != null && GetComponent<PlayerActionManager>().isShooting)
        {
            velocity = bulletVelocity;
            //Lerpeo al disparar
            Vector3 newPosition = BezierCurve(t, playerPosition, controlPoint, positionDesired);
            // GetComponent<PlayerActionManager>().isShooting = true;
            GetComponent<PlayerActionManager>().UpdateAction(newPosition, t); // Update the player's position
        }
        else if (GetComponent<PlayerActionManager>().isMoving)
        {
            velocity = playerVelocity;
            Vector3 newPosition = BezierCurve(t, playerPosition, controlPoint, positionDesired);
            //GetComponent<PlayerActionManager>().isShooting = false;
            GetComponent<PlayerActionManager>().UpdateAction(newPosition, t); // Update the player's position
        }


        // Check if we have reached the end of the curve
        if (t >= 1f)
        {
            playerBase.SetInAction(false); //Player no longer in action
            placeSelected = false;
            isMoving = false;
            playerPosition = transform.position; // Update player position to the new position
            velocity = playerVelocity;
            lineRenderer.enabled = true;  // Enable LineRenderer
        }
    }

    public bool GetIsMoving() { return isMoving; }
    public Vector3 GetPosition() { return playerPosition; }
    public void SetPositionDesired(Vector3 position) { positionDesired = position; }
    public Vector3 GetPositionDesired() { return positionDesired; }
}