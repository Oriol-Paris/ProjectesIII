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

        if(SceneManager.GetActiveScene().name == "Level6")
        {
            PlayerPrefs.SetString("LastLevelCleared", "Tutorial");
            PlayerPrefs.SetFloat("DifficultyMultiplier", PlayerPrefs.GetFloat("DifficultyMultiplier") + 0.5f);
        }
        else
        {
            PlayerPrefs.SetString("LastLevelCleared", SceneManager.GetActiveScene().name);
        }

        SceneManager.LoadScene("Node Map 2.0");
    }

    void Start()
    {
        
    }

    void Update()
    {
        expValue.text = "Exp: "+expObtained.playerParty[0].exp+"";
    }
}
