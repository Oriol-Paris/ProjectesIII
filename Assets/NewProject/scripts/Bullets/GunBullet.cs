using UnityEngine;

public class GunBullet : BulletPrefab
{
    private Vector3 direction; // Direction the bullet is moving towards

    void Start()
    {
        isHit = false;
        speed = 10f;
        if (isFromPlayer)
        {
            for (int i = 0; i < playerData.availableActions.Count; i++)
            {
                if (playerData.availableActions[i].style.prefab == playerData.bulletCollection.GetBullet(BulletType.GUN).prefab)
                {
                    damage = playerData.availableActions[i].style.damage; break;
                }
            }
        }

        // Destroy the bullet after a certain time to prevent it from existing indefinitely
       // Destroy(gameObject, 5f);
    }

    void Update()
    {
        if (IsPaused()) return;

        // Move the bullet in the set direction
        transform.position += direction * speed * Time.deltaTime;
    }
    
    private void OnTriggerEnter(Collider collision)
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
                DestroyBullet();
            }
        }
        else
        {
            PlayerBase player = collision.gameObject.GetComponent<PlayerBase>();
            if (player != null)
            {
                player.Damage(FindAnyObjectByType<CombatManager>().enemyStatMultiplier >= 1.5f ? 2 : 1, this.gameObject); //Check if sending correct value (Suposed to be sending Bullet GObject)
                isHit = true;
                DestroyBullet();
            }
        }
    }

    public override void Shoot(Vector3 direction)
    {
        // Store the direction
        this.direction = direction.normalized;
    }

    private void DestroyBullet()
    {
      //  Destroy(gameObject);
    }
}
