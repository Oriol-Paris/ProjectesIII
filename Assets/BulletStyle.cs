using System;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType { GUN, SHOTGUN, LASER };

public class BulletStyle : MonoBehaviour
{
    [NonSerialized] public GameObject prefab;
    [NonSerialized] public BulletType bulletType;
    [NonSerialized] public int level = 0;
    [NonSerialized] public float range = 0;
    [NonSerialized] public float rangePerLevel = 0;
    [NonSerialized] public int damage = 0;
    [NonSerialized] public int damagePerLevel = 0;

    public BulletStyle(BulletType type, GameObject loadedPrefab)
    {
        prefab = loadedPrefab;
        bulletType = type;

        switch(type)
        {
            case BulletType.GUN:
                rangePerLevel += 1;
                damagePerLevel += 1;
                break;
            case BulletType.SHOTGUN:
                rangePerLevel += 0.5f;
                damagePerLevel += 2;
                break;
            case BulletType.LASER:
                rangePerLevel += 0;
                damagePerLevel += 1;
                break;
        }
    }

    public void LevelUpBullet(int numOfLevels = 1)
    {
        if(level == 0)
        {
            switch (bulletType)
            {
                case BulletType.GUN:
                    range = 5;
                    damage = 0;
                    break;
                case BulletType.SHOTGUN:
                    range = 3;
                    damage = 1;
                    break;
                case BulletType.LASER:
                    range = -1;
                    damage = 1;
                    break;
            }
        }
        else
        {
            range += rangePerLevel * numOfLevels;
            damage += damagePerLevel * numOfLevels;
            level += numOfLevels;
        }
    }
}