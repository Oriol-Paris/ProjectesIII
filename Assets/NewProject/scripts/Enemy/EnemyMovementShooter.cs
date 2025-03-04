using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // Necesario para usar NavMesh

public class EnemyMovementShooter : MonoBehaviour
{
    public enum TurnActions { APPROACH, SHOOT, BACK_AWAY, NOTHING };
    public TurnActions turnAction;

    [SerializeField] private List<TimeSecuence> players;
    public TimeSecuence closestPlayer;
    private Vector3 closestPlayerPos;
    private float moveTime;
    public EnemyBase enemyStats;
    [SerializeField] private GameObject bulletShot;
    [SerializeField] private float velocity;
    [SerializeField] private float range;
    [SerializeField] private float minDistance = 2f;
    [SerializeField] private AudioClip[] shootingClips;
    public Animator fx;
    private bool onCooldown;
    private bool isReloading = false;
    private bool hasShot = false;
    private bool lastIsMovingState = false;
    float distanceToPlayer;

    private NavMeshAgent agent; // Referencia al NavMeshAgent

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
            if (closestPlayer.isExecuting)
            {
                ExecuteAction();
            }
            else
            {
                DecideAction();
                agent.isStopped = true;
            }
        }
    }

    private void ExecuteAction()
    {
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
            agent.SetDestination(closestPlayer.transform.position); // Usar NavMeshAgent para moverse
        }
    }

    private void MoveAwayFromPlayer()
    {
        if (agent != null && closestPlayer != null)
        {
            Vector3 directionAway = (transform.position - closestPlayerPos).normalized;
            Vector3 newPosition = transform.position + directionAway * minDistance; // Alejarse
            agent.SetDestination(newPosition);
        }
    }

    private void Shoot()
    {
        if (!hasShot)
        {
            Debug.Log("BANG");
            GameObject bullet = Instantiate(bulletShot, transform.position, Quaternion.identity);
            bullet.GetComponent<DestroyBullet>().setShootDirection((closestPlayerPos - transform.position).normalized);
            hasShot = true;
            onCooldown = true;
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitUntil(() => closestPlayer.GetIsExecuting() == false);
        yield return new WaitUntil(() => closestPlayer.GetIsExecuting() == true);
        yield return new WaitUntil(() => closestPlayer.GetIsExecuting() == false);
        isReloading = false;
        hasShot = false;
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
            yield return new WaitUntil(() => closestPlayer.GetIsExecuting() == true);
            yield return new WaitForSeconds(3);
            Debug.Log("Wait ended");
            onCooldown = false;
        }
        else
        {
            
            DecideAction();
        }
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
        else if (!hasShot && !isReloading)
        {
            turnAction = TurnActions.SHOOT;
        }
        else
        {
            turnAction = TurnActions.NOTHING;
        }
    }

    public void ResetTurnAction()
    {
        turnAction = TurnActions.NOTHING;
    }
}
