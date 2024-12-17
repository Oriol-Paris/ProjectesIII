using UnityEngine;

public class RestAction : ActiveAction
{
    public override void Execute(PlayerBase player, Vector3 newPos)
    {
        player.Rest();
        
        // Set all relevant flags to ensure a turn is executed
        var movementComponent = player.GetComponent<OG_MovementByMouse>();
        var actionManager = player.GetComponent<PlayerActionManager>();

        movementComponent.isMoving = true;
        actionManager.isMoving = true;
        actionManager.isShooting = true;
        actionManager.isHealing = true;
        player.SetInAction(true);

        // End the turn after resting
        actionManager.EndTurn();

        // Trigger enemies' turn
        foreach (EnemyMovementShooter enemy in GameObject.FindObjectsOfType<EnemyMovementShooter>())
        {
            enemy.DecideAction();
        }
    }
}