using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyMovementShooter : MonoBehaviour
{
    public enum TurnActions { APPROACH, SHOOT, BACK_AWAY, NOTHING };
    public TurnActions turnAction = TurnActions.NOTHING;

    [SerializeField] private List<TimeSecuence> players = new List<TimeSecuence>();
    public TimeSecuence closestPlayer;
    private Vector3 closestPlayerPos;
    public EnemyBase enemyStats;
    [SerializeField] private GameObject bulletShot;
    [SerializeField] private float velocity;
    [SerializeField] private float range;
    [SerializeField] private float minDistance = 2f;
    [SerializeField] private AudioClip[] shootingClips;
    [SerializeField] private Animator animator;
    public Animator fx;
    private bool onCooldown = false;
    private bool isPerformingAction = false;
    private bool hasShot = false;
    private float distanceToPlayer;
    private NavMeshAgent agent;
    [SerializeField] private float actionCooldown = 0f;
    private float initialMultiplier;
    [SerializeField] CombatManager combatManager;
    void Start()
    {
        enemyStats = GetComponent<EnemyBase>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = velocity;
        //agent.stoppingDistance = range;
        players.AddRange(FindObjectsOfType<TimeSecuence>());

        
        initialMultiplier = combatManager.enemyStatMultiplier;
        UpdateStats(initialMultiplier);

        agent.updateRotation = false;
    }

    void Update()
    {
        
        if (combatManager.enemyStatMultiplier != initialMultiplier)
        {
            initialMultiplier = combatManager.enemyStatMultiplier;
            UpdateStats(initialMultiplier);
        }

        FindClosestPlayer();

        if (!enemyStats.isAlive || closestPlayer == null)
            return;

        if (!closestPlayer.isExecuting)
        {
            agent.isStopped = true;
            return;
        }
        else if (agent.isStopped)
        {
            agent.isStopped = false; // Resume movement if it was previously stopped
        }

        // Only execute actions when allowed and not in cooldown
        if (!onCooldown && !isPerformingAction)
        {
            DecideAction();
            ExecuteAction();
        }
    }

    private void ExecuteAction()
    {
        if (onCooldown || isPerformingAction) return;
        isPerformingAction = true;

        switch (turnAction)
        {
            case TurnActions.APPROACH:
                MoveTowardsPlayer();
                break;

            case TurnActions.SHOOT:
                agent.isStopped = true;
                animator.SetBool("isMoving", false);
                StartCoroutine(AttackCoroutine());
                break;

            case TurnActions.BACK_AWAY:
                MoveAwayFromPlayer();
                break;

            case TurnActions.NOTHING:
                StartCoroutine(ActionCooldownCoroutine());
                break;
        }
    }

    private void MoveTowardsPlayer()
    {
        if (agent != null && closestPlayer != null)
        {
            agent.isStopped = false;
            Vector3 directionToPlayer = (closestPlayer.transform.position - transform.position).normalized;
            Vector3 destination = closestPlayer.transform.position - directionToPlayer * range;

            agent.SetDestination(destination);

            // Check if the enemy can reach the player
            //StartCoroutine(CheckIfPathIsStuck());
            StartCoroutine(Reload());
        }
    }

    private void MoveAwayFromPlayer()
    {
        if (agent != null && closestPlayer != null)
        {
            agent.isStopped = false;
            Vector3 directionAway = (transform.position - closestPlayer.transform.position).normalized;
            Vector3 newPosition = transform.position + directionAway * minDistance * 2f;

            agent.SetDestination(newPosition);

            // Ensure movement is not stuck
            //StartCoroutine(CheckIfPathIsStuck());
            StartCoroutine(WaitForMoveCompletion());
        }
    }

    // If enemy is stuck, retry the action
    private IEnumerator CheckIfPathIsStuck()
    {
        float stuckTimer = .5f; // Time to consider as stuck
        float elapsed = 0f;
        Vector3 lastPosition = transform.position;

        while (elapsed < stuckTimer)
        {
            yield return new WaitForSeconds(0.5f);
            elapsed += 0.5f;

            // If the enemy hasn't moved significantly, it's stuck
            if (Vector3.Distance(transform.position, lastPosition) < 0.1f)
            {
                // Recalculate a new path
                DecideAction();
                ExecuteAction();
                yield break;
            }

            lastPosition = transform.position;
        }

        // If it couldn't move at all, stop trying to move
        agent.isStopped = true;
        StartCoroutine(ActionCooldownCoroutine());
    }


    private IEnumerator WaitForMoveCompletion()
    {
        yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);
        
    }


    private void UpdateStats(float multiplier)
    {
        velocity *= multiplier;
        range *= multiplier;
        agent.speed = velocity;
    }



    private void FindClosestPlayer()
    {
        float closestDistanceSqr = float.MaxValue;

        foreach (var player in players)
        {
            float sqrDistance = (transform.position - player.transform.position).sqrMagnitude;
            if (sqrDistance < closestDistanceSqr)
            {
                closestDistanceSqr = sqrDistance;
                closestPlayer = player;
                closestPlayerPos = player.transform.position;
            }
        }
    }

    private void Shoot()
    {
        if (!hasShot)
        {
            Debug.Log("BANG");
            GameObject bullet = Instantiate(bulletShot, transform.position, Quaternion.identity);
            bullet.GetComponent<multiShoot>().setShootDirection((closestPlayerPos - transform.position).normalized, false);
            

            hasShot = true; // Mark that it has shot this turn

            SoundEffectsManager.instance.PlaySoundFXClip(shootingClips, transform, 1f);

            // Register the bullet with the closest player's movement script
            //closestPlayer.RegisterBullet(bulletScript);


        }
    }

    // Reload coroutine to wait until the next GetIsMoving toggle after shooting


    private IEnumerator AttackCoroutine()
    {
        fx.SetTrigger("playFX");


        if (enemyStats.isAlive && closestPlayer.isExecuting)
        {
            GetComponent<Animator>().SetTrigger("attack");
            yield return new WaitForSeconds(0.3f);
            closestPlayerPos = closestPlayer.transform.position;
            Shoot();
            fx.ResetTrigger("playFX");
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        yield return new WaitUntil(() => !closestPlayer.isExecuting);
        yield return new WaitUntil(() => closestPlayer.isExecuting == true);
        hasShot = false;
        StartCoroutine(ActionCooldownCoroutine());
    }

    public void DecideAction()
    {
        if (closestPlayer.actualTime < 1f)
        {
            turnAction = TurnActions.NOTHING;
            
        }

        distanceToPlayer = Vector3.Distance(transform.position, closestPlayerPos);

        if (distanceToPlayer > range)
        {
            turnAction = TurnActions.APPROACH;
        }
        else if (distanceToPlayer < minDistance)
        {
            turnAction = TurnActions.BACK_AWAY;
        }
        else if (!hasShot && distanceToPlayer <= range)
        {
            turnAction = TurnActions.SHOOT;
        }
    }


    private IEnumerator ActionCooldownCoroutine()
    {
        onCooldown = true;
        yield return new WaitForSeconds(actionCooldown);
        onCooldown = false;
        isPerformingAction = false;
        DecideAction();
    }

    public void ResetTurnAction()
    {
        turnAction = TurnActions.NOTHING;
    }
}