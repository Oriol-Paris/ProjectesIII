using System.Collections.Generic;
using System;
using UnityEngine;

public class BulletCollection : MonoBehaviour
{
    [SerializeField] private BulletType prefabOrderReference;
    [SerializeField] public List<GameObject> prefabs = new List<GameObject>();
    [SerializeField] public int numOfBulletTypes;
    GameObject bulletToInitialize;
    BulletStyle bullet;
    [SerializeField] public PlayerData playerData;

    [SerializeField] public List<BulletStyle> bulletCollection = new List<BulletStyle>();

    private void Awake()
    {
        InitializeBulletCollection();
    }

    private void InitializeBulletCollection()
    {
        bulletCollection.Clear();

        int typesToCreate = Mathf.Min(numOfBulletTypes, prefabs.Count);

        for (int i = 0; i < typesToCreate; i++)
        {
            if (prefabs[i] != null)
            {

                bulletToInitialize.AddComponent<BulletStyle>();
                bullet  = bulletToInitialize.GetComponent<BulletStyle>();
                bullet.Initiazlize((BulletType)i, prefabs[i]);

                bulletCollection.Add(bullet);
                playerData.LevelUpBullet(bullet.bulletType, 0);
            }
        }
    }

    public BulletStyle GetBullet(BulletType type)
    {
        if (bulletCollection.Count == 0)
        {
            InitializeBulletCollection();
        }

        foreach (BulletStyle style in bulletCollection)
        {
            if (style.bulletType == type)
                return style;
        }

        return null;
    }

    public static bool CompareBullets(BulletStyle a, BulletStyle b)
    {
        if (a == null && b == null)
            return true;

        if (a == null || b == null)
            return false;

        return a.bulletType == b.bulletType;
    }

    public static bool CompareBulletsWithType(BulletType type, BulletStyle a, BulletStyle b)
    {
        if (a == null || b == null)
            return false;

        if (a.bulletType == b.bulletType && a.bulletType == type)
            return true;
        return false;
    }
}