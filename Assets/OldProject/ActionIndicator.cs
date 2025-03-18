using UnityEngine;

public class ActionIndicator : MonoBehaviour
{
    private SpriteRenderer currentActionIndicator;
    [SerializeField] private Sprite moveIndicator;
    [SerializeField] private Sprite fleeindicator;
    [SerializeField] private Sprite shootIndicator;
    [SerializeField] private Sprite waitindicator;
    [SerializeField]EnemyMovementShooter enemyMovementShooter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentActionIndicator = GetComponent<SpriteRenderer>();    
        enemyMovementShooter = GetComponentInParent<EnemyMovementShooter>();
    }

    // Update is called once per frame
    void Update()
    {
        if(enemyMovementShooter.enemyStats.isAlive) {
            
                currentActionIndicator.enabled = true;
                switch (enemyMovementShooter.turnAction)
                {
                    case EnemyMovementShooter.TurnActions.APPROACH:
                        //Debug.Log("miau");
                        currentActionIndicator.sprite = moveIndicator;

                        break;

                    case EnemyMovementShooter.TurnActions.SHOOT:
                        currentActionIndicator.sprite = shootIndicator;
                        break;

                    case EnemyMovementShooter.TurnActions.BACK_AWAY:
                        currentActionIndicator.sprite = fleeindicator;
                        break;

                    default:
                        currentActionIndicator.sprite = waitindicator;
                        break;


                
            }
           
        } else { currentActionIndicator.enabled = false; }

    }
}
