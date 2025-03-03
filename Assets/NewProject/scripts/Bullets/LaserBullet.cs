using UnityEngine;

public class LaserBullet : BulletPrefab
{
    
    private Vector3 targetPosition;
    private float lifetime = 5f;
    [SerializeField] private bool fromPlayer;

    void Start()
    {
        isHit = false;
        speed = 20f;
        damage = FindObjectOfType<PlayerBase>().playerData.gun.damage;
    }

    void Update()
    {
      
    }

    private void OnCollisionEnter(Collision collision)
    {
      

        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Walls"))
            {
                Destroy(gameObject);
            }

            if (fromPlayer && collision.gameObject.GetComponent<EnemyBase>() != null)
            {
                EnemyBase hit = collision.gameObject.GetComponent<EnemyBase>();
                hit.Damage(damage, collision.gameObject);
                Destroy(gameObject);
            }

            if (!fromPlayer && collision.gameObject.GetComponent<PlayerBase>() != null)
            {
                PlayerBase hit = collision.gameObject.GetComponent<PlayerBase>();
                hit.Damage(damage, collision.gameObject);
                Destroy(gameObject);
            }
        }
    }

    public override void Shoot(Vector3 direction)
    {
        // Store the original direction and target position
        originalDirection = direction;
        targetPosition = transform.position + direction.normalized * 30f; // Laser has a longer range
    }
}