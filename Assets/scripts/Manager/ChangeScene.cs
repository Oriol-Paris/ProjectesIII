using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI expValue;
    [SerializeField] CombatManager combatManager;
    [SerializeField] PlayerBase player;
   

    public void GoToShop()
    {

        if(PlayerPrefs.GetString("EneteredMap") != null && PlayerPrefs.GetString("EneteredMap") != "")
        {
            if (combatManager.allEnemiesDead)
            {
                SceneManager.LoadScene(PlayerPrefs.GetString("EneteredMap"));
            }
            else
            {
               
                player.playerData.lastLevel = SceneManager.GetActiveScene().name;
                PlayerPrefs.SetString("LastLevelCleared", "");
                PlayerPrefs.SetFloat("DifficultyMultiplier", 1f);
                player.playerData.CopyDataFrom(player.playerData.originalPlayer);
                SceneManager.LoadScene("Title Screen");
            }
        } 
        else
        {
            SceneManager.LoadScene("StarterNodeMap");
        }
    }

    void Update()
    {
        expValue.text = "Exp: "+ combatManager.playerParty[0].exp+"";
    }
}
