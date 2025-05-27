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

        if(PlayerPrefs.GetString("EneteredMap") != null || PlayerPrefs.GetString("EneteredMap") != "")
        if (FindAnyObjectByType<CombatManager>().allEnemiesDead)
        {
            SceneManager.LoadScene(PlayerPrefs.GetString("EneteredMap"));
        } else
        {
            PlayerBase player = FindAnyObjectByType<PlayerBase>();
            player.playerData.lastLevel = SceneManager.GetActiveScene().name;
            PlayerPrefs.SetString("LastLevelCleared", "");
            player.playerData.CopyDataFrom(player.playerData.originalPlayer);
            SceneManager.LoadScene("Title Screen");
        }
       
        SceneManager.LoadScene("StarterNodeMap");
    }

    void Update()
    {
        expValue.text = "Exp: "+expObtained.playerParty[0].exp+"";
    }
}
