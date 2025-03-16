using System.Collections.Generic;
using System;
using UnityEngine;

public class BulletCollection : MonoBehaviour
{
    [SerializeField] private BulletType prefabOrderReference;
    [SerializeField] public List<GameObject> prefabs = new List<GameObject>();
    [NonSerialized] public int numOfBulletTypes;

    [NonSerialized] public List<BulletStyle> bulletCollection = new List<BulletStyle>();

    public BulletCollection()
    {
        numOfBulletTypes = prefabs.Count;

        for (int i = 0; i < numOfBulletTypes; i++)
        {
            bulletCollection.Add(new BulletStyle((BulletType)i, prefabs[i]));
        }
    }


    public BulletStyle GetBullet(BulletType type)
    {
        foreach (BulletStyle style in bulletCollection)
        {
            if (style.bulletType == type)
                return style;
        }

        return null;
    }

    public static bool CompareBullets(BulletStyle a, BulletStyle b)
    {
        if (a.bulletType == b.bulletType)
            return true;

        return false;
    }

    public static bool CompareBulletsWithType(BulletType type, BulletStyle a, BulletStyle b)
    {
        if (a.bulletType == b.bulletType && a.bulletType == type)
            return true;
        return false;
    }
}
