using System;
using System.Collections.Generic;

using UnityEngine;

public class MovPlayer : MonoBehaviour
{
    public Vector3 mousePosition;
    public Vector3 positionDesired;
    public Vector3 playerPosition;

    public ControlLiniarRender controlLiniarRender;
    public ControlListMovment controlListMovment;


    public float t = 0;
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


    void Start(){}

    void Update(){}

    public void PreStartMov() { CanWalk(); }


    public void StartMov() { placeSelected = true; }

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
        playerPosition = transform.position;
        currentTime = timeSceuence.totalTime;
        timeSceuence.actualTime = currentTime;
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
      

            this.GetComponent<PlayerActionManager>().WalkingSound();

             t += velocity * Time.deltaTime;

            // Interpolate along the curve
            Vector3 newPosition = Directorio.BezierCurve(t, _playerPosition, _controlPoint, _positionDesired);
            transform.position = newPosition;
        
        
    }

    public void StopMovment()
    {
        t = 0f;
        timeSceuence.actualTime = currentTime;
        
    }

   

    public void SetPositionDesired(Vector3 position) { positionDesired = position; }
    public Vector3 GetPositionDesired() { return positionDesired; }


    private void CanWalk()
    {

        currentTime = timeSceuence.actualTime;
           

        if (!Input.GetMouseButton(0) && !isMoving && !placeSelected && !isDragging)
        {
           
            isDragging = false;  
            placeSelected = false;
            controlLiniarRender.ControlLiniarRenderer();
            controlLiniarRender.UpdateLineRendererr();
        }

        if (Input.GetMouseButton(0) && !isMoving)
        {
         
           isDragging =true;
            controlLiniarRender.UpdateCurve();
            controlLiniarRender.UpdateLineRendererr();
        }

        if (Input.GetMouseButtonUp(0) && !isMoving)
        {

            controlListMovment.AddMovement(controlLiniarRender);
          
            isDragging = false;

        }


        

    }

  
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Walls")
        {
            StopMovment();
            StopAllCoroutines();
        }
    }

    public float GetCurrentTime() { return currentTime; }
}
