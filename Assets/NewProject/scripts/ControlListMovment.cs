using System.Collections.Generic;
using System;
using UnityEngine;

public class ControlListMovment : MonoBehaviour
{
   public TimeSecuence timeSceuence;
    private float currentTime;
    
    public List<Tuple<Vector3, Vector3, Vector3>> MovList = new List<Tuple<Vector3, Vector3, Vector3>>();
    public List<float> timeConsum = new List<float>();

    [SerializeField] private float maxDistance = 10f;

    public float t = 0;


    void Start()
    {
        currentTime = timeSceuence.totalTime;
        timeSceuence.actualTime = currentTime;
    }

    public void AddMovement(ControlLiniarRender controlLiniarRender, float timeConsumition, PlayerBase.ActionEnum action)
    {
        List<Vector3> curvePoints = controlLiniarRender.getCurvePoint();
 
        if (currentTime > timeConsumition)
        {
           // Debug.Log(curvePoints[0]);
           // Debug.Log(curvePoints[1]);
           

            Vector3 point3 = curvePoints.Count > 2 ? curvePoints[2] : Vector3.zero;

            MovList.Add(Tuple.Create(curvePoints[0], curvePoints[1], point3));

            timeSceuence.AddAction(action);

            FindAnyObjectByType<TopBarManager>().AddAction(action);

            Debug.Log($"Movimiento registrado. Consumo de estamina: {timeConsumition}");

            // Deducir la estamina
            currentTime -= timeConsumition;

            timeConsum.Add(timeConsumition);

            timeSceuence.actualTime = currentTime;
           

            controlLiniarRender.NextMov(point3 != Vector3.zero);
          
            t = 0f;


            controlLiniarRender.InstantiateLineMovment();


        }
        else
        {
            FindAnyObjectByType<UIBarManager>().NotEnoughStaminaAnim();
            Debug.Log("No tienes suficiente estamina para realizar este trayecto.");

        }
    }

  

    public float CalculateStaminaConsumption(float distance)
    {
        float maxDist = maxDistance;

        return Mathf.Clamp((distance / maxDist) * timeSceuence.totalTime, 0, timeSceuence.totalTime);
    }

    public void ResetControlListMovment()
    {  
        MovList.Clear();
        timeConsum.Clear();
        currentTime = timeSceuence.totalTime;
        timeSceuence.actualTime = currentTime;
    }
}
