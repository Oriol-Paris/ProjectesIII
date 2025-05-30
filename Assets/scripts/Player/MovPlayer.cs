using System;
using System.Collections.Generic;

using UnityEngine;

public class MovPlayer : MonoBehaviour
{

    public PlayerData playerData;
    public ControlLiniarRender controlLiniarRender;
    public ControlListMovment controlListMovment;
    public TimeSecuence timeSecuence;
    


    public float t;
    
    [SerializeField] public float velocity = 0.5f;
    [SerializeField] private float tiemp = 0f;
   
   
    
    public TimeSecuence timeSceuence;

    private float currentTime = 3;

    public bool placeSelected;
    public bool isMoving;
    public bool isDragging;

    void Start(){
        velocity = playerData.velocity;
        t = 0;
    }

    void Update(){}

    public void PreStartMov() { 
        if (!timeSecuence.notacction)
        {
            CanWalk();
            controlLiniarRender.Disable(false);
        }
        else
        {
            controlLiniarRender.Disable(true);
        }
      
    }


    public void StartMov() { placeSelected = true; }

    public void finish()
    {
        //playerPosition = transform.position;
        controlLiniarRender.ResetControlLiniarRenderer();
        controlListMovment.ResetControlListMovment();
    }



    public void UpdateMovement(int movCount)
    {

        this.GetComponent<Animator>().SetBool("isMoving", true);
        List<Tuple<Vector3, Vector3, Vector3>> MovList = controlListMovment.MovList;
            var firstItem = MovList[movCount];
            Vector3 _playerPosition = firstItem.Item1;
            Vector3 _controlPoint = firstItem.Item2;
            Vector3 _positionDesired = firstItem.Item3;
       

        this.GetComponent<PlayerActionManager>().WalkingSound();
       
        float duration = controlListMovment.timeConsum[movCount]; // Tiempo deseado para el movimiento
        float fixedDeltaTime = Time.deltaTime / duration;
        
       

        t += velocity * fixedDeltaTime;

        t = Mathf.Clamp(t, 0f, 1f);
       

        Vector3 newPosition = Directorio.BezierCurve(t, _playerPosition, _controlPoint, _positionDesired);
        newPosition.y = 0;
        transform.position = newPosition;
        

        tiemp += Time.deltaTime;
    }

    public void StopMovment()
    {
        t = 0f;
        timeSceuence.actualTime = currentTime;
        
        
    }

   


    private void CanWalk()
    {

        currentTime = timeSceuence.actualTime;
           

        if (!Input.GetMouseButton(0) && !isMoving && !placeSelected && !isDragging)
        {
           
            isDragging = false;  
            placeSelected = false;
            controlLiniarRender.ControlLiniarRenderer(true);
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
         
            float curveLength = controlLiniarRender.CalculateCurveLength();

            float timeConsumption = controlListMovment.CalculateStaminaConsumption(curveLength);

            float time = 1;

            controlListMovment.AddMovement(controlLiniarRender,timeConsumption,time, PlayerBase.ActionEnum.MOVE);
            t = 0f;
          
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
