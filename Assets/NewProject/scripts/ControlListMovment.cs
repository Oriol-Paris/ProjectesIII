using System.Collections.Generic;
using System;
using UnityEngine;

public class ControlListMovment : MonoBehaviour
{
   public TimeSecuence timeSceuence;
    private float currentTime;
    
    public List<Tuple<Vector3, Vector3, Vector3>> MovList = new List<Tuple<Vector3, Vector3, Vector3>>();

    [SerializeField] private float maxDistance = 20f;

    public float t = 0;


    void Start()
    {
        currentTime = timeSceuence.totalTime;
        timeSceuence.actualTime = currentTime;
    }

    public void AddMovement(ControlLiniarRender controlLiniarRender)
    {
        List<Vector3> curvePoints = controlLiniarRender.getCurvePoint();
        float curveLength = controlLiniarRender.CalculateCurveLength();

        float timeConsumption = CalculateStaminaConsumption(curveLength);


        if (currentTime > timeConsumption)
        {
            Debug.Log("aaaaa");
            MovList.Add(Tuple.Create(curvePoints[0], curvePoints[1], curvePoints[2]));

            timeSceuence.AddAction(PlayerBase.ActionEnum.MOVE, controlLiniarRender.positionDesired, null);
            Debug.Log($"Movimiento registrado. Consumo de estamina: {timeConsumption}");

            // Deducir la estamina
            currentTime -= timeConsumption;

            timeSceuence.actualTime = currentTime;


            controlLiniarRender.NextMov();

            t = 0f;


            controlLiniarRender.InstantiateLineMovment();


        }
        else
        {
            FindAnyObjectByType<UIBarManager>().NotEnoughStaminaAnim();
            Debug.Log("No tienes suficiente estamina para realizar este trayecto.");

        }
    }

  

    private float CalculateStaminaConsumption(float distance)
    {
        // Ajustar este c�lculo seg�n la l�gica de tu juego
        float consumptionPerUnit = timeSceuence.totalTime / maxDistance;
        return Mathf.Clamp(distance * consumptionPerUnit, 0, timeSceuence.totalTime);
    }
}
