using UnityEngine;

public class BasicBullet : MonoBehaviour
{
    [SerializeField]
    private float bulletVelocity = 5f;

    [SerializeField]
    private float bulletLifetime = 5f; // Tiempo de vida antes de autodestruirse

    private Vector3 direction;

    void Start()
    {
        // Destruir la bala después de bulletLifetime segundos
        Destroy(gameObject, bulletLifetime);
    }

    void Update()
    {
        // Mover la bala en la dirección establecida
        transform.position += direction * bulletVelocity * Time.deltaTime;
    }

    public void SetDirection(Vector3 targetPosition)
    {
        // Ignorar altura (ajustar targetPosition.y al nivel de la bala)
        targetPosition.y = transform.position.y;

        // Calcular dirección normalizada hacia el objetivo
        direction = (targetPosition - transform.position).normalized;
    }
}
