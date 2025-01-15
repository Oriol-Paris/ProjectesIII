using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using static PlayerData;
using UnityEngine.EventSystems;
using System.Xml;

public class ShopManager : MonoBehaviour
{
    public TextMeshProUGUI rerollText;
    [SerializeField] private GameObject buttonPrefab; // Reference to the button prefab
    public Transform buttonContainer; // Reference to the container where buttons will be instantiated
    [SerializeField] public PlayerBase player;
    [SerializeField] public TextMeshProUGUI boughtItem;
    [SerializeField] public TextMeshProUGUI currentXP;
    public Sprite moveImage;
    public Sprite gunImage;
    public Sprite shotgunImage;
    public Sprite healImage;
    public Sprite recoveryImage;
    public Sprite speedUpImage;
    public Sprite restImage;
    public Sprite maxHPImage;
    public Sprite manaImage;

    public int rerollPrice;
    bool actionExists;
    private List<PlayerData.ActionData> actionPool;
    private List<PlayerData.ActionData> activeActions;
    private List<int> pricePool;
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
            pricePool.Clear();

            // Re-initialize the shop with new actions and prices
            InitializeShop();
        }
    }

    public void BuyItem(TextMeshProUGUI itemText)
    {
        string itemName = itemText.text;
        PlayerData.ActionData actionData = actionPool.Find(action => GetActionDisplayName(action) == itemName);
        int index = buttons.FindIndex(button => button.transform.Find("Item Name").GetComponent<TextMeshProUGUI>().text == itemName);
        if (actionData != null && player.playerData.exp >= pricePool[index])
        {
            bool actionExists = false;
            PlayerData.ActionData repeatAction = null;

            foreach (var playerAction in player.playerData.availableActions)
            {
                if (actionData.action == playerAction.action)
                {
                    if (actionData.action == PlayerBase.ActionEnum.SHOOT)
                    {
                        if (actionData.style.prefab == playerAction.style.prefab)
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
                        player.playerData.exp -= pricePool[index];
                        boughtItem.enabled = true;
                        IncreaseStat(player.playerData.availableActions[i]);
                        UpdatePrices();
                        return;
                    }
                }
            }

            if (actionData.actionType == PlayerBase.ActionType.SINGLE_USE)
            {
                IncreaseStat(actionData);
            }

            if (!actionExists && index != -1)
            {
                player.playerData.exp -= pricePool[index];

                EquipNewAction(actionData);

                boughtItem.enabled = true;
                boughtItem.text = "Just bought: " + itemName;
            }

            // Update prices after the item has been bought
            UpdatePrices();
        }
        else if (player.exp < pricePool[index])
            boughtItem.text = "Not enough experience";
    }

    private void EquipNewAction(PlayerData.ActionData actionData)
    {
        if (actionData.actionType != PlayerBase.ActionType.SINGLE_USE)
        {
            actionData.key = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Alpha" + (player.playerData.availableActions.Count+1));
            player.playerData.availableActions.Add(actionData);
            statIncreaseCount[actionData] = 0;  // Initialize the stat increase count
        }
    }

    private void IncreaseStat(PlayerData.ActionData actionData)
    {
        if (!statIncreaseCount.ContainsKey(actionData))
        {
            statIncreaseCount[actionData] = 0;
        }

        statIncreaseCount[actionData]++;
        if (statIncreaseCount[actionData] > 5 && (actionData.action == PlayerBase.ActionEnum.SHOOT || actionData.action == PlayerBase.ActionEnum.HEAL))
        {
            actionData.cost += 1;  // Increase the cost of executing the action
        }

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
                boughtItem.text = "Player Time Up";
                player.InstantManaIncrease();
                break;
        }
    }

    private void InitializeShop()
    {
        PlayerData.ActionData shotgunShot = new PlayerData.ActionData(PlayerBase.ActionType.ACTIVE, PlayerBase.ActionEnum.SHOOT, KeyCode.None, 1, player.playerData.shotgun);
        PlayerData.ActionData gunShot = new PlayerData.ActionData(PlayerBase.ActionType.ACTIVE, PlayerBase.ActionEnum.SHOOT, KeyCode.None, 1, player.playerData.gun);
        PlayerData.ActionData heal = new PlayerData.ActionData(PlayerBase.ActionType.PASSIVE, PlayerBase.ActionEnum.HEAL, KeyCode.None, 1, player.playerData.healStyle);
        PlayerData.ActionData move = new PlayerData.ActionData(PlayerBase.ActionType.ACTIVE, PlayerBase.ActionEnum.MOVE, KeyCode.None, 1, player.playerData.moveStyle);
        PlayerData.ActionData recovery = new PlayerData.ActionData(PlayerBase.ActionType.SINGLE_USE, PlayerBase.ActionEnum.RECOVERY, KeyCode.None, 1, player.playerData.moveStyle);
        PlayerData.ActionData maxHpIncrease = new PlayerData.ActionData(PlayerBase.ActionType.SINGLE_USE, PlayerBase.ActionEnum.MAX_HP_INCREASE, KeyCode.None, 1, player.playerData.moveStyle);
        PlayerData.ActionData manaPotion = new PlayerData.ActionData(PlayerBase.ActionType.SINGLE_USE, PlayerBase.ActionEnum.MANA_POTION, KeyCode.None, 1, player.playerData.moveStyle);
        //PlayerData.ActionData speedUp = new PlayerData.ActionData(PlayerBase.ActionType.SINGLE_USE, PlayerBase.ActionEnum.SPEED_UP, KeyCode.None, 1, player.playerData.moveStyle);
        actionPool = new List<PlayerData.ActionData>
        {
            shotgunShot, gunShot, heal, move, recovery, maxHpIncrease, manaPotion
        };

        activeActions = new List<ActionData>(4);
        pricePool = new List<int>(4);
        buttons = new List<GameObject>(4);

        // Randomize actions and prices
        for (int i = 0; i < 4; i++) // Assuming there are only four buttons
        {
            int randomIndex = Random.Range(0, actionPool.Count);
            PlayerData.ActionData actionData = actionPool[randomIndex];
            activeActions.Add(actionData);

            // Instantiate button
            GameObject button = Instantiate(buttonPrefab, buttonContainer);
            buttons.Add(button);

            // Set item text
            TextMeshProUGUI itemText = button.transform.Find("Item Name").GetComponent<TextMeshProUGUI>();
            itemText.text = GetActionDisplayName(actionData);

            //Set item image
            Image itemImage = button.transform.Find("Item Image").GetComponent<Image>();
            itemImage.overrideSprite = GetActionImage(actionData);
            itemImage.preserveAspect = true;
            //itemImage.SetNativeSize();

            // Set price text
            int price = CalculateActionPrice(actionData);
            pricePool.Add(price);
            TextMeshProUGUI priceText = button.transform.Find("Price").GetComponent<TextMeshProUGUI>();
            priceText.text = price.ToString() + " EXP";

            // Add button click listener
            button.GetComponent<Button>().onClick.AddListener(() => BuyItem(itemText));
        }
    }

    public string GetActionDisplayName(PlayerData.ActionData actionData)
    {
        if (actionData.action == PlayerBase.ActionEnum.SHOOT)
        {
            if (actionData.style.prefab == player.playerData.gun.prefab)
            {
                return "Gun";
            }
            else if (actionData.style.prefab == player.playerData.shotgun.prefab)
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
        else if (actionData.action == PlayerBase.ActionEnum.RECOVERY)
        {
            return "Instant HP Recovery";
        }
        else if (actionData.action == PlayerBase.ActionEnum.SPEED_UP)
        {
            return "Speed Up";
        }
        else if (actionData.action == PlayerBase.ActionEnum.MAX_HP_INCREASE)
        {
            return "Max HP Up";
        }
        else if (actionData.action == PlayerBase.ActionEnum.MANA_POTION)
        {
            return "Time Points Up";
        }
        return actionData.action.ToString();
    }

    public Sprite GetActionImage(PlayerData.ActionData actionData)
    {
        if (actionData.action == PlayerBase.ActionEnum.SHOOT)
        {
            if (actionData.style.prefab == player.playerData.gun.prefab)
            {
                return gunImage;
            }
            else if (actionData.style.prefab == player.playerData.shotgun.prefab)
            {
                return shotgunImage;
            }
        }
        else if (actionData.action == PlayerBase.ActionEnum.HEAL)
        {
            return healImage;
        }
        else if (actionData.action == PlayerBase.ActionEnum.MOVE)
        {
            return moveImage;
        }
        else if (actionData.action == PlayerBase.ActionEnum.RECOVERY)
        {
            return recoveryImage;
        }
        else if (actionData.action == PlayerBase.ActionEnum.SPEED_UP)
        {
            return speedUpImage;
        }
        else if (actionData.action == PlayerBase.ActionEnum.REST)
        {
            return restImage;
        }
        else if (actionData.action == PlayerBase.ActionEnum.MAX_HP_INCREASE)
        {
            return maxHPImage;
        }
        else if (actionData.action == PlayerBase.ActionEnum.MANA_POTION)
        {
            return manaImage;
        }
        return null;
    }

    int CalculateActionPrice(ActionData action)
    {
        foreach (ActionData playerAction in FindAnyObjectByType<PlayerBase>().playerData.availableActions)
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
        pricePool.Clear();

        for (int i = 0; i < buttons.Count; ++i)
        {
            int price = CalculateActionPrice(activeActions[i]);
            pricePool.Add(price);
            buttons[i].transform.Find("Price").GetComponent<TextMeshProUGUI>().text = price.ToString() + " EXP";
        }
    }
}
