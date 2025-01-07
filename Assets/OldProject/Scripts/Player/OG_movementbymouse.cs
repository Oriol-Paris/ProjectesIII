using System.Collections.Generic;
using UnityEngine;

public class OG_MovementByMouse : MonoBehaviour
{
    public Vector3 mousePosition;
    public Vector3 positionDesired;
    public Vector3 playerPosition;
    public bool placeSelected;
    public bool isMoving;
    public bool isDragging;
    public bool isResting;
    public float t; // Parameter to control position along the curve
    [SerializeField] public float velocity; // Speed in units per second
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] public int curveResolution = 20; // Resolution for curve smoothness
    [SerializeField] private PlayerBase playerBase;

    private CombatManager combatManager;
    private PlayerActionManager playerActionManager;
    private List<Vector3> curvePoints; // Points defining the curve
    private List<BulletPrefab> bullets = new List<BulletPrefab>();

    [SerializeField] public float movementTimeLimit = 5f;
    public float timer;

    void Start()
    {
        curvePoints = new List<Vector3>();
        placeSelected = false;
        playerPosition = transform.position;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            mousePosition = hit.point;
        }

        if (Input.GetMouseButtonDown(0) && !isMoving && !placeSelected)
        {
            isDragging = false;
            positionDesired = mousePosition;

            curvePoints.Clear();
            curvePoints.Add(this.transform.position);
            curvePoints.Add(positionDesired);

            UpdateLineRenderer();
        }

        if (Input.GetMouseButton(0) && !isMoving)
        {
            isDragging = true;
            UpdateCurve();
            UpdateLineRenderer();
        }

        if (Input.GetMouseButtonUp(0) && !isMoving)
        {
            placeSelected = true;
            isDragging = false;
        }
    }

    private void UpdateCurve()
    {
        if (curvePoints.Count < 2) return;

        Vector3 startToMouse = mousePosition - curvePoints[0];
        Vector3 direction = startToMouse.normalized;

        float curveIntensity = Vector3.Distance(curvePoints[0], mousePosition) * 0.5f;
        Vector3 midPoint = curvePoints[0] + direction * curveIntensity;

        if (curvePoints.Count == 3)
        {
            curvePoints[1] = midPoint;
        }
        else if (curvePoints.Count > 3)
        {
            curvePoints[curvePoints.Count - 2] = midPoint;
        }
        else
        {
            curvePoints.Insert(1, midPoint);
        }
    }

    private void UpdateLineRenderer()
    {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = curvePoints.Count;
        lineRenderer.SetPositions(curvePoints.ToArray());
    }

    public void CanMove()
    {
        if (!placeSelected || isMoving) return;

        isMoving = true;
        t = 0f;
    }

    public void MoveAlongCurve()
    {
        if (!isMoving) return;

        if (t >= 1f)
        {
            placeSelected = false;
            isMoving = false;
            playerPosition = transform.position;
            return;
        }

        t += velocity * Time.deltaTime;

        Vector3 newPosition = CalculateQuadraticBezierPoint(t, curvePoints[0], curvePoints[1], curvePoints[2]);
        transform.position = newPosition;
    }

    public Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 point = uu * p0;
        point += 2 * u * t * p1;
        point += tt * p2;

        return point;
    }

    public void ResetMovementState()
    {
        placeSelected = false;
        isMoving = false;
        t = 0f;
        curvePoints.Clear();
    }


    public bool GetIsMoving() { return isMoving; }
    public Vector3 GetPosition() { return playerPosition; }

    public void SetPositionDesired(Vector3 position) { positionDesired = position; }
    public Vector3 GetPositionDesired() { return positionDesired; }

    public List<Vector3> GetCurvePoints()
    {
        return new List<Vector3>(curvePoints); // Retorna una copia de la curva
    }

    public bool IsReadyToMove()
    {
        return placeSelected && !isMoving; // Solo si la posición está seleccionada pero aún no se mueve
    }


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
                t = 1f;
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
