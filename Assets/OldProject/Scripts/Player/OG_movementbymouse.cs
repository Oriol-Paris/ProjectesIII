using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OG_MovementByMouse : MonoBehaviour
{
    public Vector3 mousePosition;
    public Vector3 positionDesired;
    public Vector3 playerPosition;
    public bool placeSelected;
    public bool isMoving;
    public bool isResting;
    public float t; // Parameter to control position along the curve
    [SerializeField] public float velocity; // Speed in units per second
    [SerializeField] LineRenderer lineRenderer;
    public Vector3 controlPoint;
    [SerializeField] public int curveResolution = 20; // Higher number for smoother curve in LineRenderer
    private float playerVelocity;
    private float bulletVelocity;
    [SerializeField] private PlayerBase playerBase;

    private CombatManager combatManager;
    private PlayerActionManager playerActionManager;

    private List<Vector3> curvePointsWorldRef;
    private List<Vector3> curvePointsPlayerRef;

    // Timer variables
    [SerializeField] public float movementTimeLimit = 5f; // Adjustable time limit
    public float timer;

    private List<BulletPrefab> bullets = new List<BulletPrefab>();

    void Start()
    {
        curvePointsPlayerRef = new List<Vector3>();

        this.enabled = false;
        placeSelected = false;
        playerPosition = transform.position;

        playerVelocity = velocity;
        playerBase = GetComponent<PlayerBase>();
        bulletVelocity = playerVelocity;

        combatManager = FindAnyObjectByType<CombatManager>();
        playerActionManager = GetComponent<PlayerActionManager>();

        timer = movementTimeLimit; // Initialize timer
        this.enabled = true;
    }

    void Update()
    {
        if (combatManager != null && combatManager.allEnemiesDead)
        {
            return; // Do not allow any mouse interactions if victory condition is met
        }

        mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10));
        mousePosition.z = 0;

        

        if (Input.GetMouseButtonDown(0) && !placeSelected)
        {
            positionDesired = mousePosition;
            playerPosition = transform.position;

            float range = playerBase.GetRange();
            float distanceToTarget = Vector3.Distance(playerPosition, positionDesired);
            if (distanceToTarget > range)
            {
                Vector3 direction = (positionDesired - playerPosition).normalized;
                positionDesired = playerPosition + direction * range;
            }

            t = 0;
            controlPoint = (playerPosition + positionDesired) / 2 + new Vector3(0, -1f, 0);
            
            lineRenderer.enabled = true;
        }

        if (Input.GetMouseButton(0) && !placeSelected)
        {
            float range = playerBase.GetRange();
            float distanceToTarget = Vector3.Distance(playerPosition, mousePosition);
            if (distanceToTarget > range)
            {
                Vector3 direction = (mousePosition - playerPosition).normalized;
                mousePosition = playerPosition + direction * range;
            }

            
            Vector3 controlPoint1 = (playerPosition + mousePosition) / 2 + new Vector3(0, -1f, 0);
            Vector3 controlPoint2 = (playerPosition + mousePosition) / 4 + new Vector3(0, -1f, 0);
            Vector3 controlPoint3 = ((playerPosition + mousePosition) / 4) * 3 + new Vector3(0, -1f, 0);
            Vector3 controlPoint4 = (playerPosition + mousePosition) + new Vector3(0, -1f, 0);
            if (!curvePointsPlayerRef.Contains(controlPoint1))
            {
                curvePointsPlayerRef.Add(controlPoint1);
                curvePointsPlayerRef.Add(controlPoint2);
                curvePointsPlayerRef.Add(controlPoint3);
                curvePointsPlayerRef.Add(controlPoint4);
            }
            else
            {
                curvePointsPlayerRef[0] = controlPoint1;
                curvePointsPlayerRef[1] = controlPoint2;
                curvePointsPlayerRef[2] = controlPoint3;
                curvePointsPlayerRef[3] = controlPoint4;
                
            }
            Debug.Log(curvePointsPlayerRef[0].x);
            UpdateLineRenderer(mousePosition);
        }

        if (Input.GetMouseButtonUp(0) && !isMoving)
        {
            positionDesired = mousePosition;
            float range = playerBase.GetRange();
            float distanceToTarget = Vector3.Distance(playerPosition, positionDesired);
            if (distanceToTarget > range)
            {
                Vector3 direction = (positionDesired - playerPosition).normalized;
                positionDesired = playerPosition + direction * range;
                placeSelected = true;
                lineRenderer.enabled = false;
            }
            else
            {
                positionDesired = mousePosition;
                placeSelected = true;
                lineRenderer.enabled = false;
            }
        }

        if (placeSelected)
        {
            playerBase.SetInAction(true);
            isMoving = true;
            lineRenderer.enabled = false;

            Vector3 destination = positionDesired;
            RaycastHit hit;

            if (destination.x < playerPosition.x)
                playerBase.GetComponent<SpriteRenderer>().flipX = true;
            else
                playerBase.GetComponent<SpriteRenderer>().flipX = false;

            if (Physics.Raycast(playerPosition, (destination - playerPosition).normalized, out hit, Vector3.Distance(playerPosition, destination)))
            {
                destination = hit.point;
            }

            float distanceToTarget = Vector3.Distance(playerPosition, destination);
            float tIncrement = (velocity * Time.deltaTime) / distanceToTarget;

            t = Mathf.Clamp01(t + tIncrement);

            Vector3 newPosition = Vector3.MoveTowards(transform.position, BezierCurve(t, playerPosition, controlPoint, destination), velocity);
            //for (; )
            playerActionManager.UpdateAction(newPosition, t);

            if (t >= 1f)
            {
                StopMovement();
            }
            else
            {
                timer -= Time.deltaTime; // Decrease timer
                if (timer <= 0f)
                {
                    StopMovement();
                }
            }
        }
        else if (!isMoving && !placeSelected)
        {
            // Resume all bullets when player starts moving again
            foreach (var bullet in bullets)
            {
                if(bullet != null)
                bullet.Resume();
            }
        }

        // Check for rest action key press (W)
        if (Input.GetKeyUp(KeyCode.W) && !placeSelected)
        {
            placeSelected = true;
            t = 0;
            isResting = true;
            isMoving = true;
            timer = movementTimeLimit+.5f; // Reset timer
            placeSelected = true;
        }

        // Handle the rest action countdown
        if (isResting && isMoving && placeSelected && timer > 0f)
        {
            t = 0;
            timer -= Time.deltaTime; // Decrease timer
            if (timer < 0f)
            {
                ExecuteRestAction();
                StopMovement();
            }
        }
    }

    private void StopMovement()
    {
        isResting = false;
        playerBase.SetInAction(false);
        placeSelected = false;
        isMoving = false;
        playerPosition = transform.position;
        t = 1;
        timer = movementTimeLimit; // Reset timer
        playerActionManager.ResetFlags();

        // Pause all bullets
        foreach (var bullet in bullets)
        {
            if(bullet != null)
            bullet.Pause();
        }
    }

    private void UpdateLineRenderer(Vector3 targetPosition)
    {
        float curveIntensity = Mathf.Clamp(Vector3.Distance(playerPosition, targetPosition) / 100f, 0, 2f);
        controlPoint = positionDesired + new Vector3(0, -curveIntensity, 0);

        List<Vector3> smoothCurvePoints = GenerateBezierCurve(playerPosition, controlPoint, targetPosition, curveResolution);
        lineRenderer.positionCount = smoothCurvePoints.Count;
        lineRenderer.SetPositions(smoothCurvePoints.ToArray());
    }

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

    private Vector3 BezierCurve(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
        return p;
    }

    public bool GetIsMoving() { return isMoving; }
    public Vector3 GetPosition() { return playerPosition; }

    public void SetPositionDesired(Vector3 position) { positionDesired = position; }
    public Vector3 GetPositionDesired() { return positionDesired; }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Walls"))
        {
            positionDesired = transform.position;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Walls"))
        {
            if (isMoving)
            {
                isMoving = false;
                positionDesired = transform.position;
                t = 1;
            }
        }
    }

    public void RegisterBullet(BulletPrefab bullet)
    {
        bullets.Add(bullet);
    }

    public void UnregisterBullet(BulletPrefab bullet)
    {
        bullets.Remove(bullet);
    }

    public List<BulletPrefab> GetPausedBullets()
    {
        return bullets;
    }

    public void ExecuteRestAction()
    {
        playerBase.Rest();
        playerActionManager.EndTurn(); // End the turn after resting

        // Trigger enemies' turn
        foreach (EnemyMovementShooter enemy in GameObject.FindObjectsOfType<EnemyMovementShooter>())
        {
            enemy.DecideAction();
        }
    }
}
