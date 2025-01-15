using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementShooter : MonoBehaviour
{
    public enum TurnActions { APPROACH, SHOOT, BACK_AWAY, NOTHING };
    public TurnActions turnAction;

    [SerializeField] private List<TimeSecuence> players; // Array of player references
    public TimeSecuence closestPlayer;
    private Vector3 closestPlayerPos;
    private float moveTime;
    public EnemyBase enemyStats;
    [SerializeField] private GameObject bulletShot; // Bullet prefab to shoot
    [SerializeField] private float velocity; // Movement speed
    [SerializeField] private float range; // Shooting range
    [SerializeField] private float minDistance = 2f; // Minimum distance before moving back
    [SerializeField] private AudioClip[] shootingClips;
    public Animator fx;
    private bool haveChosenAnAction;
    private bool isReloading = false; // To control the "reload" wait after shooting
    private bool hasShot = false; // To ensure only one shot per turn
    private bool lastIsMovingState = false; // Track the last state of GetIsMoving to detect state change
    float distanceToPlayer;
    void Start()
    {
        enemyStats = GetComponent<EnemyBase>();

        // Get all OG_MovementByMouse objects in the scene and add them to the players list
        TimeSecuence[] playersArray = GameObject.FindObjectsByType<TimeSecuence>(FindObjectsSortMode.None);
        foreach (TimeSecuence player in playersArray)
        {
            players.Add(player);
        }

        velocity = velocity * FindAnyObjectByType<CombatManager>().enemyStatMultiplier;
        range = range * FindAnyObjectByType<CombatManager>().enemyStatMultiplier;

        DecideAction();
    }

    void Update()
    {
        moveTime = Time.deltaTime * velocity;

        if (enemyStats.isAlive && closestPlayer != null)
        {
            if (closestPlayer.isExecuting)
            {
                ExecuteAction();
            } else
            {
                DecideAction();
            }
        }
    }

    // Decides whether to shoot or move based on distance
    private void ExecuteAction()
    {
        switch (turnAction)
        {
            case TurnActions.APPROACH:
                MoveTowardsPlayer();
                this.GetComponent<Animator>().SetBool("isMoving", true);
                break;

            case TurnActions.SHOOT:
                this.GetComponent<Animator>().SetBool("isMoving", false);
                StartCoroutine(AttackCoroutine());
                break;

            case TurnActions.BACK_AWAY:
                MoveAwayFromPlayer();
                this.GetComponent<Animator>().SetBool("isMoving", true);
                break;
        }
    }

    // Finds the closest player in the players array
    private void FindClosestPlayer()
    {
        float closestDistance = 100;

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

    // Moves towards the closest player
    private void MoveTowardsPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, closestPlayerPos, moveTime);
    }

    // Moves away from the closest player
    private void MoveAwayFromPlayer()
    {
        Vector3 directionAway = (transform.position - closestPlayerPos).normalized;
        transform.position = Vector3.MoveTowards(transform.position, transform.position + directionAway, moveTime);
    }

    // Shoots a bullet towards the closest player
    private void Shoot()
    {
        if(!hasShot)
        {
            Debug.Log("BANG");
            //SoundEffectsManager.instance.PlaySoundFXClip(shootingClips, transform,1f);
            GameObject bullet = Instantiate(bulletShot, transform.position, Quaternion.identity);
            GunBullet bulletScript = bullet.GetComponent<GunBullet>();
            bulletScript.isFromPlayer = false;
            bulletScript.Shoot((closestPlayerPos - transform.position).normalized); // Set bullet direction

            // Register the bullet with the closest player's movement script
            //closestPlayer.RegisterBullet(bulletScript);

            hasShot = true; // Mark that it has shot this turn
        }
    }

    // Reload coroutine to wait until the next GetIsMoving toggle after shooting
    private IEnumerator Reload()
    {
        //Debug.Log("RELOAD");
        isReloading = true;
        yield return new WaitUntil(() => closestPlayer.GetIsExecuting() == false);
        yield return new WaitUntil(() => closestPlayer.GetIsExecuting() == true);
        yield return new WaitUntil(() => closestPlayer.GetIsExecuting() == false);
        yield return new WaitUntil(() => closestPlayer.GetIsExecuting() == false);
        
        isReloading = false;
        hasShot = false; // Reset shooting state for the next turn
    }

    private IEnumerator AttackCoroutine()
    {
        fx.SetTrigger("playFX");
        this.GetComponent<Animator>().SetTrigger("attack");
        yield return new WaitForSeconds(0.3f);
        if(closestPlayer.isExecuting) { 
        closestPlayerPos = closestPlayer.transform.position;

            Shoot();

            fx.ResetTrigger("playFX");
    
            StartCoroutine(Reload());
        } else
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
            Debug.Log("MOVING");
            // Move towards the player if out of range
            turnAction = TurnActions.APPROACH;
        }
        else if (distanceToPlayer < minDistance)
        {
            Debug.Log("MOVING");
            // Move away from the player if too close
            turnAction = TurnActions.BACK_AWAY;    
        }
        else if (!hasShot && !isReloading) // Shoot only once per turn
        {

            Debug.Log("PIUM");
            //In range, shoot
            turnAction = TurnActions.SHOOT;
        }
        else
        {
            Debug.Log("NOTHING");
            turnAction = TurnActions.NOTHING;
        }
    }

    public void ResetTurnAction()
    {
        turnAction = TurnActions.NOTHING;
    }
}
