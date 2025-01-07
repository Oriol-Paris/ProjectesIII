using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private float range;
    [SerializeField] private float oldRange;//Esto para clase shooter
    [SerializeField] private float shootingRange;//Esto para cuando hagamos clase shooter
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Collider2D collider;
    [SerializeField] Rigidbody2D rb2d;
    private bool isMoving;
    private bool isShoooting;
    public bool isAlive;
    [SerializeField]AudioClip[] damageClips; 
    // Start is called before the first frame update
    void Start()
    {
        isAlive = true;
        isMoving = true;
        isShoooting = false;
        oldRange = range;
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        rb2d = GetComponent<Rigidbody2D>(); 
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
    public void Damage(int val)
    {
        health -= val;
        health = Mathf.Max(health, 0); // Ensure health doesn't go below 0
        SoundEffectsManager.instance.PlaySoundFXClip(damageClips, transform, 1f);
    }

    public float GetRange() { return range; }
    public void SetRange(float newRange) { range = newRange; }
    public float GetOldRange() { return oldRange; }
    public bool GetIsMoving() { return isMoving; }
    public bool GetIsShoooting() { return isShoooting; }
}
