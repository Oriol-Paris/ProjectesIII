using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using static PlayerData;
using System;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Button Prefab")]
    [SerializeField] private GameObject buttonPrefab; 
    public Transform buttonContainer;  

    [Header("Localization Lines")]
    public LocalizedString locBoughtDamage;
    public LocalizedString locBoughtHeal;
    public LocalizedString locBoughtMove;
    public LocalizedString locPlayerHealed;
    public LocalizedString locPlayerSpeedUp;
    public LocalizedString locMaxHPUp;
    public LocalizedString locNotEnoughExp;
    public LocalizedString locJustBought;

    [SerializeField] private LocalizedString gunLoc;
    [SerializeField] private LocalizedString shotgunLoc;
    [SerializeField] private LocalizedString laserLoc;
    [SerializeField] private LocalizedString healLoc;
    [SerializeField] private LocalizedString moveLoc;
    [SerializeField] private LocalizedString recoveryLoc;
    [SerializeField] private LocalizedString speedUpLoc;
    [SerializeField] private LocalizedString maxHpUpLoc;

    [SerializeField] private LocalizedString gunUpgradeLoc;
    [SerializeField] private LocalizedString shotgunUpgradeLoc;
    [SerializeField] private LocalizedString laserUpgradeLoc;
    [SerializeField] private LocalizedString healUpgradeLoc;
    [SerializeField] private LocalizedString moveUpgradeLoc;
    [SerializeField] private LocalizedString recoveryUpgradeLoc;
    [SerializeField] private LocalizedString speedUpUpgradeLoc;
    [SerializeField] private LocalizedString maxHpUpUpgradeLoc;

    [Header("Shop Canvas")]
    public Sprite moveImage;
    public Sprite gunImage;
    public Sprite shotgunImage;
    public Sprite healImage;
    public Sprite recoveryImage;
    public Sprite speedUpImage;
    public Sprite restImage;
    public Sprite maxHPImage;
    public Sprite laserImage;
    [SerializeField] public TextMeshProUGUI boughtItem;
    [SerializeField] public TextMeshProUGUI currentXP;
    [SerializeField] private HotbarManager hotbarManager;
    [SerializeField] private AudioSource buyItemSfx;

    [Header("Shop Data Elements")]
    [SerializeField] public PlayerBase player;
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
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            player.playerData.exp += 100;
        }

        currentXP.text = "EXP: " + player.playerData.exp;
    }

    public void BuyItem(TextMeshProUGUI itemText)
    {
        buyItemSfx.Play();
        string itemName = itemText.text;
        ActionData actionData = actionPool.Find(action =>
        {
            var localized = GetActionDisplayName(action);
            var localizedString = localized.GetLocalizedString(); 
            return localizedString == itemName;
        });
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
                    if (player.playerData.availableActions[i].action == repeatAction.action &&
                        player.playerData.availableActions[i].style?.bulletType == repeatAction.style?.bulletType)

                    {
                        player.playerData.exp -= pricePool[index];
                        boughtItem.enabled = true;
                        IncreaseStat(player.playerData.availableActions[i]);
                        
                        
                        hotbarManager.TriggerUpgradeAnimation(repeatAction);

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
                //boughtItem.text = "Just bought: " + itemName;
                SetLocalizedText(locJustBought, boughtItem, "item", itemName);

                
                hotbarManager.TriggerUpgradeAnimation(actionData);
            }

            player.SaveCurrentState();

            
            UpdatePrices();
        }
        else if (player.exp < pricePool[index])
            SetLocalizedText(locNotEnoughExp, boughtItem);
            //boughtItem.text = "Not enough experience";
    }

    private void EquipNewAction(ActionData actionData)
    {
        if (actionData.actionType != PlayerBase.ActionType.SINGLE_USE)
        {
            actionData.key = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Alpha" + (player.playerData.availableActions.Count + 1));
            player.playerData.availableActions.Add(actionData);
            statIncreaseCount[actionData] = 0;  
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
            if (actionData.cost < 3) { actionData.cost += 1; } //Maximum increase of 3 in cost
        }

        switch (actionData.action)
        {
            case PlayerBase.ActionEnum.SHOOT:
                //boughtItem.text = "Increased bullet damage and range";
                SetLocalizedText(locBoughtDamage, boughtItem);
                player.playerData.LevelUpBullet(actionData.bulletType);
                break;
            case PlayerBase.ActionEnum.HEAL:
                // boughtItem.text = "Increased healing amount";
                SetLocalizedText(locBoughtHeal, boughtItem);
                player.playerData.healAmount += 5; // Increase healing value
                break;
            case PlayerBase.ActionEnum.MOVE:
                //boughtItem.text = "Increased move range";
                SetLocalizedText(locBoughtMove, boughtItem);
                player.playerData.moveRange += 1; // Increase move range
                break;
            case PlayerBase.ActionEnum.RECOVERY:
                //boughtItem.text = "Player Healed";
                SetLocalizedText(locPlayerHealed, boughtItem);
                player.InstantHeal(3);
                player.playerData.timesHealed += 1;
                break;
            case PlayerBase.ActionEnum.SPEED_UP:
                //boughtItem.text = "Player Speed Up";
                SetLocalizedText(locPlayerSpeedUp, boughtItem);
                player.playerData.velocity += 1;
                //player speed up
                break;
            case PlayerBase.ActionEnum.MAX_HP_INCREASE:
                //boughtItem.text = "Player MaxHP Up";
                SetLocalizedText(locMaxHPUp, boughtItem);
                player.InstantMaxHPIncrease();
                break;
        }

        // Trigger the hotbar update
        if (hotbarManager != null)
        {
            
            hotbarManager.TriggerUpgradeAnimation(actionData);
            hotbarManager.UpdateHotbar();
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
            SetLocalizedText(GetActionDisplayName(actionData), itemText);

            // Set item description
            TextMeshProUGUI itemDescription = button.transform.Find("Upgrade Text").GetComponent<TextMeshProUGUI>();
            SetLocalizedText(UpgradeText(actionData), itemDescription);

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

    public LocalizedString GetActionDisplayName(ActionData actionData)
    {
        if (actionData.action == PlayerBase.ActionEnum.SHOOT)
        {
            if (actionData.style.bulletType == BulletType.GUN)
            {
                return gunLoc;
            }
            else if (actionData.style.bulletType == BulletType.SHOTGUN)
            {
                return shotgunLoc;
            }
            else if (actionData.style.bulletType == BulletType.LASER)
            {
                return laserLoc;
            }
        }
        else if (actionData.action == PlayerBase.ActionEnum.HEAL)
        {
            return healLoc;
        }
        else if (actionData.action == PlayerBase.ActionEnum.MOVE)
        {
            Debug.Log("afgiewq");
            return moveLoc;
        }
        else if (actionData.action == PlayerBase.ActionEnum.RECOVERY)
        {
            return recoveryLoc;
        }
        else if (actionData.action == PlayerBase.ActionEnum.SPEED_UP)
        {
            return speedUpLoc;
        }
        else if (actionData.action == PlayerBase.ActionEnum.MAX_HP_INCREASE)
        {
            return maxHpUpLoc;
        }
        return null;
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
                    return 10 + (Mathf.FloorToInt(Mathf.Pow(player.playerData.moveRange, 1.5f)));
                else if (action.action == PlayerBase.ActionEnum.HEAL)
                    return 10 + (Mathf.FloorToInt(Mathf.Pow(player.playerData.healAmount, 1.5f)));
                else if (action.action == PlayerBase.ActionEnum.SHOOT)
                {
                    foreach(var bullet in player.playerData.bulletLevels)
                    {
                        if(action.style.bulletType == bullet.bulletType)
                        {
                            if (action.style.bulletType == BulletType.GUN)
                                return 15 + Mathf.FloorToInt(Mathf.Pow(15, bullet.level));
                            else if (action.style.bulletType == BulletType.SHOTGUN)
                                return 25 + Mathf.FloorToInt(Mathf.Pow(15, bullet.level));
                            else if (action.style.bulletType == BulletType.LASER)
                                return 25 + Mathf.FloorToInt(Mathf.Pow(15, bullet.level));
                        }
                    }
                }
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

    public LocalizedString UpgradeText(ActionData actionData)
    {
        
            if (actionData.action == PlayerBase.ActionEnum.SHOOT)
            {
                if (actionData.style.bulletType == BulletType.GUN)
                {
                    return gunUpgradeLoc;
                }
                else if (actionData.style.bulletType == BulletType.SHOTGUN)
                {
                    return shotgunUpgradeLoc;
                }
                else if (actionData.style.bulletType == BulletType.LASER)
                {
                    return laserUpgradeLoc;
                }
            }
            else if (actionData.action == PlayerBase.ActionEnum.HEAL)
            {
                return healUpgradeLoc;
            }
            else if (actionData.action == PlayerBase.ActionEnum.MOVE)
            {
                return moveUpgradeLoc;
            }
            else if (actionData.action == PlayerBase.ActionEnum.RECOVERY)
            {
                return recoveryUpgradeLoc;
            }
            else if (actionData.action == PlayerBase.ActionEnum.SPEED_UP)
            {
                return speedUpUpgradeLoc;
            }
            else if (actionData.action == PlayerBase.ActionEnum.MAX_HP_INCREASE)
            {
                return maxHpUpUpgradeLoc;
            }
        return new LocalizedString { TableReference = "General", TableEntryReference = "DefaultUpgradeText" };
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


    private void SetLocalizedText(LocalizedString locString, TextMeshProUGUI target, string argName = null, object argValue = null)
    {
        if (argName != null)
        {
            locString.Arguments = new[] { new { name = argName, value = argValue } };
        }

        var op = locString.GetLocalizedStringAsync();
        if (op.IsDone)
            target.text = op.Result;
        else
        {
            op.Completed += callback => target.text = callback.Result;
        }
        target.enabled = true;
    }
}
