using System.Collections;
using System.Collections.Generic;
using TMPro;

using UnityEngine;

public class TimeSecuence : MonoBehaviour
{
    [SerializeField]
    public float actualTime;
    public float totalTime = 3;
    public bool isExecuting;
    public Vector3 lastPosition;

    public bool play = false;

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
        play = true;

        for (int i = 0; i < actions.Count; i++)
        {
           
            PlayerBase.ActionEnum action = actions[i];
            Debug.Log(action.ToString());
         

            switch (action)
            {
                case PlayerBase.ActionEnum.SHOOT:
                 
                    shootPl.UpdateShoot(movCount);
                    yield return new WaitForSeconds(0.75f);
                    movCount++;
                    break;
                case PlayerBase.ActionEnum.MOVE:

                   
                    while (movPlayer.t < 1f) 
                    {
                      
                        movPlayer.UpdateMovement(movCount);
                        yield return null; 
                    }
                    movPlayer.StopMovment();
                    movCount++;
                    break;
                
            }
        }
        ResetTurn();
    }

    void PassTurn()
    {
        if (actions.Count > 0)
        {
            isExecuting = true;

    
        
        StartCoroutine(ExecuteActions());
            actualTime = totalTime;

        }
        
    }

    private void ResetTurn()
    {
        play = false;
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

        FindAnyObjectByType<TopBarManager>().ResetTopBar();
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