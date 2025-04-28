using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public PlayerActionManager actionManager;
    public GameObject[] popUps;
    public PlayerBase playerBase; // Reference to the PlayerBase component
    public TimeSecuence timeSecuence; // Reference to the TimeSecuence component
    public GameObject dummy;
    public List<GameObject> pinPoints;
    public GameObject dummy1;

    private int popUpIndex = 0;
    private bool actionsCompleted = false; // Flag to indicate when the required actions are completed
    private bool isInsideTrigger = false; // Flag to indicate if the player is inside the trigger
    public GameObject CombatManager;
    public GameObject VictoryScreen;
    // Dictionary to store the required number of actions for each popUpIndex
    private Dictionary<int, (PlayerBase.ActionEnum action, int count)> requiredActions = new Dictionary<int, (PlayerBase.ActionEnum action, int count)>
    {
        { 0, (PlayerBase.ActionEnum.MOVE, 1) }, // Requires 1 MOVE action
        { 1, (PlayerBase.ActionEnum.MOVE, 2) }, // Requires 2 MOVE actions
        { 2, (PlayerBase.ActionEnum.SHOOT, 1) }, // Requires 1 SHOOT action
        { 3, (PlayerBase.ActionEnum.MOVE, 1) }, // Requires 1 MOVE action
        { 4, (PlayerBase.ActionEnum.SHOOT, 1) } // Requires 1 SHOOT action
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
        for(int i = 0; i< pinPoints.Count; i++)
        {
            pinPoints[i].SetActive(false);
        }

        CombatManager.SetActive(false);
        VictoryScreen.SetActive(false);
        dummy.SetActive(false);

        // Make the dummy1 invisible at the start
        dummy1.SetActive(false);
        // Display the first popup
        DisplayCurrentPopup();
    }

    private void Update()
    {
        //DisplayCurrentPopup();
        // Check if the required actions are completed and the sequence is not executing
        if (actionsCompleted && timeSecuence.isExecuting == false)
        {
            Debug.Log("Actions completed");
            actionsCompleted = false; // Reset the flag
            if (popUpIndex < 3)
            {
                popUpIndex++;
                Debug.Log("FROM HERE");
                DisplayCurrentPopup();
            }
        }

        if (timeSecuence.isExecuting == true)
        {
            popUps[popUpIndex].SetActive(false);
        }

        ExecuteCustomLogicForPopup(popUpIndex);
        // Check if the Enter key is pressed to disable the current popup
        if (Input.GetKeyDown(KeyCode.Return))
        {
            DisableCurrentPopup();
        }
        
    }

    private void DisplayCurrentPopup()
    {
       
        // Disable all popups
        foreach (var popUp in popUps)
        {
            popUp.SetActive(false);
        }

        // Enable the current popup
        if (popUpIndex < popUps.Length)
        {
            //actionManager.enabled = false;
            popUps[popUpIndex].SetActive(true);
            Debug.Log("Displaying popUpIndex: " + popUpIndex);
            ExecuteCustomLogicForPopup(popUpIndex); // Execute custom logic for the current popup
        }
        
    }

    public void DisableCurrentPopup()
    {
        if (popUpIndex < popUps.Length)
        {
            actionManager.enabled = true;
            popUps[popUpIndex].SetActive(false);
            Debug.Log("Disabling popUpIndex: " + popUpIndex);
        }
    }

    private void ExecuteCustomLogicForPopup(int index)
    {
        switch (index)
        {
            case 0:
                // Custom logic for popup index 0
                Debug.Log("Custom logic for popup index 0");
                if (requiredActions.ContainsKey(popUpIndex))
                {
                    var requiredAction = requiredActions[popUpIndex].action;
                    var requiredCount = requiredActions[popUpIndex].count;
                    pinPoints[index].SetActive(true);
                    if (playerBase.GetAction().m_action == requiredAction && (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Space)))
                    {
                        actionCounts[popUpIndex]++;
                        Debug.Log($"{requiredAction} performed {actionCounts[popUpIndex]} times");
                        transform.position = new Vector3(10, 0, 10); // Change the player's position to a new one
                        if (actionCounts[popUpIndex] >= requiredCount)
                        {
                            Debug.Log($"{requiredAction} sequence completed");
                            actionsCompleted = true; // Set the flag to indicate that the required actions are completed
                        }
                    }
                }
                break;
            case 1:
                // Custom logic for popup index 1
                Debug.Log("Custom logic for popup index 1");
                if (requiredActions.ContainsKey(popUpIndex))
                {
                    var requiredAction = requiredActions[popUpIndex].action;
                    var requiredCount = requiredActions[popUpIndex].count;
                    pinPoints[index - 1].SetActive(false);
                    pinPoints[index].SetActive(true);
                    if (playerBase.GetAction().m_action == requiredAction && (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Space)))
                    {
                        actionCounts[popUpIndex]++;
                        Debug.Log($"{requiredAction} performed {actionCounts[popUpIndex]} times");

                        if (actionCounts[popUpIndex] >= requiredCount)
                        {
                            Debug.Log($"{requiredAction} sequence completed");
                            actionsCompleted = true; // Set the flag to indicate that the required actions are completed
                        }
                    }
                }
                break;
            case 2:
                // Custom logic for popup index 2
                Debug.Log("Custom logic for popup index 2");
                dummy.SetActive(true);
                pinPoints[index - 1].SetActive(false);
                pinPoints[index].SetActive(true);
                var enemyBase = dummy.GetComponent<EnemyBase>();
                if (enemyBase != null && !enemyBase.isAlive)
                {
                    Debug.Log("Dummy1 is dead");
                    actionsCompleted = true; // Set the flag to indicate that the required actions are completed
                }
                break;

            case 3:
                VictoryScreen.SetActive(true);
                CombatManager.SetActive(true);
                pinPoints[index - 1].SetActive(false);
                pinPoints[index].SetActive(true);
                // Custom logic for popup index 2
                Debug.Log("Custom logic for popup index 2");
                dummy1.SetActive(true);
                var enemyBase1 = dummy1.GetComponent<EnemyBase>();
                if (enemyBase1 != null && !enemyBase1.isAlive)
                {
                    Debug.Log("Dummy1 is dead");
                    actionsCompleted = true; // Set the flag to indicate that the required actions are completed
                }
                break;
            // Add more cases as needed for other popup indices
            default:
                Debug.Log("No custom logic for this popup index");
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("WOOO");
        }
    }

    

    private bool isInsideGoal(){
        return isInsideTrigger;
    }
}
