using System;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType { GUN, SHOTGUN, LASER,SNIPER };

[Serializable]
public class BulletStyle : MonoBehaviour
{
    public GameObject prefab;
    public BulletType bulletType;
    public int level = 0;
    public float range = 0;
    public float rangePerLevel = 0;
    public int damage = 0;
    public int damagePerLevel = 0;


    private void Update()
    {

      
        if (prefab != null)
        {
            Debug.Log("hola");
        }
    }

    public void Initiazlize(BulletType type, GameObject loadedPrefab)
    {
        prefab = loadedPrefab;
        bulletType = type;

        switch (type)
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


    public void LevelUpBullet(int currentLevel, int numOfLevels = 1)
    {
        range = rangePerLevel * (currentLevel );

        damage = damagePerLevel * (currentLevel);
        Debug.Log(damage);
    }
}