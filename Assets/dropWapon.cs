using UnityEngine;

public class dropWapon : MonoBehaviour
{
    [Header("Prefab del arma a soltar")]
    public GameObject armaADropear;

    [Header("Probabilidad de soltar el arma (en %)")]
    [Range(0f, 100f)]
    public float probabilidadDrop = 99f;

    void OnDestroy()
    {
        // Evita errores si no hay arma asignada
        if (armaADropear == null) return;

        // Generamos número aleatorio entre 0 y 100
        float chance = Random.Range(0f, 100f);

        // Si el número es menor que la probabilidad, dropea
        if (chance < probabilidadDrop)
        {
            Instantiate(armaADropear, transform.position, Quaternion.identity);    
        }
        
    }
}