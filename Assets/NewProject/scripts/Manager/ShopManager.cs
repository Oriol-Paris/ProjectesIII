using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using static PlayerData;
using System;

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
    public Sprite laserImage;

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
        ActionData actionData = actionPool.Find(action => GetActionDisplayName(action) == itemName);
        int index = buttons.FindIndex(button => button.transform.Find("Item Name").GetComponent<TextMeshProUGUI>().text == itemName);
        if (actionData != null && player.playerData.exp >= pricePool[index])
        {
            bool actionExists = false;
            ActionData repeatAction = null;

            foreach (var playerAction in player.playerData.availableActions)
            {
                if (actionData.action == playerAction.action)
                {
                    if (actionData.action == PlayerBase.ActionEnum.SHOOT)
                    {
                        if (actionData.style.bulletType == playerAction.style.bulletType)
                        {
                            Debug.Log("MISMO ESTILO");
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

    private void EquipNewAction(ActionData actionData)
    {
        if (actionData.actionType != PlayerBase.ActionType.SINGLE_USE)
        {
            actionData.key = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Alpha" + (player.playerData.availableActions.Count + 1));
            player.playerData.availableActions.Add(actionData);
            statIncreaseCount[actionData] = 0;  // Initialize the stat increase count
            Debug.Log("Item equipped");
        }
    }

    private void IncreaseStat(ActionData actionData)
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
                player.playerData.moveRange += 1; // Increase move range
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
        }
    }

    private void InitializeShop()
    {
        ActionData shotgunShot = new ActionData(PlayerBase.ActionType.ACTIVE, PlayerBase.ActionEnum.SHOOT, KeyCode.None, 2, FindAnyObjectByType<BulletCollection>().GetBullet(BulletType.SHOTGUN), BulletType.SHOTGUN);
        ActionData gunShot = new ActionData(PlayerBase.ActionType.ACTIVE, PlayerBase.ActionEnum.SHOOT, KeyCode.None, 1, FindAnyObjectByType<BulletCollection>().GetBullet(BulletType.GUN), BulletType.GUN);
        ActionData laser = new ActionData(PlayerBase.ActionType.ACTIVE, PlayerBase.ActionEnum.SHOOT, KeyCode.None, 2, FindAnyObjectByType<BulletCollection>().GetBullet(BulletType.LASER), BulletType.LASER);
        ActionData heal = new ActionData(PlayerBase.ActionType.PASSIVE, PlayerBase.ActionEnum.HEAL, KeyCode.None, 1, null);
        ActionData move = new ActionData(PlayerBase.ActionType.ACTIVE, PlayerBase.ActionEnum.MOVE, KeyCode.None, 1, null);
        ActionData recovery = new ActionData(PlayerBase.ActionType.SINGLE_USE, PlayerBase.ActionEnum.RECOVERY, KeyCode.None, 1, null);
        ActionData maxHpIncrease = new ActionData(PlayerBase.ActionType.SINGLE_USE, PlayerBase.ActionEnum.MAX_HP_INCREASE, KeyCode.None, 1, null);
        //PlayerData.ActionData speedUp = new PlayerData.ActionData(PlayerBase.ActionType.SINGLE_USE, PlayerBase.ActionEnum.SPEED_UP, KeyCode.None, 1, player.playerData.moveStyle);
        actionPool = new List<ActionData>
        {
            shotgunShot, gunShot, heal, move, recovery, maxHpIncrease, laser
        };

        activeActions = new List<ActionData>(4);
        pricePool = new List<int>(4);
        buttons = new List<GameObject>(4);

        // Randomize actions and prices
        for (int i = 0; i < 4; i++) // Assuming there are only four buttons
        {
            int randomIndex = UnityEngine.Random.Range(0, actionPool.Count);
            ActionData actionData = actionPool[randomIndex];
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

    public string GetActionDisplayName(ActionData actionData)
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
            else if (actionData.style.bulletType == BulletType.LASER)
            {
                return "Laser";
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
        return actionData.action.ToString();
    }

    public Sprite GetActionImage(PlayerData.ActionData actionData)
    {
        if (actionData.action == PlayerBase.ActionEnum.SHOOT)
        {
            if (actionData.style.bulletType == BulletType.GUN)
            {
                return gunImage;
            }
            else if (actionData.style.bulletType == BulletType.SHOTGUN)
            {
                return shotgunImage;
            }
            else if (actionData.style.bulletType == BulletType.LASER)
            {
                return laserImage;
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
        else if (actionData.action == PlayerBase.ActionEnum.MAX_HP_INCREASE)
        {
            return maxHPImage;
        }
        return null;
    }

    int CalculateActionPrice(ActionData action)
    {
        foreach (ActionData playerAction in FindAnyObjectByType<PlayerBase>().playerData.availableActions)
        {
            if (action.action == playerAction.action && BulletCollection.CompareBullets(action.style, playerAction.style))
            {
                if (action.action == PlayerBase.ActionEnum.MOVE)
                    return 10 + (Mathf.FloorToInt(Mathf.Pow(player.playerData.moveRange, 1.25f)));
                else if (action.action == PlayerBase.ActionEnum.HEAL)
                    return 10 + (Mathf.FloorToInt(Mathf.Pow(player.playerData.healAmount, 1.25f)));
                else if (action.action == PlayerBase.ActionEnum.SHOOT && action.style.bulletType == BulletType.GUN)
                    return 15 + (Mathf.FloorToInt(Mathf.Pow(action.style.range, 1.25f)));
                else if (action.action == PlayerBase.ActionEnum.SHOOT && action.style.bulletType == BulletType.SHOTGUN)
                    return 25 + (Mathf.FloorToInt(Mathf.Pow(action.style.range, 1.25f)));
                else if (action.action == PlayerBase.ActionEnum.SHOOT && action.style.bulletType == BulletType.LASER)
                    return 25 + (Mathf.FloorToInt(Mathf.Pow(action.style.range, 1.25f)));
            }
        }

        if (action.action == PlayerBase.ActionEnum.MOVE)
            return 10;
        else if (action.action == PlayerBase.ActionEnum.HEAL)
            return 10;
        else if (action.action == PlayerBase.ActionEnum.SHOOT && action.style.bulletType == BulletType.GUN)
            return 15;
        else if (action.action == PlayerBase.ActionEnum.SHOOT && action.style.bulletType == BulletType.SHOTGUN)
            return 25;
        else if (action.action == PlayerBase.ActionEnum.SHOOT && action.style.bulletType == BulletType.LASER)
            return 25;
        else if (action.action == PlayerBase.ActionEnum.RECOVERY)
            return 10 + 10 * player.playerData.timesHealed;
        else if (action.action == PlayerBase.ActionEnum.MAX_HP_INCREASE)
            return 20 + 20 * player.playerData.timesIncreasedMaxHP;

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
