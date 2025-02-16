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
    public bool isExecuting;
    public Vector3 lastPosition;

    public MovPlayer movPlayer;

    public GameObject player;

    public shootPlayer shootPl;

    private List<PlayerBase.ActionEnum> actions = new List<PlayerBase.ActionEnum>();
    private List<Vector3> actionTargets = new List<Vector3>();
    private List<PlayerData.BulletStyle> bulletStyles = new List<PlayerData.BulletStyle>(); // Add this line

    [SerializeField] private PlayerBase playerBase;
    [SerializeField] private PlayerActionManager actionManager;
    private PlayerBase.Action selectedAction;

    void Start()
    {
        totalTime = playerBase.playerData.maxTime;
        isExecuting = false;
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
            if(actions.Count > 0&&Input.GetKeyDown(KeyCode.C)&&!isExecuting) {

            
                ResetTurn();


            }
            // Check for mouse click to store the selected action
          /*  if (selectedAction.m_action != PlayerBase.ActionEnum.MOVE && Input.GetMouseButtonDown(0))
            {
                Vector3 targetPosition = GetMouseTargetPosition();
                AddAction(selectedAction.m_action); // Modify this line
                Debug.Log("Stored action: " + selectedAction.m_action + " at position: " + targetPosition);
                //selectedAction = PlayerBase.Action.nothing; // Reset the selected action
            }*/
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && !isExecuting)
        {
            PassTurn();
        }
    }

    public void AddAction(PlayerBase.ActionEnum action) { actions.Add(action);}

    IEnumerator ExecuteActions()
    {
        int movCount = 0;
        Debug.Log(actions.Count);

        for (int i = 0; i < actions.Count; i++)
        {
           
            PlayerBase.ActionEnum action = actions[i];
            Debug.Log(action.ToString());
           // Vector3 targetPosition = actionTargets[i];
            //PlayerData.BulletStyle bulletStyle = bulletStyles[i]; // Add this line

            switch (action)
            {
                case PlayerBase.ActionEnum.SHOOT:
                    //Debug.Log("Using bullet style: " + bulletStyle.prefab.name); // Add this line
                    //((ShootAction)actionManager.activeActions[PlayerBase.ActionEnum.SHOOT]).bulletPrefab = bulletStyle.prefab; // Add this line
                    // StartCoroutine(actionManager.AttackCoroutine(action, targetPosition,bulletStyle));
                    shootPl.UpdateShoot(movCount);
                    yield return new WaitForSeconds(0.75f);
                    movCount++;
                    break;
                case PlayerBase.ActionEnum.MOVE:


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
        ResetTurn();
    }

    void PassTurn()
    {
        if (actions.Count > 0)
        {
            isExecuting = true;
        Debug.Log("Executing stored actions");
        
        StartCoroutine(ExecuteActions());
            actualTime = totalTime;
        }
        
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

    private void ResetTurn()
    {
        actions.Clear();
        actionTargets.Clear();
        bulletStyles.Clear(); // Add this line
        movPlayer.finish();
        foreach (var line in actionManager.visualPlayerAfterShoot)
        {
            Destroy(line.gameObject);
        }
        foreach (var line in actionManager.preShootPath)
        {
            Destroy(line.gameObject);
            
        }
        actionManager.preShootPath.Clear();
        actionManager.visualPlayerAfterShoot.Clear();
        actionManager.shootpoints.Clear();
        isExecuting = false;
    }
    public bool GetIsExecuting() { return isExecuting; }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Walls")
        {
            actions.Clear();
            actionTargets.Clear();
            bulletStyles.Clear(); // Add this line
            movPlayer.finish();
            isExecuting = false;
            StopAllCoroutines();
        }
    }
}