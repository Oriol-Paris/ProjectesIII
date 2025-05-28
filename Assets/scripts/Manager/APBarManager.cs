using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class APBarManager : MonoBehaviour
{
    public GameObject APBarPrefab;
    private PlayerBase player;
    [SerializeField] private List<GameObject> apBars = new List<GameObject>();
    private int currentAP = 0;

    [Header("Sprites")]
    public Sprite runImage;
    public Sprite gunImage;
    public Sprite shotgunImage;
    public Sprite laserImage;
    public Sprite healImage;
    public Sprite weaponPickupImage;


    public Color shootColor;
    public Color healColor;
    public Color moveColor;

    void Start()
    {
        InitBar();
    }

    public void InitBar()
    {
        player = FindAnyObjectByType<PlayerBase>();

        for (int i = 0; i < player.playerData.maxActionPoints; i++)
        {
            GameObject apBar = Instantiate(APBarPrefab, transform);
            apBar.transform.Find("ActionSprite").GetComponent<Image>().sprite = null;
            apBar.transform.Find("ActionSprite").GetComponent<Image>().color = Color.clear;
            apBar.transform.localScale = new Vector3(0.006f, 0.006f, 1f);
            apBars.Add(apBar);
        }

        currentAP = (int)player.playerData.maxActionPoints - 1;
    }

    public void ResetBar()
    {
        foreach (var item in apBars)
        {
            Destroy(item);
        }
        apBars.Clear();
        InitBar();
    }

    public void PaintAPCost()
    {
        var action = player.GetAction();
        int cost = action.m_cost;
        Sprite actionSprite = GetActionImage(action);
        Color actionColor = GetActionColor(action);

        for (int i = 0; i < cost; i++)
        {
            int index = currentAP - i;
            if (index >= 0 && index < apBars.Count)
            {
                Image fill = apBars[index].transform.Find("ActionSprite").GetComponent<Image>();
                Image fillImage = apBars[index].transform.Find("Fill").GetComponent<Image>();

                if (fillImage == null)
                {
                    Debug.LogError("Fill Image not found in AP bar prefab.");
                }
                else
                {
                    fillImage.color = actionColor;
                }

                fill.sprite = actionSprite;
                fill.color = Color.white;
                fill.type = Image.Type.Simple;
                fill.preserveAspect = true;
            }
        }

        currentAP -= cost;
        if (currentAP < -1) currentAP = -1;
    }


    private Sprite GetActionImage(PlayerBase.Action action)
    {
        if (action.m_action == PlayerBase.ActionEnum.SHOOT)
        {
            switch (action.m_style.bulletType)
            {
                case BulletType.GUN: return gunImage;
                case BulletType.SHOTGUN: return shotgunImage;
                case BulletType.LASER: return laserImage;
            }
        }
        else if (action.m_action == PlayerBase.ActionEnum.HEAL)
        {
            return healImage;
        }
        else if (action.m_action == PlayerBase.ActionEnum.MOVE)
        {
            return runImage;
        }
        else if (action.m_action == PlayerBase.ActionEnum.ESPECIALSHOOT)
        {
            return weaponPickupImage;
        }

        return null;
    }
    private Color GetActionColor(PlayerBase.Action action)
    {
        if (action.m_action == PlayerBase.ActionEnum.SHOOT)
        {
            switch (action.m_style.bulletType)
            {
                case BulletType.GUN: return shootColor;
                case BulletType.SHOTGUN: return shootColor;
                case BulletType.LASER: return shootColor;
            }
        }
        else if (action.m_action == PlayerBase.ActionEnum.HEAL)
        {
            return healColor;
        }
        else if (action.m_action == PlayerBase.ActionEnum.MOVE)
        {
            return moveColor;
        }
        else if (action.m_action == PlayerBase.ActionEnum.ESPECIALSHOOT)
        {
            return shootColor;
        }

        return Color.white;
    }

    public void DestroyUsedAP()
    {
        for (int i = (int)player.playerData.maxActionPoints - 1; i > currentAP; i--)
        {
            Destroy(apBars[i]);
            apBars.RemoveAt(i);
        }
    }
}
