using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static PlayerData;

public class HotbarManager : MonoBehaviour
{
    private PlayerActionManager playerActionManager;
    public PlayerBase playerData;
    public GameObject hotbarPanel;
    public GameObject actionSlotPrefab;
    private GameObject slot;
    private List<GameObject> actionSlots = new List<GameObject>();
    private List<PlayerData.ActionData> actionsDisplayed = new List<PlayerData.ActionData>();

    public Animator hotbarAnimator; // Reference to the Animator

    private List<PlayerData.ActionData> previousActions = new List<PlayerData.ActionData>(); // Tracks the previous state of actions

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
        if (HasActionsChanged())
        {
            UpdateHotbar();
        }
    }

    bool HasActionsChanged()
    {
        var currentActions = playerData.playerData.availableActions;

        // Check if the number of actions has changed
        if (currentActions.Count != previousActions.Count)
        {
            return true;
        }

        // Check if any action in the list has changed
        for (int i = 0; i < currentActions.Count; i++)
        {
            if (currentActions[i] != previousActions[i])
            {
                return true;
            }
        }

        return false;
    }

    public void UpdateHotbar()
    {
        if (playerData == null)
        {
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
            slot = Instantiate(actionSlotPrefab, hotbarPanel.transform);
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

                        "Damage: " + FindAnyObjectByType<BulletCollection>().GetBullet(action.bulletType).damage;
                        
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
                        
                        "Damage: " + FindAnyObjectByType<BulletCollection>().GetBullet(action.bulletType).damage;
                }
                else if (action.actionType == PlayerBase.ActionType.SINGLE_USE || action.actionType == PlayerBase.ActionType.PASSIVE)
                {
                    slot.transform.Find("Texts").transform.Find("Action Stats").gameObject.SetActive(false);
                }

                TextMeshProUGUI inputText = slot.transform.Find("Texts").transform.Find("Action Input").GetComponent<TextMeshProUGUI>();
                if (inputText != null)
                {
                    string inputKey = action.key.ToString();

                    if (inputKey.Contains("Alpha"))
                    {
                        inputKey = inputKey[5].ToString();
                    }

                    inputText.text = inputKey;
                }
            }

            actionSlots.Add(slot);
            actionsDisplayed.Add(action);
        }

        // Update the previous actions list
        previousActions = new List<PlayerData.ActionData>(availableActions);
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

    public void TriggerUpgradeAnimation(PlayerData.ActionData actionData)
    {
        int index = actionsDisplayed.FindIndex(action => action == actionData);
        if (index != -1 && index < actionSlots.Count)
        {
            Animator animator = actionSlots[index].GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("Upgrade", true);

                // Optionally reset the boolean after the animation is done
                StartCoroutine(ResetUpgradeAnimation());
            }
        }
    }

    private IEnumerator ResetUpgradeAnimation()
    {
        yield return new WaitForSeconds(.2f);

        for (int i = 0; i < actionSlots.Count; i++)
        {
            Animator animator = actionSlots[i].GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("Upgrade", false);
            }
        }
    }
}