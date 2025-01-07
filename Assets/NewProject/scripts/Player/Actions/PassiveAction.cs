using UnityEngine;

public abstract class PassiveAction : PlayerAction
{
    public override void Execute(PlayerBase player, Vector3 targetPosition)
    {
        // Passive actions do not require a target position, so this method is not used
    }
}