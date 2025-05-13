using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.Splines;

public class Enemies : MonoBehaviour
{
    [Header("Stats")]
    public float speed;
    public float range;
    public float preAttack;
    public float timeAttack;
    public float postAttack;
    public bool isAlive;

    [Space(5)]
    [Header("States")]
    public bool isAttacking = false;
    public bool isSleeping = false;

    [Space(5)]
    [Header("Health")]
    [SerializeField] private int health;

    [Space(5)]
    [Header("Player Reference")]
    [SerializeField] private TimeSecuence Player;

    [Space(5)]
    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    [Space(5)]
    [Header("Combat")]
    [SerializeField] private GameObject attack;
    [SerializeField] private GameObject bloodSplash;
    [SerializeField] private AudioClip[] attackClips;
    [SerializeField] private AudioClip[] damageClips;

    [Space(5)]
    [Header("Runtime/Internal")]
    private EnemyBase enemyStats;
    private Vector3 PlayerPos;
    private bool isResting = false;


    void Start()
    {

        enemyStats = GetComponent<EnemyBase>();
        agent = GetComponent<NavMeshAgent>();
        agent.acceleration = 999f;
        agent.angularSpeed = 999f;
        isAlive = true;
    }
    void Update()
    {
        if (enemyStats.isAlive && !isResting) // No moverse si está descansando
        {
            Movment();
            spriteRenderer.flipX = PlayerPos.x < transform.position.x;
        }
        else
        {
            agent.isStopped = true;
        }

    }
    void Movment()
    {

        PlayerPos = Player.transform.position;
        float distanceToPlayer = Vector3.Distance(transform.position, PlayerPos);
        if (Player.GetIsExecuting())
        {
            if (distanceToPlayer <= range)
            {
                StartCoroutine(AttackCoroutine());
                SoundEffectsManager.instance.PlaySoundFXClip(attackClips, transform, 1f);
            }
            animator.SetBool("isMoving", true);

            agent.isStopped = false;
            agent.SetDestination(PlayerPos);
        }
        else
        {
            animator.SetBool("isMoving", false);
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
           
        }
    }

    IEnumerator AttackCoroutine()
    {
        isResting = true; // Comienza el descanso

        GetComponent<Animator>().SetTrigger("preAttack");
        yield return new WaitForSeconds(preAttack);

        GetComponent<Animator>().SetTrigger("attack");

        GameObject _attack = Instantiate(attack, transform.position, Quaternion.identity, transform);

        yield return new WaitForSeconds(timeAttack);

        Destroy(_attack);


        yield return new WaitForSeconds(postAttack);

        isResting = false;
    }

    public void Damage(int val, GameObject hitObject)
    {
      
        health -= val;
        health = Mathf.Max(health, 0);
        Instantiate(bloodSplash, this.transform.position, hitObject.transform.rotation);
        if (health > 0)
        {
            StartCoroutine(StopMovementTemporarily(0.2f));
        }
        else
        {
            animator.SetBool("isDead", true);
            GetComponent<dropWapon>().enabled = true;
            isAlive = false;
            GetComponent<Collider>().enabled = false;
        }   
    }

    private IEnumerator StopMovementTemporarily(float duration)
    {
        agent.isStopped = true;
        animator.SetBool("isMoving", false);
        StartCoroutine(Shake(0.2f, 0.3f));      
        SoundEffectsManager.instance.PlaySoundFXClip(damageClips, transform, 1f);
        yield return new WaitForSeconds(duration);       
        if (Player.GetIsExecuting() && !isResting)
        {
            agent.isStopped = false;
            animator.SetBool("isMoving", true);
        }
    }
    public float GetHealth() { return health; }
    public IEnumerator Shake(float timeLenght, float range)
    {
        UnityEngine.Vector3 originalPosition = this.transform.position;
        float elapsed = 0.0f;
        while (elapsed < timeLenght)
        {
            UnityEngine.Vector3 movingPosition = UnityEngine.Vector3.zero;

            float x = 0;
            float z = 0;

            x = Random.Range(originalPosition.x - 1.0f * range, originalPosition.x + 1.0f * range);
            z = Random.Range(originalPosition.z - 1.0f * (range / 2), originalPosition.z + 1.0f * (range / 2));

            //Debug.Log(originalPosition);
            movingPosition.x = x;
            movingPosition.y = transform.position.y;
            movingPosition.z = z;

            transform.position = movingPosition;

            elapsed += Time.deltaTime;



            yield return null;

        }

        transform.position = originalPosition;

    }
}
