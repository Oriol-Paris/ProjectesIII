using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HotbarManager : MonoBehaviour
{
    private PlayerActionManager playerActionManager;
    public PlayerBase playerData;
    public GameObject hotbarPanel;
    public GameObject actionSlotPrefab;

    private List<GameObject> actionSlots = new List<GameObject>();
    private List<PlayerData.ActionData> actionsDisplayed = new List<PlayerData.ActionData>();

    void Start()
    {
        if (playerData == null)
        {
            Debug.LogError("Player is null in HotbarManager.");
            return;
        }

        UpdateHotbar();
    }

    void Update()
    {
        UpdateHotbar();
    }

    void UpdateHotbar()
    {
        if (playerData == null)
        {
            //Debug.LogError("Player is null in HotbarManager.");
            return;
        }

        var availableActions = playerData.playerData.availableActions;
        int actionCount = availableActions.Count;
        Vector2 slotSize = CalculateSlotSize(actionCount);

        // Clear existing slots
        foreach (var slot in actionSlots)
        {
            Destroy(slot);
        }
        actionSlots.Clear();
        actionsDisplayed.Clear();

        foreach (var action in availableActions)
        {
            GameObject slot = Instantiate(actionSlotPrefab, hotbarPanel.transform);
            RectTransform slotRectTransform = slot.GetComponent<RectTransform>();
            slotRectTransform.sizeDelta = slotSize;

            if (SceneManager.GetActiveScene().name == "ShopScene")
            {
                slot.transform.Find("Texts").transform.Find("Action Name").GetComponent<TextMeshProUGUI>().text = FindAnyObjectByType<ShopManager>().GetActionDisplayName(action);
                slot.transform.Find("Action Image").GetComponent<Image>().overrideSprite = FindAnyObjectByType<ShopManager>().GetActionImage(action);
                slot.transform.Find("Action Image").GetComponent<Image>().preserveAspect = true;

                if (action.action == PlayerBase.ActionEnum.SHOOT)
                {
                    slot.transform.Find("Texts").transform.Find("Action Stats").GetComponent<TextMeshProUGUI>().text =
                        "Range: " + FindAnyObjectByType<BulletCollection>().GetBullet(action.bulletType).range + 
                        "\nDamage: " + FindAnyObjectByType<BulletCollection>().GetBullet(action.bulletType).damage + 
                        "\nCost: " + action.cost;
                }
                else if (action.actionType == PlayerBase.ActionType.PASSIVE)
                {
                    slot.transform.Find("Texts").transform.Find("Action Stats").GetComponent<TextMeshProUGUI>().text = "Cost: " + action.cost;
                }
                else if (action.actionType == PlayerBase.ActionType.SINGLE_USE)
                {
                    slot.transform.Find("Texts").transform.Find("Action Stats").gameObject.SetActive(false);
                }
            }
            else
            {
                PlayerBase player = FindAnyObjectByType<PlayerBase>();

                if (action.action == player.GetAction().m_action)
                {
                    if (action.style.bulletType != BulletType.GUN && action.style.bulletType != BulletType.SHOTGUN)
                    {
                        slot.GetComponent<Image>().color = Color.yellow;
                    }
                    else if (BulletCollection.CompareBullets(action.style, player.GetAction().m_style))
                    {
                        slot.GetComponent<Image>().color = Color.yellow;
                    }
                    else
                    {
                        slot.GetComponent<Image>().color = Color.white;
                    }
                }

                slot.transform.Find("Texts").transform.Find("Action Name").GetComponent<TextMeshProUGUI>().text = GetActionName(action);

                if (action.actionType == PlayerBase.ActionType.ACTIVE)
                {
                    slot.transform.Find("Texts").transform.Find("Action Stats").GetComponent<TextMeshProUGUI>().text =
                        "Range: " + FindAnyObjectByType<BulletCollection>().GetBullet(action.bulletType).range + 
                        "\nDamage: " + FindAnyObjectByType<BulletCollection>().GetBullet(action.bulletType).damage;
                }
                else if (action.actionType == PlayerBase.ActionType.SINGLE_USE || action.actionType == PlayerBase.ActionType.PASSIVE)
                {
                    slot.transform.Find("Texts").transform.Find("Action Stats").gameObject.SetActive(false);
                }

                TextMeshProUGUI inputText = slot.transform.Find("Texts").transform.Find("Action Input").GetComponent<TextMeshProUGUI>();
                if (inputText != null)
                {
                    string inputKey = action.key.ToString();  // Accede a la clave de la acción, que es el input asignado

                    if(inputKey.Contains("Alpha"))
                    {
                        inputKey = inputKey[5].ToString();
                    }

                    inputText.text = inputKey;  // Muestra el input asignado a la acción
                }
            }

            actionSlots.Add(slot);
            actionsDisplayed.Add(action);
        }
    }

    Vector2 CalculateSlotSize(int actionCount)
    {
        float hotbarWidth = hotbarPanel.GetComponent<RectTransform>().rect.width;
        float slotWidth = hotbarWidth / actionCount;
        float slotHeight = hotbarPanel.GetComponent<RectTransform>().rect.height;
        return new Vector2(slotWidth, slotHeight);
    }

    string GetActionName(PlayerData.ActionData actionData)
    {
        if (actionData.action == PlayerBase.ActionEnum.SHOOT)
        {
            if (actionData.style.bulletType == BulletType.GUN)
            {
                return "Gun";
            }
            else if (actionData.style.bulletType == BulletType.SHOTGUN)
            {
                return "Shotgun";
            }
        }
        else if (actionData.action == PlayerBase.ActionEnum.HEAL)
        {
            return "Heal";
        }
        else if (actionData.action == PlayerBase.ActionEnum.MOVE)
        {
            return "Move";
        }
        return actionData.action.ToString();
    }
}