using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject[] popUps;
    public PlayerBase playerBase; // Reference to the PlayerBase component

    private int popUpIndex;

    // Dictionary to store the required number of actions for each popUpIndex
    private Dictionary<int, (PlayerBase.ActionEnum action, int count)> requiredActions = new Dictionary<int, (PlayerBase.ActionEnum action, int count)>
    {
        { 0, (PlayerBase.ActionEnum.MOVE, 2) }, // Requires 2 MOVE actions
        { 1, (PlayerBase.ActionEnum.SHOOT, 1) }, // Requires 1 SHOOT action
        { 2, (PlayerBase.ActionEnum.HEAL, 1) } // Requires 1 HEAL action
        // Add more entries as needed
    };

    // Dictionary to track the current count of actions performed for each popUpIndex
    private Dictionary<int, int> actionCounts = new Dictionary<int, int>();

    private void Start()
    {
        // Initialize actionCounts dictionary
        foreach (var key in requiredActions.Keys)
        {
            actionCounts[key] = 0;
        }
    }

    private void Update()
    {
        for (int i = 0; i < popUps.Length; i++)
        {
            if (i == popUpIndex)
            {
                popUps[popUpIndex].SetActive(true);
            }
            else
            {
                popUps[i].SetActive(false);
            }
        }

        CheckPlayerActions();
    }

    private void CheckPlayerActions()
    {
        if (requiredActions.ContainsKey(popUpIndex))
        {
            var requiredAction = requiredActions[popUpIndex].action;
            var requiredCount = requiredActions[popUpIndex].count;

            if (playerBase.GetAction().m_action == requiredAction && Input.GetMouseButtonUp(0))
            {
                actionCounts[popUpIndex]++;
                Debug.Log($"{requiredAction} performed {actionCounts[popUpIndex]} times");

                if (actionCounts[popUpIndex] >= requiredCount)
                {
                    Debug.Log($"{requiredAction} sequence completed");
                    popUpIndex++;
                }
            }
        }
    }
}
