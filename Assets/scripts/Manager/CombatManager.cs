using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    public List<PlayerBase> playerParty = new List<PlayerBase>();  
    public EnemyBase[] enemyParty;  
    public bool allEnemiesDead;
    public bool allPlayersDead;
    [SerializeField] public float enemyStatMultiplier = 1;
    [SerializeField] private int numberOfTurns;
    [SerializeField] Canvas winCondition;
    [SerializeField] TextMeshProUGUI textWinCondition;

    private bool hasCalculatedExp = false;  

    void Start()
    {
        winCondition.enabled = false;
        allEnemiesDead = true;
    }

    void Update()
    {
        allEnemiesDead = true;
        allPlayersDead = true;
        for (int i = 0; i < enemyParty.Length; i++)
        {
            if (enemyParty[i].isAlive)
                allEnemiesDead = false;
        }
        for (int i = 0; i < playerParty.Count; i++)
        {
            if (playerParty[i].GetIsAlive())
                allPlayersDead = false;
        }

        if (allEnemiesDead && !hasCalculatedExp)
        {
            
            winCondition.enabled = true;
            textWinCondition.text = "YOU WIN!";
            Cursor.visible = true;

           
            for (int i = 0; i < playerParty.Count; i++)
            {
                playerParty[i].victory = true;
                if (playerParty[i].GetIsAlive())
                {
                   
                    playerParty[i].playerData.exp += 25;

                   
                    playerParty[i].playerData.exp += (int)playerParty[i].health;

                   
                    int bonusExp = (int)playerParty[i].playerData.exp+10;
                    playerParty[i].playerData.exp += bonusExp;

                    Debug.Log($"Player {i} EXP: {playerParty[i].playerData.exp}");
                }
                
                Debug.Log($"Player {i} EXP after calculation: {playerParty[i].playerData.exp}");
                playerParty[i].SaveCurrentState();
                
            }

            if (SceneManager.GetActiveScene().name == "Level8")
            {
                PlayerPrefs.SetString("LastLevelCleared", "");
                PlayerPrefs.SetString("EneteredMap", "");
                PlayerPrefs.SetFloat("DifficultyMultiplier", PlayerPrefs.GetFloat("DifficultyMultiplier") + 0.5f);
            }
            else
            {
                PlayerPrefs.SetString("LastLevelCleared", SceneManager.GetActiveScene().name);
            }

            hasCalculatedExp = true;
        }

        if (allPlayersDead)
        {
            // Mostrar mensaje de derrota
            winCondition.enabled = true;
            textWinCondition.text = "YOU LOSE";
            Cursor.visible = true;

            for (int i = 0; i < playerParty.Count; i++)
            {
                playerParty[i].defeat = true;
            }

            playerParty[0].ResetToLevelStart();
        }
    }

    public Canvas GetWinCondition() { return winCondition; }
}
