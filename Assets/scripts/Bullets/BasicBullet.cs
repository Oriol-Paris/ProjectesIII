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
        // Destruir la bala despu�s de bulletLifetime segundos
        Destroy(gameObject, bulletLifetime);
    }

    void Update()
    {
        // Mover la bala en la direcci�n establecida
        transform.position += direction * bulletVelocity * Time.deltaTime;
    }

    public void SetDirection(Vector3 targetPosition)
    {
        // Ignorar altura (ajustar targetPosition.y al nivel de la bala)
        targetPosition.y = transform.position.y;

        // Calcular direcci�n normalizada hacia el objetivo
        direction = (targetPosition - transform.position).normalized;
    }
}
