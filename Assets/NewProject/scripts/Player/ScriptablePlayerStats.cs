using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData")]
public class PlayerData : ScriptableObject
{
    public float health;
    public float maxHealth;
    public float actionPoints;
    public float maxActionPoints;
    public float maxTime;
    public string lastLevel;
    public bool levelCompleted;
    public int timesHealed;
    public int timesIncreasedMaxHP;
    public int baseRange;
    public int exp;
    public bool isAlive;
    public bool victory;

    [NonSerialized] public BulletCollection bulletCollection = new BulletCollection();

    public int healAmount = 10; 
    
    public List<ActionData> availableActions = new List<ActionData>();

    [System.Serializable]
    public class ActionData
    {
        public ActionData(PlayerBase.ActionType _actionType, PlayerBase.ActionEnum _action, KeyCode _key, int _cost, BulletStyle _style = null)
        {
            actionType = _actionType;
            action = _action;
            key = _key;
            style = _style;
            cost = _cost;
        }
        public PlayerBase.ActionType actionType;
        public PlayerBase.ActionEnum action;
        public PlayerBase player;
        public KeyCode key;
        public BulletStyle style;
        public int cost;
    }
}
