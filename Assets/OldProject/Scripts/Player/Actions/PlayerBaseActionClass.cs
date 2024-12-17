using UnityEngine;

public abstract class PlayerAction : MonoBehaviour
{
    public abstract void Execute(PlayerBase player, Vector3 targetPosition);
    public abstract void Execute(PlayerBase player);
}