using UnityEngine;


public class LaserBullet : MonoBehaviour, IBulletBehavior
{

    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private GameObject hitboxVisual; // Un prefab tipo cubo con scale (1,1,1)

    private Vector3 shootDirection;

   
   
   

    private void CreateLaserHitbox(Vector3 start, Vector3 end)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;

        Vector3 midPoint = start + direction / 2f;

        GameObject hitbox = Instantiate(hitboxVisual, midPoint, Quaternion.LookRotation(direction));
        hitbox.transform.localScale = new Vector3(0.2f, 0.2f, distance); // Z se extiende en la dirección
    }
    public void setShootDirection(Vector3 _shootDirection, bool itsFromPlayer)
    {
        
        shootDirection = new Vector3(_shootDirection.x, 0f, _shootDirection.z).normalized;

        Ray ray = new Ray(transform.position, shootDirection);
        RaycastHit hit;

        Vector3 endPoint = transform.position + shootDirection * maxDistance;

        if (Physics.Raycast(ray, out hit, maxDistance, hitLayers))
        {
            if (hit.collider.CompareTag("envairoment"))
            {
                endPoint = hit.point;

             
                endPoint.y = transform.position.y;
            }
        }

        CreateLaserHitbox(transform.position, endPoint);
    }
}