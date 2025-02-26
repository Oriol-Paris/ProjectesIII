using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ExplosionHazard : MonoBehaviour
{
    [SerializeField] private float explosionDamage;
    [SerializeField] private float explosionRadius;
    private List<Object> objectsInRange;
        

    // Update is called once per frame
    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null && collision.gameObject.tag == "Bullet")
        {

        }
    }

    private void AreaScan(float radius)
    {
        GameObject[] objects;
        objects = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < objects.Length; i++) 
        {
            if (IsWithinCircle(objects[i].transform.position, this.transform.position, radius)) 
            {
                objectsInRange.Add(objects[i]);
            }
        }

    }
    public static bool IsWithinCircle(Vector3 objectPosition, Vector3 centerPosition, float radius)
    {
        float distanceSquared = (objectPosition - centerPosition).sqrMagnitude;
        float radiusSquared = radius * radius;

        return distanceSquared <= radiusSquared;
    }


    private void Explode()
    {
        
    }
}
