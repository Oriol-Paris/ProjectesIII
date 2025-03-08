using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI; // Necesario para usar NavMesh

public class EnemyMovementShooter : MonoBehaviour
{
    public enum TurnActions { APPROACH, SHOOT, BACK_AWAY, NOTHING };
    public TurnActions turnAction;

    [SerializeField] private List<TimeSecuence> players;
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
    float distanceToPlayer;

    private NavMeshAgent agent; // Referencia al NavMeshAgent

    [SerializeField] private float actionCooldown = 1f; // Cooldown duration

    void Start()
    {
        enemyStats = GetComponent<EnemyBase>();
        agent = GetComponent<NavMeshAgent>(); // Obtener el agente de navegación
        agent.speed = velocity; // Ajustar la velocidad

        TimeSecuence[] playersArray = GameObject.FindObjectsByType<TimeSecuence>(FindObjectsSortMode.None);
        foreach (TimeSecuence player in playersArray)
        {
            players.Add(player);
        }

        velocity *= FindAnyObjectByType<CombatManager>().enemyStatMultiplier;
        range *= FindAnyObjectByType<CombatManager>().enemyStatMultiplier;
        agent.updateRotation = false;

        DecideAction();
    }

    void Update()
    {
        if (enemyStats.isAlive && closestPlayer != null)
        {
            if (closestPlayer.isExecuting && !onCooldown)
            {
                Debug.Log(onCooldown);
                ExecuteAction();
            }
            else
            {
                Debug.Log("OnCooldown");
                agent.isStopped = true;
                return;
            }
        }
    }

    private void ExecuteAction()
    {
        Debug.Log(onCooldown);
        if (onCooldown) return; // Evita que el enemigo actúe mientras está en cooldown.

        switch (turnAction)
        {
            case TurnActions.APPROACH:
                MoveTowardsPlayer();
                agent.isStopped = false;
                GetComponent<Animator>().SetBool("isMoving", true);
                break;

            case TurnActions.SHOOT:
                GetComponent<Animator>().SetBool("isMoving", false);
                StartCoroutine(AttackCoroutine());
                agent.isStopped = true;
                break;

            case TurnActions.BACK_AWAY:
                MoveAwayFromPlayer();
                GetComponent<Animator>().SetBool("isMoving", true);
                agent.isStopped = true;
                break;

            case TurnActions.NOTHING:
                agent.isStopped = true;
                StartCoroutine(ActionCooldownCoroutine());
                break;
        }
    }

    private void FindClosestPlayer()
    {
        float closestDistance = float.MaxValue;

        foreach (var player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
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
            
                GameObject bullet = Instantiate(bulletShot, transform.position, Quaternion.identity);
                bullet.GetComponent<DestroyBullet>().setShootDirection((closestPlayerPos - transform.position).normalized);
                
            
            hasShot = true;
            //StartCoroutine(ActionCooldownCoroutine()); // Iniciar cooldown después de disparar
        }
    }

    private IEnumerator AttackCoroutine()
    {
        fx.SetTrigger("playFX");
        GetComponent<Animator>().SetTrigger("attack");
        yield return new WaitForSeconds(0.3f);

        if (closestPlayer.isExecuting)
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
        isReloading = false;
        hasShot = false;
        StartCoroutine(ActionCooldownCoroutine()); // Call ActionCooldownCoroutine here
    }

    public void DecideAction()
    {
        FindClosestPlayer();
        distanceToPlayer = Vector3.Distance(transform.position, closestPlayerPos);

        if (distanceToPlayer > range)
        {
            turnAction = TurnActions.APPROACH;
        }
        else if (distanceToPlayer < minDistance)
        {
            turnAction = TurnActions.BACK_AWAY;
        }
        else if (!hasShot && !isReloading && distanceToPlayer==range)
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
        turnAction = TurnActions.NOTHING;
        onCooldown = true;
        float countdown = actionCooldown;
        ExecuteAction();

        while (countdown > 0)
        {
            yield return new WaitUntil(() => closestPlayer.isExecuting);

            while (closestPlayer.isExecuting && countdown > 0)
            {
                countdown -= Time.deltaTime;
                Debug.Log("Cooldown countdown: " + countdown);
                yield return null;
            }
        }

        onCooldown = false;
        Debug.Log("Cooldown ended, deciding next action.");
        DecideAction();
    }

    public void ResetTurnAction()
    {
        turnAction = TurnActions.NOTHING;
    }
}
