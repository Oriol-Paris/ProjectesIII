using UnityEngine;

public class GunBullet : BulletPrefab
{
    private Vector3 targetPosition; // Target position the bullet is moving towards
    //private float lifetime = 5f; // Lifetime in seconds before auto-destruction

    void Start()
    {
        isHit = false;
        speed = 10f;
        if(isFromPlayer) {
            for (int i = 0; i < playerData.availableActions.Count; i++)
            {
                if (playerData.availableActions[i].style.prefab == playerData.gun.prefab)
                {
                    damage = playerData.availableActions[i].style.damage; break;
                }
            }
        }
    }

    void Update()
    {
        if (IsPaused()) return;

        // Move towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Decrease lifetime over time
        //lifetime -= Time.deltaTime;

        // Check if the bullet has reached its destination, hit something, or if its lifetime has expired
        if (isHit || Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            DestroyBullet();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
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
                enemy.Damage(damage);
                isHit = true;
            }
        }
        else
        {
            PlayerBase player = collision.gameObject.GetComponent<PlayerBase>();
            if (player != null)
            {
                player.Damage(FindAnyObjectByType<CombatManager>().enemyStatMultiplier >= 1.5f? 2 : 1);
                isHit = true;
            }
        }
    }

    public override void Shoot(Vector3 direction)
    {
        // Store the original direction and target position
        originalDirection = direction;
        targetPosition = transform.position + direction.normalized * 20f; 
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
