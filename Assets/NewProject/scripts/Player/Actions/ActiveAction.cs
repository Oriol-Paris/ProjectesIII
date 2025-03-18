using UnityEngine;

public abstract class ActiveAction : PlayerAction
{
    public override void Execute(PlayerBase player)
    {
        // Active actions require a target position, so this method is not used
    }
}