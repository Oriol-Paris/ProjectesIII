using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;

public class PlayerActionManager : MonoBehaviour
{
    #region VARIABLES

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

    [SerializeField] private float walkSoundDelay;
    float actualWalkSoundDelay;
    private CombatManager combatManager;
    private bool hasShot = false; // Flag to track if a shot has been fired
    private bool actionPointReduced;
    private Animator animationToExecute;
    [SerializeField] AudioClip[] shootClip;
    [SerializeField] AudioClip[] walkingClips;

    private bool hasMoved = false;
    private bool hasShotAction = false;
    private bool hasHealed = false;

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
        if (player == null)
        {
            Debug.LogError("PlayerBase component not found on the GameObject.");
        }
        else
        {
            Debug.Log("PlayerBase component found and assigned.");
        }

        playerData = player.playerData; // Load playerData from PlayerBase

        InitializeActions();
        combatManager = FindAnyObjectByType<CombatManager>();
    }

    private void InitializeActions()
    {
        foreach (var actionData in playerData.availableActions)
        {
            switch (actionData.actionType)
            {
                case PlayerBase.ActionType.ACTIVE:
                    if (actionData.action == PlayerBase.ActionEnum.MOVE)
                    {
                        activeActions.Add(actionData.action, new MoveAction());
                    }
                    else if (actionData.action == PlayerBase.ActionEnum.SHOOT)
                    {
                        if (!activeActions.ContainsKey(actionData.action))
                        {
                            activeActions.Add(actionData.action, new ShootAction());
                        }
                    }
                    else if (actionData.action == PlayerBase.ActionEnum.MELEE)
                    {
                        activeActions.Add(actionData.action, new MeleeAction());
                    }
                    break;
                case PlayerBase.ActionType.PASSIVE:
                    if (actionData.action == PlayerBase.ActionEnum.HEAL)
                    {
                        passiveActions.Add(actionData.action, new HealAction());
                    }
                    break;
                case PlayerBase.ActionType.SINGLE_USE:
                    if (actionData.action == PlayerBase.ActionEnum.REST)
                    {
                        activeActions.Add(actionData.action, new RestAction());
                    }
                    break;
                    // Add other cases if you have SingleUse or other action types
            }
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Title Screen");
        }
    }
    public void UpdateAction(Vector3 newPos, float t)
    {
        if (combatManager != null && combatManager.allEnemiesDead)
        {
            return; // Do not execute any actions if victory condition is met
        }

        var currentAction = player.GetAction();

        if (currentAction.m_action == PlayerBase.ActionEnum.MOVE && (!player.GetComponent<OG_MovementByMouse>().isMoving || isMoving))
        {
            isMoving = true;
            StartCoroutine(MoveCoroutine(newPos));
        }

        if (currentAction.m_action == PlayerBase.ActionEnum.SHOOT && (!player.GetComponent<OG_MovementByMouse>().isMoving || isShooting))
        {
            if (currentAction.m_cost <= playerData.actionPoints && !hasShot)
            {
                isShooting = true;
                hasShot = true; // Set the flag to indicate a shot has been fired
                StartCoroutine(AttackCoroutine(PlayerBase.ActionEnum.SHOOT, newPos));
            }
        }

        if (currentAction.m_action == PlayerBase.ActionEnum.MELEE && (!player.GetComponent<OG_MovementByMouse>().GetIsMoving() || isMoving))
        {
            if (currentAction.m_cost <= playerData.actionPoints)
            {
                isMoving = true;
                StartCoroutine(AttackCoroutine(PlayerBase.ActionEnum.MELEE, newPos));
            }
        }

        if (currentAction.m_action == PlayerBase.ActionEnum.HEAL && isHealing)
        {
            isMoving = true;
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

    private IEnumerator MoveCoroutine(Vector3 newPos)
    {
        activeActions[PlayerBase.ActionEnum.MOVE].Execute(player, newPos);
        if (actualWalkSoundDelay < 0)
        {
            SoundEffectsManager.instance.PlaySoundFXClip(walkingClips, transform, 1f);
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
            if (dialogueManager != null) 
            dialogueManager.ActionCompleted(PlayerBase.ActionEnum.MOVE);
        }
    }

    public IEnumerator AttackCoroutine(PlayerBase.ActionEnum action, Vector3 newPos)
    {
        this.GetComponent<Animator>().SetTrigger("attack");
        fx.SetTrigger("playFX");

        yield return new WaitForSeconds(0.4f);

        if (action == PlayerBase.ActionEnum.SHOOT)
        {
            ((ShootAction)activeActions[PlayerBase.ActionEnum.SHOOT]).bulletPrefab = player.activeStyle.prefab;
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
            if(dialogueManager != null)
            dialogueManager.ActionCompleted(action);
        }
    }

    private IEnumerator HealCoroutine(Vector3 newPos)
    {
        passiveActions[PlayerBase.ActionEnum.HEAL].Execute(player, newPos);
        if (!actionPointReduced)
        {
            actionPointReduced = true;
            player.actionPoints -= player.GetAction().m_cost;
            playerData.actionPoints -= player.GetAction().m_cost;
        }
        yield return new WaitForSeconds(1f); // Adjust the delay as needed
        if (!hasHealed)
        {
            hasHealed = true;
            if (dialogueManager != null)
                dialogueManager.ActionCompleted(PlayerBase.ActionEnum.HEAL);
        }
    }

    public void ResetFlags()
    {
        hasShot = false; // Reset the flag when the player stops moving
        turnAdded = false;
        actionPointReduced = false;
    }

    public PlayerBase GetPlayer() { return player; }
    public void EndTurn() { turnsDone++; } // Add this method to end the turn after resting
}
