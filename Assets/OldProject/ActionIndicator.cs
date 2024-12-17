using UnityEngine;

public class ActionIndicator : MonoBehaviour
{
    private SpriteRenderer actionIndicator;
    [SerializeField]EnemyMovementShooter enemyMovementShooter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        actionIndicator = GetComponent<SpriteRenderer>();
        enemyMovementShooter = GetComponentInParent<EnemyMovementShooter>();
    }

    // Update is called once per frame
    void Update()
    {
        if(enemyMovementShooter.enemyStats.isAlive) {
            if (!enemyMovementShooter.closestPlayer.isMoving)
            {
                actionIndicator.enabled = true;
                switch (enemyMovementShooter.turnAction)
                {
                    case EnemyMovementShooter.TurnActions.APPROACH:
                        actionIndicator.color = Color.blue;
                        break;

                    case EnemyMovementShooter.TurnActions.SHOOT:
                        actionIndicator.color = Color.red;
                        break;

                    case EnemyMovementShooter.TurnActions.BACK_AWAY:
                        actionIndicator.color = Color.green;
                        break;

                    default:
                        actionIndicator.color = Color.white;
                        break;


                }
            }
            else
            {
                actionIndicator.enabled = false;
            }
        } else { actionIndicator.enabled = false; }

    }
}
