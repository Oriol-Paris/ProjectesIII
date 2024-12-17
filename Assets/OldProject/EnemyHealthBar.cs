using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    SpriteRenderer healthBar;
    EnemyBase enemyStats;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyStats = GetComponentInParent<EnemyBase>();
        if(enemyStats != null )
        {
            Debug.Log("Stats captured correctly");
        }
        healthBar = GetComponent<SpriteRenderer>();
        healthBar.size = new Vector2(enemyStats.GetHealth(),healthBar.size.y);
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.size = new Vector2(enemyStats.GetHealth(), healthBar.size.y);
    }
}
