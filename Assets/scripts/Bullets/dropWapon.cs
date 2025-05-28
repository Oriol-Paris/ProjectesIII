using System.Collections.Generic;
using UnityEngine;

public class dropWapon : MonoBehaviour
{
    [Header("Prefabs de armas a soltar")]
    public List<GameObject> armaADropear;

    [Header("Probabilidad de soltar un arma (en %)")]
    [Range(0f, 100f)]
    public float probabilidadDrop = 99f;

    public void Start()
    {
        
        if (armaADropear == null || armaADropear.Count == 0) return;

        
        float chance = Random.Range(0f, 100f);
        if (chance < probabilidadDrop)
        {
            
            int index = Random.Range(0, armaADropear.Count);
            GameObject arma = armaADropear[index];

            if (arma != null)
            {
                Instantiate(arma, transform.position, Quaternion.identity);
            }
        }
    }
}
