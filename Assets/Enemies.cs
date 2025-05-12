using UnityEngine;
using UnityEngine.AI;

public class Enemies : MonoBehaviour
{
    public float speed;
    public float rango;
    public float preAttack;
    public float timeAttack;
    public float postAttack;
    public bool isAttacking = false; 
    public bool isSleeping = false;


    [SerializeField] private TimeSecuence Player;
    private Vector3 PlayerPos;
    private EnemyBase enemyStats;
    [SerializeField] private float velocity;
    [SerializeField] private float range;
    [SerializeField] private float restTime = 1.5f; // Tiempo de descanso tras un ataque

    [SerializeField] private NavMeshAgent agent; // Referencia al NavMeshAgent
    [SerializeField] private GameObject attack;
    private SpriteRenderer spriteRenderer; // Referencia al SpriteRenderer
    private bool isResting = false; // Indica si el enemigo está en descanso
    [SerializeField] private float stopDistance = 1.0f; // Distance to stop from the player

    [SerializeField] AudioClip[] attackClips;



    void Start()
    {
        Player = FindAnyObjectByType<TimeSecuence>();
        enemyStats = GetComponent<EnemyBase>();
        agent = GetComponent<NavMeshAgent>(); // Obtener el NavMeshAgent
        agent.speed = velocity * PlayerPrefs.GetFloat("DifficultyMultiplier");
        range *= PlayerPrefs.GetFloat("DifficultyMultiplier");
        agent.updateRotation = false;

        spriteRenderer = GetComponent<SpriteRenderer>(); // Obtener el componente SpriteRenderer
    }

   
    void Update()
    {
        if (enemyStats.isAlive) // No moverse si está descansando
        {
            PlayerPos = Player.transform.position;
            float distanceToPlayer = Vector3.Distance(transform.position, PlayerPos);

            // Voltear sprite dependiendo de la posición del jugador
            spriteRenderer.flipX = PlayerPos.x < transform.position.x;



            if (Player.GetIsExecuting() || Player.GetComponent<PlayerBase>().GetInAction())
            {
                if (distanceToPlayer <= range)
                {
                    Attack();
                }
                agent.isStopped = false;
                GetComponent<Animator>().SetBool("isMoving", true);

                // Calculate the destination with an offset
                Vector3 directionToPlayer = (PlayerPos - transform.position).normalized;
                Vector3 destination = PlayerPos - directionToPlayer * stopDistance;
                agent.SetDestination(destination); // Usar NavMeshAgent para moverse hacia el jugador
            }
            else
            {

                GetComponent<Animator>().SetBool("isMoving", false);
                agent.isStopped = true;
                agent.velocity = Vector3.zero;

            }
        }
        else
        {
            agent.speed = 0;
        }
    }

    IEnumerator AttackCoroutine()
    {
        isResting = true; // Comienza el descanso
        Debug.Log("Attacking");
        GetComponent<Animator>().SetTrigger("attack");

        GameObject _attack = Instantiate(attack, transform.position, Quaternion.identity, transform);
        yield return new WaitForSeconds(0.5f);

        Destroy(_attack);

        yield return new WaitForSeconds(restTime);

        isResting = false;
    }

    public void Attack()
    {
        if (!isResting) // Evita que ataque si aún está descansando
        {
            StartCoroutine(AttackCoroutine());
            SoundEffectsManager.instance.PlaySoundFXClip(attackClips, transform, 1f);
        }
    }


    public IEnumerator Shake(float timeLenght, float range)
    {
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
