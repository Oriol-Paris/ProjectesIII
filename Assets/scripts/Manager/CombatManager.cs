using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    [Header("Entities List")]
    public List<PlayerBase> playerParty = new List<PlayerBase>();  
    public EnemyBase[] enemyParty;

    [Header("Victory/Defeat Condition")]
    public bool allEnemiesDead;
    public bool allPlayersDead;
    [SerializeField] Canvas winCondition;

    [Header("Combat Stats")]
    [SerializeField] public float enemyStatMultiplier = 1;
    [SerializeField] private int numberOfTurns;
    private bool hasCalculatedExp = false; 

    void Start()
    {
        // Obtener todos los objetos de tipo PlayerBase en la escena
        PlayerBase[] players = GameObject.FindObjectsByType<PlayerBase>(FindObjectsSortMode.None);

        // Agregar los objetos encontrados a la lista playerParty
        foreach (PlayerBase player in players)
        {
            playerParty.Add(player);
        }
        winCondition.enabled = false;

        // Obtener todos los objetos de tipo EnemyBase en la escena
        enemyParty = GameObject.FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
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
            // Habilitar la condición de victoria
            winCondition.enabled = true;
            winCondition.GetComponentInChildren<TextMeshProUGUI>().text = "YOU WIN!";
            Cursor.visible = true;

            // Calcular experiencia
            for (int i = 0; i < playerParty.Count; i++)
            {
                playerParty[i].victory = true;
                if (playerParty[i].GetIsAlive())
                {
                    // 1. Experiencia base de 25
                    playerParty[i].playerData.exp += 25;

                    // 2. Sumar la vida del jugador
                    playerParty[i].playerData.exp += (int)playerParty[i].health;

                    // 3. Agregar 1 de experiencia extra por cada 20 de experiencia
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
            winCondition.GetComponentInChildren<TextMeshProUGUI>().text = "YOU LOSE";
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
