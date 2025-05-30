﻿using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;

public class ControlListMovment : MonoBehaviour
{
    public TimeSecuence timeSceuence;
    private float currentTime;
    public PlayerBase player;
    public List<Tuple<Vector3, Vector3, Vector3>> MovList = new List<Tuple<Vector3, Vector3, Vector3>>();
    public List<float> timeConsum = new List<float>();
    public List<GameObject> ghostsPlayers = new List<GameObject>();

    [SerializeField] private float maxDistance = 10f;

    public float t = 0;

    public GameObject ghostPlayer;


    void Start()
    {
        currentTime = timeSceuence.totalTime;
        timeSceuence.actualTime = currentTime;
        StartCoroutine(InvokeGhost());
    }

    public void AddMovement(ControlLiniarRender controlLiniarRender, float timeConsumition,float time, PlayerBase.ActionEnum action)
    {
        List<Vector3> curvePoints = controlLiniarRender.getCurvePoint();

        if (currentTime >= time)
        {
            // Debug.Log(curvePoints[0]);
            // Debug.Log(curvePoints[1]);


            Vector3 point3 = curvePoints.Count > 2 ? curvePoints[2] : Vector3.zero;

            MovList.Add(Tuple.Create(curvePoints[0], curvePoints[1], point3));

            timeSceuence.AddAction(action);

            FindAnyObjectByType<TopBarManager>().AddAction(action);

            Debug.Log($"Movimiento registrado. Consumo de estamina: {timeConsumition}");

            // Deducir la estamina
            currentTime -= time;

            timeConsum.Add(timeConsumition);

            timeSceuence.actualTime = currentTime;


            controlLiniarRender.NextMov(point3 != Vector3.zero);

            t = 0f;


            controlLiniarRender.InstantiateLineMovment();
            FindAnyObjectByType<APBarManager>().PaintAPCost();
        }
        else
        {
            Debug.Log("No tienes suficiente estamina para realizar este trayecto.");
        }
    }

    public void DestroyAllGhostrs()
    {
        foreach (GameObject obj in ghostsPlayers)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        ghostsPlayers.Clear();
    }

    public void DestroyLastGhost()
    {
        if (ghostsPlayers.Count > 0)
        {
            Destroy(ghostsPlayers[ghostsPlayers.Count - 1]);
            ghostsPlayers.RemoveAt(ghostsPlayers.Count - 1);
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



    IEnumerator InvokeGhost()
    {

        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            if (MovList.Count != 0 && timeSceuence.canInvokeGhost == true)
            {
                ghostsPlayers.Add( Instantiate(ghostPlayer,this.transform.position, this.transform.rotation));
            }
        }
    }
}
