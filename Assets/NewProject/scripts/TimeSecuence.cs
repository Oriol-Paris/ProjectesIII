using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeSecuence : MonoBehaviour
{
    [SerializeField]
    public float actualTime;
    public float totalTime = 3;
    float rang = 10f;

    public Vector3 lastPosition;

    public MovPlayer movPlayer;

    public GameObject player;

    public shootPlayer shootPl;

    private List<PlayerBase.ActionEnum> actions = new List<PlayerBase.ActionEnum>();
    private List<Vector3> actionTargets = new List<Vector3>();

    [SerializeField] private PlayerBase playerBase;
    [SerializeField] private PlayerActionManager actionManager;

    private PlayerBase.Action selectedAction;

    void Start()
    {
        actualTime = totalTime;
        lastPosition = transform.position;

        playerBase = player.GetComponent<PlayerBase>();
        actionManager = player.GetComponent<PlayerActionManager>();
    }

    void Update()
    {
        if (actualTime > 0)
        {
            // Check for action selection
            foreach (var action in playerBase.availableActions)
            {
                if (Input.GetKeyDown(action.m_key))
                {
                    selectedAction = action;
                    Debug.Log("Selected action: " + selectedAction.m_action);
                }
            }

            // Check for mouse click to store the selected action
            if (selectedAction.m_action != PlayerBase.ActionEnum.MOVE && Input.GetMouseButtonDown(0))
            {
                Vector3 targetPosition = GetMouseTargetPosition();
                AddAction(selectedAction.m_action, targetPosition);
                Debug.Log("Stored action: " + selectedAction.m_action + " at position: " + targetPosition);
                //selectedAction = PlayerBase.Action.nothing; // Reset the selected action
            }

            
                //movPlayer.PreStartMov();
            
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            PassTurn();
        }
    }

    public void AddAction(PlayerBase.ActionEnum action, Vector3 targetPosition)
    {
        actions.Add(action);
        actionTargets.Add(targetPosition);
        Debug.Log("HOLA");
    }

    IEnumerator ExecuteActions()
    {
        int movCount = 0;
        int shootCount = 0;
        Debug.Log(actions.Count);

        for (int i = 0; i < actions.Count; i++)
        {
            PlayerBase.ActionEnum action = actions[i];
            Debug.Log(action.ToString());
            Vector3 targetPosition = actionTargets[i];

            switch (action)
            {
                case PlayerBase.ActionEnum.SHOOT:
                    //Debug.Log("Executing shoot action");
                    actionManager.UpdateAction(targetPosition, 1f); // Execute the shoot action
                    StartCoroutine(actionManager.AttackCoroutine(action, targetPosition));
                       yield return new WaitForSeconds(0.75f);
                    shootCount++;
                    break;
                case PlayerBase.ActionEnum.MOVE:
                    //Debug.Log("Executing move action");
                    //movPlayer.StartMov();

                    while (movPlayer.t < 1f) // Wait for the movement to finish
                    {
                        movPlayer.UpdateMovement(movCount);
                        yield return null; // Wait for a frame
                    }
                    movPlayer.StopMovment();
                    movCount++;
                    break;
                // Add other cases for different actions if needed
            }
        }
        actions.Clear();
        actionTargets.Clear();
        movPlayer.finish();
    }

    void PassTurn()
    {
        Debug.Log("Executing stored actions");
        StartCoroutine(ExecuteActions());
        actualTime = totalTime;
    }

    private Vector3 GetMouseTargetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point; // 3D position of the hit object
        }
        return Vector3.zero; // Return Vector3.zero if no hit
    }
}