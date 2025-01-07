using UnityEngine;
using System.Collections.Generic;

public class shootPlayer : MonoBehaviour
{
    private List<Vector3> listOfShoot = new List<Vector3>();
    private List<GameObject> visualPlayerAfterShoot = new List<GameObject>();

    public GameObject preShoot; // Prefab para previsualización del disparo
    public GameObject bullet;   // Prefab de la bala
    private OG_MovementByMouse movementScript;

    void Start()
    {
        movementScript = GetComponent<OG_MovementByMouse>();
    }

    public void PreShoot(Vector3 lastPosition)
    {
        Vector3 targetPosition = GetMouseTargetPosition(); // Obtener posición con Raycast
        if (targetPosition != Vector3.zero)
        {
            listOfShoot.Add(targetPosition);

            // Crear marcador visual (opcional)
            GameObject instantiatedObject = Instantiate(preShoot, lastPosition, Quaternion.identity);
            visualPlayerAfterShoot.Add(instantiatedObject);

            // Dibujar una línea hacia el objetivo
            Directorio.Apuntar(instantiatedObject, lastPosition, targetPosition);
        }
        else
        {
            Debug.LogWarning("No se detectó un objetivo válido para disparar.");
        }
    }

    public void Shoot()
    {
        if (movementScript == null || movementScript.GetIsMoving()) return;

        Vector3 targetPosition = GetMouseTargetPosition() ;

        GameObject instantiatedBullet = Instantiate(bullet, transform.position, Quaternion.identity);

        BasicBullet bulletScript = instantiatedBullet.GetComponent<BasicBullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDirection(targetPosition);
        }

        Debug.Log($"Bala disparada hacia {targetPosition}");
    }

    private Vector3 GetMouseTargetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point; // Posición 3D del objeto impactado
        }
        return Vector3.zero; // Si no impacta, retorna Vector3.zero
    }
}
