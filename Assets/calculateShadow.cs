using UnityEngine;

public class calculateShadow : MonoBehaviour
{
    public GameObject shadowPrefab;  // Prefab de la sombra que queremos proyectar
    private float shadowHeight = -0.6f; // Altura sobre el suelo donde aparecerá la sombra (puedes ajustarlo)

  

    void Start()
    {
       
      
    }

    void Update()
    {
        // Calcular la posición de la sombra
        Vector3 spritePosition = transform.position;

        // Proyectar la sombra sobre el plano del suelo (tomando solo las coordenadas x y z)
        Vector3 shadowPosition = new Vector3(spritePosition.x, shadowHeight, spritePosition.z);

        // Actualizar la posición de la sombra
        shadowPrefab.transform.position = shadowPosition;
    }
}