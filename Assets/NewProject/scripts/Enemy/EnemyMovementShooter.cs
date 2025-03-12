using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    public Animator fx;
    private bool onCooldown = false;
    private bool isReloading = false;
    private bool hasShot = false;
    private bool isPerformingAction = false;
    private float distanceToPlayer;
    private NavMeshAgent agent;
    [SerializeField] private float actionCooldown = 0.5f;
    private float initialMultiplier;

    void Start()
    {
        enemyStats = GetComponent<EnemyBase>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = velocity;

        TimeSecuence[] playersArray = GameObject.FindObjectsByType<TimeSecuence>(FindObjectsSortMode.None);
        players.AddRange(playersArray);

        CombatManager combatManager = FindAnyObjectByType<CombatManager>();
        initialMultiplier = combatManager.enemyStatMultiplier;
        UpdateStats(initialMultiplier);

        agent.updateRotation = false;
    }

    void Update()
    {
        CombatManager combatManager = FindAnyObjectByType<CombatManager>();
        if (combatManager.enemyStatMultiplier != initialMultiplier)
        {
            initialMultiplier = combatManager.enemyStatMultiplier;
            UpdateStats(initialMultiplier);
        }

        FindClosestPlayer();

        if (enemyStats.isAlive && closestPlayer != null && closestPlayer.isExecuting && !onCooldown && !isPerformingAction)
        {
            ExecuteAction();
        }
    }

    private void UpdateStats(float multiplier)
    {
        velocity *= multiplier;
        range *= multiplier;
        agent.speed = velocity;
    }

    private void ExecuteAction()
    {
        if (onCooldown || isPerformingAction) return;

        isPerformingAction = true; // Evita que cambie de acción hasta terminar
        DecideAction(); // Solo decide una vez por turno

        switch (turnAction)
        {
            case TurnActions.APPROACH:
                MoveTowardsPlayer();
                GetComponent<Animator>().SetBool("isMoving", true);
                break;

            case TurnActions.SHOOT:
                GetComponent<Animator>().SetBool("isMoving", false);
                StartCoroutine(AttackCoroutine());
                break;

            case TurnActions.BACK_AWAY:
                MoveAwayFromPlayer();
                GetComponent<Animator>().SetBool("isMoving", true);
                break;

            case TurnActions.NOTHING:
                StartCoroutine(ActionCooldownCoroutine());
                break;
        }
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

    private void MoveTowardsPlayer()
    {
        if (agent != null && closestPlayer != null)
        {
            Vector3 directionToPlayer = (closestPlayer.transform.position - transform.position).normalized;
            Vector3 destination = closestPlayer.transform.position - directionToPlayer * range;
            agent.SetDestination(destination);
            StartCoroutine(WaitForMoveCompletion());
        }
    }

    private void MoveAwayFromPlayer()
    {
        if (agent != null && closestPlayer != null)
        {
            Vector3 directionAway = (transform.position - closestPlayer.transform.position).normalized;
            Vector3 newPosition = transform.position + directionAway * minDistance;
            agent.SetDestination(newPosition);
            StartCoroutine(WaitForMoveCompletion());
        }
    }

    private IEnumerator WaitForMoveCompletion()
    {
        yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);
        StartCoroutine(ActionCooldownCoroutine());
    }

    private void Shoot()
    {
        if (!hasShot)
        {
            Debug.Log("BANG");
            //SoundEffectsManager.instance.PlaySoundFXClip(shootingClips, transform,1f);
            GameObject bullet = Instantiate(bulletShot, transform.position, Quaternion.identity);
            bullet.GetComponent<multiShoot>().setShootDirection((closestPlayerPos - transform.position).normalized,false);
           

            // Register the bullet with the closest player's movement script
            //closestPlayer.RegisterBullet(bulletScript);

            hasShot = true; // Mark that it has shot this turn
        }
    }

    // Reload coroutine to wait until the next GetIsMoving toggle after shooting
    

    private IEnumerator AttackCoroutine()
    {
        fx.SetTrigger("playFX");
        GetComponent<Animator>().SetTrigger("attack");
        yield return new WaitForSeconds(0.3f);

        if (enemyStats.isAlive && closestPlayer.isExecuting)
        {
            closestPlayerPos = closestPlayer.transform.position;
            Shoot();
            fx.ResetTrigger("playFX");
            StartCoroutine(Reload());
        }
    }
    
    private IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(1f);
        if (enemyStats.isAlive)
        {
            isReloading = false;
            hasShot = false;
            StartCoroutine(ActionCooldownCoroutine());
        }
    }
    
    public void DecideAction()
    {
        // Solo decide si no tiene acción pendiente
        if (turnAction != TurnActions.NOTHING) return;

        distanceToPlayer = Vector3.Distance(transform.position, closestPlayerPos);

        if (distanceToPlayer > range)
        {
            turnAction = TurnActions.APPROACH;
        }
        else if (distanceToPlayer < minDistance)
        {
            turnAction = TurnActions.BACK_AWAY;
        }
        else if (!hasShot && !isReloading && distanceToPlayer <= range)
        {
            turnAction = TurnActions.SHOOT;
        }
        else
        {
            turnAction = TurnActions.NOTHING;
        }
    }

    private IEnumerator ActionCooldownCoroutine()
    {
        onCooldown = true;
        yield return new WaitForSeconds(actionCooldown);
        onCooldown = false;
        isPerformingAction = false; // Ahora puede hacer una nueva acción
        turnAction = TurnActions.NOTHING; // Resetea la acción para el siguiente turno
    }
    public void ResetTurnAction()
    {
        turnAction = TurnActions.NOTHING;
    }
}
