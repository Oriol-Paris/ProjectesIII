using UnityEngine;

public class Shotgun : BulletPrefab
{
    private float lifetime = 0.5f;
    private Vector3 targetPosition;
    private Vector3 offset;

    void Start()
    {
        isHit = false;
        speed = 10f;
        for (int i = 0; i < playerData.availableActions.Count; i++)
        {
            if (playerData.availableActions[i].style.prefab == playerData.shotgun.prefab)
            {
                damage = playerData.availableActions[i].style.damage; break;
            }
        }
    }

    void Update()
    {
        if (IsPaused()) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (isHit || Vector3.Distance(transform.position, targetPosition) <= 0.1f)
        {
            DestroyBullet();
        }
        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
        {
            DestroyBullet();
        }
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
                enemy.Damage(damage);
                isHit = true;
                DestroyBullet();
            }
        }
        else
        {
            PlayerBase player = collision.gameObject.GetComponent<PlayerBase>();
            if (player != null)
            {
                player.Damage();
                isHit = true;
                DestroyBullet();
            }
        }
    }

    public override void Shoot(Vector3 direction)
    {
        originalDirection = direction;
        targetPosition = transform.position + direction.normalized * 20f;
    }

    public void Shoot(Vector3 direction, Vector3 offset)
    {
        this.offset = offset;
        originalDirection = direction + offset;
        targetPosition = transform.position + (direction + offset).normalized * 20f;
    }
}
