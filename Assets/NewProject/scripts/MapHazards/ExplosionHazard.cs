using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class ExplosionHazard : MonoBehaviour
{
    [SerializeField] private int explosionDamage;
    [SerializeField] public float explosionRadius;
    [SerializeField] private ParticleSystem explosionEffects;
    [SerializeField] protected float explosionSpreadDelay;
    private cameraManager camera;


    private void Start()
    {
        camera = FindAnyObjectByType<cameraManager>();


    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider collision)
    {

        if (collision.gameObject.tag == "Bullet")
        {
            Explode(explosionRadius, false, collision.gameObject);
        }
    }

    public static bool IsWithinCircle(Vector3 objectPosition, Vector3 centerPosition, float radius)
    {
        float distanceSquared = (objectPosition - centerPosition).sqrMagnitude;
        float radiusSquared = radius * radius;

        return distanceSquared <= radiusSquared;
    }


    private void Explode(float radius, bool spread, GameObject DamageOrigin)
    {
        StartCoroutine(DelayAction(explosionSpreadDelay, spread, radius, DamageOrigin));
    }

    IEnumerator DelayAction(float delay, bool spread, float radius, GameObject DamageOrigin)
    {
        if (spread)
        {
            yield return new WaitForSeconds(delay);
        }
        explosionEffects.Play();
        StartCoroutine(camera.Shake(0.1f, 0.8f));
        GameObject player = null;
        player = GameObject.FindGameObjectWithTag("Player");

        GameObject[] enemies = null;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        GameObject[] explosives = null;
        explosives = GameObject.FindGameObjectsWithTag("Explosive");

          for (int i = 0; i < enemies.Length; ++i)
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

        for (int i = 0; i < explosives.Length; ++i)
        {


            if (IsWithinCircle(explosives[i].transform.position, this.transform.position, radius) && explosives[i] != this.gameObject && explosives[i] != DamageOrigin)
            {

                explosives[i].GetComponent<ExplosionHazard>().Explode(explosionRadius, true, this.gameObject);

            }

        }
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        
      
    }

   
}
