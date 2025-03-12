using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ExplosionHazard : MonoBehaviour
{
    [SerializeField] private int explosionDamage;
    [SerializeField] private float explosionRadius;
    [SerializeField] private ParticleSystem explosionEffects;
    [SerializeField] protected float explosionSpreadDelay;


    // Update is called once per frame
    private void OnTriggerEnter(Collider collision)
    {
        
        if (collision.gameObject.tag == "Bullet")
        {
            explosionEffects.Play();
            Explode(explosionRadius, false);
            Debug.LogWarning("Destroy");
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider>().enabled = false;

        }
    }

    public static bool IsWithinCircle(Vector3 objectPosition, Vector3 centerPosition, float radius)
    {
        float distanceSquared = (objectPosition - centerPosition).sqrMagnitude;
        float radiusSquared = radius * radius;
        
        return distanceSquared <= radiusSquared;
    }


    private void Explode(float radius, bool spread)
    {
        if (spread)
        {
            StartCoroutine(DelayAction(explosionSpreadDelay));
        } 
        GameObject player;
        player = GameObject.FindGameObjectWithTag("Player");

        GameObject[] enemies;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        GameObject[] explosives;
        explosives = GameObject.FindGameObjectsWithTag("Explosive");

        for (int i = 0; i < enemies.Length; i++)
        {
            if (IsWithinCircle(enemies[i].transform.position, this.transform.position, radius))
            {
                enemies[i].GetComponent<EnemyBase>().Damage(1, this.gameObject);

            }
        }

        if (IsWithinCircle(player.transform.position, this.transform.position, radius))
        {
            player.GetComponent<PlayerBase>().Damage(1, this.gameObject);

        }
        
        for (int i = 0; i < explosives.Length; i++)
        {
            if (IsWithinCircle(explosives[i].transform.position, this.transform.position, radius))
            {

                explosives[i].GetComponent<ExplosionHazard>().Explode(explosionRadius, true);

            }
        }
    }
    IEnumerator DelayAction(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
    }
}
