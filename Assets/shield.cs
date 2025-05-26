using UnityEngine;

public class shield : MonoBehaviour
{
    public Transform player;             
    public Transform childObject;         
    [Range(0.1f, 20f)]
    public float rotationSpeed = 2f;      

    void Update()
    {
        if (player == null || childObject == null)
            return;

        // Dirección hacia el jugador (sin cambiar altura)
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0;

        if (directionToPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        // El hijo siempre apunta al jugador (inmediato)
        childObject.LookAt(player.position);
    }
}
