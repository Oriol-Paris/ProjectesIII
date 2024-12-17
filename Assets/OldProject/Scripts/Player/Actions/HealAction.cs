using UnityEngine;

public class HealAction : PassiveAction
{
    [SerializeField] private int healAmount = 10;

    public override void Execute(PlayerBase player)
    {
        player.Heal(healAmount);
        Debug.Log("Player healed by " + healAmount);
    }
}