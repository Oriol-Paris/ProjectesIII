using UnityEngine;


public class LaserBullet : MonoBehaviour, IBulletBehavior
{
    public float sideDeviationAmount = 2f;
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private GameObject hitboxVisual; // Un prefab tipo cubo con scale (1,1,1)
    public GameObject flash;
    private Vector3 shootDirection;

   
   void Start()
    {
        Instantiate(flash, this.transform.position + (shootDirection.normalized * 0.5f), Quaternion.LookRotation(shootDirection));
    }
   

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

        // Normalizar y mantener el disparo en el plano horizontal
        shootDirection = new Vector3(_shootDirection.x, 0f, _shootDirection.z).normalized;

        // Calcular un vector perpendicular a shootDirection en el plano XZ
        Vector3 sideOffset = Vector3.Cross(shootDirection, Vector3.up).normalized;

        // Elegir aleatoriamente izquierda o derecha
        int randomSide = Random.value < 0.5f ? -1 : 1;

        // Aplicar desviación lateral fija
        Vector3 deviatedDirection = (shootDirection + sideOffset * randomSide * sideDeviationAmount).normalized;

        // Generar raycast desde posición actual con la dirección desviada
        Ray ray = new Ray(transform.position, deviatedDirection);
        RaycastHit hit;

        Vector3 endPoint = transform.position + deviatedDirection * maxDistance;

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