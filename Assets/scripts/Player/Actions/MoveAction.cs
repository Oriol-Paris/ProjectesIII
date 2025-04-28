using UnityEngine;

public class MoveAction : ActiveAction
{
    public override void Execute(PlayerBase player, Vector3 targetPosition)
    {
        player.transform.position = targetPosition;
    }
}