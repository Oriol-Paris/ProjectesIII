using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData")]
public class PlayerData : ScriptableObject
{
    [SerializeField] public PlayerData originalPlayer;
    [SerializeField] private PlayerData playerAtStartOfLevel;

    private const string ORIGINAL_PLAYER_PATH = "/OriginalPlayerData.json";
    private const string LEVEL_START_PATH = "/PlayerDataLevelStart.json";

    public float health;
    public float maxHealth;
    public float actionPoints;
    public float maxActionPoints;
    public float maxTime;
    public string lastLevel;
    public bool levelCompleted;
    public int timesHealed;
    public int timesIncreasedMaxHP;
    public int moveRange;
    public int exp;
    public bool isAlive;
    public bool victory;

    public int healAmount = 10;

    public List<SerializableBulletLevel> bulletLevels = new List<SerializableBulletLevel>();
    public List<ActionData> availableActions = new List<ActionData>();

    [Serializable]
    public class SerializableBulletLevel
    {
        public BulletType bulletType;
        public int level;

        public SerializableBulletLevel(BulletType type, int level)
        {
            this.bulletType = type;
            this.level = level;
        }
    }

    [Serializable]
    public class ActionData
    {
        public ActionData(PlayerBase.ActionType _actionType, PlayerBase.ActionEnum _action, KeyCode _key, int _cost, BulletStyle _style = null, BulletType _bulletType = BulletType.GUN)
        {
            actionType = _actionType;
            action = _action;
            key = _key;
            style = _style;
            cost = _cost;
            bulletType = _bulletType;
        }
        public PlayerBase.ActionType actionType;
        public PlayerBase.ActionEnum action;
        [NonSerialized] public PlayerBase player;
        public KeyCode key;
        public BulletStyle style;
        public int cost;
        public BulletType bulletType;
    }

    [Serializable]
    private class PlayerDataWrapper
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
        public int moveRange;
        public int exp;
        public bool isAlive;
        public bool victory;
        public int healAmount;

        [Serializable]
        public class SerializableActionData
        {
            public PlayerBase.ActionType actionType;
            public PlayerBase.ActionEnum action;
            public KeyCode key;
            public int cost;
            public BulletType bulletType;
        }

        // Use a list of serializable bullet levels for saving
        public List<SerializableBulletLevel> bulletLevels = new List<SerializableBulletLevel>();
        public List<SerializableActionData> availableActions = new List<SerializableActionData>();
    }

    public void Save()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
#else
        SaveToDisk();
#endif
    }

    public void SaveToDisk()
    {
        try
        {
            PlayerDataWrapper wrapper = new PlayerDataWrapper
            {
                health = this.health,
                maxHealth = this.maxHealth,
                actionPoints = this.actionPoints,
                maxActionPoints = this.maxActionPoints,
                maxTime = this.maxTime,
                lastLevel = this.lastLevel,
                levelCompleted = this.levelCompleted,
                timesHealed = this.timesHealed,
                timesIncreasedMaxHP = this.timesIncreasedMaxHP,
                moveRange = this.moveRange,
                exp = this.exp,
                isAlive = this.isAlive,
                victory = this.victory,
                healAmount = this.healAmount,
                // Convert bullet levels from availableActions
                bulletLevels = this.bulletLevels,
                availableActions = new List<PlayerDataWrapper.SerializableActionData>()
            };

            // Convert available actions to serializable format
            foreach (var action in availableActions)
            {
                wrapper.availableActions.Add(new PlayerDataWrapper.SerializableActionData
                {
                    actionType = action.actionType,
                    action = action.action,
                    key = action.key,
                    cost = action.cost,
                    bulletType = action.bulletType
                });
            }

            string json = JsonUtility.ToJson(wrapper, true);
            string path = Application.persistentDataPath + "/PlayerData.json";
            File.WriteAllText(path, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving PlayerData to disk: {e.Message}");
        }
    }

    public static PlayerData LoadFromDisk()
    {
        string path = Application.persistentDataPath + "/PlayerData.json";

        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                PlayerDataWrapper wrapper = JsonUtility.FromJson<PlayerDataWrapper>(json);

                PlayerData data = CreateInstance<PlayerData>();

                data.health = wrapper.health;
                data.maxHealth = wrapper.maxHealth;
                data.actionPoints = wrapper.actionPoints;
                data.maxActionPoints = wrapper.maxActionPoints;
                data.maxTime = wrapper.maxTime;
                data.lastLevel = wrapper.lastLevel;
                data.levelCompleted = wrapper.levelCompleted;
                data.timesHealed = wrapper.timesHealed;
                data.timesIncreasedMaxHP = wrapper.timesIncreasedMaxHP;
                data.moveRange = wrapper.moveRange;
                data.exp = wrapper.exp;
                data.isAlive = wrapper.isAlive;
                data.victory = wrapper.victory;
                data.healAmount = wrapper.healAmount;

                data.bulletLevels = wrapper.bulletLevels;
                data.availableActions = new List<ActionData>();
                foreach (var serializedAction in wrapper.availableActions)
                {
                    data.availableActions.Add(new ActionData(
                        serializedAction.actionType,
                        serializedAction.action,
                        serializedAction.key,
                        serializedAction.cost,
                        null,
                        serializedAction.bulletType
                    ));
                }

                foreach (var action in data.availableActions)
                {
                    if (action.action == PlayerBase.ActionEnum.SHOOT)
                    {
                        action.style = FindAnyObjectByType<BulletCollection>().GetBullet(action.bulletType);
                        // Find bullet level for this bullet type from the list
                        var bulletLevel = data.bulletLevels.FirstOrDefault(b => b.bulletType == action.bulletType);
                        if (bulletLevel != null)
                        {
                            action.style.LevelUpBullet(bulletLevel.level);
                        }
                    }
                }

                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading PlayerData: {e.Message}");
                return null;
            }
        }

        return null;
    }

    public void SaveOriginalPlayerIfNotExists()
    {
        string path = Application.persistentDataPath + ORIGINAL_PLAYER_PATH;
        if (!File.Exists(path))
        {
            try
            {
                SaveSpecificData(path);
                Debug.Log("Original player data saved successfully.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving original player data: {e.Message}");
            }
        }
    }

    public void SavePlayerAtLevelStart()
    {
        try
        {
            string path = Application.persistentDataPath + LEVEL_START_PATH;
            SaveSpecificData(path);
            Debug.Log("Level start player data saved successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving level start player data: {e.Message}");
        }
    }

    private void SaveSpecificData(string path)
    {
        PlayerDataWrapper wrapper = new PlayerDataWrapper
        {
            health = this.health,
            maxHealth = this.maxHealth,
            actionPoints = this.actionPoints,
            maxActionPoints = this.maxActionPoints,
            maxTime = this.maxTime,
            lastLevel = this.lastLevel,
            levelCompleted = this.levelCompleted,
            timesHealed = this.timesHealed,
            timesIncreasedMaxHP = this.timesIncreasedMaxHP,
            moveRange = this.moveRange,
            exp = this.exp,
            isAlive = this.isAlive,
            victory = this.victory,
            healAmount = this.healAmount,
            bulletLevels = this.bulletLevels,
            availableActions = new List<PlayerDataWrapper.SerializableActionData>()
        };

        foreach (var action in availableActions)
        {
            wrapper.availableActions.Add(new PlayerDataWrapper.SerializableActionData
            {
                actionType = action.actionType,
                action = action.action,
                key = action.key,
                cost = action.cost,
                bulletType = action.bulletType
            });
        }

        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(path, json);
    }

    public void LoadOriginalPlayer()
    {
        string path = Application.persistentDataPath + ORIGINAL_PLAYER_PATH;
        originalPlayer = LoadSpecificData(path);
    }

    public void LoadPlayerAtLevelStart()
    {
        string path = Application.persistentDataPath + LEVEL_START_PATH;
        playerAtStartOfLevel = LoadSpecificData(path);
    }

    private static PlayerData LoadSpecificData(string path)
    {
        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                PlayerDataWrapper wrapper = JsonUtility.FromJson<PlayerDataWrapper>(json);

                PlayerData data = CreateInstance<PlayerData>();

                data.health = wrapper.health;
                data.maxHealth = wrapper.maxHealth;
                data.actionPoints = wrapper.actionPoints;
                data.maxActionPoints = wrapper.maxActionPoints;
                data.maxTime = wrapper.maxTime;
                data.lastLevel = wrapper.lastLevel;
                data.levelCompleted = wrapper.levelCompleted;
                data.timesHealed = wrapper.timesHealed;
                data.timesIncreasedMaxHP = wrapper.timesIncreasedMaxHP;
                data.moveRange = wrapper.moveRange;
                data.exp = wrapper.exp;
                data.isAlive = wrapper.isAlive;
                data.victory = wrapper.victory;
                data.healAmount = wrapper.healAmount;

                data.bulletLevels = wrapper.bulletLevels;
                data.availableActions = new List<ActionData>();
                foreach (var serializedAction in wrapper.availableActions)
                {
                    data.availableActions.Add(new ActionData(
                        serializedAction.actionType,
                        serializedAction.action,
                        serializedAction.key,
                        serializedAction.cost,
                        null,
                        serializedAction.bulletType
                    ));
                }

                foreach (var action in data.availableActions)
                {
                    if (action.action == PlayerBase.ActionEnum.SHOOT)
                    {
                        action.style = FindAnyObjectByType<BulletCollection>().GetBullet(action.bulletType);
                        var bulletLevel = data.bulletLevels.FirstOrDefault(b => b.bulletType == action.bulletType);
                        if (bulletLevel != null)
                        {
                            action.style.LevelUpBullet(bulletLevel.level);
                        }
                    }
                }

                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading PlayerData from {path}: {e.Message}");
                return null;
            }
        }
        return null;
    }

    public void InitializeAllData()
    {
        SaveOriginalPlayerIfNotExists();
        SavePlayerAtLevelStart();
        LoadPlayerAtLevelStart();
    }

    public void ResetToOriginal()
    {
        if (originalPlayer != null)
        {
            CopyDataFrom(originalPlayer);
        }
        else
        {
            Debug.LogWarning("Original player data not found!");
            LoadOriginalPlayer();
            if (originalPlayer != null)
            {
                CopyDataFrom(originalPlayer);
            }
        }
    }

    public void ResetToLevelStart()
    {
        if (playerAtStartOfLevel != null)
        {
            CopyDataFrom(playerAtStartOfLevel);
        }
        else
        {
            
            LoadPlayerAtLevelStart();
            if (playerAtStartOfLevel != null)
            {
                CopyDataFrom(playerAtStartOfLevel);
            }
        }
    }

    public void CopyDataFrom(PlayerData source)
    {
        health = source.health;
        maxHealth = source.maxHealth;
        actionPoints = source.actionPoints;
        maxActionPoints = source.maxActionPoints;
        maxTime = source.maxTime;
        lastLevel = source.lastLevel;
        levelCompleted = source.levelCompleted;
        timesHealed = source.timesHealed;
        timesIncreasedMaxHP = source.timesIncreasedMaxHP;
        moveRange = source.moveRange;
        exp = source.exp;
        isAlive = source.isAlive;
        victory = source.victory;
        healAmount = source.healAmount;

        availableActions.Clear();
        foreach (var action in source.availableActions)
        {
            availableActions.Add(new ActionData(
                action.actionType,
                action.action,
                action.key,
                action.cost,
                action.style,
                action.bulletType
            ));
        }
    }

    public void LevelUpBullet(BulletType type, int amountOfLevels = 1)
    {
        foreach(var bullet in bulletLevels)
        {
            if (bullet.bulletType == type)
            {
                FindAnyObjectByType<BulletCollection>().GetBullet(bullet.bulletType).LevelUpBullet(bullet.level);
                bullet.level += amountOfLevels;
                return;
            }
        }

        Save();
    }
}