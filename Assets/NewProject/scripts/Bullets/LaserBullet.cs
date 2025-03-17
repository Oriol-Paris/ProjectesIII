using UnityEngine;
using UnityEngine.UIElements;

public class LaserBullet : MonoBehaviour
{

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform laserOrigin;
    [SerializeField] private Vector3 shootDirection;
    [SerializeField] private LayerMask obstacleLayers;
    [SerializeField] private LayerMask damageableLayers;
    [SerializeField] private float maxLaserDistance = 100f;
    [SerializeField] private int laserDamage;

    private Vector3 laserEndPoint;
    private void Start()
    {
        laserDamage = FindAnyObjectByType<BulletCollection>().GetBullet(BulletType.GUN).damage;
    }
    void Update()
    {
        ShootLaser();
    }

    private void ShootLaser()
    {
        RaycastHit hit;
       

        // Detectar colisi�n con un muro o enemigo
        if (Physics.Raycast(laserOrigin.position, shootDirection, out hit, maxLaserDistance, obstacleLayers | damageableLayers))
        {
            laserEndPoint = hit.point;  // Detener el l�ser en el punto de impacto

            // Si el objeto impactado es da�able, aplicar da�o
            if (((1 << hit.collider.gameObject.layer) & damageableLayers) != 0)
            {
                if (hit.collider.TryGetComponent(out EnemyBase enemy))
                {
                    enemy.Damage(laserDamage, hit.collider.gameObject);
                }
                else if (hit.collider.TryGetComponent(out PlayerBase player))
                {
                    player.Damage(laserDamage, hit.collider.gameObject);
                }
            }
        }
        else
        {
            // Si no choca con nada, el l�ser llega hasta la distancia m�xima
            laserEndPoint = laserOrigin.position + (shootDirection * maxLaserDistance);
        }

        // Actualizar LineRenderer para que se vea el l�ser
        UpdateLaserVisuals();
    }

    public void setShootDirection(Vector3 _shootDirection)
    {
        shootDirection = _shootDirection;
    }

    private void UpdateLaserVisuals()
    {
        lineRenderer.SetPosition(0, laserOrigin.position);
        lineRenderer.SetPosition(1, laserEndPoint);
    }
}