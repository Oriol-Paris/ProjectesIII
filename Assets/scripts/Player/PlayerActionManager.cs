using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;
using static PlayerData;

public class PlayerActionManager : MonoBehaviour
{
    #region VARIABLES

    public Vector3 mousePosition;
    public Vector3 positionDesired;
    public Vector3 playerPosition;

    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] LineRenderer shootLineRenderer;


    [SerializeField] public List<Vector3> shootpoints = new List<Vector3>();
    [SerializeField] private List<Vector3> curvePoints = new List<Vector3>();

    [SerializeField] public List<GameObject> visualPlayerAfterShoot = new List<GameObject>();
    [SerializeField] public List<LineRenderer> preShootPath = new List<LineRenderer>();
    public GameObject prefPreShoot;

    public TimeSecuence timeSceuence;
    


    [SerializeField] private PlayerBase player;
    [SerializeField] private Dialogue dialogueManager;
    public Animator fx;
    private PlayerData playerData;

    // Dictionaries to store actions by type
    public Dictionary<PlayerBase.ActionEnum, ActiveAction> activeActions;
    public Dictionary<PlayerBase.ActionEnum, PassiveAction> passiveActions;

    public bool isMoving = true;
    public bool isShooting = true;
  
    public bool isHealing = true; // New flag for healing
    public bool turnAdded = false;
    public int turnsDone = 0;

    private bool hasMoved = false;
    private bool hasShotAction = false;
    private bool hasHealed = false;


    public float costShoot = 0.4f;

    [SerializeField] private float walkSoundDelay;
    float actualWalkSoundDelay;
    private CombatManager combatManager;
    [SerializeField] MovPlayer movePlayer;
    private bool hasShot = false; // Flag to track if a shot has been fired
    private bool actionPointReduced;
    private Animator animationToExecute;
    [SerializeField] public AudioClip[] shootClip;
    [SerializeField] AudioClip[] walkingClips;



    public shootPlayer shootP;
    #endregion

    private void Awake()
    {
        activeActions = new Dictionary<PlayerBase.ActionEnum, ActiveAction>();
        passiveActions = new Dictionary<PlayerBase.ActionEnum, PassiveAction>();
        animationToExecute = GetComponent<Animator>();
    }

    private void Start()
    {
        player = GetComponent<PlayerBase>();
       

        playerData = player.playerData; // Load playerData from PlayerBase

        combatManager = FindAnyObjectByType<CombatManager>();
    }

  
    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "ShopScene")
        {
            UpdateAction(Vector3.zero, movePlayer.timeSceuence.actualTime);
        }
            

     
    }


    public void UpdateAction(Vector3 newPos, float t)
    {
        if (combatManager != null && combatManager.allEnemiesDead)
        {
            return; // Do not execute any actions if victory condition is met
        }

        var currentAction = player.GetAction();

        if (currentAction.m_action == PlayerBase.ActionEnum.MOVE && (!player.GetComponent<TimeSecuence>().isExecuting || isMoving))
        {
            isMoving = true;
            movePlayer.PreStartMov();
        }

        if (currentAction.m_action == PlayerBase.ActionEnum.SHOOT && (!player.GetComponent<TimeSecuence>().isExecuting || isShooting))
        {
   
            isShooting = true;
            hasShot = true; 
            isMoving = false;
            shootP.preShoot();

        }
        if (currentAction.m_action == PlayerBase.ActionEnum.ESPECIALSHOOT )
        {

            isShooting = true;
            hasShot = true;
            isMoving = false;
           
            shootP.preEspecialShoot();
         

        }

        if (currentAction.m_action == PlayerBase.ActionEnum.MELEE && (!player.GetComponent<TimeSecuence>().isExecuting || isMoving))
        {
            if (currentAction.m_cost <= playerData.actionPoints)
            {
                isMoving = false;
                StartCoroutine(AttackCoroutine(PlayerBase.ActionEnum.MELEE, newPos, null));
            }
        }

        if (currentAction.m_action == PlayerBase.ActionEnum.HEAL && isHealing)
        {
            isMoving = false;
            StartCoroutine(HealCoroutine(newPos));
        }

        if (t >= 1)
        {
            ResetFlags();

            foreach (EnemyMovementShooter enemy in FindObjectsByType<EnemyMovementShooter>(FindObjectsSortMode.None))
            {
                enemy.ResetTurnAction();
                enemy.DecideAction();
            }
        }
    }

    private void UpdateLineRendererr()
    {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = curvePoints.Count;
        lineRenderer.SetPositions(curvePoints.ToArray());
        
    }

   

    private IEnumerator MoveCoroutine(Vector3 newPos)
    {
        activeActions[PlayerBase.ActionEnum.MOVE].Execute(player, newPos);
        if (actualWalkSoundDelay < 0)
        {
            //SoundEffectsManager.instance.PlaySoundFXClip(walkingClips, transform, 1f);
            actualWalkSoundDelay = walkSoundDelay;
        }
        else
        {
            actualWalkSoundDelay -= Time.deltaTime;
        }
        if (!actionPointReduced)
        {
            actionPointReduced = true;
            player.actionPoints++;
            player.actionPoints = MathF.Min(player.actionPoints, player.maxActionPoints);
            playerData.actionPoints++;
            playerData.actionPoints = Mathf.Min(playerData.actionPoints, playerData.maxActionPoints);
        }

        if (!hasMoved)
        {
            hasMoved = true;
            yield return new WaitForSeconds(1.5f); // Adjust the delay as needed
           
        }
    }
    
    public IEnumerator AttackCoroutine(PlayerBase.ActionEnum action, Vector3 newPos, BulletStyle style)
    {
        shootLineRenderer.enabled = false;
        this.GetComponent<Animator>().SetTrigger("attack");
        fx.SetTrigger("playFX");

        yield return new WaitForSeconds(0.4f);

        if (action == PlayerBase.ActionEnum.SHOOT)
        {
            ((ShootAction)activeActions[PlayerBase.ActionEnum.SHOOT]).bulletPrefab = style.prefab;
            SoundEffectsManager.instance.PlaySoundFXClip(shootClip, transform, 1f);
            
            activeActions[PlayerBase.ActionEnum.SHOOT].Execute(player, newPos);
            
        }
        else
        {
            activeActions[PlayerBase.ActionEnum.MELEE].Execute(player, newPos);
        }

        fx.ResetTrigger("playFX");

        if (!actionPointReduced)
        {
            actionPointReduced = true;
            player.actionPoints -= player.GetAction().m_cost;
            playerData.actionPoints -= player.GetAction().m_cost;
        }


        if (action == PlayerBase.ActionEnum.SHOOT && !hasShotAction)
        {
            hasShotAction = true;
            yield return new WaitForSeconds(1.0f); // Adjust the delay as needed
            
        }
    }

    private IEnumerator HealCoroutine(Vector3 newPos)
    {
        
        passiveActions[PlayerBase.ActionEnum.HEAL].Execute(player, newPos);
        if (!actionPointReduced && player.health < player.maxHealth)
        {
            actionPointReduced = true;
            player.actionPoints -= player.GetAction().m_cost;
            playerData.actionPoints -= player.GetAction().m_cost;
            
        }
        yield return new WaitForSeconds(1f); // Adjust the delay as needed
        if (!hasHealed)
        {
            hasHealed = true;
            
        }
    }

    public void ResetFlags()
    {
        hasShot = false; // Reset the flag when the player stops moving
        turnAdded = false;
        actionPointReduced = false;
        
    }

    public void WalkingSound()
    {
        
        if (actualWalkSoundDelay < 0)
        {
            SoundEffectsManager.instance.PlaySoundFXClip(walkingClips, transform, 1f);
            actualWalkSoundDelay = walkSoundDelay;
        }
        else
        {
            actualWalkSoundDelay -= Time.deltaTime;
        }
    }
    public void EquipNewAction(ActionData actionData)
    {
        if (actionData.actionType != PlayerBase.ActionType.SINGLE_USE)
        {
            actionData.key = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Alpha" + (player.playerData.availableActions.Count + 1));
            player.playerData.availableActions.Add(actionData);
            Debug.Log("Item equipped");
        }
    }
    public PlayerBase GetPlayer() { return player; }
    public void EndTurn() { turnsDone++; } // Add this method to end the turn after resting
}
