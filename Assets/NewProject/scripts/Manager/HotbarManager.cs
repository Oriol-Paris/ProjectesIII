using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HotbarManager : MonoBehaviour
{
    public PlayerActionManager playerActionManager;
    public PlayerBase playerData;
    public GameObject hotbarPanel;
    public GameObject actionSlotPrefab;

    private List<GameObject> actionSlots = new List<GameObject>();
    private List<PlayerData.ActionData> actionsDisplayed = new List<PlayerData.ActionData>();

    void Start()
    {
        InitializeHotbar();
    }

    void InitializeHotbar()
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
            Debug.LogError("Player is null in HotbarManager.");
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

                if (action.actionType == PlayerBase.ActionType.ACTIVE)
                {
                    slot.transform.Find("Texts").transform.Find("Action Stats").GetComponent<TextMeshProUGUI>().text =
                        "Range: " + action.style.range + "\nDamage: " + action.style.damage + "\nCost: " + action.cost;
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
                if(action.action == FindAnyObjectByType<PlayerBase>().GetAction().m_action)
                {
                    if(action.style.prefab == null)
                    {
                        slot.GetComponent<Image>().color = Color.yellow;
                    }
                    else if(action.style.prefab == FindAnyObjectByType<PlayerBase>().GetAction().m_style.prefab)
                    {
                        slot.GetComponent<Image>().color = Color.yellow;
                    }
                    else
                    {
                        slot.GetComponent<Image>().color = Color.white;
                    }
                }

                slot.transform.Find("Action Image").GetComponent<Image>().enabled = false;
                slot.transform.Find("Texts").transform.Find("Action Name").GetComponent<TextMeshProUGUI>().text = GetActionName(action);
                slot.transform.Find("Texts").transform.Find("Action Name").position =
                    new Vector3(slot.transform.Find("Texts").transform.position.x + 70,
                        slot.transform.Find("Texts").transform.Find("Action Name").position.y,
                        slot.transform.Find("Texts").transform.Find("Action Name").position.z);

                if (action.actionType == PlayerBase.ActionType.ACTIVE)
                {
                    slot.transform.Find("Texts").transform.Find("Action Stats").GetComponent<TextMeshProUGUI>().text =
                        "Range: " + action.style.range + "\nDamage: " + action.style.damage + "\nCost: " + action.cost;
                    slot.transform.Find("Texts").transform.Find("Action Stats").position =
                        new Vector3(slot.transform.Find("Texts").transform.position.x + 70,
                        slot.transform.Find("Texts").transform.Find("Action Stats").position.y,
                        slot.transform.Find("Texts").transform.Find("Action Stats").position.z);
                }
                else if (action.actionType == PlayerBase.ActionType.PASSIVE)
                {
                    slot.transform.Find("Texts").transform.Find("Action Stats").GetComponent<TextMeshProUGUI>().text = "Cost: " + action.cost;
                    slot.transform.Find("Texts").transform.Find("Action Stats").position =
                        new Vector3(slot.transform.Find("Texts").transform.position.x + 70,
                        slot.transform.Find("Texts").transform.Find("Action Stats").position.y,
                        slot.transform.Find("Texts").transform.Find("Action Stats").position.z);
                }
                else if (action.actionType == PlayerBase.ActionType.SINGLE_USE)
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
            if (actionData.style.prefab == FindAnyObjectByType<PlayerBase>().playerData.gun.prefab)
            {
                return "Gun";
            }
            else if (actionData.style.prefab == FindAnyObjectByType<PlayerBase>().playerData.shotgun.prefab)
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