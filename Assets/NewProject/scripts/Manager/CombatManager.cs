using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

    // Update is called once per frame
    void Update()
    {
        allEnemiesDead = true;
        allPlayersDead = true;
        for (int i = 0; i < enemyParty.Length; i++)
        {
            if (enemyParty[i].isAlive)
                allEnemiesDead = false;
        }
        for(int i = 0; i< playerParty.Count; i++)
        {
            if (playerParty[i].GetIsAlive())
                allPlayersDead = false;
        }

        // Si todos los enemigos est�n muertos y a�n no hemos calculado la experiencia
        if (allEnemiesDead && !hasCalculatedExp)
        {
            // Habilitar la condici�n de victoria
            winCondition.enabled = true;
            winCondition.GetComponentInChildren<TextMeshProUGUI>().text = "YOU WIN!";
            Cursor.visible = true;


            // Realizar el c�lculo de la experiencia
            for (int i = 0; i < playerParty.Count; i++)
            {
                playerParty[i].victory = true;
                if (playerParty[i].GetIsAlive())
                {
                    playerParty[i].exp++;  // Sumar una experiencia b�sica
                    int turnsExpDifference = numberOfTurns - playerParty[i].turnsDone.turnsDone;
                    turnsExpDifference = Mathf.Max(0, turnsExpDifference);
                    playerParty[i].exp += turnsExpDifference; // Ajustar por los turnos
                }
                playerParty[i].playerData.exp = playerParty[i].exp;
            }

            nodeMapData.SetLevelCleared(SceneManager.GetActiveScene().name);
            playerParty[0].SaveCurrentState();

            // Marcar que ya se calcul� la experiencia
            hasCalculatedExp = true;
        }
        if (allPlayersDead)
        {
            // Habilitar la condici�n de victoria
            winCondition.enabled = true;
            winCondition.GetComponentInChildren<TextMeshProUGUI>().text = "YOU LOSE";
            Cursor.visible = true;
            for (int i = 0;i< playerParty.Count;i++)
            {
                playerParty[i].defeat = true;
            }

            playerParty[0].ResetToLevelStart();

        }
    }
    public Canvas GetWinCondition() {  return winCondition; }
}
