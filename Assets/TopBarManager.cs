using UnityEngine.UI;
using UnityEngine;
using static PlayerData;

public class TopBarManager : MonoBehaviour
{
    public GridLayoutGroup panel;
    public GameObject actionPrefab;

    public Sprite runImage;
    public Sprite gunImage;
    public Sprite healImage;

    // Agrega una acción al panel
    public void AddAction(PlayerBase.ActionEnum action)
    {
        GameObject newAction = Instantiate(actionPrefab, panel.transform);
        newAction.GetComponent<Image>().overrideSprite = GetActionImage(action);
    }

    public void ResetTopBar()
    {
        foreach (Transform child in panel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private Sprite GetActionImage(PlayerBase.ActionEnum action)
    {
        PlayerBase player = FindAnyObjectByType<PlayerBase>();

        if (action == PlayerBase.ActionEnum.SHOOT)
        {
            return gunImage;
        }
        else if (action == PlayerBase.ActionEnum.HEAL)
        {
            return healImage;
        }
        else if (action == PlayerBase.ActionEnum.MOVE)
        {
            return runImage;
        }
        return null;
    }
}