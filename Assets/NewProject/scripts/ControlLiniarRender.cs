
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

    public void Disable(bool disable)
    {
        if(disable)
        {
            lineRenderer.enabled = false;
        }
        else
        {
            lineRenderer.enabled = true;
        }
    }

    public void NextMov(bool isMoov) { 
        //Debug.Log(positionDesired);
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
        RaycastHit hit;

        Vector3 mouseposition = Directorio.mousePosition();
        curvePoints.Clear();
        curvePoints.Add(playerPosition);

        Vector3 direction = (mouseposition - playerPosition).normalized;
        float distance = Vector3.Distance(playerPosition, mouseposition);

        if (Physics.Raycast(playerPosition, direction, out hit, distance))
        {

            if (hit.collider.CompareTag("envairoment"))
            {
                float margin = 0.2f;
                 positionDesired = hit.point - (direction * margin);
                curvePoints.Add(positionDesired);
            }
            else
            {
                positionDesired = mouseposition;
                curvePoints.Add(positionDesired);
            }
        }
        else
        {
            positionDesired = mouseposition;
            curvePoints.Add(positionDesired);
        }
       
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

        Vector3 startPos = curvePoints[0];
        Vector3 endPos = curvePoints[curvePoints.Count - 1];

       
        Vector3 centerPoint = (startPos + endPos) * 0.5f;

       
        Vector3 mainDirection = (endPos - startPos).normalized;

       
        Vector3 perpendicular = Vector3.Cross(mainDirection, Vector3.up).normalized;

        
        float curveIntensity = Vector3.Distance(startPos, mousePosition) * 0.5f;
        float lateralOffset = Vector3.Dot(mousePosition - centerPoint, perpendicular);

       
        Vector3 midPoint = centerPoint + perpendicular * lateralOffset * 0.5f;



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
        playerPosition = transform.position;

    }
}
