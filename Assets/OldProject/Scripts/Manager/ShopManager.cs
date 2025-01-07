using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using static PlayerData;
using UnityEngine.EventSystems;
using System.Xml;
using UnityEditor.Experimental.GraphView;

public class ShopManager : MonoBehaviour
{
    [System.Serializable]
    public class ShopItem
    {
        public ActionData action;
        public Sprite image;
        public int basePrice;
        public string displayName;
    }

    public TextMeshProUGUI rerollText;
    [SerializeField] private GameObject buttonPrefab; // Reference to the button prefab
    public Transform buttonContainer; // Reference to the container where buttons will be instantiated
    [SerializeField] public PlayerBase player;
    [SerializeField] public TextMeshProUGUI boughtItem;
    [SerializeField] public TextMeshProUGUI currentXP;

    public List<ShopItem> items;

    public int rerollPrice;
    bool actionExists;
    private List<ShopItem> activeItems;
    private List<GameObject> buttons;

    private Dictionary<PlayerData.ActionData, int> statIncreaseCount;

    public void Start()
    {
        statIncreaseCount = new Dictionary<PlayerData.ActionData, int>();
        InitializeShop();
        boughtItem.enabled = false;
        currentXP.text = player.playerData.exp + "";
    }

    public void Update()
    {
        currentXP.text = "Current EXP: " + player.playerData.exp + "";
    }

    public ShopItem FindItem(ActionData action)
    {
        foreach(ShopItem item in items)
        {
            if (item.action == action)
                return item;
        }

        return null;
    }

    public void Reroll()
    {
        if (player.playerData.exp >= rerollPrice)
        {
            player.playerData.exp -= rerollPrice;
            rerollPrice++;
            rerollText.text = rerollPrice + " EXP";
            boughtItem.enabled = false;

            // Clear existing buttons and price pool
            foreach (var button in buttons)
            {
                Destroy(button);
            }
            buttons.Clear();

            // Re-initialize the shop with new actions and prices
            InitializeShop();
        }
    }

    public void BuyItem(TextMeshProUGUI itemText)
    {
        string itemName = itemText.text;
        ShopItem itemData = items.Find(item => item.displayName == itemName);
        int index = buttons.FindIndex(button => button.transform.Find("Item Name").GetComponent<TextMeshProUGUI>().text == itemName);
        if (itemData != null && player.playerData.exp >= CalculateActionPrice(items[index].action))
        {
            bool actionExists = false;
            ActionData repeatAction = null;

            foreach (var playerAction in player.playerData.availableActions)
            {
                if(itemData.action.action == playerAction.action)
                {
                    if(itemData.action.action == PlayerBase.ActionEnum.SHOOT)
                    {
                        if(itemData.action.style.prefab == playerAction.style.prefab)
                        {
                            Debug.Log("MISMOESTILO");
                            actionExists = true;
                            repeatAction = playerAction;
                        }
                    }
                    else
                    {
                        actionExists = true;
                        repeatAction = playerAction;
                    }
                }
            }

            if (actionExists)
            {
                for (int i = 0; i < player.playerData.availableActions.Count; ++i)
                {
                    if (player.playerData.availableActions[i] == repeatAction)
                    {
                        player.playerData.exp -= CalculateActionPrice(items[index].action);
                        boughtItem.enabled = true;
                        IncreaseStat(player.playerData.availableActions[i]);
                        UpdatePrices();
                        return;
                    }
                }
            }

            if(itemData.action.actionType == PlayerBase.ActionType.SINGLE_USE)
            {
                IncreaseStat(itemData.action);
            }

            if (!actionExists && index != -1)
            {
                player.playerData.exp -= CalculateActionPrice(items[index].action);

                EquipNewAction(itemData.action);

                boughtItem.enabled = true;
                boughtItem.text = "Just bought: " + itemName;
            }

            // Update prices after the item has been bought
            UpdatePrices();
        }
        else if (player.exp < CalculateActionPrice(items[index].action))
            boughtItem.text = "Not enough experience";
    }

    private void EquipNewAction(ActionData actionData)
    {
        if(actionData.actionType != PlayerBase.ActionType.SINGLE_USE)
        {
            actionData.key = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Alpha" + (player.playerData.availableActions.Count));
            player.playerData.availableActions.Add(actionData);
            statIncreaseCount[actionData] = 0;  // Initialize the stat increase count
        }
    }

    private void IncreaseStat(PlayerData.ActionData actionData)
    {
        if (!statIncreaseCount.ContainsKey(actionData))
            statIncreaseCount[actionData] = 0;

        statIncreaseCount[actionData]++;
        if (statIncreaseCount[actionData] > 5 && (actionData.action == PlayerBase.ActionEnum.SHOOT || actionData.action == PlayerBase.ActionEnum.HEAL))
            actionData.cost += 1;  // Increase the cost of executing the action

        switch (actionData.action)
        {
            case PlayerBase.ActionEnum.SHOOT:
                boughtItem.text = "Increased bullet damage and range";
                actionData.style.damage += 1; // Increase bullet damage
                actionData.style.range += 3; // Increase bullet range
                break;
            case PlayerBase.ActionEnum.HEAL:
                boughtItem.text = "Increased healing amount";
                player.playerData.healAmount += 5; // Increase healing value
                break;
            case PlayerBase.ActionEnum.MOVE:
                boughtItem.text = "Increased move range";
                actionData.style.range += 1;
                player.playerData.baseRange += 1; // Increase move range
                break;
            case PlayerBase.ActionEnum.RECOVERY:
                boughtItem.text = "Player Healed";
                player.InstantHeal(3);
                player.playerData.timesHealed += 1;
                break;
            case PlayerBase.ActionEnum.SPEED_UP:
                boughtItem.text = "Player Speed Up";
                //player speed up
                break;
            case PlayerBase.ActionEnum.MAX_HP_INCREASE:
                boughtItem.text = "Player MaxHP Up";
                player.InstantMaxHPIncrease();
                break;
            case PlayerBase.ActionEnum.MANA_POTION:
                boughtItem.text = "Player Mana Up";
                player.InstantManaIncrease(2);
                break;
        }
    }

    private void InitializeShop()
    {
        activeItems = new List<ShopItem>(4);
        buttons = new List<GameObject>(4);

        // Randomize actions and prices
        for (int i = 0; i < 4; i++) // Assuming there are only four buttons
        {
            int randomIndex = Random.Range(0, items.Count);

            ShopItem itemData = items[randomIndex];
            activeItems.Add(itemData);

            // Instantiate button
            GameObject button = Instantiate(buttonPrefab, buttonContainer);
            buttons.Add(button);

            // Set item text
            TextMeshProUGUI itemText = button.transform.Find("Item Name").GetComponent<TextMeshProUGUI>();
            itemText.text = itemData.displayName;

            //Set item image
            Image itemImage = button.transform.Find("Item Image").GetComponent<Image>();
            itemImage.overrideSprite = items[randomIndex].image;
            itemImage.preserveAspect = true;
            //itemImage.SetNativeSize();

            // Set price text
            int price = CalculateActionPrice(itemData.action);
            TextMeshProUGUI priceText = button.transform.Find("Price").GetComponent<TextMeshProUGUI>();
            priceText.text = price.ToString() + " EXP";

            // Add button click listener
            button.GetComponent<Button>().onClick.AddListener(() => BuyItem(itemText));
        }
    }

    int CalculateActionPrice(ActionData action)
    {
        foreach(ActionData playerAction in FindAnyObjectByType<PlayerBase>().playerData.availableActions)
        {
            if (action.action == playerAction.action && action.style.prefab == playerAction.style.prefab)
            {
                if (action.action == PlayerBase.ActionEnum.MOVE)
                    return 10 + (Mathf.FloorToInt(Mathf.Pow(action.style.range, 1.25f)));
                else if (action.action == PlayerBase.ActionEnum.HEAL)
                    return 10 + (Mathf.FloorToInt(Mathf.Pow(player.playerData.healAmount, 1.25f)));
                else if (action.action == PlayerBase.ActionEnum.SHOOT && action.style.prefab == player.playerData.gun.prefab)
                    return 15 + (Mathf.FloorToInt(Mathf.Pow(action.style.range, 1.25f)));
                else if (action.action == PlayerBase.ActionEnum.SHOOT && action.style.prefab == player.playerData.shotgun.prefab)
                    return 25 + (Mathf.FloorToInt(Mathf.Pow(action.style.range, 1.25f)));
            }
        }

        if (action.action == PlayerBase.ActionEnum.MOVE)
            return 10;
        else if (action.action == PlayerBase.ActionEnum.HEAL)
            return 10;
        else if (action.action == PlayerBase.ActionEnum.SHOOT && action.style.prefab == player.playerData.gun.prefab)
            return 15;
        else if (action.action == PlayerBase.ActionEnum.SHOOT && action.style.prefab == player.playerData.shotgun.prefab)
            return 25;
        else if (action.action == PlayerBase.ActionEnum.RECOVERY)
            return 10 + 10 * player.playerData.timesHealed;
        else if (action.action == PlayerBase.ActionEnum.MAX_HP_INCREASE)
            return 20 + 20 * player.playerData.timesIncreasedMaxHP;
        else if (action.action == PlayerBase.ActionEnum.MANA_POTION)
            return 15 + 15 * player.playerData.timesIncreasedMana;

        return 100000;
    }

    void UpdatePrices()
    {
        for(int i = 0; i < buttons.Count; ++i)
            buttons[i].transform.Find("Price").GetComponent<TextMeshProUGUI>().text = CalculateActionPrice(items[i].action).ToString() + " EXP";
    }
}