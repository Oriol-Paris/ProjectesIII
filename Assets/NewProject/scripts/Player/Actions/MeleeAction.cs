using System.Collections;
using UnityEngine;

public class MeleeAction : ActiveAction
{
    public float damage = 10f;
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.2f; // Duration of the knockback effect

    public override void Execute(PlayerBase player, Vector3 targetPosition)
    {
        player.StartCoroutine(MoveAndStrike(player, targetPosition));
    }

    private IEnumerator MoveAndStrike(PlayerBase player, Vector3 targetPosition)
    {
        bool enemyHit = false;

        // Move towards the target position
        while (Vector3.Distance(player.transform.position, targetPosition) > 0.1f && !enemyHit)
        {
            //player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, player.GetComponent<OG_MovementByMouse>().velocity * Time.deltaTime);
            yield return null;

            // Check for enemies at the current position
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(player.transform.position, 0.5f);
            foreach (var hitCollider in hitColliders)
            {
                EnemyBase enemy = hitCollider.GetComponent<EnemyBase>();
                if (enemy != null)
                {
                    // Deal damage to the enemy
                    enemy.Damage((int)damage);
                    //player.GetComponent<OG_MovementByMouse>().SetPositionDesired(player.transform.position); 
                    // Apply knockback to the enemy
                    Vector3 knockbackDirection = (enemy.transform.position - player.transform.position).normalized;
                    Vector3 knockbackTarget = enemy.transform.position + knockbackDirection * knockbackForce;
                   

                    enemyHit = true;
                    break;
                }
            }
        }

        // Reset the player's action
        player.SetInAction(false);
        player.GetComponent<PlayerActionManager>().ResetFlags();
        //player.GetComponent<PlayerBase>().activeAction = PlayerBase.Action.nothing; // Reset active action
    }

    private IEnumerator ApplyKnockback(EnemyBase enemy, Vector3 startPosition, Vector3 endPosition)
    {
        float elapsedTime = 0f;
        while (elapsedTime < knockbackDuration)
        {
            enemy.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / knockbackDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        enemy.transform.position = endPosition; // Ensure the enemy reaches the final position
    }
}