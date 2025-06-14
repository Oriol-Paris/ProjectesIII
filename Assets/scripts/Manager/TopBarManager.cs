using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

//el animator falta consultarlo

[System.Serializable]
public class ImageGroup
{
    public Image image;
    public Image key;
    public Image border;
}

public class TopBarManager : MonoBehaviour
{
    [SerializeField] private PlayerBase playerBase;
    [SerializeField] private ControlLiniarRender controlLiniarRender;

    public GridLayoutGroup topPanel;
    public List<ImageGroup> actions;
    public GameObject costIndicatorPrefab;
    public GameObject topActionPrefab;
    public GameObject bottomActionPrefab;

    [SerializeField] private List<Animator> actionSlotsAnimator = new List<Animator>();
    private List<PlayerBase.Action> actionsDisplayed = new List<PlayerBase.Action>();

    public Sprite runImage;
    public Sprite gunImage;
    public Sprite shotgunImage;
    public Sprite laserImage;
    public Sprite healImage;
    public Sprite weaponPickupImage;

    void Start()
    {
       

        if (playerBase == null)
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

        
        CheckForActionKeyPress();
    }

    private void CheckForActionKeyPress()
    {
    
        for (int i = 0; i < actionSlotsAnimator.Count; i++)
        {
           
           
            KeyCode key = KeyCode.Alpha1 + i;

            if (Input.GetKeyDown(key))
            {
                TriggerActionAnimation(i);
            }
        }
    }

    private void TriggerActionAnimation(int index)
    {
       
        if (index >= 0 && index < actionSlotsAnimator.Count)
        {
            Animator animator = actionSlotsAnimator[index];
           
            if (animator != null)
            {
                animator.SetBool("Upgrade", true);
              
               
                StartCoroutine(ResetUpgradeAnimation());
            }
        }
    }

    

    public void AddAction(PlayerBase.ActionEnum action)
    {
        GameObject newAction = Instantiate(topActionPrefab, topPanel.transform);
        newAction.GetComponent<Image>().overrideSprite = GetActionImage(playerBase.activeAction);
    }


    public void UpdateBottomHotbar()
    {
        if (playerBase == null)
        {
            Debug.LogError("Player is null in HotbarManager.");
            return;
        }

        foreach (var action in actions)
        {
            action.image.gameObject.SetActive(false);
            action.key.gameObject.SetActive(false);
            action.border.color = Color.white;

            
            //Transform costContainer = action.transform.Find("CostContainer");
            //if (costContainer != null)
            //{
            //    foreach (Transform child in costContainer)
            //    {
            //        Destroy(child.gameObject);
            //    }
            //}
        }

        var availableActions = playerBase.availableActions;
        int actionCount = availableActions.Count;

        for (int i = 0; i < actionCount; i++)
        {

            Image actionImage = actions[i].image;
            Image actionKey = actions[i].image;
            Image actionBorder = actions[i].border;

            actionImage.gameObject.SetActive(true);
            actionKey.gameObject.SetActive(true);

            var action = availableActions[i];
            PlayerBase player = FindAnyObjectByType<PlayerBase>();

            actionImage.overrideSprite = GetActionImage(action);

            
            if (action.m_action == player.GetAction().m_action)
            {
                if (action.m_key == player.GetAction().m_key)
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
            actionImage.color = GetActionColor(action.m_action);

            
            Transform costContainer = actions[i].border.transform;
            
            if (costContainer != null && costIndicatorPrefab != null)
            {
                foreach (Transform child in costContainer)
                    Destroy(child.gameObject);

                float offset = 20f;
                for (int c = 0; c < action.m_cost; c++)
                {
                    GameObject indicator = Instantiate(costIndicatorPrefab, costContainer);
                    RectTransform rt = indicator.GetComponent<RectTransform>();

                   
                    float x = -(action.m_cost - 1.5f - c) * offset;
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
        if (playerBase == null || playerBase.playerData == null)
            return false;

        var availableActions = playerBase.playerData.availableActions;

       
        if (availableActions.Count != actionsDisplayed.Count)
            return true;

       
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

       
       
        if (playerBase != null)
        {
            foreach (var slot in actionSlotsAnimator)
            {
                if (slot == null) continue;

                Image slotImage = slot.GetComponent<Image>();
                bool shouldBeHighlighted = false;

                for (int i = 0; i < availableActions.Count; i++)
                {
                    if (availableActions[i].action == playerBase.GetAction().m_action &&
                        BulletCollection.CompareBullets(availableActions[i].style, playerBase.GetAction().m_style))
                    {
                        shouldBeHighlighted = (i == actionSlotsAnimator.IndexOf(slot));
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
        else if (action.m_action == PlayerBase.ActionEnum.ESPECIALSHOOT)
        {
            return weaponPickupImage;
        }
        return null;
    }

  

    private Color GetActionColor(PlayerBase.ActionEnum action)
    {
        

        foreach(var lineColor in controlLiniarRender.lineColors)
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

        for (int i = 0; i < actionSlotsAnimator.Count; i++)
        {
            Animator animator = actionSlotsAnimator[i];
            if (animator != null)
            {
                animator.SetBool("Upgrade", false);
            }
        }
    }
}