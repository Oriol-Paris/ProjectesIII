using UnityEngine;

public class LaserBullet : BulletPrefab
{
    
    private Vector3 targetPosition;
    private float lifetime = 5f;

    void Start()
    {
        isHit = false;
        speed = 20f;
        damage = FindObjectOfType<PlayerBase>().playerData.gun.damage;
    }

    void Update()
    {
        if (IsPaused()) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        lifetime -= Time.deltaTime;

        if (isHit || Vector3.Distance(transform.position, targetPosition) < 0.1f || lifetime <= 0f)
        {
            DestroyBullet();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Walls"))
        {
            DestroyBullet();
        }
        if (isFromPlayer)
        {
            EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();
            if (enemy != null && enemy.GetHealth() > 0)
            {
                enemy.Damage(damage, collision.gameObject);
                isHit = true;
            }
        }
        else
        {
            PlayerBase player = collision.gameObject.GetComponent<PlayerBase>();
            if (player != null)
            {
                player.Damage(1, collision.gameObject);
                isHit = true;
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