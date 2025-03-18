using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    public List<PlayerBase> playerParty = new List<PlayerBase>();  // Lista para jugadores
    public EnemyBase[] enemyParty;  // Array para enemigos
    public bool allEnemiesDead;
    public bool allPlayersDead;
    [SerializeField] public float enemyStatMultiplier = 1;
    [SerializeField] private int numberOfTurns;
    [SerializeField] Canvas winCondition;

    [SerializeField] NodeMapData nodeMapData;

    private bool hasCalculatedExp = false;  // Bandera para controlar que la suma de experiencia solo se haga una vez

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
                    playerParty[i].exp += 25;

                    // 2. Sumar la vida del jugador
                    playerParty[i].exp += playerParty[i].health;

                    // 3. Agregar 1 de experiencia extra por cada 20 de experiencia
                    int bonusExp = (int)playerParty[i].exp/20;
                    playerParty[i].exp += bonusExp;
                    Debug.Log($"Player {i} EXP: {bonusExp}");
                }
                playerParty[i].playerData.exp = (int)playerParty[i].exp;
                
            }

            nodeMapData.SetLevelCleared(SceneManager.GetActiveScene().name);
            playerParty[0].SaveCurrentState();

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
