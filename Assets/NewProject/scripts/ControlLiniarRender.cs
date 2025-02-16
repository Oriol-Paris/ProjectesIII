
using System;
using System.Collections.Generic;
using UnityEngine;

public class ControlLiniarRender : MonoBehaviour
{
    public Vector3 mousePosition;
    public Vector3 positionDesired;
    public Vector3 playerPosition;

    [SerializeField] private int curveResolution = 0;

    public LineRenderer lineRenderer;
    public List<LineRenderer> lineList = new List<LineRenderer>();


    [SerializeField] private List<Vector3> curvePoints = new List<Vector3>();

    void Start()
    {
     
        playerPosition = transform.position;
        
    }

    public void NextMov(bool isMoov) { 
        Debug.Log(positionDesired);
        if (isMoov)
        {
            playerPosition = positionDesired;
        }


        else
        {
            playerPosition = playerPosition;
        }
    }

    public void ControlLiniarRenderer()
    {
        positionDesired = Directorio.mousePosition();

        curvePoints.Clear();
        curvePoints.Add(playerPosition);
        curvePoints.Add(positionDesired);
    }

    public void InstantiateLineMovment()
    {
       
        var newLine = Instantiate(lineRenderer);
        lineRenderer.enabled = true;
        newLine.positionCount = curvePoints.Count;
        newLine.SetPositions(curvePoints.ToArray());
        lineList.Add(newLine);
       
    }

    public void UpdateLineRendererr()
    {
 
        lineRenderer.enabled = true;
        lineRenderer.positionCount = curvePoints.Count;
        lineRenderer.SetPositions(curvePoints.ToArray());
      
    }

    public void UpdateCurve()
    {

        mousePosition = Directorio.mousePosition();
       
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

  
    public float CalculateCurveLength()
    {
        if (curvePoints.Count < 3) return 0;

        float length = 0f;
        Vector3 previousPoint = curvePoints[0];

        int resolution = curveResolution > 0 ? curveResolution : 50;
        for (int i = 1; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            Vector3 currentPoint = Directorio.BezierCurve(t, curvePoints[0], curvePoints[1], curvePoints[2]);
            length += Vector3.Distance(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }

        return length;
    }


    public List<Vector3> getCurvePoint() { return curvePoints; }


    public void ResetControlLiniarRenderer()
    {
        curvePoints.Clear();
        foreach (var line in lineList)
        {
            Destroy(line.gameObject);
        }
        lineList.Clear();
    }
}
