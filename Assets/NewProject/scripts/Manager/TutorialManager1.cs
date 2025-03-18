using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class TutorialManager1 : MonoBehaviour
{
    public GameObject[] popUps;
    public PlayerBase playerBase; // Reference to the PlayerBase component
    public TimeSecuence timeSecuence; // Reference to the TimeSecuence component
    public GameObject dummy;
    private int popUpIndex = 0;
    private bool actionsCompleted = false; // Flag to indicate when the required actions are completed
    private bool isInsideTrigger = false; // Flag to indicate if the player is inside the trigger

    // Dictionary to store the required number of actions for each popUpIndex
    private Dictionary<int, (PlayerBase.ActionEnum action, int count)> requiredActions = new Dictionary<int, (PlayerBase.ActionEnum action, int count)>
    {
        { 0, (PlayerBase.ActionEnum.MOVE, 1) }, // Requires 2 MOVE actions
        { 1, (PlayerBase.ActionEnum.MOVE, 2) }, // Requires 1 SHOOT action
        { 2, (PlayerBase.ActionEnum.SHOOT, 1) } // Requires 1 HEAL action
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
        // Make the dummy invisible at the start
        var collider = dummy.GetComponent<BoxCollider>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        var spriteRenderer = dummy.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        var rigidbody = dummy.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.useGravity = false;
        }
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
            popUpIndex++;
            Debug.Log("FROM HERE");
            DisplayCurrentPopup();
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
            popUps[popUpIndex].SetActive(true);
            Debug.Log("Displaying popUpIndex: " + popUpIndex);
            ExecuteCustomLogicForPopup(popUpIndex); // Execute custom logic for the current popup
        }

    }

    private void DisableCurrentPopup()
    {
        if (popUpIndex < popUps.Length)
        {
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

                    if (playerBase.GetAction().m_action == requiredAction && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
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

                    if (playerBase.GetAction().m_action == requiredAction && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
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
                var collider = dummy.GetComponent<BoxCollider>();
                if (collider != null)
                {
                    collider.enabled = true;
                }

                var spriteRenderer = dummy.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.enabled = true;
                }

                var rigidbody = dummy.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    rigidbody.useGravity = true;
                }
                if (requiredActions.ContainsKey(popUpIndex))
                {
                    var requiredAction = requiredActions[popUpIndex].action;
                    var requiredCount = requiredActions[popUpIndex].count;

                    if (playerBase.GetAction().m_action == requiredAction && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
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



    private bool isInsideGoal()
    {
        return isInsideTrigger;
    }
}
