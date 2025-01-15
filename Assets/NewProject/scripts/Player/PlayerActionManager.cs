using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;

public class PlayerActionManager : MonoBehaviour
{
    #region VARIABLES

    public Vector3 mousePosition;
    public Vector3 positionDesired;
    public Vector3 playerPosition;

    [SerializeField] LineRenderer lineRenderer;



    [SerializeField] private List<Vector3> curvePoints = new List<Vector3>();

    [SerializeField] public List<GameObject> visualPlayerAfterShoot = new List<GameObject>();

    public GameObject prefPreShoot;

    public TimeSecuence timeSceuence;
    private float currentTime = 3;


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

    public float costShoot = 0.4f;

    [SerializeField] private float walkSoundDelay;
    float actualWalkSoundDelay;
    private CombatManager combatManager;
    [SerializeField] MovPlayer movePlayer;
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
        UpdateAction(movePlayer.positionDesired, movePlayer.timeSceuence.actualTime);
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
            movePlayer.PreStartMov();
        }

        if (currentAction.m_action == PlayerBase.ActionEnum.SHOOT && (!player.GetComponent<OG_MovementByMouse>().isMoving || isShooting))
        {

            Debug.Log("aaaaaaaaaaaa");
            isShooting = true;
            hasShot = true; // Set the flag to indicate a shot has been fired
            isMoving = false;
            preShoot();
            //StartCoroutine(AttackCoroutine(PlayerBase.ActionEnum.SHOOT, newPos));


        }

        if (currentAction.m_action == PlayerBase.ActionEnum.MELEE && (!player.GetComponent<OG_MovementByMouse>().GetIsMoving() || isMoving))
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

    private void preShoot()
    {
        Debug.Log("aaaaa");

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            mousePosition = hit.point;
        }
        playerPosition = movePlayer.playerPosition;
        currentTime = timeSceuence.actualTime;

        if (!Input.GetMouseButton(0) && !isMoving)
        {

            positionDesired = mousePosition;

            curvePoints.Clear();
            curvePoints.Add(playerPosition);
            curvePoints.Add(positionDesired);

            UpdateLineRendererr();
        }

        if (Input.GetMouseButtonUp(0) && !isMoving)
        {

            if (currentTime > costShoot)
            {
                GameObject instantiatedObject = Instantiate(prefPreShoot, playerPosition, Quaternion.identity);

              
                visualPlayerAfterShoot.Add(instantiatedObject);


                currentTime -= costShoot;

                timeSceuence.actualTime = currentTime;


            }

        }
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
            if (dialogueManager != null)
                dialogueManager.ActionCompleted(PlayerBase.ActionEnum.MOVE);
        }
    }

    public IEnumerator AttackCoroutine(PlayerBase.ActionEnum action, Vector3 newPos, PlayerData.BulletStyle style)
    {
        this.GetComponent<Animator>().SetTrigger("attack");
        fx.SetTrigger("playFX");

        yield return new WaitForSeconds(0.4f);

        if (action == PlayerBase.ActionEnum.SHOOT)
        {
            ((ShootAction)activeActions[PlayerBase.ActionEnum.SHOOT]).bulletPrefab = style.prefab;
            //SoundEffectsManager.instance.PlaySoundFXClip(shootClip, transform, 1f);
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
            if (dialogueManager != null)
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

    public PlayerBase GetPlayer() { return player; }
    public void EndTurn() { turnsDone++; } // Add this method to end the turn after resting
}
