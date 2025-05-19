using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Timers;
using TMPro;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private float range;
    [SerializeField] private float oldRange;//Esto para clase shooter
    [SerializeField] private float shootingRange;//Esto para cuando hagamos clase shooter
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Collider enemyCollider;
    [SerializeField] Rigidbody rb2d;
    [SerializeField] Animator animator;
    [SerializeField] GameObject bloodSplash;
    [SerializeField] dropWapon dropWeapon;

    private UnityEngine.Vector3 originalPosition;
    private bool isMoving;
    private bool isShoooting;
    public bool isAlive;
    [SerializeField]AudioClip[] damageClips;
    
    private float hitFeedBackTime = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Color.white);
        Debug.Log(Color.red);
        isAlive = true;
        isMoving = true;
        isShoooting = false;
        oldRange = range;
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<Collider>();
        rb2d = GetComponent<Rigidbody>();

        health *= (int)PlayerPrefs.GetFloat("DifficultyMultiplier");
        //health *= FindAnyObjectByType<CombatManager>().enemyStatMultiplier;

    }

    // Update is called once per frame
    void Update()
    {

        

    }


    public float GetHealth() { return health; }
    public void Damage(int val, GameObject hitObject)
    {
        originalPosition = this.transform.position;
        GetComponent<SpriteRenderer>().color = Color.red;
        Debug.Log(hitObject.name);
        StartCoroutine(Shake(0.2f, 0.3f));
        StartCoroutine(whitecolor());
        Instantiate(bloodSplash, this.transform.position, hitObject.transform.rotation);
        SoundEffectsManager.instance.PlaySoundFXClip(damageClips, transform, 1f);
        health -= val;
        health = Mathf.Max(health, 0); // Ensure health doesn't go below 0
        if (health <= 0)
        {
            animator.SetBool("isDead", true);
            //dropWeapon.DropWeapon();
            GetComponent<dropWapon>().enabled = true;
            isAlive = false;
            spriteRenderer.color = Color.grey;
            GetComponent<Collider>().enabled = false;
            rb2d.useGravity = false;
            rb2d.detectCollisions = false;
          

        }


    }

    IEnumerator whitecolor()
    {

        

        float elapsed = 0;
        while (elapsed < hitFeedBackTime)
        {
            yield return null;
            GetComponent<SpriteRenderer>().color = new Color(1, GetComponent<SpriteRenderer>().color.b + Time.deltaTime, GetComponent<SpriteRenderer>().color.b + Time.deltaTime);


            elapsed += Time.deltaTime;
        }
        GetComponent<SpriteRenderer>().color = Color.white;
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

    public float GetRange() { return range; }
    public void SetRange(float newRange) { range = newRange; }
    public float GetOldRange() { return oldRange; }
    public bool GetIsMoving() { return isMoving; }
    public bool GetIsShoooting() { return isShoooting; }
}
