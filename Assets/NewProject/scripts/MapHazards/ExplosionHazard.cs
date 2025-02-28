using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ExplosionHazard : MonoBehaviour
{
    [SerializeField] private int explosionDamage;
    [SerializeField] private float explosionRadius;
    private List<Object> objectsInRange;
        

    // Update is called once per frame
    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null && collision.gameObject.tag == "Bullet")
        {
            Debug.LogWarning("Damaged");
            AreaScan(explosionRadius);
            Explode();
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
        for (int i = 0; i < objectsInRange.Count; i++) {
            if(((GameObject)objectsInRange[i]).GetComponent<PlayerBase>() != null)
            {
                ((GameObject)objectsInRange[i]).GetComponent<PlayerBase>().Damage(explosionDamage);
                

            }
            if (((GameObject)objectsInRange[i]).GetComponent<EnemyBase>() != null)
            {
                ((GameObject)objectsInRange[i]).GetComponent<EnemyBase>().Damage(explosionDamage);
            }

        }
    }
}
