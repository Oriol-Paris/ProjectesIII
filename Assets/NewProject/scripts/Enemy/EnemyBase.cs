using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Timers;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private float range;
    [SerializeField] private float oldRange;//Esto para clase shooter
    [SerializeField] private float shootingRange;//Esto para cuando hagamos clase shooter
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Collider collider;
    [SerializeField] Rigidbody rb2d;

    [SerializeField] GameObject bloodSplash;
    
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
        collider = GetComponent<Collider>();
        rb2d = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        if (health <= 0)
        {
            this.GetComponent<Animator>().SetBool("isDead", true);
            isAlive = false;
            spriteRenderer.color = Color.grey;
            collider.enabled = false;
            

        }

    }


    public int GetHealth() { return health; }
    public void Damage(int val, GameObject hitObject)
    {
        
        GetComponent<SpriteRenderer>().color = Color.red;
        StartCoroutine(whitecolor());
        Instantiate(bloodSplash, this.transform.position, hitObject.transform.rotation);
        SoundEffectsManager.instance.PlaySoundFXClip(damageClips, transform, 1f);
        health -= val;
        health = Mathf.Max(health, 0); // Ensure health doesn't go below 0
        
       
        
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

    public float GetRange() { return range; }
    public void SetRange(float newRange) { range = newRange; }
    public float GetOldRange() { return oldRange; }
    public bool GetIsMoving() { return isMoving; }
    public bool GetIsShoooting() { return isShoooting; }
}
