using UnityEngine;
using UnityEngine.Splines;

public class dropWapon : MonoBehaviour
{
    [Header("Prefab del arma a soltar")]
    public GameObject armaADropear;

    [Header("Probabilidad de soltar el arma (en %)")]
    [Range(0f, 100f)]
    public float probabilidadDrop = 99f;

   public void Start()
    {
        if (armaADropear == null) return;


        float chance = Random.Range(0f, 100f);


        if (chance < probabilidadDrop)
        {
            Instantiate(armaADropear, transform.position, Quaternion.identity);
        }





    }
   
}