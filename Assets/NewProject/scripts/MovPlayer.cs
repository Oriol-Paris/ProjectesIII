using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MovPlayer : MonoBehaviour
{
    public Vector3 playerPosition;
    public Vector3 mousePosition;
    public Vector3 positionDesired;

  
    public float t;
    [SerializeField] private float maxDistance = 10f; 
    [SerializeField] private float velocity = 5f;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private int curveResolution = 0;
    
    public TimeSecuence timeSceuence;

    private float currentTime = 3;


    private Vector3 controlPoint;

    private List<Tuple<Vector3, Vector3, Vector3>> MovList = new List<Tuple<Vector3, Vector3, Vector3>>();

    private List<LineRenderer> lineList = new List<LineRenderer>();

   


    void Start()
    {
        playerPosition = transform.position;
       
        t = 0;
       
    }

    void Update()
    {
     
    }

    public void PreStartMov(Vector3 lastPosition,Vector3 targetPosition, float rang)
    {
        CanWalk(lastPosition, targetPosition, rang);
    }


    public void StartMov()
    {
      
    }


   

    private float CalculateTiemConsum(float dist)
    {
        float maxDist = maxDistance;
        Debug.Log(dist);

        return Mathf.Clamp((dist / maxDist) * timeSceuence.totalTime, 0, timeSceuence.totalTime);
    }

    public void UpdateMovement(int movCount)
    {
       
            var firstItem = MovList[movCount];
            Vector3 _playerPosition = firstItem.Item1;
            Vector3 _controlPoint = firstItem.Item2;
            Vector3 _positionDesired = firstItem.Item3;


            float distanceToTarget = Vector3.Distance(_playerPosition, _positionDesired);
            float tIncrement = (velocity * Time.deltaTime) / distanceToTarget;

            t = Mathf.Clamp01(t + tIncrement);

            Vector3 newPosition = BezierCurve(t, _playerPosition, _controlPoint, _positionDesired);
            transform.position = newPosition;

        
            Directorio.Apuntar(gameObject, transform.position, _positionDesired);





    }

    public void StopMovment()
    {
       
         t = 0f;
        LineRenderer.Destroy(lineList[0]);
        lineList.RemoveAt(0);

        timeSceuence.actualTime = currentTime;
    }

    

    private void UpdateLineRenderer(Vector3 p0, Vector3 p1, Vector3 p2)
    {
       
        LineRenderer newLineRenderer = Instantiate(lineRenderer, p0, Quaternion.identity);
        newLineRenderer.transform.SetParent(transform);

        lineList.Add(newLineRenderer);


        List<Vector3> smoothCurvePoints = GenerateBezierCurve(p0, p1, p2, curveResolution);

       
        newLineRenderer.positionCount = smoothCurvePoints.Count;
        newLineRenderer.SetPositions(smoothCurvePoints.ToArray());

      
        
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



    private void CanWalk(Vector3 lastPosition, Vector3 targetPosition, float rang)
    {
        playerPosition = lastPosition;
        currentTime = timeSceuence.actualTime;

        float distanceToTarget = Vector3.Distance(playerPosition, targetPosition);
        if (distanceToTarget > rang)
        {
            Vector3 direction = (targetPosition - playerPosition).normalized;
            targetPosition = playerPosition + direction * rang;
        }

        positionDesired = targetPosition;
        controlPoint = (playerPosition + positionDesired) / 2 + new Vector3(0, -1f, 0);
        float timeRequired = CalculateTiemConsum(Vector3.Distance(playerPosition, positionDesired));
        Debug.Log(timeRequired);
        if (currentTime >= timeRequired)
        {
            currentTime -= timeRequired;

            UpdateLineRenderer(playerPosition, controlPoint, positionDesired);
           
          
           
            MovList.Add(Tuple.Create(playerPosition, controlPoint, positionDesired));
           
            timeSceuence.actualTime = currentTime;
        }

        else
        {
            Debug.Log("se te quedo larga XD");
        }
    }
}
