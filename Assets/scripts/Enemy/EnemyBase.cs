using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.Splines;

public class EnemyBase : MonoBehaviour
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

    [Header("Vision")]
    public bool NeedToSeePlayer = false;
  

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
    private bool godMode = false;
    


    void Start()
    {
        speed *= PlayerPrefs.GetFloat("DifficultyMultiplier");
        health *= (int)PlayerPrefs.GetFloat("DifficultyMultiplier");

        enemyStats = GetComponent<EnemyBase>();
        agent = GetComponent<NavMeshAgent>();
        agent.acceleration = 999f;
        agent.angularSpeed = 999f;
        agent.updateRotation = false;
        isAlive = true;
        agent.speed = speed;
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

        if(Input.GetKeyDown(KeyCode.G))
        {
            godMode = !godMode;

        }

    }
    void Movment()
    {

        PlayerPos = Player.transform.position;
        float distanceToPlayer = Vector3.Distance(transform.position, PlayerPos);
        if (Player.GetIsExecuting())
        {
            if (NeedToSeePlayer)
            {
                Vector3 origin = transform.position + Vector3.up * 1f;
                Vector3 direction = (PlayerPos - origin).normalized;
                float distance = Vector3.Distance(origin, PlayerPos);

                if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
                {
                   
                    if (hit.collider.CompareTag("envairoment"))
                    {
                      
                        animator.SetBool("isMoving", true);
                        agent.isStopped = false;
                        agent.SetDestination(PlayerPos);
                        return;
                    }
                }
            }

           
            if (distanceToPlayer <= range && !isAttacking && !isResting)
            {
                StartCoroutine(AttackCoroutine());
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
        isResting = true;
        yield return new WaitUntil(() => Player.GetIsExecuting());


        GetComponent<Animator>().SetTrigger("preAttack");
        yield return new WaitForSeconds(preAttack);

        GetComponent<Animator>().SetTrigger("attack");

        GameObject _attack = Instantiate(attack, transform.position, Quaternion.identity, transform);

        if (_attack.GetComponent<multiShoot>() != null)
        {
            _attack.GetComponent<multiShoot>().setShootDirection((Player.transform.position - transform.position).normalized, false);
        }
     


            yield return new WaitForSeconds(timeAttack);

        //Destroy(_attack);


        yield return new WaitForSeconds(postAttack);

        isResting = false;
    }

    public void Damage(int val, GameObject hitObject)
    {

        health -= val;
        health = Mathf.Max(health, 0);
        Instantiate(bloodSplash, this.transform.position, hitObject.transform.rotation);

        if(godMode)
        { 
         health -= 100;
         health = Mathf.Max(health, 0);
        }

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











//    [SerializeField] private int health;
//    [SerializeField] private float range;
//    [SerializeField] private float oldRange;//Esto para clase shooter
//    [SerializeField] private float shootingRange;//Esto para cuando hagamos clase shooter
//    [SerializeField] SpriteRenderer spriteRenderer;
//    [SerializeField] Collider enemyCollider;
//    [SerializeField] Rigidbody rb2d;
//    [SerializeField] Animator animator;
//    [SerializeField] GameObject bloodSplash;
//    [SerializeField] dropWapon dropWeapon;

//    private UnityEngine.Vector3 originalPosition;
//    private bool isMoving;
//    private bool isShoooting;
//    public bool isAlive;
//    [SerializeField]AudioClip[] damageClips;

//    private float hitFeedBackTime = 0.3f;
//    // Start is called before the first frame update
//    void Start()
//    {
//        Debug.Log(Color.white);
//        Debug.Log(Color.red);
//        isAlive = true;
//        isMoving = true;
//        isShoooting = false;
//        oldRange = range;
//        spriteRenderer = GetComponent<SpriteRenderer>();
//        enemyCollider = GetComponent<Collider>();
//        rb2d = GetComponent<Rigidbody>();

//        health *= (int)PlayerPrefs.GetFloat("DifficultyMultiplier");
//        //health *= FindAnyObjectByType<CombatManager>().enemyStatMultiplier;

//    }

//    // Update is called once per frame
//    void Update()
//    {



//    }


//    public float GetHealth() { return health; }
//    public void Damage(int val, GameObject hitObject)
//    {
//        originalPosition = this.transform.position;
//        GetComponent<SpriteRenderer>().color = Color.red;
//        Debug.Log(hitObject.name);
//        StartCoroutine(Shake(0.2f, 0.3f));
//        StartCoroutine(whitecolor());
//        Instantiate(bloodSplash, this.transform.position, hitObject.transform.rotation);
//        SoundEffectsManager.instance.PlaySoundFXClip(damageClips, transform, 1f);
//        health -= val;
//        health = Mathf.Max(health, 0); // Ensure health doesn't go below 0
//        if (health <= 0)
//        {
//            animator.SetBool("isDead", true);
//            //dropWeapon.DropWeapon();
//            GetComponent<dropWapon>().enabled = true;
//            isAlive = false;
//            spriteRenderer.color = Color.grey;
//            GetComponent<Collider>().enabled = false;
//            rb2d.useGravity = false;
//            rb2d.detectCollisions = false;


//        }


//    }

//    IEnumerator whitecolor()
//    {



//        float elapsed = 0;
//        while (elapsed < hitFeedBackTime)
//        {
//            yield return null;
//            GetComponent<SpriteRenderer>().color = new Color(1, GetComponent<SpriteRenderer>().color.b + Time.deltaTime, GetComponent<SpriteRenderer>().color.b + Time.deltaTime);


//            elapsed += Time.deltaTime;
//        }
//        GetComponent<SpriteRenderer>().color = Color.white;
//    }

//    public IEnumerator Shake(float timeLenght, float range)
//    {
//        float elapsed = 0.0f;
//        while (elapsed < timeLenght)
//        {
//            UnityEngine.Vector3 movingPosition = UnityEngine.Vector3.zero;

//            float x = 0;
//            float z = 0;

//            x = Random.Range(originalPosition.x - 1.0f * range, originalPosition.x + 1.0f * range);
//            z = Random.Range(originalPosition.z - 1.0f * (range / 2), originalPosition.z + 1.0f * (range / 2));

//            //Debug.Log(originalPosition);
//            movingPosition.x = x;
//            movingPosition.y = transform.position.y;
//            movingPosition.z = z;

//            transform.position = movingPosition;

//            elapsed += Time.deltaTime;



//            yield return null;

//        }

//        transform.position = originalPosition;

//    }


//}
