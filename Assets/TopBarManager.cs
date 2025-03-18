using UnityEngine.UI;
using UnityEngine;
using static PlayerData;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Mono.Cecil.Cil;

public class TopBarManager : MonoBehaviour
{
    public GridLayoutGroup topPanel;
    public GridLayoutGroup bottomPanel;

    public GameObject topActionPrefab;
    public GameObject bottomActionPrefab;
    public TextMeshProUGUI actionKeyIndex;
    private PlayerBase playerData;
    private int keyIndex = 1;
    private List<GameObject> actionSlots = new List<GameObject>();
    private List<PlayerBase.Action> actionsDisplayed = new List<PlayerBase.Action>();

    public Sprite runImage;
    public Sprite gunImage;
    public Sprite shotgunImage;
    public Sprite laserImage;
    public Sprite healImage;

    void Start()
    {
        actionKeyIndex = bottomActionPrefab.GetComponentInChildren<TextMeshProUGUI>();
        if(actionKeyIndex != null)
        {
            Debug.Log("Found and assigned");
        }
        playerData = FindAnyObjectByType<PlayerBase>();

        if (playerData == null)
        {
            Debug.LogError("Player is null in HotbarManager.");
            return;
        }

        UpdateBottomHotbar();

#if UNITY_EDITOR
        Cursor.visible = true;
#else
        Cursor.visible = false;
#endif
    }

    void Update()
    {
        if (HaveActionsChanged())
        {
            UpdateBottomHotbar();
        }
    }

    public void AddAction(PlayerBase.ActionEnum action)
    {
        GameObject newAction = Instantiate(topActionPrefab, topPanel.transform);
        newAction.GetComponent<Image>().overrideSprite = GetActionImage(playerData.activeAction);
       
            
        
    }
    

    void UpdateBottomHotbar()
    {
        if (playerData == null)
        {
            Debug.LogError("Player is null in HotbarManager.");
            return;
        }

        var availableActions = playerData.availableActions;
        int actionCount = availableActions.Count;
        Vector2 slotSize = CalculateSlotSize(actionCount);

        // Clean up old slots
        foreach (var slot in actionSlots)
        {
            if (slot != null)
                Destroy(slot);
        }

        actionSlots.Clear();
        actionsDisplayed.Clear();

        // Create new slots
        int currentKeyIndex = 1;
        foreach (var action in availableActions)
        {
            GameObject slot = Instantiate(bottomActionPrefab, bottomPanel.transform);

            RectTransform slotRectTransform = slot.GetComponent<RectTransform>();
            slotRectTransform.sizeDelta = slotSize;

            PlayerBase player = FindAnyObjectByType<PlayerBase>();

            if (action.m_action == player.GetAction().m_action)
            {
                if (BulletCollection.CompareBullets(action.m_style, player.GetAction().m_style))
                {
                    slot.GetComponent<Image>().color = Color.yellow;
                }
                else
                {
                    slot.GetComponent<Image>().color = Color.white;
                }
            }

            slot.transform.Find("Action Image").GetComponent<Image>().overrideSprite = GetActionImage(action);
            slot.transform.Find("Action Image").GetComponent<Image>().color = GetActionColor(action.m_action);

            // Set the action key index
            TextMeshProUGUI actionKeyIndexText = slot.GetComponentInChildren<TextMeshProUGUI>();
            if (actionKeyIndexText != null)
            {
                actionKeyIndexText.text = currentKeyIndex.ToString();
            }
            currentKeyIndex++;

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

    private bool HaveActionsChanged()
    {
        if (playerData == null || playerData.playerData == null)
            return false;

        var availableActions = playerData.playerData.availableActions;

        // Check if count has changed
        if (availableActions.Count != actionsDisplayed.Count)
            return true;

        // Check if any action has changed
        for (int i = 0; i < availableActions.Count; i++)
        {
            if (i >= actionsDisplayed.Count) return true;

            var current = availableActions[i];
            var displayed = actionsDisplayed[i];

            if (current.action != displayed.m_action ||
                current.GetType() != displayed.GetType() ||
                current.cost != displayed.m_cost ||
                !BulletCollection.CompareBullets(current.style, displayed.m_style))
            {
                return true;
            }
        }

        // Check if selected action has changed
        PlayerBase player = FindAnyObjectByType<PlayerBase>();
        if (player != null)
        {
            foreach (var slot in actionSlots)
            {
                if (slot == null) continue;

                Image slotImage = slot.GetComponent<Image>();
                bool shouldBeHighlighted = false;

                for (int i = 0; i < availableActions.Count; i++)
                {
                    if (availableActions[i].action == player.GetAction().m_action &&
                        BulletCollection.CompareBullets(availableActions[i].style, player.GetAction().m_style))
                    {
                        shouldBeHighlighted = (i == actionSlots.IndexOf(slot));
                        break;
                    }
                }

                if ((slotImage.color == Color.yellow) != shouldBeHighlighted)
                    return true;
            }
        }

        return false;
    }

    private Sprite GetActionImage(PlayerBase.Action action)
    {
        PlayerBase player = FindAnyObjectByType<PlayerBase>();

        if (action.m_action == PlayerBase.ActionEnum.SHOOT)
        {
            if(action.m_style.bulletType == BulletType.GUN)
            {
                return gunImage;
            }
            else if (action.m_style.bulletType == BulletType.SHOTGUN)
            {
                return shotgunImage;
            }
            else if (action.m_style.bulletType == BulletType.LASER)
            {
                return laserImage;
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
        return null;
    }

    private Sprite GetActionImage(PlayerData.ActionData action)
    {
        if (action.action == PlayerBase.ActionEnum.SHOOT)
        {
            if (action.style.bulletType == BulletType.GUN)
            {
                return gunImage;
            }
            else if (action.style.bulletType == BulletType.SHOTGUN)
            {
                return shotgunImage;
            }
            else if (action.style.bulletType == BulletType.LASER)
            {
                return laserImage;
            }
        }
        else if (action.action == PlayerBase.ActionEnum.HEAL)
        {
            return healImage;
        }
        else if (action.action == PlayerBase.ActionEnum.MOVE)
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