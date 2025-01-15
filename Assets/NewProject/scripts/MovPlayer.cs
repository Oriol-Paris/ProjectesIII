using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class MovPlayer : MonoBehaviour
{
    public Vector3 mousePosition;
    public Vector3 positionDesired;
    public Vector3 playerPosition;



    public float t;
    [SerializeField] private float maxDistance = 20f; 
    [SerializeField] private float velocity = 5f;
    [SerializeField]  LineRenderer lineRenderer;
    [SerializeField] private int curveResolution = 0;
    
    public TimeSecuence timeSceuence;

    private float currentTime = 3;

    public bool placeSelected;
    public bool isMoving;
    public bool isDragging;


    private Vector3 controlPoint;

    public List<Tuple<Vector3, Vector3, Vector3>> MovList = new List<Tuple<Vector3, Vector3, Vector3>>();

    private List<LineRenderer> lineList = new List<LineRenderer>();

    [SerializeField] private List<Vector3> curvePoints = new List<Vector3>();




    void Start()
    {
        playerPosition = transform.position;
        //placeSelected = false;
        t = 0;
       
    }

    void Update()
    {
     
    }

    public void PreStartMov()
    {
        CanWalk();
    }


    public void StartMov()
    {
        placeSelected = true;
      


    }

    public void finish()
    {
        curvePoints.Clear();
        MovList.Clear();
        placeSelected = false;
        isDragging = false;
        foreach (var line in lineList)
        {
            Destroy(line.gameObject);
        }
        lineList.Clear();
        //DebugMovList();
    }


    private void DebugMovList()
    {
        Debug.Log($"MovList contiene {MovList.Count} elementos.");
        for (int i = 0; i < MovList.Count; i++)
        {
            Debug.Log($"Movimiento {i}: Inicio {MovList[i].Item1}, Control {MovList[i].Item2}, Fin {MovList[i].Item3}");
        }
    }




    private float CalculateTiemConsum(float dist)
    {
        float maxDist = maxDistance;
      

        return Mathf.Clamp((dist / maxDist) * timeSceuence.totalTime, 0, timeSceuence.totalTime);
    }

    public void UpdateMovement(int movCount)
    {
        

            var firstItem = MovList[movCount];
            Vector3 _playerPosition = firstItem.Item1;
            Vector3 _controlPoint = firstItem.Item2;
            Vector3 _positionDesired = firstItem.Item3;
        //Debug.Log(movCount);

            this.GetComponent<PlayerActionManager>().WalkingSound();

        t += velocity * Time.deltaTime;

            // Interpolate along the curve
            Vector3 newPosition = BezierCurve(t, _playerPosition, _controlPoint, _positionDesired);
            transform.position = newPosition;
        
        
    }

    public void StopMovment()
    {
        t = 0f;
        timeSceuence.actualTime = currentTime;
        
    }


    private float CalculateCurveLength()
    {
        if (curvePoints.Count < 3) return 0;

        float length = 0f;
        Vector3 previousPoint = curvePoints[0];

        int resolution = curveResolution > 0 ? curveResolution : 50; 
        for (int i = 1; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            Vector3 currentPoint = BezierCurve(t, curvePoints[0], curvePoints[1], curvePoints[2]);
            length += Vector3.Distance(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }

        return length;
    }


    private float CalculateStaminaConsumption(float distance)
    {
        // Ajustar este c�lculo seg�n la l�gica de tu juego
        float consumptionPerUnit = timeSceuence.totalTime / maxDistance;
        return Mathf.Clamp(distance * consumptionPerUnit, 0, timeSceuence.totalTime);
    }


    private void UpdateCurve()
    {
        if (curvePoints.Count < 2) return;

        // Get direction from start to end point (player to mouse)
        Vector3 startToMouse = mousePosition - curvePoints[0];
        Vector3 direction = startToMouse.normalized;

        // Adjust the midpoint based on mouse movement to create curvature
        float curveIntensity = Vector3.Distance(curvePoints[0], mousePosition) * 0.5f;

        // The middle point now moves with the mouse, controlled by a factor of curve intensity
        Vector3 midPoint = curvePoints[0] + direction * curveIntensity;

        // Set the curve to create a "snake-like" bend
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

  

    private void UpdateLineRendererr()
    {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = curvePoints.Count;
        lineRenderer.SetPositions(curvePoints.ToArray());
    }


    public void SetPositionDesired(Vector3 position) { positionDesired = position; }
    public Vector3 GetPositionDesired() { return positionDesired; }

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



    private void CanWalk()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            mousePosition = hit.point;
        }
        // playerPosition = lastPosition;
        currentTime = timeSceuence.actualTime;
       
      
       
        if (!Input.GetMouseButton(0) && !isMoving && !placeSelected && !isDragging)
        {
            // Define initial straight line trajectory
            isDragging = false;
            placeSelected = false;
            isDragging = false;
            positionDesired = mousePosition;

            curvePoints.Clear();
            curvePoints.Add(playerPosition);
            curvePoints.Add(positionDesired);
           
            UpdateLineRendererr();
        }

        if (Input.GetMouseButton(0) && !isMoving)
        {
         
           isDragging =true;
            UpdateCurve();
            UpdateLineRendererr();
        }

        if (Input.GetMouseButtonUp(0) && !isMoving)
        {
            float curveLength = CalculateCurveLength();
           

            float timeConsumption = CalculateTiemConsum(curveLength);
          

            if (currentTime > timeConsumption)
            {
                MovList.Add(Tuple.Create(curvePoints[0], curvePoints[1], curvePoints[2]));
                timeSceuence.AddAction(PlayerBase.ActionEnum.MOVE, positionDesired,null);
                Debug.Log($"Movimiento registrado. Consumo de estamina: {timeConsumption}");

                // Deducir la estamina
                currentTime -= timeConsumption;

                timeSceuence.actualTime = currentTime;
                playerPosition = positionDesired;
               
                t = 0f;

                InstantiateLineMovment();
            }
            else
            {
                Debug.Log("No tienes suficiente estamina para realizar este trayecto.");
               
            }

            isDragging = false;

        }


        

    }

    private void InstantiateLineMovment()
    {
        var newLine = Instantiate(lineRenderer);
        lineRenderer.enabled = true;
        newLine.positionCount = curvePoints.Count;
        newLine.SetPositions(curvePoints.ToArray());
        lineList.Add(newLine);
    }
}
