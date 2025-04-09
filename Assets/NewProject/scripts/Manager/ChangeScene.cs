using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI expValue;
    [SerializeField] CombatManager expObtained;

    public void GoToShop()
    {
        FindAnyObjectByType<PlayerBase>().playerData.lastLevel = SceneManager.GetActiveScene().name;
        FindAnyObjectByType<PlayerBase>().playerData.levelCompleted = FindAnyObjectByType<CombatManager>().allEnemiesDead;
        SceneManager.LoadScene("Node Map");
    }

    void Start()
    {
        
    }

    void Update()
    {
        expValue.text = "Exp: "+expObtained.playerParty[0].exp+"";
    }
}
