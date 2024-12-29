using System.Collections.Generic;
using UnityEngine;

public class MovPlayer : MonoBehaviour
{
    public Vector3 playerPosition;
    public Vector3 mousePosition;
    public Vector3 positionDesired;

    public bool isMoving;
    public float t;
    [SerializeField] private float maxDistance = 10f; 
    [SerializeField] private float velocity = 5f;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int curveResolution = 0;
    
    public TimeSecuence timeSceuence;

    private float currentTime;


    private Vector3 controlPoint;


    void Start()
    {
        playerPosition = transform.position;
        lineRenderer.enabled = false;
        t = 0;
        currentTime = timeSceuence.totalTime;
    }

    void Update()
    {
        if (isMoving)
        {
            UpdateMovement();
        }
    }


    public void StartMov(Vector3 targetPosition, float rang)
    {
        playerPosition = transform.position;

        float distanceToTarget = Vector3.Distance(playerPosition, targetPosition);
        if(distanceToTarget > rang)
        {
            Vector3 direction = (targetPosition - playerPosition).normalized;
            targetPosition = playerPosition + direction * rang;
        }

        positionDesired = targetPosition;
        controlPoint = (playerPosition + positionDesired) / 2 + new Vector3(0, -1f, 0);
        float timeRequired = CalculateTiemConsum(Vector3.Distance(playerPosition, positionDesired));
        Debug.Log (timeRequired);
        if (currentTime >= timeRequired)
        {
            currentTime -= timeRequired;

            UpdateLineRenderer(playerPosition, controlPoint, positionDesired);
            lineRenderer.enabled = true;
            isMoving = true;
            t = 0;
        }

        else
        {
            Debug.Log("se te quedo larga XD");
        }
    }


    private float CalculateTiemConsum(float dist)
    {
        float maxDist = maxDistance;
        Debug.Log(dist);

        return Mathf.Clamp((dist / maxDist) * timeSceuence.totalTime, 0, timeSceuence.totalTime);
    }

    private void UpdateMovement()
    {
        float distanceToTarget = Vector3.Distance(playerPosition, positionDesired);
        float tIncrement = (velocity * Time.deltaTime) / distanceToTarget;
        t = Mathf.Clamp01(t + tIncrement);

        Vector3 newPosition = BezierCurve(t, playerPosition, controlPoint, positionDesired);
        transform.position = newPosition;

        if(t >= 1f)
        {
            StopMovment();
        }

        
    }

    private void StopMovment()
    {
        isMoving = false;
        t = 0;
        lineRenderer.enabled=false;
        playerPosition = transform.position;
        timeSceuence.actualTime = currentTime;
    }

    

    private void UpdateLineRenderer(Vector3 p0, Vector3 p1, Vector3 p2)
    {
        List<Vector3> smoothCurvePoints = GenerateBezierCurve(p0, p1, p2, curveResolution);
        lineRenderer.positionCount = smoothCurvePoints.Count;
        lineRenderer.SetPositions(smoothCurvePoints.ToArray());
    }




    private List<Vector3> GenerateBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, int resolution)
    {
        List<Vector3> points = new List<Vector3>();
        for(int i = 0; i <= resolution; i++)
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
}
