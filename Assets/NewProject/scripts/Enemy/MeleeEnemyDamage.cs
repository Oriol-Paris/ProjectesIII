using UnityEngine;

public class MeleeEnemyDamage : MonoBehaviour
{
    public int damage;
    public bool hasDealtDamage = false;

    public void OnEnable()
    {
        hasDealtDamage = false; // Reset the flag when the collider is enabled
    }

    private void OnTriggerEnter(Collider other)
    {
        if(this.GetComponentInParent<EnemyBase>().isAlive) {
            if (!hasDealtDamage && other.gameObject.CompareTag("Player"))
            {
                Debug.Log("PIUM");
                other.GetComponent<PlayerBase>().Damage(damage, other.gameObject);
                hasDealtDamage = true; // Set the flag to true after dealing damage
            }
        }
    }
}
