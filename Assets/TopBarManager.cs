using UnityEngine.UI;
using UnityEngine;
using static PlayerData;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class TopBarManager : MonoBehaviour
{
    public GridLayoutGroup topPanel;
    public GridLayoutGroup bottomPanel;

    public GameObject topActionPrefab;
    public GameObject bottomActionPrefab;

    private PlayerActionManager playerActionManager;
    private PlayerBase playerData;

    private List<GameObject> actionSlots = new List<GameObject>();
    private List<ActionData> actionsDisplayed = new List<ActionData>();

    public Sprite runImage;
    public Sprite gunImage;
    public Sprite healImage;

    void Start()
    {
        playerData = FindAnyObjectByType<PlayerBase>();
        playerActionManager = playerData.GetComponent<PlayerActionManager>();

        if (playerData == null)
        {
            Debug.LogError("Player is null in HotbarManager.");
            return;
        }

        UpdateBottomHotbar();

        Cursor.visible = false;
    }

    void Update()
    {
        UpdateBottomHotbar();
    }

    public void AddAction(PlayerBase.ActionEnum action)
    {
        GameObject newAction = Instantiate(topActionPrefab, topPanel.transform);
        newAction.GetComponent<Image>().overrideSprite = GetActionImage(action);
    }

    void UpdateBottomHotbar()
    {
        if (playerData == null)
        {
            Debug.LogError("Player is null in HotbarManager.");
            return;
        }

        var availableActions = playerData.playerData.availableActions;
        int actionCount = availableActions.Count;
        Vector2 slotSize = CalculateSlotSize(actionCount);

        foreach (var slot in actionSlots)
        {
            Destroy(slot);
        }

        actionSlots.Clear();
        actionsDisplayed.Clear();

        foreach (var action in availableActions)
        {
            GameObject slot = Instantiate(bottomActionPrefab, bottomPanel.transform);
            RectTransform slotRectTransform = slot.GetComponent<RectTransform>();
            slotRectTransform.sizeDelta = slotSize;

            PlayerBase player = FindAnyObjectByType<PlayerBase>();

            if (action.action == player.GetAction().m_action)
            {
                if (action.style.prefab != player.playerData.gun.prefab && action.style.prefab != player.playerData.shotgun.prefab)
                {
                    slot.GetComponent<Image>().color = Color.yellow;
                }
                else if ((action.style.prefab == player.playerData.gun.prefab && player.GetAction().m_style.prefab == player.playerData.gun.prefab)
                    || (action.style.prefab == player.playerData.shotgun.prefab && player.GetAction().m_style.prefab == player.playerData.shotgun.prefab))
                {
                    slot.GetComponent<Image>().color = Color.yellow;
                }
                else
                {
                    slot.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
                }
            }

            slot.transform.Find("Action Image").GetComponent<Image>().overrideSprite = GetActionImage(action.action);
            slot.transform.Find("Action Image").GetComponent<Image>().color = GetActionColor(action.action);

            actionSlots.Add(slot);
            actionsDisplayed.Add(action);
        }
    }

    public void ResetTopBar()
    {
        foreach (Transform child in topPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    Vector2 CalculateSlotSize(int actionCount)
    {
        float hotbarWidth = bottomPanel.GetComponent<RectTransform>().rect.width;
        float slotWidth = hotbarWidth / actionCount;
        float slotHeight = bottomPanel.GetComponent<RectTransform>().rect.height;
        return new Vector2(slotWidth, slotHeight);
    }

    private Sprite GetActionImage(PlayerBase.ActionEnum action)
    {
        PlayerBase player = FindAnyObjectByType<PlayerBase>();

        if (action == PlayerBase.ActionEnum.SHOOT)
        {
            return gunImage;
        }
        else if (action == PlayerBase.ActionEnum.HEAL)
        {
            return healImage;
        }
        else if (action == PlayerBase.ActionEnum.MOVE)
        {
            return runImage;
        }
        return null;
    }

    private Color GetActionColor(PlayerBase.ActionEnum action)
    {
        ControlLiniarRender clr = FindAnyObjectByType<ControlLiniarRender>();

        foreach(var lineColor in clr.lineColors)
        {
            if (lineColor.m_action == action)
            {
                return lineColor.m_color;
            }
        }

        return Color.white;
    }
}