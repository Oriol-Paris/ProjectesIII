using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;


public class TopBarManager : MonoBehaviour
{
    public GridLayoutGroup topPanel;
    public List<GameObject> actions;
    public GameObject costIndicatorPrefab;
    public GameObject topActionPrefab;
    public GameObject bottomActionPrefab;
    private PlayerBase playerData;
    private List<GameObject> actionSlots = new List<GameObject>();
    private List<PlayerBase.Action> actionsDisplayed = new List<PlayerBase.Action>();

    public Sprite runImage;
    public Sprite gunImage;
    public Sprite shotgunImage;
    public Sprite laserImage;
    public Sprite healImage;
    public Sprite weaponPickupImage;

    void Start()
    {
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

        // Check for key presses to trigger animations
        CheckForActionKeyPress();
    }

    private void CheckForActionKeyPress()
    {
    
        for (int i = 0; i < actionSlots.Count; i++)
        {
           
            // Convert the index to the corresponding key (1-9)
            KeyCode key = KeyCode.Alpha1 + i;

            if (Input.GetKeyDown(key))
            {
                TriggerActionAnimation(i);
            }
        }
    }

    private void TriggerActionAnimation(int index)
    {
       
        if (index >= 0 && index < actionSlots.Count)
        {
            Animator animator = actionSlots[index].GetComponent<Animator>();
           
            if (animator != null)
            {
                animator.SetBool("Upgrade", true);
              
                // Optionally reset the animation after a short delay
                StartCoroutine(ResetUpgradeAnimation());
            }
        }
    }

    

    public void AddAction(PlayerBase.ActionEnum action)
    {
        GameObject newAction = Instantiate(topActionPrefab, topPanel.transform);
        newAction.GetComponent<Image>().overrideSprite = GetActionImage(playerData.activeAction);
    }


    public void UpdateBottomHotbar()
    {
        if (playerData == null)
        {
            Debug.LogError("Player is null in HotbarManager.");
            return;
        }

        foreach (var action in actions)
        {
            action.transform.Find("Image").gameObject.SetActive(false);
            action.transform.Find("Key").gameObject.SetActive(false);
            action.transform.Find("Border").GetComponent<Image>().color = Color.white;

            // Limpiar indicadores de coste (por si quedaron residuos)
            Transform costContainer = action.transform.Find("CostContainer");
            if (costContainer != null)
            {
                foreach (Transform child in costContainer)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        var availableActions = playerData.availableActions;
        int actionCount = availableActions.Count;

        for (int i = 0; i < actionCount; i++)
        {
            actions[i].transform.Find("Image").gameObject.SetActive(true);
            actions[i].transform.Find("Key").gameObject.SetActive(true);

            Image actionImage = actions[i].transform.Find("Image").GetComponent<Image>();
            Image actionBorder = actions[i].transform.Find("Border").GetComponent<Image>();

            var action = availableActions[i];
            PlayerBase player = FindAnyObjectByType<PlayerBase>();

            actionImage.overrideSprite = GetActionImage(action);

            // Resaltar si es la acción seleccionada
            if (action._action == player.GetAction()._action)
            {
                if (action._key == player.GetAction()._key)
                {
                    actionBorder.color = Color.yellow;
                }
                else
                {
                    actionBorder.color = Color.white;
                }
            }
            else
            {
                actionBorder.color = Color.white;
            }

            
            actionImage.overrideSprite = GetActionImage(action);
            actionImage.color = GetActionColor(action._action);

            
            Transform costContainer = actions[i].transform.Find("Border");
            
            if (costContainer != null && costIndicatorPrefab != null)
            {
                foreach (Transform child in costContainer)
                    Destroy(child.gameObject);

                float offset = 20f;
                for (int c = 0; c < action._cost; c++)
                {
                    GameObject indicator = Instantiate(costIndicatorPrefab, costContainer);
                    RectTransform rt = indicator.GetComponent<RectTransform>();

                   
                    float x = -(action._cost - 1.5f - c) * offset;
                    rt.anchoredPosition = new Vector2(x, 40);
                }

            }

        }

        actionsDisplayed = new List<PlayerBase.Action>(availableActions);
    }


    public void ResetTopBar()
    {
        foreach (Transform child in topPanel.transform)
        {
            Destroy(child.gameObject);
        }
        TriggerActionAnimation(0);
    }

   


    public void EraseLastAction()
    {
        Destroy(topPanel.transform.GetChild(topPanel.transform.childCount - 1).gameObject);
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

            if (current.action != displayed._action ||
                current.GetType() != displayed.GetType() ||
                current.cost != displayed._cost ||
                !BulletCollection.CompareBullets(current.style, displayed._style))
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
                    if (availableActions[i].action == player.GetAction()._action &&
                        BulletCollection.CompareBullets(availableActions[i].style, player.GetAction()._style))
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
        if (action._action == PlayerBase.ActionEnum.SHOOT)
        {
            if(action._style.bulletType == BulletType.GUN)
            {
                return gunImage;
            }
            else if (action._style.bulletType == BulletType.SHOTGUN)
            {
                return shotgunImage;
            }
            else if (action._style.bulletType == BulletType.LASER)
            {
                return laserImage;
            }
        }
        else if (action._action == PlayerBase.ActionEnum.HEAL)
        {
            return healImage;
        }
        else if (action._action == PlayerBase.ActionEnum.MOVE)
        {
            return runImage;
        }
        else if (action._action == PlayerBase.ActionEnum.ONESHOT)
        {
            return weaponPickupImage;
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